using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Qsi.Debugger.Models;

namespace Qsi.Debugger.Converters
{
    public class ColumnParentTypeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not QsiColumnTreeItem item)
                return value;

            if (item.Column.ObjectReferences.Count > 0)
            {
                var objectReference = item.Column.ObjectReferences[0];
                return $"{objectReference.Type}: {objectReference.Identifier}";
            }

            var type = item.Column.Parent.Type.ToString().ToLower();

            if (item.Column.Parent.HasIdentifier)
                return $"{type}: {item.Column.Parent.Identifier}";

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
