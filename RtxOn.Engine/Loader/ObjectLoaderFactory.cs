namespace RtxOn.Console.Loader
{
    public class ObjectLoaderFactory : IObjectLoaderFactory
    {
        public IObjectLoader CreateLoader(string fileName) => 
            fileName.Split(".").Last() switch
            {
                "obj" => new ObjLoader(fileName),
                "stl" => new StlLoader(),
                _ => throw new NotSupportedException(fileName)
            };
    }
}