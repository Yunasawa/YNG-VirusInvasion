using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;

public class CharacterStats : MonoBehaviour
{
    public (uint Current, uint Max) Health;
    public Dictionary<ResourceType, ushort> Resources = new();
    public float _experimentMultiplier = 1;
}