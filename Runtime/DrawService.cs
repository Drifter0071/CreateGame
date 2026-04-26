using Silk.NET.OpenGL;

namespace CreateGame.Runtime;

public class DrawService
{
    private readonly List<Volume3D> _objects = [];
    private VertexArray<float, uint> _unitCubeVao = null!;

    public void Register(Volume3D obj) => _objects.Add(obj);

    public void Initialize(Renderer renderer)
    {
        // 1x1x1 cube data
        float[] vertices = {
            -0.5f,-0.5f,-0.5f,  0.5f,-0.5f,-0.5f,  0.5f, 0.5f,-0.5f, -0.5f, 0.5f,-0.5f,
            -0.5f,-0.5f, 0.5f,  0.5f,-0.5f, 0.5f,  0.5f, 0.5f, 0.5f, -0.5f, 0.5f, 0.5f
        };
        uint[] indices = { 0,1,3, 1,2,3, 4,5,7, 5,6,7, 0,4,3, 4,7,3, 1,5,2, 5,6,2, 3,2,7, 2,6,7, 0,1,4, 1,5,4 };

        _unitCubeVao = new VertexArray<float, uint>(renderer.Gl!, vertices, indices);
        _unitCubeVao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 3, 0);
    }

    public void DrawAll(Renderer renderer)
    {
        _unitCubeVao.Bind();
        foreach (var obj in _objects)
        {
            obj.Draw(renderer);
            unsafe
            {
                renderer.Gl!.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, (void*)0);
            }
        }
    }
}