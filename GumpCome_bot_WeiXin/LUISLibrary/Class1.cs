using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LUISLibrary
{
    public class LUISHelper
    {
        public static async void MakeRequest()
        {
            string SubscriptionKey = "1813ab32ba51469ca2c09d17731e3e8d";
            string LUISid = "e59a1eb2-0823-444a-9f65-9005c7c2102e";
            var client = new HttpClient();
            //var queryString = HttpUtility.ParseQueryString("贩卖机13号货到卖什么");
            //var queryString = "贩卖机13号货到卖什么";
            var queryString = HttpUtility.UrlEncode("贩卖机13号货到卖什么");
            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            var uri = "https://api.projectoxford.ai/luis/v1/application?id=" + LUISid + "&q=" + queryString;
            var response = await client.GetAsync(uri);
            string JSON = await response.Content.ReadAsStringAsync();
            LUIS luis = JsonHelper.Deserialize<LUIS>(JSON);

            Intent intent = null;
            Entity entity = null;
            if (luis.intents != null && luis.intents.Count != 0)
                intent = luis.intents.OrderByDescending(sn => sn.score).FirstOrDefault();
            if (luis.entities != null && luis.entities.Count != 0)
                entity = luis.entities.OrderByDescending(sn => sn.score).FirstOrDefault();
            return;
        }
    }

    [DataContract]
    public class Intent
    {
        [DataMember]
        public string intent { get; set; }
        [DataMember]
        public double score { get; set; }
    }

    [DataContract]
    public class Resolution
    {
        [DataMember]
        public string date { get; set; }
    }

    [DataContract]
    public class Entity
    {
        [DataMember]
        public string entity { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public int startIndex { get; set; }
        [DataMember]
        public int endIndex { get; set; }
        [DataMember]
        public double score { get; set; }
        [DataMember]
        public Resolution resolution { get; set; }
    }

    [DataContract]
    public class LUIS
    {
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public List<Intent> intents { get; set; }
        [DataMember]
        public List<Entity> entities { get; set; }
    }


    public class JsonHelper
    {
        /// <summary>
        /// 将JSON字符串反序列化成数据对象
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>返回数据对象</returns>
        public static T Deserialize<T>(string json)
        {
            var _Bytes = Encoding.Unicode.GetBytes(json);
            using (MemoryStream _Stream = new MemoryStream(_Bytes))
            {
                var _Serializer = new DataContractJsonSerializer(typeof(T));
                return (T)_Serializer.ReadObject(_Stream);
            }
        }

        /// <summary>
        /// 将object序列化成JSON字符串 
        /// </summary>
        /// <param name="instance">被序列化对象</param>
        /// <returns>返回json字符串</returns>
        public static string Serialize(object instance)
        {
            using (MemoryStream _Stream = new MemoryStream())
            {
                var _Serializer = new DataContractJsonSerializer(instance.GetType());
                _Serializer.WriteObject(_Stream, instance);
                _Stream.Position = 0;
                using (StreamReader _Reader = new StreamReader(_Stream))
                { return _Reader.ReadToEnd(); }
            }
        }
    }
}
