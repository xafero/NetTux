using org.redline_rpm.payload;
using NetTux.Common;
using java.io;

using static NetTux.Common.LinuxIO;

namespace NetTux.Rpm
{
    public class RpmContents
    {
        readonly Contents _includes;

        public RpmContents(Contents includes)
        {
            _includes = includes;
        }

        public void Add(IContent c)
        {
            var source = new File(c.Source);
            var directive = Directive.NONE;
            const bool addParents = true;
            var path = FixSlash(c.Path).TrimStart('.');
            var permissions = c.Permissions ?? GetPermissions(path, true);
            var dirMode = GetPermissions(path, false);
            if (path.Contains("/doc/"))
                directive = Directive.DOC;
            _includes.addFile(path, source, permissions, directive, c.User, c.Group, dirMode, addParents);
        }
    }
}