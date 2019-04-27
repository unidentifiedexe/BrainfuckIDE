using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodeMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            var inStr = Console.ReadLine();
            var convertesStr = AutoMaker.AutoMake(inStr);
            Console.WriteLine(convertesStr);
            Console.ReadLine();
        }
    }
}
