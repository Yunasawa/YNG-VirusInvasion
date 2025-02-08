using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class Lock : MonoBehaviour
{
    [SerializeField] private GameObject _lockUI;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    private Door _door;
    [SerializeField] private List<ResourcesInfo> _requirements = new();

    private void Awake()
    {
        _door = GetComponent<Door>();

        _button.onClick.AddListener(Unlock);
        _text.text = GetRequirementText();
    }

    private string GetRequirementText()
    {
        string text = "";

        foreach (var requirement in _requirements)
        {
            text += $"{requirement.Amount}<sprite name={requirement.Type}> ";
        }

        return text;
    }

    private void Unlock()
    {
        bool meetRequirement = true;

        foreach (var requirement in _requirements)
        {
            if (Game.Data.PlayerStats.Resources[requirement.Type] < requirement.Amount)
            {
                meetRequirement = false;
                break;
            }
        }

        if (!meetRequirement) return;

        foreach (var requirement in _requirements)
        {
            Player.OnConsumeResources?.Invoke(requirement.Type, requirement.Amount);
        }

        _door.OpenDoor();
        _lockUI.SetActive(false);
    }
}