using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace ConsoleAppForTest
{
    class Program
    {
        static void Main()
        {
            MakeRequest();

            //PostMessage1("你好");
            //return;
            //MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();

            //https://api.projectoxford.ai/luis/v1/application?id=e59a1eb2-0823-444a-9f65-9005c7c2102e&subscription-key=1813ab32ba51469ca2c09d17731e3e8d&q=%E8%B4%A9%E5%8D%96%E6%9C%BA13%E5%8F%B7%E8%B4%A7%E5%88%B0%E5%8D%96%E4%BB%80%E4%B9%88 
        }
        public static async void MakeRequest()
        {
            //LUISLibrary.LUISHelper.MakeRequest();
            string SubscriptionKey = "1813ab32ba51469ca2c09d17731e3e8d";
            string LUISid = "e59a1eb2-0823-444a-9f65-9005c7c2102e";
            var client = new HttpClient();
            //var queryString = HttpUtility.ParseQueryString("贩卖机13号货到卖什么");
            //var queryString = "贩卖机13号货到卖什么";
            var queryString = HttpUtility.UrlEncode("呵呵");
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

        private async static void PostMessage(string message)
        {
            HttpClient client;
            HttpResponseMessage response;

            bool IsReplyReceived = false;

            client = new HttpClient();
            client.BaseAddress = new Uri("https://directline.botframework.com/api/conversations/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "k76EO3K9-4k.cwA.dJE.H3FiSZ1k26xokQUGCf4nke78Lo7_eu9UU0_hqvd3_Lw");
            response = await client.GetAsync("/api/tokens/");
            if (response.IsSuccessStatusCode)
            {
                var conversation = new Conversation();
                response = await client.PostAsJsonAsync("/api/conversations/", conversation);
                //response = await client.PostAsync("/api/conversations/", null);
                if (response.IsSuccessStatusCode)
                {
                    Conversation ConversationInfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
                    string conversationUrl = ConversationInfo.conversationId + "/messages/";
                    Message msg = new Message() { text = message };
                    response = await client.PostAsJsonAsync(conversationUrl, msg);
                    if (response.IsSuccessStatusCode)
                    {
                        response = await client.GetAsync(conversationUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageSet BotMessage = response.Content.ReadAsAsync(typeof(MessageSet)).Result as MessageSet;
                            //string Messages = BotMessage;
                            IsReplyReceived = true;
                        }
                    }
                }

            }
            //return IsReplyReceived;
        }


        private async static void PostMessage1(string message)
        {
            HttpClient client;
            HttpResponseMessage response;

            bool IsReplyReceived = false;

            client = new HttpClient();
            client.BaseAddress = new Uri("https://directline.botframework.com");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "k76EO3K9-4k.cwA.dJE.H3FiSZ1k26xokQUGCf4nke78Lo7_eu9UU0_hqvd3_Lw");
            //response = await client.GetAsync("/api/tokens/");
            //if (response.IsSuccessStatusCode)
            //{
            var conversation = new Conversation();
            response = await client.PostAsJsonAsync("/api/conversations/", conversation);
            //response = await client.PostAsync("/api/conversations/", null);
            if (response.IsSuccessStatusCode)
            {
                Conversation ConversationInfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
                string conversationUrl = "/api/conversations/" + ConversationInfo.conversationId + "/messages/";
                Message msg = new Message() { text = message };
                response = await client.PostAsJsonAsync(conversationUrl, msg);
                if (response.IsSuccessStatusCode)
                {
                    response = await client.GetAsync(conversationUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageSet BotMessage = response.Content.ReadAsAsync(typeof(MessageSet)).Result as MessageSet;
                        //string Messages = BotMessage;
                        IsReplyReceived = true;
                    }
                }
            }

            //}
            //return IsReplyReceived;
        }

        public class Conversation
        {
            public string conversationId { get; set; }
            public string token { get; set; }
            public string eTag { get; set; }
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

    public class Conversation
    {
        public string conversationId { get; set; }
        public string token { get; set; }
        public string eTag { get; set; }
    }

    public class MessageSet
    {
        public Message[] messages { get; set; }
        public string watermark { get; set; }
        public string eTag { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string conversationId { get; set; }
        public DateTime created { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public string channelData { get; set; }
        public string[] images { get; set; }
        public Attachment[] attachments { get; set; }
        public string eTag { get; set; }
    }

    public class Attachment
    {
        public string url { get; set; }
        public string contentType { get; set; }
    }

}
