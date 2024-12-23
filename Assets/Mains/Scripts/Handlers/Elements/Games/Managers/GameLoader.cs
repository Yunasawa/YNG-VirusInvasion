using Cysharp.Threading.Tasks;
using UnityEngine;
using YNL.Extensions.Methods;

public class GameLoader : MonoBehaviour
{
    private SaveData _saveData;
    private string _fileName;

    public void MonoAwake()
    {
         LoadData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void LoadData()
    {
        _saveData = MJson.LoadNewtonJson<SaveData>(Key.Paths.SaveFile);

        if (_saveData.IsNull())
        {
            Game.Data.PlayerStats.Reset();
            SaveData();
            _saveData = MJson.LoadNewtonJson<SaveData>(Key.Paths.SaveFile);
        }

        Game.Data.PlayerStats.CurrentLevel = _saveData.Level.CurrentLevel;
        Game.Data.PlayerStats.CurrentExp = _saveData.Level.CurrentExp;
        Game.Data.PlayerStats.Resources = _saveData.Resources.Clone();
        Game.Data.PlayerStats.Bonuses = _saveData.Bonuses.Clone();
        Game.Data.PlayerStats.Levels = _saveData.Levels.Clone();
        Game.Data.PlayerStats.ExtraStatsLevel = _saveData.ExtraStatsLevel.Clone();
        Game.Data.PlayerStats.FarmStats = _saveData.FarmStats.Clone();

        Game.Data.CapacityStats.CurrentCapacity = _saveData.CurrentCapacity;
        Game.Data.CapacityStats.Resources = _saveData.CapacityResources.Clone();

        LoadDataDelay().Forget();
    }

    private async UniTaskVoid LoadDataDelay()
    {
        await UniTask.WaitForEndOfFrame(this);

        CameraManager.Instance.transform.position = _saveData.CurrentPosition.ToVector3();
        Player.Character.Move(_saveData.CurrentPosition.ToVector3());
    }

    private void SaveData()
    {
        _saveData = new SaveData();

        _saveData.Level = new(Game.Data.PlayerStats.CurrentLevel, Game.Data.PlayerStats.CurrentExp);
        _saveData.Resources = Game.Data.PlayerStats.Resources.Clone();
        _saveData.Bonuses = Game.Data.PlayerStats.Bonuses.Clone();
        _saveData.Levels = Game.Data.PlayerStats.Levels.Clone();
        _saveData.ExtraStatsLevel = Game.Data.PlayerStats.ExtraStatsLevel.Clone();
        _saveData.FarmStats = Game.Data.PlayerStats.FarmStats.Clone();

        _saveData.CurrentPosition = new SerializableVector3(Player.Transform.position);

        _saveData.CurrentCapacity = Game.Data.CapacityStats.CurrentCapacity;
        _saveData.CapacityResources = Game.Data.CapacityStats.Resources.Clone();

        _saveData.SaveNewtonJson(Key.Paths.SaveFile);
    }
}