using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class ExchangerNodeUI : MonoBehaviour
{
    private ExchangerStatsNode _node;

    [SerializeField] private TextMeshProUGUI _cost1;
    [SerializeField] private Image _icon1;
    [SerializeField] private TextMeshProUGUI _cost2;
    [SerializeField] private Image _icon2;
    [SerializeField] private SwitchButton _button;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private bool _enoughResource;

    private void Awake()
    {
        Player.OnChangeResources += UpdateNode;
    }

    private void OnDestroy()
    {
        Player.OnChangeResources -= UpdateNode;
    }

    public void Initialize(ExchangerStatsNode node)
    {
        _node = node;

        _cost1.text = _node.From.Amount.ToString();
        _icon1.sprite = Game.Data.Vault.ResourceIcons[_node.From.Type];

        _cost2.text = _node.To.Amount.ToString();
        _icon2.sprite = Game.Data.Vault.ResourceIcons[_node.To.Type];

        UpdateNode();
    }

    private void UpdateNode()
    {
        _button.OnClick.AddListener(OnButtonClicked);
        _enoughResource = Game.Data.PlayerStats.Resources[_node.From.Type] >= _node.From.Amount;

        EnableInteraction();
    }

    private void EnableInteraction()
    {
        _buttonText.color = _enoughResource ? Color.white : Color.red;
        _button.Button.targetGraphic.color = _enoughResource ? Color.white : "#FFFFFF80".ToColor();
    }

    private void OnButtonClicked()
    {
        if (!_enoughResource) return;

        Player.OnExchangeResources?.Invoke(Player.Construction.Construct.Name, _node.From, _node.To);
    }
}