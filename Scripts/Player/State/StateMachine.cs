public sealed class StateMachine<T>
{
    public IState<T> Current { get; private set; }

    public void ChangeState(T ctx, IState<T> next)
    {
        if (next == null || next == Current) return;
        Current?.OnExit(ctx);
        Current = next;
        Current.OnEnter(ctx);
    }

    public void Update(T ctx) => Current?.OnUpdate(ctx);
}