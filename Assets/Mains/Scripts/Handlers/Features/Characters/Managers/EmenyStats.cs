using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class EmenyStats : MonoBehaviour
{
    public EnemySources Stats => Game.Data.EnemySources[ID];

    private Enemy _manager;

    public string ID;

    public int CurrentHealth;

    private void Awake()
    {
        _manager = GetComponent<Enemy>();
    }

    private void Start()
    {
        Initialization();
    }

    [Button]
    public void Assign()
    {
        string name = this.gameObject.name;
        string pattern = @"(\d+)\s\((\d+)\)";

        Match match = Regex.Match(name, pattern);

        if (match.Success)
        {
            string number = match.Groups[1].Value;
            string stage = match.Groups[2].Value;

            ID = $"Virus {stage} - {number}";
        }
    }

    public void Initialization()
    {
        CurrentHealth = Stats.HP;
    }

    public void OnKilled()
    {
        _manager.OnEnemyKilled?.Invoke();
        _manager.OnEnemyKilled = null;
        Player.OnCollectEnemyDrops?.Invoke(Stats.Drops.Select(pair => (pair.Key, pair.Value)).ToArray());
        Player.OnCollectEnemyExp?.Invoke(Stats.Exp);
    }
}