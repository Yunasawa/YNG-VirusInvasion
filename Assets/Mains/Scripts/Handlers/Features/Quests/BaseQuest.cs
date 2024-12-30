using YNL.Extensions.Methods;

public abstract class BaseQuest
{
    public bool IsCompleted = false;

    public abstract void OnAcceptQuest();
    public abstract void OnCompleteQuest();

    public abstract string GetProgress();
}