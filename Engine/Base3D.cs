using System.Numerics;

namespace CreateGame.Engine;

public interface IBody3D
{
    Vector3 Position { get; set; }
    Vector3 Size { get; set; }
}

public abstract class Volume3D : IBody3D
{
    public Vector3 Position { get; set; }
    public Vector3 Size { get; set; }
    public bool IsVisible { get; set; } = true;

    public uint VertexArrayObject { get; protected set; }

    public abstract void Initialize(Renderer renderer);
    public abstract void Draw(Renderer renderer);
}

public class PrimitiveObject : Volume3D
{
    public Vector4 Color { get; set; } = new Vector4(1, 1, 1, 1);

    public override void Initialize(Renderer renderer) {}
    public override void Draw(Renderer renderer)
    {
        if (renderer.ActiveShader == null) return;

        var model = Matrix4x4.CreateScale(Size) * Matrix4x4.CreateTranslation(Position);
        
        renderer.ActiveShader.SetMatrix4("uModel", model);
        renderer.ActiveShader.SetVector4("uColor", Color);
    }
}