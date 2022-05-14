namespace RtxOn.Console.Loader
{
    public interface IObjectLoaderFactory
    {
        IObjectLoader CreateLoader(string fileName);
    }
}