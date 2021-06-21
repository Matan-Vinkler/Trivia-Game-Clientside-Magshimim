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
    /// Interaction logic for newRoom1.xaml
    /// </summary>
    public partial class newRoom1 : Window
    {
        public newRoom1()
        {
            InitializeComponent();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow sW = new MainWindow();
            sW.Show();

            this.Close();
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            int num;
            if (this.roomName.Text.Length == 0 || this.playerNum.Text.Length == 0 || this.questionNum.Text.Length == 0 || this.questionTime.Text.Length == 0)
            {
                this.IsFail.Content = "Please fill all fields";
            }
            else if (!int.TryParse(this.playerNum.Text, out num)  || int.Parse(this.playerNum.Text) == 0 || this.playerNum.Text.Length != 1)
            {
                this.IsFail.Content = "Wrong player number";
            }
            else if (!int.TryParse(this.questionNum.Text, out num) || int.Parse(this.questionNum.Text) == 0 || this.questionNum.Text.Length > 2)
            {
                this.IsFail.Content = "Wrong question number";
            }
            else if (!int.TryParse(this.questionTime.Text, out num) || int.Parse(this.questionTime.Text) == 0 || this.questionTime.Text.Length > 2)
            {
                this.IsFail.Content = "Wrong time per question";
            }
            else
            {
                try
                {
                    string msg = string.Format("213{0}{1}{2}{3}{4}", Helper.NumToStr(this.roomName.Text.Length), this.roomName.Text, this.playerNum.Text, Helper.NumToStr(int.Parse(this.questionNum.Text)), Helper.NumToStr(int.Parse(this.questionTime.Text)));
                    byte[] buff = new ASCIIEncoding().GetBytes(msg);

                    Conn.clientStream.Write(buff, 0, buff.Length);
                    Conn.clientStream.Flush();

                    buff = new byte[4];
                    Conn.clientStream.Read(buff, 0, buff.Length);
                    Conn.clientStream.Flush();

                    msg = new ASCIIEncoding().GetString(buff);

                    if (msg == "1140")
                    {
                        Room.roomName = this.roomName.Text;
                        Room.maxPlayers = this.playerNum.Text;
                        Room.questionsNum = this.questionNum.Text;
                        Room.timePerOne = this.questionTime.Text;
                        Room.isAdmin = true;

                        waitRoom sW = new waitRoom();
                        sW.Show();

                        this.Close();
                    }
                    else if (msg == "1141")
                    {
                        this.IsFail.Content = "Creating Room Failed!";
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Something's wrong, I can Feel it.");
                    this.Close();
                }
            }
        }
           
    }
}
