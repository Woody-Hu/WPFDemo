using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDemo
{
    public class ControlDoubleClick : DependencyObject
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ControlDoubleClick), new PropertyMetadata(OnCommandChanged));

        public static ICommand GetCommand(Control target)
        {
            return (ICommand)target.GetValue(CommandProperty);
        }

        public static void SetCommand(Control target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        private static void Element_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Control control = sender as Control;
            ICommand command = GetCommand(control);

            if (command.CanExecute(null))
            {
                command.Execute(control.Tag);
                e.Handled = true;
            }
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = d as Control;
            control.PreviewMouseDoubleClick += new MouseButtonEventHandler(Element_PreviewMouseDoubleClick);
        }
    }
}
