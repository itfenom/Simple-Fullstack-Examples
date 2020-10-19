using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Playground.Core.Utilities
{
    public class Encryption
    {
        [DebuggerNonUserCode]
        public Encryption()
        {
        }

        private static CryptoStream CreateCryptoStream(Stream oIo, bool bEncrypt)
        {
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();

            if (cryptoServiceProvider == null)
                throw new Exception(@"unable to instantiate crypto provider");

            string s1 = @"H32(8s_)28nweupz_r1tw3VW";
            string s2 = @"s3*134bs";
            cryptoServiceProvider.Key = Encoding.ASCII.GetBytes(s1);
            cryptoServiceProvider.IV = Encoding.ASCII.GetBytes(s2);
            CryptoStream cryptoStream;
            if (bEncrypt)
            {
                ICryptoTransform encryptor = cryptoServiceProvider.CreateEncryptor();
                cryptoStream = new CryptoStream(oIo, encryptor, CryptoStreamMode.Write);
            }
            else
            {
                ICryptoTransform decryptor = cryptoServiceProvider.CreateDecryptor();
                cryptoStream = new CryptoStream(oIo, decryptor, CryptoStreamMode.Read);
            }
            return cryptoStream;
        }

        public static string Encrypt(string sText)
        {
            string retVal;
            try
            {
                using (var memStream = new MemoryStream())
                {
                    using (var cryptoStr = CreateCryptoStream(memStream, true))
                    {
                        if (cryptoStr == null)
                            throw new Exception("unable to instantiate crypto stream");

                        byte[] bytes = Encoding.ASCII.GetBytes(sText);
                        cryptoStr.Write(bytes, 0, bytes.Length);
                        cryptoStr.FlushFinalBlock();
                        retVal = Convert.ToBase64String(memStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("unable to encrypt text", ex);
                Console.WriteLine(ex.Message);
                retVal = null;
            }

            return retVal;
        }

        public static string Decrypt(string sText)
        {
            string retVal;
            try
            {
                using (var memStream = new MemoryStream(Convert.FromBase64String(sText)))
                {
                    using (var cryptoStr = CreateCryptoStream(memStream, false))
                    {
                        if (cryptoStr == null)
                            throw new Exception("unable to instantiate crypto stream");

                        using (var strReader = new StreamReader(cryptoStr))
                        {
                            retVal = strReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("unable to decrypt string", ex);
                Console.WriteLine(ex.Message);
                retVal = null;
            }

            return retVal;
        }
    }
}
