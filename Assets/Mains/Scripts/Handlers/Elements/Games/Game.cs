using UnityEngine;
using UnityEngine.EventSystems;

public static class Game
{
    public static GameManager Manager => GameManager.Instance;
    public static GameInput Input => Manager.Input;

    public static Vector2 Resolution => new Vector2(Screen.width, Screen.height);
}