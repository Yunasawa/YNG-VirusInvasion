using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class HealthFieldUI : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;

    [SerializeField] private Image _healthFill;
    [SerializeField] private Image _healthFade;
    [SerializeField] private TextMeshProUGUI _healthText;

    public void UpdateHealth()
    {
        _healthFill.fillAmount = _playerStats.RatioHP;
        _healthText.text = $"HP ({_playerStats.CurrentHP}/{_playerStats.MaxHP})";

        _healthFade.DOFillAmount(_playerStats.RatioHP, 1).SetEase(Ease.OutExpo);
    }
}