using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Utilities.Security.Cryptography.X509Certificates
{
    internal class RSAParameterTraits
    {
        public Int32 SizeMod { get; private set; }
        public Int32 SizeExp { get; private set; }
        public Int32 SizeD { get; private set; }
        public Int32 SizeP { get; private set; }
        public Int32 SizeQ { get; private set; }
        public Int32 SizeDP { get; private set; }
        public Int32 SizeDQ { get; private set; }
        public Int32 SizeInvQ { get; private set; }

        public RSAParameterTraits(Int32 modulusLengthInBits)
        {
            // The modulus length is supposed to be one of the common lengths, which is the commonly referred to strength of the key,
            // like 1024 bit, 2048 bit, etc.  It might be a few bits off though, since if the modulus has leading zeros it could show
            // up as 1016 bits or something like that.
            Int32 assumedLength = -1;
            Double logbase = Math.Log(modulusLengthInBits, 2);
            if (logbase == (Int32)logbase)
            {
                assumedLength = modulusLengthInBits;
            }
            else
            {
                assumedLength = (Int32)(logbase + 1.0);
                assumedLength = (Int32)(Math.Pow(2, assumedLength));
            }

            switch (assumedLength)
            {
                case 1024:
                    SizeMod = 0x80;
                    SizeExp = -1;
                    SizeD = 0x80;
                    SizeP = 0x40;
                    SizeQ = 0x40;
                    SizeDP = 0x40;
                    SizeDQ = 0x40;
                    SizeInvQ = 0x40;
                    break;
                case 2048:
                    SizeMod = 0x100;
                    SizeExp = -1;
                    SizeD = 0x100;
                    SizeP = 0x80;
                    SizeQ = 0x80;
                    SizeDP = 0x80;
                    SizeDQ = 0x80;
                    SizeInvQ = 0x80;
                    break;
                case 4096:
                    SizeMod = 0x200;
                    SizeExp = -1;
                    SizeD = 0x200;
                    SizeP = 0x100;
                    SizeQ = 0x100;
                    SizeDP = 0x100;
                    SizeDQ = 0x100;
                    SizeInvQ = 0x100;
                    break;
                default: throw new CryptographicException("Unknown key size");
            }
        }
    }
}
