using SamuS._100to1.CorePortable;
using System.Collections.Generic;

namespace SamuS._100to1.Core
{
    public abstract class Round
    {
        public List<Quiz> Quizes { get; set; }

        public Quiz CurrentQuiz { get; set; }

        public Game Game { get; set; }

        public int Score { get; set; }

        public string Name { get; set; }

        public string Jingle { get; set; }

        public string Char { get; set; }

        public abstract int Play(out Team Winner);

        public virtual void Reveal()
        {
            var quiz = CurrentQuiz;
            while (!quiz.IsSolved)
            {
                Answer answer = Game.RequestAnswer(CurrentQuiz);
                if(answer != null)
                    Game.Open(answer);
            }
        }


    }
}
