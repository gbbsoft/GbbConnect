using System.Runtime.InteropServices;
using GbbEngine.Configuration;
using Microsoft.Extensions.Logging;

namespace GbbConnect
{
    public partial class MainForm : Form, GbbLib.IOurLog
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

        private void MainForm_Shown(object sender, EventArgs e)
        {
            try
            {
                // Autostart Server
                if (Program.Parameters.Server_AutoStart)
                    StartServer_button_Click(sender, e);

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

        private void VerboseLog_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GbbEngine.Configuration.Parameters.IsVerboseLog = this.VerboseLog_checkBox.Checked;
            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }
        }

        private void Clear_button_Click(object sender, EventArgs e)
        {
            try
            {
                Log_textBox.Clear();

            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }

        }


        // ======================================
        // Plants
        // ======================================

        private void plantsBindingSource_AddingNew(object sender, System.ComponentModel.AddingNewEventArgs e)
        {
            Plant ret = new();

            // calculate next InverterNumber
            ret.Number = 1;
            foreach (var itm in Program.Parameters.Plants)
                if (itm.Number >= ret.Number)
                    ret.Number = itm.Number + 1;

            e.NewObject = ret;
        }

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
                                    Log($"Value of {RegisterNo_numericUpDown.Value}: {answer[0] * 256 + answer[1]}");
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

        private async void TestSolarmanV5_button_Click(object sender, EventArgs e)
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

                                    answer = await driver.ReadHoldingRegister(1, (ushort)RegisterNo_numericUpDown.Value, (ushort)RegisterCount_numericUpDown.Value);

                                    System.Text.StringBuilder sb = new();
                                    for (int i = 0; i < RegisterCount_numericUpDown.Value; i++)
                                    {
                                        sb.Append(RegisterNo_numericUpDown.Value + i);
                                        sb.Append('=');
                                        sb.Append(answer[i * 2] * 256 + answer[i * 2 + 1]);
                                        sb.Append(", ");
                                    }

                                    Log($"Answer: {sb.ToString()}");
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

        // ======================================
        // Log
        // ======================================

        private object LogSync = new();

        private void Log(string message)
        {
            this.Log_textBox.AppendText($"{DateTime.Now}: {message}\r\n");
        }

        // log from engine
        public void OurLog(LogLevel LogLevel, string message, params object?[] args)
        {
            var nw = DateTime.Now;

            if (args.Length > 0)
                message = string.Format(message, args);

            // add time
            string msg;
            if (LogLevel == LogLevel.Error)
                msg = $"{nw}: ERROR: {message}\r\n";
            else
                msg = $"{nw}: {message}\r\n";

            lock (LogSync)
            {

                // directory for log
                string FileName = Path.Combine(GbbEngine.Configuration.Parameters.OurGetUserBaseDirectory(), "Log");
                Directory.CreateDirectory(FileName);

                // filename of log
                FileName = Path.Combine(FileName, $"{nw:yyyy-MM-dd}.txt");
                File.AppendAllText(FileName, msg);
            }



            // log also to Log_textbox
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    if (Log_textBox.Text.Length > 10000)
                        Log_textBox.Text = Log_textBox.Text.Substring(5000) + msg;
                    else
                        Log_textBox.AppendText(msg);
                }));
            else
                Log_textBox.AppendText(msg);


        }

        // ======================================
        // Server
        // ======================================
        private GbbEngine.Server.JobManager? JobManeger;

        private void StartServer_button_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateChildren();
                this.tabControl1.SelectedTab = this.Log_tabPage2;

                if (JobManeger == null)
                {
                    JobManeger = new();
                    JobManeger.OurStartJobs(Program.Parameters, this);
                }
                this.StartServer_button.Enabled = false;
                this.StopServer_button.Enabled = true;

            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }

        }

        private void StopServer_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (JobManeger != null)
                {
                    JobManeger.OurStopJobs(Program.Parameters);
                    JobManeger = null;
                }
                this.StartServer_button.Enabled = true;
                this.StopServer_button.Enabled = false;

            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }
        }

        // ======================================
        // Prevent sleep
        // ======================================

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // Prevent Idle-to-Sleep (monitor not affected) (see note above)
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
            }
            catch (Exception ex)
            {
                GbbLibWin.Log.ErrMsgBox(this, ex);
            }
        }

    }
}