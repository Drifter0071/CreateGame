using Silk.NET.OpenGL;

namespace CreateGame.Engine;

public class VertexArray<TVertex, TIndex> : IDisposable
    where TVertex : unmanaged
    where TIndex : unmanaged
{
    private readonly uint _handle;
    private readonly uint _vbo;
    private readonly uint _ebo;
    private readonly GL _gl = null!;

    public VertexArray(GL gl, Span<TVertex> vertecies, Span<TIndex> indices)
    {
        _gl = gl;
        _handle = _gl.GenVertexArray();
        _gl.BindVertexArray(_handle);

        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        unsafe
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, 
                        (nuint)(vertecies.Length * sizeof(TVertex)), 
                        vertecies, BufferUsageARB.StaticDraw);
        }

        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        unsafe
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, 
                        (nuint)(indices.Length * sizeof(TIndex)), 
                        indices, BufferUsageARB.StaticDraw);
        }
    }

    public void Bind() => _gl.BindVertexArray(_handle);

    public unsafe void VertexAttributePointer(uint index, int count, 
                                            VertexAttribPointerType type, 
                                            uint stride, int offset) 
    {
        _gl.VertexAttribPointer(index, count, type, false, 
                            stride * (uint)sizeof(float), (void*)(offset * sizeof(float)));
        _gl.EnableVertexAttribArray(index);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_handle);
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteBuffer(_ebo);
        GC.SuppressFinalize(this);
    }
}