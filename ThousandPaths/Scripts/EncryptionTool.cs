using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionTool
{
    private static readonly string encryptionKey = "Himitsu146215390"; //16 Char Key
    private static readonly string encryptionIV = "Wallz69420nanara"; //16 Char IV

    public static byte[] Encrypt(byte[] data)
    {
        using (AesManaged aes = new AesManaged())
        {
            aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aes.IV = Encoding.UTF8.GetBytes(encryptionIV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms,aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }
    }
    public static byte[] Decrypt(byte[] encryptedData)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aes.IV = Encoding.UTF8.GetBytes(encryptionIV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedData, 0, encryptedData.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }
    }
}
