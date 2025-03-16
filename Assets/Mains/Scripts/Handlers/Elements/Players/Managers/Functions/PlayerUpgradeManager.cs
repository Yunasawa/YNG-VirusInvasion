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
        UpdateModel();
        UpdateColor();
        if (_currentRevolution < 2) _currentRevolution++;
        else _currentRevolution = 0;

            void UpdateModel()
            {
                RevolutionUpgrade[_currentRevolution].SetActive(false);
                RevolutionUpgrade[_currentRevolution + 1].SetActive(true);
            }

        void UpdateColor()
        {
            ApplyColor(RevolutionColors[_currentRevolution + 1]);

            void ApplyColor(UpgradeColor color)
            {
                Material.SetColor("_BaseColor", color.BaseColor);
                Material.SetColor("_ColorDim", color.ShadedColor);
                Material.SetColor("_FlatSpecularColor", color.SpecularColor);
            }
        }
    }
}