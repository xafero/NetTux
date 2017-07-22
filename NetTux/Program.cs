using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

using static NetTux.Archiver;

using File = System.IO.File;

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
            WriteTarGzArchive(dataTgz, "NetTux.exe.config");
            // Collect control stuff
            var control = Path.Combine(temp, "control");
            Debian.WriteControl(control, config, enc);
            var md5sums = Path.Combine(temp, "md5sums");
            Debian.WriteHashes(md5sums, config, enc);
            var postinst = Path.Combine(temp, "postinst");
            Debian.WriteScript(postinst, config, enc);
            var postrm = Path.Combine(temp, "postrm");
            Debian.WriteScript(postrm, config, enc);
            // Write control bundle
            var controlTgz = Path.Combine(temp, "control.tar.gz");
            WriteTarGzArchive(controlTgz, control, md5sums, postinst, postrm);
            // Write version
            var binaryFile = Path.Combine(temp, "debian-binary");
            File.WriteAllText(binaryFile, "2.0" + '\n', enc);
            // Write Debian package
            var debFile = Path.Combine("test.deb");
            WriteArFile(debFile, binaryFile, controlTgz, dataTgz);
            // Done
            return 0;
        }
    }
}