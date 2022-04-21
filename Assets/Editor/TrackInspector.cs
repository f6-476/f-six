using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackMap))]
public class TrackInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TrackMap trackMap = (TrackMap)target;
        if(GUILayout.Button("Generate")) trackMap.Generate();
        if(GUILayout.Button("Clear")) trackMap.Clear();
    }
}
