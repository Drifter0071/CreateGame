using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using System.Numerics;

namespace CreateGame.Runtime;

public class EngineWindow
{
        private readonly IWindow _window;
        private Renderer _renderer = null!;
        private Input _input = null!;
        private Shader _shader = null!;
        private Matrix4x4 _projectionMatrix;
        private Camera _camera = null!;

        private DrawService _drawService = new();

        public EngineWindow() 
        {
                var options = WindowOptions.Default;
                options.Size = new Vector2D<int>(1280, 720);
                options.Title = "The Create Game";

                _window = Window.Create(options);
                _window.Load += OnLoad;
                _window.Update += OnUpdate;
                _window.Render += OnRender;
        }

        public void Run() => _window.Run();

        private void OnLoad()
        {
                _renderer = new Renderer(GL.GetApi(_window));
                _input = new Input(_window.CreateInput());

                _camera = new Camera(new Vector3(0, 15, 20));

                _shader = new Shader(_renderer.Gl!, "Shaders/shader.vert", "Shaders/shader.frag");
                _renderer.ActiveShader = _shader;

                _drawService.Initialize(_renderer);

                // Test Baseplate
                var baseplate = new PrimitiveObject
                {
                        Position = new Vector3(0, -5, 0),
                        Size = new Vector3(200, 10, 200),
                        Color = new Vector4(0.3f, 0.3f, 0.3f, 1.0f)
                };
                _drawService.Register(baseplate);

                // Test Cube
                var cube = new PrimitiveObject
                {
                        Position = new Vector3(0, 2, 0),
                        Size = new Vector3(2, 2, 2),
                        Color = new Vector4(1.0f, 0.5f, 0.0f, 1.0f)
                };
                _drawService.Register(cube);

                // perspective lens
                _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                        MathF.PI / 4f,
                        1280f / 720f, // aspect ratio
                        0.1f,  // near clipping plane
                        1000.0f // far clipping plane
                );
        }

        private void OnUpdate(double deltaTime)
        {
                _input.Update();

                if (_input.IsKeyPressed(Key.Escape))
                {
                        _window.Close();
                }

                _camera.Move(_input.GetMovementInput(), (float)deltaTime);
                _camera.Look(_input.MouseDelta);
        }

        private void OnRender(double deltaTime)
        {
                _renderer.ClearScreen(245f/255f, 245f/255f, 245f/255f);

                // activate shader program
                _shader.Use();

                // send camera math to gpu
                _shader.SetMatrix4("uView", _camera.GetViewMatrix());
                _shader.SetMatrix4("uProjection", _projectionMatrix);

                // draw objects
                _drawService.DrawAll(_renderer);
        }
}