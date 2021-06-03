using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Debugger.Controls;
using Qsi.Debugger.Models;
using Qsi.Debugger.Utilities;
using Qsi.Debugger.Vendor;
using Qsi.Debugger.Vendor.Cql;
using Qsi.Debugger.Vendor.JSql;
using Qsi.Debugger.Vendor.MySql;
using Qsi.Debugger.Vendor.Oracle;
using Qsi.Debugger.Vendor.PhoenixSql;
using Qsi.Debugger.Vendor.PostgreSql;
using Qsi.Debugger.Vendor.PrimarSql;
using Qsi.Debugger.Vendor.SqlServer;
using Qsi.SqlServer.Common;
using Qsi.Tree;

namespace Qsi.Debugger
{
    public class MainWindow : Window
    {
        private static readonly Assembly _qsiAssembly = typeof(IQsiTreeNode).Assembly;

        private readonly ComboBox _cbLanguages;
        private readonly TextEditor _codeEditor;
        private readonly TextBlock _tbError;
        private readonly CheckBox _chkQsiProperty;
        private readonly TextBlock _tbQsiStatus;
        private readonly TreeView _tvRaw;
        private readonly TreeView _tvQsi;
        private readonly TreeView _tvResult;
        private readonly TextBox _tbResult;

        private readonly Dictionary<string, Lazy<VendorDebugger>> _vendors;

        private VendorDebugger _vendor;
        private QsiScriptRenderer _scriptRenderer;

        public MainWindow()
        {
            InitializeComponent();

            _vendors = new Dictionary<string, Lazy<VendorDebugger>>
            {
                ["MySQL 5.6.2"] = new(() => new MySqlDebugger(new Version(5, 6, 2))),
                ["MySQL 5.7.13"] = new(() => new MySqlDebugger(new Version(5, 7, 13))),
                ["MySQL 8.0.22"] = new(() => new MySqlDebugger(new Version(8, 0, 22))),
                ["MySQL 8.0.22 (No Delimiter)"] = new(() => new MySqlDebugger(new Version(8, 0, 22), false)),
                ["PostgreSQL"] = new(() => new PostgreSqlDebugger()),
                ["JSqlParser"] = new(() => new JSqlDebugger()),
                ["Oracle"] = new(() => new OracleDebugger()),
                ["SQL Server 2000"] = new(() => new SqlServerDebugger(TransactSqlVersion.Version80)),
                ["SQL Server 2012"] = new(() => new SqlServerDebugger(TransactSqlVersion.Version110)),
                ["SQL Server 2017"] = new(() => new SqlServerDebugger(TransactSqlVersion.Version140)),
                ["SQL Server 2019"] = new(() => new SqlServerDebugger(TransactSqlVersion.Version150)),
                ["Phoenix 5.0.0"] = new(() => new PhoenixSqlDebugger()),
                ["CassandraQL 3"] = new(() => new CqlDebugger()),
                ["PrimarSql"] = new(() => new PrimarSqlDebugger())
            };

            _cbLanguages = this.Find<ComboBox>("cbLanguages");
            _codeEditor = this.Find<TextEditor>("codeEditor");
            _tbError = this.Find<TextBlock>("tbError");
            _chkQsiProperty = this.Find<CheckBox>("chkQsiProperty");
            _tbQsiStatus = this.Find<TextBlock>("tbQsiStatus");
            _tvRaw = this.Find<TreeView>("tvRaw");
            _tvQsi = this.Find<TreeView>("tvQsi");
            _tvResult = this.Find<TreeView>("tvResult");
            _tbResult = this.Find<TextBox>("tbResult");

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

            _scriptRenderer = new QsiScriptRenderer(_codeEditor.TextArea.TextView.CurrentLineBorder);
            _codeEditor.TextArea.TextView.BackgroundRenderers.Add(_scriptRenderer);

            _codeEditor.TextChanged += CodeEditorOnTextInput;
        }

        private void CbLanguagesOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_vendor != null)
            {
                var vendorRepositoryProvider = (VendorRepositoryProvider)_vendor.Engine.RepositoryProvider;
                vendorRepositoryProvider.DataTableRequested -= VendorRepositoryProviderOnDataTableRequested;
            }

            if (_vendors.TryGetValue(((string)_cbLanguages.SelectedItem)!, out Lazy<VendorDebugger> vendor))
            {
                _vendor = vendor.Value;

                var vendorRepositoryProvider = (VendorRepositoryProvider)_vendor.Engine.RepositoryProvider;
                vendorRepositoryProvider.DataTableRequested += VendorRepositoryProviderOnDataTableRequested;

                Update();
            }
            else
            {
                _vendor = null;
                _scriptRenderer.Update(null);
            }
        }

        private void VendorRepositoryProviderOnDataTableRequested(object sender, DataTableRequestEventArgs e)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(_tbResult.Text))
            {
                builder.AppendLine(_tbResult.Text);
                builder.AppendLine();
            }

            builder.AppendLine($"Type: {e.Script.ScriptType}");
            builder.AppendLine($"Script: {e.Script.Script}");

            if (e.Parameters?.Length > 0)
            {
                builder.AppendLine("Parameters:");

                foreach (var parameter in e.Parameters)
                    builder.AppendLine($" - {parameter.Name}({parameter.Type}): {parameter.Value ?? "<null>"}");
            }

            _tbResult.Text = builder.ToString();
        }

        private void CodeEditorOnTextInput(object sender, EventArgs e)
        {
            Flow.Debounce(_codeEditor, Update, 500);
        }

        private async void Update()
        {
            ClearError();
            ClearRawTree();
            ClearQsiTree();
            ClearResultTree();
            ClearResultLog();

            try
            {
                if (_vendor == null || string.IsNullOrWhiteSpace(_codeEditor.Text))
                    return;

                var input = _codeEditor.Text;

                QsiScript[] scripts = _vendor.Engine.ScriptParser.Parse(input, default).ToArray();
                _scriptRenderer.Update(scripts);

                var script = scripts.First(s => s.ScriptType != QsiScriptType.Comment && s.ScriptType != QsiScriptType.Delimiter);

                // Raw Tree

                _tvRaw.Items = new[] { _vendor.RawTreeParser.Parse(script.Script) };

                var sw = Stopwatch.StartNew();
                var tree = _vendor.Engine.TreeParser.Parse(script);
                sw.Stop();

                _tbQsiStatus.Text = $"parsed in {sw.Elapsed.TotalMilliseconds:0.0000} ms";

                BuildQsiTree(tree);

                // Execute

                var fakeParameters = new QsiParameter[] { new(default, default, default) };
                var result = await _vendor.Engine.Execute(script, fakeParameters);

                if (result is QsiTableAnalysisResult tableResult)
                {
                    BuildQsiTableTree(tableResult.Table);
                }
                else
                {
                    // throw new NotSupportedException(result.GetType().Name);
                }
            }
            catch (Exception e)
            {
                _tbError.Text = e.Message;
                _tbError.IsVisible = true;

                if (System.Diagnostics.Debugger.IsAttached)
                    ExceptionDispatchInfo.Throw(e);
            }
        }

        private void ClearResultLog()
        {
            _tbResult.Clear();
        }

        private void ClearError()
        {
            _tbError.Text = null;
            _tbError.IsVisible = false;
        }

        private void ClearRawTree()
        {
            _tvRaw.Items = null;
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
            var nodeType = node.GetType();
            bool hideProperty = _chkQsiProperty.IsChecked ?? false;

            var nodeItem = new QsiElementItem
            {
                Header = NormalizeElementName(nodeType.Name)
            };

            var nodeItemChild = new List<QsiTreeItem>();

            IEnumerable<PropertyInfo> properties = nodeType.GetInterfaces()
                .Where(t => t != typeof(IQsiTreeNode) && typeof(IQsiTreeNode).IsAssignableFrom(t))
                .SelectMany(t => t.GetProperties());

            if (nodeType.Assembly != _qsiAssembly)
            {
                IEnumerable<PropertyInfo> customProperties = nodeType.GetProperties()
                    .Where(pi => IsCustomProperty(nodeType, pi));

                properties = properties.Concat(customProperties);
            }

            foreach (var property in properties)
            {
                var value = property.GetValue(node);

                switch (value)
                {
                    case null:
                    case IQsiTreeNodeProperty<QsiTreeNode> { IsEmpty: true }:
                        continue;

                    case IQsiTreeNodeProperty<QsiTreeNode> nodeProperty:
                        value = nodeProperty.Value;
                        break;
                }

                if (value.GetType().IsValueType &&
                    value.Equals(Activator.CreateInstance(value.GetType())))
                {
                    continue;
                }

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

        private bool IsCustomProperty(Type type, PropertyInfo property)
        {
            if (type != property.DeclaringType)
                return false;

            var method = property.GetMethod;

            while (method != null)
            {
                if (method.DeclaringType!.Assembly == _qsiAssembly)
                {
                    return false;
                }

                var baseMethod = method.GetBaseDefinition();

                if (baseMethod == method)
                    break;

                method = baseMethod;
            }

            return true;
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
        private void BuildQsiTableTree(QsiTableStructure table)
        {
            _tvResult.Items = table.Columns
                .Select(c => new QsiColumnTreeItem(c))
                .ToArray();
        }
        #endregion
    }
}
