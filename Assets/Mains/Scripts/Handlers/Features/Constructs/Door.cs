using DG.Tweening;
using UnityEngine;
using YNL.Extensions.Methods;

public class Door : MonoBehaviour
{
    public StageType Stage1;
    public StageType Stage2;

    public int OpenLevel;

    [SerializeField] private Transform _doorBarrier;

    public void OpenDoor()
    {
        _doorBarrier.DOLocalMove(_doorBarrier.localPosition.SetY(-11), 2).SetEase(Ease.Linear);
    }
}