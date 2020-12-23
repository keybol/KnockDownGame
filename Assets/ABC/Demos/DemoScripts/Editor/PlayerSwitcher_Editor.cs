using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerSwitcher))]
public class PlayerSwitcher_Editor : Editor {


    #region Window Colors
    public Color inspectorSectionHeaderColor = new Color32(137, 134, 134, 210);
    public Color inspectorSectionHeaderTextColor = new Color(0, 0.45f, 1, 1);

    public Color inspectorSectionHelpColor = new Color32(113, 151, 243, 200);
    public Color inspectorSectionBoxColor = new Color32(255, 255, 255, 190);
    #endregion

    // ************************* Inspector Design Functions ***********************************

    #region Design Functions

    public void InspectorPropertyListBox(string header, SerializedProperty list, int listIndex, Color headerColor, bool includeUpDown = false, bool includeBox = true, SerializedProperty foldOut = null) {

        GUI.color = inspectorSectionBoxColor;
        if (includeBox) {
            EditorGUILayout.BeginVertical("box");
        }
        else {
            EditorGUILayout.BeginVertical();
        }
        GUI.color = Color.white;

        GUI.color = headerColor;
        EditorGUILayout.BeginHorizontal();

        if (foldOut != null && foldOut.boolValue == true) {
            if (GUILayout.Button(DownFoldSymbol.ToString(), GUILayout.Width(30))) {
                foldOut.boolValue = false;
            }
        }
        else if (foldOut != null) {
            if (GUILayout.Button(RightFoldSymbol.ToString(), GUILayout.Width(30))) {
                foldOut.boolValue = true;
            }

        }
        GUILayout.Box(header, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(19) });

        if (includeUpDown) {
            GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
            if (GUILayout.Button(UpArrowSymbol.ToString(), GUILayout.Width(20))) {
                list.MoveArrayElement(listIndex, listIndex - 1);
                EditorUtility.SetDirty(playerSwitcher);
            }
            if (GUILayout.Button(DownArrowSymbol.ToString(), GUILayout.Width(20))) {
                list.MoveArrayElement(listIndex, listIndex + 1);
                EditorUtility.SetDirty(playerSwitcher);
            }
        }


        Color originalTextColor = GUI.skin.button.normal.textColor; 
        GUI.skin.button.normal.textColor = Color.red;
        if (GUILayout.Button("X", GUILayout.Width(30))) {
            list.DeleteArrayElementAtIndex(listIndex);
        }
        GUILayout.EndHorizontal();


        GUI.color = Color.white;
        GUI.skin.button.normal.textColor = originalTextColor;
    }

    public void InspectorListBox(string title, SerializedProperty list, bool expandWidth = false) {

        if (expandWidth) {
            EditorGUILayout.BeginVertical();
        }
        else {
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
        }

        GUI.color = new Color32(208, 212, 211, 255);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box(title, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(19) });
        GUI.color = Color.white;
        GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
        if (GUILayout.Button(new GUIContent("+"), GUILayout.Width(30))) {
            list.InsertArrayElementAtIndex(list.arraySize);
        }
        GUILayout.EndHorizontal();

        Color originalTextColor = GUI.skin.button.normal.textColor;
        if (list.arraySize > 0) {
            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < list.arraySize; i++) {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(element, new GUIContent(""));
                GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
                if (GUILayout.Button(UpArrowSymbol.ToString())) {
                    list.MoveArrayElement(i, i - 1);
                }
                if (GUILayout.Button(DownArrowSymbol.ToString())) {
                    list.MoveArrayElement(i, i + 1);
                }


                GUI.skin.button.normal.textColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(40))) {
                    list.DeleteArrayElementAtIndex(i);
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();

        GUI.skin.button.normal.textColor = originalTextColor;
    }

    public void InspectorHeader(string text, bool space = true) {

        if (space == true) {
            EditorGUILayout.Space();
        }

        GUIStyle myStyle = new GUIStyle("Box");
        myStyle.normal.textColor = inspectorSectionHeaderTextColor;
        myStyle.alignment = TextAnchor.MiddleCenter;
        myStyle.fontStyle = FontStyle.Bold;
        myStyle.fontSize = 11;
        myStyle.wordWrap = true;
        Color originalTextColor = GUI.skin.button.normal.textColor;
        GUI.color = inspectorSectionHeaderColor;
        GUILayout.Box(" " + text, myStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(19) });
        GUI.color = Color.white;
        GUI.skin.box.normal.textColor = originalTextColor;
    }

    public void InspectorHelpBox(string text, bool space = true, bool alwaysShow = false) {


        GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
        myStyle.richText = true;
        myStyle.wordWrap = true;
        myStyle.fixedWidth = 0;

        GUI.color = inspectorSectionHelpColor;
        EditorGUILayout.LabelField(text, myStyle, GUILayout.ExpandWidth(true));

        if (space == true) {
            EditorGUILayout.Space();
        }


        GUI.color = Color.white;



    }

    // symbols used for Aesthetics only
    char UpArrowSymbol = '\u2191';
    char RightFoldSymbol = '\u25B6';
    char DownFoldSymbol = '\u25BC';
    char DownArrowSymbol = '\u2193';


    #endregion

    // ************************* Settings / Serialized Properties ***********************************

    #region Settings
    PlayerSwitcher playerSwitcher;
    SerializedObject GetTarget;
    SerializedProperty meSwitchEntitiesList;

    #endregion


    void OnEnable() {

        playerSwitcher = (PlayerSwitcher)target;
        GetTarget = new SerializedObject(playerSwitcher);
        meSwitchEntitiesList = GetTarget.FindProperty("switchEntities");
    }



    public override void OnInspectorGUI() {

        //Update 
        GetTarget.Update();

        EditorGUILayout.PropertyField(GetTarget.FindProperty("playerCamera"));
        EditorGUILayout.Space();



        if (GUILayout.Button(new GUIContent(" Add Switch Entity", "Add New Switch Entity"))) {
            // add standard defaults here
            playerSwitcher.switchEntities.Add(new PlayerSwitcher.SwitchEntity());
            EditorUtility.SetDirty(playerSwitcher);

        }

        EditorGUILayout.Space();

        #region Switch Entities
        for (int n = 0; n < meSwitchEntitiesList.arraySize; n++) {

            SerializedProperty MySwitchEntitiesListRef = meSwitchEntitiesList.GetArrayElementAtIndex(n);
            SerializedProperty switchEntitiesFoldOut = MySwitchEntitiesListRef.FindPropertyRelative("foldOut");
            //SerializedProperty levelLoadMessage = MyLevelManagerListRef.FindPropertyRelative("levelLoadMessage");

            InspectorPropertyListBox(MySwitchEntitiesListRef.FindPropertyRelative("switchEntityName").stringValue, meSwitchEntitiesList, n, inspectorSectionHeaderColor, true, false, switchEntitiesFoldOut);

            if (meSwitchEntitiesList.arraySize == 0 || n > meSwitchEntitiesList.arraySize - 1) {
                break;
            }

            if (switchEntitiesFoldOut.boolValue == false)
                continue;

            if (GUILayout.Button(new GUIContent(" Copy", "Copy Switch Entity"))) {

                PlayerSwitcher.SwitchEntity clone = playerSwitcher.switchEntities[n];
                PlayerSwitcher.SwitchEntity newSwitchEntity = new PlayerSwitcher.SwitchEntity();

                foreach (FieldInfo pi in clone.GetType().GetFields()) {
                    newSwitchEntity.GetType().GetField(pi.Name).SetValue(newSwitchEntity, pi.GetValue(clone));
                }

                playerSwitcher.switchEntities.Add(newSwitchEntity);
            }

            EditorGUILayout.PropertyField(MySwitchEntitiesListRef.FindPropertyRelative("switchEntityName"));
            EditorGUILayout.PropertyField(MySwitchEntitiesListRef.FindPropertyRelative("switchID"));
            EditorGUILayout.PropertyField(MySwitchEntitiesListRef.FindPropertyRelative("entityObject"));

            EditorGUIUtility.labelWidth = 160;
            EditorGUILayout.PropertyField(MySwitchEntitiesListRef.FindPropertyRelative("enableABCAIOnDisable"), new GUIContent("Enable ABC AI On Disable") );
            EditorGUIUtility.labelWidth = 120;
            InspectorHelpBox("If enabled then instead of disabling/enabling ABC Controllers the AI is enabled/disabled instead", false);
         

            EditorGUILayout.Space();

            InspectorHeader("Enabled On Activate");
            InspectorListBox("Components", MySwitchEntitiesListRef.FindPropertyRelative("enabledComponentsOnActivate"));
            InspectorListBox("Objects", MySwitchEntitiesListRef.FindPropertyRelative("enabledObjectsOnActivate"));
            EditorGUILayout.Space();

            InspectorHeader("Disabled On Activate");
            InspectorListBox("Components", MySwitchEntitiesListRef.FindPropertyRelative("disabledComponentsOnActivate"));
            InspectorListBox("Objects", MySwitchEntitiesListRef.FindPropertyRelative("disabledObjectsOnActivate"));
            EditorGUILayout.Space();

            InspectorHeader("Enabled On Deactivate");
            InspectorListBox("Components", MySwitchEntitiesListRef.FindPropertyRelative("enabledComponentsOnDeactivate"));
            InspectorListBox("Objects", MySwitchEntitiesListRef.FindPropertyRelative("enabledObjectsOnDeactivate"));
            EditorGUILayout.Space();

            InspectorHeader("Disabled On Deactivate");
            InspectorListBox("Components", MySwitchEntitiesListRef.FindPropertyRelative("disabledComponentsOnDeactivate"));
            InspectorListBox("Objects" , MySwitchEntitiesListRef.FindPropertyRelative("disabledObjectsOnDeactivate"));

            EditorGUILayout.EndHorizontal();



            EditorGUILayout.Space();

        }

        #endregion


        GetTarget.ApplyModifiedProperties();
    }
}
