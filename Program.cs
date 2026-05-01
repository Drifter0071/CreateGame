using System.Numerics;
using Silk.NET.Input;
using CreateGame.Engine;
using CreateGame.Physics;

namespace CreateGame;

public class Program
{
    public static void Main()
    {
        var game = new GameWindow();

        // RenderStarted — runs once after the GL context and DrawServiceare ready.
        RunService.RenderStarted.Connect(() =>
        {
            // Test Baseplate
            var baseplate = new PrimitiveObject
            {
                Position = new Vector3(0, -5, 0),
                Size     = new Vector3(200, 10, 200),
                Color    = new Vector4(0.3f, 0.3f, 0.3f, 1.0f)
            };
            DrawService.Register(baseplate, true);

            // Test Wall
            var wall = new PrimitiveObject
            {
                Position = new Vector3(5, 2, 0),
                Size     = new Vector3(2, 5, 10),
                Color    = new Vector4(1.0f, 0.5f, 0.0f, 1.0f)
            };
            DrawService.Register(wall, true);

            // Test Spinner
            var spinner = new PrimitiveObject
            {
                Position = new Vector3(0, 3, -5),
                Size     = new Vector3(2, 2, 2),
                Color    = new Vector4(0.0f, 0.9f, 0.2f, 1.0f)
            };
            DrawService.Register(spinner, true);

            // Player body (invisible — camera sits inside it)
            var playerBody = new RigidBody3D
            {
                Position  = new Vector3(0, 5, 0),
                Size      = new Vector3(0.5f, 1.8f, 0.5f),
                Mass      = 1.0f,
                Drag      = 8.0f,
                IsVisible = false
            };
            DrawService.Register(playerBody, false);
            PhysicsEngine.Register(playerBody);

            // RenderStepped — runs every frame with delta-time.
            // Capture playerBody via closure so this lambda can mutate it.
            const float jumpForce = 25.0f;
            const float walkSpeed = 50.0f;

            float angle = 0f;
            RunService.RenderStepped.Connect(dt =>
            {
                // Spin the spinner
                angle += dt;
                spinner.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, angle)
                    * Quaternion.CreateFromAxisAngle(Vector3.UnitX, angle * 0.6f);

                // -- Movement input --
                Vector2 input   = game.Input.GetMovementInput();
                Vector3 moveDir = Vector3.Zero;

                if (input.X != 0 || input.Y != 0)
                {
                    Vector3 forward = Vector3.Normalize(new Vector3(game.Camera.Front.X, 0, game.Camera.Front.Z));
                    Vector3 right   = Vector3.Normalize(new Vector3(game.Camera.Right.X, 0, game.Camera.Right.Z));
                    moveDir = Vector3.Normalize(forward * input.Y + right * input.X);
                }

                if (moveDir != Vector3.Zero)
                    playerBody.ApplyForce(moveDir * walkSpeed);

                // -- Jump --
                if (game.Input.IsKeyPressed(Key.Space) && playerBody.Physics.IsGrounded)
                {
                    var state = playerBody.Physics;
                    state.Velocity.Y = jumpForce;
                    playerBody.Physics = state;
                }

                PhysicsEngine.Update(dt, DrawService.GetStaticObjects());

                game.Camera.Position = playerBody.Position + new Vector3(0, 0.75f, 0);

                if (game.Input.IsKeyPressed(Key.Escape))
                    game.Window.Close();

                // Debug title
                game.Window.Title = $"Pos: {playerBody.Position.Y:F2} | Vel: {playerBody.Physics.Velocity.Y:F2} | G: {playerBody.Physics.IsGrounded}";
            });
        });

        game.Run();
    }
}