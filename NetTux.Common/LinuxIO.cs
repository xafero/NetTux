using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

namespace NetTux.Common
{
    public static class LinuxIO
    {
        public static void WriteScript(string path, Encoding enc, IEnumerable<string> lines)
        {
            var text = string.Join('\n' + "", lines);
            File.WriteAllText(path, text, enc);
        }

        public static void WriteHashes(string path, Encoding enc)
        {
            File.WriteAllText(path, "", enc);
        }

        public static void Add(ICollection<string> lines, string key, string value)
        {
            lines.Add($"{key}: {value}");
        }

        public static string FormatDesc(string description)
            => description.Replace('|' + "", '\n' + " ");

        public static string FixSlash(string name) => name.Replace('\\', '/');

        public static int GetPermissions(string name, bool isFile)
        {
            if (!isFile)
                return Convert.ToInt32("000" + "755", 8);
            var exe = name == "postinst" || name == "postrm";
            return Convert.ToInt32("100" + (exe ? "755" : "644"), 8);
        }
    }
}