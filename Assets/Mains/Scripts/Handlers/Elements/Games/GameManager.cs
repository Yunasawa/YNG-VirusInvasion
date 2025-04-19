using SupersonicWisdomSDK;
using UnityEngine;
using YNL.Patterns.Singletons;

public class GameManager : Singleton<GameManager>
{
    public GameInput Input;
    public GameData Data;
    public GameLoader Loader;
    public GameEnemy Enemy;

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
        base.Awake();

        Application.targetFrameRate = 60;

        SupersonicWisdom.Api.AddOnReadyListener(Start);
        SupersonicWisdom.Api.Initialize();
    }

    private void Start()
    {
        Data.MonoAwake();
        Loader.MonoAwake();
        Enemy.MonoAwake();

        Game.OnStart?.Invoke();
    }
}