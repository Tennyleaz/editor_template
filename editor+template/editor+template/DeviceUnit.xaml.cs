using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace editor_template
{
    /// <summary>
    /// DeviceUnit.xaml 的互動邏輯
    /// </summary>
    public partial class DeviceUnit : UserControl
    {
        private readonly MyTcpClient myTcpClient = new MyTcpClient();
        private IPAddress myIP;
        private int myPort;
        private int IP_OK = 0;
        private bool Port_OK = false;

        private Task ReceivingTask;
        private readonly byte[] DataReceived = new byte[64];
        private int DataReceivedLength;
        private bool IsReceiving = false;

        public DeviceUnit()
        {
            InitializeComponent();
            CloseButton.IsEnabled = false;

            IPBox1.Text = "192";
            IPBox2.Text = "168";
            IPBox3.Text = "11";
            IPBox4.Text = "254";
            PortBox.Text = "8080";
            IP_OK = 0b1111;
            Port_OK = true;
            Update_IPAdress();
        }

        public DeviceUnit(string IP1_string, string IP2_string, string IP3_string, string IP4_string, string Port_string)
        {
            InitializeComponent();
            CloseButton.IsEnabled = false;

            IPBox1.Text = IP1_string;
            IPBox2.Text = IP2_string;
            IPBox3.Text = IP3_string;
            IPBox4.Text = IP4_string;
            PortBox.Text = Port_string;
            IP_OK = 0b1111;
            Port_OK = true;
            Update_IPAdress();
        }

        ~DeviceUnit()
        {
            myTcpClient.Dispose();
            ReceivingTask?.Dispose();
        }

        private void Update_IPAdress()
        {
            if(IP_OK == 0b1111 && Port_OK)
            {
                myIP = IPAddress.Parse(IPBox1.Text + '.' + IPBox2.Text + '.' + IPBox3.Text + '.' + IPBox4.Text);
                myPort = int.Parse(PortBox.Text);
                ConnectButton.IsEnabled = true;
            }
            else
            {
                ConnectButton.IsEnabled = false;
            }
        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectButton.IsEnabled = false;
            await myTcpClient.Connect(myIP, myPort);
            if (myTcpClient.isConnected)
            {
                CloseButton.IsEnabled = true;
                Debug.WriteLine("Start received");
                IsReceiving = true;
                ReceivingTask = Task.Run(new Action(Start_Receiving)); //Infinited loop until close button is clicked
            }
            else 
            {
                MessageBox.Show("無法連線到裝置 IP:" + myIP.ToString() + " Port:" + myPort, "連線失敗", MessageBoxButton.OK, MessageBoxImage.Warning);
                CloseButton.IsEnabled = false;
                ConnectButton.IsEnabled = true;
            }
        }

        private void Start_Receiving()
        {
            Debug.WriteLine("Start received");
            int i = 0;
            while (IsReceiving)
            {
                //Debug.WriteLine("Received " + i++);
                DataReceivedLength = myTcpClient.Receive(DataReceived, 0, 64);
                if (DataReceivedLength == 0)
                {
                    //Debug.WriteLine("No data");
                }
                else
                {
                    Debug.WriteLine(DataReceived.ToString());
                }
                //Debug.WriteLine("Received " + i++);
            }
        }

        private async void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            IsReceiving = false;
            await Task.Delay(1000);
            myTcpClient.Close();
            CloseButton.IsEnabled = false;
            ConnectButton.IsEnabled = true;
        }

        private void IPBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            int parsed_text;
            if (int.TryParse(IPBox1.Text, out parsed_text) && parsed_text >= 0 && parsed_text < 256)
            {
                IP_OK |= 0b0001;
                IPBox1.Background = Brushes.White;
            }
            else
            {
                IP_OK &= 0b1110;
                IPBox1.Background = Brushes.Red;
            }
            Update_IPAdress();
        }

        private void IPBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            int parsed_text;
            if (int.TryParse(IPBox2.Text, out parsed_text) && parsed_text >= 0 && parsed_text < 256)
            {
                IP_OK |= 0b0010;
                IPBox2.Background = Brushes.White;
            }
            else
            {
                IP_OK &= 0b1101;
                IPBox2.Background = Brushes.Red;
            }
            Update_IPAdress();
        }

        private void IPBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            int parsed_text;
            if (int.TryParse(IPBox3.Text, out parsed_text) && parsed_text >= 0 && parsed_text < 256)
            {
                IP_OK |= 0b0100;
                IPBox3.Background = Brushes.White;
            }
            else
            {
                IP_OK &= 0b1011;
                IPBox3.Background = Brushes.Red;
            }
            Update_IPAdress();
        }

        private void IPBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            int parsed_text;
            if (int.TryParse(IPBox4.Text, out parsed_text) && parsed_text >= 0 && parsed_text < 256)
            {
                IP_OK |= 0b1000;
                IPBox4.Background = Brushes.White;
            }
            else
            {
                IP_OK &= 0b0111;
                IPBox4.Background = Brushes.Red;
            }
            Update_IPAdress();
        }

        private void PortBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(PortBox.Text, out myPort) && myPort >0 && myPort < 65535)
            {
                Port_OK = true;
                PortBox.Background = Brushes.White;
            }
            else
            {
                Port_OK = false;
                PortBox.Background = Brushes.Red;
            }
            Update_IPAdress();
        }
    }
}
