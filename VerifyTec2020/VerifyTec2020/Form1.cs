using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace VerifyTec2020
{
    public partial class Form1 : Form
    {

        System.Timers.Timer sysTimer;

        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Value = 4;
            numericUpDown2.Value = 10000000;
            chart1.Series.Clear();
            timer1.Interval = 100;
            timer1.Start();
            sysTimer = new System.Timers.Timer();
            sysTimer.Interval = 100;
            sysTimer.Elapsed += SysTimer_Elapsed;
            sysTimer.Start();
        }

        private void SysTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (counterEnable == false && counterNeedStart == true)
            {
                counterEnable = true;
                counterNeedStart = false;
                string msg = string.Format("[{0}] Enable Counter.\n",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                LogMsgs.Add(msg);
            }
            if (MulitRunning == true)
            {
                bool tempret = true;
                for (int i = 0; i < ThreadNum; i++)
                {
                    tempret &= resultState[i];
                }
                if (tempret == true)
                {
                    NeedUpdateChart = true;
                    MulitRunning = false;
                }
                
            }
        }

        bool counterNeedStart = false;
        bool counterEnable = false;

        private delegate void PlusCounterEndCallback(object target);

        private PlusCounterEndCallback PCC;
        private void PlusCounter(object para)
        {
            while (counterEnable == false)
            {
                Thread.Sleep(10);
            }

            string msg = string.Format("[{0}] Counter Start to work with target of {1}.\n",
                DateTime.Now.ToString("yyyy-MM-ss HH:mm:ss.fff"), para);
            LogMsgs.Add(msg);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int target = Convert.ToInt32(para);
            for (int i = 0; i < target; i++)
            {

            }
            sw.Stop();
            counterEnable = false;
            msg = string.Format("[{0}] Counter Fin the work by {1}ms.\n",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), sw.ElapsedMilliseconds);
            LogMsgs.Add(msg);
            PCC?.Invoke(para);

        }

        private void PCCEndProcess(object para)
        {
            string msg = string.Format("[{0}] This is end process.\n",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            LogMsgs.Add(msg);
        }
       


        private void button1_Click(object sender, EventArgs e)
        {
            string msg = string.Format("[{0}] Pluscounter Started.\n",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            LogMsgs.Add(msg);

            PCC = new PlusCounterEndCallback(PCCEndProcess);
            Thread thread = new Thread(PlusCounter);
            thread.Start(100000000);
            counterNeedStart = true;

            msg = string.Format("[{0}] Pluscounter Stopped.\n",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            LogMsgs.Add(msg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (LogMsgs.Count > 0)
                {
                    if (richTextBox1.SelectionColor != Color.Blue)
                    {
                        richTextBox1.SelectionColor = Color.Blue;
                    }
                    richTextBox1.AppendText(LogMsgs[0]);
                    richTextBox1.ScrollToCaret();
                    LogMsgs.RemoveAt(0);
                }

                if(NeedUpdateChart == true)
                {   chart1.Series.Clear();
                    if (chart1.Titles.Count == 0)
                    {
                        chart1.Titles.Add("Mulit Threads Test");
                    }
                    Series s = new Series($"{ThreadNum} Threads");
                    s.ChartType = SeriesChartType.Bar;
                    TotalTime = 0;
                    for (int i = 0; i< ThreadNum; i++)
                    {
                        s.Points.AddXY($"T{i}", Convert.ToDouble(Result[i]));
                        TotalTime += Result[i];
                    }
                    chart1.Series.Add(s);
                    string msg = string.Format("Total Time: {0} ms\n", TotalTime);
                    LogMsgs.Add(msg);
                    NeedUpdateChart = false;
                    numericUpDown1.Enabled = true;
                    numericUpDown2.Enabled = true;
                    button3.Enabled = true;
                    button3.Focus();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        List<string> LogMsgs = new List<string>();

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            NeedUpdateChart = false;
            resultState = new List<bool>();
            Result = new Dictionary<int, long>();
            long target = ThreadTarget;
            for(int i = 0; i< ThreadNum; i++)
            {
                resultState.Add(false);
                Result.Add(i, 0);
                Thread thread = new Thread(MulitPlusCounter);
                List<object> para = new List<object>();
                para.Add(i);
                para.Add(target);
                thread.Start(para);
            }
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            MulitRunning = true;
        }

        bool MulitRunning = false;
        List<bool> resultState = new List<bool>();
        Dictionary<int, long> Result = new Dictionary<int, long>();
        readonly object thislock = new object();
        bool NeedUpdateChart = false;
        int ThreadNum = 0;
        long TotalTime = 0;
        long ThreadTarget = 0;

        private void MulitPlusCounter(object para)
        {
            List<object> paras = para as List<object>;
            int index = Convert.ToInt32(paras[0]);
            long target = Convert.ToInt64(paras[1]);
            Stopwatch sw = new Stopwatch();
            string msg = string.Format("[{0}] Thread<{1}> started.\n",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), index);
            LogMsgs.Add(msg);
            sw.Start();
            for (long i = 0; i < target; i++) ;
            sw.Stop();
            Result[index] = sw.ElapsedMilliseconds;
            msg = string.Format("[{0}] Thread<{1}> stopped.\n",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), index);
            LogMsgs.Add(msg);
            resultState[index] = true;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            ThreadTarget = Convert.ToInt64(numericUpDown2.Value);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ThreadNum = Convert.ToInt32(numericUpDown1.Value);
        }
    }
}
