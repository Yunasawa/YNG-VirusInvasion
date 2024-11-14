using UnityEngine;

public abstract class ColliderTriggerListener : MonoBehaviour
{
    public virtual void OnColliderTriggerEnter(Collider other) { }
    public virtual void OnColliderTriggerStay(Collider other) { }
    public virtual void OnColliderTriggerExit(Collider other) { }
}