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
        _icon1.sprite = Game.Data.Vault.ResourceIcons[node.From.Type];

        _cost2.text = node.To.Amount.ToString();
        _icon2.sprite = Game.Data.Vault.ResourceIcons[node.To.Type];

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
        Player.OnExchangeResources?.Invoke(Player.Construction.Construct.Name, _node.From, _node.To);
    }
}