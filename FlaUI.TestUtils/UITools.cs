using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlaUI.Core.Capturing;
using UI.TestUtils.Extensions;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;
using FlaUI.Core.AutomationElements;
using System.Text.RegularExpressions;

namespace UI.TestUtils
{
    public static class UITools
    {
        public static void GetScreenshot(string imagePrefixName)
        {
            var targetDir = "Screenshots";

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            var imageFileName = string.Format(CultureInfo.InvariantCulture, "{0}_{1:yyyy-MM-dd-HH-mm-ss}.png", imagePrefixName, DateTime.Now);

            var screenShot = FlaUI.Core.Capturing.Capture.Screen();
            screenShot.ApplyOverlays(new MouseOverlay(screenShot));
            screenShot.ToFile(Path.Combine(targetDir, imageFileName));
        }

        private static bool Contains(this string input, string value, StringComparison comparison) =>
            input.IndexOf(value, comparison) >= 0;

        private static void RecursiveDeleteDirectory(DirectoryInfo dir, StringBuilder sb)
        {
            var files = dir.GetFileSystemInfos();
            foreach (var file in files)
            {
                if (file.Attributes == FileAttributes.Directory)
                    RecursiveDeleteDirectory(new DirectoryInfo(file.FullName), sb);
                else
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        var reason = string.Empty;
                        if (ex.Message.Contains("denied", StringComparison.InvariantCultureIgnoreCase))
                            reason = "denied";
                        else if (ex.Message.Contains("used by", StringComparison.InvariantCultureIgnoreCase))
                            reason = "used";
                        sb.Append($"File {file.Name} {reason}; ");
                    }
                }
            }

            try
            {
                dir.Delete(true);
            }
            catch
            {
                sb.Append($"Dir {dir.Name}; ");
            }
        }

        public static void KillAllProcesses()
        {
            var lockingProcessNameList = new[] {
                "MEW.Holter2g",
                "MEW.Main",
                "MEW.Agent",
                "MEW.NetworkAgent",
                "PdfInterface",
                "ProcessExplorer64",
                "ProcessExplorer",
                "procexp64",
                "procexp",
                "Taskmgr",
                "ConnectionString",
                "msiexec",
                "MewGDTPlugin",
                "Btl08WinPlugin",
                "BtlCardioPointCommunicationServerSetup",
                "BtlCardioPointTerminalSetup",
                "SQLite2MySQL",
                "MewSQLiteUpgrade",
                "MewSQLiteSetup",
                "MewMySqlUpgrade",
                "MewMySqlSetup",
                "MewMySqlExportCS",
                "BTL_TV_QS",
                "MewSetup",
                "MEW.Wizard",
                "Setup"};

            KillProcessNameList(lockingProcessNameList);
        }

        private static void KillProcessNameList(string[] lockingProcessNameList)
        {
            var endTasks = new List<Task>();
            foreach (var processName in lockingProcessNameList)
            {
                var processList = Process.GetProcessesByName(processName);
                foreach (var process in processList)
                {
                    var endTask = Task.Run(() =>
                    {
                        try
                        {
                            process.Kill();
                            process.WaitUntilExited(Constants.Times.DefaultTimeSpan);
                        }
                        catch
                        {
                            // I don't freaking care
                        }
                    });
                    endTasks.Add(endTask);
                }
            }

            Task.WaitAll(endTasks.ToArray());
        }

        public static byte[] ImageToByte(Bitmap image)
        {
            var bitmap = CropBitmap(image, 3, 2, image.Width - 6, image.Height - 5);

            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
        }

        private static Bitmap CropBitmap(Bitmap image, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            var rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            var cropped = image.Clone(rect, image.PixelFormat);
            return cropped;
        }

        private static readonly Random _random = new Random();
        public static string RandomString(int length)
        {
            const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(CHARS, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        public static string RandomNumber(int length)
        {
            const string CHARS = "0123456789";
            return new string(Enumerable.Repeat(CHARS, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        public static string RandomEmail(int length)
        {
            const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var name = new string(Enumerable.Repeat(CHARS, length)
               .Select(s => s[_random.Next(s.Length)]).ToArray());
            return string.Format(CultureInfo.InvariantCulture, "{0}@{0}.com", name);
        }
        public static string RandomDate()
        {
            var start = new DateTime(1905, 1, 1);
            var range = (DateTime.Today - start).Days;
            var test = start.AddDays(_random.Next(range));
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", test.Day, test.Month, test.Year);
        }

        public static void SaveList(List<string> names, string logintype)
        {
            using (var file =
             new StreamWriter(logintype + ".txt"))
            {
                foreach (var line in names)
                {
                    file.WriteLine(line);
                }
            }
        }
        public static void LogEx (string testname, Exception ex)
        {
            string fileName = $"{"Exception"}_{testname}_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.txt";
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            string LogPath = Path.Combine(outputDir, fileName);
            using (var file =
                new StreamWriter(LogPath))
            {
                file.WriteLine(ex.Message);
                file.WriteLine(ex.StackTrace);
            }
        }

        public static List<string> ReadList(string path)
        {
            var names = File.ReadAllLines(path).ToList();
            return names;
        }
        public static void GetVersions(Window window, out string agentVersion, out string serverVersion)
        {
            string[] versions = window.Name.Split(',');
            agentVersion = Regex.Replace(versions[1], "[^0-9.]", "");
            serverVersion = Regex.Replace(versions[2], "[^0-9.]", "");
        }

    }
}