using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tests.Drivers
{
    public static class QsiTableStructureHelper
    {
        // table(x): 'x'
        // join(x, y): 'x, y'
        // alias(x, y): 'x { y }'
        // union(x, y): 'x + y'
        // except(x, y): 'x - y'
        // intersect(x, y): 'x ∩ y'
        // inline: inline
        public static string GetPseudoName(QsiTableStructure tableStructure)
        {
            var set = new HashSet<QsiTableStructure>();
            return GetPseudoTableNameCore(tableStructure, 0, set);

            static string GetPseudoTableNameCore(QsiTableStructure tableStructure, int depth, HashSet<QsiTableStructure> set)
            {
                if (set.Contains(tableStructure))
                    return "<recusive>";

                set.Add(tableStructure);

                switch (tableStructure.Type)
                {
                    case QsiTableType.Inline:
                        return "inline";

                    case QsiTableType.Table:
                    case QsiTableType.View:
                    case QsiTableType.MaterializedView:
                        return tableStructure.HasIdentifier
                            ? tableStructure.Identifier.ToString()
                            : $"<unknown_{tableStructure.Type.ToString().ToLowerInvariant()}>";

                    case QsiTableType.Derived when tableStructure.HasIdentifier:
                    {
                        var elements = tableStructure.References
                            .Distinct()
                            .Select(x => GetPseudoTableNameCore(x, depth + 1, set))
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToArray();

                        return $"{tableStructure.Identifier} {{ {string.Join(", ", elements)} }}";
                    }

                    case QsiTableType.Derived:
                    {
                        var elements = tableStructure.References
                            .Distinct()
                            .Select(x => GetPseudoTableNameCore(x, depth + 1, set))
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToArray();

                        return elements.Length switch
                        {
                            0 => "<no tables>",
                            1 => elements[0],
                            _ => $"[{string.Join(", ", elements)}]",
                        };
                    }

                    case QsiTableType.Join
                        or QsiTableType.Union
                        or QsiTableType.Except
                        or QsiTableType.Intersect:
                    {
                        var elements = tableStructure.References
                            .Distinct()
                            .Select(x => GetPseudoTableNameCore(x, depth + 1, set))
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToArray();

                        if (elements.Length < 2)
                            return "?";

                        var op = tableStructure.Type switch
                        {
                            QsiTableType.Join => ", ",
                            QsiTableType.Union => " + ",
                            QsiTableType.Except => " - ",
                            QsiTableType.Intersect => " ∩ ",
                            _ => throw new InvalidOperationException()
                        };

                        var result = string.Join(op, elements);

                        return depth > 0 ? $"({result})" : result;
                    }
                }

                return null;
            }
        }
    }
}
