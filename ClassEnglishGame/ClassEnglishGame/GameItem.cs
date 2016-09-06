using System;
using System.Xml.Serialization;

namespace ClassEnglishGame
{
    [XmlRoot("GameItem"), Serializable]
    public class GameItem
    {
        [XmlAttribute("Name")]
        public string GameName { get; set; }

        [XmlAttribute("Topic")]
        public string Topic { get; set; }

        [XmlElement("Title")]
        public string Title { get; set; }

        //[XmlElement("ImageLink")]
        //public string ImageLink { get; set; }
        [XmlElement("Image")]
        public byte[] Image { get; set; }

        [XmlElement("IgnoreWord")]
        public string IgnoreWord { get; set; }

        [XmlElement("ExplainText")]
        public string ExplainText { get; set; }

        [XmlIgnore]
        public int Order { get; set; }
    }
}