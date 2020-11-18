using System;
using Qsi.Shared;
using Qsi.Tree.Data;

namespace Qsi.Cassandra.Tree
{
    internal static class CqlTree
    {
        public static KeyIndexer<Range> Span { get; }

        static CqlTree()
        {
            Span = new KeyIndexer<Range>(new Key<Range>("span"));
        }
    }
}
