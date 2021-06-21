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

namespace trivia_clientside
{
    /// <summary>
    /// Interaction logic for register.xaml
    /// </summary>
    public partial class register : Window
    {
        public register()
        {
            InitializeComponent();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Username.Text.Length == 0 || this.Password.Text.Length == 0 || this.Email.Text.Length == 0)
            {
                this.IsFail.Content = "Please fill all fields";
            }
            else
            {
                try
                {
                    string msg = string.Format("203{0}{1}{2}{3}{4}{5}", Helper.NumToStr(this.Username.Text.Length), this.Username.Text, Helper.NumToStr(this.Password.Text.Length), this.Password.Text, Helper.NumToStr(this.Email.Text.Length), this.Email.Text);
                    byte[] buff = new ASCIIEncoding().GetBytes(msg);

                    Conn.clientStream.Write(buff, 0, buff.Length);
                    Conn.clientStream.Flush();

                    buff = new byte[4];

                    Conn.clientStream.Read(buff, 0, 4);
                    Conn.clientStream.Flush();

                    msg = new ASCIIEncoding().GetString(buff);

                    if (msg == "1040")
                    {
                        this.IsFail.Content = "Signed Up Successfully!";
                        MainWindow sW = new MainWindow();
                        sW.Show();

                        this.Close();
                    }
                    else if (msg == "1041")
                    {
                        this.IsFail.Content = "Incorrect Password";
                    }
                    else if (msg == "1042")
                    {
                        this.IsFail.Content = "User is already exist!";
                    }
                    else if (msg == "1043")
                    {
                        this.IsFail.Content = "Username is illegal!";
                    }
                    else if (msg == "1044")
                    {
                        this.IsFail.Content = "Signing Up Failed!";
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Something's wrong, I can Feel it.");
                    this.Close();
                }
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
