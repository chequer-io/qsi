using System;
using System.Data;
using System.Data.Common;

namespace Qsi.Data
{
    public class QsiParameter
    {
        public QsiParameterType Type
        {
            get
            {
                if (_dbParameter == null)
                    return _type;

                return string.IsNullOrEmpty(_dbParameter.ParameterName) ?
                    QsiParameterType.Index :
                    QsiParameterType.Name;
            }
        }

        public string Name => _name ?? _dbParameter?.ParameterName;

        public object Value => _value ?? _dbParameter?.Value;

        private readonly QsiParameterType _type;
        private readonly string _name;
        private readonly object _value;

        private readonly DbParameter _dbParameter;

        public QsiParameter(QsiParameterType type, string name, object value)
        {
            _type = type;
            _name = name;
            _value = value;
        }

        public QsiParameter(DbParameter parameter)
        {
            _dbParameter = parameter;
        }

        public DbParameter ToDbParameter()
        {
            return _dbParameter ?? new QsiDbParameter(this);
        }
    }

    public sealed class QsiDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }

        public override ParameterDirection Direction { get; set; }

        public override bool IsNullable { get; set; }

        public override string ParameterName
        {
            get => _parameter.Name;
            set => throw new NotSupportedException();
        }

        public override string SourceColumn { get; set; }

        public override object Value
        {
            get => _parameter.Value;
            set => throw new NotSupportedException();
        }

        public override bool SourceColumnNullMapping { get; set; }

        public override int Size { get; set; }

        private readonly QsiParameter _parameter;

        internal QsiDbParameter(QsiParameter parameter)
        {
            _parameter = parameter;
        }

        public override void ResetDbType()
        {
            DbType = default;
            Direction = default;
            IsNullable = default;
            SourceColumn = default;
            SourceColumnNullMapping = default;
            Size = default;
        }
    }
}
