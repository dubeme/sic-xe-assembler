using System;
using System.Collections.Generic;

namespace SIC.Assembler.Model
{
    public class Instruction
    {
        private static Instructions _ALL = null;
        private static Dictionary<string, Instruction> AllInstructions = new Dictionary<string, Instruction>();

        private Instruction()
        {
        }

        public static Instructions All
        {
            get
            {
                return _ALL ?? (_ALL = new Instructions());
            }
        }

        public InstructionFormat Format { get; set; }

        public string Mnemonic { get; set; }

        public int OpCode { get; set; }

        private static void RegisterInstruction(string mnemonic, int opcode, byte format)
        {
            AllInstructions.Add(mnemonic, new Instruction
            {
                Mnemonic = mnemonic,
                OpCode = opcode,
                Format = (InstructionFormat)Enum.Parse(typeof(InstructionFormat), format.ToString())
            });
        }

        public sealed class Instructions
        {
            public Instruction this[string instructionStr]
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(instructionStr))
                    {
                        return null;
                    }

                    var isFormat4 = instructionStr.StartsWith("+");
                    var instruction = AllInstructions[instructionStr.TrimStart('+')];

                    if (instruction == null)
                    {
                        return null;
                    }

                    // In other to avoid someone modifying the original values for the instruction
                    // I'm returning a totally new object
                    return new Instruction
                    {
                        Mnemonic = instruction.Mnemonic,
                        OpCode = instruction.OpCode,
                        Format = isFormat4 ? InstructionFormat.Four : instruction.Format
                    };
                }
            }
        }
    }
}