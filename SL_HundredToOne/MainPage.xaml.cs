using Newtonsoft.Json;
using SamuS._100to1.CorePortable;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SL_HundredToOne
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            // Required to initialize variables
            InitializeComponent();

            int port = 4530;

            this.Loaded += (s, e) =>
            {
                DnsEndPoint endPoint = new DnsEndPoint(Application.Current.Host.Source.DnsSafeHost, port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.UserToken = socket;
                args.RemoteEndPoint = endPoint;
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSocketConnectCompleted);
                socket.ConnectAsync(args);
            };

            (Resources["TurnScoreToTitle"] as Storyboard).Completed += (s, e) =>
            {
                billet1.FlipToBack();
                billet2.FlipToBack();
                billet3.FlipToBack();
                billet4.FlipToBack();
                billet5.FlipToBack();
                billet6.FlipToBack();
                FirstLeft.TurnOff();
                SecondLeft.TurnOff();
                ThirdLeft.TurnOff();
                FirstRight.TurnOff();
                SecondRight.TurnOff();
                ThirdRight.TurnOff();
                TotalScore.Text = 0.ToString("D3");
                RoundNumberLeft.Text = RoundNumberRight.Text = scoreChar;
            };

            sound.MediaOpened += (s, o) => { (s as MediaElement).Play(); };
            timer.MediaOpened += (s, o) => { (s as MediaElement).Play(); };
        }

        private void OnSocketConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                byte[] response = new byte[1024];
                e.SetBuffer(response, 0, response.Length);
                e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnSocketConnectCompleted);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(OnSocketReceive);
                Socket socket = (Socket)e.UserToken;
                socket.ReceiveAsync(e);
            }
            else
            {
                Dispatcher.BeginInvoke(() => System.Windows.Browser.HtmlPage.Window.Alert(e.SocketError.ToString()));
            }
        }

        private void OnSocketReceive(object sender, SocketAsyncEventArgs e)
        {
            StringReader sr = null;
            try
            {
                string data = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                var message = JsonConvert.DeserializeObject<Message>(data);
                Dispatcher.BeginInvoke(() =>
                {

                    switch (message.Command)
                    {
                        case "Open":
                            if (activeBoard == "Score")
                            {
                                sound.Stop();
                                sound.Source = new Uri("/line_open.mp3", UriKind.Relative);
                                getAnswerBilletByIndex(message.Answer.Index).FlipToFront(message.Answer.Text, message.Answer.Score.ToString());
                            }
                            if (activeBoard == "Blackboard")
                            {
                                var answer = new BlackboardAnswer() { Height = 37D };
                                answer.Text.Text = message.Answer.Text.ToUpper();
                                answer.Score.Text = message.Answer.Score.ToString();
                                (message.Answer.IsLeft ? LeftBlackboard : RightBlackboard)
                                    .Children.Add(answer);
                            }
                            break;
                        case "Refresh":
                            scoreChar = message.Tag;
                            RefreshBoard("Score");
                            break;
                        case "Blackboard":
                            RefreshBoard("Blackboard");
                            break;
                        case "Title":
                            HideBoard(null);
                            break;
                        case "Wrong":
                            sound.Stop();
                            sound.Source = new Uri("/wrong.mp3", UriKind.Relative);

                            getWrongLampByIndex(message.Team.IsLeft, message.Retry).TurnOn();
                            break;
                        case "Nope":
                            sound.Stop();
                            sound.Source = new Uri("/wrong.mp3", UriKind.Relative);
                            var wrongLamp = getWrongLampByIndex(message.Team.IsLeft, 1);
                            wrongLamp.TurnOn();
                            new System.Threading.Timer((o) =>
                            {
                                Dispatcher.BeginInvoke(() => { wrongLamp.TurnOff(); });
                            }, null, 1000, System.Threading.Timeout.Infinite);
                            break;
                        case "RoundScore":
                            if(activeBoard == "Score")
                                TotalScore.Text = message.Score.ToString("D3");
                            else
                                BlackboardScore.Text = message.Score.ToString("D3");
                            break;
                        case "Score":
                            SetScores(message.Team.IsLeft, message.Team.Score);
                            break;
                        case "Timer":
                            timer.Stop();
                            timer.Source = new Uri("/20 Seconds+Alarm.mp3", UriKind.Relative);
                             break;
                        case "SameAnswer":
                            sound.Stop();
                            sound.Source = new Uri("/The Same Answer.mp3", UriKind.Relative);
                            break;
                        case "Music":
                            sound.Stop();
                            sound.Source = new Uri(message.Tag, UriKind.Relative);
                            break;
                        case "Toss":
                            sound.Stop();
                            sound.Source = new Uri("/button.mp3", UriKind.Relative);
                            Storyboard animation = Resources[message.Team.IsLeft ? "BlinkingScoreLeft" : "BlinkingScoreRight"] as Storyboard;
                            animation.Begin();
                            break;
                    }
                });
            }
            catch (Exception)
            {
            }

            //Prepare to receive more data
            Socket socket = (Socket)e.UserToken;
            socket.ReceiveAsync(e);
        }

        private void RefreshBoard(string board)
        {
            if (activeBoard != "Title")
            {
                HideBoard((s, a) =>
                {
                    activeBoard = board;
                    OpenBoard();
                });
            }
            else
            {
                activeBoard = board;
                OpenBoard();
            }
        }

        AnswerBilletControl getAnswerBilletByIndex(int index)
        {
            switch (index)
            {
                case 1: return billet1;
                case 2: return billet2;
                case 3: return billet3;
                case 4: return billet4;
                case 5: return billet5;
                case 6: return billet6;
                default: return null;
            }
        }

        WrongAnswerLampControl getWrongLampByIndex(bool leftTeam, int index)
        {
            switch (index)
            {
                case 1: return leftTeam? FirstLeft : FirstRight;
                case 2: return leftTeam ? SecondLeft : SecondRight;
                case 3: return leftTeam ? ThirdLeft : ThirdRight;
                default: return null;
            }
        }

        string activeBoard = "Title";
        string scoreChar = "";

        void OpenBoard()
        {
            (Resources[$"TurnTitleTo{activeBoard}"] as Storyboard).Begin();
        }

        void HideBoard(EventHandler continueDelegate)
        {
            if (activeBoard == "Title")
                return;
            Storyboard animation = (Resources[$"Turn{activeBoard}ToTitle"] as Storyboard);
            if (continueDelegate != null)
            {
                animation.Completed += continueDelegate;
                animation.Completed += (s, a) => { animation.Completed -= continueDelegate; };
            }
            animation.Begin();
            activeBoard = "Title";
        }

        void SetScores(bool IsLeftTeam, int Score)
        {
            (IsLeftTeam ? LeftScore : RightScore).Text = Score.ToString("D3");
        }
    }
}