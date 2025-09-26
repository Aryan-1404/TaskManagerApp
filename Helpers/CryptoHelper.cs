using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
    public static class CryptoHelper
    {
        private const string DefaultPassphrase = "ChangeThis_Default_Passphrase_2025!";
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("s@1tValue1234567");

        private static (byte[] Key, byte[] IV) DeriveKeyAndIV(string passphrase)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(passphrase, Salt, 10000, HashAlgorithmName.SHA256);
            var key = deriveBytes.GetBytes(32);
            var iv = deriveBytes.GetBytes(16);
            return (key, iv);
        }

        public static string? Encrypt(string plainText, string? passphrase = null)
        {
            try
            {
                passphrase ??= Environment.GetEnvironmentVariable("TASKMGR_SECRET") ?? DefaultPassphrase;
                var (key, iv) = DeriveKeyAndIV(passphrase);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using var ms = new MemoryStream();
                using (var crypto = aes.CreateEncryptor())
                using (var cs = new CryptoStream(ms, crypto, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs, Encoding.UTF8))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Encryption failed. Invalid key/IV or algorithm.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during encryption: {ex.Message}");
                return null;
            }
        }

        public static string? Decrypt(string cipherText, string? passphrase = null)
        {
            try
            {
                passphrase ??= Environment.GetEnvironmentVariable("TASKMGR_SECRET") ?? DefaultPassphrase;
                var (key, iv) = DeriveKeyAndIV(passphrase);

                var buffer = Convert.FromBase64String(cipherText);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using var ms = new MemoryStream(buffer);
                using var crypto = aes.CreateDecryptor();
                using var cs = new CryptoStream(ms, crypto, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid cipher text format.");
                return null;
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Decryption failed. Data may be corrupted or wrong key.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during decryption: {ex.Message}");
                return null;
            }
        }
    }
}
