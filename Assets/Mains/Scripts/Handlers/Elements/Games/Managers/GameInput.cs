using Sirenix.OdinInspector;
using UnityEngine;

[AddComponentMenu("Game: Input")]
public class GameInput : MonoBehaviour
{
    public Joystick MovementJoystick;

    //public TutorialPanelUI TutorialPanelUI;
    public FarmConstruct FoodFarm2;

    public Transform HomeBase;

    [Title("Mini Cell Prefabs")]
    public DeliveryCell DeliveryCell;
    public HunterCell HunterCell;
}