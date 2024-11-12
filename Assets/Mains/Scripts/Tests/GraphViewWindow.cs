//using UnityEditor;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.UIElements;
//using UnityEngine;
//using UnityEngine.UIElements;

//public class GraphViewWindow : EditorWindow
//{
//    [MenuItem("Window/GraphView Example")]
//    public static void ShowWindow()
//    {
//        GraphViewWindow wnd = GetWindow<GraphViewWindow>();
//        wnd.titleContent = new GUIContent("GraphView Example");
//    }

//    private void OnEnable()
//    {
//        GraphViewExample graphView = new GraphViewExample
//        {
//            name = "GraphViewExample"
//        };

//        graphView.StretchToParentSize();
//        rootVisualElement.Add(graphView);

//        // Add a toolbar
//        Toolbar toolbar = new Toolbar();
//        Button addNodeButton = new Button(() => { graphView.AddNode("New Node"); })
//        {
//            text = "Add Node"
//        };
//        toolbar.Add(addNodeButton);
//        rootVisualElement.Add(toolbar);
//    }

//    public class GraphViewExample : GraphView
//    {
//        public GraphViewExample()
//        {
//            // Add grid background
//            GridBackground grid = new GridBackground();
//            Insert(0, grid);
//            grid.StretchToParentSize();

//            // Setup zoom capabilities
//            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

//            // Add manipulators for selection and dragging
//            this.AddManipulator(new ContentDragger());
//            this.AddManipulator(new SelectionDragger());
//            this.AddManipulator(new RectangleSelector());

//            // Set style for better visibility
//            styleSheets.Add(Resources.Load<StyleSheet>("GraphViewStyle"));
//            AddToClassList("graphView");
//        }

//        public void AddNode(string nodeName)
//        {
//            var node = new Node
//            {
//                title = nodeName,
//                style =
//                {
//                    left = 100,
//                    top = 100
//                }
//            };

//            // Add input and output ports
//            Port input = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
//            input.portName = "Input";
//            node.inputContainer.Add(input);

//            Port output = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
//            output.portName = "Output";
//            node.outputContainer.Add(output);

//            node.RefreshExpandedState();
//            node.RefreshPorts();
//            node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));
//            AddElement(node);
//        }
//    }
//}
