using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHider : MonoBehaviour
{
    public List<GameObject> Objects = new();
    public List<Image> Images = new();
    public List<TextMeshProUGUI> Texts = new();

    private Dictionary<Image, Color> _imageColors = new();
    private Dictionary<TextMeshProUGUI, Color> _textColors = new();

    [SerializeField] private Button _hiderButton;
    private bool _isHidden = false;

    private void Awake()
    {
        _hiderButton.onClick.AddListener(Hide);

        AssignColor();
    }

    private void AssignColor()
    {
        _imageColors.Clear();
        _textColors.Clear();

        foreach (var image in Images) _imageColors.Add(image, image.color);
        foreach (var text in Texts) _textColors.Add(text, text.color);
    }

    private void Hide()
    {
        _isHidden = !_isHidden;

        foreach (var image in Images) image.color = _isHidden ? Color.clear : _imageColors[image];
        foreach (var text in Texts) text.color = _isHidden ? Color.clear : _textColors[text];
        foreach (var obj in Objects) obj.SetActive(!_isHidden);
    }
}