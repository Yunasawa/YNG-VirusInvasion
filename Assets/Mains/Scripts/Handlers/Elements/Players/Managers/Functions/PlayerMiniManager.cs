using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class PlayerMiniManager : MonoBehaviour
{
    private SerializableDictionary<string, PlayerStats.ExtraStats> _extraStatsLevel => Game.Data.PlayerStats.ExtraStatsLevel;

    [SerializeField] private Transform _deliveryContainer;
    [SerializeField] private Transform _hunterContainer;

    public List<DeliveryCell> DeliveryCells = new();
    public List<HunterCell> HunterCells = new();

    private void Awake()
    {
        Player.OnExtraStatsUpdate += OnUpdateStats;
    }

    private void OnDestroy()
    {
        Player.OnExtraStatsUpdate -= OnUpdateStats;
    }

    private void Start()
    {
        for (int i = 0; i < _extraStatsLevel[Key.Stats.DeliveryCellAmount].Value; i++)
        {
            InitializeCells(i);
        }
    }

    private void InitializeCells(int index)
    {
        DeliveryCell delivery = Instantiate(Game.Input.DeliveryCell, _deliveryContainer);
        delivery.Initialize(Game.Data.RuntimeStats.RuntimeMiniCell.DeliveryCells[index]);
        DeliveryCells.Add(delivery);
    }

    private void OnUpdateStats(string stats)
    {
        InitializeCells(DeliveryCells.Count);
    }
}