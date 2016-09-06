using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;

namespace ClassEnglishGame
{
    public static class EnglishGameHelper
    {
        private static SpeechSynthesizer _reader = new SpeechSynthesizer();

        public static void SpeakText(string text)
        {
            _reader.Dispose();
            if (!String.IsNullOrEmpty(text))
            {
                _reader = new SpeechSynthesizer();
                _reader.SpeakAsync(text);
            }
        }

        public static T CloneItem<T>(T obj)
        {
            return FileManagement.FromXmlString<T>(FileManagement.ToXmlString(obj));
        }

        public static void SpeakNumberBetween(int number, int max=10, int min=0)
        {
            if (number<=10 && number>=0)
            {
                SpeakText(number.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static List<GameItem> RandomOrder(List<GameItem> gameItems)
        {
            var random=new Random();
            foreach (var gameItem in gameItems)
            {
                gameItem.Order = random.Next(0, gameItems.Count);
            }

            return gameItems.OrderBy(x => x.Order).ToList();
        }
    }
}