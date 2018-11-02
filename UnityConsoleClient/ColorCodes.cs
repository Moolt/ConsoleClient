using System;
using System.Collections.Generic;

namespace UnityConsoleClient
{
    public static class ColorCodes
    {
        private static Dictionary<string, ConsoleColor> _mapping = new Dictionary<string, ConsoleColor>()
        {
            { "x", ConsoleColor.DarkRed },
            { "!", ConsoleColor.DarkYellow },
        };

        private const string Identifier = "#>>";

        public static bool IsColorCoded(string text)
        {
            return text.StartsWith(Identifier);
        }

        public static ConsoleColor ExtractColor(ref string text)
        {
            text = text.Remove(0, Identifier.Length);
            //Read message type
            var messageType = text[0] + "";
            //Remove message type identifier
            text = text.Remove(0, 1);

            if(_mapping.ContainsKey(messageType))
            {
                return _mapping[messageType];
            }

            return ConsoleColor.White;
        }
    }
}
