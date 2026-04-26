using Silk.NET.OpenGL;

namespace CreateGame.Engine;

public class Renderer
{
    public GL? Gl { get; private set; }
    public Shader? ActiveShader { get; set; }

    public Renderer(GL glContext)
    {
        Gl = glContext;

        Gl.Enable(EnableCap.DepthTest);
    }

    public void ClearScreen(float r, float g, float b)
    {
        Gl!.ClearColor(r, g, b, 1.0f);
        // clear color and depth
        Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
    }
}