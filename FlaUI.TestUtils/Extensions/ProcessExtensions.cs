using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

namespace UI.TestUtils.Extensions
{
    public static class ProcessEx
    {
        public static void WaitForExit(string path, TimeSpan timeout)
        {
            var process = WaitForStart(path, TimeSpan.FromSeconds(3), false);
            if (process == null)
                return;

            process.WaitUntilExited(timeout);
        }

        public static Process WaitForStart(string path, TimeSpan timeout, bool throwOnTimeout = true)
        {
            var fileName = path;
            if (Path.IsPathRooted(path))
                fileName = Path.GetFileNameWithoutExtension(path);

            var result = Retry.WhileNull(() => Process.GetProcessesByName(fileName).FirstOrDefault(),
                timeout, ignoreException: true, throwOnTimeout: throwOnTimeout);

            return result.Result;
        }

        public static void WaitUntilExited(this Process process, TimeSpan timeout, bool forceKill = false)
        {
            var result = Retry.WhileFalse(() => process.HasExited, timeout, ignoreException: true);

            if (forceKill && !result.Result)
            {
                process.Kill();
                Retry.WhileFalse(() => process.HasExited, timeout, ignoreException: true, throwOnTimeout: true);
            }

            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(500));
        }
    }
}