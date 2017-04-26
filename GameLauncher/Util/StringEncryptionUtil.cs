using System;
using System.Security.Cryptography;
using System.Text;

namespace GameLauncher.Util
{
    static class StringEncryptionUtil
    {
        private static readonly byte[] Key = { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static readonly byte[] Iv = { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static string Encrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(Key, Iv);

            byte[] inputBuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

            return Convert.ToBase64String(outputBuffer);
        }

        public static string Decrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(Key, Iv);

            byte[] inputBuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

            return Encoding.Unicode.GetString(outputBuffer);
        }
    }
}