using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TutorialPanelUI : MonoBehaviour
{
    public TutorialWindowUI StartGameTutorial;
    public TutorialWindowUI ReturnToBaseTutorial;
}

public abstract class TutorialWindowUI : MonoBehaviour
{
    protected bool _isTutorialActivated = false;

    public abstract void ShowTutorial();
}