using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TopCom.Models
{
    public class DataHelperBase : IDataHelper
    {
        #region Properties
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
            }
        }

        public object Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
                Save();
            }
        }
        #endregion

        #region Privates
        private string _FilePath;
        private object _Data;
        #endregion

        protected string Encrypt(string stringToEncrypt)
        {
            string EncryptionKey = "QMPCIJH4RDYL1BG82V9UAN6ZFT30KXO57ESW";
            byte[] clearBytes = Encoding.Unicode.GetBytes(stringToEncrypt);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x08, 0x31, 0x3f, 0x2a, 0xdd, 0x0a, 0x56, 0x5a, 0x0a, 0xe7, 0x2b, 0xba, 0xf8, 0x33, 0xe9, 0xd7
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    stringToEncrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            return stringToEncrypt;
        }

        protected string Decrypt()
        {
            using (StreamWriter w = File.AppendText(FilePath)) { }
            string cipherText = File.ReadAllText(FilePath);

            string EncryptionKey = "QMPCIJH4RDYL1BG82V9UAN6ZFT30KXO57ESW";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x08, 0x31, 0x3f, 0x2a, 0xdd, 0x0a, 0x56, 0x5a, 0x0a, 0xe7, 0x2b, 0xba, 0xf8, 0x33, 0xe9, 0xd7
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public virtual void Save()
        {
        }

        /// <summary>
        /// Change Data on Load function
        /// </summary>
        public virtual void Load<T>()
        {
        }
    }
}
