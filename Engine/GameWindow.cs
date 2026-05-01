using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using System.Numerics;

namespace CreateGame.Engine;

public class GameWindow
{
        private readonly IWindow _window;
        private Renderer _renderer = null!;
        private Shader _shader = null!;
        private Matrix4x4 _projectionMatrix;

        // Exposed game input and camera updates
        public Input  Input  { get; private set; } = null!;
        public Camera Camera { get; private set; } = null!;

        // Exposed window
        public IWindow Window => _window;

        public GameWindow()
        {
                var options = WindowOptions.Default;
                options.Size  = new Vector2D<int>(1280, 720);
                options.Title = "The Create Game";

                _window = Silk.NET.Windowing.Window.Create(options);
                _window.Load   += OnLoad;
                _window.Update += OnUpdate;
                _window.Render += OnRender;
        }

        public void Run() => _window.Run();

        private void OnLoad()
        {
                _renderer      = new Renderer(GL.GetApi(_window));
                Input          = new Input(_window.CreateInput());
                Camera         = new Camera(new Vector3(0, 15, 20));
                _shader        = new Shader(_renderer.Gl!, "Shaders/shader.vert", "Shaders/shader.frag");
                _renderer.ActiveShader = _shader;

                DrawService.Initialize(_renderer);

                _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                        MathF.PI / 4f,
                        1280f / 720f, // aspect ratio
                        0.1f,         // near clipping plane
                        1000.0f       // far clipping plane
                );

                // Let game code populate the scene
                RunService.RenderStarted.Fire();
        }

        private void OnUpdate(double deltaTime)
        {
                float dt = (float)deltaTime;
                Input.Update();
                Camera.Look(Input.MouseDelta);

                // Let game code handle per-frame logic
                RunService.RenderStepped.Fire(dt);
        }

        private void OnRender(double deltaTime)
        {
                _renderer.ClearScreen(245f / 255f, 245f / 255f, 245f / 255f);

                _shader.Use();
                _shader.SetMatrix4("uView",       Camera.GetViewMatrix());
                _shader.SetMatrix4("uProjection", _projectionMatrix);

                DrawService.DrawAll(_renderer);
        }
}