namespace EyeInTheSky.Services
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
            
            if (value == "true" || value == "1" || value == "yes" || value == "on" || value == "enable" || value == "enabled")
            {
                result = true;
                return true;
            }
            
            if (value == "false" || value == "0" || value == "no" || value == "off" || value == "disable" || value == "disabled")
            {
                result = false;
                return true;
            }
            
            return bool.TryParse(value, out result);
        }
    }
}