using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Qsi.Data;

namespace Qsi.Debugger.Converters
{
    public class ColumnParentTypeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is QsiDataColumn column))
                return value;

            var type = column.Parent.Type.ToString().ToLower();

            if (column.Parent.HasIdentifier)
                return $"{type}: {column.Parent.Identifier}";
            
            return type;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
