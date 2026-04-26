using Silk.NET.OpenGL;
using System.Linq;

namespace CreateGame.Engine;

public class DrawService
{
    private readonly List<Volume3D> _objects = [];
    private readonly List<PrimitiveObject> _staticColliders = [];
    private VertexArray<float, uint> _unitCubeVao = null!;

    public void Register(Volume3D obj, bool isCollidable = false)
    {
        _objects.Add(obj);

        if (isCollidable && obj is PrimitiveObject primitive)
        {
            _staticColliders.Add(primitive);
        }
    }

    public List<PrimitiveObject> GetStaticObjects()
    {
        return _staticColliders;
    }

    public void Initialize(Renderer renderer)
    {
        // 1x1x1 cube data
        float[] vertices = {
            -0.5f,-0.5f,-0.5f,  0.5f,-0.5f,-0.5f,  0.5f, 0.5f,-0.5f, -0.5f, 0.5f,-0.5f,
            -0.5f,-0.5f, 0.5f,  0.5f,-0.5f, 0.5f,  0.5f, 0.5f, 0.5f, -0.5f, 0.5f, 0.5f
        };

        // Corrected winding order: all faces CCW when viewed from outside
        uint[] indices = {
            // Back  (-Z): 0,3,1  1,3,2
            0,3,1,  1,3,2,
            // Front (+Z): 4,5,7  5,6,7
            4,5,7,  5,6,7,
            // Left  (-X): 0,4,3  4,7,3
            0,4,3,  4,7,3,
            // Right (+X): 1,2,5  5,2,6
            1,2,5,  5,2,6,
            // Top   (+Y): 3,7,2  2,7,6
            3,7,2,  2,7,6,
            // Bottom(-Y): 0,1,4  1,5,4
            0,1,4,  1,5,4
        };

        _unitCubeVao = new VertexArray<float, uint>(renderer.Gl!, vertices, indices);
        _unitCubeVao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 3, 0);

        // Enable face culling
        renderer.Gl!.Enable(EnableCap.CullFace);
        renderer.Gl.CullFace(TriangleFace.Back);
        renderer.Gl.FrontFace(FrontFaceDirection.Ccw);
    }

    public void DrawAll(Renderer renderer)
    {
        _unitCubeVao.Bind();
        foreach (var obj in _objects)
        {
            if (!obj.IsVisible) continue;

            obj.Draw(renderer);
            unsafe
            {
                renderer.Gl!.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, (void*)0);
            }
        }
    }
}