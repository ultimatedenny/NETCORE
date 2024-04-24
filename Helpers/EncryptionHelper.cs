using System.Security.Cryptography;
using System.Text;
using WebApi.Helpers;

namespace NETCORE.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly AppSettings _appSettings;

        static string GenerateStrongKey(params string[] values)
        {
            var concatenatedValues = string.Join("|", values);
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedValues));
            return Convert.ToBase64String(hashBytes);
        }
        private static readonly string FixedKey = "0123456789ABCDEF0123456789ABCDEF"; // 256-bit key

        public static string Encrypt(this string clearText, string siteKey)
        {
            try
            {
                using Aes aesAlg = Aes.Create();
                aesAlg.Key = Encoding.UTF8.GetBytes(siteKey);
                aesAlg.Mode = CipherMode.CBC;

                byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
                byte[] iv = aesAlg.IV;

                using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

                using MemoryStream msEncrypt = new();
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(clearBytes, 0, clearBytes.Length);
                }

                byte[] encryptedBytes = msEncrypt.ToArray();
                byte[] result = new byte[iv.Length + encryptedBytes.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

                return Convert.ToBase64String(result);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Decrypt(this string cipherText, string siteKey)
        {
            try
            {
                using Aes aesAlg = Aes.Create();
                aesAlg.Key = Encoding.UTF8.GetBytes(siteKey);
                aesAlg.Mode = CipherMode.CBC;

                byte[] encryptedBytes = Convert.FromBase64String(cipherText);
                byte[] iv = new byte[16];
                Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

                using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);

                using MemoryStream msDecrypt = new(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);

                using StreamReader srDecrypt = new(csDecrypt);
                return srDecrypt.ReadToEnd();
            }
            catch (Exception)
            {
                return "null";
            }
        }
    }
}
