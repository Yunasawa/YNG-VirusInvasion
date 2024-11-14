using UnityEngine;

public class ColliderTriggerDetector : MonoBehaviour
{
    public ColliderTriggerListener[] Listeners = new ColliderTriggerListener[0];

    private void OnTriggerEnter(Collider other)
    {
        foreach (var listener in Listeners) listener.OnColliderTriggerEnter(other);
    }
    private void OnTriggerStay(Collider other)
    {
        foreach (var listener in Listeners) listener.OnColliderTriggerStay(other);
    }
    private void OnTriggerExit(Collider other)
    {
        foreach (var listener in Listeners) listener.OnColliderTriggerExit(other);
    }
}