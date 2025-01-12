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

        SaveGameContinuously().Forget();
    }

    private async UniTaskVoid SaveGameContinuously()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(10, ignoreTimeScale: true);
            SaveData();
        }
    }

    private void LoadData()
    {
        _saveData = MJson.LoadNewtonJson<SaveData>(Key.Path.SaveFile);

        if (_saveData.IsNull())
        {
            Game.Data.RuntimeStats.Reset();
            SaveData();
            _saveData = MJson.LoadNewtonJson<SaveData>(Key.Path.SaveFile);
        }

        Game.IsStartGameTutorialActivated = _saveData.TutorialToggle.StartGameTutorial;
        Game.IsReturnToBaseTutorialActivated = _saveData.TutorialToggle.ReturnToBaseTutorial;
        Game.IsUpgradeAttributeTutorialActivated = _saveData.TutorialToggle.UpgradeAttributeTutorial;
        Game.IsFocusOnMainQuest1FirstTime = _saveData.TutorialToggle.IsFocusOnMainQuest1FirstTime;

        Game.Data.PlayerStats = _saveData.PlayerStats;
        Game.Data.CapacityStats = _saveData.CapacityStats;
        Game.Data.RuntimeStats.ConstructStats = _saveData.RuntimeConstructStats;

        LoadDataDelay().Forget();
    }

    private async UniTaskVoid LoadDataDelay()
    {
        await UniTask.NextFrame();

        _saveData.SerializeQuestStats.DeserializeData();

        CameraManager.Instance.transform.position = _saveData.CurrentPosition.ToVector3();
        Player.Character.enabled = false;
        Player.Transform.position = _saveData.CurrentPosition.ToVector3();
        await UniTask.NextFrame();
        Player.Character.enabled = true;

        Player.Stage.CurrentStage = _saveData.CurrentStage;
    }

    private void SaveData()
    {
        new SaveData().Set().SaveNewtonJson(Key.Path.SaveFile);
    }
}