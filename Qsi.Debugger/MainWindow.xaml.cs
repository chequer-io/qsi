using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using Qsi.Compiler;
using Qsi.Data;
using Qsi.Debugger.Models;
using Qsi.Debugger.Utilities;
using Qsi.Debugger.Vendor;
using Qsi.Debugger.Vendor.MySql;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.Debugger
{
    public class MainWindow : Window
    {
        private readonly ComboBox _cbLanguages;
        private readonly TextEditor _codeEditor;
        private readonly TextBlock _tbError;
        private readonly CheckBox _chkQsiProperty;
        private readonly TextBlock _tbQsiStatus;
        private readonly TreeView _tvRaw;
        private readonly TreeView _tvQsi;
        private readonly TreeView _tvResult;

        private readonly Dictionary<string, Lazy<VendorDebugger>> _vendors;

        private VendorDebugger _vendor;

        public MainWindow()
        {
            InitializeComponent();

            this.AttachDevTools();

            _vendors = new Dictionary<string, Lazy<VendorDebugger>>
            {
                ["MySQL"] = new Lazy<VendorDebugger>(() => new MySqlDebugger())
            };

            _cbLanguages = this.Find<ComboBox>("cbLanguages");
            _codeEditor = this.Find<TextEditor>("codeEditor");
            _tbError = this.Find<TextBlock>("tbError");
            _chkQsiProperty = this.Find<CheckBox>("chkQsiProperty");
            _tbQsiStatus = this.Find<TextBlock>("tbQsiStatus");
            _tvRaw = this.Find<TreeView>("tvRaw");
            _tvQsi = this.Find<TreeView>("tvQsi");
            _tvResult = this.Find<TreeView>("tvResult");

            InitializeEditor();

            _cbLanguages.SelectionChanged += CbLanguagesOnSelectionChanged;
            _cbLanguages.Items = _vendors.Keys;
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
            if (_vendors.TryGetValue((string)_cbLanguages.SelectedItem, out Lazy<VendorDebugger> vendor))
            {
                _vendor = vendor.Value;
                Update();
            }
            else
            {
                _vendor = null;
            }
        }

        private void CodeEditorOnTextInput(object sender, EventArgs e)
        {
            Flow.Debounce(_codeEditor, Update, 500);
        }

        private async void Update()
        {
            ClearError();
            ClearQsiTree();
            ClearResultTree();

            try
            {
                if (_vendor == null || string.IsNullOrWhiteSpace(_codeEditor.Text))
                    return;

                var input = _codeEditor.Text;

                // Raw Tree

                _tvRaw.Items = new[] { _vendor.RawParser.Parse(input) };

                // TODO: change to IQsiScriptParser
                var sentence = input.Split(';', 2)[0].Trim();
                var script = new QsiScript(sentence, QsiScriptType.Select);

                _vendor.Parser.SyntaxError += ErrorHandler;

                var sw = Stopwatch.StartNew();
                var tree = _vendor.Parser.Parse(script);
                sw.Stop();

                _vendor.Parser.SyntaxError -= ErrorHandler;

                _tbQsiStatus.Text = $"parsed in {sw.Elapsed.TotalMilliseconds:0.0000} ms";

                BuildQsiTree(tree);

                // Execute

                var compiler = new QsiTableCompiler(_vendor.LanguageService);
                var result = await compiler.ExecuteAsync((IQsiTableNode)tree);

                if (result.Exceptions?.Length > 0)
                {
                    if (result.Exceptions.Length == 1)
                        throw result.Exceptions[0];

                    throw new AggregateException(result.Exceptions);
                }

                BuildQsiTableTree(result.Table);
            }
            catch (Exception e)
            {
                _tbError.Text = e.Message;
                _tbError.IsVisible = true;
            }

            static void ErrorHandler(object sender, QsiSyntaxErrorException e)
            {
                throw e;
            }
        }

        private void ClearError()
        {
            _tbError.Text = null;
            _tbError.IsVisible = false;
        }

        private void ClearQsiTree()
        {
            _tvQsi.Items = null;
        }

        private void ClearResultTree()
        {
            _tvResult.Items = null;
        }

        #region Qsi TreeView
        private void BuildQsiTree(IQsiTreeNode node)
        {
            _tvQsi.Items = new[] { BuildQsiTreeImpl(node) };
        }

        private QsiTreeItem BuildQsiTreeImpl(IQsiTreeNode node)
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
                        var childItem = BuildQsiTreeImpl(childNode);

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
                        QsiTreeItem[] childItems = childNodes.Select(BuildQsiTreeImpl).ToArray();

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

        #region Qsi Table TreeView
        private void BuildQsiTableTree(QsiDataTable table)
        {
            _tvResult.Items = table.Columns
                .Select(c => new QsiColumnTreeItem(c))
                .ToArray();
        }
        #endregion
    }
}