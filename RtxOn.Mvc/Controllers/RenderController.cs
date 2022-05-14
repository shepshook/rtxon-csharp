using Microsoft.AspNetCore.Mvc;
using RtxOn.Console.Common;
using RtxOn.Console.Engine;
using RtxOn.Console.Loader;

namespace RtxOn.Mvc.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RenderController : ControllerBase
{
    private readonly IObjectLoaderFactory _loaderFactory;

    public RenderController(IObjectLoaderFactory loaderFactory)
    {
        _loaderFactory = loaderFactory;
    }

    [HttpPost]
    public Color[,] Render(ImportScene importScene)
    {
        var scene = new Scene();
        return Engine.Render(scene);
    }
}
