using YNL.Extensions.Methods;

public abstract class BaseQuest
{
    public bool IsCompleted = false;

    public abstract void Initialize(bool isCompleted, int target, int current);

    public abstract void OnAcceptQuest();
    public abstract void OnCompleteQuest();

    public abstract string GetProgress();
    public abstract (int Current, int Target) GetSerializeProgress();
}