using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SamuS._100to1.CorePortable;
using SamuS._100to1.Core;
using System.Diagnostics;

namespace SamuS._100to1.ControllerApp
{
    public class TossHelper
    {
        bool tossEnabled = false, tossCompleted = false;
        char[] startSequence, endSequence;
        Dictionary<char, Team> teamChars;
        Dictionary<Team, Timer> teamTimeouts = new Dictionary<Team, Timer>();

        public TossHelper(Dictionary<char, Team> teamChars, char[] startSequence, char[] endSequence)
        {
            var timer1 = new Timer() { Enabled = false, Interval = 300D };
            timer1.Elapsed += (s, a) => { timer1.Enabled = false; };
            var timer2 = new Timer() { Enabled = false, Interval = 300D };
            timer2.Elapsed += (s, a) => { timer2.Enabled = false; };

            this.teamChars = teamChars;
            this.startSequence = startSequence;
            this.endSequence = endSequence;

            var teams = teamChars.Values.Distinct();

            teamTimeouts.Add(teams.ElementAt(0), timer1);
            teamTimeouts.Add(teams.ElementAt(1), timer2);

        }

        public Game Game { get; set; }

        public Team Toss()
        {
            int startSequenceStep = 0, endSequenceStep = 0;
            Team winner = null;
            while (!tossCompleted)
            {
                var key = Console.ReadKey().KeyChar;
                Debug.WriteLine($"Key pressed: {key}");

                if (startSequenceStep < startSequence.Length && startSequence[startSequenceStep] == key)
                {
                    startSequenceStep++;
                    Debug.WriteLine($"Start sequence process: {startSequenceStep} of {startSequence.Length}");
                }

                if (startSequenceStep == startSequence.Length)
                {
                    tossEnabled = true;
                    Debug.WriteLine($"Enabling toss...");
                }

                if (endSequenceStep < endSequence.Length && endSequence[endSequenceStep] == key)
                {
                    endSequenceStep++;
                    Debug.WriteLine($"End sequence process: {endSequenceStep} of {endSequence.Length}");
                }

                if (endSequenceStep == endSequence.Length)
                {
                    tossCompleted = true;
                    Debug.WriteLine($"Completing toss...");

                }

                if (teamChars.ContainsKey(key))
                {
                    Debug.WriteLine($".. {teamChars[key].Name} timer enabled {teamTimeouts[teamChars[key]].Enabled}");
                    if (!teamTimeouts[teamChars[key]].Enabled)
                    {
                        teamTimeouts[teamChars[key]].Enabled = true;
                        if (tossEnabled)
                        {
                            winner = teamChars[key];
                            tossEnabled = false;
                            startSequenceStep = 0;
                            Debug.WriteLine($"Winner {winner.Name}. Disabling toss...");
                            Winner?.Invoke(winner);
                        }
                    }
                }
            }

            return winner;
        }

        public event Action<Team> Winner;

    }

}
