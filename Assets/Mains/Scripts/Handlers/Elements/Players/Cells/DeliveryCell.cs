using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class DeliveryCell : MonoBehaviour
{
    public Transform Target;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    [Button]
    public void Follow()
    {
        _agent.SetDestination(Target.position);
    }
}