using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal; 
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

[CustomEditor(typeof(ABC_ExportedAbilities))]

public class ABC_ExportedAbilities_Editor : Editor {

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
        if (expAbilityCont.showHelpInformation == true) {
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

    public void InspectorSectionHeader(string text, string description = "") {
        Color originalTextColor = GUI.skin.button.normal.textColor;

        GUIStyle myStyle = new GUIStyle("Button");
        if (EditorGUIUtility.isProSkin)
        {
            myStyle.normal.textColor = inspectorSectionHeaderTextProColor;
        }
        else
        {
            myStyle.normal.textColor = inspectorSectionHeaderTextColor;
        }
        myStyle.alignment = TextAnchor.MiddleLeft;
        myStyle.fontStyle = FontStyle.Bold;
        myStyle.fontSize = 13;
        myStyle.wordWrap = true;


        if (EditorGUIUtility.isProSkin)
        {
            GUI.color = inspectorSectionHeaderProColor;
        }
        else
        {
            GUI.color = inspectorSectionHeaderColor;
        }
        GUILayout.Box(" " + text, myStyle, new GUILayoutOption[] { });

        GUI.color = Color.grey;
        GUILayout.Box(" ", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(0.01f) });


        GUI.color = Color.white;
        GUI.skin.box.normal.textColor = originalTextColor;


        if (description != "")
            InspectorHelpBox(description);
    }

    public void InspectorHorizontalSpace(int width) {
        EditorGUILayout.LabelField("", GUILayout.Width(width));
    }


    // Button Icons
    Texture AddIcon;
    Texture RemoveIcon;
    Texture ExportIcon;
    Texture CopyIcon;
    Texture ImportIcon;


    #endregion

    private ReorderableList reorderableListExportedAbilities;

	// our exported ability script
	ABC_ExportedAbilities expAbilityCont;
	SerializedObject GetTarget;
	SerializedProperty meAbilityList;

	// how many exported abilities in list
	int AbilityCount;

	// the index of the current selected ability in the export list
	int? selectedAbilityIndex = null;
	// the current selected ability in the export list
	ABC_Ability selectedAbility;

	// scroll position on export list 
	Vector2 abilityScrollPos;




    void OnEnable(){
		
		expAbilityCont = (ABC_ExportedAbilities)target;
		GetTarget = new SerializedObject (expAbilityCont);
		meAbilityList = GetTarget.FindProperty ("ExportedAbilities"); // Find the List in our script and create a refrence of it 

        AddIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Add");
        RemoveIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Remove");
        CopyIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Copy");
        ExportIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Export");
        ImportIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Import");

        char BranchedSymbol = '\u251C';
        char SmallDownArrowSymbol = '\u25BE';

        #region reorderable list
        // ********************************** ReorderableList Ability *******************************************

        // reorderable list for abilities
        reorderableListExportedAbilities = new ReorderableList(serializedObject, 
		                                                       meAbilityList, 
		                                                       false, false, false, false);


        reorderableListExportedAbilities.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Exported Abilities:" + AbilityCount);
        };

        // design of the reorderable list (with rect parameters we show in a line: active, name, recast time and key) have also added space for user to be able to select line
        reorderableListExportedAbilities.drawElementCallback =  
		(Rect rect, int index, bool isActive, bool isFocused) => {
			var element = reorderableListExportedAbilities.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;

            string name = "";

            if (element.FindPropertyRelative("parentAbilityID").intValue != 0) {
                name = BranchedSymbol.ToString() + " ";
            }

            var test = expAbilityCont.ExportedAbilities.ToList();

            //If the ability is a parent to any hiearchy abilities then show small or right arrow depending on if show children is true
            if (expAbilityCont.ExportedAbilities.Where(a => a.parentAbilityID == element.FindPropertyRelative("abilityID").intValue).Count() > 0) {
                    name += SmallDownArrowSymbol.ToString() + " ";             
            }

            name = name + element.FindPropertyRelative("name").stringValue;  

            EditorGUI.PrefixLabel(
            new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
            1, new GUIContent(name));


            EditorGUI.PrefixLabel(
				new Rect(rect.x + rect.width/2, rect.y, 30, EditorGUIUtility.singleLineHeight),
				1, new GUIContent("Import:"));
			
			EditorGUI.PropertyField(
				new Rect(rect.x + rect.width/2 + rect.width/7, rect.y, 30, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("enableImport"), GUIContent.none);

        };

		// when we select any of the list then it will set the current ability to show the ability details ready to be changed
		reorderableListExportedAbilities.onSelectCallback = (ReorderableList l) => {  
			selectedAbilityIndex = l.index;
			selectedAbility = expAbilityCont.ExportedAbilities [Convert.ToInt32 (selectedAbilityIndex)];
			
		};

        #endregion

    }



    // ********************************** GUI Layout *******************************************

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

        EditorGUIUtility.labelWidth = 140;
		EditorGUIUtility.fieldWidth = 35;
		
		// keep up to date with count
		AbilityCount = meAbilityList.arraySize;

		
		//Update our list
		GetTarget.Update();
		
		
		EditorGUILayout.Space ();


        //EditorGUILayout.PropertyField(GetTarget.FindProperty("showHelpInformation"), new GUIContent("Show Help Boxes"));

        EditorGUILayout.Space();

        InspectorSectionHeader ("Exported Abilities");
		
		EditorGUILayout.Space ();
		
		EditorGUILayout.PropertyField(GetTarget.FindProperty("exportDescription"), GUILayout.MinHeight(30));
		
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Created By: " + GetTarget.FindProperty ("createdBy").stringValue);
		EditorGUILayout.LabelField ("Creation Date: " + GetTarget.FindProperty ("creationDate").stringValue);
		
		EditorGUILayout.Space ();
		
		InspectorHelpBox ("Below shows a list of the exported Abilities. From here you can change the name of any of the abilities and set which abilities to import when loaded.");
		
	
		// if count is over 11 then we will add a scroll view and a max height 
		if (AbilityCount > 11){
			EditorGUILayout.BeginVertical("Box", GUILayout.MaxHeight(300));
		abilityScrollPos = EditorGUILayout.BeginScrollView (abilityScrollPos, false, false);

		} else {
			EditorGUILayout.BeginVertical("Box");
		}
		// show reordable list defined in on enable
		reorderableListExportedAbilities.DoLayoutList();
		if (AbilityCount > 11){
		EditorGUILayout.EndScrollView(); 
		}
		EditorGUILayout.EndVertical();


        if (selectedAbilityIndex != null) {

        if (selectedAbility.iconImage.Texture2D != null) { 

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label(selectedAbility.iconImage.Texture2D, GUILayout.Height(150));
            EditorGUILayout.EndHorizontal();
        }

            EditorGUILayout.BeginVertical("Box");
	


          
            EditorGUILayout.Space();
            GUILayout.Label (selectedAbility.description);

			EditorGUILayout.Space ();
			GUILayout.Label("Starting Position: " + selectedAbility.startingPosition.ToString()); 
			GUILayout.Label("Travel Type: " + selectedAbility.travelType.ToString()); 
			EditorGUILayout.Space ();

            if (GUILayout.Button(new GUIContent(" Remove " + selectedAbility.name, RemoveIcon)) && EditorUtility.DisplayDialog("Delete Exported Ability?", "Are you sure you want to delete " + selectedAbility.name.ToString(), "Yes", "No")) {
				expAbilityCont.ExportedAbilities.RemoveAt (Convert.ToInt32(selectedAbilityIndex));
			}

            EditorGUILayout.EndVertical();

        } else {
			InspectorHelpBox("Select an Ability for more information");
		}
		

    

		
		
		
		//Apply the changes to our list
		GetTarget.ApplyModifiedProperties();
		
	}
	
	
}
