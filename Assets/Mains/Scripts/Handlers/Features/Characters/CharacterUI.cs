using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [HideInInspector] public CharacterManager Manager;

    [SerializeField] private Transform _billboardCanvas;
    [SerializeField] private Image _healthBar;
    public Image HealthBar => _healthBar;

    private void Awake()
    {
        Manager = GetComponent<CharacterManager>();
    }

    public void MonoUpdate()
    {
        Vector3 direction = Camera.main.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);

        _billboardCanvas.transform.rotation = rotation;
    }

    public void UpdateHealthBar(uint current, uint max)
    {
        float ratio = (float)current / max;
        _healthBar.fillAmount = ratio;
    }
}