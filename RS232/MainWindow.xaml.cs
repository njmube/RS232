using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO.Ports;

namespace RS232
{
    
    public partial class MainWindow : Window
    {
        private static SerialPort comPort = new SerialPort();
        private static byte[] ReadBuffer = null;
        private static byte[] WriteBuffer = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitBaud()
        {
            baudBox.Items.Add(300);
            baudBox.Items.Add(600);
            baudBox.Items.Add(1200);
            baudBox.Items.Add(2400);
            baudBox.Items.Add(9600);
            baudBox.Items.Add(14400);
            baudBox.Items.Add(19200);
            baudBox.Items.Add(38400);
            baudBox.Items.Add(57600);
            baudBox.Items.Add(115200);
        }
        private void checkPort(SerialPort Port)
        {
            if(Port.IsOpen)
                Port.Close();
                           
        }
        private void InitPort(SerialPort Port)
        {
            if (comBox.Text.Length != 0)
            {
                try
                {
                    Port.PortName = Convert.ToString(comBox.Text);
                }
                catch (Exception ex)
                {
                    Port.Close();
                }

            }
            else
                MessageBox.Show("Select COM port.");
            if (comBox.Text.Length != 0)
                Port.BaudRate = Convert.ToInt32(baudBox.Text);
            else
                MessageBox.Show("Select baudrate.");
            Port.DataBits = 8;
            Port.Parity = (Parity)0;
            Port.StopBits = (StopBits)1;
            try
            {
                Port.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                
                     
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string [] COMList = SerialPort.GetPortNames();
            if(COMList.Length == 0)
            {
                MessageBox.Show("No COM interfaces.");
            }
            else
            {
                for (int i = 0; i < COMList.Length; i++)                                   
                    comBox.Items.Add(COMList[i]);
            }
            InitBaud();        
            
        }

        private async void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            InitPort(comPort);
            ReadBuffer = new byte[comPort.BaseStream.Length];
            await comPort.BaseStream.ReadAsync(ReadBuffer, 0, (int)comPort.BaseStream.Length);
            InBytes.Items.Add(ReadBuffer.ToString());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            comPort.Close();
        }

        
    }
}
