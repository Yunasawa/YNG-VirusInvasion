using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class HealthFieldUI : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;

    [SerializeField] private Image _healthFill;
    [SerializeField] private TextMeshProUGUI _healthText;

    public void UpdateHealth()
    {
        _healthFill.fillAmount = (float)_playerStats.CurrentHP / _playerStats.MaxHP;
        _healthText.text = $"HP ({_playerStats.CurrentHP}/{_playerStats.MaxHP})";
    }
}