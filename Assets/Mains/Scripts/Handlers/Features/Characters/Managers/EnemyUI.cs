using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    private Enemy _manager;

    [SerializeField] private Transform _billboardCanvas;
    [SerializeField] private Image _healthBar;
    public Image HealthBar => _healthBar;

    private void Awake()
    {
        _manager = GetComponent<Enemy>();
    }

    private void Start()
    {
        Initialization();
    }

    public void Initialization()
    {
        _healthBar.fillAmount = 1;
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