using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Qsi.Data;
using Qsi.MySql;
using Qsi.Parsing;
using Qsi.Playground.Utilities;
using Qsi.Tree;

namespace Qsi.Playground
{
    public class MainWindow : Window
    {
        private readonly ComboBox _cbLanguages;
        private readonly TextBox _tbInput;
        private readonly TextBlock _tbError;
        private readonly TreeView _tvQsi;

        private readonly IBrush _terminalBrush = Brush.Parse("#c70039");
        private readonly IBrush _propertyBrush = Brush.Parse("#cf7500");

        private readonly Dictionary<string, Lazy<IQsiParser>> _parsers;

        private IQsiParser _qsiParser;

        public MainWindow()
        {
            InitializeComponent();

            _parsers = new Dictionary<string, Lazy<IQsiParser>>
            {
                ["MySQL"] = new Lazy<IQsiParser>(() => new MySqlParser())
            };

            _cbLanguages = this.Find<ComboBox>("cbLanguages");
            _tbInput = this.Find<TextBox>("tbInput");
            _tbError = this.Find<TextBlock>("tbError");
            _tvQsi = this.Find<TreeView>("tvQsi");

            _cbLanguages.SelectionChanged += CbLanguagesOnSelectionChanged;
            _cbLanguages.Items = _parsers.Keys;
            _cbLanguages.SelectedIndex = 0;

            _tbInput.GetObservable(TextBox.TextProperty).Subscribe(_ => OnInputChanged());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CbLanguagesOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_parsers.TryGetValue((string)_cbLanguages.SelectedItem, out Lazy<IQsiParser> parser))
            {
                _qsiParser = parser.Value;
                Update();
            }
            else
            {
                _qsiParser = null;
            }
        }

        private void OnInputChanged()
        {
            Flow.Debounce(_tbInput, Update, 500);
        }

        private void Update()
        {
            ClearError();
            ClearVisualTree();

            try
            {
                if (_qsiParser == null || string.IsNullOrWhiteSpace(_tbInput.Text))
                    return;

                var input = _tbInput.Text;

                // TODO: change to IQsiScriptParser
                var sentence = input.Split(';', 2)[0].Trim();
                var script = new QsiScript(sentence, QsiScriptType.Select);

                _qsiParser.SyntaxError += ErrorHandler;
                var tree = _qsiParser.Parse(script);
                _qsiParser.SyntaxError -= ErrorHandler;

                BuildVisualTree(tree);
            }
            catch (Exception e)
            {
                _tbError.Text = e.Message;
            }

            static void ErrorHandler(object sender, QsiSyntaxErrorException e)
            {
                throw e;
            }
        }

        private void ClearError()
        {
            _tbError.Text = null;
        }

        private void ClearVisualTree()
        {
            _tvQsi.Items = null;
        }

        #region Qsi TreeView
        private void BuildVisualTree(IQsiTreeNode node)
        {
            _tvQsi.Items = new[] { BuildVisualTreeImpl(node) };
        }

        private TreeViewItem BuildVisualTreeImpl(IQsiTreeNode node)
        {
            var nodeItem = new TreeViewItem
            {
                Header = $"{node.GetType().Name}",
                Foreground = Brushes.Black,
                IsExpanded = true
            };

            var nodeItemChild = new List<TreeViewItem>();

            IEnumerable<PropertyInfo> properties = node.GetType().GetInterfaces()
                .Where(t => t != typeof(IQsiTreeNode))
                .SelectMany(t => t.GetProperties());

            foreach (var property in properties)
            {
                var value = property.GetValue(node);

                if (value == null || value is bool || value is int)
                    continue;

                var item = new TreeViewItem
                {
                    Header = property.Name,
                    Foreground = _propertyBrush,
                    IsExpanded = true
                };

                nodeItemChild.Add(item);

                switch (value)
                {
                    case IQsiTreeNode childNode:
                        item.Items = new[] { BuildVisualTreeImpl(childNode) };
                        break;

                    case IEnumerable<IQsiTreeNode> childNodes:
                        item.Items = childNodes.Select(BuildVisualTreeImpl).ToArray();
                        break;

                    default:
                        item.Header = $"{property.Name}: {value}";
                        item.Foreground = _terminalBrush;
                        break;
                }
            }

            nodeItem.Items = nodeItemChild.ToArray();

            return nodeItem;
        }
        #endregion
    }
}
