using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using YNL.Extensions.Methods;

public class CameraDoorManager : MonoBehaviour
{
    [System.Serializable]
    public class DoorGroup
    {
        public int Stage;
        public Door[] Doors = new Door[0];
    }

    [SerializeField] private CameraManager _manager;
    [SerializeField] private List<DoorGroup> _doors = new();

    private void Awake()
    {
        Player.OnLevelUp += FocusDoorOnLevelUp;
    }

    private void OnDestroy()
    {
        Player.OnLevelUp -= FocusDoorOnLevelUp;
    }

    private void FocusDoorOnLevelUp()
    {
        FocusOnDoor().Forget();
    }

    private async UniTaskVoid FocusOnDoor()
    {
        Ease ease = Ease.OutCubic;

        DoorGroup doorGroup = _doors.Find(i => i.Stage == Game.Data.PlayerStats.CurrentLevel);
        Camera.main.transform.DOLocalMove(new Vector3(0, 85, -52), 2).SetEase(ease);
        _manager.Movement.EnableFollowTarget = false;

        if (!doorGroup.IsNull())
        {
            foreach (var door in doorGroup.Doors)
            {
                this.transform.DOMove(door.transform.position, 2).SetEase(ease).OnComplete(door.OpenDoor);
                await UniTask.WaitForSeconds(5);
            }
        }

        Camera.main.transform.DOLocalMove(new Vector3(0, 30, -20), 2).SetEase(ease);
        this.transform.DOMove(Player.Transform.position, 2).SetEase(ease).OnComplete(OnBackToPlayerCompleted);

        void OnBackToPlayerCompleted()
        {
            _manager.Movement.EnableFollowTarget = true;
        }
    }
    public async UniTaskVoid FocusOnTarget(Vector3 target, float time, float wait = 1)
    {
        Ease ease = Ease.OutCubic;

        this.transform.DOMove(target, time).SetEase(ease);
        Camera.main.transform.DOLocalMove(new Vector3(0, 85, -52), time).SetEase(ease);

        _manager.Movement.EnableFollowTarget = false;

        await UniTask.WaitForSeconds(wait);

        Camera.main.transform.DOLocalMove(new Vector3(0, 30, -20), time).SetEase(ease);
        this.transform.DOLocalMove(Player.Transform.position, time).SetEase(ease).OnComplete(() =>
        {
            _manager.Movement.EnableFollowTarget = true;
        });
    }
}