namespace RtxOn.Console.Loader
{
    public class LoaderFactory : ILoaderFactory
    {
        public ILoader CreateLoader(string fileName) => 
            fileName.Split(".").Last() switch
            {
                "obj" => new ObjLoader(fileName),
                "stl" => new StlLoader(),
                _ => throw new NotSupportedException(fileName)
            };
    }
}