namespace SIC.Assembler.Model
{
    public enum AssemblerDirectiveType
    {
        Start = 1,
        End = 2,
        Byte = 3,
        Word = 4,
        Resb = 5,
        Resw = 6,
        Base = 7,
        Equ = 8,
        Extdef = 9,
        Extref = 10,
        Unknown = 0
    }
}