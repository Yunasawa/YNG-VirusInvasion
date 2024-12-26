using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Extensions.Methods;

public class Boundary : MonoBehaviour
{
    [HideInInspector] public List<Bound> Boundaries = new();

    public Vector3 GetRandomPositionInRandomBoundary()
    {
        Bound boundary = Boundaries[Random.Range(0, Boundaries.Count)];
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
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        Boundary manager = (Boundary)target;

        // Add Button to add new Bound
        Button addButton = new Button(() =>
        {
            Vector3 topleft = manager.transform.position.AddX(-10).AddZ(10);
            Vector3 bottomRight = manager.transform.position.AddX(10).AddZ(-10);
            if (!manager.Boundaries.IsEmpty())
            {
                topleft = manager.Boundaries[^1].TopFrontLeft.AddX(-10).AddZ(10);
                bottomRight = manager.Boundaries[^1].BottomBackRight.AddX(10).AddZ(-10);
            }
            manager.Boundaries.Add(new Bound(topleft, bottomRight, true));
            RefreshListView(root, manager);

            // Force scene view to repaint
            SceneView.RepaintAll();
        })
        {
            text = "Add Bound"
        };
        root.Add(addButton);

        // ListView to display the Boundaries
        var listView = CreateListView(manager);
        root.Add(listView);

        return root;
    }

    private ListView CreateListView(Boundary manager)
    {
        var listView = new ListView(
            manager.Boundaries,
            25,
            () => new VisualElement()) { style = { flexDirection = FlexDirection.Column } };
        listView.bindItem = (VisualElement element, int index) =>
            {
                if (index < manager.Boundaries.Count)
                {
                    Bound boundary = manager.Boundaries[index];

                    var container = new VisualElement { style = { flexDirection = FlexDirection.Row } };

                    Button editButton = new Button() { text = boundary.isEditing ? "Done" : "Edit" };
                    editButton.clicked += (() =>
                    {
                        boundary.isEditing = !boundary.isEditing;
                        element.parent.MarkDirtyRepaint();
                        editButton.text = boundary.isEditing ? "Done" : "Edit";

                        // Force scene view to repaint
                        SceneView.RepaintAll();
                    });
                    container.Add(editButton);

                    Button removeButton = new Button(() =>
                    {
                        manager.Boundaries.RemoveAt(index);
                        listView.Rebuild(); // Rebuild the ListView to reflect changes
                        if (element != null && element.parent != null) element.parent.MarkDirtyRepaint();

                        // Force scene view to repaint
                        SceneView.RepaintAll();
                    })
                    {
                        text = "Remove"
                    };
                    container.Add(removeButton);

                    // Labels for positions
                    var topLeftField = new Label($"TopFrontLeft: {boundary.TopFrontLeft}");
                    container.Add(topLeftField);

                    var bottomRightField = new Label($"BottomBackRight: {boundary.BottomBackRight}");
                    container.Add(bottomRightField);

                    element.Clear();
                    element.Add(container);
                }
            };

        return listView;
    }


    private void RefreshListView(VisualElement root, Boundary manager)
    {
        // Remove old ListView if any
        var oldListView = root.Q<ListView>();
        if (oldListView != null)
        {
            root.Remove(oldListView);
        }

        // Add updated ListView
        var newListView = CreateListView(manager);
        root.Add(newListView);
    }


    private void OnSceneGUI()
    {
        Boundary manager = (Boundary)target;

        foreach (var boundary in manager.Boundaries)
        {
            if (boundary.isEditing)
            {
                // Draw handles for the corners
                Vector3 topFrontLeftHandle = Handles.PositionHandle(boundary.TopFrontLeft, Quaternion.identity);
                Vector3 bottomBackRightHandle = Handles.PositionHandle(boundary.BottomBackRight, Quaternion.identity);

                // Update the boundary if the handles are moved
                if (topFrontLeftHandle != boundary.TopFrontLeft || bottomBackRightHandle != boundary.BottomBackRight)
                {
                    boundary.TopFrontLeft = topFrontLeftHandle;
                    boundary.BottomBackRight = bottomBackRightHandle;

                    // Mark the object as dirty to save changes
                    EditorUtility.SetDirty(manager);
                }
            }

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
        }
    }
}
#endif
