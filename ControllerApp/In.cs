using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuS._100to1.ControllerApp
{
    public class In
    {
        static Logger Out { get { return LogManager.GetLogger("Output"); } }

        public static string Query(string Promt)
        {
            Out.Info($"{Promt}{Environment.NewLine}>");
            return Console.ReadLine();
        }
    }
}
