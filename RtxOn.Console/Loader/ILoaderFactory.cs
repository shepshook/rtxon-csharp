namespace RtxOn.Console.Loader
{
    public interface ILoaderFactory
    {
        ILoader CreateLoader(string fileName);
    }
}