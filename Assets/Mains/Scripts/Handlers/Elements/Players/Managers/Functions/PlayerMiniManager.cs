using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
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
        for (int i = 0; i < _extraStatsLevel[Key.Stats.DeliveryAmount].Value; i++) CreateDeliveryCell(i);
        for (int i = 0; i < _extraStatsLevel[Key.Stats.HunterAmount].Value; i++) CreateHunterCell(i);
    }

    private void CreateDeliveryCell(int index)
    {
        DeliveryCell delivery = Instantiate(Game.Input.DeliveryCell, _deliveryContainer);
        delivery.Initialize(Game.Data.RuntimeStats.RuntimeMiniCell.DeliveryCells[index]);
        DeliveryCells.Add(delivery);
    }
    private void CreateHunterCell(int index)
    {
        HunterCell hunter = Instantiate(Game.Input.HunterCell, _hunterContainer);
        HunterCellStats stats = Game.Data.RuntimeStats.RuntimeMiniCell.HunterCells[index];
        hunter.Initialize(Game.Data.RuntimeStats.RuntimeMiniCell.HunterCells[index]);
        HunterCells.Add(hunter);
    }

    private void OnUpdateStats(string stats)
    {
        if (stats == Key.Stats.DeliveryAmount) CreateDeliveryCell(DeliveryCells.Count);
        if (stats == Key.Stats.HunterAmount) CreateHunterCell(DeliveryCells.Count);
    }
}