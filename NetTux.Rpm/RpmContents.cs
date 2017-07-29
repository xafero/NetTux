using org.redline_rpm.payload;
using NetTux.Common;
using java.io;

namespace NetTux.Rpm
{
    class RpmContents
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
            const int dirMode = 0;
            _includes.addFile(c.Path, source, c.Permissions, directive, c.User, c.Group, dirMode, addParents);
        }
    }
}