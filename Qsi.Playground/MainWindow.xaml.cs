using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private readonly IBrush _terminalBrush = Brush.Parse("#F2AAAA");
        private readonly IBrush _elementBrush = Brush.Parse("#DDF3F5");

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

            _tbInput.KeyDown += TbInputOnKeyDown;
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

        private void TbInputOnKeyDown(object sender, KeyEventArgs e)
        {
            Flow.Debounce(sender, Update, 500);
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

                var tree = _qsiParser.Parse(script);
               BuildVisualTree(tree);
            }
            catch (Exception e)
            {
                _tbError.Text = e.ToString();
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
                Background = _elementBrush
            };

            var nodeItemChild = new List<TreeViewItem>();

            foreach (var property in node.GetType().GetTypeInfo().DeclaredProperties)
            {
                var item = new TreeViewItem
                {
                    Header = property.Name
                };

                nodeItemChild.Add(item);

                var value = property.GetValue(node);

                switch (value)
                {
                    case IQsiTreeNode childNode:
                        item.Items = new[] { BuildVisualTreeImpl(childNode) };
                        break;

                    case IEnumerable<IQsiTreeNode> childNodes:
                        item.Items = childNodes.Select(BuildVisualTreeImpl).ToArray();
                        break;

                    default:
                        item.Header = $"{property.Name}: {value} (terminal node)";
                        item.Background = _terminalBrush;
                        break;
                }
            }

            nodeItem.Items = nodeItemChild.ToArray();

            return nodeItem;
        }
        #endregion
    }
}
