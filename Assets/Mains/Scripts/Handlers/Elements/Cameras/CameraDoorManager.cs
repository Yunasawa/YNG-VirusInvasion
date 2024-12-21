using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

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
        Camera.main.transform.DOLocalMove(new Vector3(0, 85, -52), 2).SetEase(ease);//.DOFieldOfView(110f, 2).SetEase(ease);
        _manager.Movement.EnableFollowTarget = false;

        foreach (var door in doorGroup.Doors)
        {
            MDebug.Log("HEHEHE");
            this.transform.DOMove(door.transform.position, 2).SetEase(ease).OnComplete(door.OpenDoor);
            await UniTask.WaitForSeconds(5);
        }

        Camera.main.transform.DOLocalMove(new Vector3(0, 30, -20), 2).SetEase(ease); //DOFieldOfView(50f, 2).SetEase(ease);
        this.transform.DOMove(Player.Transform.position, 2).SetEase(ease).OnComplete(OnBackToPlayerCompleted);

        void OnBackToPlayerCompleted()
        {
            _manager.Movement.EnableFollowTarget = true;
        }
    }
}