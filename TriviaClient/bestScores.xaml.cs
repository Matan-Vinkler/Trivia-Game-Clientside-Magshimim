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
    /// Interaction logic for bestScores.xaml
    /// </summary>
    public partial class bestScores : Window
    {
        public bestScores(bool end = false)
        {
            InitializeComponent();

            try
            {
                byte[] buff;

                if (!end)
                {
                    buff = new ASCIIEncoding().GetBytes("223");

                    Conn.clientStream.Write(buff, 0, buff.Length);
                    Conn.clientStream.Flush();
                }

                buff = new byte[4096];

                Conn.clientStream.Read(buff, 0, 4096);
                Conn.clientStream.Flush();

                string msg = new ASCIIEncoding().GetString(buff);

                int first_name_length = int.Parse(msg.Substring(3, 2));
                string first_name = msg.Substring(5, first_name_length);
                int first_score = int.Parse(msg.Substring(first_name_length + 5, 6));

                int second_name_length = int.Parse(msg.Substring(first_name_length + 11, 2));
                string second_name = msg.Substring(first_name_length + 13, second_name_length);
                int second_score = int.Parse(msg.Substring(first_name_length + second_name_length + 13, 6));

                int third_name_length = int.Parse(msg.Substring(first_name_length + second_name_length + 19, 2));
                string third_name = msg.Substring(first_name_length + second_name_length + 21, third_name_length);
                int third_score = int.Parse(msg.Substring(first_name_length + second_name_length + third_name_length + 21, 6));

                this.First.Content = string.Format("{0}: {1}", first_name, first_score);
                this.Second.Content = string.Format("{0}: {1}", second_name, second_score);
                this.Third.Content = string.Format("{0}: {1}", third_name, third_score);
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
