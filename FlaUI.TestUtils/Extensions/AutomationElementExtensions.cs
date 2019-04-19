using System;
using System.Drawing;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using UI.TestUtils.Exceptions;
using FlaUI.UIA3;
using FlaUI.Core;

namespace UI.TestUtils.Extensions
{
    public static class AutomationElementExtensions
    {
        public static AutomationElement GetWindowAndButton(this AutomationElement element, Window window, UIA3Automation automation, Application app, string nameWindow, string nameButton)
        {
            try
            {
                element = window.FindFirstDescendant(x => x.ByName(nameButton), Constants.Times.NetConsultTimeSpan);
                WaitEx.UntilResponsive(window, Constants.Times.DefaultTimeSpan);
                return element;
            }
            catch
            {
                window = app.GetWindow(automation, x => x.Name == nameWindow, Constants.Times.DefaultTimeSpan);
                WaitEx.UntilResponsive(window, Constants.Times.DefaultTimeSpan);
                element = window.FindFirstDescendant(x => x.ByName(nameButton), Constants.Times.NetConsultTimeSpan);
                WaitEx.UntilResponsive(window, Constants.Times.DefaultTimeSpan);
                return element;
            }
        }


        public static AutomationElement FindFirstDescendant(this AutomationElement element, Func<ConditionFactory, ConditionBase> conditionFunc, TimeSpan timeout, bool nothrow = false)
        {
            var condition = conditionFunc(element.ConditionFactory);

            var result = Retry.WhileNull(() => element.FindFirst(TreeScope.Descendants, condition),
                timeout, ignoreException: true, throwOnTimeout: !nothrow);

            return result.Result;
        }

        public static void WaitUntilEnabled(this AutomationElement element, TimeSpan timeout) =>
            Retry.WhileFalse(() => element.IsEnabled, timeout, ignoreException: true, throwOnTimeout: true);

        public static void WaitUntilVisible(this AutomationElement element, TimeSpan timeout) =>
            Retry.WhileTrue(() => element.BoundingRectangle.IsEmpty, timeout, ignoreException: true, throwOnTimeout: true);

        public static void EnhancedClick(this AutomationElement element, bool moveMouse, bool nothrow = false)
        {
            var timeout = TimeSpan.FromSeconds(3);

            var clickable = element.WaitUntilClickableEx(timeout);
            if (clickable)
            {
                element.Click(moveMouse);
                return;
            }

            element.SetForeground();
            clickable = element.WaitUntilClickableEx(timeout);
            if (clickable)
            {
                element.Click(moveMouse);
                return;
            }

            var window = element.GetParentWindow();
            if (window != null)
            {
                WaitEx.UntilResponsive(window, timeout);
                try
                {
                    window.Patterns?.Window?.Pattern?.SetWindowVisualState(WindowVisualState.Maximized);
                }
                catch
                {
                    // ignored
                }

                WaitEx.UntilResponsive(window, timeout);

                clickable = element.WaitUntilClickableEx(timeout);
                if (clickable)
                    element.Click(moveMouse);
                else if(!nothrow)
                    throw new NotClickableException();
            }
        }

        private static bool WaitUntilClickableEx(this AutomationElement element, TimeSpan timeout)
        {
            Point point;
            var result = Retry.WhileFalse(() => element.TryGetClickablePoint(out point), timeout, ignoreException: true);
            return result.Result;
        }

        private static Window GetParentWindow(this AutomationElement element)
        {

            var automationElement2 = element;
            var controlViewWalker = element.Automation.TreeWalkerFactory.GetControlViewWalker();
            while (automationElement2.Properties.NativeWindowHandle.ValueOrDefault == new IntPtr(0))
                automationElement2 = controlViewWalker.GetParent(automationElement2);

            return automationElement2.AsWindow();
        }
    }
}
