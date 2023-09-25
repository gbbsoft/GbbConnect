namespace GbbConnect
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Program.Parameters_Load();

                DateTime td = DateTime.Today;

                // dictionaries
                this.inverterInfoBindingSource.DataSource = GbbEngine.Inverters.InverterInfo.OurGetInverterInfos();


                // parameters
                this.ParametersBindingSource.DataSource = Program.Parameters;


                //this.ImportVRM_FromDate_dateTimePicker.Value = DateTime.Today.AddDays(-28);
                //this.ImportVRM_ToDate_dateTimePicker.Value = DateTime.Today.AddDays(-1);

                //GbbLib.Application.StatusBar.OnStatusBarMessage += (arg) => { this.toolStripStatusLabel1.Text = arg.sInfo; };


                // About
                if (td.Year == 2023)
                    this.About_label2.Text = this.About_label2.Text + " 2023";
                else
                    this.About_label2.Text = this.About_label2.Text + " 2023 - {DateTime.Today.Year}";
            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Program.Parameters_Save();
            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }

        }

        // ======================================
        // Operations
        // ======================================

        private void Save_button_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateChildren();
                Program.Parameters_Save();
            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }

        }

        // ======================================
        // Plants
        // ======================================

        private void Plants_DataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            // check new/change row
            if (e.RowIndex >= 0
            && !Plants_DataGridView.Rows[e.RowIndex].IsNewRow
            && e.RowIndex < Program.Parameters.Plants.Count)
                Program.Parameters.Plants[e.RowIndex].OurCheckDataForUI();

        }

        private void TestConnections_button_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateChildren();
                if (GbbLibWin.Log.LogMsgBox(this, "Do you want to start connection test?", MessageBoxButtons.YesNo, DialogResult.Yes, Microsoft.Extensions.Logging.LogLevel.Information) == DialogResult.Yes)
                {
                    foreach (var itm in Program.Parameters.Plants)
                    {

                        if (itm.IsDisabled == 0 && itm.AddressIP != null && itm.PortNo != null)
                        {
                            try
                            {

                                Log($"Plant: {itm.Name}");
                                var driver = new ModbusTCP.Master(itm.AddressIP, (ushort)itm.PortNo);
                                driver.OnException += Driver_OnException;
                                try
                                {

                                    byte[] answer = { 0, 66 };

                                    driver.ReadHoldingRegister(1, 1, (ushort)RegisterNo_numericUpDown.Value, 1, ref answer);

                                    Log($"Value (first 2 bytes): {answer[0] * 256 + answer[1]}");
                                }
                                finally
                                {
                                    driver.disconnect();
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(ex.Message);
                            }
                        }

                    }
                    GbbLibWin.Log.LogMsgBox(this, "Done");

                }
            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }
        }

        private void Driver_OnException(ushort id, byte unit, byte function, byte exception)
        {
            throw new ApplicationException($"Exceeption from driver: function: {function}, exception: {exception}");
        }

        private void TestSolarmanV5_button_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateChildren();
                if (GbbLibWin.Log.LogMsgBox(this, "Do you want to start connection test?", MessageBoxButtons.YesNo, DialogResult.Yes, Microsoft.Extensions.Logging.LogLevel.Information) == DialogResult.Yes)
                {
                    foreach (var itm in Program.Parameters.Plants)
                    {

                        if (itm.IsDisabled == 0 && itm.AddressIP != null && itm.PortNo != null)
                        {
                            try
                            {

                                Log($"Plant: {itm.Name}");
                                var driver = new GbbEngine.Drivers.SolarmanV5.SolarmanV5Driver(itm.AddressIP, itm.PortNo.Value, itm.SerialNumber ?? 0);
                                try
                                {


                                    byte[] answer = { 0, 66 };
                                    //driver.WriteMultipleRegister(0, 1, 184, answer);

                                    answer = driver.ReadHoldingRegister(1, (ushort)RegisterNo_numericUpDown.Value, 1);

                                    Log($"SOC: {answer[0] * 256 + answer[1]}");
                                }
                                finally
                                {
                                    driver.Dispose();
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(ex.Message);
                            }
                        }

                    }
                    GbbLibWin.Log.LogMsgBox(this, "Done");

                }
            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }
        }

        private void Search_button_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateChildren();
                if (GbbLibWin.Log.LogMsgBox(this, "Do you want to search for SolarmanV5?", MessageBoxButtons.YesNo, DialogResult.Yes, Microsoft.Extensions.Logging.LogLevel.Information) == DialogResult.Yes)
                {

                    Log($"{DateTime.Now}: Start search (5 sec)");
                    var ll = GbbEngine.Drivers.SolarmanV5.SolarmanV5Driver.OurSearchSolarman();

                    Log($"{DateTime.Now}: Result: (IpAddress, MAC address, SerialNo)");
                    if (ll.Count == 0)
                        Log("Nothing found...");
                    else
                    {
                        Log($"==========================");
                        foreach (var itm in ll)
                        {
                            Log(itm);
                        }
                        Log($"==========================");
                    }

                }

            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }
        }

        private void Log(string message)
        {
            this.TestLog_textBox.AppendText($"{DateTime.Now}: {message}\r\n");
        }

    }
}