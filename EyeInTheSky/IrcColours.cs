using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky
{
    static class IrcColours
    {
        public const string colorChar = "\x03";

        public const string boldChar = "\x02";

        public enum Colours
        {
            white = 0,
            black = 1,
            blue = 2,
            green = 3,
            red = 4,
            brown = 5,
            purple = 6,
            orange = 7,
            yellow = 8,
            lime = 9,
            teal = 10,
            cyan = 11,
            lightblue = 12,
            pink = 13,
            grey = 14,
            lightgrey = 15
        }

        public static string boldText(string text)
        {
            return boldChar + text + boldChar;
        }

        public static string colouredText(Colours color, string text)
        {
            return colorChar + ((int) color) + text + colorChar;
        }

        public static string colouredText(Colours color, Colours background, string text)
        {
            return colorChar + ((int)color) + "," + ((int)background) + text + colorChar;
        }
    }
}
