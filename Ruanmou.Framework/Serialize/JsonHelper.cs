using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;//System.Web.Extensions
using Newtonsoft.Json;//nuget Newtonsoft

namespace Ruanmou.Framework.Serialize
{
    public class JsonHelper
    {
        #region Json
        /// <summary>
        /// JavaScriptSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToString<T>(T obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(obj);
        }

        /// <summary>
        /// JavaScriptSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T StringToObject<T>(string content)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<T>(content);
        }

        /// <summary>
        /// JsonConvert.SerializeObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// JsonConvert.DeserializeObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T ToObject<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
        #endregion Json

        public static T JsonFileToObject<T>(string filename)
        {
            string fullname = Path.Combine(StaticConstant.JsonPath, filename);
            if (File.Exists(fullname))
            {
                string json = File.ReadAllText(fullname, Encoding.UTF8);
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                throw new Exception($"json文件 {fullname} 不存在");
            }
        }
    }
}
