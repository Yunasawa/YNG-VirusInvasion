using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class UpgradeWindowUI : MonoBehaviour
{
    [SerializeField] private UpgradeFieldUI _upgradeField;
    [SerializeField] private Transform _upgradeContainer;
    private List<UpgradeFieldUI> _upgradeFields = new();

    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _resourceNodes = new();

    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        _closeButton.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
            View.OnCloseUpgradeWindow?.Invoke();
        });

        Player.OnChangeResources += UpdateResourceNodes;
    }

    private void OnDestroy()
    {
        Player.OnChangeResources -= UpdateResourceNodes;
    }

    private void Start()
    {
        CreateUpgradeFields();
        OnEnable();
    }

    private void OnEnable()
    {
        UpdateUpgradeFields();
        UpdateResourceNodes();
    }

    private void CreateUpgradeFields()
    {
        _upgradeContainer.DestroyAllChildren();

        foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
        {
            UpgradeFieldUI upgradeField = Instantiate(_upgradeField, _upgradeContainer);
            _upgradeFields.Add(upgradeField);

            upgradeField.Type = type;
        }
    }
    private void UpdateUpgradeFields()
    {
        foreach (var field in _upgradeFields)
        {
            field.UpdateField();
        }
    }

    private void UpdateResourceNodes()
    {
        foreach (var pair in _resourceNodes)
        {
            pair.Value.UpdateNode(Game.Data.PlayerStats.Resources[pair.Key]);
        }
    }
}