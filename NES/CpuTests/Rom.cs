using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuTests
{
    internal class Rom
    {
        private byte[] romData;
        private ushort entryPoint;
        private string title;
        private string manufacturer;
        private byte version;
        public Rom(string romname)
        {
            romData = File.ReadAllBytes(romname);
            entryPoint = 0x100;
            DecodeHeaders();
        }
        public byte ReadByte(ushort address)
        {
            return romData[address];
        }

        public void WriteByte(ushort address, byte value)
        {
            romData[address] = value;
        }
        
        public void DecodeHeaders()
        {
            title = GetRomTitle();
            manufacturer = GetRomManufacturer();
            version = ReadByte(0x14c);
        }
        private string GetRomTitle()
        {
            const int titleStartAddress = 0x0134;
            const int titleEndAddress = 0x0143;
            int titleLength = titleEndAddress - titleStartAddress + 1;

            byte[] titleBytes = new byte[titleLength];
            Array.Copy(romData, titleStartAddress, titleBytes, 0, titleLength);

            return Encoding.ASCII.GetString(titleBytes);
        }
        private string GetRomManufacturer()
        {
            const int manufacturerStartAddress = 0x013F;
            const int manufacturerEndAddress = 0x0142;
            int titleLength = manufacturerEndAddress - manufacturerStartAddress + 1;

            byte[] titleBytes = new byte[titleLength];
            Array.Copy(romData, manufacturerStartAddress, titleBytes, 0, titleLength);

            return Encoding.ASCII.GetString(titleBytes);
        }

    }
}
