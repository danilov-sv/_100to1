using SamuS._100to1.CorePortable;
using SamuS._100to1.Core.Rounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuS._100to1.Core
{
    public class Game
    {
        public List<Round> Rounds { get; }

        public FinalRound FinalRound { get; set; }

        public List<Team> Teams { get; }

        public Team ActiveTeam { get; protected set; }

        public Round CurrentRound { get; protected set; }

        public Game()
        {
            Rounds = new List<Round>();
            Teams = new List<Team>();
        }

        public void Play(out Team Winner)
        {
            Start?.Invoke();
            Jingle?.Invoke("/Opening.mp3");
            RequestIn("Готовы к следующему раунду?");

            foreach (Round round in Rounds)
            {
                CurrentRound = round;
                RequestOut($"== {round.Name} ==");
                Jingle?.Invoke(round.Jingle);
                RefreshBoard?.Invoke();

                int score = round.Play(out Winner);
                Winner.Score += score;
                SetScore();

                RequestOut("-- Открываем табло --");
                round.Reveal();
                RequestIn("Готовы к следующему раунду?");
            }

            CurrentRound = FinalRound;
            RequestOut($"== {FinalRound.Name} ==");
            Jingle?.Invoke(FinalRound.Jingle);
            ShowTitle();
            FinalRound.Play(out Winner);

            Winner = Teams.OrderByDescending(team => team.Score)
                .Take(1)
                .Single();

            Jingle?.Invoke("/Closing.mp3");
            RequestIn("Нажмите для завершения игры");
            ShowTitle();
            TheEnd?.Invoke();
        }

        public Game Add(Round Round)
        {
            Rounds.Add(Round);
            Round.Game = this;
            return this;
        }

        public Game Add(FinalRound Round)
        {
            FinalRound = Round;
            FinalRound.Game = this;
            return this;
        }

        public Game Add(Team Team)
        {
            Teams.Add(Team);
            return this;
        }

        public event Action<QueryArgs> AnswerRequested;

        public Answer RequestAnswer(Quiz Quiz)
        {
            Answer answer = null;
            do
            {
                QueryArgs args = new QueryArgs();
                AnswerRequested?.Invoke(args);

                if (!args.Correct)
                    return null;

                int answerIndex = 0;
                if (!int.TryParse(Convert.ToString(args.Result), out answerIndex))
                {
                    Warn($"Не могу преобразовать '{args.Result}' в номер ответа!");
                    continue;
                }

                if (answerIndex <= 0 || answerIndex > Quiz.Count)
                {
                    Warn($"Введите число от 1 до '{Quiz.Count}'!");
                    continue;
                }

                answer = Quiz[answerIndex];
                if (answer.Opened)
                    Warn($"Вопрос номер {Quiz.Index(answer)} уже открыт!");
            }
            while (answer == null || answer.Opened);

            return answer;
        }

        public void Warn(string message)
        {
            Out?.Invoke(message);
        }

        public void RequestOut(string message)
        {
            Out?.Invoke(message);
        }

        public string RequestIn(string message)
        {
            QueryArgs args = new QueryArgs();
            In?.Invoke(message, args);
            return Convert.ToString(args.Result);
        }

        public event Action<string> Out;
        public event Action<string, QueryArgs> In;
        public event Action Start;
        public event Action RefreshBoard;
        public event Action ShowBlackboard;
        public event Action Title;
        public event Action<Answer> OpenAnswer;
        public event Action<int> Wrong;
        public event Action Nope;
        public event Action Score;
        public event Action RoundScore;
        public event Action TheEnd;
        public event Action Timer;
        public event Action Signaling;
        public event Action<string> Jingle;
        public event Action<TossArgs> Toss;

        public void Open(Answer answer)
        {
            answer.Opened = true;
            OpenAnswer?.Invoke(answer);
        }

        public void SetActiveTeam(Team Team)
        {
            ActiveTeam = Team;
            Out.Invoke($"Переход хода к команде {ActiveTeam.Name}");
        }

        public void ChangeActiveTeam()
        {
            ActiveTeam = ActiveTeam == Teams[0] ? Teams[1] : Teams[0];
            Out.Invoke($"Переход хода к команде {ActiveTeam.Name}");
        }

        internal void AnswerWrong(int retryNumber)
        {
            Out.Invoke($"Ответ отсутствует. Ошибка номер {retryNumber}.");
            Wrong?.Invoke(retryNumber);
        }

        internal void AnswerNope()
        {
            Nope?.Invoke();
        }

        internal void SetScore()
        {
            Score?.Invoke();
        }

        internal void SetRoundScore()
        {
            Out.Invoke($"Выиграно очков в раунде: {CurrentRound.Score}.");
            RoundScore?.Invoke();
        }

        internal void StartTimer()
        {
            Timer?.Invoke();
        }

        internal void InstantSignaling()
        {
            Signaling?.Invoke();
        }

        internal void Blackboard() { ShowBlackboard?.Invoke(); }

        internal void ShowTitle() { Title?.Invoke(); }

        internal Team RequestToss()
        {
            TossArgs args = new TossArgs();
            Toss?.Invoke(args);
            return args.Team;
        }
    }
}
