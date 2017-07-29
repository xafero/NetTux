using NetTux.Common;
using System.Collections.Generic;
using System.Text;
using System.IO;

using static NetTux.Common.LinuxIO;

namespace NetTux.Deb
{
    public static class Debian
    {
        public static void WriteControl(string path, TuxConfig config, Encoding enc)
        {
            var lines = new List<string>();
            Add(lines, "Package", config.PkgName);
            Add(lines, "Version", config.Version);
            Add(lines, "Architecture", "amd64");
            Add(lines, "Maintainer", config.Maintainer);
            Add(lines, "Recommends", "mono");
            Add(lines, "Section", "gnome");
            Add(lines, "Priority", "optional");
            Add(lines, "Homepage", config.Homepage);
            Add(lines, "Description", FormatDesc(config.Description));
            File.WriteAllLines(path, lines, enc);
        }
    }
}