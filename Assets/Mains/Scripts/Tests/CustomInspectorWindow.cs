//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UIElements;
//using System.Reflection;
//using YNL.Extensions.Methods;

//public class InspectorMenuItem
//{
//    [MenuItem("Window/Open Custom Inspector")]
//    public static void OpenInspector()
//    {
//        // Get the Inspector window
//        EditorWindow inspectorWindow = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow"));

//        // Create and add a custom label to the Inspector's toolbar
//        AddCustomLabelToInspector(inspectorWindow);
//    }

//    private static void AddCustomLabelToInspector(EditorWindow inspectorWindow)
//    {
//        if (inspectorWindow == null) return;

//        var root = inspectorWindow.rootVisualElement;

//        VisualElement container = root.Q("rootVisualContainer10");
//        MDebug.Log(container == null);

//    }
//}
