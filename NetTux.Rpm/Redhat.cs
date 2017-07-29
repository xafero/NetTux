using NetTux.Common;
using org.redline_rpm;
using org.redline_rpm.header;
using org.redline_rpm.payload;
using System;
using System.Linq;

using static NetTux.Common.LinuxIO;
using File = java.io.File;

namespace NetTux.Rpm
{
    public static class Redhat
    {
        public static void CreateRpm(TuxConfig cfg)
        {
            var destination = new File(cfg.AppTemp);
            var arch = Architecture.X86_64;
            var host = Environment.MachineName;
            const string license = "Proprietary";
            const string release = "unstable";
            const string group = "gnome";
            var desc = FormatDesc(cfg.Description).Split('\n');
            var summary = desc.First();
            var description = string.Join(string.Empty, desc.Skip(1));
            var include = new Contents();
            include.addDirectory(cfg.BuildDirectory);
            var builder = new Builder();
            builder.setPackage(cfg.PkgName, cfg.Version, release);
            builder.setType(RpmType.BINARY);
            builder.setPlatform(arch, Os.LINUX);
            builder.setSummary(summary);
            builder.setDescription(description);
            builder.setBuildHost(host);
            builder.setLicense(license);
            builder.setGroup(group);
            builder.setPackager(cfg.Maintainer.Split('<').First().Trim());
            builder.setVendor(cfg.Maintainer);
            builder.setUrl(cfg.Homepage);
            builder.setProvides(cfg.PkgName);
            builder.setFiles(include);
            builder.build(destination);
        }
    }
}