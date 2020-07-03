using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaEdit
{
    public class RoutedCommand : ICommand
    {
        static RoutedCommand()
        {
            CanExecuteEvent.AddClassHandler<IRoutedCommandBindable>(CanExecuteEventHandler);
            ExecutedEvent.AddClassHandler<IRoutedCommandBindable>(ExecutedEventHandler);
        }

        public RoutedCommand(string name, KeyGesture keyGesture = null)
        {
            Name = name;
            Gesture = keyGesture;
        }

        public string Name { get; }

        public KeyGesture Gesture { get; }

        public static RoutedEvent<CanExecuteRoutedEventArgs> CanExecuteEvent { get; } = RoutedEvent.Register<CanExecuteRoutedEventArgs>(nameof(CanExecuteEvent), RoutingStrategies.Bubble, typeof(RoutedCommand));

        public static RoutedEvent<ExecutedRoutedEventArgs> ExecutedEvent { get; } = RoutedEvent.Register<ExecutedRoutedEventArgs>(nameof(ExecutedEvent), RoutingStrategies.Bubble, typeof(RoutedCommand));

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter, Application.Current.FocusManager.Current);
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter, Application.Current.FocusManager.Current);
        }

        // TODO
        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        private static void CanExecuteEventHandler(IRoutedCommandBindable control, CanExecuteRoutedEventArgs e)
        {
            var binding = control.CommandBindings.Where(c => c != null)
                .FirstOrDefault(c => c.Command == e.Command && c.DoCanExecute(control, e));

            e.CanExecute = binding != null;
        }

        private static void ExecutedEventHandler(IRoutedCommandBindable control, ExecutedRoutedEventArgs e)
        {
            // ReSharper disable once UnusedVariable
            var binding = control.CommandBindings.Where(c => c != null)
                .FirstOrDefault(c => c.Command == e.Command && c.DoExecuted(control, e));
        }

        public bool CanExecute(object parameter, IInputElement target)
        {
            if (target == null) return false;

            var args = new CanExecuteRoutedEventArgs(this, parameter);
            target.RaiseEvent(args);

            return args.CanExecute;
        }

        public void Execute(object parameter, IInputElement target)
        {
            if (target == null) return;

            var args = new ExecutedRoutedEventArgs(this, parameter);
            target.RaiseEvent(args);
        }
    }

    public interface IRoutedCommandBindable : IInteractive
    {
        IList<RoutedCommandBinding> CommandBindings { get; }
    }

    public class RoutedCommandBinding
    {
        public RoutedCommandBinding(RoutedCommand command,
                                    EventHandler<ExecutedRoutedEventArgs> executed = null,
                                    EventHandler<CanExecuteRoutedEventArgs> canExecute = null)
        {
            Command = command;
            if (executed != null) Executed += executed;
            if (canExecute != null) CanExecute += canExecute;
        }

        public RoutedCommand Command { get; }

        public event EventHandler<CanExecuteRoutedEventArgs> CanExecute;

        public event EventHandler<ExecutedRoutedEventArgs> Executed;

        internal bool DoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Handled) return true;

            EventHandler<CanExecuteRoutedEventArgs> canExecute = CanExecute;

            if (canExecute == null)
            {
                if (Executed != null)
                {
                    e.Handled = true;
                    e.CanExecute = true;
                }
            }
            else
            {
                canExecute(sender, e);

                if (e.CanExecute)
                    e.Handled = true;
            }

            return e.CanExecute;
        }

        internal bool DoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                EventHandler<ExecutedRoutedEventArgs> executed = Executed;

                if (executed != null)
                    if (DoCanExecute(sender, new CanExecuteRoutedEventArgs(e.Command, e.Parameter)))
                    {
                        executed(sender, e);
                        e.Handled = true;
                        return true;
                    }
            }

            return false;
        }
    }

    public sealed class CanExecuteRoutedEventArgs : RoutedEventArgs
    {
        internal CanExecuteRoutedEventArgs(ICommand command, object parameter)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Parameter = parameter;
            RoutedEvent = RoutedCommand.CanExecuteEvent;
        }

        public ICommand Command { get; }

        public object Parameter { get; }

        public bool CanExecute { get; set; }
    }

    public sealed class ExecutedRoutedEventArgs : RoutedEventArgs
    {
        internal ExecutedRoutedEventArgs(ICommand command, object parameter)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Parameter = parameter;
            RoutedEvent = RoutedCommand.ExecutedEvent;
        }

        public ICommand Command { get; }

        public object Parameter { get; }
    }
}
