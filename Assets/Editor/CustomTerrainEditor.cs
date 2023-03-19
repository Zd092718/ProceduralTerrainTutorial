using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorGUITable;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]

public class CustomTerrainEditor : Editor
{
    //properties --------------------------------
    private CustomTerrain terrain;
    private SerializedProperty randomHeightRange;

    //fold outs ---------------------------------
    private bool showRandom = false;
    private void OnEnable()
    {
        terrain = (CustomTerrain)target;

        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        showRandom = EditorGUILayout.Foldout(showRandom, "Random");
        if (showRandom)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            if(GUILayout.Button("Random Heights"))
            {
                terrain.RandomTerrain();
            }
        }



        serializedObject.ApplyModifiedProperties();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
