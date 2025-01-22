using System.Text.RegularExpressions;
using TMPro;

public class AdjustStatusCheaterUI : CheaterViewUI
{
    public TMP_InputField HPField;
    public TMP_InputField EXPField;
    public TMP_InputField LevelField;

    protected override void Awake()
    {
        base.Awake();

        HPField.onValueChanged.AddListener(OnChangeHP);
        EXPField.onValueChanged.AddListener(OnChangeEXP);
        LevelField.onValueChanged.AddListener(OnChangeLevel);
    }

    public override void OnOpenView()
    {
        HPField.text = Game.Data.PlayerStats.CurrentHP.ToString();
        EXPField.text = Game.Data.PlayerStats.CurrentExp.ToString();
        LevelField.text = Game.Data.PlayerStats.CurrentLevel.ToString();
    }

    private void OnChangeHP(string input)
    {
        input = Regex.Replace(input, @"[^\d]", "");

        Game.Data.PlayerStats.CurrentHP = int.Parse(input);
        Player.Stats.UpdateHealthBar();
    }
    private void OnChangeEXP(string input)
    {
        input = Regex.Replace(input, @"[^\d]", "");

        Game.Data.PlayerStats.CurrentExp = int.Parse(input);
        Player.OnCollectEnemyExp?.Invoke(0);
    }
    private void OnChangeLevel(string input)
    {
        input = Regex.Replace(input, @"[^\d]", "");

        Game.Data.PlayerStats.CurrentLevel = int.Parse(input);
        Player.OnLevelUp?.Invoke();
    }
}