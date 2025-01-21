using System;
using TMPro;
using UnityEngine;
using YNL.Extensions.Methods;

public class PlayerConstructionManager : ColliderTriggerListener
{
    public ConstructManager Construct;

    [SerializeField] private Transform _homeBase;
    [SerializeField] private Transform _homeBasePointer;
    [SerializeField] private TextMeshPro _homeDistance;

    private void Update()
    {
        PointToHomeBase();
    }

    private void PointToHomeBase()
    {
        Vector3 direction = _homeBase.position - transform.position;

        _homeBasePointer.rotation = Quaternion.LookRotation(direction, Vector3.up);

        _homeDistance.text = $"{Mathf.RoundToInt(direction.magnitude) - 10}m";
        _homeDistance.transform.localRotation = Quaternion.Euler(0, 0, _homeBasePointer.rotation.eulerAngles.y);
        float zRotation = _homeDistance.transform.eulerAngles.z;
        float zRotationRadians = Mathf.Deg2Rad * zRotation;
        float xPosition = -Mathf.Sin(zRotationRadians);
        float yPosition = Mathf.Cos(zRotationRadians);
        _homeDistance.transform.localPosition = new Vector3(xPosition, yPosition, 0) * 3.25f;
    }

    public override void OnColliderTriggerEnter(Collider other)
    {
        if (other.tag.IsStringEqualToEnum(out ConstructType type))
        {
            ConstructManager manager = other.GetComponent<ConstructManager>();
            if (manager.Type != ConstructType.Base) manager.UI.SetActive(true);

            if (manager.Type == ConstructType.Base)
            {
                Player.OnEnterHomeBase?.Invoke();

                Game.IsReturnToBaseTutorialActivated = true;
            }
            else if (manager.Type == ConstructType.Quest)
            {
                QuestConstruct questConstruct = manager.GetComponent<QuestConstruct>();
                questConstruct?.QuestUI.gameObject.SetActive(true);
                questConstruct?.QuestUI.OnUpdateQuestStatus(questConstruct.QuestName);
            }

            InteractWith(true, manager.Type, manager);
        }
    }

    public override void OnColliderTriggerExit(Collider other)
    {
        if (other.tag.IsStringEqualToEnum(out ConstructType type))
        {
            ConstructManager manager = other.GetComponent<ConstructManager>();
            if (manager.Type != ConstructType.Base) manager.UI.SetActive(false);

            InteractWith(false, manager.Type, manager);
        }
    }

    public void InteractWith(bool isEnter, ConstructType type, ConstructManager manager)
    {
        Construct = isEnter ? manager : null;
        Player.OnInteractWithConstruct?.Invoke(isEnter, type, manager);
    }
}

public static class EnumExtensions
{
    public static bool IsStringEqualToEnum<T>(this string str, out T result) where T : struct, Enum
    {
        if (Enum.TryParse(str, true, out result) && Enum.IsDefined(typeof(T), result))
        {
            return true;
        }

        result = default;
        return false;
    }
}
