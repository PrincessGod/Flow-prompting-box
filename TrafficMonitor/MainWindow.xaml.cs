using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TrafficMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer1 = new DispatcherTimer();
        private List<Line> lineListUp = new List<Line>();
        private List<Line> lineListDown = new List<Line>();
        public MainWindow()
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
            timer1.Tick += timer1_Tick;
            timer1.Interval = new TimeSpan(0,0,1);
            timer1.Start();

            this.UplodeLabel.Content = String.Format("{0}{1}{2}{3}{4}{5}", text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)]);
            this.DownloadLabel.Content = String.Format("{0}{1}{2}{3}{4}{5}", text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)]);
        }

        private void AddFlayLine(double lineWidth , bool isUP)
        {
            Line line = new Line();
            line.X1 = -lineWidth - 1;
            line.Y1 = line.Y2 = random.Next(1, 17);
            line.X2 = -1;
            line.StrokeThickness = 2;

            if (isUP)
            {
                UpGrid.Children.Add(line);

                line.Stroke = new SolidColorBrush(Color.FromRgb((byte)random.Next(180,256), (byte)random.Next(100), (byte)random.Next(256)));
                ThicknessAnimation da = new ThicknessAnimation();
                da.From = new Thickness(0);    //起始值
                da.To = new Thickness(103 + lineWidth, 0, 0, 0);      //结束值
                da.Duration = TimeSpan.FromSeconds(2);         //动画持续时间
                da.BeginTime = TimeSpan.FromSeconds(random.NextDouble()); 
                da.EasingFunction = new CircleEase() {EasingMode = EasingMode.EaseOut};
                line.BeginAnimation(Line.MarginProperty, da);//开始动画

                lineListUp.Add(line);
                if (lineListUp.Count > 50)
                {
                    this.UpGrid.Children.Remove(lineListUp[0]);
                    lineListUp.RemoveAt(0);
                }

            }
                
            else
            {
                DownGrid.Children.Add(line);

                line.Stroke = new SolidColorBrush(Color.FromRgb((byte)random.Next(100), (byte)random.Next(180,256), (byte)random.Next(256)));
                ThicknessAnimation da = new ThicknessAnimation();
                da.From = new Thickness(-1,0,0,0);    //起始值
                da.To = new Thickness(103 + lineWidth, 0, 0, 0);      //结束值
                da.Duration = TimeSpan.FromSeconds(2);         //动画持续时间
                da.BeginTime = TimeSpan.FromSeconds(random.NextDouble());
                da.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
                line.BeginAnimation(Line.MarginProperty, da);//开始动画

                lineListDown.Add(line);
                if (lineListDown.Count > 50)
                {
                    this.UpGrid.Children.Remove(lineListDown[0]);
                    lineListDown.RemoveAt(0);
                }
            }
               
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
                this.AddFlayLine(random.Next(1,2),false);
            }
            else if (netRecv >= 1024 && netRecv < 1024 * 1024)
            {
                netRecvText = (netRecv / 1024).ToString("0.00") + "KB/s";
                if ((netRecv / 1024) < 50)
                    this.AddFlayLine(1, false);
                else
                {
                    int x = (int)((netRecv / 1024) / 50);
                    this.AddFlayLine(4 + x, false);
                }
            }
            else if (netRecv >= 1024 * 1024)
            {
                netRecvText = (netRecv / (1024 * 1024)).ToString("0.00") + "MB/s";
                if ((netRecv / (1024 * 1024)) <2)
                    this.AddFlayLine(random.Next(20,30),false);
                else if((netRecv / (1024 * 1024)) <3)
                    this.AddFlayLine(random.Next(30, 40), false);
                else
                    this.AddFlayLine(random.Next(40, 50), false);
            }

            if (netSend < 1024)
            {
                netSendText = netSend.ToString("0.00") + "B/s";
                this.AddFlayLine(random.Next(1,2), true);
                //netSendText = String.Format("{0}{1}{2}{3}{4}{5}", text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)], text[random.Next(9)]);
            }
            else if (netSend >= 1024 && netSend < 1024 * 1024)
            {
                netSendText = (netSend / 1024).ToString("0.00") + "KB/s";
                if ((netSend / 1024) < 50)
                    this.AddFlayLine(1, true);
                else
                {
                    int x = (int)((netSend / 1024) / 50);
                    this.AddFlayLine(4 + x , true);
                }
            }
            else if (netSend >= 1024 * 1024)
            {
                netSendText = (netSend / (1024 * 1024)).ToString("0.00") + "MB/s";
                if ((netSend / (1024 * 1024)) < 2)
                    this.AddFlayLine(random.Next(25, 35), true);
                else if ((netSend / (1024 * 1024)) < 3)
                    this.AddFlayLine(random.Next(35, 45), true);
                else
                    this.AddFlayLine(random.Next(45, 60), true);
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

            this.UplodeLabel.Content = netSendText;
            this.DownloadLabel.Content = netRecvText;
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

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }


        private void label_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Right)
                NetMonitorCore.TryNextConnect();
            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.UplodeLabel.Visibility == Visibility.Visible)
                {
                    this.UplodeLabel.Visibility = Visibility.Hidden;
                    this.DownloadLabel.Visibility = Visibility.Hidden;
                    return;
                }
                else
                {
                    this.UplodeLabel.Visibility = Visibility.Visible;
                    this.DownloadLabel.Visibility = Visibility.Visible;
                }
                
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 100)
                this.Close();
        }

    }
}
