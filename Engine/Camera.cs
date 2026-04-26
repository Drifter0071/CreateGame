using System.Numerics;

namespace CreateGame.Engine;

public class Camera
{
    public Vector3 Position { get; set; }
    public Vector3 Front { get; private set; }
    public Vector3 Up { get; private set; }
    public Vector3 Right { get; private set; }

    public float Yaw { get; set; } = -90f;
    public float Pitch { get; set; } = 0f;
    public float Speed { get; set; } = 15f;
    public float Sensitivity { get; set; } = 0.1f;

    public Camera(Vector3 startPosition)
    {
        Position = startPosition;
        UpdateVectors();
    }

    // view matrix
    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(Position, Position+Front, Up);
    }

    public void Move(Vector2 inputDirection, float deltaTime)
    {
        float velocity = Speed * deltaTime;
        // forward, back
        Position += Front * inputDirection.Y * velocity;
        // left/right
        Position += Right * inputDirection.X * velocity;
    }

    public void Look(Vector2 mouseDelta)
    {
        Yaw += mouseDelta.X * Sensitivity;
        Pitch -= mouseDelta.Y * Sensitivity;

        Pitch = Math.Clamp(Pitch, -89.0f, 89f);
        UpdateVectors();
    }

    private void UpdateVectors()
    {
        Front = Vector3.Normalize(new Vector3(
            MathF.Cos(Yaw * (MathF.PI / 180f)) * MathF.Cos(Pitch * (MathF.PI / 180f)),
            MathF.Sin(Pitch * (MathF.PI / 180f)),
            MathF.Sin(Yaw * (MathF.PI / 180f)) * MathF.Cos(Pitch * (MathF.PI / 180f))
        ));

        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
}