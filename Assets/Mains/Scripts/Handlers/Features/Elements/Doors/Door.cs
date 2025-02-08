using DG.Tweening;
using UnityEngine;
using YNL.Extensions.Methods;

public class Door : MonoBehaviour
{
    public StageType Stage1;
    public StageType Stage2;

    public int OpenLevel;

    [SerializeField] private Transform _doorBarrier;

    private void Start()
    {
        if (Game.Data.PlayerStats.CurrentLevel >= (int)Stage2 && this.GetComponent<Lock>().IsNull()) LowDoor();
    }

    public void OpenDoor()
    {
        _doorBarrier.DOLocalMove(_doorBarrier.localPosition.SetY(-11), 2).SetEase(Ease.Linear);
    }

    public void LowDoor()
    {
        _doorBarrier.localPosition = _doorBarrier.localPosition.SetY(-11);
    }
}