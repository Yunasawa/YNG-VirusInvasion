using System.Collections.Generic;

public class AdjustAttributeCheaterUI : CheaterViewUI
{
    public List<AdjustAttributeFieldUI> Fields = new();

    public override void OnOpenView()
    {
        foreach (var field in Fields) field.UpdateUI();
    }
}