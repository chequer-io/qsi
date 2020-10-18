using System;
using System.Collections.Generic;
using System.Text;
using Qsi.Tree;

namespace Qsi.MongoDB.Analyzers
{
    public sealed class JsObjectInfo
    {
        public JsObjectType ObjectType { get; set; }
        
        public string DatabaseName { get; set; }
        
        public string CollectionName { get; set; }
        
        public bool IsDatabase => !string.IsNullOrEmpty(DatabaseName) && string.IsNullOrEmpty(CollectionName) && Columns == null;
            
        public bool IsCollection => !string.IsNullOrEmpty(DatabaseName) && !string.IsNullOrEmpty(CollectionName) && Columns == null;
        
        public bool IsView { get; set; }
        
        // if object is function
        public string ReturnType { get; set; }
        
        // if find() function filter columns
        public IQsiColumnNode[] Columns { get; set; }

        // db.inventory.find().map(c => {test: c.column1})
        // column1, test
        public (string, string)[] Mapping { get; set; }
        
        public string LastExpression { get; private set; }
        
        public StringBuilder ExpressionStringBuilder { get; } = new StringBuilder();

        public string Expression => ExpressionStringBuilder.ToString();
        
        public void ClearExpression()
        {
            ExpressionStringBuilder.Clear();
        }
        
        public void AppendExpression(string expression)
        {
            if (ExpressionStringBuilder.Length == 0)
                ExpressionStringBuilder.Append(expression);
            else
                ExpressionStringBuilder.Append($".{expression}");

            LastExpression = expression;
        }

        public void AppendParameter(string parameter)
        {
            if (ExpressionStringBuilder.Length == 0)
                throw new InvalidOperationException("No expression");

            ExpressionStringBuilder.Append($"({parameter})");
        }

        public void AppendParameters(IEnumerable<string> parameters)
        {
            if (ExpressionStringBuilder.Length == 0)
                throw new InvalidOperationException("No expression");

            ExpressionStringBuilder.Append($"({string.Join(", ", parameters)})");
        }
    }
    
    public enum JsObjectType
    {
        Database,
        Collection,
        Cursor,
        Function,
        String,
        Integer,
        // TODO: Add Types
    }
}
