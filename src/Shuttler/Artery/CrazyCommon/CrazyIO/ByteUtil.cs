
using System;
using System.Text;

namespace Shuttler.Artery
{
    public class ByteUtil
    {
        private const string HEX_INDEX = "0123456789abcdef          ABCDEF";
        private const string HEX_CHARS = "0123456789ABCDEF";

        public static string GetString(byte[] bytes)
        {
            return GetString(bytes, 0, bytes.Length);
        }

        public static string GetString(byte[] bytes, int offset, int length)
        {
            string s = string.Empty;

            for (int i = offset; i < length && i < bytes.Length; i++)
                s += (char)bytes[i];

            return s;
        }

        public static string PrintByte(byte b)
        {
#if(MF)
			return b.ToString();
#else
            return b.ToString("X2");
#endif
        }

        public static string BytesToHex(byte[] b)
        {
            string res = string.Empty;

            for (int i = 0; i < b.Length; i++)
                res += ByteToHex(b[i]);

            return res;
        }

        public static string ByteToHex(byte b)
        {
            int lowByte = b & 0x0F;
            int highByte = (b & 0xF0) >> 4;

            return new string(
                new char[] { HEX_CHARS[highByte], HEX_CHARS[lowByte] }
            );
        }

        public static byte[] HexToByte(string s)
        {
            int l = s.Length / 2;
            byte[] data = new byte[l];
            int j = 0;

            for (int i = 0; i < l; i++)
            {
                char c = s[j++];
                int n, b;

                n = HEX_INDEX.IndexOf(c);
                b = (n & 0xf) << 4;
                c = s[j++];
                n = HEX_INDEX.IndexOf(c);
                b += (n & 0xf);
                data[i] = (byte)b;
            }

            return data;
        }

        public static string PrintBytes(byte[] bytes)
        {
            return PrintBytes(bytes, bytes.Length);
        }

        public static string PrintBytes(byte[] bytes, bool wrapLines)
        {
            return PrintBytes(bytes, bytes.Length, wrapLines);
        }

        public static string PrintBytes(byte[] bytes, int length)
        {
            return PrintBytes(bytes, length, true);
        }

        public static string PrintBytes(byte[] bytes, int length, bool wrapLines)
        {
            string s = string.Empty;

            int c = 0;

            for (int i = 0; i < length && i < bytes.Length; i++)
            {
                s += PrintByte(bytes[i]);

                if (++c == 24 && wrapLines)
                {
                    s += "\r\n";
                    c = 0;
                }
                else
                    if (i < length - 1)
                        s += "-";
            }

            return s;
        }
    }
}