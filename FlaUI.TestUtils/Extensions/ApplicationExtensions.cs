using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.UIA3;

namespace UI.TestUtils.Extensions
{
    public static class ApplicationEx
    {
        public static Window GetWindow(this Application app, UIA3Automation automation, Func<Window, bool> condition, TimeSpan timeout)
        {
            var result = Retry.WhileNull(() => app.GetAllTopLevelWindows(automation).Where(condition).FirstOrDefault(),
                timeout, ignoreException: true, throwOnTimeout: true);

            return result.Result;
        }

        public static Application AttachOrLaunch(string executable, TimeSpan timeout, int index = 0)
        {
            var array = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(executable));
            if (array.Length > index)
            {
                var attach = Retry.WhileNull(() => Application.Attach(array[index]),
                    timeout, TimeSpan.FromSeconds(5), ignoreException: true, throwOnTimeout: true);

                return attach.Result;
            }

            var launch = Retry.WhileNull(() => Application.Launch(executable),
                timeout, TimeSpan.FromSeconds(5), ignoreException: true, throwOnTimeout: true);

            return launch.Result;
        }

        public static Application Attach(string executable, TimeSpan timeout)
        {
            var fileName = executable;
            if (Path.IsPathRooted(executable))
                fileName = Path.GetFileNameWithoutExtension(executable);

            var result = Retry.WhileNull(() => Process.GetProcessesByName(fileName).FirstOrDefault(),
                timeout, ignoreException: true, throwOnTimeout: true);

            if (result.Result != null)
            {
                var attach = Retry.WhileNull(() => Application.Attach(result.Result),
                    timeout, TimeSpan.FromSeconds(10), ignoreException: true, throwOnTimeout: true);

                return attach.Result;
            }

            return null;
        }

        public static void WaitUntilExited(this Application app, TimeSpan timeout)
        {
            Retry.WhileFalse(() => app.HasExited, timeout, ignoreException: true, throwOnTimeout: true);
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(500));
        }

        public static void WaitUntilExitedWithAgent(this Application app, TimeSpan timeout)
        {
            Retry.WhileFalse(() => app.HasExited, timeout, ignoreException: true, throwOnTimeout: true);
            ProcessEx.WaitForExit(Constants.Paths.MewAgentPath, Constants.Times.DefaultTimeSpan);
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(500));
        }
    }
}
