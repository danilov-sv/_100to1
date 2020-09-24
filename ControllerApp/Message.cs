using SamuS._100to1.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SamuS._100to1.ControllerApp
{
    public class Message
    {
        public string Command { get; set; }

        public Quiz Quiz { get; set; }

        public Answer Answer { get; set; }

        public int Retry { get; set; }

        public Team Team { get; set; }
    }
}
