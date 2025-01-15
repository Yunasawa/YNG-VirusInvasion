using YNL.Extensions.Methods;

public abstract class BaseQuest
{
    public bool IsCompleted => Current >= _target;
    protected int _target;
    public float Current;
    public float Ratio => Current / _target;

    public abstract void Initialize();
    public abstract void Refresh(float current);

    public abstract void OnAcceptQuest();
    public abstract void OnCompleteQuest();

    public abstract string GetProgress();
}