using Microsoft.Xaml.Behaviors;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfRefactorFramework.Helpers
{
    public class LineLimitingBehavior : Behavior<TextBox>
    {
        public int? TextBoxMaxAllowedLines { get; set; }
        protected override void OnAttached()
        {
            if (TextBoxMaxAllowedLines > 0)
            {
                AssociatedObject.TextChanged += OnTextBoxTextChanged;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= OnTextBoxTextChanged;
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;

            var textLineCount = textBox.Text.Length;

            //Use Dispatcher to undo - http://stackoverflow.com/a/25453051/685341
            if (textLineCount > TextBoxMaxAllowedLines)
            {
                // Очистить после
                _ = Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)(() => textBox.Clear()));
            }
        }
        
    }
}