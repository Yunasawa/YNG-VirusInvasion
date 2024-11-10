//using System.Collections.Generic;
//using Unity.Mathematics;
//using UnityEditor;
//using UnityEngine;

//namespace SODynamics.Demo
//{
//    public class SecondOrderDemo : MonoBehaviour
//    {
//        [Range(0, 10)]
//        public float f;

//        [Range(0, 2)]
//        public float z;

//        [Range(-10, 10)]
//        public float r;

//        public Transform target;

//        private float f0, z0, r0;

//        private SecondOrderDynamics func;

//        private void Awake() => InitFunction();

//        private void Update()
//        {
//            if (target == null)
//                return;

//            if (f != f0 || r != r0 || z != z0)
//                InitFunction();
//            else
//            {
//                Vector3? funcOutput = func.Update(Time.deltaTime, target.position);

//                if (funcOutput != null)
//                    transform.position = new Vector3(funcOutput.Value.x, funcOutput.Value.y, funcOutput.Value.z);
//            }
//        }

//        private void InitFunction()
//        {
//            f0 = f;
//            z0 = z;
//            r0 = r;

//            func = new SecondOrderDynamics(f, z, r, transform.position);
//        }
//    }
//}

//namespace SODynamics
//{
//    public class SecondOrderDynamics
//    {
//        private Vector3? xp;
//        private Vector3? y, yd;
//        private float _w, _z, _d, k1, k2, k3;

//        public SecondOrderDynamics(float f, float z, float r, Vector3 x0)
//        {
//            _w = 2 * math.PI * f;
//            _z = z;
//            _d = _w * math.sqrt(math.abs(z * z - 1));
//            k1 = z / (math.PI * f);
//            k2 = 1 / (_w * _w);
//            k3 = r * z / _w;

//            xp = x0;
//            y = x0;
//            yd = Vector3.zero;
//        }

//        public Vector3? Update(float T, Vector3 x, Vector3? xd = null)
//        {
//            if (xd == null)
//            {
//                xd = (x - xp) / T;
//                xp = x;
//            }

//            float k1_stable, k2_stable;
//            if (_w * T < _z)
//            {
//                k1_stable = k1;
//                k2_stable = Mathf.Max(k2, T * T / 2 + T * k1 / 2, T * k1);
//            }
//            else
//            {
//                float t1 = math.exp(-_z * _w * T);
//                float alpha = 2 * t1 * (_z <= 1 ? math.cos(T * _d) : math.cosh(T * _d));
//                float beta = t1 * t1;
//                float t2 = T / (1 + beta - alpha);
//                k1_stable = (1 - beta) * t2;
//                k2_stable = T * t2;
//            }

//            y = y + T * yd;
//            yd = yd + T * (x + k3 * xd - y - k1_stable * yd) / k2_stable;
//            return y;
//        }
//    }
//}

//namespace SODynamics.Demo
//{
//    [CustomEditor(typeof(SecondOrderDemo))]
//    public class SecondOrderDemoInspector : Editor
//    {
//        private const float defaultLenght = 2.0f;
//        private const float defaultValue = 1.0f;

//        private const float paddingLeft = 10f;
//        private const float paddingRight = 2f;
//        private const float paddingTop = 15f;
//        private const float paddingBottom = 15f;

//        private const int evaluationSteps = 300;

//        private float f, f0, z, z0, r, r0;

//        private SecondOrderDynamics func;

//        private Material mat;

//        private EvaluationData evalData;

//        private void OnEnable()
//        {
//            var shader = Shader.Find("Hidden/Internal-Colored");
//            mat = new Material(shader);

//            evalData = new();

//            InitFunction();
//        }

//        private void OnDisable()
//        {
//            func = null;
//            evalData = null;

//            f = f0 = z = z0 = r = r0 = float.NaN;

//            DestroyImmediate(mat);
//        }

//        public override void OnInspectorGUI()
//        {
//            DrawDefaultInspector();
//            UpdateInput();

//            Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
//            if (Event.current.type == EventType.Repaint)
//            {
//                GUI.BeginClip(rect);
//                GL.PushMatrix();

//                GL.Clear(true, false, Color.black);
//                mat.SetPass(0);

//                float rectWidth = rect.width - paddingLeft - paddingRight;
//                float rectHeight = rect.height - paddingTop - paddingBottom;

//                float x_AxisOffset = rectHeight * math.remap(evalData.Y_min, evalData.Y_max, 0, 1, 0);
//                float defaultValueOffset = rectHeight * math.remap(evalData.Y_min, evalData.Y_max, 0, 1, 1); ;

//                // draw base graph
//                GL.Begin(GL.LINES);
//                GL.Color(new Color(1, 1, 1, 1));
//                // draw Y axis
//                GL.Vertex3(paddingLeft, paddingTop, 0);
//                GL.Vertex3(paddingLeft, rect.height - paddingBottom, 0);
//                // draw X axis
//                GL.Vertex3(paddingLeft, rect.height - x_AxisOffset - paddingBottom, 0);
//                GL.Vertex3(rect.width - paddingRight, rect.height - x_AxisOffset - paddingBottom, 0);
//                // draw default values
//                GL.Color(Color.green);
//                GL.Vertex3(paddingLeft, rect.height - defaultValueOffset - paddingBottom, 0);
//                GL.Vertex3(rect.width - paddingRight, rect.height - defaultValueOffset - paddingBottom, 0);
//                GL.End();

//                // evaluate func values
//                if (evalData.IsEmpty) EvaluateFunction();

//                // re-evaluate func values after input values changed
//                if (f != f0 || z != z0 || r != r0)
//                {
//                    InitFunction();
//                    EvaluateFunction();
//                }

//                // draw graph
//                GL.Begin(GL.LINE_STRIP);
//                GL.Color(Color.cyan);
//                for (int i = 0; i < evalData.Length; i++)
//                {
//                    Vector2 point = evalData.GetItem(i);

//                    float x_remap = math.remap(evalData.X_min, evalData.X_max, 0, rectWidth, point.x);
//                    float y_remap = math.remap(evalData.Y_min, evalData.Y_max, 0, rectHeight, point.y);

//                    GL.Vertex3(paddingLeft + x_remap, rect.height - y_remap - paddingBottom, 0.0f);
//                }
//                GL.End();

//                GL.PopMatrix();
//                GUI.EndClip();

//                // draw values
//                float squareSize = 10;
//                EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, rect.y + rect.height - defaultValueOffset - paddingBottom - squareSize / 2, squareSize, squareSize), "1"); // heigt "1" mark
//                EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, rect.y + rect.height - x_AxisOffset - paddingBottom + (squareSize * 0.2f), squareSize, squareSize), "0"); // height "0" mark
//                EditorGUI.LabelField(new Rect(rect.x + rect.width - paddingRight - squareSize, rect.y + rect.height - x_AxisOffset - paddingBottom + (squareSize * 0.2f), squareSize, squareSize), "2"); // max lenght mark
//            }
//        }

//        private void UpdateInput()
//        {
//            f = ((SecondOrderDemo)target).f;
//            z = ((SecondOrderDemo)target).z;
//            r = ((SecondOrderDemo)target).r;
//        }

//        private void InitFunction()
//        {
//            f0 = f = ((SecondOrderDemo)target).f;
//            z0 = z = ((SecondOrderDemo)target).z;
//            r0 = r = ((SecondOrderDemo)target).r;

//            func = new SecondOrderDynamics(f, z, r, new Vector3(-defaultLenght, 0, 0));
//        }

//        private void EvaluateFunction()
//        {
//            evalData.Clear();

//            for (int i = 0; i < evaluationSteps; i++)
//            {
//                float T = 0.016f; // constant deltaTime (60 frames per second)

//                // input step function params
//                float x_input = math.remap(0, evaluationSteps - 1, -defaultLenght, defaultLenght, i);
//                float y_input = x_input > 0 ? defaultValue : 0;

//                Vector3? funcValues = func.Update(T, new Vector3(x_input, y_input, 0));

//                if (x_input <= 0) continue; // data is gathered only after the Y value has changed

//                evalData.Add(new Vector2(funcValues.Value.x, funcValues.Value.y));
//            }
//        }
//    }
//}

//namespace SODynamics.Demo
//{
//    public class EvaluationData
//    {
//        private List<Vector2> points;
//        private Vector2 x_limits;
//        private Vector2 y_limits;

//        public int Length => points.Count;
//        public float X_min => x_limits.x;
//        public float X_max => x_limits.y;
//        public float Y_min => y_limits.x;
//        public float Y_max => y_limits.y;

//        public bool IsEmpty => points.Count <= 0;

//        public EvaluationData()
//        {
//            points = new List<Vector2>();
//            x_limits = new Vector2(float.NaN, float.NaN);
//            y_limits = new Vector2(float.NaN, float.NaN);
//        }

//        public void Add(Vector2 point)
//        {
//            if (points.Count == 0)
//            {
//                x_limits = new Vector2(point.x, point.x);
//                y_limits = new Vector2(point.y, point.y);
//            }
//            else
//            {
//                x_limits.x = point.x < x_limits.x ? point.x : x_limits.x;
//                x_limits.y = point.x > x_limits.y ? point.x : x_limits.y;

//                y_limits.x = point.y < y_limits.x ? point.y : y_limits.x;
//                y_limits.y = point.y > y_limits.y ? point.y : y_limits.y;
//            }

//            points.Add(point);
//        }

//        public Vector2 GetItem(int index) => points[index];

//        public void Clear()
//        {
//            points.Clear();
//            x_limits = new Vector2(float.NaN, float.NaN);
//            y_limits = new Vector2(float.NaN, float.NaN);
//        }
//    }
//}