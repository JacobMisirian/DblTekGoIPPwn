using System;
using System.IO;

namespace DblTekPwn
{
    public class DblTekPwnArgumentParser
    {
        private string[] args;
        private int position;

        public DblTekPwnConfig Parse(string[] args)
        {
            if (args.Length <= 0)
                displayHelp();

            this.args = args;

            var config = new DblTekPwnConfig();
            for (position = 0; position < args.Length; position++)
            {
                switch (args[position].ToLower())
                {
                    case "-c":
                    case "--compute-response":
                        config.DblTekPwnMode = DblTekPwnMode.ComputeResponse;
                        config.Challenge = expectData("[CHALLENGE]");
                        break;
                    case "-r":
                    case "--root-shell":
                        config.DblTekPwnMode = DblTekPwnMode.Shell;
                        break;
                    case "-s":
                    case "--send-commands":
                        config.DblTekPwnMode = DblTekPwnMode.SendCommands;
                        config.CommandFile = expectData("[COMMAND_FILE]");
                        testFile(config.CommandFile);
                        break;
                    case "-t":
                    case "--test":
                        config.DblTekPwnMode = DblTekPwnMode.TestLogin;
                        break;
                    case "-h":
                    case "--help":
                        displayHelp();
                        break;

                    case "-n":
                    case "--name":
                        config.Host = expectData("[IP]");
                        break;
                    case "-f":
                    case "--file":
                        config.HostFile = expectData("[IP_FILE]");
                        testFile(config.HostFile);
                        break;

                    case "-o":
                    case "--output":
                        config.OutputFile = expectData("[OUTPUT_FILE]");
                        break;

                    default:
                        die("Unknown flag {0}. Run --help for help.", args[position]);
                        break;
                }
            }
            return config;
        }

        private string expectData(string type)
        {
            if (args[++position].StartsWith("-"))
                throw new Exception(string.Format("Expected data type {0}, got flag {1}!", type, args[position]));
            return args[position];
        }

        private void displayHelp()
        {
            Console.WriteLine("USAGE: DblTekPwn.exe [MODE] [HOSTS] [OUTPUT]");
            Console.WriteLine();
            Console.WriteLine("[MODE]:");
            Console.WriteLine("-c --compute-response [CHALLENGE]         Computes a response to the given challenge.");
            Console.WriteLine("-r --root-shell                           Starts a root shell with the vulnerable host.");
            Console.WriteLine("-s --send-commands    [COMMAND_FILE]      Sends commands from a file to vulnerable hosts.");
            Console.WriteLine("-t --test                                 Tests hosts and determines if they are vulnerable.");
            Console.WriteLine("-h --help                                 Displays this help and exits.");
            Console.WriteLine();
            Console.WriteLine("[HOSTS]:");
            Console.WriteLine("-n --name             [IP]                Specifies a single IP address.");
            Console.WriteLine("-f --file             [IP_FILE]           Specifies a file with IP\\nIP\\nIP.");
            Console.WriteLine();
            Console.WriteLine("[OUTPUT]:");
            Console.WriteLine("-o --output           [OUTPUT_FILE]       Specifies an output file. Default stdin.");
            die();
        }

        private void testFile(string filePath)
        {
            if (!File.Exists(filePath))
                die("Error! No such file {0}", filePath);
        }

        private void die(string msg = "", params string[] args)
        {
            Console.WriteLine(msg, args);
            Environment.Exit(0);
        }
    }
}

