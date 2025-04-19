using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class PlayerUpgradeManager : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeColor
    {
        public Color BaseColor = Color.white;
        public Color ShadedColor = Color.white;
        public Color SpecularColor = Color.white;
    }

    private int _currentRevolution
    {
        get => Game.Data.PlayerStats.CurrentRevolution;
        set => Game.Data.PlayerStats.CurrentRevolution = value;
    }
    private int _previousRevolution = 0;

    public bool Enabled;

    public SerializableDictionary<int, GameObject> RevolutionUpgrade = new();

    public Material Material;
    public SerializableDictionary<int, UpgradeColor> RevolutionColors = new();

    private void Awake()
    {
        Player.OnUpgradeRevolution += OnUpgradeRevolution;
    }

    private void OnDestroy()
    {
        Player.OnUpgradeRevolution -= OnUpgradeRevolution;
    }

    private void Start()
    {
        _previousRevolution = _currentRevolution = 0;
        OnUpgradeRevolution();
    }


    [Button]
    public void Change()
    {
        var color = RevolutionColors[0];

        Material.SetColor("_BaseColor", color.BaseColor);
        Material.SetColor("_ColorDim", color.ShadedColor);
        Material.SetColor("_FlatSpecularColor", color.SpecularColor);
    }

    private void OnUpgradeRevolution()
    {
        if (!Enabled) return;

        if (_currentRevolution >= 2)
        {
            _currentRevolution = 0;
            _previousRevolution = 2;
        }
        else
        {
            _previousRevolution = _currentRevolution;
            _currentRevolution++;
        }

            UpdateModel();
        UpdateColor();

        void UpdateModel()
        {
            RevolutionUpgrade[_previousRevolution].SetActive(false);
            RevolutionUpgrade[_currentRevolution].SetActive(true);
        }

        void UpdateColor()
        {
            ApplyColor(RevolutionColors[_currentRevolution]);

            void ApplyColor(UpgradeColor color)
            {
                Material.SetColor("_BaseColor", color.BaseColor);
                Material.SetColor("_ColorDim", color.ShadedColor);
                Material.SetColor("_FlatSpecularColor", color.SpecularColor);
            }
        }
    }
}