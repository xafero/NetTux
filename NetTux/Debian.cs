using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

namespace NetTux
{
    public static class Debian
    {
        public static void WriteControl(string path, TuxConfig config)
        {
            var enc = Encoding.UTF8;
            var lines = new List<string>();
            Add(lines, "Package", config.PkgName);
            Add(lines, "Version", config.Version);
            Add(lines, "Architecture", "amd64");
            Add(lines, "Maintainer", config.Maintainer);
            Add(lines, "Recommends", "mono");
            Add(lines, "Section", "gnome");
            Add(lines, "Priority", "optional");
            Add(lines, "Homepage", config.Homepage);
            Add(lines, "Description", config.Description);
            File.WriteAllLines(path, lines, enc);
        }

        public static void WriteScript(string path, TuxConfig config)
        {
            var enc = Encoding.UTF8;
            File.WriteAllText(path, "", enc);
        }

        public static void WriteHashes(string path, TuxConfig config)
        {
            var enc = Encoding.UTF8;
            File.WriteAllText(path, "", enc);
        }

        static void Add(ICollection<string> lines, string key, string value)
        {
            lines.Add($"{key}: {value}");
        }
    }
}