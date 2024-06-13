using Microsoft.Extensions.Options;
using RPFBE.Settings;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RPFBE.Service.ExtServs
{
    public class AESc : IAESc
    {
        //private readonly byte[] _key;
        private byte[] _IV;
        public AESc()
        {
            //    _key = Encoding.UTF8.GetBytes(Key);
            _IV = Aes.Create().IV;
        }
        public byte[] EncryptStringToBytes(string plainText, string Key)
        {
            byte[]  _key = Encoding.UTF8.GetBytes(Key);
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (_key == null || _key.Length <= 0)
                throw new ArgumentNullException(nameof(_key));
            if (_IV == null || _IV.Length <= 0)
                throw new ArgumentNullException(nameof(_IV));
            byte[] encrypted;

            // Create a Aes object with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public string DecryptStringFromBytes(byte[] cipherText, string Key)
        {
            byte[] _key = Encoding.UTF8.GetBytes(Key);
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (_key == null || _key.Length <= 0)
                throw new ArgumentNullException(nameof(_key));
            if (_IV == null || _IV.Length <= 0)
                throw new ArgumentNullException(nameof(_IV));

            // Declare the string used to hold the decrypted text.
            string plaintext = null;

            // Create a Aes object with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        //Custom AES implementation
    }
}
