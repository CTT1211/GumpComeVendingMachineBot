
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CognitiveServicesAI
{
    public class CognitiveServicesAIHelper
    {
        #region
        public static async Task<string> MakeAnalyzeImageFaceRequest(string URL)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            
            queryString["visualFeatures"] = "Categories,Faces";
            queryString["details"] = "Celebrities";
            queryString["language"] = "zh";

            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "a258a16179054b67b081c3622528279a");
            //var uri = "https://api.projectoxford.ai/vision/v1.0/analyze?" + queryString;

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "7e1f24eb51d647feafafcbf3b0628dec");
            var uri = "https://api.cognitive.azure.cn/vision/v1.0/analyze?" + queryString;
            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"https://portalstoragewuprod2.azureedge.net/face/demo/detection%205.jpg\"}");
            //byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"http://g.hiphotos.baidu.com/baike/w%3D268%3Bg%3D0/sign=363b9fb9ef24b899de3c7e3e563d7aa8/0823dd54564e9258cf0a33579a82d158ccbf4e48.jpg\"}");
            //byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"" + URL + "\"}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                string JSON = await response.Content.ReadAsStringAsync();

                StringBuilder sb = new StringBuilder();

                AnalyzeImageObject ro = JsonHelper.Deserialize<AnalyzeImageObject>(JSON);

                if (ro.faces.Count != 0)
                {
                    Face _face = ro.faces[0];

                    if (_face.gender == "Male")
                        sb.Append("男生_");
                    else
                        sb.Append("女生_");

                    sb.Append("看上去" + _face.age + "岁_");

                    if (ro.categories[0].detail.celebrities.Count != 0)
                    {
                        sb.Append("这是" + ro.categories[0].detail.celebrities[0].name + "吗？");
                    }
                }
                else
                {
                    sb.Append("没发现有人啊");
                }

                return sb.ToString();
            }

        }

        public static async Task<string> MakeAnalyzeImageEmotionRequest(string URL)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "6a6efaff78d64a3ebe92e3db7276bd42");
            //var uri = "https://api.projectoxford.ai/emotion/v1.0/recognize?" + queryString;

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "af9f603c21b449328baaacbb89b95adf");
            var uri = "https://api.cognitive.azure.cn/emotion/v1.0/recognize?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"https://portalstoragewuprod2.azureedge.net/face/demo/detection%205.jpg\"}");
            //byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"" + URL + "\"}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                string JSON = await response.Content.ReadAsStringAsync();

                StringBuilder sb = new StringBuilder();

                List<AnalyzeEmotionObject> ro = JsonHelper.Deserialize<List<AnalyzeEmotionObject>>(JSON);

                if (ro.Count != 0)
                {
                    sb.Append(ro[0].scores.GetEmotion() + "的样子");
                }

                return sb.ToString();
            }
        }
        #endregion
    }
}
