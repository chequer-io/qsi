using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Qsi.Data;
using Qsi.Debugger.Models;

namespace Qsi.Debugger.Converters
{
    public class ColumnBrushConverter : MarkupExtension, IValueConverter
    {
        private static readonly Brush _ink1;
        private static readonly Brush _red;
        private static readonly Brush _skyblue;
        private static readonly Brush _grey;
        private static readonly Brush _orange;
        private static readonly Brush _yellow;
        private static readonly Brush _yellowgreen;

        static ColumnBrushConverter()
        {
            _ink1 = (Brush)Application.Current.FindResource("Ink.1.Brush");
            _skyblue = (Brush)Application.Current.FindResource("Skyblue.Brush");
            _red = (Brush)Application.Current.FindResource("Red.Brush");
            _grey = (Brush)Application.Current.FindResource("Grey.Brush");
            _orange = (Brush)Application.Current.FindResource("Orange.Brush");
            _yellow = (Brush)Application.Current.FindResource("Yellow.Brush");
            _yellowgreen = (Brush)Application.Current.FindResource("Yellowgreen.Brush");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not QsiColumnTreeItem item)
                return Brushes.Red;

            if (item.Depth == 0)
                return _yellowgreen;

            if (item.Column.IsExpression)
                return _yellow;

            if (item.Column.IsAnonymous)
                return _grey;

            if (item.Column.Parent.Type is QsiTableType.Table or QsiTableType.View or QsiTableType.MaterializedView)
                return _red;

            return _grey;
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
