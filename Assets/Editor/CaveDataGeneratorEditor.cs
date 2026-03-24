using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaveDataGenerator))]
public class CaveDataGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CaveDataGenerator generator = (CaveDataGenerator)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Level"))
        {
            generator.GenerateLevel(1); // Default to day 1, or you can add a field for currentDay
        }
    }
}
