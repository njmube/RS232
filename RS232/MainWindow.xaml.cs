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
        
        public MainWindow()
        {
            InitializeComponent();
            SendMode.Items.Add("ASCII");
            SendMode.Items.Add("HEX");
            
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
                MessageBox.Show("Select BaudRate.");
            
            Port.DataBits = 8;
            Port.Parity = (Parity)0;
            Port.StopBits = (StopBits)1;
            
            try
            {
                Port.DataReceived += Port_DataReceived;
                
                Port.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
                
                     
        }
        
        void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            var ReadBuffer = new Byte[comPort.BytesToRead];
            try
            {
                comPort.BaseStream.ReadAsync(ReadBuffer, 0, comPort.BytesToRead);
                
                
                
                    Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        InBox.Items.Add(BitConverter.ToString(ReadBuffer).Replace("-", " "));
                        
                    }));
                
            }
            catch(Exception ex)
            {
                if(comPort.BytesToRead > ReadBuffer.Length)
                {
                    ReadBuffer = new byte[comPort.BytesToRead];
                    comPort.BaseStream.ReadAsync(ReadBuffer, 0, comPort.BytesToRead);
                }
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

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            InitPort(comPort);
            
            
            

                        
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            comPort.Close();
        }
        private void SendData()
        {
            var OutBuffer = new byte[Out.Text.Length];
            OutBuffer = Encoding.ASCII.GetBytes(Out.Text);
            if (Convert.ToString(SendMode.Text) == "HEX")
            {
                try
                {
                    comPort.BaseStream.WriteAsync(OutBuffer, 0, OutBuffer.Length);
                }
                catch (Exception ex)
                {
                    InitPort(comPort);
                }
                OutBox.Items.Add(BitConverter.ToString(OutBuffer).Replace("-", " "));
            }
            else if (Convert.ToString(SendMode.Text) == "ASCII")
            {

                try
                {
                    comPort.BaseStream.WriteAsync(OutBuffer, 0, OutBuffer.Length);
                }
                catch (Exception ex)
                {
                    InitPort(comPort);
                }
                OutBox.Items.Add(System.Text.ASCIIEncoding.ASCII.GetString(OutBuffer));
            }
            else
            {
                MessageBox.Show("Please select mode");
            }
        }
        private void Out_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendData();                
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            SendData();
        }

        
    }
}
