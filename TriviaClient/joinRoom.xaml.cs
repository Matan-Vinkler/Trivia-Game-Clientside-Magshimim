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
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace trivia_clientside
{
    /// <summary>
    /// Interaction logic for joinRoom.xaml
    /// </summary>
    public partial class joinRoom : Window
    {
        public joinRoom()
        {
            InitializeComponent();
            Thread.Sleep(100);
            searchButton_Click(null, null);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buff = new ASCIIEncoding().GetBytes("205");

                Conn.clientStream.Write(buff, 0, buff.Length);
                Conn.clientStream.Flush();

                buff = new byte[4096];

                Conn.clientStream.Read(buff, 0, buff.Length);
                Conn.clientStream.Flush();

                string msg = new ASCIIEncoding().GetString(buff);

                int roomNumber = int.Parse(msg.Substring(3, 4));
                int roomIndex = 0;

                string roomName, roomID;
                int roomNameLength;

                ListBoxItem item;
                Button button;

                int i = 7;

                while (roomIndex < roomNumber)
                {
                    roomID = msg.Substring(i, 4);
                    i += 4;

                    roomNameLength = int.Parse(msg.Substring(i, 2));
                    i += 2;

                    roomName = msg.Substring(i, roomNameLength);
                    i += roomNameLength;

                    item = new ListBoxItem();
                    button = new Button();

                    button.Visibility = Visibility.Hidden;
                    item.Visibility = Visibility.Hidden;

                    button.Content = string.Format("{0}-{1}", roomName, roomID);
                    button.Click += showButton_Click;

                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.HorizontalAlignment = HorizontalAlignment.Left;

                    button.FontFamily = new FontFamily("Arial");
                    button.FontSize = 10;
                    button.FontWeight = FontWeights.Bold;
                    button.Background = Brushes.Yellow;

                    button.Visibility = Visibility.Visible;
                    item.Visibility = Visibility.Visible;

                    item.Content = button;
                    this.roomList.Items.Add(item);

                    roomIndex++;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }

        private void showButton_Click(object sender, EventArgs e)
        {
            try
            {
                Button thisButton = (Button)sender;

                string roomName = thisButton.Content.ToString().Split('-')[0];
                string roomID = thisButton.Content.ToString().Split('-')[1];

                string msg = string.Format("207{0}", roomID);
                byte[] buff = new ASCIIEncoding().GetBytes(msg);

                Conn.clientStream.Write(buff, 0, buff.Length);
                Conn.clientStream.Flush();

                buff = new byte[4096];

                Conn.clientStream.Read(buff, 0, buff.Length);
                Conn.clientStream.Flush();

                msg = new ASCIIEncoding().GetString(buff);

                int playerNum = int.Parse(msg.Substring(3, 1));
                int playerIndex = 0;

                int username_length;
                string username;

                ListBoxItem item;

                int i = 4;

                while (playerIndex < playerNum)
                {
                    username_length = int.Parse(msg.Substring(i, 2));
                    i += 2;

                    username = msg.Substring(i, username_length);
                    i += username_length;

                    item = new ListBoxItem();

                    item.Visibility = Visibility.Hidden;

                    item.Content = username;

                    item.FontFamily = new FontFamily("Arial");
                    item.FontSize = 12;
                    item.FontWeight = FontWeights.Bold;

                    item.Visibility = Visibility.Visible;

                    this.userList.Items.Add(item);

                    playerIndex++;
                }

                Room.roomName = roomName;
                Room.roomID = roomID;

                this.joinButton.IsEnabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }

        private void joinButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = string.Format("209{0}", Room.roomID);
                byte[] buff = new ASCIIEncoding().GetBytes(msg);

                Conn.clientStream.Write(buff, 0, buff.Length);
                Conn.clientStream.Flush();

                buff = new byte[8];

                Conn.clientStream.Read(buff, 0, buff.Length);
                Conn.clientStream.Flush();

                msg = new ASCIIEncoding().GetString(buff);

                if (msg[3] == '0')
                {
                    Room.questionsNum = msg.Substring(4, 2);
                    Room.timePerOne = msg.Substring(6, 2);
                    Room.isAdmin = false;

                    waitRoom sW = new waitRoom();
                    sW.Show();

                    this.Close();
                }
                else if (msg[3] == '1')
                {
                    this.Failed.Content = "failed - room is full ";
                }
                else if (msg[3] == '2')
                {

                    this.Failed.Content = "failed - room not exist or other reason";
                }
            }
            catch(Exception ex)
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
