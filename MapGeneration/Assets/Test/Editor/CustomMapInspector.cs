using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class CustomMapInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Map map = target as Map;
        if (GUILayout.Button("Generate"))
        {
            map.Generate();
        }
    }
}
