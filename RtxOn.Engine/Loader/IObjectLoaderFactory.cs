namespace RtxOn.Engine.Loader
{
    public interface IObjectLoaderFactory
    {
        IObjectLoader CreateLoader(string fileName);
    }
}