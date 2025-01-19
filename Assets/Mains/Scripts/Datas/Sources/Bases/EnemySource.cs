#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Addons;
using YNL.Utilities.Addons;

namespace YNL.Bases
{
    [System.Serializable]
    public class EnemySources
    {
        public Enemy Enemy;
        public MRange MaxEnemyInAnArea = new(8, 12);
        public int Capacity = 3;
        public uint RespawnTime = 10;
        public int Exp = 200;
        public int HP = 10000;
        public int MS;
        public int AttackDamage;
        public SerializableDictionary<ResourceType, uint> Drops = new() { { ResourceType.Food1, 3000 } };

        public int EnemyAmount => Mathf.RoundToInt(Random.Range(MaxEnemyInAnArea.Min, MaxEnemyInAnArea.Max));
    }
}

#if UNITY_EDITOR && true
[CustomPropertyDrawer(typeof(EnemySources))]
public class EnemySourcesDrawer : OdinValueDrawer<EnemySources>
{
    private bool _showProperties = false;

    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect position = EditorGUILayout.GetControlRect();
        _showProperties = EditorGUI.Toggle(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Show properties", _showProperties);

        if (_showProperties) CallNextDrawer(label);
        else return;
    }
}

#endif