using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB
{
    internal class Rom
    {
        private byte[] romData;
        private ushort EntryPoint;
        private string title;
        public Rom(string romname)
        {
            romData = File.ReadAllBytes(romname);
            EntryPoint = 0x100;
        }
        public byte ReadByte(ushort address)
        {
            return romData[address];
        }

        public void WriteByte(ushort address, byte value)
        {
            romData[address] = value;
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
        public void DecodeHeaders()
        {
            title = GetRomTitle();
        }
    }
}
