using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Writers;
using System.IO;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors;
using org.apache.commons.compress.archivers.ar;
using java.io;

using File = System.IO.File;
using org.apache.commons.compress.utils;

namespace NetTux
{
    static class Archiver
    {
        public static void WriteArFile(string output, params string[] inputs)
        {
            using (var stream = new FileOutputStream(output))
            using (var ar = new ArArchiveOutputStream(stream))
            {
                foreach (var file in inputs)
                {
                    var info = new FileInfo(file);
                    var name = Path.GetFileName(file);
                    var entry = new ArArchiveEntry(name, info.Length);
                    ar.putArchiveEntry(entry);
                    using (var input = new FileInputStream(file))
                        IOUtils.copy(input, ar);
                    ar.closeArchiveEntry();
                }
            }
        }

        public static void WriteTarGzArchive(string output, string[] inputs, string baseDir = null)
        {
            if (baseDir != null)
                baseDir = Path.GetFullPath(baseDir);
            using (var stream = File.Create(output))
            using (var gzip = new GZipStream(stream, CompressionMode.Compress, CompressionLevel.BestCompression))
            using (var tar = TarArchive.Create())
            {
                foreach (var file in inputs)
                {
                    var info = new FileInfo(file);
                    var name = info.Name;
                    if (baseDir != null)
                        name = Path.GetFullPath(file).Replace(baseDir, "").TrimStart(Path.DirectorySeparatorChar);
                    var source = File.OpenRead(file);
                    var size = info.Length;
                    var modified = info.LastWriteTime;
                    tar.AddEntry(name, source, true, size, modified);
                }
                tar.SaveTo(gzip, new WriterOptions(CompressionType.None));
            }
        }
    }
}