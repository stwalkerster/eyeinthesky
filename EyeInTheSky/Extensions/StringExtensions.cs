namespace EyeInTheSky.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class StringExtensions
    {
        private const string Quote = "\"";
        
        public static IEnumerable<string> ToParameters(this string input)
        {
            if (input == String.Empty)
            {
                return new List<string>();
            }
            
            var strings = input.Split(' ');
            var output = new List<string>();

            for (var i = 0; i < strings.Length; i++)
            {
                if (strings[i].StartsWith(Quote) && strings[i].EndsWith(Quote))
                {
                    output.Add(strings[i].Substring(1, strings[i].Length - 2));
                    continue;
                }

                if (strings[i].StartsWith(Quote))
                {
                    var joined = strings[i].Substring(1);

                    while (i < strings.Length - 1)
                    {
                        i++;

                        if (strings[i].EndsWith(Quote))
                        {
                            joined += " " + strings[i].Substring(0, strings[i].Length - 1);
                            break;
                        }

                        joined += " " + strings[i];
                    }
                    
                    output.Add(joined);
                    continue;
                }
                
                output.Add(strings[i]);
            }

            return output;
        }
    }
}