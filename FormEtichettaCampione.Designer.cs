namespace SerialPortListener
{
    partial class FormEtichettaCampione
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblFornitore = new System.Windows.Forms.Label();
            this.comboBoxFornitoriCarne = new System.Windows.Forms.ComboBox();
            this.lblCodFornitore = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblArticolo = new System.Windows.Forms.Label();
            this.comboBoxArticoli = new System.Windows.Forms.ComboBox();
            this.lblCodArticolo = new System.Windows.Forms.Label();
            this.tbLottoEti = new System.Windows.Forms.TextBox();
            this.lblLottoEti = new System.Windows.Forms.Label();
            this.lblProgCollo = new System.Windows.Forms.Label();
            this.lblTotColli = new System.Windows.Forms.Label();
            this.tbProgCollo = new System.Windows.Forms.TextBox();
            this.tbTotColli = new System.Windows.Forms.TextBox();
            this.dtpScadenza = new System.Windows.Forms.DateTimePicker();
            this.lblDataScad = new System.Windows.Forms.Label();
            this.btnStampa = new System.Windows.Forms.Button();
            this.btnChiudi = new System.Windows.Forms.Button();
            this.comboBoxNazione = new System.Windows.Forms.ComboBox();
            this.lblNazione = new System.Windows.Forms.Label();
            this.pictureBoxNazione = new System.Windows.Forms.PictureBox();
            this.tbPesoNetto = new System.Windows.Forms.TextBox();
            this.lblPesoNetto = new System.Windows.Forms.Label();
            this.tbTara = new System.Windows.Forms.TextBox();
            this.lblTara = new System.Windows.Forms.Label();
            this.comboBoxTipoCarne = new System.Windows.Forms.ComboBox();
            this.lblTipoCarne = new System.Windows.Forms.Label();
            this.lblTipoCarne_2 = new System.Windows.Forms.Label();
            this.dtpRicevimento = new System.Windows.Forms.DateTimePicker();
            this.lblDataRic = new System.Windows.Forms.Label();
            this.checkBoxIGP = new System.Windows.Forms.CheckBox();
            this.lblFlagIGP = new System.Windows.Forms.Label();
            this.lblChkIGP = new System.Windows.Forms.Label();
            this.lblConformita = new System.Windows.Forms.Label();
            this.comboBoxConformita = new System.Windows.Forms.ComboBox();
            this.lblCodConformita = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNazione)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFornitore
            // 
            this.lblFornitore.AutoSize = true;
            this.lblFornitore.Location = new System.Drawing.Point(12, 15);
            this.lblFornitore.Name = "lblFornitore";
            this.lblFornitore.Size = new System.Drawing.Size(52, 13);
            this.lblFornitore.TabIndex = 1;
            this.lblFornitore.Text = "Fornitore*";
            // 
            // comboBoxFornitoriCarne
            // 
            this.comboBoxFornitoriCarne.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFornitoriCarne.FormattingEnabled = true;
            this.comboBoxFornitoriCarne.Location = new System.Drawing.Point(66, 12);
            this.comboBoxFornitoriCarne.Name = "comboBoxFornitoriCarne";
            this.comboBoxFornitoriCarne.Size = new System.Drawing.Size(245, 21);
            this.comboBoxFornitoriCarne.TabIndex = 2;
            this.comboBoxFornitoriCarne.SelectedIndexChanged += new System.EventHandler(this.comboBoxFornitoriCarne_SelectedIndexChanged);
            // 
            // lblCodFornitore
            // 
            this.lblCodFornitore.AutoSize = true;
            this.lblCodFornitore.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodFornitore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblCodFornitore.Location = new System.Drawing.Point(328, 15);
            this.lblCodFornitore.Name = "lblCodFornitore";
            this.lblCodFornitore.Size = new System.Drawing.Size(72, 14);
            this.lblCodFornitore.TabIndex = 3;
            this.lblCodFornitore.Text = "cod Fornitore";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SerialPortListener.Properties.Resources.etichettacolli;
            this.pictureBox1.Location = new System.Drawing.Point(297, 309);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(127, 130);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // lblArticolo
            // 
            this.lblArticolo.AutoSize = true;
            this.lblArticolo.Location = new System.Drawing.Point(12, 55);
            this.lblArticolo.Name = "lblArticolo";
            this.lblArticolo.Size = new System.Drawing.Size(46, 13);
            this.lblArticolo.TabIndex = 5;
            this.lblArticolo.Text = "Articolo*";
            // 
            // comboBoxArticoli
            // 
            this.comboBoxArticoli.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxArticoli.FormattingEnabled = true;
            this.comboBoxArticoli.Location = new System.Drawing.Point(66, 55);
            this.comboBoxArticoli.Name = "comboBoxArticoli";
            this.comboBoxArticoli.Size = new System.Drawing.Size(245, 21);
            this.comboBoxArticoli.TabIndex = 6;
            this.comboBoxArticoli.SelectedIndexChanged += new System.EventHandler(this.comboBoxArticoli_SelectedIndexChanged);
            // 
            // lblCodArticolo
            // 
            this.lblCodArticolo.AutoSize = true;
            this.lblCodArticolo.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodArticolo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblCodArticolo.Location = new System.Drawing.Point(328, 58);
            this.lblCodArticolo.Name = "lblCodArticolo";
            this.lblCodArticolo.Size = new System.Drawing.Size(65, 14);
            this.lblCodArticolo.TabIndex = 7;
            this.lblCodArticolo.Text = "cod Articolo";
            // 
            // tbLottoEti
            // 
            this.tbLottoEti.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLottoEti.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tbLottoEti.Location = new System.Drawing.Point(66, 98);
            this.tbLottoEti.Name = "tbLottoEti";
            this.tbLottoEti.Size = new System.Drawing.Size(100, 22);
            this.tbLottoEti.TabIndex = 8;
            this.tbLottoEti.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbLottoEti_KeyPress);
            // 
            // lblLottoEti
            // 
            this.lblLottoEti.AutoSize = true;
            this.lblLottoEti.Location = new System.Drawing.Point(12, 101);
            this.lblLottoEti.Name = "lblLottoEti";
            this.lblLottoEti.Size = new System.Drawing.Size(35, 13);
            this.lblLottoEti.TabIndex = 9;
            this.lblLottoEti.Text = "Lotto*";
            // 
            // lblProgCollo
            // 
            this.lblProgCollo.AutoSize = true;
            this.lblProgCollo.Location = new System.Drawing.Point(265, 101);
            this.lblProgCollo.Name = "lblProgCollo";
            this.lblProgCollo.Size = new System.Drawing.Size(64, 13);
            this.lblProgCollo.TabIndex = 10;
            this.lblProgCollo.Text = "Progr. collo*";
            // 
            // lblTotColli
            // 
            this.lblTotColli.AutoSize = true;
            this.lblTotColli.Location = new System.Drawing.Point(265, 129);
            this.lblTotColli.Name = "lblTotColli";
            this.lblTotColli.Size = new System.Drawing.Size(62, 13);
            this.lblTotColli.TabIndex = 11;
            this.lblTotColli.Text = "Totale colli*";
            // 
            // tbProgCollo
            // 
            this.tbProgCollo.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbProgCollo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tbProgCollo.Location = new System.Drawing.Point(331, 98);
            this.tbProgCollo.Name = "tbProgCollo";
            this.tbProgCollo.Size = new System.Drawing.Size(84, 22);
            this.tbProgCollo.TabIndex = 12;
            this.tbProgCollo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbProgCollo_KeyPress);
            // 
            // tbTotColli
            // 
            this.tbTotColli.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTotColli.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tbTotColli.Location = new System.Drawing.Point(331, 126);
            this.tbTotColli.Name = "tbTotColli";
            this.tbTotColli.Size = new System.Drawing.Size(84, 22);
            this.tbTotColli.TabIndex = 13;
            this.tbTotColli.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTotColli_KeyPress);
            // 
            // dtpScadenza
            // 
            this.dtpScadenza.Cursor = System.Windows.Forms.Cursors.Default;
            this.dtpScadenza.Location = new System.Drawing.Point(66, 338);
            this.dtpScadenza.Name = "dtpScadenza";
            this.dtpScadenza.Size = new System.Drawing.Size(200, 20);
            this.dtpScadenza.TabIndex = 14;
            // 
            // lblDataScad
            // 
            this.lblDataScad.AutoSize = true;
            this.lblDataScad.Location = new System.Drawing.Point(12, 338);
            this.lblDataScad.Name = "lblDataScad";
            this.lblDataScad.Size = new System.Drawing.Size(40, 26);
            this.lblDataScad.TabIndex = 15;
            this.lblDataScad.Text = "Data\r\nscad. *";
            // 
            // btnStampa
            // 
            this.btnStampa.Image = global::SerialPortListener.Properties.Resources.printer;
            this.btnStampa.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStampa.Location = new System.Drawing.Point(12, 379);
            this.btnStampa.Name = "btnStampa";
            this.btnStampa.Size = new System.Drawing.Size(100, 60);
            this.btnStampa.TabIndex = 16;
            this.btnStampa.Text = "Stampa";
            this.btnStampa.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnStampa.UseVisualStyleBackColor = true;
            this.btnStampa.Click += new System.EventHandler(this.btnStampa_Click);
            // 
            // btnChiudi
            // 
            this.btnChiudi.Image = global::SerialPortListener.Properties.Resources.exit;
            this.btnChiudi.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnChiudi.Location = new System.Drawing.Point(166, 379);
            this.btnChiudi.Name = "btnChiudi";
            this.btnChiudi.Size = new System.Drawing.Size(100, 60);
            this.btnChiudi.TabIndex = 17;
            this.btnChiudi.Text = "Chiudi";
            this.btnChiudi.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnChiudi.UseVisualStyleBackColor = true;
            this.btnChiudi.Click += new System.EventHandler(this.btnChiudi_Click);
            // 
            // comboBoxNazione
            // 
            this.comboBoxNazione.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNazione.FormattingEnabled = true;
            this.comboBoxNazione.Items.AddRange(new object[] {
            "IT - Italia",
            "DE - Germania",
            "DK - Danimarca",
            "NL - Olanda",
            "ES - Spagna",
            "FR - Francia",
            "IE - Irlanda",
            "AT - Austria",
            "BE - Belgio",
            "GB - Gran Bretagna"});
            this.comboBoxNazione.Location = new System.Drawing.Point(66, 141);
            this.comboBoxNazione.Name = "comboBoxNazione";
            this.comboBoxNazione.Size = new System.Drawing.Size(100, 21);
            this.comboBoxNazione.TabIndex = 18;
            this.comboBoxNazione.SelectedIndexChanged += new System.EventHandler(this.comboBoxNazione_SelectedIndexChanged);
            // 
            // lblNazione
            // 
            this.lblNazione.AutoSize = true;
            this.lblNazione.Location = new System.Drawing.Point(12, 144);
            this.lblNazione.Name = "lblNazione";
            this.lblNazione.Size = new System.Drawing.Size(50, 13);
            this.lblNazione.TabIndex = 19;
            this.lblNazione.Text = "Nazione*";
            // 
            // pictureBoxNazione
            // 
            this.pictureBoxNazione.Image = global::SerialPortListener.Properties.Resources.austria;
            this.pictureBoxNazione.Location = new System.Drawing.Point(173, 145);
            this.pictureBoxNazione.Name = "pictureBoxNazione";
            this.pictureBoxNazione.Size = new System.Drawing.Size(25, 21);
            this.pictureBoxNazione.TabIndex = 20;
            this.pictureBoxNazione.TabStop = false;
            this.pictureBoxNazione.Visible = false;
            // 
            // tbPesoNetto
            // 
            this.tbPesoNetto.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPesoNetto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tbPesoNetto.Location = new System.Drawing.Point(331, 179);
            this.tbPesoNetto.Name = "tbPesoNetto";
            this.tbPesoNetto.Size = new System.Drawing.Size(84, 22);
            this.tbPesoNetto.TabIndex = 21;
            this.tbPesoNetto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPesoNetto_KeyPress);
            // 
            // lblPesoNetto
            // 
            this.lblPesoNetto.AutoSize = true;
            this.lblPesoNetto.Location = new System.Drawing.Point(269, 182);
            this.lblPesoNetto.Name = "lblPesoNetto";
            this.lblPesoNetto.Size = new System.Drawing.Size(62, 13);
            this.lblPesoNetto.TabIndex = 22;
            this.lblPesoNetto.Text = "Peso netto*";
            // 
            // tbTara
            // 
            this.tbTara.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tbTara.Location = new System.Drawing.Point(331, 206);
            this.tbTara.Name = "tbTara";
            this.tbTara.Size = new System.Drawing.Size(84, 22);
            this.tbTara.TabIndex = 23;
            this.tbTara.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTara_KeyPress);
            // 
            // lblTara
            // 
            this.lblTara.AutoSize = true;
            this.lblTara.Location = new System.Drawing.Point(294, 209);
            this.lblTara.Name = "lblTara";
            this.lblTara.Size = new System.Drawing.Size(33, 13);
            this.lblTara.TabIndex = 24;
            this.lblTara.Text = "Tara*";
            // 
            // comboBoxTipoCarne
            // 
            this.comboBoxTipoCarne.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTipoCarne.FormattingEnabled = true;
            this.comboBoxTipoCarne.Items.AddRange(new object[] {
            "",
            "FRE",
            "CNG"});
            this.comboBoxTipoCarne.Location = new System.Drawing.Point(66, 180);
            this.comboBoxTipoCarne.Name = "comboBoxTipoCarne";
            this.comboBoxTipoCarne.Size = new System.Drawing.Size(100, 21);
            this.comboBoxTipoCarne.TabIndex = 25;
            this.comboBoxTipoCarne.SelectedIndexChanged += new System.EventHandler(this.comboBoxTipoCarne_SelectedIndexChanged);
            // 
            // lblTipoCarne
            // 
            this.lblTipoCarne.AutoSize = true;
            this.lblTipoCarne.Location = new System.Drawing.Point(12, 179);
            this.lblTipoCarne.Name = "lblTipoCarne";
            this.lblTipoCarne.Size = new System.Drawing.Size(38, 26);
            this.lblTipoCarne.TabIndex = 26;
            this.lblTipoCarne.Text = "Tipo\r\ncarne*";
            // 
            // lblTipoCarne_2
            // 
            this.lblTipoCarne_2.AutoSize = true;
            this.lblTipoCarne_2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipoCarne_2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTipoCarne_2.Location = new System.Drawing.Point(173, 182);
            this.lblTipoCarne_2.Name = "lblTipoCarne_2";
            this.lblTipoCarne_2.Size = new System.Drawing.Size(19, 14);
            this.lblTipoCarne_2.TabIndex = 27;
            this.lblTipoCarne_2.Text = "TC";
            this.lblTipoCarne_2.Visible = false;
            // 
            // dtpRicevimento
            // 
            this.dtpRicevimento.Location = new System.Drawing.Point(66, 309);
            this.dtpRicevimento.Name = "dtpRicevimento";
            this.dtpRicevimento.Size = new System.Drawing.Size(200, 20);
            this.dtpRicevimento.TabIndex = 28;
            // 
            // lblDataRic
            // 
            this.lblDataRic.AutoSize = true;
            this.lblDataRic.Location = new System.Drawing.Point(12, 303);
            this.lblDataRic.Name = "lblDataRic";
            this.lblDataRic.Size = new System.Drawing.Size(50, 26);
            this.lblDataRic.TabIndex = 29;
            this.lblDataRic.Text = "Data \r\nricevim. *";
            // 
            // checkBoxIGP
            // 
            this.checkBoxIGP.AutoSize = true;
            this.checkBoxIGP.Location = new System.Drawing.Point(66, 222);
            this.checkBoxIGP.Name = "checkBoxIGP";
            this.checkBoxIGP.Size = new System.Drawing.Size(57, 17);
            this.checkBoxIGP.TabIndex = 30;
            this.checkBoxIGP.Text = "SI/NO";
            this.checkBoxIGP.UseVisualStyleBackColor = true;
            this.checkBoxIGP.CheckedChanged += new System.EventHandler(this.checkBoxIGP_CheckedChanged);
            // 
            // lblFlagIGP
            // 
            this.lblFlagIGP.AutoSize = true;
            this.lblFlagIGP.Location = new System.Drawing.Point(12, 222);
            this.lblFlagIGP.Name = "lblFlagIGP";
            this.lblFlagIGP.Size = new System.Drawing.Size(48, 13);
            this.lblFlagIGP.TabIndex = 31;
            this.lblFlagIGP.Text = "Flag IGP";
            // 
            // lblChkIGP
            // 
            this.lblChkIGP.AutoSize = true;
            this.lblChkIGP.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChkIGP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblChkIGP.Location = new System.Drawing.Point(147, 223);
            this.lblChkIGP.Name = "lblChkIGP";
            this.lblChkIGP.Size = new System.Drawing.Size(24, 14);
            this.lblChkIGP.TabIndex = 32;
            this.lblChkIGP.Text = "IGP";
            this.lblChkIGP.Visible = false;
            // 
            // lblConformita
            // 
            this.lblConformita.AutoSize = true;
            this.lblConformita.Location = new System.Drawing.Point(12, 265);
            this.lblConformita.Name = "lblConformita";
            this.lblConformita.Size = new System.Drawing.Size(56, 13);
            this.lblConformita.TabIndex = 33;
            this.lblConformita.Text = "Conform. *";
            // 
            // comboBoxConformita
            // 
            this.comboBoxConformita.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxConformita.FormattingEnabled = true;
            this.comboBoxConformita.Location = new System.Drawing.Point(66, 262);
            this.comboBoxConformita.Name = "comboBoxConformita";
            this.comboBoxConformita.Size = new System.Drawing.Size(245, 21);
            this.comboBoxConformita.TabIndex = 34;
            this.comboBoxConformita.SelectedIndexChanged += new System.EventHandler(this.comboBoxConformita_SelectedIndexChanged);
            // 
            // lblCodConformita
            // 
            this.lblCodConformita.AutoSize = true;
            this.lblCodConformita.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodConformita.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblCodConformita.Location = new System.Drawing.Point(328, 265);
            this.lblCodConformita.Name = "lblCodConformita";
            this.lblCodConformita.Size = new System.Drawing.Size(75, 14);
            this.lblCodConformita.TabIndex = 35;
            this.lblCodConformita.Text = "CONFORMITA";
            // 
            // FormEtichettaCampione
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 451);
            this.Controls.Add(this.lblCodConformita);
            this.Controls.Add(this.comboBoxConformita);
            this.Controls.Add(this.lblConformita);
            this.Controls.Add(this.lblChkIGP);
            this.Controls.Add(this.lblFlagIGP);
            this.Controls.Add(this.checkBoxIGP);
            this.Controls.Add(this.lblDataRic);
            this.Controls.Add(this.dtpRicevimento);
            this.Controls.Add(this.lblTipoCarne_2);
            this.Controls.Add(this.lblTipoCarne);
            this.Controls.Add(this.comboBoxTipoCarne);
            this.Controls.Add(this.lblTara);
            this.Controls.Add(this.tbTara);
            this.Controls.Add(this.lblPesoNetto);
            this.Controls.Add(this.tbPesoNetto);
            this.Controls.Add(this.pictureBoxNazione);
            this.Controls.Add(this.lblNazione);
            this.Controls.Add(this.comboBoxNazione);
            this.Controls.Add(this.btnChiudi);
            this.Controls.Add(this.btnStampa);
            this.Controls.Add(this.lblDataScad);
            this.Controls.Add(this.dtpScadenza);
            this.Controls.Add(this.tbTotColli);
            this.Controls.Add(this.tbProgCollo);
            this.Controls.Add(this.lblTotColli);
            this.Controls.Add(this.lblProgCollo);
            this.Controls.Add(this.lblLottoEti);
            this.Controls.Add(this.tbLottoEti);
            this.Controls.Add(this.lblCodArticolo);
            this.Controls.Add(this.comboBoxArticoli);
            this.Controls.Add(this.lblArticolo);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblCodFornitore);
            this.Controls.Add(this.comboBoxFornitoriCarne);
            this.Controls.Add(this.lblFornitore);
            this.Name = "FormEtichettaCampione";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "appPeso \\\\ Crea e stampa etichetta campione";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNazione)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblFornitore;
        private System.Windows.Forms.ComboBox comboBoxFornitoriCarne;
        private System.Windows.Forms.Label lblCodFornitore;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblArticolo;
        private System.Windows.Forms.ComboBox comboBoxArticoli;
        private System.Windows.Forms.Label lblCodArticolo;
        private System.Windows.Forms.TextBox tbLottoEti;
        private System.Windows.Forms.Label lblLottoEti;
        private System.Windows.Forms.Label lblProgCollo;
        private System.Windows.Forms.Label lblTotColli;
        private System.Windows.Forms.TextBox tbProgCollo;
        private System.Windows.Forms.TextBox tbTotColli;
        private System.Windows.Forms.DateTimePicker dtpScadenza;
        private System.Windows.Forms.Label lblDataScad;
        private System.Windows.Forms.Button btnStampa;
        private System.Windows.Forms.Button btnChiudi;
        private System.Windows.Forms.ComboBox comboBoxNazione;
        private System.Windows.Forms.Label lblNazione;
        private System.Windows.Forms.PictureBox pictureBoxNazione;
        private System.Windows.Forms.TextBox tbPesoNetto;
        private System.Windows.Forms.Label lblPesoNetto;
        private System.Windows.Forms.TextBox tbTara;
        private System.Windows.Forms.Label lblTara;
        private System.Windows.Forms.ComboBox comboBoxTipoCarne;
        private System.Windows.Forms.Label lblTipoCarne;
        private System.Windows.Forms.Label lblTipoCarne_2;
        private System.Windows.Forms.DateTimePicker dtpRicevimento;
        private System.Windows.Forms.Label lblDataRic;
        private System.Windows.Forms.CheckBox checkBoxIGP;
        private System.Windows.Forms.Label lblFlagIGP;
        private System.Windows.Forms.Label lblChkIGP;
        private System.Windows.Forms.Label lblConformita;
        private System.Windows.Forms.ComboBox comboBoxConformita;
        private System.Windows.Forms.Label lblCodConformita;
    }
}