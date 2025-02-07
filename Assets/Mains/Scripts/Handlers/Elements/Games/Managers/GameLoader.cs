using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEditor;
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
        _saveData = MJson2.LoadNewtonJson<SaveData>(Key.Path.SaveFile);

        if (_saveData.IsNull())
        {
            Game.Data.RuntimeStats.Reset();
            SaveData();
            _saveData = MJson2.LoadNewtonJson<SaveData>(Key.Path.SaveFile);
        }

        Game.IsStartGameTutorialActivated = _saveData.TutorialToggle.StartGameTutorial;
        Game.IsReturnToBaseTutorialActivated = _saveData.TutorialToggle.ReturnToBaseTutorial;
        Game.IsUpgradeAttributeTutorialActivated = _saveData.TutorialToggle.UpgradeAttributeTutorial;
        Game.IsFocusOnMainQuest1FirstTime = _saveData.TutorialToggle.IsFocusOnMainQuest1FirstTime;

        Game.Data.PlayerStats = _saveData.PlayerStats;
        Game.Data.CapacityStats = _saveData.CapacityStats;
        Game.Data.RuntimeStats.ConstructStats = _saveData.RuntimeConstructStats;

        Game.Data.RuntimeEnemy.EnemyStats = _saveData.RuntimeEnemyStats == null ? new() : _saveData.RuntimeEnemyStats;

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

public static class MJson2
{
    /// <summary>
    /// Convert data to JSON string (not including unserializable type).
    /// </summary>
    public static bool SaveNewtonJson<T>(this T data, string path, Action saveDone = null)
    {
        // Check if the target file exists
        if (!File.Exists(path))
        {
            // Create the file and close it immediately
            using (File.Create(path))
            {
                Debug.LogWarning("Target JSON file doesn't exist! Created a new file.");
            }
        }

        // Serialize data to JSON
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        // Write JSON to the file path
        File.WriteAllText(path, json);

#if UNITY_EDITOR
        // Refresh AssetDatabase if in Unity Editor
        AssetDatabase.Refresh();
#endif

        saveDone?.Invoke(); // Call saveDone action if provided
        return true;
    }

    /// <summary>
    /// Load data from JSON string (not including unserializable type).
    /// </summary>
    public static T LoadNewtonJson<T>(string path, Action<T> complete = null, Action<string> fail = null)
    {
        T data = default;

        // Check if the target file exists
        if (File.Exists(path))
        {
            try
            {
                // Read JSON from the file path
                string json = File.ReadAllText(path);
                // Deserialize JSON to data
                data = JsonConvert.DeserializeObject<T>(json);

                // Call complete action if provided
                complete?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading JSON: {e.Message}");
                fail?.Invoke("Load JSON failed!");
            }
        }
        else
        {
            Debug.LogWarning("Target JSON file not found!");
            fail?.Invoke("Load JSON failed!");
        }

        return data;
    }
}
