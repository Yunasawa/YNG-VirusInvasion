using System;
using UnityEngine;
#if SUPERSONIC_WISDOM
using Altom.AltTester;
#endif

[DisallowMultipleComponent]
public class SwUiElementId : MonoBehaviour
{
    #region --- Members ---

    [SerializeField] private string _id;
    
#if SUPERSONIC_WISDOM
    private AltId _aldId;
#endif
    public string Id
    {
        get { return _id; }
        set
        {
            _id = value;
            Refresh();
        }
    }

    #endregion


    #region --- Mono Override ---

    protected void OnValidate()
    {
        Id ??= Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        Refresh();
    }

    private void Refresh()
    {
#if SUPERSONIC_WISDOM
        if(_aldId == null){
            _aldId = gameObject.AddComponent<AltId>();
        }

        _aldId.altID = Id;
#endif
    }

    #endregion
}