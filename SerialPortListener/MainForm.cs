using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SerialPortListener.Serial;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Configuration;

// pacchetti aggiuntivi NuGet
using QRCoder;
using Spire.Barcode;

namespace SerialPortListener
{
    public partial class MainForm : Form
    {
        SerialPortManager _spManager;
        public object InputSimulator { get; private set; }
        public object ConnectionStringSettings { get; private set; }
        public bool modalitaStandard; // variabile che memorizza se sono in modalità standard o manuale

        #region GESTIONE GENERALE DELLA FORM (RENDERING, CARICAMENTO, USCITA)
        public MainForm()
        {
            InitializeComponent();
            UserInitialization();
            Load += Form1_Shown;
        }

        private void Form1_Shown(Object sender, EventArgs e)
        {
            this.Text = "appPeso \\\\ registra pesate da bilancia \\\\ " +Environment.UserName; // titolo della form
            linkBrowser.LinkBehavior = LinkBehavior.NeverUnderline;
            linkCartella.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabelErick.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabelEncrypt.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabelDecrypt.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabelCambiaPostazione.LinkBehavior = LinkBehavior.NeverUnderline;
            lblPostazione.Text = "Postazione: " + ConfigurationManager.AppSettings["stazionePC"];
            lblUtente.Text = "User: " + Environment.MachineName+"\\"+ Environment.UserName;
            lblVersioneSW.Text = String.Format("versione SW: {0}", Application.ProductVersion);

            // label DATABASE
            if (ConfigurationManager.AppSettings["schemaDB"] == "CRPDTA.")
            {
                lblDatabase.Text = "Database: TEST";
                lblDatabase.ForeColor = Color.Red;
            }
            else if (ConfigurationManager.AppSettings["schemaDB"] == "PRODDTA.")
                lblDatabase.Text = "Database: PRODUZIONE";
            else
            {
                lblDatabase.Text = "NESSUN DATABASE SETTATO!";
                lblDatabase.ForeColor = Color.Red;
            }

            // controllo se viaggio in modalità normale (OFF) o in simulazione (ON)
            if (ConfigurationManager.AppSettings["simulazione"] == "OFF")
            {
                btnStart.PerformClick();
                lblSimulazione.Text = "Modalità simulazione: OFF";
                modalitaStandard = true; // vado normale/standard dalla seriale
                tbLordo.ReadOnly = true;
                tbTara.ReadOnly = true;
                tbNetto.ReadOnly = true;
                btnPesataManuale.Enabled = false;
                btnSvuota.Enabled = false;
            }
            else
            {
                btnStop.PerformClick();
                groupBoxCOMSettings.Enabled = false;
                lblSimulazione.ForeColor = Color.Red;
                lblSimulazione.Text = "Modalità simulazione: ON";
                modalitaStandard = false; // vado in manuale senza seriale
                tbLordo.ReadOnly = false;
                tbLordo.MaxLength = 4;
                tbNetto.ReadOnly = false;
                tbNetto.MaxLength = 4;
                btnPesataManuale.Enabled = true;
                btnSvuota.Enabled = true;
                btnStart.Enabled = false;
                btnStop.Enabled = false;
            }
        }

        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        // se chiudo l'applicativo dalla classica icona X, non chiude ma minimizza
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }
        #endregion

        #region GESTIONE EVENTO LETTURA DA PORTA SERIALE
        // funzione che gestisce cosa fare alla ricezione dell'evento pesata sulla seriale
        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            // svuoto l'area di testo
            string str = "";

            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            str = Encoding.ASCII.GetString(e.Data);

            int scritturaSuDB = ScriviSuDatabase(str, modalitaStandard);

            _spManager.StopListening();
            _spManager.Dispose();
            _spManager.StartListening();

            if (scritturaSuDB == 1)
            {
                lblErrore.Visible = false;
                btnRefreshJDE.PerformClick();
            }
            else if (scritturaSuDB == -1)
            {
                this.WindowState = FormWindowState.Normal;
                this.TopMost = true;
                lblErrore.Visible = true;
                lblErrore.Text = "PESATA\nNON VALIDA";
                SvuotaEtichetteForm();
                webBrowserArticolo.DocumentText = "";//Properties.Resources.semafororosso;
            }
            else if (scritturaSuDB == -2)
            {
                this.WindowState = FormWindowState.Normal;
                this.TopMost = true;
                lblErrore.Visible = true;
                lblErrore.Text = "SEMAFORO\nNON ATTIVO!\nCONTROLLARE\nJD EDWARDS";
                SvuotaEtichetteForm();
                webBrowserArticolo.DocumentText = "";//Properties.Resources.semafororosso;
            }
            else if (scritturaSuDB == 0)
            {
                this.WindowState = FormWindowState.Normal;
                this.TopMost = true;
                lblErrore.Visible = true;
                lblErrore.Text = "ERRORE\nSUBSTRING\nPESATA";
                SvuotaEtichetteForm();
                webBrowserArticolo.DocumentText = "";//Properties.Resources.semafororosso;
            }
        }
        #endregion

        #region GESTIONE BOTTONI e LINKLABEL DELLA FORM

        //linkLabel cambia postazione, per switchare al volo da una postazione all'altra
        private void linkLabelCambiaPostazione_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string configPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "appPeso.exe");
            Configuration config = ConfigurationManager.OpenExeConfiguration(configPath);

            if (ConfigurationManager.AppSettings["stazionePC"] == "RICCARNI1")
                config.AppSettings.Settings["stazionePC"].Value = "RICCARNI2";
            else
                config.AppSettings.Settings["stazionePC"].Value = "RICCARNI1";

            // se confermo il riavvio, allora salvo le impostazioni di app.config e riavvio
            if (MessageBox.Show("E' stato richiesto di cambiare la postazione di lavoro; per rendere effettive le modifiche il programma verrà riavviato.\n\nConfermi il cambio postazione?", "ATTENZIONE!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                config.Save();
                Application.Restart();
            }
        }

        // gestisco l'evento click del pulsante "Metti in ascolto"
        private void btnStart_Click(object sender, EventArgs e)
        {
            _spManager.StartListening();
            groupBoxCOMSettings.Enabled = false;
            btnStart.Enabled = false;
            btnQRCode.Enabled = false;
            btnBarcode.Enabled = false;
            lblErrore.Visible = false;
        }

        // gestisco l'evento click del pulsante "Stoppa e pulisci"
        private void btnStop_Click(object sender, EventArgs e)
        {
            groupBoxCOMSettings.Enabled = true;
            btnQRCode.Enabled = false;
            btnBarcode.Enabled = false;
            btnStart.Enabled = true;
            lblFornitore_2.Text = "--";
            lblTipoCarne_2.Text = "--";
            lblCodArt_2.Text = "--";
            lblDescrArt_2.Text = "--";
            lblLotto_2.Text = "--";
            lblScadLotto_2.Text = "--";
            lblColli_2.Text = "--";
            lblOrdineWO_2.Text = "--";
            lblBollaDDT_2.Text = "--";
            lblPaeseOrigine_2.Text = "--";
            tbLordo.Clear();
            tbTara.Clear();
            tbNetto.Clear();
            pictureBoxSemaforo.Visible = false;
            pictureBoxAlertColli.Visible = false;
            pictureBoxPaeseOrigine.Visible = false;
            lblErrore.Visible = false;
            if (ConfigurationManager.AppSettings["simulazione"] == "OFF")
                _spManager.StopListening();
        }

        // bottone che apre il form figlo per la stampa delle etichette colli
        private void btnEtichettaCampione_Click(object sender, EventArgs e)
        {
            FormEtichettaCampione f = new FormEtichettaCampione();
            f.ShowDialog();
        }

        // gestisco l'evento click del pulsante "Esci"
        private void btnEsci_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Chiudendo questa applicazione non funzionerà lo scambio dati tra bilancia e gestionale.\n\nSEI SICURO DI VOLER USCIRE?", "ATTENZIONE!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (MessageBox.Show("SICURO SICURO??", "ATTENZIONE!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if(ConfigurationManager.AppSettings["simulazione"] == "OFF")
                        _spManager.Dispose();
                    Application.Exit();
                }
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // pulsante che manda il comando CTRL+ALT+i al browser come se si cliccasse sulla lente di JDE
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        private void btnRefreshJDE_Click(object sender, EventArgs e)
        {
            // intercetto il processo Internet Explorer, Firefox o Chrome
            Process[] ie = Process.GetProcessesByName("iexplore");
            Process[] firefox = Process.GetProcessesByName("firefox");
            Process[] chrome = Process.GetProcessesByName("chrome");
            Process[] edge = Process.GetProcessesByName("MicrosoftEdge");
            Process[] edgeCP = Process.GetProcessesByName("MicrosoftEdgeCP");
            Process[] edgeSH = Process.GetProcessesByName("MicrosoftEdgeSH");

            foreach (Process proc in ie)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                SendKeys.SendWait("^%(i)"); //CTRL+ALT+i
            }
            foreach (Process proc in firefox)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                SendKeys.SendWait("^%(i)"); //CTRL+ALT+i
            }
            foreach (Process proc in chrome)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                SendKeys.SendWait("^%(i)");//CTRL+ALT+i
            }
            foreach (Process proc in edge)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                SendKeys.SendWait("^%(i)");//CTRL+ALT+i
            }
            foreach (Process proc in edgeCP)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                SendKeys.SendWait("^%(i)");//CTRL+ALT+i
            }
            foreach (Process proc in edgeSH)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                SendKeys.SendWait("^%(i)");//CTRL+ALT+i
            }
        }

        private void btnSvuota_Click(object sender, EventArgs e)
        {
            tbLordo.Text = string.Empty;
            tbTara.Text = string.Empty;
            tbNetto.Text = string.Empty;
        }

        private void btnQRCode_Click(object sender, EventArgs e)
        {
            if (PE55ETBC1 == String.Empty)
            {
                MessageBox.Show("Etichetta barcode non trovata!", "ERRORE QR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            { 
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(PE55ETBC1, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(10, Color.DarkRed, Color.White, true);
                qrCodeImage.Save(@"QRcode.jpg");

                using (Form form = new Form())
                {
                    Bitmap iconaForm = new Bitmap(Properties.Resources.serialport1);
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.Size = qrCodeImage.Size;
                    form.Icon = Icon.FromHandle(iconaForm.GetHicon());
                    form.Text = "QR Code - CLICCA SULL'IMMAGINE PER STAMPARE";
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;

                    PictureBox pb = new PictureBox();
                    pb.Dock = DockStyle.Fill;
                    pb.Image = qrCodeImage;
                    pb.Cursor = Cursors.Hand;
                    pb.Click += new System.EventHandler(QRcodeImageClick); // quando clicco sull'immagine scateno l'evento
                    form.Controls.Add(pb);
                    form.ShowDialog();
                }
            }
        }

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            if (PE55ETBC1 == String.Empty)
            {
                MessageBox.Show("Etichetta barcode non trovata!", "ERRORE BARCODE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                BarcodeSettings bs = new BarcodeSettings();
                bs.Type = BarCodeType.Code39;
                bs.Data = PE55ETBC1;
                bs.AutoResize = true;
                bs.ForeColor = Color.Black;
                BarCodeGenerator bg = new BarCodeGenerator(bs);
                Image barcodeImage = bg.GenerateImage();
                Bitmap fileImgFisico = new Bitmap(barcodeImage);
                fileImgFisico.Save(@"barcodeCode39.jpg");

                using (Form form = new Form())
                {
                    Bitmap iconaForm = new Bitmap(Properties.Resources.serialport1);
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.Size = barcodeImage.Size;
                    form.Icon = Icon.FromHandle(iconaForm.GetHicon());
                    form.Text = "barcode Code39 - CLICCA SULL'IMMAGINE PER STAMPARE";
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;

                    PictureBox pb = new PictureBox();
                    pb.Dock = DockStyle.Fill;
                    pb.Image = barcodeImage;
                    pb.Cursor = Cursors.Hand;
                    pb.Click += new System.EventHandler(barcodeImageClick); // quando clicco sull'immagine scateno l'evento

                    form.Controls.Add(pb);
                    form.ShowDialog();
                }
            }
        }
        private void barcodeImageClick(object sender, EventArgs e)
        {
            Process.Start(@"barcodeCode39.jpg");
        }
        private void QRcodeImageClick(object sender, EventArgs e)
        {
            Process.Start(@"QRcode.jpg");
        }
        #endregion

        #region GESTIONE LETTURA DATI SEMAFORO DA DATABASE
        // dichiaro tutte le variabili da impostare quando leggo il SEMAFORO e da passare alle tabelle PESATE + ETICHETTE
        // occhio ai tipi di dato (float, string, ecc.) !!!!
        string stampante;
        string PSBHAPPL = null;
        string PSBHVER = null;
        string PSUSER = null;
        string PSUPMJ = null;
        float PSUPMT = 0;
        string PSURCD = null;
        float PSURAB = 0;
        string PSURDT = null;
        string PSURRF = null;
        float PSURAT = 0;
        string PSEDSP = null;
        string PSSTATUS = null;
        string PSPIND = null;
        float PS55RCPTID = 0;
        float PS55RCPTLN = 0;
        string PS55STZP = null;
        string PS55STPP;
        float PSAN8 = 0;
        string PSALPH = null;
        float PSITM = 0;
        string PSLITM = null;
        string PSDSC1 = null;
        string PSDSC2 = null;
        string PSSHCM = null;
        string PS55DSSHCM = null;
        string PSPRP9 = null;
        string PS55DSPRP9 = null;
        string PSLOTG = null;
        string PS55DSLOTG = null;
        string PSORIG = null;
        string PS55DSORIG = null;
        string PSRCD = null;
        string PS55DSRCD = null;
        string PS55SZRCDJ = null;
        string PSRLOT = null;
        string PSLOTN = null;
        string PS55SZMMEJ = null;
        float PS55TARA = 0;
        float PS55TCLL = 0;
        float PS55NCOL = 0;
        string P2DOCO = null;
        float P2LNID = 0;

        // dichiaro le variabili da calcolare e scrivere in tabella PESATE
        float PE55PNET = 0; // peso netto da calcolare
        string PE55ETBC1 = ""; // barcode1 = LITM(6) + LOTN(12) + PESO(4) + COLLO(2)
        string PE55ETBC2 = ""; // barcode2 aggiuntivo, per il momento non utilizato
        string PE55ETBC3 = ""; // barcode3 aggiuntivo, per il momento non utilizato

        private int ControllaELeggiSemaforo()
        {
            string connectionString = null;
            string sql = null;
            string stazionePC = null;
            string schemaDB = null;

            // recupero i valori dello schema del DB da utilizzare nelle query dal file app.config
            schemaDB = ConfigurationManager.AppSettings["schemaDB"];

            // recupero i valori della substring della pesata e della stazione PC dal file app.config
            stazionePC = ConfigurationManager.AppSettings["stazionePC"];

            // inizializzo la connection string
            connectionString = ConfigurationManager.ConnectionStrings["JDE"].ConnectionString;

            // leggo dalla tabella Semaforo se il lo status è blank è alto e la stazione PC è la mia
            sql = "set nocount on;select * from "+ schemaDB + "F554312S sem INNER JOIN "+ schemaDB + " F554312A agg on (agg.P255RCPTID=sem.PS55RCPTID and agg.P255RCPTLN=sem.PS55RCPTLN) where sem.PSPIND = 'P' and sem.PSSTATUS = '' and sem.PS55STZP='" + stazionePC+"'";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, cnn))
            {
                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read()) // leggo tutti i campi della tabella semaforo e li metto nelle rispettive variabili
                        {
                            PSBHAPPL = Convert.ToString(reader["PSBHAPPL"]);
                            PSBHVER = Convert.ToString(reader["PSBHVER"]);
                            PSUSER = Convert.ToString(reader["PSUSER"]);
                            PSUPMJ = Convert.ToString(reader["PSUPMJ"]);
                            PSUPMT = float.Parse(Convert.ToString(reader["PSUPMT"]));
                            PSURCD = Convert.ToString(reader["PSURCD"]);
                            PSURAB = float.Parse(Convert.ToString(reader["PSURAB"]));
                            PSURDT = Convert.ToString(reader["PSURDT"]);
                            PSURRF = Convert.ToString(reader["PSURRF"]);
                            PSURAT = float.Parse(Convert.ToString(reader["PSURAT"]));
                            PSEDSP = Convert.ToString(reader["PSEDSP"]);
                            PSSTATUS = Convert.ToString(reader["PSSTATUS"]);
                            PSPIND = Convert.ToString(reader["PSPIND"]);
                            PS55RCPTID = float.Parse(Convert.ToString(reader["PS55RCPTID"]));
                            PS55RCPTLN = float.Parse(Convert.ToString(reader["PS55RCPTLN"]));
                            PS55STZP = Convert.ToString(reader["PS55STZP"]);
                            PS55STPP = Convert.ToString(reader["PS55STPP"]);
                            PSAN8 = float.Parse(Convert.ToString(reader["PSAN8"]));
                            PSALPH = Convert.ToString(reader["PSALPH"]);
                            PSITM = float.Parse(Convert.ToString(reader["PSITM"]));
                            PSLITM = Convert.ToString(reader["PSLITM"]);
                            PSDSC1 = Convert.ToString(reader["PSDSC1"]);
                            PSDSC2 = Convert.ToString(reader["PSDSC2"]);
                            PSSHCM = Convert.ToString(reader["PSSHCM"]);
                            PS55DSSHCM = Convert.ToString(reader["PS55DSSHCM"]);
                            PSPRP9 = Convert.ToString(reader["PSPRP9"]);
                            PS55DSPRP9 = Convert.ToString(reader["PS55DSPRP9"]);
                            PSLOTG = Convert.ToString(reader["PSLOTG"]);
                            PS55DSLOTG = Convert.ToString(reader["PS55DSLOTG"]);
                            PSORIG = Convert.ToString(reader["PSORIG"]);
                            PS55DSORIG = Convert.ToString(reader["PS55DSORIG"]);
                            PSRCD = Convert.ToString(reader["PSRCD"]);
                            PS55DSRCD = Convert.ToString(reader["PS55DSRCD"]);
                            PS55SZRCDJ = Convert.ToString(reader["PS55SZRCDJ"]);
                            PSRLOT = Convert.ToString(reader["PSRLOT"]);
                            PSLOTN = Convert.ToString(reader["PSLOTN"]);
                            PS55SZMMEJ = Convert.ToString(reader["PS55SZMMEJ"]);
                            PS55TARA = float.Parse(Convert.ToString(reader["PS55TARA"]));
                            PS55TCLL = float.Parse(Convert.ToString(reader["PS55TCLL"]));
                            PS55NCOL = float.Parse(Convert.ToString(reader["PS55NCOL"]));
                            P2DOCO = Convert.ToString(reader["P2DOCO"]);
                            P2LNID = float.Parse(Convert.ToString(reader["P2LNID"]));
                            
                            lblFornitore_2.Visible = true;
                            lblFornitore_2.Text = Convert.ToString(reader["PSAN8"]).Trim() + "\n" + Convert.ToString(reader["PSALPH"]).Trim(); // cod. fornitore e desc. fornitore

                            lblTipoCarne_2.Visible = true;
                            lblTipoCarne_2.Text = Convert.ToString(reader["PSSHCM"]).Trim() +" "+ Convert.ToString(reader["PSPRP9"]).Trim();

                            lblCodArt_2.Visible = true;
                            lblCodArt_2.Text = Convert.ToString(reader["PSLITM"]).Trim(); // codice articolo

                            lblDescrArt_2.Visible = true;
                            lblDescrArt_2.Text = Convert.ToString(reader["PSDSC1"]).Trim(); // metto solo la DSC1 in quanto la DSC2 è sempre "Carni"

                            lblLotto_2.Visible = true;
                            lblLotto_2.Text = Convert.ToString(reader["PSLOTN"]).Trim(); // lotto

                            lblScadLotto_2.Visible = true;
                            lblScadLotto_2.Text = Convert.ToString(reader["PS55SZMMEJ"]).Trim(); // data scadenza lotto

                            lblOrdineWO_2.Visible = true;
                            lblOrdineWO_2.Text = Convert.ToString(reader["P2DOCO"]).Trim(); // numero ordine - WO

                            lblBollaDDT_2.Visible = true;
                            lblBollaDDT_2.Text = Convert.ToString(reader["P2VRMK"]).Trim(); // numero e data bolla

                            lblPaeseOrigine_2.Visible = true;
                            lblPaeseOrigine_2.Text = Convert.ToString(reader["PSORIG"]).Trim() + ' ' + Convert.ToString(reader["PS55DSORIG"]).Trim(); // Paese di origine e relativa bandiera
                            switch (Convert.ToString(reader["PSORIG"]).Trim())
                            {
                                case "IT":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.italy;
                                    break;
                                case "DE":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.germany;
                                    break;
                                case "DK":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.denmark;
                                    break;
                                case "NL":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.netherlands;
                                    break;
                                case "ES":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.spain;
                                    break;
                                case "FR":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.france;
                                    break;
                                case "IE":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.ireland;
                                    break;
                                case "AT":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.austria;
                                    break;
                                case "BE":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.belgium;
                                    break;
                                case "GB":
                                    pictureBoxPaeseOrigine.Visible = true;
                                    pictureBoxPaeseOrigine.Image = Properties.Resources.gb_uk;
                                    break;
                                default:
                                    pictureBoxPaeseOrigine.Visible = false;
                                    break;
                            }

                            lblColli_2.Visible = true;
                            lblColli_2.Text = Convert.ToString(reader["PS55NCOL"]) +" di "+ Convert.ToString(reader["PS55TCLL"]);
                            if(PS55NCOL > PS55TCLL)
                            {
                                pictureBoxAlertColli.Visible = true;
                                pictureBoxAlertColli.Image = Properties.Resources.alert;
                            }
                            else if(PS55NCOL == PS55TCLL)
                            {
                                pictureBoxAlertColli.Visible = true;
                                pictureBoxAlertColli.Image = Properties.Resources.tick_ok;
                            }

                            pictureBoxSemaforo.Visible = true;
                            pictureBoxSemaforo.Refresh();
                            pictureBoxSemaforo.Image = Properties.Resources.semaforoOK;

                            webBrowserArticolo.Refresh();
                            webBrowserArticolo.Navigate("http://jdewebsrv02:85/URL_QS?output=img&dimensione=original&articolo="+ Convert.ToString(reader["PSLITM"]).TrimEnd());
                        }
                        return 1;
                    }
                    else
                    {
                        pictureBoxSemaforo.Visible = true;
                        pictureBoxSemaforo.Refresh();
                        pictureBoxSemaforo.Image = Properties.Resources.semaforoKO;
                        lblErrore.Visible = true;
                        lblErrore.Text = "SEMAFORO NON ATTIVO!\nCONTROLLARE JD EDWARDS";
                        SvuotaEtichetteForm();
                        webBrowserArticolo.DocumentText = Properties.Resources.semafororosso;

                        return 0;
                    }
                }
            }
        }
        #endregion

        #region GESTIONE SCRITTURA SU DATABASE (SIA DA SERIALE CHE IN MANUALE)
        // se il semaforo è OK procedo: recupero STAZIONE, PESATA, CARATTERE_OK_PESATA 
        private int ScriviSuDatabase(string str, bool modalita)
        {
            if (ControllaELeggiSemaforo() == 1) // se il semaforo è ok procedo, altrimenti esco
            {
                string connectionString = null;
                string sql_stmt_1 = null;
                string sql_stmt_2 = null;
                string sql_stmt_3 = null;
                string carattereOKPesata_da_Config = null;
                string carattereOKPesata_da_LetturaBilancia = null;
                string pesataLetta = null;
                string stazionePC = null;
                string timestamp_unique = null;
                string schemaDB = null;

                // recupero i valori dello schema del DB da utilizzare nelle query dal file app.config
                schemaDB = ConfigurationManager.AppSettings["schemaDB"];

                // recupero i valori della substring della pesata, del secondo carattere della stringa e della stazione PC dal file app.config
                stazionePC = ConfigurationManager.AppSettings["stazionePC"];

                // recupero dai parametri di config il carattere che dice se la pesata è valida e OK (nella bilancia è = 2, nella pistola è = 1)
                // recupero dalla stringa della bilancia seriale il secondo carattere che mi dice se la pesata è OK o meno
                if (modalitaStandard == true)
                {
                    carattereOKPesata_da_Config = ConfigurationManager.AppSettings["carattereOKPesata"];
                    carattereOKPesata_da_LetturaBilancia = str.Substring(1, 1);

                    // recupero dalla stringa a 32bit della seriale la sottostringa che mi dà il peso lordo della carne
                    if (str.Length >= 16)
                        pesataLetta = str.Substring(int.Parse(ConfigurationManager.AppSettings["carattereInizio"]), int.Parse(ConfigurationManager.AppSettings["lunghezzaStringa"]));
                    else
                        return 0;
                }

                // inizializzo la connection string
                connectionString = ConfigurationManager.ConnectionStrings["JDE"].ConnectionString;

                // statement SQL per inserire dati nella tabella delle pesate (F554312P) e in quella delle etichette (F554312Z)
                sql_stmt_1 = "set nocount on;insert into " + schemaDB + "F554312P(PEBHAPPL,PEBHVER,PEUSER,PEUPMJ,PEUPMT,PEURCD,PEURAB,PEURDT,PEURRF,PEURAT,PEEDSP,PESTATUS,PEPIND,PE55RCPTID,PE55RCPTLN,PE55NCOL,PE55TCLL,PE55STZP,PE55PLRD,PE55TARA,PE55PNET,PE55ETID,PE55ETBC1,PE55ETBC2,PE55ETBC3) " +
                            "values(@BHAPPL,@BHVER,@USER,@UPMJ,@UPMT,@URCD,@URAB,@URDT,@URRF,@URAT,@EDSP,@STATUS,@PIND,@55RCPTID,@55RCPTLN,@55NCOL,@55TCLL,@55STZP,@55PLRD,@55TARA,@55PNET,@55ETID,@55ETBC1,@55ETBC2,@55ETBC3)";

                sql_stmt_2 = "set nocount on;insert into " + schemaDB + "F554312Z (PZBHAPPL,PZBHVER,PZUSER,PZUPMJ,PZUPMT,PZURCD,PZURAB,PZURDT,PZURRF,PZURAT,PZEDSP,PZSTATUS,PZPIND,PZ55ETID,PZ55STZP,PZ55STPP,PZAN8,PZALPH,PZITM,PZLITM,PZDSC1,PZDSC2,PZSHCM,PZ55DSSHCM,PZPRP9,PZ55DSPRP9,PZLOTG,PZ55DSLOTG,PZORIG,PZ55DSORIG,PZRCD,PZ55DSRCD,PZ55SZRCDJ,PZRLOT,PZLOTN,PZ55SZMMEJ,PZ55NCOL,PZ55TCLL,PZ55TARA,PZ55PNET,PZ55ETBC1,PZ55ETBC2,PZ55ETBC3) " +
                            "values(@BHAPPL,@BHVER,@USER,@UPMJ,@UPMT,@URCD,@URAB,@URDT,@URRF,@URAT,@EDSP,@STATUS,@PIND,@55ETID,@55STZP,@55STPP,@AN8,@ALPH,@ITM,@LITM,@DSC1,@DSC2,@SHCM,@55DSSHCM,@PRP9,@55DSPRP9,@LOTG,@55DSLOTG,@ORIG,@55DSORIG,@RCD,@55DSRCD,@55SZRCDJ,@RLOT,@LOTN,@55SZMMEJ,@55NCOL,@55TCLL,@55TARA,@55PNET,@55ETBC1,@55ETBC2,@55ETBC3)";

                sql_stmt_3 = "set nocount on;update " + schemaDB + "F554312S set PS55NCOL = @55NCOL where PS55RCPTID = @55RCPTID and PS55RCPTLN = @55RCPTLN";

                // campo calcolato Peso Netto da mettere nella insert => nelle tabelle delle PESATE e delle ETICHETTE
                if (modalitaStandard == true)
                    PE55PNET = float.Parse(pesataLetta) * 1000 - PS55TARA;
                else
                    PE55PNET = pesoManualeNetto * 1000;

                // recupero le informazioni sulla stampante di default del sistema
                string stampanteDefault;
                System.Drawing.Printing.PrinterSettings _ps = new System.Drawing.Printing.PrinterSettings();
                stampanteDefault = _ps.PrinterName;
                if (chbxStampante.Checked)
                    stampante = stampanteDefault;
                else
                    stampante = PS55STPP;

                // la stringa che compone il BARCODE deve avere lunghezza fissa a 24 caratteri => nelle tabelle delle PESATE e delle ETICHETTE
                StringBuilder _sb = new StringBuilder();
                _sb.Append(string.Format("{0,6}", PSLITM.Substring(0,6)));
                _sb.Append(string.Format("{0,12}", PSLOTN.Substring(0,12)));
                _sb.Append(string.Format("{0,4}", (PE55PNET/1000).ToString("0000")));
                _sb.Append(string.Format("{0,2}", InserisciCifraZero((Int32)PS55NCOL)).Substring(0,2));
                PE55ETBC1 = _sb.Replace(" ", string.Empty).ToString();
                PE55ETBC1.Substring(0, 24).TrimEnd();

                // creo la chiave univoca concatenando data e ora del semaforo (valori fissi) + timestamp GG/MM/MIN/SEC di adesso + num. stazione, fino a 25 caratteri
                string timeStampAdesso = InserisciCifraZero(DateTime.Now.Day) + InserisciCifraZero(DateTime.Now.Month) + InserisciCifraZero(DateTime.Now.Minute) + InserisciCifraZero(DateTime.Now.Second);
                timestamp_unique = PSUPMJ + PSUPMT + "_" + timeStampAdesso + "_" + stazionePC.Substring(stazionePC.Length - 1);
                
                SqlTransaction trans = null;
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    try
                    {
                        cnn.Open();

                        // preparo i rispettivi comandi di scrittura nella tabella delle PESATE e delle ETICHETTE
                        SqlCommand cmd1 = new SqlCommand(sql_stmt_1, cnn);
                        SqlCommand cmd2 = new SqlCommand(sql_stmt_2, cnn);
                        SqlCommand cmd3 = new SqlCommand(sql_stmt_3, cnn);

                        // creo e imposto i parametri da mettere nella INSERT delle PESATE leggendoli dal semaforo
                        cmd1.Parameters.Add("@BHAPPL", SqlDbType.VarChar).Value = PSBHAPPL;
                        cmd1.Parameters.Add("@BHVER", SqlDbType.VarChar).Value = PSBHVER;
                        cmd1.Parameters.Add("@USER", SqlDbType.VarChar).Value = PSUSER;
                        cmd1.Parameters.Add("@UPMJ", SqlDbType.VarChar).Value = PSUPMJ;
                        cmd1.Parameters.Add("@UPMT", SqlDbType.Float).Value = PSUPMT;
                        cmd1.Parameters.Add("@URCD", SqlDbType.VarChar).Value = PSURCD;
                        cmd1.Parameters.Add("@URAB", SqlDbType.Float).Value = PSURAB;
                        cmd1.Parameters.Add("@URDT", SqlDbType.VarChar).Value = PSURDT;
                        cmd1.Parameters.Add("@URRF", SqlDbType.VarChar).Value = P2DOCO.ToString() +"-"+ P2LNID.ToString("000000"); // concateno P2DOCO + "-" + P2LNID (fisso a 6 con 0 davanti) solo nelle pesate
                        cmd1.Parameters.Add("@URAT", SqlDbType.Float).Value = PSURAT;
                        cmd1.Parameters.Add("@EDSP", SqlDbType.Char).Value = PSEDSP;
                        cmd1.Parameters.Add("@STATUS", SqlDbType.Char).Value = PSSTATUS;
                        cmd1.Parameters.Add("@PIND", SqlDbType.Char).Value = PSPIND;
                        cmd1.Parameters.Add("@55RCPTID", SqlDbType.Float).Value = PS55RCPTID;
                        cmd1.Parameters.Add("@55RCPTLN", SqlDbType.Float).Value = PS55RCPTLN;
                        cmd1.Parameters.Add("@55NCOL", SqlDbType.Float).Value = PS55NCOL;
                        cmd1.Parameters.Add("@55TCLL", SqlDbType.Float).Value = PS55TCLL;
                        cmd1.Parameters.Add("@55STZP", SqlDbType.VarChar).Value = PS55STZP;

                        // qui prendo la MIA pesata appena letta (lordo) e la moltiplico per 1000 per JDE
                        if (modalitaStandard == true)
                            cmd1.Parameters.Add("@55PLRD", SqlDbType.Float).Value = float.Parse(pesataLetta) * 1000;  
                        else
                            cmd1.Parameters.Add("@55PLRD", SqlDbType.Float).Value = pesoManualeLordo * 1000;

                        cmd1.Parameters.Add("@55TARA", SqlDbType.Float).Value = PS55TARA;

                        // qui prendo la MIA pesata appena letta (netto) e la moltiplico per 1000 per JDE
                        if (modalitaStandard == true)
                            cmd1.Parameters.Add("@55PNET", SqlDbType.Float).Value = PE55PNET; // quello appena calcolato sopra
                        else
                            cmd1.Parameters.Add("@55PNET", SqlDbType.Float).Value = pesoManualeNetto * 1000;

                        cmd1.Parameters.Add("@55ETID", SqlDbType.VarChar).Value = timestamp_unique;
                        cmd1.Parameters.Add("@55ETBC1", SqlDbType.VarChar).Value = PE55ETBC1;
                        cmd1.Parameters.Add("@55ETBC2", SqlDbType.VarChar).Value = PE55ETBC2;
                        cmd1.Parameters.Add("@55ETBC3", SqlDbType.VarChar).Value = PE55ETBC3;

                        // creo e imposto i parametri da mettere nella INSERT delle ETICHETTE leggendoli dal semaforo
                        cmd2.Parameters.Add("@BHAPPL", SqlDbType.VarChar).Value = PSBHAPPL;
                        cmd2.Parameters.Add("@BHVER", SqlDbType.VarChar).Value = PSBHVER;
                        cmd2.Parameters.Add("@USER", SqlDbType.VarChar).Value = PSUSER;
                        cmd2.Parameters.Add("@UPMJ", SqlDbType.VarChar).Value = PSUPMJ;
                        cmd2.Parameters.Add("@UPMT", SqlDbType.Float).Value = PSUPMT;
                        cmd2.Parameters.Add("@URCD", SqlDbType.VarChar).Value = PSURCD;
                        cmd2.Parameters.Add("@URAB", SqlDbType.Float).Value = PSURAB;
                        cmd2.Parameters.Add("@URDT", SqlDbType.VarChar).Value = PSURDT;
                        cmd2.Parameters.Add("@URRF", SqlDbType.VarChar).Value = PSURRF;
                        cmd2.Parameters.Add("@URAT", SqlDbType.Float).Value = PSURAT;
                        cmd2.Parameters.Add("@EDSP", SqlDbType.Char).Value = ""; // blank sempre
                        cmd2.Parameters.Add("@STATUS", SqlDbType.Char).Value = PSSTATUS;
                        cmd2.Parameters.Add("@PIND", SqlDbType.Char).Value = PSPIND;
                        cmd2.Parameters.Add("@55ETID", SqlDbType.VarChar).Value = timestamp_unique;
                        cmd2.Parameters.Add("@55STZP", SqlDbType.VarChar).Value = PS55STZP;
                        cmd2.Parameters.Add("@55STPP", SqlDbType.VarChar).Value = stampante;
                        cmd2.Parameters.Add("@AN8", SqlDbType.Float).Value = PSAN8;
                        cmd2.Parameters.Add("@ALPH", SqlDbType.VarChar).Value = PSALPH;
                        cmd2.Parameters.Add("@ITM", SqlDbType.Float).Value = PSITM;
                        cmd2.Parameters.Add("@LITM", SqlDbType.VarChar).Value = PSLITM;
                        cmd2.Parameters.Add("@DSC1", SqlDbType.VarChar).Value = PSDSC1;
                        cmd2.Parameters.Add("@DSC2", SqlDbType.VarChar).Value = PSDSC2;
                        cmd2.Parameters.Add("@SHCM", SqlDbType.VarChar).Value = PSSHCM;
                        cmd2.Parameters.Add("@55DSSHCM", SqlDbType.VarChar).Value = PS55DSSHCM;
                        cmd2.Parameters.Add("@PRP9", SqlDbType.VarChar).Value = PSPRP9;
                        cmd2.Parameters.Add("@55DSPRP9", SqlDbType.VarChar).Value = PS55DSPRP9;
                        cmd2.Parameters.Add("@LOTG", SqlDbType.VarChar).Value = PSLOTG;
                        cmd2.Parameters.Add("@55DSLOTG", SqlDbType.VarChar).Value = PS55DSLOTG;
                        cmd2.Parameters.Add("@ORIG", SqlDbType.VarChar).Value = PSORIG;
                        cmd2.Parameters.Add("@55DSORIG", SqlDbType.VarChar).Value = PS55DSORIG;
                        cmd2.Parameters.Add("@RCD", SqlDbType.VarChar).Value = PSRCD;
                        cmd2.Parameters.Add("@55DSRCD", SqlDbType.VarChar).Value = PS55DSRCD;
                        cmd2.Parameters.Add("@55SZRCDJ", SqlDbType.VarChar).Value = PS55SZRCDJ;
                        cmd2.Parameters.Add("@RLOT", SqlDbType.VarChar).Value = PSRLOT;
                        cmd2.Parameters.Add("@LOTN", SqlDbType.VarChar).Value = PSLOTN;
                        cmd2.Parameters.Add("@55SZMMEJ", SqlDbType.VarChar).Value = PS55SZMMEJ;
                        cmd2.Parameters.Add("@55NCOL", SqlDbType.Float).Value = PS55NCOL;
                        cmd2.Parameters.Add("@55TCLL", SqlDbType.Float).Value = PS55TCLL;
                        cmd2.Parameters.Add("@55TARA", SqlDbType.Float).Value = PS55TARA/1000;
                        
                        // qui prendo la MIA pesata da input (solo netto) e NON la moltiplico per 1000 perchévado sulle etichette
                        if (modalitaStandard == true)
                            cmd2.Parameters.Add("@55PNET", SqlDbType.Float).Value = PE55PNET/1000; 
                        else
                            cmd2.Parameters.Add("@55PNET", SqlDbType.Float).Value = pesoManualeNetto;

                        cmd2.Parameters.Add("@55ETBC1", SqlDbType.VarChar).Value = PE55ETBC1;
                        cmd2.Parameters.Add("@55ETBC2", SqlDbType.VarChar).Value = PE55ETBC2;
                        cmd2.Parameters.Add("@55ETBC3", SqlDbType.VarChar).Value = PE55ETBC3;

                        // creo e imposto i parametri da mettere nella UPDATE delle SEMAFORO leggendoli dal semaforo
                        cmd3.Parameters.Add("@55NCOL", SqlDbType.Float).Value = ++PS55NCOL; // incremento il contatore
                        cmd3.Parameters.Add("@55RCPTID", SqlDbType.Float).Value = PS55RCPTID;
                        cmd3.Parameters.Add("@55RCPTLN", SqlDbType.Float).Value = PS55RCPTLN;

                        if(modalitaStandard == false)
                        {
                            carattereOKPesata_da_LetturaBilancia = "0";
                            carattereOKPesata_da_Config = "0";
                        }
                        if (carattereOKPesata_da_LetturaBilancia == carattereOKPesata_da_Config)
                        {
                            // apro la transazione e la associo ai tre comandi SQL 
                            trans = cnn.BeginTransaction(); 
                            cmd1.Transaction = trans;
                            cmd2.Transaction = trans;
                            cmd3.Transaction = trans;

                            try
                            {
                                cmd1.ExecuteNonQuery(); // scrivo su tabella pesate F554312P
                                cmd2.ExecuteNonQuery(); // scrivo su tabella etichette F554312Z
                                cmd3.ExecuteNonQuery(); // aggiorno progressivo colli nel semaforo F554312S
                                trans.Commit(); // se è tutto ok chiudo la transazione

                                if (modalitaStandard == true)
                                {
                                    // mostro a video la pesata lorda e posiziono il cursore in fondo a destra
                                    tbLordo.Text = pesataLetta.Trim();
                                    tbLordo.SelectionStart = tbLordo.Text.Length;
                                    tbLordo.SelectionLength = 0;

                                    // mostro a video la tara e posiziono il cursore in fondo a destra
                                    tbTara.Text = (PS55TARA / 1000).ToString();
                                    tbTara.SelectionStart = tbTara.Text.Length;
                                    tbTara.SelectionLength = 0;

                                    // mostro a video il peso netto calcolato e posiziono il cursore in fondo a destra
                                    tbNetto.Text = (PE55PNET / 1000).ToString();
                                    tbNetto.SelectionStart = tbNetto.Text.Length;
                                    tbNetto.SelectionLength = 0;
                                }

                                // abilito la stampa del QRCode e ritorno 1
                                btnQRCode.Enabled = true;
                                btnBarcode.Enabled = true;
                                return 1;
                            }
                            catch(Exception e)
                            {
                                trans.Rollback();
                                MessageBox.Show("SCRITTURE SU DATABASE FALLITE, RIPETERE OPERAZIONE!\n\n"+e.Message, "KO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            return -1; // PESATA NON VALIDA
                        }
                        cnn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ERRORE GENERICO SCRITTURA DATABASE:" + ex.Message);
                    }
                }
            }
            else
            {
                //semaforo non attivo => non faccio niente
                return -2;
            }
            return 0;
        }
        #endregion

        #region FUNZIONI VARIE

        // svuoto le etichette/label
        private void SvuotaEtichetteForm()
        {
            lblFornitore_2.Text = "--";
            lblTipoCarne_2.Text = "--";
            lblCodArt_2.Text = "--";
            lblDescrArt_2.Text = "--";
            lblLotto_2.Text = "--";
            lblScadLotto_2.Text = "--";
            lblOrdineWO_2.Text = "--";
            lblBollaDDT_2.Text = "--";
            lblPaeseOrigine_2.Text = "--";
            lblColli_2.Text = "--";
            pictureBoxAlertColli.Visible = false;
            pictureBoxPaeseOrigine.Visible = false;
        } 

        // forzo la mancata visualizzazione del caret/cursore nelle 3 textbox dei pesi
        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hWnd);
        public void HideCaret()
        {
            HideCaret(tbLordo.Handle);
            HideCaret(tbTara.Handle);
            HideCaret(tbNetto.Handle);
        }

        private string InserisciCifraZero(int n)
        {
            return (n < 10 ? "0" : "") + n;
        }
        #endregion

        #region GESTIONE LINK
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(ConfigurationManager.AppSettings["stazionePC"] == "RICCARNI1")
                Process.Start("iexplore.exe", "-nomerge http://jdewebsrv01:10100/jde/ShortcutLauncher?OID=P554312C_W554312CA_FEL0001&FormDSTmpl=|1|2|&FormDSData=|||");
            else if (ConfigurationManager.AppSettings["stazionePC"] == "RICCARNI2")
                Process.Start("iexplore.exe", "-nomerge http://jdewebsrv01:10100/jde/ShortcutLauncher?OID=P554312C_W554312CA_FEL0002&FormDSTmpl=|1|2|&FormDSData=|||");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("\\\\webportal\\DDT_DA_ARCHIVIARE\\DDT_Carni_OC");
        }

        private void linkLabelErick_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:ebaldi@felsineo.com");
        }
        #endregion

        #region GESTIONE PESATE IN MANUALE (MODALITA' SIMULAZIONE ON)
        // gestisco l'immissione di soli INTERI nelle caselle dei pesi LORDO e NETTO
        private void tbLordo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void tbNetto_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        float pesoManualeNetto = 0;
        float pesoManualeLordo = 0;
        private void btnPesataManuale_Click(object sender, EventArgs e)
        {
            // se entrambe le caselle dei pesi sono vuote => ERRORE
            if (tbLordo.Text == "" && tbNetto.Text == "")
            {
                MessageBox.Show("INSERIRE ALMENO UN PESO LORDO O NETTO", "INSERIRE PESO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // se entrambe le caselle dei pesi sono riempite => ERRORE, DECIDITI!
            if (tbLordo.Text != "" && tbNetto.Text != "")
            {
                MessageBox.Show("HAI INSERITO SIA LORDO CHE NETTO!\nSVUOTA UNA DELLE DUE CASELLE!", "PESO DOPPIO", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // se inserisco il LORDO, allora mi devo calcolare il netto = lordo - tara (la tara la dà sempre il semaforo F554312S !)
            if (tbLordo.Text != "")
            {
                if (MessageBox.Show("Hai inserito il PESO LORDO.\nSei sicuro di mandare al sistema il LORDO?\n\nIn tal caso a sistema verrà calcolato il NETTO in base al lordo inserito e alla tara letta da JD Edwards.", "INSERITO PESO LORDO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (ControllaELeggiSemaforo() == 1) // leggo il semaforo per recuperarmi la TARA e vedere se posso scrivere
                    { 
                        pesoManualeLordo = float.Parse(tbLordo.Text);
                        pesoManualeNetto = float.Parse(tbLordo.Text) - (PS55TARA / 1000);

                        int scritturaSuDB = ScriviSuDatabase(pesoManualeNetto.ToString(), modalitaStandard);

                        if (scritturaSuDB == 1)
                        {
                            lblErrore.Visible = false;
                            btnRefreshJDE.PerformClick();
                        }
                        else if (scritturaSuDB == -1)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.TopMost = true;
                            lblErrore.Visible = true;
                            lblErrore.Text = "PESATA\nNON VALIDA";
                            SvuotaEtichetteForm();
                            webBrowserArticolo.Navigate("about:blank");
                        }
                        else if (scritturaSuDB == -2)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.TopMost = true;
                            lblErrore.Visible = true;
                            lblErrore.Text = "SEMAFORO\nNON ATTIVO";
                            SvuotaEtichetteForm();
                            webBrowserArticolo.Navigate("about:blank");
                        }
                        else if (scritturaSuDB == 0)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.TopMost = true;
                            lblErrore.Visible = true;
                            lblErrore.Text = "ERRORE\nSUBSTRING\nPESATA";
                            SvuotaEtichetteForm();
                            webBrowserArticolo.Navigate("about:blank");
                        }
                    }
                }
            }

            // se inserisco il NETTO, allora mi devo calcolare il lordo = netto + tara (la tara la dà sempre il semaforo F554312S !)
            if (tbNetto.Text != "")
            {
                if (MessageBox.Show("Hai inserito il PESO NETTO.\nSei sicuro di mandare al sistema il NETTO?\n\nIn tal caso a sistema verrà calcolato il LORDO in base al netto inserito e alla tara letta da JD Edwards.", "INSERITO PESO NETTO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (ControllaELeggiSemaforo() == 1)
                    { // leggo il semaforo per recuperarmi la TARA
                        pesoManualeLordo = float.Parse(tbNetto.Text) + PS55TARA / 1000; ;
                        pesoManualeNetto = float.Parse(tbNetto.Text);

                        int scritturaSuDB = ScriviSuDatabase(pesoManualeNetto.ToString(), modalitaStandard);

                        if (scritturaSuDB == 1)
                        {
                            lblErrore.Visible = false;
                            btnRefreshJDE.PerformClick();
                        }
                        else if (scritturaSuDB == -1)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.TopMost = true;
                            lblErrore.Visible = true;
                            lblErrore.Text = "PESATA\nNON VALIDA";
                            SvuotaEtichetteForm();
                            webBrowserArticolo.Navigate("about:blank");
                        }
                        else if (scritturaSuDB == -2)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.TopMost = true;
                            lblErrore.Visible = true;
                            lblErrore.Text = "SEMAFORO\nNON ATTIVO";
                            SvuotaEtichetteForm();
                            webBrowserArticolo.Navigate("about:blank");
                        }
                        else if (scritturaSuDB == 0)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.TopMost = true;
                            lblErrore.Visible = true;
                            lblErrore.Text = "ERRORE\nSUBSTRING\nPESATA";
                            SvuotaEtichetteForm();
                            webBrowserArticolo.Navigate("about:blank");
                        }
                    }
                }
            }
        }
        #endregion

        #region MECCANISMO PROTEZIONE CONNECTIONSTRING APP.CONFIG
        // clicco sul link encrypt/codifica e lancio la funzione che cripta la connectionString del file App.config
        // ovviamente il meccanismo di decodifica funziona solo se sei Erick/Samuele oppure Admin
        private void linkLabelEncrypt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("Sicuro di voler codificare / criptare il file .config?", "PROCEDERE?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                EncryptConnectionString(true, "appPeso.exe");
           
        }

        // clicco sul link decrypt/decodifica e lancio la funzione che cripta la connectionString del file App.config
        // ovviamente il meccanismo funziona solo se sei Erick/Samuele oppure Admin
        private void linkLabelDecrypt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Environment.UserName.Equals("ebaldi", StringComparison.InvariantCultureIgnoreCase)
                || Environment.UserName.Equals("spalmonari", StringComparison.InvariantCultureIgnoreCase)
                || Environment.UserName.Equals("jde", StringComparison.InvariantCultureIgnoreCase)
                || Environment.UserName.Equals("administrator", StringComparison.InvariantCultureIgnoreCase))
            {
                if (MessageBox.Show("Sicuro di voler decodificare / decriptare il file .config?", "PROCEDERE?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    EncryptConnectionString(false, "appPeso.exe");
            }
            else
                MessageBox.Show("Funzione di tipo admin non consentita all'utente " + Environment.UserName, "KO", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        public static void EncryptConnectionString(bool encrypt, string fileName)
        {
            string verso = null;
            Configuration configuration = null;

            try
            {
                // Apro il file App.Config e cerco la parte/tag connectionString da criptare
                configuration = ConfigurationManager.OpenExeConfiguration(fileName);
                ConnectionStringsSection configSection = configuration.GetSection("connectionStrings") as ConnectionStringsSection;
                
                if ((!configSection.ElementInformation.IsLocked) && (!configSection.SectionInformation.IsLocked))
                {
                    // critto il file
                    if (encrypt && !configSection.SectionInformation.IsProtected)
                    {
                        configSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider"); 
                    }

                    // decritto il file
                    if (!encrypt && configSection.SectionInformation.IsProtected)
                    {
                        configSection.SectionInformation.UnprotectSection();
                    }

                    //re-save the configuration file section
                    configSection.SectionInformation.ForceSave = true;
                    
                    // Salva la configurazione corrente
                    configuration.Save();

                    // scrivo l'esito a video
                    if (encrypt == true)
                        verso = "criptata";
                    else
                        verso = "de-criptata";
                    MessageBox.Show("OK\n\nLa connectionString è stata " + verso + "!\n\nIl file è stato salvato al percorso "+ configuration.FilePath, "ConnectionString criptata!",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);

                    // apro il file
                    Process.Start("notepad.exe", configuration.FilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion
    }
}