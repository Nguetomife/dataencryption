// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataEncryptionServiceProvider.cs" company="Siemens AG">
//   Copyright (c) Siemens AG. All rights reserved.
// </copyright>
// <summary>
//   provides algorithms based on symetric algorithm to encrypt and decrypt data for security purposes. it uses a wrapper object of TripleDESCryptoServiceProvider to access the cryptographic service provider (CSP) 
// version of the Data Encryption Standard (DES) algorithm
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataEncryptionStandardServiceProvider
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using CuttingEdge.Conditions;

    /// <summary> provides algorithms based on symetric algorithm to encrypt and decrypt data for security purposes </summary>
    public static class DataEncryptionServiceProvider
    {
        /// <summary> The security token  used for hash generation. </summary>
        private static readonly string SecurityToken =
            "7WY8GvClb8jRLcbzHxUjhhYcCAnmVF32C2ajxOEKXYAHbNhUlbdDtbR6NesHVwKxyVR7KwRHuspGfGABTPfm06tNlXvgtgO3cFaXc2EHu4oLM0mEPC2QKvD5t9lYqa1C";

        /// <summary>The encrypt string.</summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>The <see cref="string"/> the encrypted string </returns>
        public static string EncryptString(string plainText)
        {
            
            return Convert.ToBase64String(EncryptedPlainTextToByte(plainText, HashTheSecurityToken()));
        }

        /// <summary>The decrypt string.</summary>
        /// <param name="encryptedString">The encrypted string.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string DecryptString(string encryptedString)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetString(DecryptTheEncryptedTextToByte(encryptedString, HashTheSecurityToken()));
        }

        /// <summary>The encrypted plain text to byte.</summary>
        /// <param name="plainText">The plain text to ecrypt </param>
        /// <param name="key">The key used to encrypt the text </param>
        /// <returns>The <see cref="byte"/> return the encrypted text in bytes </returns>
        private static byte[] EncryptedPlainTextToByte(string plainText, byte[] key)
        {
            ValidateArgument(plainText, "Text");

            var tripleDesCryptoService = CreateTripleDesCryptoServiceProvider(key);
            var dataToEncrypt = new UTF8Encoding().GetBytes(plainText);
            try
            {
                var encryptor = tripleDesCryptoService.CreateEncryptor();
                return encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                tripleDesCryptoService.Clear();
            }
        }

        /// <summary>The Decrypt the encrypted  byte.</summary>
        /// <param name="encryptedText">the encrypted text </param>
        /// <param name="key">The key used to encrypt the text </param>
        /// <returns>The <see cref="byte"/> return the encrypted text in bytes </returns>
        private static byte[] DecryptTheEncryptedTextToByte(string encryptedText, byte[] key)
        {
            ValidateArgument(encryptedText, "Text");

            var tripleDesCryptoService = CreateTripleDesCryptoServiceProvider(key);
           // var dataToEncrypt = new UTF8Encoding().GetBytes(encryptedText);
            try
            {
                var dataToDecrypt = Convert.FromBase64String(encryptedText);
                var decryptor = tripleDesCryptoService.CreateDecryptor();
                return decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                tripleDesCryptoService.Clear();
            }
        }

        /// <summary>The validate argument.</summary>
        /// <param name="input">The plain text.</param>
        /// <param name="name">The name.</param>
        private static void ValidateArgument(string input, string name)
        {
            Condition.Requires(input, name).IsNotNull().IsNotEmpty();
            Condition.Ensures(input.Length, "Text lenght to encrypt").IsGreaterThan(0);
        }

        /// <summary>The create triple des crypto service provider.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="TripleDESCryptoServiceProvider"/> the instantiated DES </returns>
        private static TripleDESCryptoServiceProvider CreateTripleDesCryptoServiceProvider(byte[] key)
        {
            ValidateArgument(Convert.ToBase64String(key), "Key");

            var tripleDesCryptoService = new TripleDESCryptoServiceProvider();

            // the CBC (Chipher Bolck Chaining) is more efficient and secured than ECB
            // we need to fix the IV for encryption and decryption. This is because the encryption and decryption methods need have the same IV
            // tripleDesCryptoService.KeySize = key.Length;
            // https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation#Electronic_Codebook_.28ECB.29
            var initializationVector = new byte[] { 13, 22, 102, 174, 194, 3, 99, 234 };

            // var provider = new RNGCryptoServiceProvider();
            // provider.GetBytes(initializationVector);
            tripleDesCryptoService.Key = key;
            tripleDesCryptoService.IV = initializationVector;
            tripleDesCryptoService.Mode = CipherMode.CBC;
           tripleDesCryptoService.Padding = PaddingMode.PKCS7;       

            return tripleDesCryptoService;
        }

        /// <summary>The hash the security token.</summary>
        /// <returns>The <see /> hashed security token </returns>
        private static byte[] HashTheSecurityToken()
        {
            var encoding = new UTF8Encoding();
            var md5CryptoService = new MD5CryptoServiceProvider();
            return md5CryptoService.ComputeHash(encoding.GetBytes(SecurityToken));
        }
    }
}
