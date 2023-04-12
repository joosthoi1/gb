using System;

namespace GB
{
    internal class Cpu
    {
        #region Registers
        [Flags]
        public enum CpuFlags : byte
        {
            None = 0,
            Carry = 1 << 4,
            HalfCarry = 1 << 5,
            Subtract = 1 << 6,
            Zero = 1 << 7
        }
        private ushort AFRegister { get; set; }
        private ushort BCRegister { get; set; }
        public ushort DERegister { get; set; }
        public ushort HLRegister { get; set; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }

        private byte A {
            get { return (byte)(AFRegister >> 8); }
            set { AFRegister = (ushort)((AFRegister & 0x00FF) | (value << 8)); }
        }
        private CpuFlags F
        {
            get { return (CpuFlags)(AFRegister & 0xFF); }
            set { AFRegister = (ushort)((AFRegister & 0xFF00) | (byte)value); }
        }

        private byte B
        {
            get { return (byte)(BCRegister >> 8); }
            set { BCRegister = (ushort)((BCRegister & 0x00FF) | (value << 8)); }
        }
        private byte C
        {
            get { return (byte)(BCRegister & 0xFF); }
            set { AFRegister = (ushort)((BCRegister & 0xFF00) | (byte)value); }
        }
        public byte D
        {
            get { return (byte)(DERegister >> 8); }
            set { DERegister = (ushort)((DERegister & 0x00FF) | (value << 8)); }
        }

        public byte E
        {
            get { return (byte)(DERegister & 0xFF); }
            set { DERegister = (ushort)((DERegister & 0xFF00) | value); }
        }

        public byte H
        {
            get { return (byte)(HLRegister >> 8); }
            set { HLRegister = (ushort)((HLRegister & 0x00FF) | (value << 8)); }
        }

        public byte L
        {
            get { return (byte)(HLRegister & 0xFF); }
            set { HLRegister = (ushort)((HLRegister & 0xFF00) | value); }
        }
        #endregion

        public void ExecuteInstruction(byte opcode)
        {
            switch (opcode)
            {
                // Handle your new instruction here
                //case MyOpcode:
                //    DoMyInstruction();
                //    break;

                // Handle other instructions here...

                default:
                    throw new InvalidOperationException($"Invalid opcode: 0x{opcode:X2}");
            }
        }

        #region Instructions
        #endregion
    }
}
