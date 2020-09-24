using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuS._100to1.CorePortable
{
    public class Quiz: IEnumerable<Answer>
    {
        public string Text { get; set; }

        private List<Answer> answers = new List<Answer>();
        public Quiz AddAnswer(Answer Answer)
        {
            answers.Add(Answer);
            Answer.Index = answers.Count;
            return this;
        }

        public bool IsSolved { get { return !answers.Where(a => !a.Opened).Any(); } }

        public int Index(Answer Answer)
        {
            return answers.IndexOf(Answer) + 1;
        }

        public IEnumerator<Answer> GetEnumerator()
        {
            return answers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return answers.GetEnumerator();
        }

        public Answer this[int i]
        {
            get { return answers.ElementAt(i - 1); }
        }

        public int Count { get { return answers.Count; } }
    }
}
