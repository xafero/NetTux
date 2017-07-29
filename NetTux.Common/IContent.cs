namespace NetTux.Common
{
    public interface IContent
    {
        string Source { get; }

        string Path { get; }

        int Permissions { get; }

        string User { get; }

        string Group { get; }
    }
}