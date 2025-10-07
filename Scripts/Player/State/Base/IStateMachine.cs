public interface IStateMachine<TState>
{
    TState CurrentKey { get; }
    bool HasCurrent { get; }
    void Add(TState key, IState<TState> state);
    void Change(TState key);
    void OnUpdate(float dt);
}