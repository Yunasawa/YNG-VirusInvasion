using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TutorialPanelUI : MonoBehaviour
{
    public TutorialWindowUI StartGameTutorial;
    public TutorialWindowUI ReturnToBaseTutorial;
    public TutorialWindowUI UpgradeAttributeTutorial;
}

public abstract class TutorialWindowUI : MonoBehaviour
{
    public abstract void ShowTutorial();
}