namespace View.Helpers
{
    public abstract class RichTextHelper
    {
        public static string FormatRed(int textInt)
        {
            return FormatRed(textInt.ToString());
        }

        public static string FormatRed(string text)
        {
            return FormatColor(text, "red");
        }
        
        public static string FormatGreen(int textInt)
        {
            return FormatGreen(textInt.ToString());
        }

        public static string FormatGreen(string text)
        {
            return FormatColor(text, "green");
        }
        
        public static string FormatYellow(int textInt)
        {
            return FormatYellow(textInt.ToString());
        }

        public static string FormatYellow(string text)
        {
            return FormatColor(text, "yellow");
        }
        
        public static string FormatBlue(int textInt)
        {
            return FormatBlue(textInt.ToString());
        }

        public static string FormatBlue(string text)
        {
            return FormatColor(text, "blue");
        }
        
        public static string FormatColor(int text, string color)
        {
            return FormatColor(text.ToString(), color);
        }
        
        public static string FormatColor(string text, string color)
        {
            return $"<color={color}>{text}</color>";
        }
    }
}