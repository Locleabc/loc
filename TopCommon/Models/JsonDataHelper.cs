using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TopCom.Models
{
    public class JsonDataHelper : DataHelperBase
    {
        public delegate object SetDefaultData();
        private SetDefaultData HandlerSetDefaultData;

        public JsonDataHelper(string filePath, SetDefaultData _handlerSetDefaultData)
        {
            FilePath = filePath;
            HandlerSetDefaultData = _handlerSetDefaultData;
        }

        public override void Load<T>()
        {
            try
            {
                string decryptString = Decrypt();

                if (decryptString.Length < 3)
                {
                    Data = (T)HandlerSetDefaultData();
                }
                else
                {
                    Data = JsonConvert.DeserializeObject<T>(decryptString);
                }
            }
            catch (Exception)
            {
                Data = (T)HandlerSetDefaultData();
            }
        }

        public override void Save()
        {
            string stringAfterEncrypt = Encrypt(JsonConvert.SerializeObject(Data));

            using (StreamWriter writer = new StreamWriter(FilePath, false))
            {
                writer.Write(stringAfterEncrypt);
            }
        }
    }
}
