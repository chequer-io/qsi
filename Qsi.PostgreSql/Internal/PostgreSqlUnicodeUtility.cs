using System;
using System.Linq;
using System.Text;

namespace Qsi.PostgreSql.Internal;

internal static class PostgreSqlUnicodeUtility
{
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
    public static string ProcessUnicodeEscape(string input, char escape, int position = 0)
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
                continue;
            }
            
            var bit16 = input[(i + 1)..(i + 5)];

            if (bit16.All(char.IsNumber))
            {
                AddUnicode(bit16, ref pairFirst, ref output);

                i += 5;
                continue;
            }

            // If the next letter is plus, and also if next belonging 6 letters are numbers, insert a unicode(3 bytes).
            if (i + 7 > input.Length)
            {
                continue;
            }
            
            var bit24 = input[(i + 2)..(i + 8)];
            
            if (input[i + 1] == '+' && bit24.All(char.IsNumber))
            {
                AddUnicode(bit24, ref pairFirst, ref output);
                
                i += 8;
            }
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
                throw new Exception("INVALID_PAIR");
            }
        }
        else if (is_utf16_surrogate_second(unicode))
        {
            throw new Exception("INVALID_PAIR");
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
