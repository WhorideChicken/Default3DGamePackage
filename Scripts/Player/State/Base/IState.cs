public interface IState<T>
{
    void OnEnter(T ctx);
    void OnUpdate(T ctx);
    void OnExit(T ctx);
}