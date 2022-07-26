#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapGenerator))]
public sealed class MapGeneratorEditor : Editor
{
    MapGeneratorEditor() { }
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (Application.isPlaying) return;
        MapGenerator mapGenerator = (MapGenerator)target;
        if (GUILayout.Button("Generate Map"))
        {
            mapGenerator.GenerateMap();
        }
        if (GUILayout.Button("Delete Old Map"))
        {
            mapGenerator.DeleteOldMap();
        }
    }

}
#endif