using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Qsi.Data;

namespace Qsi.Debugger.Converters
{
    public class PropertyValueConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Format(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private string Format(object value)
        {
            switch (value)
            {
                case string strValue:
                    return strValue;

                case QsiIdentifier identifier:
                    return identifier.ToString();

                case QsiQualifiedIdentifier qualifiedIdentifier:
                    return qualifiedIdentifier.ToString();

                case null:
                    return "null";

                case IEnumerable<object> enumerable:
                    return $"[{string.Join(", ", enumerable.Select(Format))}]";

                default:
                    return value.ToString();
            }
        }
    }
}
