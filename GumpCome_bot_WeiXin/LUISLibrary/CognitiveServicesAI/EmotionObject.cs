using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesAI
{
    public class Scores
    {
        /// <summary>
        /// 气愤
        /// </summary>
        public double anger { get; set; }
        /// <summary>
        /// 蔑视
        /// </summary>
        public double contempt { get; set; }
        /// <summary>
        /// 厌恶
        /// </summary>
        public double disgust { get; set; }
        /// <summary>
        /// 害怕
        /// </summary>
        public double fear { get; set; }
        /// <summary>
        /// 高兴
        /// </summary>
        public double happiness { get; set; }
        /// <summary>
        /// 自然
        /// </summary>
        public double neutral { get; set; }
        /// <summary>
        /// 沮丧
        /// </summary>
        public double sadness { get; set; }
        /// <summary>
        /// 惊讶
        /// </summary>
        public double surprise { get; set; }

        public string GetEmotion()
        {
            Dictionary<string, double> emotions = new Dictionary<string, double>();
            emotions.Add("气愤", anger);
            emotions.Add("蔑视", contempt);
            emotions.Add("厌恶", disgust);
            emotions.Add("害怕", fear);
            emotions.Add("高兴", happiness);
            emotions.Add("自然", neutral);
            emotions.Add("沮丧", sadness);
            emotions.Add("惊讶", surprise);
            var emotion = emotions.OrderByDescending(sn => sn.Value).FirstOrDefault();
            return emotion.Key;
        }
    }

    public class AnalyzeEmotionObject
    {
        public FaceRectangle faceRectangle { get; set; }
        public Scores scores { get; set; }
    }

}
