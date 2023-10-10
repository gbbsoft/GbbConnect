using System.Security.Cryptography;
using System.Text;

namespace GbbLib
{
    public class Encryption
    {

        // Źródło: https://damienbod.com/2020/08/19/symmetric-and-asymmetric-encryption-in-net-core/

        // ======================================
        // Hash
        // ======================================

        public static string Hash(SHA256 sha256Hash, string Text) // using (SHA256 sha256Hash = SHA256.Create())
        {
            // Convert the input string to a byte array and compute the hash.
            return Hash(sha256Hash, Encoding.UTF8.GetBytes(Text));
        }

        public static string Hash(SHA256 sha256Hash, byte[] Text) // using (SHA256 sha256Hash = SHA256.Create())
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = sha256Hash.ComputeHash(Text);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        // ======================================
        // RSA - asymetryczny
        // ======================================
        public static (string PrivKeyBase64, string PubKeyBase64) RSA_GenKeys(RSA rsa) // using (RSA rsa = RSA.Create())
        {
            var PrivKey = System.Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            var PubKey = System.Convert.ToBase64String(rsa.ExportRSAPublicKey());
            return (PrivKey, PubKey);
        }

        public static string RSA_Encrypt(RSA rsa, string PubKeyBase64, string Text)
        {
            rsa.ImportRSAPublicKey(System.Convert.FromBase64String(PubKeyBase64), out _);

            var plainTextBytes = Encoding.Unicode.GetBytes(Text);

            var cipherTextBytes = rsa.Encrypt(plainTextBytes, RSAEncryptionPadding.Pkcs1);
            return System.Convert.ToBase64String(cipherTextBytes);
        }

        public static string RSA_Decrypt(RSA rsa, string PrivKeyBase64, string cipherText) // using (RSA rsa = RSA.Create())
        {
            rsa.ImportRSAPrivateKey(System.Convert.FromBase64String(PrivKeyBase64), out _);

            var cipherTextBytes = System.Convert.FromBase64String(cipherText);
            var plainTextBytes = rsa.Decrypt(cipherTextBytes, RSAEncryptionPadding.Pkcs1);
            return Encoding.Unicode.GetString(plainTextBytes);
        }

        // ======================================
        // AES - symetryczny
        // ======================================
        public static string AES_Encrypt(string text, string IV, string key)
        {
            Aes cipher = AES_CreateCipher(key);
            cipher.IV = System.Convert.FromBase64String(IV);

            ICryptoTransform cryptTransform = cipher.CreateEncryptor();
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

            return System.Convert.ToBase64String(cipherText);
        }

        public static string AES_Decrypt(string encryptedText, string IV, string key)
        {
            Aes cipher = AES_CreateCipher(key);
            cipher.IV = System.Convert.FromBase64String(IV);

            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);
            byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

        // ======================================

        public static (Aes cipher, string keyBase64, string IVBase64) 
            AES_InitSymmetricEncryptionKeyIV()
        {
            var keyBase64 = GetEncodedRandomString(32); // 256
            var cipher = AES_CreateCipher(keyBase64);
            var IVBase64 = System.Convert.ToBase64String(cipher.IV);
            return (cipher, keyBase64, IVBase64);
        }

        public static string AES_Encrypt(Aes cipher, string IVBase64, string text) // using(Aes cipher = CreateCipher(key))
        {
            cipher.IV = System.Convert.FromBase64String(IVBase64);

            ICryptoTransform cryptTransform = cipher.CreateEncryptor();
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

            return System.Convert.ToBase64String(cipherText);
        }


        public static string AES_Decrypt(Aes cipher, string IVBase64, string encryptedText) // using(Aes cipher = CreateCipher(key))
        {
            cipher.IV = System.Convert.FromBase64String(IVBase64);

            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);
            byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

        public static Aes AES_CreateCipher(string keyBase64)
        {
            // Default values: Keysize 256, Padding PKC27
            Aes cipher = Aes.Create();
            cipher.Mode = CipherMode.CBC;  // Ensure the integrity of the ciphertext if using CBC

            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = System.Convert.FromBase64String(keyBase64);

            return cipher;
        }

        // ======================================
        // Tools
        // ======================================

        public static string GetEncodedRandomString(int length)
        {
            var base64 = System.Convert.ToBase64String(GenerateRandomBytes(length));
            return base64;
        }

        public static byte[] GenerateRandomBytes(int length)
        {
            var byteArray = new byte[length];
            RandomNumberGenerator.Fill(byteArray);
            return byteArray;
        }

    }
}
