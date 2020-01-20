using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MlsExclusive.Utilites
{
    public static class Crypt
    {
        public static byte[] Encrypt(byte[] data, string password)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            ICryptoTransform ct = sa.CreateEncryptor(
                (new PasswordDeriveBytes(password, null)).GetBytes(16),
                new byte[16]);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        public static string EncryptString(string data, string password)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data), password));
        }

        static public byte[] Decrypt(byte[] data, string password)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            ICryptoTransform ct = sa.CreateDecryptor(
                (new PasswordDeriveBytes(password, null)).GetBytes(16),
                new byte[16]);

            MemoryStream ms = new MemoryStream(data);
            CryptoStream st = new CryptoStream(ms, ct, CryptoStreamMode.Read);
            st.Read(data, 0, data.Length);
            return ms.ToArray();
        }

        static public string DecryptString(string data, string password)
        {
            return Convert.ToBase64String(Decrypt(Encoding.UTF8.GetBytes(data), password));
        }
    }
}
