using UnityEngine;
using YNL.Patterns.Singletons;

public class GameManager : Singleton<GameManager>
{
    public GameInput Input;
    public GameData Data;
    public GameLoader Loader;

    public bool IsMobileDevice
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android) return true;
            else return true;
        }
    }

    protected override void Awake()
    {
        Application.targetFrameRate = 60;

        Data.MonoAwake();
        Loader.MonoAwake();
    }

    private void Start()
    {
    }
}