using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using System.Numerics;
using CreateGame.Physics;

namespace CreateGame.Engine;

public class GameWindow
{
        private readonly IWindow _window;
        private Renderer _renderer = null!;
        private Input _input = null!;
        private Shader _shader = null!;
        private Matrix4x4 _projectionMatrix;
        private Camera _camera = null!;

        private readonly DrawService _drawService = new();

        private readonly PhysicsEngine _physicsEngine = new();
        private RigidBody3D _playerBody = null!;
        private readonly float _jumpForce = 15.0f;
        private readonly float _walkSpeed = 50.0f;

        public GameWindow() 
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
                _drawService.Register(baseplate, true);

                // Test Wall
                var cube = new PrimitiveObject
                {
                        Position = new Vector3(5, 2, 0),
                        Size = new Vector3(2, 5, 10),
                        Color = new Vector4(1.0f, 0.5f, 0.0f, 1.0f)
                };
                _drawService.Register(cube, true);

                // perspective lens
                _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                        MathF.PI / 4f,
                        1280f / 720f, // aspect ratio
                        0.1f,  // near clipping plane
                        1000.0f // far clipping plane
                );

                // player body
                _playerBody = new RigidBody3D
                {
                        Position = new Vector3(0, 5, 0),
                        Size = new Vector3(0.5f, 1.8f, 0.5f),
                        Mass = 1.0f,
                        Drag = 8.0f,
                        IsVisible = false // don't draw — camera is inside it
                };

                _drawService.Register(_playerBody, false);
                _physicsEngine.Register(_playerBody);
        }

        private void OnUpdate(double deltaTime)
        {
                float dt = (float)deltaTime;
                _input.Update();

                _camera.Look(_input.MouseDelta);

                Vector2 input = _input.GetMovementInput();
                Vector3 moveDir = Vector3.Zero;
                if (input.X != 0 || input.Y != 0)
                {
                        Vector3 forward = Vector3.Normalize(new Vector3(_camera.Front.X, 0, _camera.Front.Z));
                        Vector3 right = Vector3.Normalize(new Vector3(_camera.Right.X, 0, _camera.Right.Z));
                        moveDir = Vector3.Normalize(forward * input.Y + right * input.X);
                }

                if (moveDir != Vector3.Zero)
                {
                        _playerBody.ApplyForce(moveDir * _walkSpeed);
                }

                // jumping
                if (_input.IsKeyPressed(Key.Space) && _playerBody.Physics.IsGrounded)
                {
                        var state = _playerBody.Physics;
                        state.Velocity.Y = _jumpForce;
                        _playerBody.Physics = state;
                }

                // physics step, check for collisions with objects
                _physicsEngine.Update(dt, _drawService.GetStaticObjects());

                // attach the camera to the player
                _camera.Position = _playerBody.Position + new Vector3(0, 0.75f, 0);

                if (_input.IsKeyPressed(Key.Escape))
                {
                        _window.Close();
                }
                _window.Title = $"Pos: {_playerBody.Position.Y:F2} | Vel: {_playerBody.Physics.Velocity.Y:F2} | G: {_playerBody.Physics.IsGrounded}";
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