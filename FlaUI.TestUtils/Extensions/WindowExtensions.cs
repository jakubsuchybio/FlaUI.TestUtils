using System;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

namespace UI.TestUtils.Extensions
{
    public static class WindowExtensions
    {
        public static Window GetModalWindow(this Window window, Func<Window, bool> condition, TimeSpan timeout)
        {
            var result = Retry.WhileNull(() => window.ModalWindows.Where(condition).FirstOrDefault(),
                timeout, ignoreException: true, throwOnTimeout: true);

            return result.Result;
        }

        public static void WaitUntilWindowClosed(this Window window, TimeSpan timeout)
        {
            Retry.WhileTrue(() => window.Patterns.Window.IsSupported, timeout, ignoreException: true, throwOnTimeout: true);
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(500));
        }
    }
}