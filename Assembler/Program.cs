using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC.Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder a = new StringBuilder("SDSDSDSDSDeqwer");
            Console.WriteLine(a);
            change( a);
            Console.WriteLine(a);
        }

        static void change( StringBuilder o)
        {
            o = new StringBuilder("New value");
            Console.WriteLine("\n\n\n\n");
            Console.WriteLine(o);
            Console.WriteLine("\n\n\n\n");
        }
    }
}
