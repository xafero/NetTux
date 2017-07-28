using System.Collections.Generic;
using System.Text;
using System.IO;

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
    }
}