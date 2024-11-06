using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [HideInInspector] public CharacterManager Manager;

    [SerializeField] private Transform _billboardCanvas;
    [SerializeField] private Image _healthBar;

    private void Awake()
    {
        Manager = GetComponent<CharacterManager>();
    }

    public void MonoUpdate()
    {
        Vector3 direction = (Camera.main.transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        _billboardCanvas.transform.rotation = rotation;

        //_billboardCanvas.LookAt(Camera.main.transform);
    }

    public void UpdateHealthBar(uint current, uint max)
    {
        float ratio = (float)current / max;
        _healthBar.fillAmount = ratio;
    }
}