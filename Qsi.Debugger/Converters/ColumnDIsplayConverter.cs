using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Qsi.Data;
using Qsi.Debugger.Models;

namespace Qsi.Debugger.Converters
{
    public class ColumnDisplayConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is QsiColumnTreeItem item))
                return "<ERROR>";

            if (item.Column.IsExpression)
            {
                if (!item.Column.IsAnonymous)
                    return $"{item.Column.Name.Value} {{expression}}";

                return "{expression}";
            }

            return item.Column.Name.Value;
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
