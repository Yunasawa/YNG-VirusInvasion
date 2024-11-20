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
    [SerializeField] private TextMeshProUGUI _buttonText;

    public void Initialize(ExchangerStatsNode node)
    {
        _node = node;

        _cost1.text = node.From.Amount.ToString();
        _cost2.text = node.To.Amount.ToString();
        _button.onClick.AddListener(OnButtonClicked);
        if (Game.Data.PlayerStats.Resources[node.From.Type] >= node.From.Amount)
        {
            _button.interactable = true;
            _buttonText.color = Color.white;
        }
        else
        {
            _button.interactable = false;
            _buttonText.color = Color.red;
        }
    }

    private void OnButtonClicked()
    {
        Player.OnExchangeResources?.Invoke(_node.From, _node.To);
    }
}