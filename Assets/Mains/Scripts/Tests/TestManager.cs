using UnityEngine;
using Unity.Jobs;
using System.Collections.Generic;
public class TestManager : MonoBehaviour
{
    [SerializeField] private CharacterManager _monster;
    [SerializeField] private int _amount = 10;

    private List<CharacterManager> _characters = new();

    private void Start()
    {
        _characters.Clear();

        for (byte i = 0; i < _amount; i++)
        {
            CharacterManager character = Instantiate(_monster, transform);
            _characters.Add(character);
        }
    }

    private void Update()
    {
        for (byte i = 0; i < _characters.Count; i++)
        {
            _characters[i].Movement?.MonoUpdate();
            _characters[i].UI?.MonoUpdate();
        }
    }
}
