using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ABC_StateManager))]



public class ABC_StateManager_Editor : Editor {


    #region Window Colors
    public Color inspectorBackgroundColor = Color.white;
    public Color inspectorBackgroundProColor = new Color32(155, 185, 255, 255);

    public Color inspectorSectionHeaderColor = new Color32(137, 134, 134, 210);
    public Color inspectorSectionHeaderProColor = new Color32(165, 195, 255, 255);

    public Color inspectorSectionHeaderTextColor = new Color(0, 0.45f, 1, 1);
    public Color inspectorSectionHeaderTextProColor = new Color(1, 1, 1, 1f);

    public Color inspectorSectionBoxColor = new Color32(255, 255, 255, 190);
    public Color inspectorSectionBoxProColor = new Color32(0, 0, 0, 255);

    public Color inspectorSectionHelpColor = new Color32(113, 151, 243, 200);
    public Color inspectorSectionHelpProColor = new Color32(215, 235, 255, 255);
    #endregion

    #region Inspector Design Functions



    public void InspectorHelpBox(string text) {
        if (stateManager.showHelpInformation == true) {
            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = inspectorSectionHelpProColor;
            }
            else
            {
                GUI.color = inspectorSectionHelpColor;
            }
            EditorGUILayout.HelpBox(text, MessageType.None, true);
        }
        GUI.color = Color.white;
        EditorGUILayout.Space();
    }


    public void IntegrationButton(string IntegrationName, string IntegrationDefineSymbol)   {
        EditorGUILayout.BeginHorizontal();

        if (ABC_Utilities.IntegrationDefineSymbolExists(IntegrationDefineSymbol)){

            if (GUILayout.Button(new GUIContent(" Disable " + IntegrationName + " Integration"), GUILayout.Width(280)))
                ABC_Utilities.RemoveIntegrationDefineSymbols(IntegrationDefineSymbol);

        }else {

            if (GUILayout.Button(new GUIContent(" Enable " + IntegrationName + " Integration"), GUILayout.Width(280)))
                ABC_Utilities.AddIntegrationDefineSymbols(IntegrationDefineSymbol);

        }

        EditorGUILayout.EndHorizontal();

        InspectorHelpBox("Click to enable/disable integration with " + IntegrationName);

    }

    #endregion

    ABC_StateManager stateManager;
	SerializedObject GetTarget;

	
	void OnEnable(){
		stateManager = (ABC_StateManager)target;
		GetTarget = new SerializedObject(stateManager);
	}



    public override void OnInspectorGUI(){

        if (EditorGUIUtility.isProSkin)
        {
            GUI.backgroundColor = inspectorBackgroundProColor;
            GUI.contentColor = Color.white;
        }
        else
        {
            GUI.backgroundColor = inspectorBackgroundColor;
            GUI.contentColor = Color.black;
        }

        EditorGUIUtility.labelWidth = 190;
        EditorGUIUtility.fieldWidth = 35;

        //Update our list
        GetTarget.Update();


        EditorGUILayout.Space();

        if (GUILayout.Button("State Manager"))
        {
            // add standard defaults here
            var window = EditorWindow.GetWindow(typeof(ABC_StateManager_EditorWindow), true);
            window.maxSize = new Vector2(837f, 660f);
            window.minSize = window.maxSize;
        }

        EditorGUILayout.Space();

        //EditorGUILayout.PropertyField(GetTarget.FindProperty("showHelpInformation"), new GUIContent("Show Help Boxes"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("updateEditorAtRunTime"));
        InspectorHelpBox("Will decrease game performance but update editors at run time");


        GetTarget.FindProperty("foldOutIntegration").boolValue = EditorGUILayout.Foldout(GetTarget.FindProperty("foldOutIntegration").boolValue, "Integrations");

        if (GetTarget.FindProperty("foldOutIntegration").boolValue == true)
        {
            IntegrationButton("Game Creator", "ABC_GC_Integration");
            IntegrationButton("Game Creator Stats", "ABC_GC_Stats_Integration");

            EditorGUILayout.Space();
            IntegrationButton("Emerald AI", "ABC_EmeraldAI_Integration");
        }

      


        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
	}
}
