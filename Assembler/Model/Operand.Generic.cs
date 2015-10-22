using SIC.Assembler.Providers;
using System;
using System.Linq;
using System.Text;

namespace SIC.Assembler.Model
{
    public class Operand<T> : Operand
    {
        public T Value { get; set; }

        public static Operand<T> Parse()
        {

            return null;
        }
    }
}