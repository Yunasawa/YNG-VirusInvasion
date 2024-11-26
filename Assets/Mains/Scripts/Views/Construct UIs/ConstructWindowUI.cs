using UnityEngine;

public abstract class ConstructWindowUI : MonoBehaviour
{
    public ConstructType Type;

    public abstract void OnOpenWindow();
    public virtual void OnCloseWindow() { }
}