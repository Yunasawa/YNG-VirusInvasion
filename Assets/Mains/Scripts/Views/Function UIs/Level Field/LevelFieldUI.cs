using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelFieldUI : MonoBehaviour
{
    [SerializeField] private Image _expBar;
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private TextMeshProUGUI _levelText;

    private void Awake()
    {
        View.OnChangeLevelField += OnChangeLevelField;
    }

    private void OnDestroy()
    {
        View.OnChangeLevelField -= OnChangeLevelField;
    }

    private void Start()
    {
        OnChangeLevelField();
    }

    private void OnChangeLevelField()
    {
        _expBar.fillAmount = (float)Game.Data.PlayerStats.CurrentExp / Formula.Level.GetMaxExp();
        _expText.text = $"{Game.Data.PlayerStats.CurrentExp.RoundValue()}/{Formula.Level.GetMaxExp().RoundValue()}";
        _levelText.text = $"{Game.Data.PlayerStats.CurrentLevel}";
    }
}