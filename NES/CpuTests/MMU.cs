using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuTests
{
    /*
    Memory map
    0000	3FFF	16 KiB ROM bank 00	From cartridge, usually a fixed bank
    4000    7FFF    16 KiB ROM Bank 01~NN   From cartridge, switchable bank via mapper(if any)
    8000    9FFF    8 KiB Video RAM(VRAM)  In CGB mode, switchable bank 0 / 1
    A000    BFFF    8 KiB External RAM  From cartridge, switchable bank if any
    C000    CFFF    4 KiB Work RAM(WRAM)
    D000    DFFF    4 KiB Work RAM(WRAM)   In CGB mode, switchable bank 1~7
    E000    FDFF    Mirror of C000~DDFF(ECHO RAM)  Nintendo says use of this area is prohibited.
    FE00 FE9F    Sprite attribute table(OAM)
    FEA0    FEFF    Not Usable  Nintendo says use of this area is prohibited
    FF00    FF7F    I / O Registers
    FF80    FFFE    High RAM(HRAM)
    FFFF    FFFF    Interrupt Enable register(IE)

    IO Ranges
    $FF00	    	DMG	Joypad input
    $FF01	$FF02	DMG	Serial transfer
    $FF04	$FF07	DMG	Timer and divider
    $FF10	$FF26	DMG	Audio
    $FF30	$FF3F	DMG	Wave pattern
    $FF40	$FF4B	DMG	LCD Control, Status, Position, Scrolling, and Palettes
    $FF4F	    	CGB	VRAM Bank Select
    $FF50	    	DMG	Set to non-zero to disable boot ROM
    $FF51	$FF55	CGB	VRAM DMA
    $FF68	$FF69	CGB	BG / OBJ Palettes
    $FF70	    	CGB	WRAM Bank Select
    */



    public class MMU
    {
        private byte[] memory;
        public MMU(int size)
        {
            memory = new byte[size];
        }
        public byte ReadByte(ushort address)
        {
            return memory[address];
        }

        public void WriteByte(ushort address, byte value)
        {
            memory[address] = value;
        }

        public ushort ReadWord(ushort address)
        {
            byte lowByte = ReadByte(address);
            byte highByte = ReadByte((ushort)(address + 1));
            return (ushort)(lowByte | (highByte << 8));
        }

        public void WriteWord(ushort address, ushort value)
        {
            // NES uses little-endian byte order
            byte lowByte = (byte)(value & 0xFF);
            byte highByte = (byte)(value >> 8);
            WriteByte(address, lowByte);
            WriteByte((ushort)(address + 1), highByte);
        }
    }
}
