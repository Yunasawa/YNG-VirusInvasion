using System;
using UnityEngine;
using YNL.Extensions.Methods;

public class PlayerConstructionManager : ColliderTriggerListener
{
    public override void OnColliderTriggerEnter(Collider other)
    {
        if (other.tag.IsStringEqualToEnum<ConstructType>(out ConstructType type))
        {
            if (Vector3.Distance(other.transform.position, Player.Transform.position) > 7) return;

            ConstructManager manager = other.GetComponent<ConstructManager>();
            InteractWith(true, ConstructType.Base, manager);
        }
    }

    public override void OnColliderTriggerStay(Collider other)
    {
        OnColliderTriggerEnter(other);
    }

    public override void OnColliderTriggerExit(Collider other)
    {
        if (other.tag.IsStringEqualToEnum<ConstructType>(out ConstructType type))
        {
            ConstructManager manager = other.GetComponent<ConstructManager>();
            InteractWith(false, ConstructType.Base, manager);
        }
    }

    public void InteractWith(bool isEnter, ConstructType type, ConstructManager manager)
    {
        Player.OnInteractWithConstruct?.Invoke(isEnter, type);
        MDebug.Log(manager.Name);
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
