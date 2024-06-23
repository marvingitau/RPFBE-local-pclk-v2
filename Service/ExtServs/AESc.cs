using Microsoft.Extensions.Configuration;
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
        public IConfiguration Configuration { get; }

        //private readonly byte[] _key;
        //private byte[] _IV;
        public AESc(IConfiguration configuration)
        {
            Configuration = configuration;
            //    _key = Encoding.UTF8.GetBytes(Key);
            //_IV = Aes.Create().IV;
        }
        public string EncryptStringToBytes(string plainText)
        {
            byte[] _key = Encoding.UTF8.GetBytes(Configuration["JWT:KEY"]);
            byte[] _iv = Encoding.UTF8.GetBytes(Configuration["JWT:IV"]);
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (_key == null || _key.Length <= 0)
                throw new ArgumentNullException(nameof(_key));
            if (_iv == null || _iv.Length <= 0)
                throw new ArgumentNullException(nameof(_iv));
            string encrypted;

            // Create a Aes object with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

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
                        encrypted = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public string DecryptStringFromBytes(string cipherText)
        {
            byte[] _key = Encoding.UTF8.GetBytes(Configuration["JWT:KEY"]);
            byte[] _iv = Encoding.UTF8.GetBytes(Configuration["JWT:IV"]);
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (_key == null || _key.Length <= 0)
                throw new ArgumentNullException(nameof(_key));
            if (_iv == null || _iv.Length <= 0)
                throw new ArgumentNullException(nameof(_iv));

            // Declare the string used to hold the decrypted text.
            string plaintext = null;

            // Create a Aes object with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
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
