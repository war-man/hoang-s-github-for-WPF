using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ClassEnglishGame
{
    public static class FileManagement
    {
        public static List<GameItem> GetListGameData()
        {
            return !File.Exists(Constant.FileName)
                       ? new List<GameItem>()
                       : FromXmlString<List<GameItem>>(File.ReadAllText(Constant.FileName));
        }

        public static void SaveListGameData(List<GameItem> gameItems)
        {
            File.WriteAllText(Constant.FileName, ToXmlString(gameItems.OrderBy(x=>x.GameName).ToList()));
        }

        public static string ToXmlString<T>(T objectToSerialize)
        {
            var stream = new MemoryStream();

            TextWriter writer = new StreamWriter(stream, new UTF8Encoding());

            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(writer, objectToSerialize);

            return Encoding.UTF8.GetString(stream.ToArray(), 0, Convert.ToInt32(stream.Length));
        }

        public static T FromXmlString<T>(String source)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            var sr = new StringReader(source);
            return (T)xmlSerializer.Deserialize(sr);
        }

        public static string SaveBackupData(List<GameItem> gameItems)
        {
            var fileName = string.Format("Data_{0}.xml", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            File.WriteAllText(fileName, ToXmlString(gameItems.OrderBy(x => x.GameName).ToList()));
            return fileName;
        }
    }
}
