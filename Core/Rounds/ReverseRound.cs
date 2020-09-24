using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamuS._100to1.CorePortable;

namespace SamuS._100to1.Core.Rounds
{
    public class ReverseRound : Round
    {
        public override int Play(out Team Winner)
        {
            CurrentQuiz = Quizes[0];
            Game.SetActiveTeam(Game.Teams.OrderBy(team => team.Score)
                .Take(1)
                .Single());

            Game.RequestOut("-- Раунд --");

            Game.RequestIn("Запустите таймер после того, как будет задан вопрос");
            Game.StartTimer();

            while (!CurrentQuiz.IsSolved)
            {
                Answer answer = Game.RequestAnswer(CurrentQuiz);
                if (answer != null)
                {
                    Team team = ChooseTeam();
                    Game.Open(answer);
                    if (team != null)
                    {
                        team.Score += answer.Score;
                        Game.SetActiveTeam(team);
                        Game.SetScore();
                    }
                    else
                        Game.AnswerNope();
                }
                else
                    Game.AnswerNope();
            }

            Winner = Game.Teams.OrderByDescending(team => team.Score)
                .Take(1)
                .Single();

            return 0;
        }

        public override void Reveal()
        {
            return;
        }

        Team ChooseTeam()
        {
            int number = 0;
            while (true)
            {
                string answer = Game.RequestIn("Какой команде зачислить очки? (1, 2 или 0)");
                if (int.TryParse(answer, out number) && new[] { 0, 1, 2 }.Contains(number))
                    break;
                Game.RequestOut("Не могу распознать ответ!");
            }

            switch (number)
            {
                default:
                    return null;
                case 1:
                    return Game.Teams[0];
                case 2:
                    return Game.Teams[1];
            }
        }
    }
}
