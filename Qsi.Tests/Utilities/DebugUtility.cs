using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Kodnix.Character;
using Kodnix.Character.Extensions;
using Qsi.Data;

namespace Qsi.Tests.Utilities;

internal static class DebugUtility
{
    public static string Print(IEnumerable<QsiDataManipulationResult> results)
    {
        using var writer = new StringWriter();

        foreach (var action in results)
            Print(writer, action);

        return writer.ToString();
    }

    private static void Print(StringWriter writer, QsiDataManipulationResult result)
    {
        if (result.DeleteRows != null)
            Print(writer, result, result.DeleteRows, "DELETE");

        if (result.UpdateBeforeRows != null)
            Print(writer, result, result.UpdateBeforeRows, "UPDATE_BEFORE");

        if (result.UpdateAfterRows != null)
            Print(writer, result, result.UpdateAfterRows, "UPDATE_AFTER");

        if (result.InsertRows != null)
            Print(writer, result, result.InsertRows, "INSERT");

        if (result.DuplicateRows != null)
            Print(writer, result, result.DuplicateRows, "DUPLICATE");
    }

    public static void Print(StringWriter writer, QsiDataManipulationResult result, QsiDataRowCollection collection, string summary)
    {
        int rows = Math.Min(collection.Count, 20) + 1;
        QsiTableColumn[] visibleColumns = result.Table.VisibleColumns.ToArray();
        var table = new string[rows, visibleColumns.Length];

        for (int c = 0; c < visibleColumns.Length; c++)
        {
            table[0, c] = visibleColumns[c].Name.ToString();
        }

        for (int r = 1; r < rows; r++)
        {
            for (int c = 0; c < visibleColumns.Length; c++)
            {
                var item = collection[r - 1].Items[c];
                ref var cell = ref table[r, c];

                switch (item.Value)
                {
                    case DateTime dateTime:
                        cell = dateTime.ToString(DateTimeFormatInfo.InvariantInfo);
                        break;

                    case DateTimeOffset dateTimeOffset:
                        cell = dateTimeOffset.ToString(DateTimeFormatInfo.InvariantInfo);
                        break;

                    case DateOnly dateOnly:
                        cell = dateOnly.ToString(DateTimeFormatInfo.InvariantInfo);
                        break;

                    case TimeSpan timeSpan:
                        cell = timeSpan.ToString("G");
                        break;

                    default:
                        cell = item.Value?.ToString() ?? item.Type.ToString().ToLower();
                        break;
                }
            }
        }

        Print(writer, table, $"{result.Table.Identifier} - {summary}", true);
    }

    internal static void Print(StringWriter writer, string[,] table, string title, bool includeHeader = false, int maxColumnSize = 100)
    {
        int rows = table.GetLength(0);
        int columns = table.GetLength(1);

        var valueTable = new EastAsianString[rows, columns];
        var widths = new int[columns];

        for (int c = 0; c < columns; c++)
        {
            ref int width = ref widths[c];

            for (int r = 0; r < rows; r++)
            {
                ref var value = ref valueTable[r, c];

                value = table[r, c].ToEastAsianString();
                width = Math.Min(Math.Max(width, value.Length + 2), maxColumnSize);
            }
        }

        int actualWidth = widths.Sum() + columns - 1;
        var titleValue = title.ToEastAsianString();
        var titleWidth = titleValue.Length + 2;

        if (actualWidth < titleWidth)
        {
            int delta = titleWidth - actualWidth;
            int index = 0;

            do
            {
                widths[index]++;

                if (++index >= columns)
                    index = 0;
            } while (--delta > 0);

            actualWidth = titleWidth;
        }

        var line = $"+{string.Join("+", widths.Select(s => new string('-', s)))}+";

        writer.WriteLine();
        writer.WriteLine($"+{new string('-', actualWidth)}+");

        writer.Write('|');
        WriteCell(titleValue, actualWidth, true);
        writer.Write('|');

        writer.WriteLine();
        writer.WriteLine(line);

        for (int r = 0; r < rows; r++)
        {
            writer.Write('|');

            for (int c = 0; c < columns; c++)
            {
                int width = widths[c];
                var value = valueTable[r, c];

                if (value.Length + 2 > width)
                    value = value.Substring(0, width - 2);

                WriteCell(value, width, includeHeader && r == 0);

                writer.Write('|');
            }

            writer.WriteLine();

            if (includeHeader && r == 0)
                writer.WriteLine(line);
        }

        writer.WriteLine($"+{new string('-', actualWidth)}+");

        void WriteCell(EastAsianString value, int columnSize, bool center)
        {
            int leftPadding;
            int rightPadding;

            if (center)
            {
                leftPadding = (columnSize - value.Length) / 2;
                rightPadding = columnSize - value.Length - leftPadding;
            }
            else
            {
                leftPadding = 1;
                rightPadding = columnSize - 1 - value.Length;
            }

            if (leftPadding > 0)
                writer.Write(new string(' ', leftPadding));

            writer.Write(value);

            if (rightPadding > 0)
                writer.Write(new string(' ', rightPadding));
        }
    }
}
