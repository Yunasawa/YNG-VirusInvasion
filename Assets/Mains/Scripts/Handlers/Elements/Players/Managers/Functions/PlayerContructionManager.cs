using UnityEngine;

public class PlayerConstructionManager : MonoBehaviour
{
    public void OnInteractionEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Base": InteractWith(true, ConstructType.Base); break;
            case "Market": InteractWith(true, ConstructType.Market); break;
            case "Exchanger": InteractWith(true, ConstructType.Exchanger); break;
            case "Farm": InteractWith(true, ConstructType.Farm); break;
        }
    }

    public void OnInteractionExit(Collider other)
    {
        switch (other.tag)
        {
            case "Base": InteractWith(false, ConstructType.Base); break;
            case "Market": InteractWith(false, ConstructType.Market); break;
            case "Exchanger": InteractWith(false, ConstructType.Exchanger); break;
            case "Farm": InteractWith(false, ConstructType.Farm); break;
        }
    }

    public void InteractWith(bool isEnter, ConstructType type)
    {

    }
}