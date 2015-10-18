using SIC.Assembler.Utilities.Extensions;
using System;
using System.Collections.Generic;

namespace SIC.Assembler.Model
{
    public class Instruction
    {
        private static readonly Dictionary<string, Instruction> AllInstructions = new Dictionary<string, Instruction>();

        private static readonly string[] AssemblerDirectives = {
            "START",
            "END",
            "BYTE",
            "WORD",
            "RESB",
            "RESW",
            "BASE",
            "EQU",
            "EXTDEF",
            "EXTREF"
        };

        private static Instructions _ALL = null;

        static Instruction()
        {
            AssemblerDirectives.ForEach(directive => AllInstructions.Add(directive, new Instruction
            {
                Format = InstructionFormat.None,
                Mnemonic = directive,
                OpCode = int.MinValue,
                DirectiveType = (AssemblerDirectiveType)Enum.Parse(typeof(AssemblerDirectiveType), directive)
            }));
        }

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

        public int ByteWeight
        {
            get
            {
                return (int)this.Format;
            }
        }

        public AssemblerDirectiveType DirectiveType { get; set; }

        public InstructionFormat Format { get; set; }

        public bool IsAssemblerDirective
        {
            get
            {
                return this.DirectiveType != AssemblerDirectiveType.Unknown;
            }
        }

        public string Mnemonic { get; set; }

        public int OpCode { get; set; }

        public static Instruction Parse(string instruction)
        {
            return All[instruction];
        }

        public static void RegisterInstruction(string mnemonic, string hexOpCode, byte format)
        {
            AllInstructions.Add(mnemonic, new Instruction
            {
                Mnemonic = mnemonic,
                OpCode = int.Parse(hexOpCode, System.Globalization.NumberStyles.HexNumber),
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

                    instructionStr = instructionStr.ToLower();
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