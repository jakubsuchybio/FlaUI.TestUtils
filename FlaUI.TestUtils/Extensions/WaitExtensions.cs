using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

namespace UI.TestUtils.Extensions
{
    public static class WaitEx
    {
        public static void UntilResponsive(AutomationElement element, TimeSpan timeout) =>
            Retry.WhileFalse(() => Wait.UntilResponsive(element, timeout), timeout, ignoreException: true, throwOnTimeout: true);
    }
}
