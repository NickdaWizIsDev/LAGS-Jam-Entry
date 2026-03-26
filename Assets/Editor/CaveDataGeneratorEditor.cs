using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager generator = (GameManager)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Start Day 1"))
        {
            generator.StartManually();
        }
    }
}
