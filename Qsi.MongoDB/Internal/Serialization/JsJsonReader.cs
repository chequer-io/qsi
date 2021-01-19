using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Qsi.MongoDB.Internal.Serialization
{
    public class JsJsonReader : JsonReader
    {
        public override char QuoteChar => _reader.QuoteChar;

        public override JsonToken TokenType => _read ? _reader.TokenType : JsonToken.StartObject;
        
        public override object Value => _read ? _reader.Value : null;
        
        public override Type ValueType => _read ? _reader.ValueType : null; 
        
        public override int Depth => _reader.Depth;
        
        public override string Path =>_reader.Path;
        
        private readonly JsonReader _reader;
        private bool _read;

        public JsJsonReader(JsonReader reader)
        {
            _reader = reader;
        }

        public override bool Read()
        {
            if (!_read)
            {
                _read = true;
                return true;
            }

            return _reader.Read();
        }

        public override async Task<bool> ReadAsync(CancellationToken cancellationToken = new())
        {
            if (!_read)
            {
                _read = true;
                return true;
            }

            return await _reader.ReadAsync(cancellationToken);
        }

        public override async Task<bool?> ReadAsBooleanAsync(CancellationToken cancellationToken = default)
        {
            return _read ? await _reader.ReadAsBooleanAsync(cancellationToken) : null;
        }

        public override async Task<byte[]> ReadAsBytesAsync(CancellationToken cancellationToken = default)
        {
            return _read ? await _reader.ReadAsBytesAsync(cancellationToken) : null;
        }

        public override async Task<DateTime?> ReadAsDateTimeAsync(CancellationToken cancellationToken = default)
        {
            return _read ? await _reader.ReadAsDateTimeAsync(cancellationToken) : null;
        }

        public override async Task<DateTimeOffset?> ReadAsDateTimeOffsetAsync(CancellationToken cancellationToken = default)
        {
            return _read ? await _reader.ReadAsDateTimeOffsetAsync(cancellationToken) : null;
        }

        public override async Task<decimal?> ReadAsDecimalAsync(CancellationToken cancellationToken = default)
        {
            return _read ? await _reader.ReadAsDecimalAsync(cancellationToken) : null;
        }

        public override async Task<double?> ReadAsDoubleAsync(CancellationToken cancellationToken = default)
        {
            return _read ? await _reader.ReadAsDoubleAsync(cancellationToken) : null;
        }

        public override async Task<int?> ReadAsInt32Async(CancellationToken cancellationToken = default)
        {
            return _read ? await _reader.ReadAsInt32Async(cancellationToken) : null;
        }

        public override async Task<string> ReadAsStringAsync(CancellationToken cancellationToken = default)
        {
            return _read ? await _reader.ReadAsStringAsync(cancellationToken) : null;
        }

        public override int? ReadAsInt32()
        {
            return _read ? _reader.ReadAsInt32() : null;
        }

        public override string ReadAsString()
        {
            return _read ? _reader.ReadAsString() : null;
        }

        public override byte[] ReadAsBytes()
        {
            return _read ? _reader.ReadAsBytes() : null;
        }

        public override double? ReadAsDouble()
        {
            return _read ? _reader.ReadAsDouble() : null;
        }

        public override bool? ReadAsBoolean()
        {
            return _read ? _reader.ReadAsBoolean() : null;
        }

        public override decimal? ReadAsDecimal()
        {
            return _read ? _reader.ReadAsDecimal() : null;
        }

        public override DateTime? ReadAsDateTime()
        {
            return _read ? _reader.ReadAsDateTime() : null;
        }

        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            return _read ? _reader.ReadAsDateTimeOffset() : null;
        }

        public override void Close()
        {
            _reader.Close();
        }
    }
}