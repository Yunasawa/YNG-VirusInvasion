//using UnityEditor;
//using UnityEditor.IMGUI.Controls;
//using UnityEngine;

//public class BoxScaler : MonoBehaviour
//{
//    public Vector3 boxCenter = Vector3.zero;
//    public Vector3 boxSize = Vector3.one;
//}

//[CustomEditor(typeof(BoxScaler))]
//public class BoxScalerEditor : Editor
//{
//    private BoxBoundsHandle boxBoundsHandle;
//    private BoxScaler boxScaler;

//    private void OnEnable()
//    {
//        boxScaler = (BoxScaler)target;
//        boxBoundsHandle = new BoxBoundsHandle();
//        boxBoundsHandle.center = boxScaler.boxCenter;
//        boxBoundsHandle.size = boxScaler.boxSize;
//    }

//    private void OnSceneGUI()
//    {
//        // Draw the wireframe box
//        Handles.color = Color.green;
//        Handles.DrawWireCube(boxBoundsHandle.center, boxBoundsHandle.size);

//        // Draw the BoxBoundsHandle
//        EditorGUI.BeginChangeCheck();
//        boxBoundsHandle.DrawHandle();
//        if (EditorGUI.EndChangeCheck())
//        {
//            Undo.RecordObject(boxScaler, "Scale Box");
//            boxScaler.boxCenter = boxBoundsHandle.center;
//            boxScaler.boxSize = boxBoundsHandle.size;
//        }
//    }
//}


