using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

using static NetTux.Archiver;

using File = System.IO.File;
using System.Collections.Generic;

namespace NetTux
{
    public static class Program
    {
        public static int Main(string[] parms)
        {
            if (parms.Length != 1)
            {
                Console.WriteLine("Usage: [json-file]");
                return -1;
            }
            // Read configuration
            var set = new JsonSerializerSettings();
            var text = File.ReadAllText(parms.First(), Encoding.UTF8);
            var config = JsonConvert.DeserializeObject<TuxConfig>(text, set);
            config.BuildDirectory = Environment.GetEnvironmentVariable("BUILD_DMG") ?? config.BuildDirectory;
            // Create temporary directory
            var temp = config.AppTemp;
            Directory.CreateDirectory(temp);
            var enc = new UTF8Encoding(false);
            // Write data stuff
            var dataTgz = Path.Combine(temp, "data.tar.gz");
            var dataFiles = Directory.GetFiles(config.BuildDirectory, "*.*", SearchOption.AllDirectories);
            var lowerPkgName = config.PkgName.ToLowerInvariant();
            var installRoot = Path.Combine(".", "opt", lowerPkgName);
            var dataStuff = new TarInput
            {
                Files = dataFiles,
                InstallDir = installRoot,
                BaseDir = config.BuildDirectory
            };
            // Write meta stuff
            var docRoot = Path.Combine(".", "usr", "share", "doc", lowerPkgName);
            var copyright = Path.Combine(temp, "copyright");
            Debian.WriteScript(copyright, config, enc, GenerateCopyright(config));
            var metaStuff = new TarInput
            {
                Files = new[] { copyright },
                InstallDir = docRoot,
            };
            WriteTarGzArchive(dataTgz, metaStuff, dataStuff);
            // Collect control stuff
            var control = Path.Combine(temp, "control");
            Debian.WriteControl(control, config, enc);
            var md5sums = Path.Combine(temp, "md5sums");
            Debian.WriteHashes(md5sums, config, enc);
            var postinst = Path.Combine(temp, "postinst");
            Debian.WriteScript(postinst, config, enc, new[]
            {
                "#!/bin/sh",
                "set -e",
                "if [ \"$1\" = \"configure\" ] && [ -x \"`which update-menus 2>/dev/null`\" ]; then",
                "\tupdate-menus",
                "fi"
            });
            var postrm = Path.Combine(temp, "postrm");
            Debian.WriteScript(postrm, config, enc, new[]
            {
                "#!/bin/sh",
                "set -e",
                "if [ -x \"`which update-menus 2>/dev/null`\" ]; then update-menus; fi"
            });
            // Write control bundle
            var controlTgz = Path.Combine(temp, "control.tar.gz");
            WriteTarGzArchive(controlTgz, new TarInput
            {
                Files = new[] { control, md5sums, postinst, postrm },
                InstallDir = Path.Combine(".")
            });
            // Write version
            var binaryFile = Path.Combine(temp, "debian-binary");
            File.WriteAllText(binaryFile, "2.0" + '\n', enc);
            // Write Debian package
            var debFile = config.PackageFile;
            WriteArFile(debFile, binaryFile, controlTgz, dataTgz);
            // Done
            return 0;
        }

        static IEnumerable<string> GenerateCopyright(TuxConfig config)
        {
            return new[]
            {
                "Format: https://www.debian.org/doc/packaging-manuals/copyright-format/1.0/",
                $"Upstream-Name: {config.PkgName}",
                $"Source: {config.Homepage}",
                "",
                "Files: *",
                $"Copyright: 2017 {config.Maintainer}",
                "License: Proprietary"
            };
        }
    }
}