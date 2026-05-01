namespace CreateGame.Engine;

public class Signal
{
    private readonly List<Action> _listeners = [];

    public void Connect(Action callback) => _listeners.Add(callback);

    internal void Fire()
    {
        foreach (var callback in _listeners) callback();
    }
}

public class Signal<T>
{
    private readonly List<Action<T>> _listeners = [];

    public void Connect(Action<T> callback) => _listeners.Add(callback);

    internal void Fire(T arg)
    {
        foreach (var callback in _listeners) callback(arg);
    }
}