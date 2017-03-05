using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DblTekPwn
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new DblTekPwn(args[0]).Pwn());
        }
    }
}
