using UnityEngine;
using UnityEngine.EventSystems;

public static partial class Game
{
    public static GameManager Manager => GameManager.Instance;
    public static GameInput Input => Manager.Input;
    public static GameData Data => Manager.Data;

    public static Vector2 Resolution => new Vector2(Screen.width, Screen.height);
}