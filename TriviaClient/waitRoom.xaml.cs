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
using System.ComponentModel;

namespace trivia_clientside
{
    /// <summary>
    /// Interaction logic for waitRoom.xaml
    /// </summary>
    /// 

    public class FirstAnswer
    {
        public static string msg;
        public static int questionNumber = 0;
        public static int score = 0;
    }

    public partial class waitRoom : Window
    {

        BackgroundWorker worker;
        bool close;
        bool except;

        private void showUsers(object sender, EventArgs e)
        {
            try
            {
                byte[] buff;
                while (!worker.CancellationPending)
                {
                    buff = new byte[4096];

                    Conn.clientStream.Read(buff, 0, buff.Length);
                    Conn.clientStream.Flush();

                    string msg = new ASCIIEncoding().GetString(buff);
                    if ((!msg.StartsWith("118")) && (!msg.StartsWith("116")))
                    {
                        worker.ReportProgress(50, msg);
                    }
                    else
                    {
                        FirstAnswer.msg = msg;
                        FirstAnswer.questionNumber = 1;
                        worker.CancelAsync();

                    }

                }
            }
            catch(Exception ex)
            {
                except = true;
                worker.CancelAsync();
            }
        }

        private void updateList(object sender, ProgressChangedEventArgs e)
        {
            string msg = (string)e.UserState;

            this.playersList.Items.Clear();

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
                item.FontSize = 15;
                item.FontWeight = FontWeights.Bold;

                item.Visibility = Visibility.Visible;
                this.playersList.Items.Add(item);

                playerIndex++;
            }
        }

        private void endThread(object sender, AsyncCompletedEventArgs e)
        {
            if(except)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
                return;
            }

            if (FirstAnswer.msg.StartsWith("118"))
            {
                question sw = new question();
            }
            else if (FirstAnswer.msg.StartsWith("116"))
            {
                MainWindow s = new MainWindow();
                s.Show();
            }

            this.Close();
            return;
        }

        public waitRoom()
        {
            InitializeComponent();
            Thread.Sleep(500);

            close = false;
            except = false;

            try
            {
                this.roomName.Content = string.Format("You are connected to room {0}", Room.roomName);
                this.questionNum.Content = string.Format("Questions:      {0}", int.Parse(Room.questionsNum));
                this.questionTime.Content = string.Format("Time per One: {0}", int.Parse(Room.timePerOne));

                if (!Room.isAdmin)
                {
                    this.startButton.Visibility = Visibility.Hidden;
                    this.closeButton.Visibility = Visibility.Hidden;

                    byte[] buff = new byte[4096];

                    Conn.clientStream.Read(buff, 0, buff.Length);
                    Conn.clientStream.Flush();

                    string msg = new ASCIIEncoding().GetString(buff);

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
                        item.FontSize = 15;
                        item.FontWeight = FontWeights.Bold;

                        item.Visibility = Visibility.Visible;
                        this.playersList.Items.Add(item);

                        playerIndex++;
                    }
                }
                else
                {
                    this.leaveButton.Visibility = Visibility.Hidden;

                    ListBoxItem item = new ListBoxItem();
                    item.Visibility = Visibility.Hidden;

                    item.Content = Helper.Username;

                    item.FontFamily = new FontFamily("Arial");
                    item.FontSize = 15;
                    item.FontWeight = FontWeights.Bold;

                    item.Visibility = Visibility.Visible;
                    this.playersList.Items.Add(item);
                }

                worker = new BackgroundWorker();

                worker.WorkerSupportsCancellation = true;
                worker.WorkerReportsProgress = true;

                worker.DoWork += showUsers;
                worker.ProgressChanged += updateList;
                worker.RunWorkerCompleted += endThread;
                worker.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                worker.CancelAsync();

                byte[] buff = new ASCIIEncoding().GetBytes("217");
                Conn.clientStream.Flush();

                Conn.clientStream.Write(buff, 0, buff.Length);
                Conn.clientStream.Flush();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }           
        }


        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                close = true;
                worker.CancelAsync();

                byte[] buff = new ASCIIEncoding().GetBytes("215");
                Conn.clientStream.Flush();

                Conn.clientStream.Write(buff, 0, buff.Length);
                Conn.clientStream.Flush();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }

        private void leaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                close = true;
                worker.CancelAsync();

                byte[] buff = new ASCIIEncoding().GetBytes("211");
                Conn.clientStream.Flush();

                Conn.clientStream.Write(buff, 0, buff.Length);
                Conn.clientStream.Flush();
                FirstAnswer.msg = "211";
                MainWindow sW = new MainWindow();
                sW.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }
    }
}
