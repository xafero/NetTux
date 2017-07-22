using System.IO;
using System.Text;

using static NetTux.Archiver;

using File = System.IO.File;

namespace NetTux
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var temp = "tmp";
            Directory.CreateDirectory(temp);

            var enc = Encoding.UTF8;

            var dataTgz = Path.Combine(temp, "data.tar.gz");
            WriteTarGzArchive(dataTgz, "NetTux.exe.config");

            var controlTgz = Path.Combine(temp, "control.tar.gz");
            WriteTarGzArchive(controlTgz, "control");

            var binaryFile = Path.Combine(temp, "debian-binary");
            File.WriteAllText(binaryFile, "2.0", enc);

            var debFile = Path.Combine("test.deb");
            WriteArFile(debFile, binaryFile, controlTgz, dataTgz);
        }        
    }
}