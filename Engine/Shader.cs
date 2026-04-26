using System.Numerics;
using Silk.NET.OpenGL;

namespace CreateGame.Engine;

public class Shader : IDisposable
{
    private readonly uint _handle;
    private readonly GL _gl;

    // cache variables
    private readonly Dictionary<string, int> _uniformLocationCache = [];

    public uint Handle => _handle;

    public Shader(GL gl, string vertexPath, string fragmentPath)
    {
        _gl = gl;

        // load shader source code
        string vertexSrc = File.ReadAllText(vertexPath);
        string fragmentSrc = File.ReadAllText(fragmentPath);

        // compile shaders
        uint vertex = LoadShader(ShaderType.VertexShader, vertexSrc);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentSrc);

        // link into program
        _handle = _gl.CreateProgram();
        _gl.AttachShader(_handle, vertex);
        _gl.AttachShader(_handle, fragment);
        _gl.LinkProgram(_handle);
        _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Program failed to link: {_gl.GetProgramInfoLog(_handle)}");
        }

        // clean up
        _gl.DetachShader(_handle, vertex);
        _gl.DetachShader(_handle, fragment);
        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);
    }

    public void Use() => _gl.UseProgram(_handle);

    public int GetUniformLocation(string name)
    {
        if (_uniformLocationCache.TryGetValue(name, out int location))
        {
            return location;
        }

        location = _gl.GetUniformLocation(_handle, name);

        if (location == -1)
        {
            Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
        }

        _uniformLocationCache[name] = location;
        return location;
    }

    public void SetMatrix4(string name, Matrix4x4 matrix)
    {
        int location = GetUniformLocation(name);

        unsafe
        {
            _gl.UniformMatrix4(location, 1, false, (float*)&matrix);
        }
    }

    public void SetVector4(string name, Vector4 vector)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform4(location, vector.X, vector.Y, vector.Z, vector.W);
    }

    private uint LoadShader(ShaderType type, string src)
    {
        uint handle = _gl.CreateShader(type);
        _gl.ShaderSource(handle, src);
        _gl.CompileShader(handle);
        string infoLog = _gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }
        return handle;
    }

    public void Dispose()
    {
        _gl.DeleteProgram(_handle);
        GC.SuppressFinalize(this);
    }
}