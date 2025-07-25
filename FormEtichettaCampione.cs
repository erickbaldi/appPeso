using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace SerialPortListener
{
    public partial class FormEtichettaCampione : Form
    {
        #region Variabili generiche globali
        
        // variabili pubbliche
        string stampante = "HP LaserJet P2035";
        float codFornitore = 0;
        string descFornitore = null;
        string codArticolo = null;
        int codArticoloCorto = 0;
        int giorniScadenza = 0;
        string descArticolo = null;
        string codNazione = null;
        string descNazione = null;
        string tipoCarne = null;
        string descTipoCarne = null;
        string codIGP = null;
        string descIGP = null;
        string julianDate = null;
        string codConformita = null;
        string descConformita = null;
        string etichettaBarcode = null;

        // variabili pubbliche per la connesione al database
        string connectionString = ConfigurationManager.ConnectionStrings["JDE"].ConnectionString;
        string schemaDB = ConfigurationManager.AppSettings["schemaDB"];
        string stazionePC = ConfigurationManager.AppSettings["stazionePC"];
        #endregion

        #region INIT FORM
        public FormEtichettaCampione()
        {
            InitializeComponent();
            popolaComboFornitori();
            popolaComboArticoli();
            popolaComboConformita();
            Load += FormEtichettaCampione_Shown;
        }

        private void FormEtichettaCampione_Shown(Object sender, EventArgs e)
        {
            checkBoxIGP.Checked = true;
            tbLottoEti.MaxLength = 12;
            tbProgCollo.MaxLength = 3;
            tbTotColli.MaxLength = 3;
            tbPesoNetto.MaxLength = 4;
            tbTara.MaxLength = 4;
            dtpScadenza.CustomFormat = "dd/MM/yyyy";
            dtpRicevimento.CustomFormat = "dd/MM/yyyy";
            pictureBoxNazione.Visible = false;
            lblChkIGP.Visible = false;
            lblTipoCarne_2.Visible = false;
        }
        #endregion

        #region ComboBox Fornitore
        private void comboBoxFornitoriCarne_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFornitoriCarne.GetItemText(this.comboBoxFornitoriCarne.SelectedValue).ToString() == "0")
                lblCodFornitore.Text = "";
            else
                lblCodFornitore.Text = comboBoxFornitoriCarne.GetItemText(this.comboBoxFornitoriCarne.SelectedValue).ToString();

            codFornitore = Convert.ToSingle(comboBoxFornitoriCarne.GetItemText(this.comboBoxFornitoriCarne.SelectedValue));
        }
        private void popolaComboFornitori()
        {
            string query = "set nocount on; select '' as CodFornitore, '' as Fornitore UNION select ABAN8 as CodFornitore, ltrim(rtrim(ABALPH)) as Fornitore from " + schemaDB + "F0101 with(nolock) where (ABAT1 != 'OF' and len(ABAN8) = 5 and ABAC02 = 'FCA') OR ABAN8 = 1 order by Fornitore"; //tutti i fornitori carne (FCA) + Felsineo (che è GEN = generico)
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();

            da.Fill(dt);

            comboBoxFornitoriCarne.ValueMember = "CodFornitore";
            comboBoxFornitoriCarne.DisplayMember = "Fornitore";
            comboBoxFornitoriCarne.DataSource = dt;

        }
        #endregion

        #region ComboBox Articolo
        private void comboBoxArticoli_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblCodArticolo.Text = comboBoxArticoli.GetItemText(this.comboBoxArticoli.SelectedValue).ToString();
            codArticolo = comboBoxArticoli.GetItemText(this.comboBoxArticoli.SelectedValue).ToString();
            //descArticolo = comboBoxArticoli.GetItemText(this.comboBoxArticoli.SelectedIndex).ToString();
        }
        private void popolaComboArticoli()
        {
            string query = "set nocount on;select '' as CodArticolo, '' as Articolo UNION select rtrim(IMLITM) as CodArticolo, ltrim(rtrim(IMLITM)) + ' - ' + ltrim(rtrim(IMDSC1)) as Articolo from " + schemaDB + "F4101 with(nolock) where imprp1 = 'CRN' and IMLITM != 'ACQUA' order by CodArticolo";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();

            da.Fill(dt);

            comboBoxArticoli.ValueMember = "CodArticolo";
            comboBoxArticoli.DisplayMember = "Articolo";
            comboBoxArticoli.DataSource = dt;
        }

        #endregion

        #region ComboBox Conformità
        private void comboBoxConformita_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblCodConformita.Text = comboBoxConformita.GetItemText(this.comboBoxConformita.SelectedValue).ToString().ToUpper();
            codConformita = comboBoxConformita.GetItemText(this.comboBoxConformita.SelectedValue).ToString();
            //descConformita = comboBoxConformita.GetItemText(this.comboBoxConformita.SelectedIndex).ToString();
        }
        private void popolaComboConformita()
        {
            string query = "set nocount on; select ltrim(rtrim(DRKY)) as CodConformita, ltrim(rtrim(DRDL01)) as Conformita from PRODCTL.F0005 with(nolock) where DRSY = '55RC' and DRRT = 'LG'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();

            da.Fill(dt);

            comboBoxConformita.ValueMember = "CodConformita";
            comboBoxConformita.DisplayMember = "Conformita";
            comboBoxConformita.DataSource = dt;
        }
        #endregion

        #region ComboBox Nazione
        private void comboBoxNazione_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxNazione.SelectedItem)
            {
                case "IT - Italia":
                    codNazione = "IT";
                    descNazione = "Italia";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.italy;
                    break;
                case "DE - Germania":
                    codNazione = "DE";
                    descNazione = "Germania";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.germany;
                    break;
                case "DK - Danimarca":
                    codNazione = "DK";
                    descNazione = "Danimarca";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.denmark;
                    break;
                case "NL - Olanda":
                    codNazione = "NL";
                    descNazione = "Olanda";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.netherlands;
                    break;
                case "ES - Spagna":
                    codNazione = "ES";
                    descNazione = "Spagna";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.spain;
                    break;
                case "FR - Francia":
                    codNazione = "FR";
                    descNazione = "Francia";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.france;
                    break;
                case "IE - Irlanda":
                    codNazione = "IE";
                    descNazione = "Irlanda";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.ireland;
                    break;
                case "AT - Austria":
                    codNazione = "AT";
                    descNazione = "Austria";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.austria;
                    break;
                case "BE - Belgio":
                    codNazione = "BE";
                    descNazione = "Belgio";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.belgium;
                    break;
                case "GB - Gran Bretagna":
                    codNazione = "GB";
                    descNazione = "Gran Bretagna";
                    pictureBoxNazione.Visible = true;
                    pictureBoxNazione.Image = Properties.Resources.gb_uk;
                    break;
                default:
                    codNazione = "";
                    descNazione = "";
                    pictureBoxNazione.Visible = false;
                    break;
            }
            pictureBoxNazione.Visible = true;
        }
        #endregion

        #region ComboBox TipoCarne
        private void comboBoxTipoCarne_SelectedIndexChanged(object sender, EventArgs e)
        {
            int giorniCNG = 0;
            int giorniFRE = 0;
            switch (comboBoxTipoCarne.SelectedItem)
            {
                case "FRE":
                    giorniFRE = CalcoloGiorniScadenzaCarneFresca();
                    dtpScadenza.Value = DateTime.Now.AddDays(giorniFRE).Date;
                    tipoCarne = "FRE";
                    descTipoCarne = "Carne Fresca";
                    lblTipoCarne_2.Visible = true;
                    lblTipoCarne_2.Text = "Carne\nFresca";
                    break;
                case "CNG":
                    giorniCNG = CalcoloGiorniScadenzaCarneCongelata();
                    dtpScadenza.Value = DateTime.Now.AddDays(giorniCNG).Date;
                    tipoCarne = "CNG";
                    descTipoCarne = "Carne Congelata";
                    lblTipoCarne_2.Visible = true;
                    lblTipoCarne_2.Text = "Carne\nCongelata";
                    break;
                default:
                    tipoCarne = "";
                    descTipoCarne = "";
                    lblTipoCarne_2.Text = "";
                    break;
            }
        }
        #endregion

        #region Bottoni (Azioni)
        private void btnChiudi_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStampa_Click(object sender, EventArgs e)
        {
            // controllo obbligatorietà dei campi da compilare
            if (tbLottoEti.Text == "" || tbPesoNetto.Text == "" || tbTara.Text == "" || tbProgCollo.Text == "" || tbTotColli.Text ==""
                || comboBoxFornitoriCarne.SelectedIndex == 0 || comboBoxArticoli.SelectedIndex == -1 || comboBoxNazione.SelectedIndex == -1 || comboBoxConformita.SelectedIndex == -1 || comboBoxTipoCarne.SelectedIndex == -1
                || dtpRicevimento.Value == null || dtpScadenza.Value == null)
            {
                MessageBox.Show("Compilare tutti i campi contrassegnati con asterisco (*)","",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (MessageBox.Show("HAI CONTROLLATO BENE I DATI INSERITI?", "ATTENZIONE!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // la stringa che compone il BARCODE deve avere lunghezza fissa a 24 caratteri => nelle tabelle delle PESATE e delle ETICHETTE
                    StringBuilder _sb = new StringBuilder();
                    _sb.Append(string.Format("{0,6}", codArticolo.Substring(0, 6)));
                    _sb.Append(string.Format("{0,12}", tbLottoEti.Text.Substring(0, 12)));
                    _sb.Append(string.Format("{0,4}", tbPesoNetto.Text.PadLeft(4,'0')));
                    _sb.Append(string.Format("{0,2}", InserisciCifraZero(Convert.ToInt32(tbTotColli.Text))).Substring(0, 2));
                    etichettaBarcode = _sb.Replace(" ", string.Empty).ToString();
                    etichettaBarcode.Substring(0, 24).TrimEnd();

                    //chiamo la funzione che ricava la data giuliana, e le tre funzioni che ricavano i dati da inserire nella F554312Z 
                    RicavaDataGiuliana();
                    LeggiDatiFornitoreDaDatabase();
                    LeggiDatiArticoloDaDatabase();
                    LeggiDatiConformitaDaDatabase();

                    // creo la chiave univoca concatenando data e ora del semaforo (valori fissi) + timestamp GG/MM/MIN/SEC di adesso + num. stazione, fino a 25 caratteri
                    string timestamp_unique = null;
                    string timeStampAdesso = InserisciCifraZero(DateTime.Now.Day) + InserisciCifraZero(DateTime.Now.Month) + InserisciCifraZero(DateTime.Now.Minute) + InserisciCifraZero(DateTime.Now.Second);
                    timestamp_unique = julianDate + "000000" + "_" + timeStampAdesso + "_" + stazionePC.Substring(stazionePC.Length - 1);

                    // preparo transazione e statement/istruzione SQL di insert
                    SqlTransaction trans = null;
                    string sql_stmt =
                            "set nocount on;" +
                            "insert into " + schemaDB + "F554312Z (PZBHAPPL,PZBHVER,PZUSER,PZUPMJ,PZUPMT,PZURCD,PZURAB,PZURDT,PZURRF,PZURAT,PZEDSP,PZSTATUS,PZPIND,PZ55ETID,PZ55STZP,PZ55STPP,PZAN8,PZALPH,PZITM,PZLITM,PZDSC1,PZDSC2,PZSHCM,PZ55DSSHCM,PZPRP9,PZ55DSPRP9,PZLOTG,PZ55DSLOTG,PZORIG,PZ55DSORIG,PZRCD,PZ55DSRCD,PZ55SZRCDJ,PZRLOT,PZLOTN,PZ55SZMMEJ,PZ55NCOL,PZ55TCLL,PZ55TARA,PZ55PNET,PZ55ETBC1,PZ55ETBC2,PZ55ETBC3) " +
                            "values(@BHAPPL,@BHVER,@USER,@UPMJ,@UPMT,@URCD,@URAB,@URDT,@URRF,@URAT,@EDSP,@STATUS,@PIND,@55ETID,@55STZP,@55STPP,@AN8,@ALPH,@ITM,@LITM,@DSC1,@DSC2,@SHCM,@55DSSHCM,@PRP9,@55DSPRP9,@LOTG,@55DSLOTG,@ORIG,@55DSORIG,@RCD,@55DSRCD,@55SZRCDJ,@RLOT,@LOTN,@55SZMMEJ,@55NCOL,@55TCLL,@55TARA,@55PNET,@55ETBC1,@55ETBC2,@55ETBC3)";

                    // parte il vero e proprio blocco di INSERT nel DB
                    using (SqlConnection cnn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            cnn.Open();

                            SqlCommand cmd = new SqlCommand(sql_stmt, cnn);

                            cmd.Parameters.Add("@BHAPPL", SqlDbType.VarChar).Value = "AppPeso";
                            cmd.Parameters.Add("@BHVER", SqlDbType.VarChar).Value = "InsManuale";// inserimento dati manuale (max 10 caratteri!)
                            cmd.Parameters.Add("@USER", SqlDbType.VarChar).Value = Environment.UserName.ToUpper();
                            cmd.Parameters.Add("@UPMJ", SqlDbType.VarChar).Value = julianDate;
                            cmd.Parameters.Add("@UPMT", SqlDbType.Float).Value = 0;
                            cmd.Parameters.Add("@URCD", SqlDbType.VarChar).Value = "";
                            cmd.Parameters.Add("@URAB", SqlDbType.Float).Value = 0;
                            cmd.Parameters.Add("@URDT", SqlDbType.VarChar).Value = 0;
                            cmd.Parameters.Add("@URRF", SqlDbType.VarChar).Value = "";
                            cmd.Parameters.Add("@URAT", SqlDbType.Float).Value = 0;
                            cmd.Parameters.Add("@EDSP", SqlDbType.Char).Value = ""; // blank sempre
                            cmd.Parameters.Add("@STATUS", SqlDbType.Char).Value = "";
                            cmd.Parameters.Add("@PIND", SqlDbType.Char).Value = "P";
                            cmd.Parameters.Add("@55ETID", SqlDbType.VarChar).Value = timestamp_unique;
                            cmd.Parameters.Add("@55STZP", SqlDbType.VarChar).Value = stazionePC;
                            cmd.Parameters.Add("@55STPP", SqlDbType.VarChar).Value = stampante + " " + stazionePC; // concateno la radice della stampante alla stazione di lavoro
                            cmd.Parameters.Add("@AN8", SqlDbType.Float).Value = codFornitore;
                            cmd.Parameters.Add("@ALPH", SqlDbType.VarChar).Value = descFornitore;
                            cmd.Parameters.Add("@ITM", SqlDbType.Float).Value = codArticoloCorto;
                            cmd.Parameters.Add("@LITM", SqlDbType.VarChar).Value = codArticolo;
                            cmd.Parameters.Add("@DSC1", SqlDbType.VarChar).Value = descArticolo;
                            cmd.Parameters.Add("@DSC2", SqlDbType.VarChar).Value = "";
                            cmd.Parameters.Add("@SHCM", SqlDbType.VarChar).Value = tipoCarne;
                            cmd.Parameters.Add("@55DSSHCM", SqlDbType.VarChar).Value = descTipoCarne;
                            cmd.Parameters.Add("@PRP9", SqlDbType.VarChar).Value = codIGP;
                            cmd.Parameters.Add("@55DSPRP9", SqlDbType.VarChar).Value = descIGP;
                            cmd.Parameters.Add("@LOTG", SqlDbType.VarChar).Value = codConformita.ToUpper();
                            cmd.Parameters.Add("@55DSLOTG", SqlDbType.VarChar).Value = descConformita;
                            cmd.Parameters.Add("@ORIG", SqlDbType.VarChar).Value = codNazione;
                            cmd.Parameters.Add("@55DSORIG", SqlDbType.VarChar).Value = descNazione;
                            cmd.Parameters.Add("@RCD", SqlDbType.VarChar).Value = "";
                            cmd.Parameters.Add("@55DSRCD", SqlDbType.VarChar).Value = "";
                            cmd.Parameters.Add("@55SZRCDJ", SqlDbType.VarChar).Value = dtpRicevimento.Value.Date.ToString("dd/MM/yyyy");
                            cmd.Parameters.Add("@RLOT", SqlDbType.VarChar).Value = "";
                            cmd.Parameters.Add("@LOTN", SqlDbType.VarChar).Value = tbLottoEti.Text;
                            cmd.Parameters.Add("@55SZMMEJ", SqlDbType.VarChar).Value = dtpScadenza.Value.Date.ToString("dd/MM/yyyy");
                            cmd.Parameters.Add("@55NCOL", SqlDbType.Float).Value = Convert.ToSingle(tbProgCollo.Text);
                            cmd.Parameters.Add("@55TCLL", SqlDbType.Float).Value = Convert.ToSingle(tbTotColli.Text);
                            cmd.Parameters.Add("@55TARA", SqlDbType.Float).Value = Convert.ToSingle(tbTara.Text);
                            cmd.Parameters.Add("@55PNET", SqlDbType.Float).Value = Convert.ToSingle(tbPesoNetto.Text);
                            cmd.Parameters.Add("@55ETBC1", SqlDbType.VarChar).Value = etichettaBarcode;
                            cmd.Parameters.Add("@55ETBC2", SqlDbType.VarChar).Value = "";
                            cmd.Parameters.Add("@55ETBC3", SqlDbType.VarChar).Value = "";

                            trans = cnn.BeginTransaction();
                            cmd.Transaction = trans;

                            try
                            {
                                cmd.ExecuteNonQuery();
                                trans.Commit();
                                cnn.Close();
                                MessageBox.Show("Dati inseriti in coda di stampa!\nAttendi per prelevare la stampa in corso...", "STAMPA CORRETTA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                // this.Close(); // non chiudo piu il form tck#1591
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                cnn.Close();
                                MessageBox.Show("Scrittura su database etichette fallita!\n\n" + ex.Message + "\n\nRIPETERE OPERAZIONE", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("ERRORE GENERICO SCRITTURA DATABASE:" + ex.Message);
                        }
                    }
                }
            }
        }
        #endregion

        #region Funzioni varie
        // non permetto di inserire valori alfabetici nei campi numerici (Lotto, Pesi vari, Colli, ecc...)
        private void tbProgCollo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void tbTotColli_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void tbPesoNetto_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void tbTara_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void tbLottoEti_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private string InserisciCifraZero(int n)
        {
            return (n < 10 ? "0" : "") + n;
        }

        private int LeggiDatiFornitoreDaDatabase()
        {
            string sql_descFornitore = "set nocount on; select ltrim(rtrim(ABALPH)) as descFornitore from " + schemaDB + "F0101 with(nolock) where ABAN8 = " + codFornitore;

            using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql_descFornitore, cnn))
            {
                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            descFornitore = Convert.ToString(reader["descFornitore"]).Trim();
                        }
                        return 1;
                    }
                    else
                        return 0;
                }

            }

        }

        private int LeggiDatiArticoloDaDatabase()
        {
            string sql_Articolo = "set nocount on; select IMITM, ltrim(rtrim(IMDSC1)) as descArticolo from " + schemaDB + "F4101 with(nolock) where IMLITM = '" + codArticolo +"'";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql_Articolo, cnn))
            {
                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            codArticoloCorto = Convert.ToInt32(reader["IMITM"]);
                            descArticolo = Convert.ToString(reader["descArticolo"]).Trim();
                        }
                        return 1;
                    }
                    else
                        return 0;
                }
            }
        }

        private int LeggiDatiConformitaDaDatabase()
        {
            string sql_Conformita = "set nocount on; select ltrim(rtrim(DRDL01)) as descConformita from PRODCTL.F0005 with(nolock) where DRSY = '55RC' and DRRT = 'LG' and Ltrim(Rtrim(DRKY)) = '" + codConformita+"'";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql_Conformita, cnn))
            {
                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            descConformita = Convert.ToString(reader["descConformita"]).Trim();
                        }
                        return 1;
                    }
                    else
                        return 0;
                }
            }
        }

        private string RicavaDataGiuliana()
        {
            string sql_Julian = "set nocount on; select dbo.DateToJulian(convert(varchar(10), getdate(), 112)) as JulianDate";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql_Julian, cnn))
            {
                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            julianDate = Convert.ToString(reader["JulianDate"]).Trim();
                        }
                        return julianDate;
                    }
                    else
                        return "";
                }
            }
        }


        private int CalcoloGiorniScadenzaCarneCongelata()
        {
            string sql_giorniScadenzaCarneCongelata = "set nocount on; select IMSLD from " + schemaDB + "F4101 with(nolock) where IMLITM = '" + codArticolo + "'";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql_giorniScadenzaCarneCongelata, cnn))
            {
                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            giorniScadenza = Convert.ToInt32(reader["IMSLD"]);
                        }
                        return giorniScadenza;
                    }
                    else
                        return 0;
                }

            }

        }

        private int CalcoloGiorniScadenzaCarneFresca()
        {
            string sql_giorniScadenzaCarneFresca = "set nocount on; select IMSRP9 from " + schemaDB + "F4101 with(nolock) where IMLITM = '" + codArticolo + "'";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql_giorniScadenzaCarneFresca, cnn))
            {
                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            giorniScadenza = Convert.ToInt32(reader["IMSRP9"]);
                        }
                        return giorniScadenza;
                    }
                    else
                        return 0;
                }

            }

        }
        #endregion

        #region CheckBox IGP
        private void checkBoxIGP_CheckedChanged(object sender, EventArgs e)
        {
            if(this.checkBoxIGP.Checked == true)
            {
                lblChkIGP.Visible = true;
                codIGP = "IGP";
                descIGP = "I.G.P.";
            }
            else
            {
                lblChkIGP.Visible = false;
                codIGP = "";
                descIGP = "";
            }
        }
        #endregion
    }
}

