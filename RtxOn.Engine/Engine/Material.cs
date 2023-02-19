using RtxOn.Engine.Common;

namespace RtxOn.Engine.Tracer;

public class Material
{
    public string Name { get; set; }

    public Color AmbientColor { get; set; }

    public Color DiffuseColor { get; set; }

    public Color SpecularColor { get; set; }

    public double SpecularHighlights { get; set; }

    public double OpticalDensity { get; set; }

    public ImageTexture Texture { get; set; }
}

