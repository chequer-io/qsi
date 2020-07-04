using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Qsi.Data;

namespace Qsi.Debugger.Converters
{
    public class ColumnDisplayConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is QsiDataColumn column))
                return "<ERROR>";

            if (column.IsAnonymous)
            {
                if (column.IsExpression)
                    return "{anonymous, expression}";
                
                return "{anonymous}";
            }

            return column.Name.Value;
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
