﻿namespace Qsi.SqlServer.Tree
{
    public abstract class VisitorBase
    {
        protected TableVisitor TableVisitor => _context.TableVisitor;

        protected ExpressionVisitor ExpressionVisitor => _context.ExpressionVisitor;

        protected IdentifierVisitor IdentifierVisitor => _context.IdentifierVisitor;

        protected SqlServerParser SqlParser => _context.SqlParser;
        
        private readonly IContext _context;

        protected VisitorBase(IContext context)
        {
            _context = context;
        }
    }
}
