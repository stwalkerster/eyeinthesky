namespace Stwalkerster.IrcClient
{
    public static class IrcColours
    {
        public const string ColorChar = "\x03";

        public const string BoldChar = "\x02";

        public enum Colours
        {
            White = 0,
            Black = 1,
            Blue = 2,
            Green = 3,
            Red = 4,
            Brown = 5,
            Purple = 6,
            Orange = 7,
            Yellow = 8,
            Lime = 9,
            Teal = 10,
            Cyan = 11,
            Lightblue = 12,
            Pink = 13,
            Grey = 14,
            Lightgrey = 15
        }

        public static string BoldText(string text)
        {
            return BoldChar + text + BoldChar;
        }

        public static string ColouredText(Colours color, string text)
        {
            return ColorChar + (int) color + text + ColorChar;
        }

        public static string ColouredText(Colours color, Colours background, string text)
        {
            return ColorChar + (int)color + "," + (int)background + text + ColorChar;
        }
    }
}
