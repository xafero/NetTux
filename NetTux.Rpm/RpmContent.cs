using NetTux.Common;
using System.Collections.Generic;

using SysPath = System.IO.Path;

namespace NetTux.Rpm
{
    public class RpmContent : IContent
    {
        public string Path { get; set; }
        public string Source { get; set; }
        public int Permissions { get; }
        public string Group { get; set; }
        public string User { get; set; }

        public static IEnumerable<IContent> Wrap(TarInput input)
        {
            var baseDir = input.BaseDir == null ? null : SysPath.GetFullPath(input.BaseDir);
            foreach (var file in input.Files)
            {
                var relative = baseDir == null ? SysPath.GetFileName(file)
                    : SysPath.GetFullPath(file).Replace(baseDir, string.Empty)
                    .TrimStart(SysPath.DirectorySeparatorChar);
                yield return new RpmContent
                {
                    Source = file,
                    Path = SysPath.Combine(input.InstallDir, relative),
                    User = "root",
                    Group = "root"
                };
            }
        }
    }
}