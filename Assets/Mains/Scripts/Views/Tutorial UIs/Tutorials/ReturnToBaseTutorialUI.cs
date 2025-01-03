using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ReturnToBaseTutorialUI : TutorialWindowUI
{
    [SerializeField] private Transform _tutorialPanel;
    [SerializeField] private TextMeshProUGUI _messageText;
    private int _currentVisibleCharacterIndex = 0;

    [SerializeField] private Transform _homeBase;
    [SerializeField] private Transform _locationRing;

    public override void ShowTutorial()
    {
        if (_isTutorialActivated) return;

        _isTutorialActivated = true;

        _tutorialPanel.gameObject.SetActive(true);
        _tutorialPanel.DOScale(Vector3.one, 1).SetEase(Ease.OutCubic);

        _messageText.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;

        TypeWriter().Forget();
        CameraManager.Instance.Door.FocusOnTarget(_homeBase.position, 1, 3).Forget();
    }

    private async UniTaskVoid TypeWriter()
    {
        TMP_TextInfo textInfo = _messageText.textInfo;
        while (_currentVisibleCharacterIndex < _messageText.text.Length)
        {
            _messageText.maxVisibleCharacters++;
            await UniTask.Delay(Key.Config.TypeWritterDelay);
            _currentVisibleCharacterIndex++;
        }

        await UniTask.WaitForSeconds(3);
        _tutorialPanel.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            _tutorialPanel.gameObject.SetActive(false);
        }).SetEase(Ease.OutCubic);

        await UniTask.Delay(500);

        _locationRing.DOScale(Vector3.one, 1);
    }
}