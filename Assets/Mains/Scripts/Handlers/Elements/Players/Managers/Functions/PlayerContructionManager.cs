using System;
using UnityEngine;
using YNL.Extensions.Methods;

public class PlayerConstructionManager : ColliderTriggerListener
{
    public ConstructManager Construct;

    public override void OnColliderTriggerEnter(Collider other)
    {
        if (other.tag.IsStringEqualToEnum(out ConstructType type))
        {
            ConstructManager manager = other.GetComponent<ConstructManager>();

            if (manager.Type == ConstructType.Base)
            {
                Player.OnEnterHomeBase?.Invoke();
            }

            InteractWith(true, manager.Type, manager);
        }
    }

    public override void OnColliderTriggerExit(Collider other)
    {
        if (other.tag.IsStringEqualToEnum(out ConstructType type))
        {
            ConstructManager manager = other.GetComponent<ConstructManager>();
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
