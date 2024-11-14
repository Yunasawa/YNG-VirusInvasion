using UnityEngine;
using YNL.Extensions.Methods;

public class PlayerInteractingManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player.Enemy.OnInteractionEnter(other);
        Player.Construction.OnInteractionEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Player.Enemy.OnInteractionExit(other);
        Player.Construction.OnInteractionExit(other);
    }
}