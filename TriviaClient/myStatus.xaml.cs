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
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace trivia_clientside
{
    /// <summary>
    /// Interaction logic for myStatus.xaml
    /// </summary>
    public partial class myStatus : Window
    {
        public myStatus()
        {
            InitializeComponent();

            try
            {
                byte[] buff = new ASCIIEncoding().GetBytes("225");

                Conn.clientStream.Write(buff, 0, buff.Length);
                Conn.clientStream.Flush();

                buff = new byte[23];

                Conn.clientStream.Read(buff, 0, 23);
                Conn.clientStream.Flush();

                string msg = new ASCIIEncoding().GetString(buff);

                if (msg != "12600000000000000000000")
                {

                    this.gamesNum.Content = int.Parse(msg.Substring(3, 4)).ToString();
                    this.correctAns.Content = int.Parse(msg.Substring(7, 6)).ToString();
                    this.incorrectAns.Content = int.Parse(msg.Substring(13, 6)).ToString();
                    this.timeAvg.Content = msg.Substring(19, 4);
                    string firstNumber = this.timeAvg.Content.ToString().Substring(0, 2);
                    string secondNumber = this.timeAvg.Content.ToString().Substring(2, 2);
                    this.timeAvg.Content = firstNumber + '.' + secondNumber;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow sW = new MainWindow();
            sW.Show();

            this.Close();
        }
    }
}
