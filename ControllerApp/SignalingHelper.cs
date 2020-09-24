using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuS._100to1.ControllerApp
{
    internal class SignalingHelper
    {
        public event Action Signal;

        public void Signaling(char cancelKey)
        {
            char key = ' ';
            while (key != cancelKey)
            {
                key = Console.ReadKey().KeyChar;
                if(key != cancelKey)
                    Signal?.Invoke();
            }
        }
    }
}
