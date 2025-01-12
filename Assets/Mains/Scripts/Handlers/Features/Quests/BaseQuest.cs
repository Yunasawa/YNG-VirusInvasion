using YNL.Extensions.Methods;

public abstract class BaseQuest
{
    public bool IsCompleted = false;
    protected int _target;
    public float Current;
    public float Ratio => Current / _target;

    public abstract void Initialize(bool isCompleted, float current);

    public abstract void OnAcceptQuest();
    public abstract void OnCompleteQuest();

    public abstract string GetProgress();
}