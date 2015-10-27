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
        private string _mnemonic;

        static Instruction()
        {
            AssemblerDirectives.ForEach(directive => AddInstruction(
                directive,
                int.MinValue,
                InstructionFormat.None,
                (AssemblerDirectiveType)Enum.Parse(typeof(AssemblerDirectiveType), directive, true))
            );
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

        public string Mnemonic
        {
            get
            {
                if (this.Format == InstructionFormat.Four)
                {
                    return "+" + this._mnemonic;
                }
                return this._mnemonic;
            }
            private set
            {
                this._mnemonic = value;
            }
        }

        public int OpCode { get; set; }

        public static Instruction Parse(string instruction)
        {
            return All[instruction];
        }

        public static void RegisterInstruction(string mnemonic, string hexOpCode, byte format)
        {
            if (string.IsNullOrWhiteSpace(mnemonic))
            {
                throw new ArgumentException("Mnemonic can't be empty OR null", nameof(mnemonic));
            }

            if (string.IsNullOrWhiteSpace(hexOpCode))
            {
                throw new ArgumentException("HexOpCode can't be empty OR null", nameof(hexOpCode));
            }

            AddInstruction(
                mnemonic,
                int.Parse(hexOpCode, System.Globalization.NumberStyles.HexNumber),
                (InstructionFormat)Enum.Parse(typeof(InstructionFormat), format.ToString()),
                AssemblerDirectiveType.Unknown);
        }

        private static void AddInstruction(string mnemonic, int opCode, InstructionFormat format, AssemblerDirectiveType directiveType)
        {
            AllInstructions.Add(mnemonic.ToLower(), new Instruction
            {
                Mnemonic = mnemonic,
                OpCode = opCode,
                Format = format,
                DirectiveType = directiveType
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
                    var mnemonic = instructionStr.TrimStart('+');

                    if (AllInstructions.ContainsKey(mnemonic))
                    {
                        var instruction = AllInstructions[mnemonic];
                        // In other to avoid someone modifying the original values for the instruction
                        // I'm returning a totally new object
                        return new Instruction
                        {
                            Mnemonic = instruction.Mnemonic,
                            OpCode = instruction.OpCode,
                            DirectiveType = instruction.DirectiveType,
                            Format = isFormat4 ? InstructionFormat.Four : instruction.Format
                        };
                    }

                    return null;
                }
            }
        }
    }
}