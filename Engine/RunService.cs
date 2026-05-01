namespace CreateGame.Engine;

public static class RunService
{
    // fires when the scene is ready to be populated
    public static readonly Signal RenderStarted = new();

    // fires every frame: parameter is delta-time in seconds
    public static readonly Signal<float> RenderStepped = new();
}