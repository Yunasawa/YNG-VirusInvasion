using System;
using UnityEngine;
using YNL.Utilities.Addons;

namespace YNL.Bases
{
    [System.Serializable]
    public class CapacityStats
    {
        public uint CurrentCapacity;
        public SerializableDictionary<ResourceType, uint> Resources = new();

        public void AdjustResources(ResourceType type, int amount)
        {
            if (amount <= 0) return;

            uint currentResourceAmount = Resources.ContainsKey(type) ? Resources[type] : 0; 
            int newResourceAmount = (int)currentResourceAmount + amount; 
            int currentTotalCapacity = (int)CurrentCapacity;

            if (amount > 0) // Adding resources
            { 
                if (currentTotalCapacity < Game.Data.PlayerStats.Capacity) 
                { 
                    int maxAddableAmount = Mathf.RoundToInt(Game.Data.PlayerStats.Capacity - currentTotalCapacity); 
                    int amountToAdd = Math.Min(maxAddableAmount, amount); 
                    Resources[type] = (uint)(currentResourceAmount + amountToAdd); 
                    CurrentCapacity = (uint)(currentTotalCapacity + amountToAdd);
                } 
            } 
            else if (amount < 0) // Removing resources
            { 
                int amountToRemove = Math.Min((int)currentResourceAmount, -amount); 
                Resources[type] = (uint)(currentResourceAmount - amountToRemove); 
                CurrentCapacity = (uint)(currentTotalCapacity - amountToRemove); 
            }
        }

        public void ClearCapacity()
        {
            CurrentCapacity = 0;
            foreach (var pair in Resources) Resources[pair.Key] = 0;
        }
    }
}