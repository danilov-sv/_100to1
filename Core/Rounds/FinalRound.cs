using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamuS._100to1.CorePortable;

namespace SamuS._100to1.Core.Rounds
{
    public class FinalRound: Round
    {
        public override int Play(out Team Winner)
        {
            bool firstPlayer = true;

            Winner = Game.Teams.OrderByDescending(t => t.Score).Take(1).Single();
            Game.SetActiveTeam(Winner);

            CurrentQuiz = new Quiz();

            for (int stage = 0; stage < 2; stage++)
            {
                firstPlayer = stage == 0;

                Game.RequestIn($"{(firstPlayer ? "Первому" : "Второму")} игроку приготовиться к запуску таймера!");
                foreach (var answer in CurrentQuiz)
                    Game.RequestOut($"[{answer.Index}] {answer.Text}");

                //Game.StartTimer();
                Game.InstantSignaling();
                Game.Blackboard();

                for (int index = 0; index < 5; index++)
                {
                    Answer answer = requestAnswer();
                    answer.IsLeft = firstPlayer;
                    CurrentQuiz.AddAnswer(answer);
                    Game.Open(answer);
                    Score += answer.Score;
                    Game.SetRoundScore();
                }

                if(firstPlayer)
                {
                    Game.RequestIn($"Нажмите Enter чтобы закрыть табло и повозите второго игрока!");
                    Game.ShowTitle();
                }
            }

            return 0;
        }

        public override void Reveal()
        {
            return;
        }

        Answer requestAnswer()
        {
            string text = null;
            int score = 0;
            while (true)
            {
                string[] answer = Game.RequestIn($"Введите ответ и стоимость через ':'").Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (answer.Length != 2 || !int.TryParse(answer[1], out score)) 
                {
                    Game.Warn("Неверный формат.");
                    continue;
                }
                text = answer[0];
                break;
            }
            return new Answer() { Text = text, Score = score };
        }
    }
}
