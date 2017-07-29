using NetTux.Common;

namespace NetTux.Rpm
{
    class RpmContent : IContent
    {
        public RpmContent()
        {
            Path = "a";
            Source = "b";
            Permissions = 755;
            Group = "c";
            User = "d";
        }

        public string Path { get; }
        public int Permissions { get; }
        public string Source { get; }
        public string Group { get; }
        public string User { get; }
    }
}