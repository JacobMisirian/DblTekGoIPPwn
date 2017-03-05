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
            DblTekPwnConfig.Execute(new DblTekPwnArgumentParser().Parse(args));
        }
    }
}
