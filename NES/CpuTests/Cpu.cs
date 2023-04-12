using System;

namespace CpuTests
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
        private ushort AF { get; set; }
        private ushort BC { get; set; }
        public ushort DE { get; set; }
        public ushort HL { get; set; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }


        private byte A {
            get { return (byte)(AF >> 8); }
            set { AF = (ushort)((AF & 0x00FF) | (value << 8)); }
        }
        private CpuFlags F
        {
            get { return (CpuFlags)(AF & 0xFF); }
            set { AF = (ushort)((AF & 0xFF00) | (byte)value); }
        }
        public void SetFlag(CpuFlags flag, bool value)
        {
            if (value)
            {
                F |= flag;
            }
            else
            {
                F &= ~flag;
            }
        }

        private byte B
        {
            get { return (byte)(BC >> 8); }
            set { BC = (ushort)((BC & 0x00FF) | (value << 8)); }
        }
        private byte C
        {
            get { return (byte)(BC & 0xFF); }
            set { AF = (ushort)((BC & 0xFF00) | (byte)value); }
        }
        public byte D
        {
            get { return (byte)(DE >> 8); }
            set { DE = (ushort)((DE & 0x00FF) | (value << 8)); }
        }

        public byte E
        {
            get { return (byte)(DE & 0xFF); }
            set { DE = (ushort)((DE & 0xFF00) | value); }
        }

        public byte H
        {
            get { return (byte)(HL >> 8); }
            set { HL = (ushort)((HL & 0x00FF) | (value << 8)); }
        }

        public byte L
        {
            get { return (byte)(HL & 0xFF); }
            set { HL = (ushort)((HL & 0xFF00) | value); }
        }
        #endregion
        private int cycles;
        private MMU mmu;
        public Cpu(MMU mmu)
        {
            AF = 0;
            BC = 0;
            DE = 0;
            HL = 0;
            cycles = 0;
            this.mmu = mmu;
            PC = 0x100;
        }
        public void ExecuteInstruction()
        {
            byte opcode = mmu.ReadByte(PC++);
            switch (opcode)
            {
                case 0x00: //NOP            1  4    - - - -
                    break;
                case 0x01: //LD BC,D16      3  12   - - - -
                    BC = mmu.ReadWord(PC);
                    PC+=2;
                    break;
                case 0x02: //LD (BC),A      1  8    - - - -
                    mmu.WriteByte(BC, A);
                    break;
                case 0x03: //INC BC         1  8    - - - -
                    BC++;
                    break;
                case 0x04: //INC B          1  4    Z 0 H -
                    B = INC(B);
                    break;
                case 0x05: //DEC B          1  4    Z 1 H -
                    B = DEC(B);
                    break;
                case 0x06: //LD B,d8        2  8    - - - -
                    B = mmu.ReadByte(PC++);
                    break;
                case 0x07: //RLCA           1  4    0 0 0 C
                    throw new NotImplementedException();
                    break;
                case 0x08: //LD (a16),SP    3  20   - - - -
                    mmu.WriteWord(mmu.ReadWord(PC), SP);
                    PC += 2;
                    break;
                case 0x09: //ADD HL,BC      1  8    - 0 H C
                    ADD_HL(BC);
                    break;
                case 0x0a: //LD A,(BC)      1  8    - - - -
                    A = mmu.ReadByte(BC);
                    break;
                case 0x0b: //DEC BC         1  8    - - - -
                    BC--;
                    break;
                case 0x0c: //INC C          1  4    Z 0 H -
                    C = INC(C);
                    break;
                case 0x0d: //DEC C          1  4    Z 1 H -
                    C = DEC(C);
                    break;
                case 0x0e: //LD C, d8       2  8    - - - -
                    C = mmu.ReadByte(PC++);
                    break;
                case 0x0f: //RRCA           1  4    0 0 0 C
                    throw new NotImplementedException();
                    break;

                case 0x10: // STOP 0        2  4    - - - -
                    throw new NotImplementedException();
                    break;
                case 0x11: //LD DE,d16      3  12   - - - -
                    DE = mmu.ReadWord(PC);
                    PC += 2;
                    break;
                case 0x12: //LD (DE),A      1  8    - - - -
                    mmu.WriteByte(DE, A);
                    break;
                case 0x13: //INC DE         1  8    - - - -
                    DE++;
                    break;
                case 0x14: //INC D          1  4    Z 0 H -
                    D = INC(D);
                    break;
                case 0x15: //DEC D          1  4    Z 1 H -
                    D = DEC(D);
                    break;
                case 0x16: //LD D,d8        2  8    - - - -
                    D = mmu.ReadByte(PC++);
                    break;
                case 0x17: //RLA            1  4    0 0 0 C
                    throw new NotImplementedException();
                    break;
                case 0x18: //JR r8          2  12   - - - -
                    throw new NotImplementedException();
                    break;
                case 0x19: //ADD HL,DE      1  8    - 0 H C
                    ADD_HL(DE);
                    break;
                case 0x1a: //LD A,(DE)      1  8    - - - -
                    A = mmu.ReadByte(DE);
                    break;
                case 0x1b: //DEC DE         1  8    - - - -
                    DE--;
                    break;
                case 0x1c: //INC E          1  4    Z 0 H -
                    E = INC(E);
                    break;
                case 0x1d: //DEC E          1  4    Z 1 H -
                    E = DEC(E);
                    break;
                case 0x1e: //LD E, d8       2  8    - - - -
                    E = mmu.ReadByte(PC++);
                    break;
                case 0x1f: //RRA            1  4    0 0 0 C
                    throw new NotImplementedException();
                    break;

                case 0x21: //LD HL,d16      3  12   - - - -
                    HL = mmu.ReadWord(PC);
                    PC += 2;
                    break;
                case 0x22: //LD (HL+),A      1  8    - - - -
                    mmu.WriteByte(HL++, A);
                    break;
                case 0x23: //INC HL         1  8    - - - -
                    HL++;
                    break;
                case 0x24: //INC H          1  4    Z 0 H -
                    H = INC(H);
                    break;
                case 0x25: //DEC H          1  4    Z 1 H -
                    H = DEC(H);
                    break;
                case 0x26: //LD H,d8        2  8    - - - -
                    H = mmu.ReadByte(PC++);
                    break;
                case 0x27: //DAA            1  4    Z - 0 C
                    throw new NotImplementedException();
                    break;
                case 0x28: //JR Z,r8        2  12/8 - - - -
                    throw new NotImplementedException();
                    break;
                case 0x29: //ADD HL,HL      1  8    - 0 H C
                    ADD_HL(HL);
                    break;
                case 0x2a: //LD A,(HL+)     1  8    - - - -
                    A = mmu.ReadByte(HL++);
                    break;
                case 0x2b: //DEC HL         1  8    - - - -
                    HL--;
                    break;
                case 0x2c: //INC L          1  4    Z 0 H -
                    L = INC(L);
                    break;
                case 0x2d: //DEC L          1  4    Z 1 H -
                    L = DEC(L);
                    break;
                case 0x2e: //LD L, d8       2  8    - - - -
                    L = mmu.ReadByte(PC++);
                    break;
                case 0x2f: //CPL            1  4    - 1 1 -
                    throw new NotImplementedException();
                    break;

                case 0x31: //LD SP,d16      3  12   - - - -
                    SP = mmu.ReadWord(PC);
                    PC += 2;
                    break;
                case 0x32: //LD (HL-),A      1  8    - - - -
                    mmu.WriteByte(HL--, A);
                    break;
                case 0x33: //INC SP         1  8    - - - -
                    SP++;
                    break;
                case 0x34: //INC (HL)       1  12   Z 0 H -
                    mmu.WriteByte(HL, INC(mmu.ReadByte(HL)));
                    break;
                case 0x35: //DEC (HL)       1  12   Z 1 H -
                    mmu.WriteByte(HL, DEC(mmu.ReadByte(HL)));
                    break;
                case 0x36: //LD (HL),d8     2  12   - - - -
                    mmu.WriteByte(HL, mmu.ReadByte(PC++));
                    break;
                case 0x37: //SCF            1  4    - 0 0 1
                    throw new NotImplementedException();
                    break;
                case 0x38: //JR C,r8        2  12/8 - - - -
                    throw new NotImplementedException();
                    break;
                case 0x39: //ADD HL,SP      1  8    - 0 H C
                    ADD_HL(SP);
                    break;
                case 0x3a: //LD A,(HL-)     1  8    - - - -
                    A = mmu.ReadByte(HL--);
                    break;
                case 0x3b: //DEC SP         1  8    - - - -
                    SP--;
                    break;
                case 0x3c: //INC A          1  4    Z 0 H -
                    A = INC(A);
                    break;
                case 0x3d: //DEC A          1  4    Z 1 H -
                    A = DEC(A);
                    break;
                case 0x3e: //LD A, d8       2  8    - - - -
                    A = mmu.ReadByte(PC++);
                    break;
                case 0x3f: //CCF            1  4    - 0 0 C
                    throw new NotImplementedException();
                    break;

                case 0x40: //LD B,B         1  4    - - - -
                    B = B;
                    break;
                case 0x41: //LD B,C         1  4    - - - -
                    B = C;
                    break;
                case 0x42: //LD B,D         1  4    - - - -
                    B = D;
                    break;
                case 0x43: //LD B,E 1  4 - - - -
                    B = E;
                    break;
                case 0x44: //LD B,H 1  4 - - - -
                    B = H;
                    break;
                case 0x45: //LD B,L 1  4 - - - -
                    B = L;
                    break;
                case 0x46: //LD B,(HL) 1  8 - - - -
                    B = mmu.ReadByte(HL);
                    break;
                case 0x47: //LD B,A 1  4 - - - -
                    B = A;
                    break;
                case 0x48: //LD C,B 1  4 - - - -
                    C = B;
                    break;
                case 0x49: //LD C,C 1  4 - - - -
                    C = C;
                    break;
                case 0x4a: //LD C,D 1  4 - - - -
                    C = D;
                    break;
                case 0x4b: //LD C,E 1  4 - - - -
                    C = E;
                    break;
                case 0x4c: //LD C,H 1  4 - - - -
                    C = H;
                    break;
                case 0x4d: //LD C,L 1  4 - - - -
                    C = L;
                    break;
                case 0x4e: //LD C,(HL) 1  8 - - - -
                    C = mmu.ReadByte(HL);
                    break;
                case 0x4f: //LD C,A 1  4 - - - -
                    C = A;
                    break;

                case 0x50: //LD D,B 1  4 - - - -
                    D = B;
                    break;
                case 0x51: //LD D,C 1  4 - - - -
                    D = C;
                    break;
                case 0x52: //LD D,D 1  4 - - - -
                    D = D;
                    break;
                case 0x53: //LD D,E 1  4 - - - -
                    D = E;
                    break;
                case 0x54: //LD D,H 1  4 - - - -
                    D = H;
                    break;
                case 0x55: //LD D,L 1  4 - - - -
                    D = L;
                    break;
                case 0x56: //LD D,(HL) 1  8 - - - -
                    D = mmu.ReadByte(HL);
                    break;
                case 0x57: //LD D,A 1  4 - - - -
                    D = A;
                    break;
                case 0x58: //LD E,B 1  4 - - - -
                    E = B;
                    break;
                case 0x59: //LD E,C 1  4 - - - -
                    E = C;
                    break;
                case 0x5a: //LD E,D 1  4 - - - -
                    E = D;
                    break;
                case 0x5b: //LD E,E 1  4 - - - -
                    E = E;
                    break;
                case 0x5c: //LD E,H 1  4 - - - -
                    E = H;
                    break;
                case 0x5d: //LD E,L 1  4 - - - -
                    E = L;
                    break;
                case 0x5e: //LD E,(HL) 1  8 - - - -
                    E = mmu.ReadByte(HL);
                    break;
                case 0x5f: //LD E,A 1  4 - - - -
                    E = A;
                    break;

                case 0x60: //LD H,B 1  4 - - - -
                    H = B;
                    break;
                case 0x61: //LD H,C 1  4 - - - -
                    H = C;
                    break;
                case 0x62: //LD H,D 1  4 - - - -
                    H = D;
                    break;
                case 0x63: //LD H,E 1  4 - - - -
                    H = E;
                    break;
                case 0x64: //LD H,H 1  4 - - - -
                    H = H;
                    break;
                case 0x65: //LD H,L 1  4 - - - -
                    H = L;
                    break;
                case 0x66: //LD H,(HL) 1  8 - - - -
                    H = mmu.ReadByte(HL);
                    break;
                case 0x67: //LD H,A 1  4 - - - -
                    H = A;
                    break;
                case 0x68: //LD L,B 1  4 - - - -
                    L = B;
                    break;
                case 0x69: //LD L,C 1  4 - - - -
                    L = C;
                    break;
                case 0x6a: //LD L,D 1  4 - - - -
                    L = D;
                    break;
                case 0x6b: //LD L,E 1  4 - - - -
                    L = E;
                    break;
                case 0x6c: //LD L,H 1  4 - - - -
                    L = H;
                    break;
                case 0x6d: //LD L,L 1  4 - - - -
                    L = L;
                    break;
                case 0x6e: //LD L,(HL) 1  8 - - - -
                    L = mmu.ReadByte(HL);
                    break;
                case 0x6f: //LD L,A 1  4 - - - -
                    L = A;
                    break;

                case 0x70: //LD (HL),B 1  8 - - - -
                    mmu.WriteByte(HL, B);
                    break;
                case 0x71: //LD (HL),C 1  8 - - - -
                    mmu.WriteByte(HL, C);
                    break;
                case 0x72: //LD (HL),D 1  8 - - - -
                    mmu.WriteByte(HL, D);
                    break;
                case 0x73: //LD (HL),E 1  8 - - - -
                    mmu.WriteByte(HL, E);
                    break;
                case 0x74: //LD (HL),H 1  8 - - - -
                    mmu.WriteByte(HL, H);
                    break;
                case 0x75: //LD (HL),L 1  8 - - - -
                    mmu.WriteByte(HL, L);
                    break;
                case 0x76: //HALT 1  4 - - - -
                    throw new NotImplementedException();
                    break;
                case 0x77: //LD (HL),A 1  8 - - - -
                    mmu.WriteByte(HL, A);
                    break;
                case 0x78: //LD A,B 1  4 - - - -
                    A = B;
                    break;
                case 0x79: //LD A,C 1  4 - - - -
                    A = C;
                    break;
                case 0x7a: //LD A,D 1  4 - - - -
                    A = D;
                    break;
                case 0x7b: //LD A,E 1  4 - - - -
                    A = E;
                    break;
                case 0x7c: //LD A,H 1  4 - - - -
                    A = H;
                    break;
                case 0x7d: //LD A,L 1  4 - - - -
                    A = L;
                    break;
                case 0x7e: //LD A,(HL) 1  8 - - - -
                    A = mmu.ReadByte(HL);
                    break;
                case 0x7f: //LD A,A 1  4 - - - -
                    A = A;
                    break;

                case 0x80: //ADD A,B 1  4 Z 0 H C
                    ADD(B);
                    break;
                case 0x81: //ADD A,C 1  4 Z 0 H C
                    ADD(C);
                    break;
                case 0x82: //ADD A,D 1  4 Z 0 H C
                    ADD(D);
                    break;
                case 0x83: //ADD A,E 1  4 Z 0 H C
                    ADD(E);
                    break;
                case 0x84: //ADD A,H 1  4 Z 0 H C
                    ADD(H);
                    break;
                case 0x85: //ADD A,L 1  4 Z 0 H C
                    ADD(L);
                    break;
                case 0x86: //ADD A,(HL) 1  8 Z 0 H C
                    ADD(mmu.ReadByte(HL));
                    break;
                case 0x87: //ADD A,A 1  4 Z 0 H C
                    ADD(A);
                    break;
                case 0x88: //ADC A,B 1  4 Z 0 H C
                    ADC(B);
                    break;
                case 0x89: //ADC A,C 1  4 Z 0 H C
                    ADC(C);
                    break;
                case 0x8a: //ADC A,D 1  4 Z 0 H C
                    ADC(D);
                    break;
                case 0x8b: //ADC A,E 1  4 Z 0 H C
                    ADC(E);
                    break;
                case 0x8c: //ADC A,H 1  4 Z 0 H C
                    ADC(H);
                    break;
                case 0x8d: //ADC A,L 1  4 Z 0 H C
                    ADC(L);
                    break;
                case 0x8e: //ADC A,(HL) 1  8 Z 0 H C
                    ADC(mmu.ReadByte(HL));
                    break;
                case 0x8f: //ADC A,A 1  4 Z 0 H C
                    ADC(A);
                    break;

                case 0x90: //SUB B 1  4 Z 1 H C
                    SUB(B);
                    break;
                case 0x91: //SUB C 1  4 Z 1 H C
                    SUB(C);
                    break;
                case 0x92: //SUB D 1  4 Z 1 H C
                    SUB(D);
                    break;
                case 0x93: //SUB E 1  4 Z 1 H C
                    SUB(E);
                    break;
                case 0x94: //SUB H 1  4 Z 1 H C
                    SUB(H);
                    break;
                case 0x95: //SUB L 1  4 Z 1 H C
                    SUB(L);
                    break;
                case 0x96: //SUB (HL) 1  8 Z 1 H C
                    SUB(mmu.ReadByte(HL));
                    break;
                case 0x97: //SUB A 1  4 Z 1 H C
                    SUB(A);
                    break;
                case 0x98: //SBC A,B 1  4 Z 1 H C
                    SBC(B);
                    break;
                case 0x99: //SBC A,C 1  4 Z 1 H C
                    SBC(C);
                    break;
                case 0x9a: //SBC A,D 1  4 Z 1 H C
                    SBC(D);
                    break;
                case 0x9b: //SBC A,E 1  4 Z 1 H C
                    SBC(E);
                    break;
                case 0x9c: //SBC A,H 1  4 Z 1 H C
                    SBC(H);
                    break;
                case 0x9d: //SBC A,L 1  4 Z 1 H C
                    SBC(L);
                    break;
                case 0x9e: //SBC A,(HL) 1  8 Z 1 H C
                    SBC(mmu.ReadByte(HL));
                    break;
                case 0x9f: //SBC A,A 1  4 Z 1 H C
                    SBC(A);
                    break;

                case 0xa0: //AND B 1  4 Z 0 1 0
                    AND(B);
                    break;
                case 0xa1: //AND C 1  4 Z 0 1 0
                    AND(C);
                    break;
                case 0xa2: //AND D 1  4 Z 0 1 0
                    AND(D);
                    break;
                case 0xa3: //AND E 1  4 Z 0 1 0
                    AND(E);
                    break;
                case 0xa4: //AND H 1  4 Z 0 1 0
                    AND(H);
                    break;
                case 0xa5: //AND L 1  4 Z 0 1 0
                    AND(L);
                    break;
                case 0xa6: //AND (HL) 1  8 Z 0 1 0
                    AND(mmu.ReadByte(HL));
                    break;
                case 0xa7: //AND A 1  4 Z 0 1 0
                    AND(A);
                    break;
                case 0xa8: //XOR B 1  4 Z 0 0 0
                    XOR(B);
                    break;
                case 0xa9: //XOR C 1  4 Z 0 0 0
                    XOR(C);
                    break;
                case 0xaa: //XOR D 1  4 Z 0 0 0
                    XOR(D);
                    break;
                case 0xab: //XOR E 1  4 Z 0 0 0
                    XOR(E);
                    break;
                case 0xac: //XOR H 1  4 Z 0 0 0
                    XOR(H);
                    break;
                case 0xad: //XOR L 1  4 Z 0 0 0
                    XOR(L);
                    break;
                case 0xae: //XOR (HL) 1  8 Z 0 0 0
                    XOR(mmu.ReadByte(HL));
                    break;
                case 0xaf: //XOR A 1  4 Z 0 0 0
                    XOR(A);
                    break;

                case 0xb0: //OR B 1  4 Z 0 0 0
                    OR(B);
                    break;
                case 0xb1: //OR C 1  4 Z 0 0 0
                    OR(C);
                    break;
                case 0xb2: //OR D 1  4 Z 0 0 0
                    OR(D);
                    break;
                case 0xb3: //OR E 1  4 Z 0 0 0
                    OR(E);
                    break;
                case 0xb4: //OR H 1  4 Z 0 0 0
                    OR(H);
                    break;
                case 0xb5: //OR L 1  4 Z 0 0 0
                    OR(L);
                    break;
                case 0xb6: //OR (HL) 1  8 Z 0 0 0
                    OR(mmu.ReadByte(HL));
                    break;
                case 0xb7: //OR A 1  4 Z 0 0 0
                    OR(A);
                    break;
                case 0xb8: //CP B 1  4 Z 1 H C
                    CP(B);
                    break;
                case 0xb9: //CP C 1  4 Z 1 H C
                    CP(C);
                    break;
                case 0xba: //CP D 1  4 Z 1 H C
                    CP(D);
                    break;
                case 0xbb: //CP E 1  4 Z 1 H C
                    CP(E);
                    break;
                case 0xbc: //CP H 1  4 Z 1 H C
                    CP(H);
                    break;
                case 0xbd: //CP L 1  4 Z 1 H C
                    CP(L);
                    break;
                case 0xbe: //CP (HL) 1  8 Z 1 H C
                    CP(mmu.ReadByte(HL));
                    break;
                case 0xbf: //CP A 1  4 Z 1 H C
                    CP(A);
                    break;

                case 0xc0: //RET NZ 1  20/8 - - - -
                    break;
                case 0xc1: //POP BC 1  12 - - - -
                    break;
                case 0xc2: //JP NZ,a16 3  16/12 - - - -
                    break;
                case 0xc3: //JP a16 3  16 - - - -
                    break;
                case 0xc4: //CALL NZ,a16 3  24/12 - - - -
                    break;
                case 0xc5: //PUSH BC 1  16 - - - -
                    break;
                case 0xc6: //ADD A,d8 2  8 Z 0 H C
                    break;
                case 0xc7: //RST 00H 1  16 - - - -
                    break;
                case 0xc8: //RET Z 1  20/8 - - - -
                    break;
                case 0xc9: //RET 1  16 - - - -
                    break;
                case 0xca: //JP Z,a16 3  16/12 - - - -
                    break;
                case 0xcb: //PREFIX CB 1  4 - - - -
                    break;
                case 0xcc: //CALL Z,a16 3  24/12 - - - -
                    break;
                case 0xcd: //CALL a16 3  24 - - - -
                    break;
                case 0xce: //ADC A,d8 2  8 Z 0 H C
                    break;
                case 0xcf: //RST 08H 1  16 - - - -
                    break;

                case 0xd0: //RET NC 1  20/8 - - - -
                    break;
                case 0xd1: //POP DE 1  12 - - - -
                    break;
                case 0xd2: //JP NC,a16 3  16/12 - - - -
                    break;
                case 0xd3: //
                    break;
                case 0xd4: //CALL NC,a16 3  24/12 - - - -
                    break;
                case 0xd5: //PUSH DE 1  16 - - - -
                    break;
                case 0xd6: //SUB d8 2  8 Z 1 H C
                    break;
                case 0xd7: //RST 10H 1  16 - - - -
                    break;
                case 0xd8: //RET C 1  20/8 - - - -
                    break;
                case 0xd9: //RETI 1  16 - - - -
                    break;
                case 0xda: //JP C,a16 3  16/12 - - - -
                    break;
                case 0xdb: //
                    break;
                case 0xdc: //CALL C,a16 3  24/12 - - - -
                    break;
                case 0xdd: //
                    break;
                case 0xde: //SBC A,d8 2  8 Z 1 H C
                    break;
                case 0xdf: //RST 18H 1  16 - - - -
                    break;

                case 0xe0: //LDH (a8),A 2  12 - - - -
                    break;
                case 0xe1: //POP HL 1  12 - - - -
                    break;
                case 0xe2: //LD (C),A 2  8 - - - -
                    break;
                case 0xe3: //
                    break;
                case 0xe4: //
                    break;
                case 0xe5: //PUSH HL 1  16 - - - -
                    break;
                case 0xe6: //AND d8 2  8 Z 0 1 0
                    break;
                case 0xe7: //RST 20H 1  16 - - - -
                    break;
                case 0xe8: //ADD SP,r8 2  16 0 0 H C
                    break;
                case 0xe9: //JP (HL) 1  4 - - - -
                    break;
                case 0xea: //LD (a16),A 3  16 - - - -
                    break;
                case 0xeb: //
                    break;
                case 0xec: //
                    break;
                case 0xed: //
                    break;
                case 0xee: //XOR d8 2  8 Z 0 0 0
                    break;
                case 0xef: //RST 28H 1  16 - - - -
                    break;

                case 0xf0: //LDH A,(a8) 2  12 - - - -
                    break;
                case 0xf1: //POP AF 1  12 Z N H C
                    break;
                case 0xf2: //LD A,(C) 2  8 - - - -
                    break;
                case 0xf3: //DI 1  4 - - - -
                    break;
                case 0xf4: //
                    break;
                case 0xf5: //PUSH AF 1  16 - - - -
                    break;
                case 0xf6: //OR d8 2  8 Z 0 0 0
                    break;
                case 0xf7: //RST 30H 1  16 - - - -
                    break;
                case 0xf8: //LD HL,SP+r8 2  12 0 0 H C
                    break;
                case 0xf9: //LD SP,HL 1  8 - - - -
                    break;
                case 0xfa: //LD A,(a16) 3  16 - - - -
                    break;
                case 0xfb: //EI 1  4 - - - -
                    break;
                case 0xfc: //
                    break;
                case 0xfd: //
                    break;
                case 0xfe: //CP d8 2  8 Z 1 H C
                    break;
                case 0xff: //RST 38H 1  16 - - - -
                    break;

                default:
                    throw new InvalidOperationException($"Invalid opcode: 0x{opcode:X2}");
            }
        }

        private void SetFlagZ(byte result)
        {
            SetFlag(CpuFlags.Zero, result == 0);
        }
        private void SetFlagZ(bool result)
        {
            SetFlag(CpuFlags.Zero, result);
        }
        private void SetFlagS(bool result)
        {
            SetFlag(CpuFlags.Subtract, result);
        }
        private void SetFlagH(byte a, byte b)
        {
            SetFlag(CpuFlags.HalfCarry, (a & 0x0F) + (b & 0x0F) > 0x0F);
        }
        private void SetFlagH(bool result)
        {
            SetFlag(CpuFlags.HalfCarry, result);
        }
        private void SetFlagH(ushort a, ushort b)
        {
            SetFlag(CpuFlags.HalfCarry, (a & 0x0FFF) + (b & 0x0FFF) > 0x0FFF);
        }
        private void SetFlagHSub(byte a, byte b)
        {
            SetFlag(CpuFlags.HalfCarry, (a & 0x0F) < (b & 0x0F));
        }
        private void SetFlagC(bool result)
        {
            SetFlag(CpuFlags.Carry, result);
        }
        private void SetFlagC(int result)
        {
            SetFlag(CpuFlags.Carry, (result >> 8) != 0);
        }
        private void SetFlagCShort(int result)
        {
            SetFlag(CpuFlags.Carry, (result >> 16) != 0);
        }
        #region Instructions
        private byte INC(byte operand) //Z 0 H -
        {
            byte result = operand++;
            SetFlagZ(result);
            SetFlagS(false);
            SetFlagH(operand, 1);
            return result;
        }
        private byte DEC(byte operand) //Z 1 H -
        {
            byte result = operand++;
            SetFlagZ(result);
            SetFlagS(true);
            SetFlagHSub(operand, 1);
            return result;
        }
        private void ADD_HL(ushort operand) //- 0 H C
        {
            int result = HL + operand;
            SetFlagS(false);
            SetFlagH(HL, operand);
            SetFlagCShort(result);

        }
        private void ADD(byte b) //Z 0 H C
        {
            int result = A + b;
            byte bresult = (byte)result;
            SetFlagZ(bresult);
            SetFlagS(false);
            SetFlagH(A, b);
            SetFlagC(result);
            A = bresult;
        }
        private void ADC(byte b) //Z 0 H C
        {
            throw new NotImplementedException();
        }
        private void SUB(byte b) //Z 1 H C
        {
            int result = A - b;
            byte bresult = (byte)result;
            SetFlagZ(bresult);
            SetFlagS(true);
            SetFlagHSub(A, b);
            SetFlagC(result);
            A = bresult;
        }
        private void SBC(byte b) //Z 1 H C
        { 
            throw new NotImplementedException();
        }
        private void AND(byte b) //Z 0 1 0 
        {
            byte result = (byte)(A & b);
            SetFlagZ(result);
            SetFlagS(false);
            SetFlagH(true);
            SetFlagC(false);
            A = result;
        }
        private void XOR(byte b) //Z 0 0 0
        {
            byte result = (byte)(A ^ b);
            SetFlagZ(result);
            SetFlagS(false);
            SetFlagH(false);
            SetFlagC(false);
            A = result;
        }
        private void OR(byte b) //Z 0 0 0
        {
            byte result = (byte)(A | b);
            SetFlagZ(result);
            SetFlagS(false);
            SetFlagH(false);
            SetFlagC(false);
            A = result;
        }
        private void CP(byte b) //Z 1 H C
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
