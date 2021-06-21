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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace trivia_clientside
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class Conn
    {
        public static TcpClient client;
        public static IPEndPoint serverEndPoint;
        public static NetworkStream clientStream;
    }

   

    public class Helper
    {
        public static string NumToStr(int num)
        {
            if (num >= 10 && num <= 99)
            {
                return string.Format("{0}", num);
            }
            else
            {
                return string.Format("0{0}", num);
            }
        }

        public static string Username;
        public static string Password;
        public static string Email;
        public static bool signedIn = false;
        public static bool connected = false;
    }

    public class Room
    {
        public static string roomName;
        public static string maxPlayers;
        public static string questionsNum;
        public static string timePerOne;
        public static string roomID;
        public static bool isAdmin;
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                if (!Helper.connected)
                {
                    Conn.client = new TcpClient();
                    Conn.serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8825);

                    Conn.client.Connect(Conn.serverEndPoint);

                    Conn.clientStream = Conn.client.GetStream();
                    Helper.connected = true;

                }

                if (Helper.signedIn)
                {
                    this.SignInButton.IsEnabled = false;
                    this.SignUpButton.IsEnabled = false;

                    this.NewRoomButton.IsEnabled = true;
                    this.JoinButton.IsEnabled = true;
                    this.MyStatusButton.IsEnabled = true;
                    this.BestButton.IsEnabled = true;
                    this.SignOutButton.IsEnabled = true;

                    this.SignInButton.Foreground = Brushes.Gray;
                    this.SignUpButton.Foreground = Brushes.Gray;

                    this.NewRoomButton.Foreground = Brushes.Black;
                    this.JoinButton.Foreground = Brushes.Black;
                    this.MyStatusButton.Foreground = Brushes.Black;
                    this.BestButton.Foreground = Brushes.Black;
                    this.SignOutButton.Foreground = Brushes.Black;

                    this.Welcome.Content = "Hello " + Helper.Username + "!";
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Username.Text.Length == 0 || this.Password.Text.Length == 0)
                {
                    this.Welcome.Content = "Please fill all fields";
                }
                else
                {
                    string msg = string.Format("200{0}{1}{2}{3}", Helper.NumToStr(this.Username.Text.Length), this.Username.Text, Helper.NumToStr(this.Password.Text.Length), this.Password.Text);
                    byte[] buff = new ASCIIEncoding().GetBytes(msg);

                    Conn.clientStream.Write(buff, 0, buff.Length);
                    Conn.clientStream.Flush();

                    buff = new byte[4];

                    Conn.clientStream.Read(buff, 0, 4);
                    Conn.clientStream.Flush();
                    msg = new ASCIIEncoding().GetString(buff);

                    if (msg == "1020")
                    {
                        this.Welcome.Content = "Hello " + this.Username.Text + "!";

                        Helper.Username = this.Username.Text;
                        Helper.Password = this.Password.Text;

                        Helper.signedIn = true;

                        this.SignInButton.IsEnabled = false;
                        this.SignUpButton.IsEnabled = false;

                        this.NewRoomButton.IsEnabled = true;
                        this.JoinButton.IsEnabled = true;
                        this.MyStatusButton.IsEnabled = true;
                        this.BestButton.IsEnabled = true;
                        this.SignOutButton.IsEnabled = true;

                        this.SignInButton.Foreground = Brushes.Gray;
                        this.SignUpButton.Foreground = Brushes.Gray;

                        this.NewRoomButton.Foreground = Brushes.Black;
                        this.JoinButton.Foreground = Brushes.Black;
                        this.MyStatusButton.Foreground = Brushes.Black;
                        this.BestButton.Foreground = Brushes.Black;
                        this.SignOutButton.Foreground = Brushes.Black;

                        this.Username.IsReadOnly = true;
                        this.Password.IsReadOnly = true;

                    }
                    else if (msg == "1021")
                    {
                        this.Welcome.Content = "Wrong details!";
                    }
                    else if (msg == "1022")
                    {
                        this.Welcome.Content = "User is already connected!";
                    }

                    this.Username.Text = string.Empty;
                    this.Password.Text = string.Empty;

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }
        

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            register sW = new register();
            sW.Show();

            this.Close();
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buff = new ASCIIEncoding().GetBytes("201");

                Conn.clientStream.Write(buff, 0, buff.Length);
                Conn.clientStream.Flush();

                this.SignInButton.IsEnabled = true;
                this.SignUpButton.IsEnabled = true;

                this.NewRoomButton.IsEnabled = false;
                this.JoinButton.IsEnabled = false;
                this.MyStatusButton.IsEnabled = false;
                this.BestButton.IsEnabled = false;
                this.SignOutButton.IsEnabled = false;


                this.SignInButton.Foreground = Brushes.Black;
                this.SignUpButton.Foreground = Brushes.Black;

                this.NewRoomButton.Foreground = Brushes.Gray;
                this.JoinButton.Foreground = Brushes.Gray;
                this.MyStatusButton.Foreground = Brushes.Gray;
                this.BestButton.Foreground = Brushes.Gray;
                this.SignOutButton.Foreground = Brushes.Gray;

                this.Username.IsReadOnly = false;
                this.Password.IsReadOnly = false;


                Helper.Username = string.Empty;
                Helper.Password = string.Empty;
                Helper.Email = string.Empty;
                Helper.signedIn = false;
                this.Welcome.Content = string.Empty;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something's wrong, I can Feel it.");
                this.Close();
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        { 
            this.Close();
        }

        private void NewRoomButton_Click(object sender, RoutedEventArgs e)
        {
            newRoom1 sW = new newRoom1();
            sW.Show();

            this.Close();
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            joinRoom sW = new joinRoom();
            sW.Show();

            this.Close();
        }

        private void MyStatusButton_Click(object sender, RoutedEventArgs e)
        {
            myStatus sW = new myStatus();
            sW.Show();

            this.Close();
        }

        private void BestButton_Click(object sender, RoutedEventArgs e)
        {
            bestScores sW = new bestScores();
            sW.Show();

            this.Close();
        }
    }
}
