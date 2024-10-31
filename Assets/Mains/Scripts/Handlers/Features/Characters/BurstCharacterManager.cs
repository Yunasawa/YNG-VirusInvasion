using Unity.Burst;
using UnityEngine;

//[BurstCompile]
public class BurstCharacterManager : MonoBehaviour
{
    [SerializeField] private CharacterManager[] _characterPrefabs = new CharacterManager[0];
    [SerializeField] private int _characterAmount = 10;

    [SerializeField] private BurstCharacterMovement _burstMovement;

    private void OnDestroy()
    {
        _burstMovement.BurstOnDestroy();
    }

    private void Start()
    {
        _burstMovement.BurstStart(_characterAmount);

        for (byte i = 0; i < _characterAmount; i++)
        {
            CharacterManager character = Instantiate(_characterPrefabs[0], this.transform);
            _burstMovement.InitializeCharacters(i, character);
        }
    }

    private void Update()
    {
        _burstMovement.BurstUpdate();
    }
}