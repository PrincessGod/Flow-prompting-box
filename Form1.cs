using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetMonitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        //cpu
        //private const string CategoryName = "Processor";
        //private const string CounterName = "% Processor Time";
        //private const string InstanceName = "_Total";
        //PerformanceCounter pc = new PerformanceCounter(CategoryName, CounterName, InstanceName);

        //memory
        //ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
        

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.SplitterDistance = this.splitContainer1.Height/2;
            label1.Text = String.Format("{0}{1}{2}{3}{4}{5}", text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)]);
            label2.Text = String.Format("{0}{1}{2}{3}{4}{5}", text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)]);
        }

        //float netRecvlast = 0;
        //float netSendlast = 0;
        //float cpulast = 0;
        string[] text = new string[9] { "▗", "▘", "▚", "▛", "▞", "▝", "▙", "▟", "▜" };
        Random random = new Random();

        /// <summary>
        /// Get information per second
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            float netRecv = NetMonitorCore.GetNetRecv();
            float netSend = NetMonitorCore.GetNetSend();

            //netRecvlast = (netRecv + netRecvlast) / 2;
            //netSendlast = (netSend + netSendlast) / 2;

            //netRecv = netRecvlast;
            //netSend = netSendlast;
            string netRecvText = "";
            string netSendText = "";

            if (netRecv < 1024)
            {
                netRecvText = netRecv.ToString("0.00") + "B/s";
                //netRecvText = String.Format("{0}{1}{2}{3}{4}{5}",text[random.Next(9)],text[random.Next(9)],text[random.Next(9)],text[random.Next(9)],text[random.Next(9)],text[random.Next(9)]);
            }
            else if (netRecv >= 1024 && netRecv < 1024 * 1024)
            {
                netRecvText = (netRecv / 1024).ToString("0.00") + "KB/s";
            }
            else if (netRecv >= 1024 * 1024)
            {
                netRecvText = (netRecv / (1024 * 1024)).ToString("0.00") + "MB/s";
            }

            if (netSend < 1024)
            {
                netSendText = netSend.ToString("0.00") + "B/s";
                //netSendText = String.Format("{0}{1}{2}{3}{4}{5}", text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)]);
            }
            else if (netSend >= 1024 && netSend < 1024 * 1024)
            {
                netSendText = (netSend / 1024).ToString("0.00") + "KB/s";
            }
            else if (netSend >= 1024 * 1024)
            {
                netSendText = (netSend / (1024 * 1024)).ToString("0.00") + "MB/s";
            }

            //ManagementObjectCollection moc = mc.GetInstances();

            //long xx = 0;
            //long availablebytes = 0;
            //foreach (ManagementObject mo in moc)
            //{
            //    if (mo["TotalVisibleMemorySize"] != null)
            //    {
            //        xx = long.Parse(mo["TotalVisibleMemorySize"].ToString());
            //    }

            //    if (mo["FreePhysicalMemory"] != null)
            //    {
            //        availablebytes = long.Parse(mo["FreePhysicalMemory"].ToString());
            //    }
            //} 

            label1.Text = netSendText;
            label2.Text = netRecvText;
            //label1.BackColor = Color.FromArgb(random.Next(256),random.Next(256),random.Next(256));
            //label2.BackColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

            //float cpu = (pc.NextValue() + cpulast) / 2;
            //cpulast = cpu;
            //label2.Text = cpu.ToString("0.") + "%";
            //if (xx != 0)
            //{
            //    label2.Text = ((100 - ((double)((double)availablebytes / (double)xx) * 100))).ToString("0.") + "%";
            //}
                

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }


        private void label2_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle && e.Clicks == 2)
                this.Close();
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.Clicks == 2)
                NetMonitorCore.TryNextConnect();
        }

    }
}
