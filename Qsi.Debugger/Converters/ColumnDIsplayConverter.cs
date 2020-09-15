using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Qsi.Data;
using Qsi.Debugger.Models;
using Qsi.Utilities;

namespace Qsi.Debugger.Converters
{
    public class ColumnDisplayConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is QsiColumnTreeItem item))
                return "<ERROR>";

            var name = item.Column.Name.IsEscaped ?
                IdentifierUtility.Unescape(item.Column.Name.Value) :
                item.Column.Name.Value;

            if (item.Column.IsExpression)
            {
                if (!item.Column.IsAnonymous)
                    return $"{name} {{expression}}";

                return "{expression}";
            }

            return name;
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
