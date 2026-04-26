using Silk.NET.Input;
using System.Numerics;

namespace CreateGame.Engine;

public class Input
{
    private readonly IKeyboard _primaryKeyboard;
    private readonly IMouse _primaryMouse;
    private Vector2 _lastMousePosition;
    private bool _firstMove = true;

    public Vector2 MouseDelta { get; private set; }

    public Input(IInputContext context)
    {
        _primaryKeyboard = context.Keyboards[0];
        _primaryMouse = context.Mice[0];

        _primaryMouse.Cursor.CursorMode = CursorMode.Raw;
    }

    public void Update()
    {
        var currentPos = _primaryMouse.Position;
        if (_firstMove)
        {
            _lastMousePosition = currentPos;
            _firstMove = false;
        }

        MouseDelta = new Vector2(currentPos.X - _lastMousePosition.X, currentPos.Y - _lastMousePosition.Y);
        _lastMousePosition = currentPos;
    }

    public Vector2 GetMovementInput()
    {
        float x = 0, y = 0;
        if (_primaryKeyboard.IsKeyPressed(Key.W)) y += 1;
        if (_primaryKeyboard.IsKeyPressed(Key.S)) y -= 1;
        if (_primaryKeyboard.IsKeyPressed(Key.A)) x -= 1;
        if (_primaryKeyboard.IsKeyPressed(Key.D)) x += 1;
        return new Vector2(x, y);
    }

    public bool IsKeyPressed(Key key) => _primaryKeyboard.IsKeyPressed(key);
}