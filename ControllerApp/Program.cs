using DanWahlin;
using Newtonsoft.Json;
using NLog;
using SamuS._100to1.Core;
using SamuS._100to1.Core.Rounds;
using SamuS._100to1.CorePortable;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace SamuS._100to1.ControllerApp
{
    class Program
    {
        static Logger Out { get { return LogManager.GetLogger("Output"); } }

        static SocketServer server;


        static void Main(string[] args)
        {
            server = new SocketServer(4530);
            Thread serverThread = new Thread(server.StartSocketServer);
            serverThread.Start();

            In.Query("Дождитесь подключения всех клиентов и нажмите Enter для начала игры.");
            StartGame();
        }

        public static void StartGame()
        {
            SignalingHelper sameAnswerSignal = new SignalingHelper();
            sameAnswerSignal.Signal += () => {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "SameAnswer"}));
            };

            Game game = new Game();
            game.AnswerRequested += (arg) =>
            {
                foreach (Answer answer in game.CurrentRound.CurrentQuiz)
                    Out.Debug($"[{(answer.Opened ? ' '.ToString() : answer.Index.ToString())}] {answer.Text} : {answer.Score}");
                arg.Result = In.Query($"Какой вариант ответа открыть (0 для ошибки)?");
                arg.Correct = arg.Result.ToString() != "0";

            };
            game.Out += (message) => { Out.Info(message); };
            game.In += (message, args) => { args.Result = In.Query(message); };
            game.OpenAnswer += (answer) =>
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Open", Answer = answer }));
            };
            game.Wrong += (retry) =>
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Wrong", Team = game.ActiveTeam, Retry = retry }));
            };
            game.Nope += () =>
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Nope", Team = game.ActiveTeam }));
            };
            game.RefreshBoard += () =>
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Refresh", Tag = game.CurrentRound.Char }));
            };
            game.ShowBlackboard += () =>
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Blackboard" }));
            };
            game.Title += () =>
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Title" }));
            };
            game.RoundScore += () => 
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "RoundScore", Score = game.CurrentRound.Score }));
            };
            game.Score += () => 
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Score", Team = game.ActiveTeam }));
            };
            game.Timer += () =>
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Timer" }));
            };
            game.Signaling += () =>
            {
                Char cancelKey = '0';
                Out.Info($"Любая клавиша для сигнала. Клавиша '{cancelKey}' для завершения.");
                sameAnswerSignal.Signaling(cancelKey);
            };
            game.Jingle += (fileName) =>
            {
                server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Music", Tag = fileName }));
            };


            game.Add(new Team() { Name = "First team", Score = 0, IsLeft = true })
                .Add(new Team() { Name = "Second team", Score = 0, IsLeft = false })
                .Add(new TupleRound()
                {
                    Name = "Простая игра",
                    ScoreMultiple = 1,
                    Char = "1",
                    Jingle = "/Simple Game.mp3",
                    Quizes = new List<Quiz>()
                    {
                        new Quiz() { Text = "На какой вопрос Олег Геннадьевич ответил “Вам нужно бегать и поститься”?" }
                            .AddAnswer(new Answer() { Text = "По здоровью", Score = 23, Opened = false })
                            .AddAnswer(new Answer() { Text = "На любой", Score = 17, Opened = false })
                            .AddAnswer(new Answer() { Text = "Что делать?", Score = 10, Opened = false })
                            .AddAnswer(new Answer() { Text = "Как похудеть?", Score = 9, Opened = false })
                            .AddAnswer(new Answer() { Text = "Как победить судьбу?", Score = 8, Opened = false })
                            .AddAnswer(new Answer() { Text = "Где взять силы?", Score = 7, Opened = false })
                    }
                })
                .Add(new TupleRound() {
                    Name = "Двойная игра",
                    ScoreMultiple = 2,
                    Char = "2",
                    Jingle = "/Double Game.mp3",
                    Quizes = new List<Quiz>()
                    {
                        new Quiz() { Text = "С чем я ушёл с лекции Олега Геннадьевича?" }
                            .AddAnswer(new Answer() { Text = "С вдохновением", Score = 25, Opened = false })
                            .AddAnswer(new Answer() { Text = "Со знанием/ответом", Score = 14, Opened = false })
                            .AddAnswer(new Answer() { Text = "С благодарностью", Score = 12, Opened = false })
                            .AddAnswer(new Answer() { Text = "С хорошим настроением", Score = 7, Opened = false })
                            .AddAnswer(new Answer() { Text = "С мандаринкой", Score = 6, Opened = false })
                            .AddAnswer(new Answer() { Text = "С цветочком", Score = 5, Opened = false })
                    }
                })
                .Add(new TupleRound() {
                    Name = "Тройная игра",
                    ScoreMultiple = 3,
                    Char = "3",
                    Jingle = "/Triple Game.mp3",
                    Quizes = new List<Quiz>()
                    {
                        new Quiz() { Text = "Каким достижением гордится участник клуба?" }
                            .AddAnswer(new Answer() { Text = "Режимом дня", Score = 25, Opened = false })
                            .AddAnswer(new Answer() { Text = "Служением", Score = 16, Opened = false })
                            .AddAnswer(new Answer() { Text = "Дружбой", Score = 14, Opened = false })
                            .AddAnswer(new Answer() { Text = "Настроением Благости", Score = 12, Opened = false })
                            .AddAnswer(new Answer() { Text = "Победой над собой/судьбой", Score = 10, Opened = false })
                            .AddAnswer(new Answer() { Text = "Он пробежал 10 км", Score = 8, Opened = false })
                    }
                })
                .Add(new ReverseRound() {
                    Name = "Игра наоборот",
                    Char = "?",
                    Jingle = "/Game Vice Versa.mp3",
                    Quizes = new List<Quiz>()
                    {
                        new Quiz() { Text = "На встречу клуба ведущий принёс тяжёлую коробку. Что в ней?" }
                            .AddAnswer(new Answer() { Text = "Книги", Score = 15, Opened = false})
                            .AddAnswer(new Answer() { Text = "Подарки", Score = 30, Opened = false})
                            .AddAnswer(new Answer() { Text = "Фрукты", Score = 60, Opened = false})
                            .AddAnswer(new Answer() { Text = "Угощение", Score = 120, Opened = false})
                            .AddAnswer(new Answer() { Text = "Мандарины", Score = 180, Opened = false})
                            .AddAnswer(new Answer() { Text = "Техника для встречи", Score = 240, Opened = false})
                    }
                })
                .Add(new FinalRound() {
                    Name = "Большая игра",
                    Jingle = "/Big Game.mp3"
                });



            game.Toss += (a) =>
            {
                
                var t1 = new List<char> { '1', '2', '3', '4', '5', 'q', 'w', 'e', 'r', 't', 'a', 's', 'd', 'f', 'g', 'z', 'x', 'c', 'v' };
                var t2 = new List<char> { '7', '8', '9', '0', '-', 'u', 'i', 'o', 'p', '[', 'h', 'j', 'k', 'l', ';', 'n', 'm', ',', '.' };
                var startSq = new char[] { '-', '=' };
                var endSq = new char[] { '[', ']' };

                TossHelper tossHelper = new TossHelper(
                    t1.Union(t2)
                        .ToDictionary(k => k, k => t1.Contains(k) ? game.Teams[0] : game.Teams[1]),
                    startSq,
                    endSq);

                tossHelper.Winner += (team) => { server.SendData(JsonConvert.SerializeObject(new Message() { Command = "Toss", Team = team })); };

                Out.Debug($"Запуск жребия: {string.Join(", ", startSq)}, Останов жребия: {string.Join(", ", endSq)}");

                a.Team = tossHelper.Toss();
            };

            Out.Info($"Игра начинается...");

            Team winner = null;
            game.Play(out winner);

            Out.Info($"Победила команда {winner.Name}. Поздравляем!");
            Out.Info($"Игра окончена.");
            Out.Info($"Press any key to continue");

            Console.ReadLine();
        }


    }
}
