using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public (uint Current, uint Max) Health;
    public Dictionary<ResourceType, ushort> Resources;


}