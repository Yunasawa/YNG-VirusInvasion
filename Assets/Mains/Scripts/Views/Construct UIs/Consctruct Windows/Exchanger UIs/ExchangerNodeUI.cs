using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class ExchangerNodeUI : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private ConstructStats _constructionStat => Game.Data.ConstructStats;
    private ExchangerStatsNode _node;

    [SerializeField] private TextMeshProUGUI _cost1;
    [SerializeField] private Image _icon1;
    [SerializeField] private TextMeshProUGUI _cost2;
    [SerializeField] private Image _icon2;
    [SerializeField] private Button _button;

    public void Initialize(ExchangerStatsNode node)
    {
        _node = node;

        _cost1.text = node.From.Amount.ToString();
        _cost2.text = node.To.Amount.ToString();
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {

    }
}