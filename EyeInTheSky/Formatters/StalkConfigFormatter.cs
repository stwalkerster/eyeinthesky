namespace EyeInTheSky.Formatters
{
    using System;
    using System.Security;
    using System.Text.RegularExpressions;

    public class StalkConfigFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!this.Equals(formatProvider))
            {
                return null;
            }

            var result = arg.ToString();

            if (!string.IsNullOrEmpty(format))
            {
                if (format.Contains("R"))
                {
                    result = Regex.Escape(result);
                }
            }

            // escape for xml
            result = SecurityElement.Escape(result);

            return result;
        }
    }
}