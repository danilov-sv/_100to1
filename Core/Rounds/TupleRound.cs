using SamuS._100to1.CorePortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuS._100to1.Core.Rounds
{
    public class TupleRound : Round
    {
        public int ScoreMultiple { get; set; }

        public TupleRound()
        {
            ScoreMultiple = 1;
        }

        public override int Play(out Team Winner)
        {
            CurrentQuiz = Quizes[0];

            //Toss phase
            Game.RequestOut("-- Розыгрыш раунда --");
            string activeTeamIndex = Game.RequestIn("Какая команда выиграла жребий (1, 2)?");
            Game.SetActiveTeam(activeTeamIndex == "1" ? Game.Teams.First() : Game.Teams.Last());

            int tossScore1 = 0, tossScore2 = 0;
            while(tossScore1 == tossScore2)
            {
                Answer answer1 = Game.RequestAnswer(CurrentQuiz);
                if (answer1 != null)
                {
                    Game.Open(answer1);
                    Score += (tossScore1 = answer1.Score) * ScoreMultiple;
                    Game.SetRoundScore();

                    if (answer1.Index == 1)
                        break;
                }
                else
                    Game.AnswerNope();
                Game.ChangeActiveTeam();

                Answer answer2 = Game.RequestAnswer(CurrentQuiz);
                if (answer2 != null)
                {
                    Game.Open(answer2);
                    Score += (tossScore2 = answer2.Score) * ScoreMultiple;
                    Game.SetRoundScore();
                }
                else
                    Game.AnswerNope();
                Game.ChangeActiveTeam();

                if (tossScore2 > tossScore1)
                    Game.ChangeActiveTeam();
            }
            Game.RequestOut($"Раунд играет команда {Game.ActiveTeam.Name}");

            //Main Phase
            Game.RequestOut("-- Раунд --");
            int retry = 0, maxRetries = 3;
            do
            {
                Answer answer = Game.RequestAnswer(CurrentQuiz);
                if (answer == null)
                {
                    retry++;
                    Game.AnswerWrong(retry);
                    if (retry == maxRetries)
                        break;
                }
                else
                {
                    Game.Open(answer);
                    Score += answer.Score * ScoreMultiple;
                    Game.SetRoundScore();
                }
            }
            while (!CurrentQuiz.IsSolved);

            //Blitz Phase
            Game.RequestOut("-- Блиц-опрос --");
            if (!CurrentQuiz.IsSolved)
            {
                Game.ChangeActiveTeam();
                Answer answer = Game.RequestAnswer(CurrentQuiz);
                if(answer == null)
                {
                    Game.AnswerNope();
                    Game.ChangeActiveTeam();
                }
                else
                {
                    Game.Open(answer);
                    Score += answer.Score * ScoreMultiple;
                    Game.SetRoundScore();
                }
            }

            Winner = Game.ActiveTeam;
            return Score;
        }
    }
}
