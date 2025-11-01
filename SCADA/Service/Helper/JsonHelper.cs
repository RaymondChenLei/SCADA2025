using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Helper
{
    public class JsonHelper
    {
        public static T GetData<T>(string filePath)
        {
            string content;
            content = File.ReadAllText(filePath);
            T jsonObject = JsonConvert.DeserializeObject<T>(content)!;
            return jsonObject;
        }

        public static void WrtToFile(string filePath, object o)
        {
            string conFilePath = filePath;

            if (!File.Exists(conFilePath))
            {
                File.Create(conFilePath).Close();
            }

            //把模型数据写到文件
            using (StreamWriter sw = new StreamWriter(conFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;

                //构建Json.net的写入流
                JsonWriter writer = new JsonTextWriter(sw);
                //把模型数据序列化并写入Json.net的JsonWriter流中
                serializer.Serialize(writer, o);
                //ser.Serialize(writer, ht);
                writer.Close();
                sw.Close();
            }
        }
    }
}