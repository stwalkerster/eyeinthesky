namespace EyeInTheSky.Helpers
{
    public class BooleanParser
    {
        public static bool TryParse(string value, out bool result)
        {
            if (value == null)
            {
                result = false;
                return false;
            }
            
            value = value.ToLowerInvariant().Trim();
            
            if (value == "true" || value == "1" || value == "yes" || value == "on")
            {
                result = true;
                return true;
            }
            
            if (value == "false" || value == "0" || value == "no" || value == "off")
            {
                result = false;
                return true;
            }
            
            return bool.TryParse(value, out result);
        }
    }
}