using UnityEngine;
using YNL.Extensions.Methods;

public class CharacterWindowUI : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;

    [SerializeField] private Transform _resourceContainer;
    [SerializeField] private ResourceNodeUI _resourceNode;

    private void Awake()
    {
        _enemy = transform.parent.GetComponent<Enemy>();
    }

    private void Start()
    {
        CreateResourceNodes();
    }

    private void CreateResourceNodes()
    {
        _resourceContainer.DestroyAllChildren();

        foreach (var drop in _enemy.Stats.Stats.Drops)
        {
            ResourceNodeUI node = Instantiate(_resourceNode, _resourceContainer);
            node.UpdateNode(drop.Value);
            node.UpdateIcon(Game.Data.Vault.ResourceIcons[drop.Key]);
        }
    }
}