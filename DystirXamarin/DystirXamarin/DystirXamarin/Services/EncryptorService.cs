﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DystirXamarin.Services
{
    public class EncryptorService
    {
        const string ENCTRIPTION_KEY = "PREDRAG01021977";
        public string Token;

        public string Encrypt(string inText)
        {
            byte[] bytesBuff = Encoding.Unicode.GetBytes(inText);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(ENCTRIPTION_KEY, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    Token = Convert.ToBase64String(mStream.ToArray());
                }
            }
            return Token.Replace("+", "").Replace("/", "").Replace("-", "");
        }

        public string Decrypt(string cryptTxt)
        {
            //cryptTxt = cryptTxt.Replace(" ", "+");
            byte[] bytesBuff = Convert.FromBase64String(cryptTxt);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(ENCTRIPTION_KEY, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    cryptTxt = Encoding.Unicode.GetString(mStream.ToArray());
                }
            }
            return cryptTxt;
        }
    }
}
