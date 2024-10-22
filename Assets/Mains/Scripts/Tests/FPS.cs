using System.Collections;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private FPSType _type;

    [SerializeField] private TextMeshProUGUI _fpsText;
    private Coroutine _coroutine;

    private void Start()
    {
        _coroutine = StartCoroutine(FPSOverSecond());
        Application.targetFrameRate = 60;
    }

    private IEnumerator FPSOverSecond()
    {
        yield return new WaitForSeconds(1);

        if (_type == FPSType.FPS) _fpsText.text = $"{(int)(1 / Time.deltaTime * Time.timeScale)}fps";
        else if (_type == FPSType.MS) _fpsText.text = $"{(int)(1000 * Time.deltaTime / Time.timeScale)}ms";

        StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(FPSOverSecond());
    }
}

public enum FPSType
{
    FPS, MS
}