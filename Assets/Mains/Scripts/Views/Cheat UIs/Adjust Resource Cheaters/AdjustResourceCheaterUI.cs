using System.Collections.Generic;
using UnityEngine;

public class AdjustResourceCheaterUI : CheaterViewUI
{
    public List<AdjustResourceFieldUI> Fields = new();

    public override void OnOpenView()
    {
        foreach (var field in Fields) field.UpdateUI();
    }
}