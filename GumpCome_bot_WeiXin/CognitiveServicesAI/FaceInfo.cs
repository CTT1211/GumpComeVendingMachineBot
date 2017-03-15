using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesAI
{
    [DataContract]
    public class FaceRectangle
    {
        [DataMember]
        public int left { get; set; }
        [DataMember]
        public int top { get; set; }
        [DataMember]
        public int width { get; set; }
        [DataMember]
        public int height { get; set; }
    }

    [DataContract]
    public class Celebrity
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public FaceRectangle faceRectangle { get; set; }
        [DataMember]
        public double confidence { get; set; }
    }

    [DataContract]
    public class Detail
    {
        [DataMember]
        public List<Celebrity> celebrities { get; set; }
    }

    [DataContract]
    public class Category
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public double score { get; set; }
        [DataMember]
        public Detail detail { get; set; }
    }

    [DataContract]
    public class Metadata
    {
        [DataMember]
        public int width { get; set; }
        [DataMember]
        public int height { get; set; }
        [DataMember]
        public string format { get; set; }
    }

    [DataContract]
    public class Face
    {
        [DataMember]
        public int age { get; set; }
        [DataMember]
        public string gender { get; set; }
        [DataMember]
        public FaceRectangle faceRectangle { get; set; }
    }

    [DataContract]
    public class AnalyzeImageObject
    {
        [DataMember]
        public List<Category> categories { get; set; }
        [DataMember]
        public string requestId { get; set; }
        [DataMember]
        public Metadata metadata { get; set; }
        [DataMember]
        public List<Face> faces { get; set; }
    }
}
