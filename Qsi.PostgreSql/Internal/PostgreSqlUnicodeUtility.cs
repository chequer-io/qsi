using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qsi.PostgreSql.Internal;

internal static class PostgreSqlUnicodeUtility
{
    /// <summary>
    /// Get a quoted string from a unicode string.
    /// </summary>
    /// <param name="input">Input string which contains unicodes.</param>
    /// <param name="quote">Quote character to attach at the start and the end. Default value is &#39;&#34;&#39;.</param>
    /// <returns>A string with quotes.</returns>
    /// <exception cref="Exception"></exception>
    public static string GetString(string input, char quote = '\"')
    {
        // If input length is invalid, return itself.
        if (input.Length < 2)
        {
            return input;
        }
        
        // If input is not starting with unicode symbol, return itself.
        if (input[..2] is not "U&" and not "u&")
        {
            return input;
        }
        
        // Split quotes and escape letter
        var i = 2;
        var splitted = new List<string>();
        bool isQuoted = false;
        var temp = "";
        
        while (i < input.Length)
        {
            if (input[i] == '"')
            {
                isQuoted = !isQuoted;
            }

            temp += input[i];
            
            if (!isQuoted)
            {
                break;
            }

            i++;
        }

        if (isQuoted)
        {
            return input;
        }
        
        splitted.Add(temp);

        if (i + 1 < input.Length)
        {
            var rest = input[(i + 1)..];

            if (rest.Length != 0)
            {
                splitted.AddRange(rest.Split().Where(s => !string.IsNullOrWhiteSpace(s)));
            }
        }
        
        // If the number of splitted strings are invalid, return the input.
        if (splitted.Count is not 1 and not 3)
        {
            Console.WriteLine($"INVALID_INPUT, {splitted.Count}");
            return input;
        }

        // Parse splitted strings.
        var unicode = Dequote(splitted[0]);
        
        string parsed;
        
        if (splitted.Count == 3)
        {
            var escape = Dequote(splitted[2]);

            if (escape.Length != 1)
            {
                throw new Exception($"Invalid Unicode escape character \"{escape}\".");
            }

            if (escape[0] == '"')
            {
                throw new Exception($"Invalid Unicode escape character \"{escape}\".");
            }
            
            parsed = Parse(unicode, escape[0]);
        }
        else
        {
            parsed = Parse(unicode, '\\');   
        }
        
        // Add quotes to parsed string.
        parsed = parsed.Insert(0, quote.ToString());
        parsed += quote;
        
        return parsed;
    }
    
    /// <summary>
    /// Remove &#34;U&#38;&#34;, &#34;u&#38;&#34; and quotes.
    /// </summary>
    /// <param name="quotedInput">Input which contains escapes and quotes.</param>
    /// <returns>A string without escapes and quotes.</returns>
    public static string Dequote(string quotedInput)
    {
        if (quotedInput[..2] is "U&" or "u&")
        {
            quotedInput = quotedInput.Remove(0, 2);
        }

        if (quotedInput[0] is '"' or '\'' && quotedInput[^1] is '"' or '\'')
        {
            quotedInput = quotedInput[1..^1];
        }
        
        // quotedInput = IdentifierUtility.Unescape(quotedInput);

        return quotedInput;
    }

    /// <summary>
    /// Process Unicode escapes, producing a usual string.
    /// </summary>
    ///
    /// <param name="input">input string.</param>
    /// <param name="escape">The escape character to use.</param>
    /// <param name="position">Start position of string token. Default value is zero.</param>
    ///
    /// <remarks>
    /// Original function is written by C++ in <a href="https://github.com/postgres/postgres/blob/f4fb45d15c59d7add2e1b81a9d477d0119a9691a/src/backend/parser/parser.c#L362">HERE</a>.
    /// </remarks>
    public static string Parse(string input, char escape, int position = 0)
    {
        var output = "";
        int pairFirst = 0;

        var i = position;

        while (i < input.Length)
        {
            // If the letter is not an escape, pipe.
            if (input[i] != escape)
            {
                if (pairFirst != 0)
                {
                    throw new Exception("INVALID_PAIR");
                }

                output += input[i];
                i++;

                continue;
            }

            // If the next leter is also an escape, insert escape letter itself.
            if (i + 1 >= input.Length)
            {
                throw new Exception("Invalid Unicode escape. Hint: Unicode escapes must be \\XXXX or \\+XXXXXX.");
            }

            if (input[i + 1] == escape)
            {
                if (pairFirst != 0)
                {
                    throw new Exception("INVALID_PAIR");
                }

                output += escape;
                i += 2;

                continue;
            }

            // If next 4 letters are numbers, insert a unicode(2 bytes).
            if (i + 5 > input.Length)
            {
                throw new Exception("Invalid Unicode escape. Hint: Unicode escapes must be \\XXXX or \\+XXXXXX.");
            }
            
            var bit16 = input[(i + 1)..(i + 5)];

            if (bit16.All(char.IsNumber))
            {
                AddUnicode(bit16, ref pairFirst, ref output);

                i += 5;
                continue;
            }

            // If the next letter is plus, and also if next belonging 6 letters are numbers, insert a unicode(3 bytes).
            if (i + 8 > input.Length)
            {
                throw new Exception("Invalid Unicode escape. Hint: Unicode escapes must be \\XXXX or \\+XXXXXX.");
            }
            
            var bit24 = input[(i + 2)..(i + 8)];
            
            if (input[i + 1] == '+' && bit24.All(char.IsNumber))
            {
                AddUnicode(bit24, ref pairFirst, ref output);
                
                i += 8;
                continue;
            }

            throw new Exception("Invalid Unicode escape. Hint: Unicode escapes must be \\XXXX or \\+XXXXXX.");
        }

        return output;
    }

    /// <summary>
    /// Add unicode to the output.
    /// </summary>
    /// <param name="number">A string which represents number.</param>
    /// <param name="pairFirst">A unicode for surrogate check.</param>
    /// <param name="output">An output string.</param>
    /// <exception cref="Exception">Exception occurs when the pair is invalid.</exception>
    private static void AddUnicode(string number, ref int pairFirst, ref string output)
    {
        var unicode = int.Parse(number, System.Globalization.NumberStyles.HexNumber);

        if (pairFirst != 0)
        {
            if (is_utf16_surrogate_second(unicode))
            {
                unicode = surrogate_pair_to_codepoint(pairFirst, unicode);
            }
            else
            {
                Console.WriteLine("INVALID_PAIR");
                return;
                
                // throw new Exception("INVALID_PAIR");
            }
        }
        else if (is_utf16_surrogate_second(unicode))
        {
            Console.WriteLine("INVALID_PAIR");
            return;
            
            // throw new Exception("INVALID_PAIR");
        }
                
        if (is_utf16_surrogate_first(unicode))
        {
            pairFirst = unicode;
        }
        else
        {
            var result = UnicodeToString(unicode);
            output += result;
        }
    }

    /// <remark>
    /// See <a href="https://github.com/postgres/postgres/blob/27b77ecf9f4d5be211900eda54d8155ada50d696/src/include/mb/pg_wchar.h#L545">HERE</a> for more details.
    /// </remark>
    /// <returns></returns>
    private static bool is_utf16_surrogate_first(int number)
    {
        return number is >= 0xD800 and <= 0xDBFF;
    }
    
    /// <remark>
    /// See <a href="https://github.com/postgres/postgres/blob/27b77ecf9f4d5be211900eda54d8155ada50d696/src/include/mb/pg_wchar.h#L551">HERE</a> for more details.
    /// </remark>
    /// <returns></returns>
    private static bool is_utf16_surrogate_second(int number)
    {
        return number is >= 0xDC00 and <= 0xDFFF;
    }

    /// <remark>
    /// See <a href="https://github.com/postgres/postgres/blob/27b77ecf9f4d5be211900eda54d8155ada50d696/src/include/mb/pg_wchar.h#L557">HERE</a> for more details.
    /// </remark>
    /// <returns></returns>
    private static int surrogate_pair_to_codepoint(int first, int second)
    {
        return ((first & 0x3FF) << 10) + 0x10000 + (second & 0x3FF);
    }

    /// <summary>
    /// Convert a single Unicode code point into a string in the server encoding.
    /// </summary>
    /// <param name="unicode">A unicode to convert.</param>
    /// 
    /// <remarks>
    /// <para>
    /// See <a href="https://github.com/postgres/postgres/blob/27b77ecf9f4d5be211900eda54d8155ada50d696/src/backend/utils/mb/mbutils.c#L864">HERE</a> for more details.
    /// </para>
    /// </remarks>
    private static string UnicodeToString(int unicode)
    {
        var binary = BitConverter.GetBytes(unicode);
        var letter = Encoding.UTF32.GetString(binary);

        return letter;
    }
}
