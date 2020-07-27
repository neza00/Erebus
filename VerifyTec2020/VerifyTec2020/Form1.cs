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

namespace VerifyTec2020
{
    public partial class Form1 : Form
    {

        System.Timers.Timer sysTimer;

        public Form1()
        {
            InitializeComponent();
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
    }
}
