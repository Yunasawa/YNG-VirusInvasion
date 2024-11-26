using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using YNL.Extensions.Methods;

public class ReplaceWall : MonoBehaviour
{
    public List<GameObject> boxes;
    public List<GameObject> walls;

    public Transform Container;

    [Button]
    public void ReplaceBoxesWithWalls()
    {
        foreach (var box in boxes)
        {
            GameObject selectedWall = walls[Random.Range(0, walls.Count)];

            GameObject spawnedObject = Instantiate(selectedWall, Container);
            spawnedObject.transform.localPosition = box.transform.localPosition;
            spawnedObject.transform.localScale = (box.transform.localScale / 10).SetY(0.55f);
        }
    }
}
