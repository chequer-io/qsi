using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Qsi.Data;

namespace Qsi.Debugger.Converters
{
    public class ColumnBrushConverter : MarkupExtension, IValueConverter
    {
        private readonly Brush _ink1;
        private readonly Brush _skyblue;
        private readonly Brush _grey;
        private readonly Brush _orange;

        public ColumnBrushConverter()
        {
            _ink1 = (Brush)Application.Current.FindResource("Ink.1.Brush");
            _skyblue = (Brush)Application.Current.FindResource("Skyblue.Brush");
            _grey = (Brush)Application.Current.FindResource("Grey.Brush");
            _orange = (Brush)Application.Current.FindResource("Orange.Brush");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is QsiDataColumn column))
                return Brushes.Red;

            if (column.IsExpression)
                return _orange;

            if (column.IsAnonymous)
                return _grey;

            if (column.Parent.Type == QsiDataTableType.Table ||
                column.Parent.Type == QsiDataTableType.View ||
                column.Parent.Type == QsiDataTableType.MaterializedView)
            {
                return _skyblue;
            }

            return _ink1;
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
