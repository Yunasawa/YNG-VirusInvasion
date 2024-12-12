using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class QuestBillboardUI : MonoBehaviour
{
    [SerializeField] private Button _questButton;

    [SerializeField] private GameObject _questAccept;
    [SerializeField] private GameObject _progressArea;
    [SerializeField] private TextMeshProUGUI _progressText;

    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private GameObject _checkMark;

    private void Awake()
    {
        _questButton.onClick.AddListener(() => MDebug.Log("Hello"));
    }
}