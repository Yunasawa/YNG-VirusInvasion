using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class QuestBillboardUI : MonoBehaviour
{
    [SerializeField] private string _questName;

    [SerializeField] private Button _questButton;

    [SerializeField] private GameObject _questAccept;
    [SerializeField] private GameObject _progressArea;
    [SerializeField] private TextMeshProUGUI _progressText;

    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private GameObject _checkMark;

    private void Awake()
    {
        _questButton.onClick.AddListener(OpenQuestWindow);

        Player.OnFinishQuest += OnFinishQuest;
    }

    private void OnDestroy()
    {
        Player.OnFinishQuest -= OnFinishQuest;
        
    }

    private void OpenQuestWindow()
    {
        Player.OnOpenQuestWindow?.Invoke(_questName, this);
    }
    
    public void AcceptQuest()
    {
        _questButton.onClick.RemoveListener(AcceptQuest);

        Game.Data.QuestRuntime.CurrentQuests.Add(_questName, false);
        Player.OnSendQuestUI?.Invoke(_questName, this);
        Player.OnAcceptQuest?.Invoke(_questName);

        _questAccept.SetActive(false);

        _progressArea.SetActive(true);
        _progressText.gameObject.SetActive(true);
    }

    public void ClaimReward()
    {
        ResourcesInfo info = Game.Data.QuestStats.Quests[_questName].Resource;
        Game.Data.PlayerStats.AdjustResources(info.Type, (int)info.Amount);
        Player.OnChangeResources?.Invoke();

        Player.OnCompleteQuest?.Invoke(_questName);
        Game.Data.QuestRuntime.CurrentQuests.Remove(_questName);

        this.gameObject.SetActive(false);
    }

    private void OnFinishQuest(string name)
    {
        if (name != _questName) return;

        _exclamationMark.gameObject.SetActive(false);
        _checkMark.gameObject.SetActive(true);

        _questButton.onClick.AddListener(ClaimReward);
    }

    public void SetProgressText(string text)
    {
        _progressText.text = text;
    }
}