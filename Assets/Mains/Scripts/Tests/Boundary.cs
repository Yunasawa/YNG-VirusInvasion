using UnityEngine;
using System.Collections.Generic;
using YNL.Extensions.Methods;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Boundary : MonoBehaviour
{
    public List<Bound> Boundaries = new();

    public Vector3 GetRandomPositionInRandomBoundary()
    {
        Bound boundary = Boundaries.GetRandom();
        float x = Random.Range(boundary.TopFrontLeft.x, boundary.BottomBackRight.x); 
        float y = Random.Range(boundary.BottomBackRight.y, boundary.TopFrontLeft.y); 
        float z = Random.Range(boundary.TopFrontLeft.z, boundary.BottomBackRight.z); 
        return new Vector3(x, y, z);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Boundary))]
public class BoundaryManagerEditor : Editor
{

    private void OnSceneGUI()
    {
        Boundary manager = (Boundary)target;

        for (int i = 0; i < manager.Boundaries.Count; i++)
        {
            Bound boundary = manager.Boundaries[i];

            // Draw handles for the corners
            Vector3 topFrontLeftHandle = Handles.PositionHandle(boundary.TopFrontLeft, Quaternion.identity);
            Vector3 bottomBackRightHandle = Handles.PositionHandle(boundary.BottomBackRight, Quaternion.identity);

            // Draw the boundary box
            Vector3 topFrontRight = new Vector3(boundary.BottomBackRight.x, boundary.TopFrontLeft.y, boundary.TopFrontLeft.z);
            Vector3 bottomFrontLeft = new Vector3(boundary.TopFrontLeft.x, boundary.BottomBackRight.y, boundary.TopFrontLeft.z);
            Vector3 topBackLeft = new Vector3(boundary.TopFrontLeft.x, boundary.TopFrontLeft.y, boundary.BottomBackRight.z);
            Vector3 bottomBackLeft = new Vector3(boundary.TopFrontLeft.x, boundary.BottomBackRight.y, boundary.BottomBackRight.z);
            Vector3 topBackRight = new Vector3(boundary.BottomBackRight.x, boundary.TopFrontLeft.y, boundary.BottomBackRight.z);
            Vector3 bottomFrontRight = new Vector3(boundary.BottomBackRight.x, boundary.BottomBackRight.y, boundary.TopFrontLeft.z);

            Handles.DrawLine(boundary.TopFrontLeft, topFrontRight);
            Handles.DrawLine(boundary.TopFrontLeft, topBackLeft);
            Handles.DrawLine(boundary.BottomBackRight, bottomBackLeft);
            Handles.DrawLine(boundary.BottomBackRight, bottomFrontRight);
            Handles.DrawLine(topFrontRight, bottomFrontRight);
            Handles.DrawLine(topFrontRight, topBackRight);
            Handles.DrawLine(topBackLeft, topBackRight);
            Handles.DrawLine(topBackLeft, bottomBackLeft);
            Handles.DrawLine(bottomBackLeft, bottomFrontLeft);
            Handles.DrawLine(bottomFrontRight, bottomFrontLeft);

            // Update the boundary if the handles are moved
            if (topFrontLeftHandle != boundary.TopFrontLeft || bottomBackRightHandle != boundary.BottomBackRight)
            {
                boundary.TopFrontLeft = topFrontLeftHandle;
                boundary.BottomBackRight = bottomBackRightHandle;
                manager.Boundaries[i] = boundary;

                // Mark the object as dirty to save changes
                EditorUtility.SetDirty(manager);
            }
        }
    }
}
#endif
