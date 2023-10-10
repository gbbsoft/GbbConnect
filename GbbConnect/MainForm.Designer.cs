namespace GbbConnect
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            groupBox2 = new GroupBox();
            textBox4 = new TextBox();
            plantsBindingSource = new BindingSource(components);
            ParametersBindingSource = new BindingSource(components);
            label5 = new Label();
            textBox5 = new TextBox();
            textBox6 = new TextBox();
            label7 = new Label();
            label6 = new Label();
            bindingNavigator2 = new GbbLibWin.BindingNavigator();
            groupBox1 = new GroupBox();
            textBox3 = new TextBox();
            label3 = new Label();
            textBox1 = new TextBox();
            label1 = new Label();
            textBox2 = new TextBox();
            label4 = new Label();
            label2 = new Label();
            Plants_DataGridView = new GbbLibWin.OurDataGridView();
            InverterNumber = new DataGridViewTextBoxColumn();
            nameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            Inverter = new GbbLibWin.OurDataGridViewComboBoxColumn2();
            inverterInfoBindingSource = new BindingSource(components);
            IsDisabled = new DataGridViewCheckBoxColumn();
            tabPage3 = new TabPage();
            groupBox4 = new GroupBox();
            checkBox1 = new CheckBox();
            groupBox3 = new GroupBox();
            label9 = new Label();
            textBox8 = new TextBox();
            textBox9 = new TextBox();
            label10 = new Label();
            Log_tabPage2 = new TabPage();
            checkBox2 = new CheckBox();
            DriverLog_checkBox = new CheckBox();
            label12 = new Label();
            RegisterCount_numericUpDown = new NumericUpDown();
            Clear_button = new Button();
            VerboseLog_checkBox = new CheckBox();
            label11 = new Label();
            RegisterNo_numericUpDown = new NumericUpDown();
            Search_button = new Button();
            TestSolarmanV5_button = new Button();
            Log_textBox = new TextBox();
            TestConnections_button = new Button();
            tabPage4 = new TabPage();
            textBox7 = new TextBox();
            About_label2 = new Label();
            linkLabel1 = new LinkLabel();
            label8 = new Label();
            Save_button = new Button();
            StartServer_button = new Button();
            StopServer_button = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            Version_label = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)plantsBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ParametersBindingSource).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Plants_DataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inverterInfoBindingSource).BeginInit();
            tabPage3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            Log_tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RegisterCount_numericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RegisterNo_numericUpDown).BeginInit();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(Log_tabPage2);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Location = new Point(12, 41);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(961, 524);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox2);
            tabPage1.Controls.Add(bindingNavigator2);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(Plants_DataGridView);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(953, 496);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Plants";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(textBox4);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(textBox5);
            groupBox2.Controls.Add(textBox6);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label6);
            groupBox2.Location = new Point(397, 215);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(385, 150);
            groupBox2.TabIndex = 8;
            groupBox2.TabStop = false;
            groupBox2.Text = "GbbVictronWeb:";
            // 
            // textBox4
            // 
            textBox4.DataBindings.Add(new Binding("Text", plantsBindingSource, "GbbVictronWeb_PlantToken", true));
            textBox4.Location = new Point(174, 80);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(178, 23);
            textBox4.TabIndex = 7;
            // 
            // plantsBindingSource
            // 
            plantsBindingSource.DataMember = "Plants";
            plantsBindingSource.DataSource = ParametersBindingSource;
            plantsBindingSource.AddingNew += plantsBindingSource_AddingNew;
            // 
            // ParametersBindingSource
            // 
            ParametersBindingSource.DataSource = typeof(GbbEngine.Configuration.Parameters);
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(5, 83);
            label5.Name = "label5";
            label5.Size = new Size(76, 15);
            label5.TabIndex = 6;
            label5.Text = "Plant Token*:";
            // 
            // textBox5
            // 
            textBox5.DataBindings.Add(new Binding("Text", plantsBindingSource, "GbbVictronWeb_PlantId", true));
            textBox5.Location = new Point(174, 51);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(178, 23);
            textBox5.TabIndex = 5;
            // 
            // textBox6
            // 
            textBox6.DataBindings.Add(new Binding("Text", plantsBindingSource, "GbbVictronWeb_UserEmail", true));
            textBox6.Location = new Point(174, 22);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(178, 23);
            textBox6.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(5, 25);
            label7.Name = "label7";
            label7.Size = new Size(70, 15);
            label7.TabIndex = 2;
            label7.Text = "User Email*:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 54);
            label6.Name = "label6";
            label6.Size = new Size(55, 15);
            label6.TabIndex = 8;
            label6.Text = "Plant Id*:";
            // 
            // bindingNavigator2
            // 
            bindingNavigator2.BindingSource = plantsBindingSource;
            bindingNavigator2.HideAddAndDelete = true;
            bindingNavigator2.Location = new Point(6, 184);
            bindingNavigator2.Name = "bindingNavigator2";
            bindingNavigator2.Size = new Size(272, 25);
            bindingNavigator2.TabIndex = 8;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBox3);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(label4);
            groupBox1.Location = new Point(6, 215);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(385, 150);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Inverter Logger:";
            // 
            // textBox3
            // 
            textBox3.DataBindings.Add(new Binding("Text", plantsBindingSource, "SerialNumber", true));
            textBox3.Location = new Point(174, 80);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(178, 23);
            textBox3.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(5, 83);
            label3.Name = "label3";
            label3.Size = new Size(153, 15);
            label3.TabIndex = 6;
            label3.Text = "Logger serial number (SN)*:";
            // 
            // textBox1
            // 
            textBox1.DataBindings.Add(new Binding("Text", plantsBindingSource, "PortNo", true));
            textBox1.Location = new Point(174, 51);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(178, 23);
            textBox1.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 54);
            label1.Name = "label1";
            label1.Size = new Size(82, 15);
            label1.TabIndex = 4;
            label1.Text = "Port number*:";
            // 
            // textBox2
            // 
            textBox2.DataBindings.Add(new Binding("Text", plantsBindingSource, "AddressIP", true));
            textBox2.Location = new Point(174, 22);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(178, 23);
            textBox2.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(5, 25);
            label4.Name = "label4";
            label4.Size = new Size(68, 15);
            label4.TabIndex = 2;
            label4.Text = "IP address*:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 13);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 6;
            label2.Text = "Plants:";
            // 
            // Plants_DataGridView
            // 
            Plants_DataGridView.AutoGenerateColumns = false;
            Plants_DataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Plants_DataGridView.Columns.AddRange(new DataGridViewColumn[] { InverterNumber, nameDataGridViewTextBoxColumn, Inverter, IsDisabled });
            Plants_DataGridView.DataSource = plantsBindingSource;
            Plants_DataGridView.Location = new Point(6, 31);
            Plants_DataGridView.Name = "Plants_DataGridView";
            Plants_DataGridView.OurPaste_BlockMultiLine = false;
            Plants_DataGridView.RowTemplate.Height = 25;
            Plants_DataGridView.Size = new Size(727, 150);
            Plants_DataGridView.TabIndex = 5;
            Plants_DataGridView.RowValidating += Plants_DataGridView_RowValidating;
            // 
            // InverterNumber
            // 
            InverterNumber.DataPropertyName = "Number";
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(255, 255, 192);
            InverterNumber.DefaultCellStyle = dataGridViewCellStyle1;
            InverterNumber.HeaderText = "No";
            InverterNumber.Name = "InverterNumber";
            InverterNumber.Width = 50;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            dataGridViewCellStyle2.BackColor = Color.FromArgb(255, 255, 192);
            nameDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            nameDataGridViewTextBoxColumn.HeaderText = "Name";
            nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            nameDataGridViewTextBoxColumn.Width = 200;
            // 
            // Inverter
            // 
            Inverter.DataPropertyName = "InverterNumber";
            Inverter.DataSource = inverterInfoBindingSource;
            Inverter.DisplayMember = "Name";
            Inverter.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            Inverter.HeaderText = "Inverter";
            Inverter.Name = "Inverter";
            Inverter.SortMode = DataGridViewColumnSortMode.Automatic;
            Inverter.ValueMember = "Number";
            Inverter.Width = 300;
            // 
            // inverterInfoBindingSource
            // 
            inverterInfoBindingSource.DataSource = typeof(GbbEngine.Inverters.InverterInfo);
            // 
            // IsDisabled
            // 
            IsDisabled.DataPropertyName = "IsDisabled";
            IsDisabled.FalseValue = "0";
            IsDisabled.HeaderText = "IsDisabled";
            IsDisabled.Name = "IsDisabled";
            IsDisabled.TrueValue = "1";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(groupBox4);
            tabPage3.Controls.Add(groupBox3);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(953, 496);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Parameters";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(checkBox1);
            groupBox4.Location = new Point(394, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(385, 150);
            groupBox4.TabIndex = 10;
            groupBox4.TabStop = false;
            groupBox4.Text = "Server";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.DataBindings.Add(new Binding("Checked", ParametersBindingSource, "Server_AutoStart", true));
            checkBox1.Location = new Point(6, 22);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(191, 19);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Start server after program starts";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label9);
            groupBox3.Controls.Add(textBox8);
            groupBox3.Controls.Add(textBox9);
            groupBox3.Controls.Add(label10);
            groupBox3.Location = new Point(3, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(385, 150);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "GbbVictronWeb:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(6, 54);
            label9.Name = "label9";
            label9.Size = new Size(66, 15);
            label9.TabIndex = 6;
            label9.Text = "Mqtt port*:";
            // 
            // textBox8
            // 
            textBox8.DataBindings.Add(new Binding("Text", ParametersBindingSource, "GbbVictronWeb_Mqtt_Port", true));
            textBox8.Location = new Point(174, 51);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(178, 23);
            textBox8.TabIndex = 5;
            // 
            // textBox9
            // 
            textBox9.DataBindings.Add(new Binding("Text", ParametersBindingSource, "GbbVictronWeb_Mqtt_Address", true));
            textBox9.Location = new Point(174, 22);
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(178, 23);
            textBox9.TabIndex = 3;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(5, 25);
            label10.Name = "label10";
            label10.Size = new Size(84, 15);
            label10.TabIndex = 2;
            label10.Text = "Mqtt address*:";
            // 
            // Log_tabPage2
            // 
            Log_tabPage2.Controls.Add(checkBox2);
            Log_tabPage2.Controls.Add(DriverLog_checkBox);
            Log_tabPage2.Controls.Add(label12);
            Log_tabPage2.Controls.Add(RegisterCount_numericUpDown);
            Log_tabPage2.Controls.Add(Clear_button);
            Log_tabPage2.Controls.Add(VerboseLog_checkBox);
            Log_tabPage2.Controls.Add(label11);
            Log_tabPage2.Controls.Add(RegisterNo_numericUpDown);
            Log_tabPage2.Controls.Add(Search_button);
            Log_tabPage2.Controls.Add(TestSolarmanV5_button);
            Log_tabPage2.Controls.Add(Log_textBox);
            Log_tabPage2.Controls.Add(TestConnections_button);
            Log_tabPage2.Location = new Point(4, 24);
            Log_tabPage2.Name = "Log_tabPage2";
            Log_tabPage2.Padding = new Padding(3);
            Log_tabPage2.Size = new Size(953, 496);
            Log_tabPage2.TabIndex = 1;
            Log_tabPage2.Text = "Tests and Log";
            Log_tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.DataBindings.Add(new Binding("Checked", ParametersBindingSource, "IsDriverLog2", true, DataSourceUpdateMode.OnPropertyChanged));
            checkBox2.Location = new Point(784, 35);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(113, 19);
            checkBox2.TabIndex = 10;
            checkBox2.Text = "Driver level2 Log";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // DriverLog_checkBox
            // 
            DriverLog_checkBox.AutoSize = true;
            DriverLog_checkBox.DataBindings.Add(new Binding("Checked", ParametersBindingSource, "IsDriverLog", true, DataSourceUpdateMode.OnPropertyChanged));
            DriverLog_checkBox.Location = new Point(656, 34);
            DriverLog_checkBox.Name = "DriverLog_checkBox";
            DriverLog_checkBox.Size = new Size(113, 19);
            DriverLog_checkBox.TabIndex = 7;
            DriverLog_checkBox.Text = "Driver level1 Log";
            DriverLog_checkBox.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(390, 39);
            label12.Name = "label12";
            label12.Size = new Size(63, 15);
            label12.TabIndex = 9;
            label12.Text = "No of Reg:";
            // 
            // RegisterCount_numericUpDown
            // 
            RegisterCount_numericUpDown.Location = new Point(459, 35);
            RegisterCount_numericUpDown.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
            RegisterCount_numericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            RegisterCount_numericUpDown.Name = "RegisterCount_numericUpDown";
            RegisterCount_numericUpDown.Size = new Size(76, 23);
            RegisterCount_numericUpDown.TabIndex = 4;
            RegisterCount_numericUpDown.TextAlign = HorizontalAlignment.Center;
            RegisterCount_numericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // Clear_button
            // 
            Clear_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Clear_button.Location = new Point(6, 467);
            Clear_button.Name = "Clear_button";
            Clear_button.Size = new Size(75, 23);
            Clear_button.TabIndex = 7;
            Clear_button.Text = "Clear Log";
            Clear_button.UseVisualStyleBackColor = true;
            Clear_button.Click += Clear_button_Click;
            // 
            // VerboseLog_checkBox
            // 
            VerboseLog_checkBox.AutoSize = true;
            VerboseLog_checkBox.DataBindings.Add(new Binding("Checked", ParametersBindingSource, "IsVerboseLog", true, DataSourceUpdateMode.OnPropertyChanged));
            VerboseLog_checkBox.Location = new Point(656, 9);
            VerboseLog_checkBox.Name = "VerboseLog_checkBox";
            VerboseLog_checkBox.Size = new Size(90, 19);
            VerboseLog_checkBox.TabIndex = 6;
            VerboseLog_checkBox.Text = "Verbose Log";
            VerboseLog_checkBox.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(252, 39);
            label11.Name = "label11";
            label11.Size = new Size(52, 15);
            label11.TabIndex = 5;
            label11.Text = "Register:";
            // 
            // RegisterNo_numericUpDown
            // 
            RegisterNo_numericUpDown.Location = new Point(308, 35);
            RegisterNo_numericUpDown.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            RegisterNo_numericUpDown.Name = "RegisterNo_numericUpDown";
            RegisterNo_numericUpDown.Size = new Size(76, 23);
            RegisterNo_numericUpDown.TabIndex = 3;
            RegisterNo_numericUpDown.TextAlign = HorizontalAlignment.Center;
            RegisterNo_numericUpDown.Value = new decimal(new int[] { 184, 0, 0, 0 });
            // 
            // Search_button
            // 
            Search_button.Location = new Point(6, 6);
            Search_button.Name = "Search_button";
            Search_button.Size = new Size(240, 23);
            Search_button.TabIndex = 2;
            Search_button.Text = "Search for Inverters (SolarmanV5)";
            Search_button.UseVisualStyleBackColor = true;
            Search_button.Click += Search_button_Click;
            // 
            // TestSolarmanV5_button
            // 
            TestSolarmanV5_button.Location = new Point(252, 6);
            TestSolarmanV5_button.Name = "TestSolarmanV5_button";
            TestSolarmanV5_button.Size = new Size(199, 23);
            TestSolarmanV5_button.TabIndex = 2;
            TestSolarmanV5_button.Text = "Read register(s) from SolarmanV5";
            TestSolarmanV5_button.UseVisualStyleBackColor = true;
            TestSolarmanV5_button.Click += TestSolarmanV5_button_Click;
            // 
            // Log_textBox
            // 
            Log_textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Log_textBox.Location = new Point(6, 64);
            Log_textBox.MaxLength = 65000;
            Log_textBox.Multiline = true;
            Log_textBox.Name = "Log_textBox";
            Log_textBox.ReadOnly = true;
            Log_textBox.ScrollBars = ScrollBars.Both;
            Log_textBox.Size = new Size(941, 397);
            Log_textBox.TabIndex = 1;
            // 
            // TestConnections_button
            // 
            TestConnections_button.Location = new Point(459, 6);
            TestConnections_button.Name = "TestConnections_button";
            TestConnections_button.Size = new Size(143, 23);
            TestConnections_button.TabIndex = 5;
            TestConnections_button.Text = "Read from ModBusTCP";
            TestConnections_button.UseVisualStyleBackColor = true;
            TestConnections_button.Click += TestConnections_button_Click;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(textBox7);
            tabPage4.Controls.Add(About_label2);
            tabPage4.Controls.Add(linkLabel1);
            tabPage4.Controls.Add(label8);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(953, 496);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "About";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // textBox7
            // 
            textBox7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox7.Location = new Point(3, 66);
            textBox7.Multiline = true;
            textBox7.Name = "textBox7";
            textBox7.ReadOnly = true;
            textBox7.ScrollBars = ScrollBars.Both;
            textBox7.Size = new Size(947, 427);
            textBox7.TabIndex = 3;
            textBox7.Text = "Version 1.1\r\n- Change place for parameters.xml to MyDocuments/GbbConnect\r\nVersion 1.0\r\n- Suport for Deya SUN-xK-SG0xLP3";
            // 
            // About_label2
            // 
            About_label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            About_label2.Location = new Point(3, 20);
            About_label2.Name = "About_label2";
            About_label2.Size = new Size(947, 20);
            About_label2.TabIndex = 2;
            About_label2.Text = "Author: Gbb Software";
            About_label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linkLabel1.Location = new Point(3, 40);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(947, 23);
            linkLabel1.TabIndex = 1;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://github.com/gbbsoft/GbbConnect";
            linkLabel1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label8.Location = new Point(3, 0);
            label8.Name = "label8";
            label8.Size = new Size(947, 20);
            label8.TabIndex = 0;
            label8.Text = "GbbConnect";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Save_button
            // 
            Save_button.Location = new Point(12, 12);
            Save_button.Name = "Save_button";
            Save_button.Size = new Size(75, 23);
            Save_button.TabIndex = 1;
            Save_button.Text = "Save";
            Save_button.UseVisualStyleBackColor = true;
            Save_button.Click += Save_button_Click;
            // 
            // StartServer_button
            // 
            StartServer_button.Location = new Point(136, 12);
            StartServer_button.Name = "StartServer_button";
            StartServer_button.Size = new Size(110, 23);
            StartServer_button.TabIndex = 2;
            StartServer_button.Text = "Start Server";
            StartServer_button.UseVisualStyleBackColor = true;
            StartServer_button.Click += StartServer_button_Click;
            // 
            // StopServer_button
            // 
            StopServer_button.Enabled = false;
            StopServer_button.Location = new Point(252, 12);
            StopServer_button.Name = "StopServer_button";
            StopServer_button.Size = new Size(110, 23);
            StopServer_button.TabIndex = 3;
            StopServer_button.Text = "Stop Server";
            StopServer_button.UseVisualStyleBackColor = true;
            StopServer_button.Click += StopServer_button_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 60000;
            timer1.Tick += timer1_Tick;
            // 
            // Version_label
            // 
            Version_label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Version_label.Location = new Point(866, 16);
            Version_label.Name = "Version_label";
            Version_label.Size = new Size(104, 19);
            Version_label.TabIndex = 4;
            Version_label.Text = "Version:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(985, 577);
            Controls.Add(Version_label);
            Controls.Add(StopServer_button);
            Controls.Add(StartServer_button);
            Controls.Add(Save_button);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "GbbConnect";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Shown += MainForm_Shown;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)plantsBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)ParametersBindingSource).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Plants_DataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)inverterInfoBindingSource).EndInit();
            tabPage3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            Log_tabPage2.ResumeLayout(false);
            Log_tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)RegisterCount_numericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)RegisterNo_numericUpDown).EndInit();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private GbbLibWin.BindingNavigator bindingNavigator2;
        private GroupBox groupBox1;
        private TextBox textBox2;
        private Label label4;
        private Label label2;
        private GbbLibWin.OurDataGridView Plants_DataGridView;
        private TabPage Log_tabPage2;
        private Button Save_button;
        private TextBox Log_textBox;
        private Button TestConnections_button;
        private BindingSource plantsBindingSource;
        private BindingSource ParametersBindingSource;
        private TextBox textBox1;
        private Label label1;
        private Button TestSolarmanV5_button;
        private TextBox textBox3;
        private Label label3;
        private Button Search_button;
        private NumericUpDown RegisterNo_numericUpDown;
        private BindingSource inverterInfoBindingSource;
        private GroupBox groupBox2;
        private TextBox textBox4;
        private Label label5;
        private TextBox textBox5;
        private TextBox textBox6;
        private Label label7;
        private TabPage tabPage3;
        private GroupBox groupBox3;
        private TextBox textBox8;
        private TextBox textBox9;
        private Label label10;
        private TabPage tabPage4;
        private Label About_label2;
        private LinkLabel linkLabel1;
        private Label label8;
        private Label label6;
        private Label label9;
        private Label label11;
        private Button StartServer_button;
        private Button StopServer_button;
        private GroupBox groupBox4;
        private CheckBox checkBox1;
        private CheckBox VerboseLog_checkBox;
        private DataGridViewTextBoxColumn InverterNumber;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private GbbLibWin.OurDataGridViewComboBoxColumn2 Inverter;
        private DataGridViewCheckBoxColumn IsDisabled;
        private System.Windows.Forms.Timer timer1;
        private Button Clear_button;
        private Label label12;
        private NumericUpDown RegisterCount_numericUpDown;
        private CheckBox DriverLog_checkBox;
        private Label Version_label;
        private CheckBox checkBox2;
        private TextBox textBox7;
    }
}