using System.IO;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors;
using org.apache.commons.compress.archivers.ar;
using java.io;

using File = System.IO.File;
using org.apache.commons.compress.utils;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.Text;
using NetTux.Common;

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

        public static void WriteGzArchive(string output, string input)
        {
            using (var inp = File.OpenRead(input))
            using (var stream = File.Create(output))
            using (var gzip = new GZipStream(stream, CompressionMode.Compress) { LastModified = null })
                inp.CopyTo(gzip);
        }

        public static void WriteTarGzArchive(string output, params TarInput[] bunches)
        {
            var dirs = new HashSet<string>();
            using (var stream = File.Create(output))
            using (var gzip = new GZipStream(stream, CompressionMode.Compress, CompressionLevel.BestCompression))
            using (var tar = new TarOutputStream(gzip))
            {
                foreach (var bunch in bunches)
                {
                    var baseDir = bunch.BaseDir;
                    var prefixDir = bunch.InstallDir;
                    var inputs = bunch.Files;
                    if (baseDir != null)
                        baseDir = Path.GetFullPath(baseDir);
                    foreach (var file in inputs)
                    {
                        var info = new FileInfo(file);
                        var name = info.Name;
                        if (baseDir != null)
                            name = Path.GetFullPath(file).Replace(baseDir, "").TrimStart(Path.DirectorySeparatorChar);
                        if (prefixDir != null)
                            name = Path.Combine(prefixDir, name);
                        EnsureDirectories(name, dirs, tar);
                        var source = File.OpenRead(file);
                        var size = info.Length;
                        var modified = info.LastWriteTime;
                        var entry = TarEntry.CreateEntryFromFile(file);
                        var header = entry.TarHeader;
                        header.Name = FixSlash(name);
                        header.Size = size;
                        header.ModTime = modified;
                        header.GroupName = "root";
                        header.UserName = "root";
                        var exe = info.Name == "postinst" || info.Name == "postrm";
                        entry.TarHeader.Mode = Convert.ToInt32("100" + (exe ? "755" : "644"), 8);
                        tar.PutNextEntry(entry);
                        using (source)
                            source.CopyTo(tar);
                        tar.CloseEntry();
                    }
                }
            }
        }

        static void EnsureDirectories(string name, ICollection<string> dirs, TarOutputStream tar)
        {
            var root = Path.GetDirectoryName(name);
            var parts = root.Split(Path.DirectorySeparatorChar);
            var path = new StringBuilder();
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (string.IsNullOrWhiteSpace(part))
                    continue;
                if (i != 0)
                    path.Append(Path.DirectorySeparatorChar);
                path.Append(part);
                var current = path.ToString() + Path.DirectorySeparatorChar;
                if (dirs.Contains(current))
                    continue;
                dirs.Add(current);
                var entry = TarEntry.CreateTarEntry(FixSlash(current));
                var header = entry.TarHeader;
                TarEntry.NameTarHeader(header, FixSlash(current));
                header.GroupName = "root";
                header.UserName = "root";
                header.Mode = Convert.ToInt32("000" + "755", 8);
                tar.PutNextEntry(entry);
                tar.CloseEntry();
            }
        }

        static string FixSlash(string name) => name.Replace('\\', '/');
    }
}