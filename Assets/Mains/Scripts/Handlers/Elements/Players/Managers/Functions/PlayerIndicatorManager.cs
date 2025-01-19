using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class PlayerIndicatorManager : MonoBehaviour
{
    [System.Serializable]
    private class IndicatorGroup
    {
        public GameObject Container;
        public List<IndicatorPair> Pairs = new();
    }

    [System.Serializable]
    public class IndicatorPair
    {
        public GameObject Indicator;
        public GameObject Target;
        public TextMeshPro Text;
    }

    [SerializeField] private SerializableDictionary<StageType, IndicatorGroup> _stageIndicators = new();

    private IndicatorGroup _indicatorGroup;

    private void Awake()
    {
        Player.OnEnterStage += OnEnterStage;
    }

    private void OnDestroy()
    {
        Player.OnEnterStage -= OnEnterStage;
    }

    private void Start()
    {
        foreach (var group in _stageIndicators)
        {
            if (group.Key != Player.Stage.CurrentStage) group.Value.Container.SetActive(false);
        }
    }

    private void Update()
    {
        if (_indicatorGroup.IsNull()) return;

        foreach (var pair in _indicatorGroup.Pairs)
        {
            PointToHomeBase(pair.Indicator.transform, pair.Target.transform, pair.Text);
        }
    }

    private void OnEnterStage(StageType previous, StageType current)
    {
        if (_stageIndicators.ContainsKey(previous)) _stageIndicators[previous].Container.SetActive(false);
        if (_stageIndicators.ContainsKey(current))
        {
            _stageIndicators[current].Container.SetActive(true);

            _indicatorGroup = _stageIndicators[current];
        }
    }

    private void PointToHomeBase(Transform indicator, Transform target, TextMeshPro text)
    {
        Vector3 direction = target.position - transform.position;

        indicator.rotation = Quaternion.LookRotation(direction, Vector3.up);

        text.text = $"{Mathf.RoundToInt(direction.magnitude) - 5}m";
        text.transform.localRotation = Quaternion.Euler(0, 0, indicator.rotation.eulerAngles.y);
        float zRotation = text.transform.eulerAngles.z;
        float zRotationRadians = Mathf.Deg2Rad * zRotation;
        float xPosition = -Mathf.Sin(zRotationRadians);
        float yPosition = Mathf.Cos(zRotationRadians);
        text.transform.localPosition = new Vector3(xPosition, yPosition, 0) * 3.25f;
    }

    [Button]
    public void GetText()
    {
        foreach (var group in _stageIndicators)
        {
            foreach (var pair in group.Value.Pairs)
            {
                TextMeshPro text = pair.Indicator.GetComponentInChildren<TextMeshPro>();
                pair.Text = text;
            }
        }
    }
}