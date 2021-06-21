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
using System.ComponentModel;

namespace trivia_clientside
{
    /// <summary>
    /// Interaction logic for question.xaml
    /// </summary>

    public partial class question : Window
    {

        public int timer = int.Parse(Room.timePerOne);
        public BackgroundWorker worker;
        public BackgroundWorker nextQ;
        public static bool quit;
        public static bool cancel;
        public string msg;
        public byte[] buff;

        private void Timer(object sender, DoWorkEventArgs e)
        {
            while (timer != 0 && !worker.CancellationPending)
            {
                worker.ReportProgress(1, timer);
                Thread.Sleep(1000);
                timer--;

            }
        }

        private void NextQUpdateUI(object sender, ProgressChangedEventArgs e)
        {
            return;
        }


        private void NextQEnd(object sender, RunWorkerCompletedEventArgs e)
        {
            if (FirstAnswer.msg.StartsWith("118"))
            {
                FirstAnswer.questionNumber++;
                question sW = new question();

                this.Close();
            }
            else if (FirstAnswer.msg.StartsWith("121"))
            {
                string finishMsg = "username: " + Helper.Username + ", score: " + FirstAnswer.score;
                MessageBox.Show(finishMsg);
                MainWindow sw = new MainWindow();
                sw.Show();
                this.Close();
            }
        }

        private void NextQuestion(object sender, DoWorkEventArgs e)
        {
            buff = new byte[4096];

            Conn.clientStream.Read(buff, 0, buff.Length);
            Conn.clientStream.Flush();

            FirstAnswer.msg = new ASCIIEncoding().GetString(buff);
        }

        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {
            this.Clock.Content = (int)e.UserState;
        }

        private void TimerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (quit) return;

            if (!cancel)
            {
                string msg_ = string.Format("2195{0}", int.Parse(Room.timePerOne) - timer);
                byte[] buff_ = new ASCIIEncoding().GetBytes(msg_);

                Conn.clientStream.Write(buff_, 0, buff_.Length);
                Conn.clientStream.Flush();

               
            }
            buff = new byte[4];

            Conn.clientStream.Read(buff, 0, buff.Length);
            Conn.clientStream.Flush();

            msg = new ASCIIEncoding().GetString(buff);

            if (msg[3] == '1')
            {
                FirstAnswer.score++;

                this.Correct.Foreground = Brushes.Green;
                this.Correct.Content = "Correct Answer!";
            }
            else
            {
                this.Correct.Foreground = Brushes.Red;
                this.Correct.Content = "Incorrect Answer!";
            }

            cancel = false;
            timer = int.Parse(Room.timePerOne);

            nextQ = new BackgroundWorker();

            nextQ.WorkerSupportsCancellation = true;
            nextQ.WorkerReportsProgress = true;

            nextQ.DoWork += NextQuestion;
            nextQ.ProgressChanged += NextQUpdateUI;
            nextQ.RunWorkerCompleted += NextQEnd;

            nextQ.RunWorkerAsync();
        }

        public question()
        {
            InitializeComponent();


            question.quit = false;
            cancel = false;

            Thread.Sleep(10);
            msg = FirstAnswer.msg;
            this.Number.Content = string.Format("Question {0} of {1}:", FirstAnswer.questionNumber, int.Parse(Room.questionsNum));
            this.User.Content = Helper.Username;
            this.Room1.Content = Room.roomName;
            this.Clock.Content = int.Parse(Room.timePerOne);
            this.Score.Content = string.Format("Score: {0}/{1}", FirstAnswer.score, int.Parse(Room.questionsNum));
            this.Correct.Content = "";

            int questionLength = int.Parse(msg.Substring(3, 3));
            this.Question.Content = msg.Substring(6, questionLength);

            int firstLen = int.Parse(msg.Substring(questionLength + 6, 3));
            this.Answer1.Content = msg.Substring(questionLength + 9, firstLen);

            int secondLen = int.Parse(msg.Substring(questionLength + firstLen + 9, 3));
            this.Answer2.Content = msg.Substring(questionLength + firstLen + 12, secondLen);

            int thirdLen = int.Parse(msg.Substring(questionLength + firstLen + secondLen + 12, 3));
            this.Answer3.Content = msg.Substring(questionLength + firstLen + secondLen + 15, thirdLen);

            int fourthLen = int.Parse(msg.Substring(questionLength + firstLen + secondLen + thirdLen + 15, 3));
            this.Answer4.Content = msg.Substring(questionLength + firstLen + secondLen + thirdLen + 18, fourthLen);
            this.Show();


            worker = new BackgroundWorker();

            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            worker.DoWork += Timer;
            worker.ProgressChanged += UpdateUI;
            worker.RunWorkerCompleted += TimerCompleted;

            worker.RunWorkerAsync();
        }

        private void Answer1_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Format("2191{0}", int.Parse(Room.timePerOne) - timer);
            byte[] buff = new ASCIIEncoding().GetBytes(msg);

            Conn.clientStream.Write(buff, 0, buff.Length);
            Conn.clientStream.Flush();

            cancel = true;
            worker.CancelAsync();
        }

        private void Answer2_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Format("2192{0}", int.Parse(Room.timePerOne) - timer);
            byte[] buff = new ASCIIEncoding().GetBytes(msg);

            Conn.clientStream.Write(buff, 0, buff.Length);
            Conn.clientStream.Flush();

            cancel = true;
            worker.CancelAsync();
        }

        private void Answer3_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Format("2193{0}", int.Parse(Room.timePerOne) - timer);
            byte[] buff = new ASCIIEncoding().GetBytes(msg);

            Conn.clientStream.Write(buff, 0, buff.Length);
            Conn.clientStream.Flush();

            cancel = true;
            worker.CancelAsync();
        }

        private void Answer4_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Format("2194{0}", int.Parse(Room.timePerOne) - timer);
            byte[] buff = new ASCIIEncoding().GetBytes(msg);

            Conn.clientStream.Write(buff, 0, buff.Length);
            Conn.clientStream.Flush();

            cancel = true;
            worker.CancelAsync();
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {

            byte[] buff = new ASCIIEncoding().GetBytes("222");
            quit = true;

            Conn.clientStream.Write(buff, 0, buff.Length);
            Conn.clientStream.Flush();

            MainWindow sW = new MainWindow();
            sW.Show();
            worker.CancelAsync();

            this.Close();

        }
    }
}