using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Qsi.SqlServer.Diagnostics;

internal sealed partial class SqlServerRawTreeVisitor : TSqlFragmentVisitor
{
    private delegate void VisitorDelegate<in T>(T fragment);

    public SqlServerRawTree Root { get; set; }

    private readonly Stack<SqlServerRawTree> _stack;

    private SqlServerRawTreeVisitor()
    {
        _stack = new Stack<SqlServerRawTree>();
    }

    private void MakeTree<T>(T fragment, VisitorDelegate<T> visitor) where T : TSqlFragment
    {
        var current = new SqlServerRawTree(typeof(T).Name);

        if (_stack.TryPeek(out var parent))
        {
            parent.AddChild(current);
        }
        else if (Root == null)
        {
            Root = current;
        }
        else
        {
            throw new InvalidOperationException();
        }

        _stack.Push(current);
        visitor(fragment);
        _stack.Pop();

        if (current.ChildrenCount == 0 && fragment.FragmentLength != -1)
        {
            var builder = new StringBuilder();

            for (int i = fragment.FirstTokenIndex; i <= fragment.LastTokenIndex; i++)
            {
                builder.Append(fragment.ScriptTokenStream[i].Text);
            }

            current.AddChild(new SqlServerRawTreeTerminalNode(builder.ToString()));
        }

        current.Freeze();
    }

    public static SqlServerRawTree CreateRawTree(TSqlFragment fragment)
    {
        var visitor = new SqlServerRawTreeVisitor();
        fragment.Accept(visitor);

        return visitor.Root ?? throw new InvalidOperationException();
    }
}
