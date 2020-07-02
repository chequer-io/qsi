using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using Qsi.Data;
using Qsi.MySql;
using Qsi.Parsing;
using Qsi.Playground.Models;
using Qsi.Playground.Utilities;
using Qsi.Tree;

namespace Qsi.Playground
{
    public class MainWindow : Window
    {
        private readonly ComboBox _cbLanguages;
        private readonly TextEditor _codeEditor;
        private readonly TextBlock _tbError;
        private readonly CheckBox _chkQsiProperty;
        private readonly TextBlock _tbQsiStatus;
        private readonly TreeView _tvQsi;

        private readonly IBrush _terminalBrush = Brush.Parse("#FA8072");
        private readonly IBrush _propertyBrush = Brush.Parse("#B0B0B0");

        private readonly Dictionary<string, Lazy<IQsiParser>> _parsers;

        private IQsiParser _qsiParser;

        public MainWindow()
        {
            InitializeComponent();

            this.OpenDevTools();
            _parsers = new Dictionary<string, Lazy<IQsiParser>>
            {
                ["MySQL"] = new Lazy<IQsiParser>(() => new MySqlParser())
            };

            _cbLanguages = this.Find<ComboBox>("cbLanguages");
            _codeEditor = this.Find<TextEditor>("codeEditor");
            _tbError = this.Find<TextBlock>("tbError");
            _chkQsiProperty = this.Find<CheckBox>("chkQsiProperty");
            _tbQsiStatus = this.Find<TextBlock>("tbQsiStatus");
            _tvQsi = this.Find<TreeView>("tvQsi");

            InitializeEditor();

            _cbLanguages.SelectionChanged += CbLanguagesOnSelectionChanged;
            _cbLanguages.Items = _parsers.Keys;
            _cbLanguages.SelectedIndex = 0;

            _chkQsiProperty.GetObservable(ToggleButton.IsCheckedProperty).Subscribe(_ => Update());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InitializeEditor()
        {
            using var reader = XmlReader.Create(AssetManager.FindResource("Sql.xshd"));
            var xshd = HighlightingLoader.LoadXshd(reader);
            var highlighting = HighlightingLoader.Load(xshd, new HighlightingManager());

            _codeEditor.ShowLineNumbers = true;
            _codeEditor.SyntaxHighlighting = highlighting;
            _codeEditor.TextArea.SelectionBrush = Brush.Parse("#6623577B");
            _codeEditor.TextArea.Options.ShowColumnRuler = true;
            _codeEditor.TextArea.Options.ColumnRulerPosition = 100;
            _codeEditor.TextArea.TextView.CurrentLineBackground = Brushes.Transparent;
            _codeEditor.TextArea.TextView.CurrentLineBorder = new Pen(Brush.Parse("#1BFFFFFF"));
            _codeEditor.TextArea.Options.HighlightCurrentLine = true;

            _codeEditor.TextChanged += CodeEditorOnTextInput;
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

        private void CodeEditorOnTextInput(object sender, EventArgs e)
        {
            Flow.Debounce(_codeEditor, Update, 500);
        }

        private void Update()
        {
            ClearError();
            ClearVisualTree();

            try
            {
                if (_qsiParser == null || string.IsNullOrWhiteSpace(_codeEditor.Text))
                    return;

                var input = _codeEditor.Text;

                // TODO: change to IQsiScriptParser
                var sentence = input.Split(';', 2)[0].Trim();
                var script = new QsiScript(sentence, QsiScriptType.Select);

                _qsiParser.SyntaxError += ErrorHandler;

                var sw = Stopwatch.StartNew();
                var tree = _qsiParser.Parse(script);
                sw.Stop();

                _qsiParser.SyntaxError -= ErrorHandler;

                _tbQsiStatus.Text = $"parsed in {sw.Elapsed.TotalMilliseconds:0.0000} ms";

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

        private QsiTreeItem BuildVisualTreeImpl(IQsiTreeNode node)
        {
            bool hideProperty = _chkQsiProperty.IsChecked ?? false;

            var nodeItem = new QsiElementItem
            {
                Header = NormalizeElementName(node.GetType().Name)
            };

            var nodeItemChild = new List<QsiTreeItem>();

            IEnumerable<PropertyInfo> properties = node.GetType().GetInterfaces()
                .Where(t => t != typeof(IQsiTreeNode))
                .SelectMany(t => t.GetProperties());

            foreach (var property in properties)
            {
                var value = property.GetValue(node);

                if (value == null || value is bool || value is int)
                    continue;

                QsiTreeItem item;

                switch (value)
                {
                    case IQsiTreeNode childNode:
                        var childItem = BuildVisualTreeImpl(childNode);

                        if (hideProperty)
                        {
                            nodeItemChild.Add(childItem);
                            continue;
                        }

                        item = new QsiChildElementItem
                        {
                            Header = property.Name,
                            Items = new[] { childItem }
                        };

                        break;

                    case IEnumerable<IQsiTreeNode> childNodes:
                        QsiTreeItem[] childItems = childNodes.Select(BuildVisualTreeImpl).ToArray();

                        if (hideProperty)
                        {
                            nodeItemChild.AddRange(childItems);
                            continue;
                        }

                        item = new QsiChildElementItem
                        {
                            Header = property.Name,
                            Items = childItems
                        };

                        break;

                    default:
                        item = new QsiPropertyItem
                        {
                            Header = property.Name,
                            Value = value
                        };

                        break;
                }

                nodeItemChild.Add(item);
            }

            nodeItem.Items = nodeItemChild.ToArray();

            return nodeItem;
        }

        private string NormalizeElementName(string name)
        {
            if (name.StartsWith("Qsi"))
                name = name[3..];

            if (name.EndsWith("Node"))
                name = name[..^4];

            return name;
        }
        #endregion
    }
}
