using SIC.Assembler.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIC.Assembler.Providers
{
    public class OperandEvaluator
    {
        public static LiteralTable ParseExpressions(IList<string> expressions, SymbolTable symbolTable, Action<object> printFunction, Action<object> errorFunction)
        {
            if (expressions == null || symbolTable == null)
            {
                return null;
            }

            var literalTable = new LiteralTable();

            printFunction(Operand.HeaderText());
            foreach (var operandString in expressions.Where(str => !string.IsNullOrWhiteSpace(str)))
            {
                try
                {
                    printFunction(Operand.CreateOperand(operandString.ToLower(), 0, symbolTable, literalTable));
                }
                catch (Exception ex)
                {
                    errorFunction(string.Format("Input = {0}\n{1}", operandString, ex.Message));
                }
            }

            return literalTable;
        }
    }
}