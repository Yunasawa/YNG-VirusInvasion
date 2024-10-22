using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Enemy;
    public int Amount;

    public Button SpawnButton;
    public Button DestroyButton;
    public TMP_InputField InputField;

    private void Start()
    {
        SpawnButton.onClick.AddListener(Spawn);
        DestroyButton.onClick.AddListener(Destroy);
        InputField.onValueChanged.AddListener(OnValueChanged);
        InputField.text = Amount.ToString();
    }

    public void Spawn()
    {
        Destroy();
        for (int i = 0; i < Amount; i++) Instantiate(Enemy, new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f)), Quaternion.identity, this.transform);
    }

    public void Destroy()
    {
        this.transform.DestroyAllChildren();
    }

    public void OnValueChanged(string input) => int.TryParse(input, out Amount);
}