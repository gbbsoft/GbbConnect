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
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            bindingNavigator2 = new GbbLibWin.BindingNavigator();
            groupBox1 = new GroupBox();
            textBox2 = new TextBox();
            plantsBindingSource = new BindingSource(components);
            ParametersBindingSource = new BindingSource(components);
            label4 = new Label();
            label2 = new Label();
            Plants_DataGridView = new GbbLibWin.OurDataGridView();
            nameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tabPage2 = new TabPage();
            TestLog_textBox = new TextBox();
            TestConnections_button = new Button();
            Save_button = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)plantsBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ParametersBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Plants_DataGridView).BeginInit();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 41);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(776, 397);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(bindingNavigator2);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(Plants_DataGridView);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(768, 369);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Plants";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // bindingNavigator2
            // 
            bindingNavigator2.BindingSource = null;
            bindingNavigator2.HideAddAndDelete = true;
            bindingNavigator2.Location = new Point(6, 184);
            bindingNavigator2.Name = "bindingNavigator2";
            bindingNavigator2.Size = new Size(272, 25);
            bindingNavigator2.TabIndex = 8;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(label4);
            groupBox1.Location = new Point(284, 31);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(385, 150);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "System:";
            // 
            // textBox2
            // 
            textBox2.DataBindings.Add(new Binding("Text", plantsBindingSource, "AddressIP", true));
            textBox2.Location = new Point(174, 22);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(178, 23);
            textBox2.TabIndex = 3;
            // 
            // plantsBindingSource
            // 
            plantsBindingSource.DataMember = "Plants";
            plantsBindingSource.DataSource = ParametersBindingSource;
            // 
            // ParametersBindingSource
            // 
            ParametersBindingSource.DataSource = typeof(Configuration.Parameters);
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
            Plants_DataGridView.Columns.AddRange(new DataGridViewColumn[] { nameDataGridViewTextBoxColumn });
            Plants_DataGridView.DataSource = plantsBindingSource;
            Plants_DataGridView.Location = new Point(6, 31);
            Plants_DataGridView.Name = "Plants_DataGridView";
            Plants_DataGridView.OurPaste_BlockMultiLine = false;
            Plants_DataGridView.RowTemplate.Height = 25;
            Plants_DataGridView.Size = new Size(272, 150);
            Plants_DataGridView.TabIndex = 5;
            Plants_DataGridView.RowValidating += Plants_DataGridView_RowValidating;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            dataGridViewCellStyle1.BackColor = Color.FromArgb(255, 255, 192);
            nameDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            nameDataGridViewTextBoxColumn.HeaderText = "Name";
            nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            nameDataGridViewTextBoxColumn.Width = 200;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(TestLog_textBox);
            tabPage2.Controls.Add(TestConnections_button);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(768, 369);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Tests";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // TestLog_textBox
            // 
            TestLog_textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TestLog_textBox.Location = new Point(6, 35);
            TestLog_textBox.Multiline = true;
            TestLog_textBox.Name = "TestLog_textBox";
            TestLog_textBox.Size = new Size(745, 328);
            TestLog_textBox.TabIndex = 1;
            // 
            // TestConnections_button
            // 
            TestConnections_button.Location = new Point(6, 6);
            TestConnections_button.Name = "TestConnections_button";
            TestConnections_button.Size = new Size(143, 23);
            TestConnections_button.TabIndex = 0;
            TestConnections_button.Text = "Test connections";
            TestConnections_button.UseVisualStyleBackColor = true;
            TestConnections_button.Click += TestConnections_button_Click;
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
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Save_button);
            Controls.Add(tabControl1);
            Name = "MainForm";
            Text = "GbbConnect";
            WindowState = FormWindowState.Maximized;
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)plantsBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)ParametersBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)Plants_DataGridView).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
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
        private TabPage tabPage2;
        private Button Save_button;
        private TextBox TestLog_textBox;
        private Button TestConnections_button;
        private BindingSource plantsBindingSource;
        private BindingSource ParametersBindingSource;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private TextBox textBox1;
        private Label label1;
    }
}