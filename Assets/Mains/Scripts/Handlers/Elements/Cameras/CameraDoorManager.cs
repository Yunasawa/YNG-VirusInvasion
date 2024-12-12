using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
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
        DoorGroup doorGroup = _doors.Find(i => i.Stage == Game.Data.PlayerStats.CurrentLevel);
        Camera.main.DOFieldOfView(110f, 1);

        foreach (var door in doorGroup.Doors)
        {
            _manager.Movement.Target = door.transform;
            await UniTask.WaitForSeconds(5);
        }

        Camera.main.DOFieldOfView(50f, 1);
        _manager.Movement.Target = Player.Transform;
    }
}