using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;

public class ABC_Controller_EditorWindow : EditorWindow {
    [MenuItem("Window/ABC/Controller Manager")]
    public static void ShowWindow() {
        EditorWindow window = EditorWindow.GetWindow(typeof(ABC_Controller_EditorWindow));
        window.maxSize = new Vector2(windowWidth, windowHeight);
        window.minSize = window.maxSize;
    }



    #region Window Sizes

    static float windowHeight = 660f;
    static float windowWidth = 837f;

    //Width of first column in left part of main body 
    int settingButtonsWidth = 170;

    public int minimumSectionHeight = 0;
    public int minimumSideBySideSectionWidth = 312;
    public int minimumAllWaySectionWidth = 628;

    #endregion

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

    // ************************* Inspector Design Functions ***********************************

    #region Design Functions

    public void InspectorHeader(string text, bool space = true) {

        if (space == true) {
            EditorGUILayout.Space();
        }

        Color originalTextColor = GUI.skin.button.normal.textColor;


        GUIStyle myStyle = new GUIStyle("Box");
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
        myStyle.fontSize = 11;
        myStyle.wordWrap = true;

        if (EditorGUIUtility.isProSkin)
        {
            GUI.color = inspectorSectionHeaderProColor;
        }
        else
        {
            GUI.color = inspectorSectionHeaderColor;
        }


        GUILayout.Box(" " + text, myStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(19) });
        GUI.color = Color.white;
        GUI.skin.box.normal.textColor = originalTextColor;
    }


    public void InspectorSectionHeader(string text, string description = "") {
        GUIStyle myStyle = new GUIStyle("Button");
        Color originalTextColor = GUI.skin.button.normal.textColor;

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
        GUILayout.Box(" " + text, myStyle, new GUILayoutOption[] { GUILayout.MaxWidth(minimumAllWaySectionWidth) });

        GUI.color = Color.grey;
        GUILayout.Box(" ", new GUILayoutOption[] { GUILayout.MaxWidth(minimumAllWaySectionWidth), GUILayout.Height(0.01f) });


        GUI.color = Color.white;
        GUI.skin.box.normal.textColor = originalTextColor;


        if (description != "")
            InspectorHelpBox(description, false, true);
    }

    public void InspectorVerticalBox(bool SideBySide = false) {

        if (EditorGUIUtility.isProSkin)
        {
            GUI.color = inspectorSectionBoxProColor;
        }
        else
        {
            GUI.color = inspectorSectionBoxColor;
        }

        if (SideBySide) {
            EditorGUILayout.BeginVertical("Box", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));
        }
        else {
            EditorGUILayout.BeginVertical("Box", GUILayout.MinWidth(minimumAllWaySectionWidth));
        }


        GUI.color = Color.white;

    }

    public void InspectorHelpBox(string text, bool space = true, bool alwaysShow = false) {
        if (abilityCont.showHelpInformation == true || alwaysShow == true) {

            GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
            myStyle.richText = true;
            myStyle.wordWrap = true;
            myStyle.fixedWidth = 0;

            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = inspectorSectionHelpProColor;
            }
            else
            {
                GUI.color = inspectorSectionHelpColor;
            }
            EditorGUILayout.LabelField(text, myStyle, GUILayout.MaxWidth(minimumAllWaySectionWidth));

            if (space == true) {
                EditorGUILayout.Space();
            }
        }
        else {

            EditorGUILayout.Space();

        }
        GUI.color = Color.white;



    }

    public void InspectorBoldVerticleLine() {
        GUI.color = Color.white;
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.Width(1f), GUILayout.ExpandHeight(true) });
        GUI.color = Color.white;


    }

    public void ResetLabelWidth() {

        EditorGUIUtility.labelWidth = 110;

    }

    public void InspectorListBox(string title, SerializedProperty list, bool expandWidth = false) {

        if (expandWidth) {
            EditorGUILayout.BeginVertical();
        } else {
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
        }

        Color originalTextColor = GUI.skin.button.normal.textColor;


        GUI.color = new Color32(208, 212, 211, 255);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box(title, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(19) });
        GUI.color = Color.white;
        GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
        if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(30))) {
            list.InsertArrayElementAtIndex(list.arraySize);

            if (list.GetArrayElementAtIndex(list.arraySize - 1).type.ToString() == "string")
                list.GetArrayElementAtIndex(list.arraySize - 1).stringValue = "";
        }
        GUILayout.EndHorizontal();


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

    public void InspectorPropertyBox(string header, SerializedProperty list, int listIndex, bool UpDown = false) {
        Color originalTextColor = GUI.skin.button.normal.textColor;
        EditorGUILayout.BeginVertical();
        GUI.color = new Color32(208, 212, 211, 255);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box(header, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(19) });

        if (UpDown == true){
            GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
            if (GUILayout.Button(UpArrowSymbol.ToString()))
            {
                list.MoveArrayElement(listIndex, listIndex - 1);
            }
            if (GUILayout.Button(DownArrowSymbol.ToString()))
            {
                list.MoveArrayElement(listIndex, listIndex + 1);
            }
        }

        GUI.skin.button.normal.textColor = Color.red;

        if (GUILayout.Button("X", GUILayout.Width(30))) {
            list.DeleteArrayElementAtIndex(listIndex);
        }
        GUILayout.EndHorizontal();

        GUI.color = Color.white;
        GUI.skin.button.normal.textColor = originalTextColor;
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    // places an ability select list 
    public void InspectorAbilityListBox(string title, SerializedProperty list) {
        Color originalTextColor = GUI.skin.button.normal.textColor;

        EditorGUILayout.BeginVertical();
        GUI.color = new Color32(208, 212, 211, 255);
        GUILayout.BeginHorizontal("box");


        //List<string> abilityList = new List<string>();


        GUI.color = Color.white;
        abilityListChoice = EditorGUILayout.Popup(title, abilityListChoice, abilityCont.Abilities.Select(item => item.name).ToArray(), GUILayout.Width(250));
        GUI.color = new Color32(208, 212, 211, 255);
        GUI.color = Color.white;
        if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(30))) {

            var stateIndex = list.arraySize;
            list.InsertArrayElementAtIndex(stateIndex);

            SerializedProperty ability = list.GetArrayElementAtIndex(stateIndex);

            ability.intValue = abilityCont.Abilities[abilityListChoice].abilityID;


        }

        GUILayout.EndHorizontal();

        GUI.color = Color.white;

        if (list.arraySize > 0) {
            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < list.arraySize; i++) {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();


                if (abilityCont.Abilities.Where(item => item.abilityID == element.intValue).FirstOrDefault() != null)
                EditorGUILayout.LabelField(abilityCont.Abilities.Where(item => item.abilityID == element.intValue).FirstOrDefault().name);

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


                GUI.skin.button.normal.textColor = originalTextColor;

            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    // places an scroll select list 
    public void InspectorScrollAbilityListBox(string title, SerializedProperty list) {
        Color originalTextColor = GUI.skin.button.normal.textColor;

        EditorGUILayout.BeginVertical();
        GUI.color = new Color32(208, 212, 211, 255);
        GUILayout.BeginHorizontal("box");



        GUI.color = Color.white;
        abilityListChoice = EditorGUILayout.Popup(title, abilityListChoice, abilityCont.Abilities.Where(item => item.scrollAbility == true).Select(item => item.name).ToArray(), GUILayout.Width(250));
        GUI.color = new Color32(208, 212, 211, 255);
        GUI.color = Color.white;
        if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(30))) {

            var stateIndex = list.arraySize;
            list.InsertArrayElementAtIndex(stateIndex);

            SerializedProperty ability = list.GetArrayElementAtIndex(stateIndex);

            ability.intValue = abilityCont.Abilities.Where(item => item.scrollAbility == true).ToArray()[abilityListChoice].abilityID;


        }

        GUILayout.EndHorizontal();

        GUI.color = Color.white;

        if (list.arraySize > 0) {
            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < list.arraySize; i++) {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();

                if (abilityCont.Abilities.Where(item => item.abilityID == element.intValue).FirstOrDefault() != null)
                    EditorGUILayout.LabelField(abilityCont.Abilities.Where(item => item.abilityID == element.intValue).FirstOrDefault().name);

                GUI.skin.button.normal.textColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(40))) {
                    list.DeleteArrayElementAtIndex(i);
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();


                GUI.skin.button.normal.textColor = originalTextColor;

            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    // places an group select list 
    public void InspectorAbilityGroupListBox(SerializedProperty list, SerializedProperty listChoice) {
        Color originalTextColor = GUI.skin.button.normal.textColor;

        EditorGUILayout.BeginVertical();
        GUI.color = new Color32(208, 212, 211, 255);
        GUILayout.BeginHorizontal("box");
        //EditorGUILayout.LabelField(title, EditorStyles.boldLabel);



        GUI.color = Color.white;
        listChoice.intValue = EditorGUILayout.Popup("Select Group:", listChoice.intValue, abilityCont.AbilityGroups.Select(item => item.groupName).ToArray());
        GUI.color = Color.white;
        if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(40))) {

            var stateIndex = list.arraySize;
            list.InsertArrayElementAtIndex(stateIndex);

            SerializedProperty groupID = list.GetArrayElementAtIndex(stateIndex);

            groupID.intValue = abilityCont.AbilityGroups[listChoice.intValue].groupID;


        }

        GUILayout.EndHorizontal();

        GUI.color = Color.white;


        if (list.arraySize > 0) {
            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < list.arraySize; i ++) {


                SerializedProperty element = list.GetArrayElementAtIndex(i);

                if (abilityCont.AbilityGroups.Where(item => item.groupID == element.intValue).FirstOrDefault() == null)
                    continue; 

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(abilityCont.AbilityGroups.Where(item => item.groupID == element.intValue).FirstOrDefault().groupName);

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

                GUI.skin.button.normal.textColor = originalTextColor;


            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }



    // symbols used for aesthetics only
    char DiamondSymbol = '\u25C6';
    char UpArrowSymbol = '\u2191';
    char DownArrowSymbol = '\u2193';

    // Button Icons
    Texture AddIcon;
    Texture RemoveIcon;
    Texture ExportIcon;
    Texture CopyIcon;
    Texture ImportIcon;

    Vector2 editorScrollPos;
    Vector2 listScrollPos;
    Vector2 listScrollPosShowAssigned; 

    GUIStyle textureButton = new GUIStyle();

    #endregion


    private ReorderableList reorderableListAIRules;
    private ReorderableList reorderableListIconUIs;
    private ReorderableList reorderableListAbilityGroups;
    private ReorderableList reorderableListWeapons; 

    ABC_Controller abilityCont;
    SerializedObject GetTarget;
    SerializedProperty meAbilityList;
    SerializedProperty meAbilityGroup;
    SerializedProperty meManaList;
    SerializedProperty meIconUI;
    SerializedProperty meAIRule;
    SerializedProperty meWeaponList;

    // string which is used for lists when picking abilities (list of ability names)
    List<string> abilityList = new List<string>();

    // string which is used for lists when picking weapons (list of weapon names)
    List<string> weaponList = new List<string>();

    public int toolbarABCSelection;
    //public string[] toolbarABC = new string[] { "Settings", "Target Settings", "Ability Groups", "AI" };
    public GUIContent[] toolbarABC;


    public int generalSettingsToolbarSelection;
    public string[] generalSettingsToolbar = new string[] { "General & Error", "Mana", "Weapons" , "Scrollable & Reload", "UI Icons", "Diagnostic"};

    public int targetSettingsToolbarSelection; 
    public string[] targetSettingsToolbar = new string[] { "Target & Soft Target", "Auto & Tab Target", "Crosshair"};

    public int aiSettingsToolbarSelection;
    public string[] aiSettingsToolbar = new string[] { "AI Settings", "Rules", "Navigation", "Navigation Animation" };

    public int abilityGroupsSettingsToolbarSelection;
    public string[] abilityGroupsSettingsToolbar = new string[] { "ABC Groups" };

    public int weaponSettingsToolbarSelection;
    public string[] weaponSettingsToolbar = new string[] { "General & Graphic", "Animations", "Ammo & Reload", "Abilities & Groups", "Weapon Block", "Weapon Parry", "Weapon Drop"};


    // ENUM Used for clicing effects in Inspector () 



    // ************************* Settings / Serialized Properties ***********************************

    #region Settings (Serialized Properties)


    int abilityListChoice;

    

    #endregion





    //  ****************************************** Setup Re-Ordablelists and define abilityController************************************************************

    void OnFocus() {

        // get new target 
        GameObject sel = Selection.activeGameObject;

        // get ABC from current target 
        if (sel != null && sel.GetComponent<ABC_Controller>() != null) {
            abilityCont = sel.GetComponent<ABC_Controller>();

            GUIContent titleContent = new GUIContent(sel.gameObject.name + "'s ABC Controller Manager");
            GetWindow<ABC_Controller_EditorWindow>().titleContent = titleContent;
        }

        //If we have controller then setup abilities 
        if (abilityCont != null) {



            //Retrieve the main serialized object. This is the main property which is updated to retrieve current state, fields changed and then modifications applied back to the real object
            GetTarget = new SerializedObject(abilityCont);
            meAbilityList = GetTarget.FindProperty("Abilities"); // Find the List in our script and create a refrence of it 
            meAbilityGroup = GetTarget.FindProperty("AbilityGroups"); // list of Ability Groups
            meManaList = GetTarget.FindProperty("ManaGUIList"); // list of Mana GUI
            meIconUI = GetTarget.FindProperty("IconUIs"); // list of Icon UIs
            meAIRule = GetTarget.FindProperty("AIRules"); // list of AI Rules
            meWeaponList = GetTarget.FindProperty("Weapons"); // list of AI Rules



            // ********************************** Ability List *******************************************
            abilityList.Clear();
            for (int l = 0; l < meAbilityList.arraySize; l++) {
                abilityList.Add(meAbilityList.GetArrayElementAtIndex(l).FindPropertyRelative("name").stringValue);
            }

            // ********************************** Weapon List *******************************************
            weaponList.Clear();
            for (int l = 0; l < meWeaponList.arraySize; l++) {
                weaponList.Add(meWeaponList.GetArrayElementAtIndex(l).FindPropertyRelative("weaponName").stringValue);
            }


            // ********************************** ReorderableList UI Icons *******************************************


            // Create reorderable list to the side 
            IList iconUIslist = abilityCont.IconUIs;


            // reorderable list for abilities
            reorderableListIconUIs = new ReorderableList(iconUIslist,
                                                         typeof(ABC_IconUI),
                                                         true, false, false, false);

            // name the header
            reorderableListIconUIs.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "List Of UI Icons");
            };


            // when the + sign is called it will add a new ability
            reorderableListIconUIs.onAddCallback = (ReorderableList l) => {
                // add standard defaults here
                ABC_IconUI icon = new ABC_IconUI();
                icon.iconID = ABC_Utilities.GenerateUniqueID();
                abilityCont.IconUIs.Add(icon);
            };


            // when we select any of the list then it will set the current ability to show the ability details ready to be changed
            reorderableListIconUIs.onSelectCallback = (ReorderableList l) => {
                abilityCont.CurrentIconUI = l.index;

            };

            reorderableListIconUIs.onReorderCallback = (ReorderableList l) => {

                //get current ability
                ABC_IconUI movedElement = abilityCont.IconUIs[abilityCont.CurrentIconUI];

                //insert it back to l.index where the element was dragged to in the list
                abilityCont.IconUIs.Insert(l.index, movedElement);

                //remove current ability
                abilityCont.IconUIs.Remove(movedElement);
             

      

            };


            // design of the reorderable list 
            reorderableListIconUIs.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                if (index < meIconUI.arraySize) {
                    SerializedProperty MyListRef = meIconUI.GetArrayElementAtIndex(index);
                    SerializedProperty iconName = MyListRef.FindPropertyRelative("iconName");

                    string name = iconName.stringValue;

                    rect.y += 2;

                    EditorGUI.PrefixLabel(
                        new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight),
                        1, new GUIContent(name));
                }
            };

            // ********************************** ReorderableList AI Rules *******************************************


            // Create reorderable list to the side 
            IList aiRulelist = abilityCont.AIRules;


            // reorderable list for abilities
            reorderableListAIRules = new ReorderableList(aiRulelist, 
                                                         typeof(ABC_Controller.AIRule),
                                                         true, false, false, false);

            // name the header
            reorderableListAIRules.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "List Of AI Rules");
            };


            // when the + sign is called it will add a new ability
            reorderableListAIRules.onAddCallback = (ReorderableList l) =>
            {
                // add standard defaults here
                abilityCont.AIRules.Add(new ABC_Controller.AIRule());
            };


            // when we select any of the list then it will set the current ability to show the ability details ready to be changed
            reorderableListAIRules.onSelectCallback = (ReorderableList l) => {
                abilityCont.CurrentAI = l.index;

            };

            reorderableListAIRules.onReorderCallback = (ReorderableList l) => {

                //get current ability
                ABC_Controller.AIRule movedElement = abilityCont.AIRules[abilityCont.CurrentAI];
                //insert it back to l.index where the element was dragged to in the list
                abilityCont.AIRules.Insert(l.index, movedElement);
                //remove current ability
                abilityCont.AIRules.Remove(movedElement);
     

                EditorUtility.SetDirty(abilityCont);

            };



            // design of the reorderable list 
            reorderableListAIRules.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                if (index < meAIRule.arraySize) {
                    SerializedProperty MyListRef = meAIRule.GetArrayElementAtIndex(index);
                    SerializedProperty ruleName = MyListRef.FindPropertyRelative("RuleName");

                    string name = ruleName.stringValue;

                    rect.y += 2;

                    EditorGUI.PrefixLabel(
                        new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight),
                        1, new GUIContent(name));
                }
            };


            // ********************************** ReorderableList Ability Groups *******************************************


            // Create reorderable list to the side 
            IList abilityGrouplist = abilityCont.AbilityGroups;


            // reorderable list for abilities
            reorderableListAbilityGroups = new ReorderableList(abilityGrouplist,
                                                         typeof(ABC_Controller.AbilityGroup),
                                                         true, false, false, false);

            // name the header
            reorderableListAbilityGroups.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "List Of ABC Groups");
            };


            // when the + sign is called it will add a new ability
            reorderableListAbilityGroups.onAddCallback = (ReorderableList l) => {
                // add standard defaults here
                //abilityCont.AIRules.Add(new ABC_Controller.AbilityGroup());
            };


            // when we select any of the list then it will set the current ability to show the ability details ready to be changed
            reorderableListAbilityGroups.onSelectCallback = (ReorderableList l) => {
                abilityCont.CurrentAbilityGroup = l.index;

            };


            reorderableListAbilityGroups.onReorderCallback = (ReorderableList l) => {

                //get current ability
                ABC_Controller.AbilityGroup movedElement = abilityCont.AbilityGroups[abilityCont.CurrentAbilityGroup];

                //insert it back to l.index where the element was dragged to in the list
                abilityCont.AbilityGroups.Insert(l.index, movedElement);

                //remove current ability
                abilityCont.AbilityGroups.Remove(movedElement);

                EditorUtility.SetDirty(abilityCont);


            };



            // design of the reorderable list 
            reorderableListAbilityGroups.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                if (index < meAbilityGroup.arraySize) {
                    SerializedProperty MyListRef = meAbilityGroup.GetArrayElementAtIndex(index);
                    SerializedProperty Name = MyListRef.FindPropertyRelative("groupName");

                    string name = Name.stringValue;

                    rect.y += 2;

                    EditorGUI.PrefixLabel(
                        new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight),
                        1, new GUIContent(name));
                }
            };


            // ********************************** ReorderableList Weapon Settings *******************************************


            // Create reorderable list to the side 
            IList weaponlist = abilityCont.Weapons;


            // reorderable list for abilities
            reorderableListWeapons = new ReorderableList(weaponlist,
                                                         typeof(ABC_Controller.Weapon),
                                                         true, false, false, false);

            // name the header
            reorderableListWeapons.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "List Of Weapons");
            };


            // when the + sign is called it will add a new ability
            reorderableListWeapons.onAddCallback = (ReorderableList l) => {
                // add standard defaults here
                //abilityCont.AIRules.Add(new ABC_Controller.AbilityGroup());
            };


            // when we select any of the list then it will set the current ability to show the ability details ready to be changed
            reorderableListWeapons.onSelectCallback = (ReorderableList l) => {
                abilityCont.CurrentWeapon = l.index;

            };


            reorderableListWeapons.onReorderCallback = (ReorderableList l) => {

                //get current ability
                ABC_Controller.Weapon movedElement = abilityCont.Weapons[abilityCont.CurrentWeapon];

                //insert it back to l.index where the element was dragged to in the list
                abilityCont.Weapons.Insert(l.index, movedElement);

                //remove current ability
                abilityCont.Weapons.Remove(movedElement);

                EditorUtility.SetDirty(abilityCont);



            };



            // design of the reorderable list 
            reorderableListWeapons.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                if (index < meWeaponList.arraySize)
                {
                    SerializedProperty MyListRef = meWeaponList.GetArrayElementAtIndex(index);
                    SerializedProperty Name = MyListRef.FindPropertyRelative("weaponName");

                    string name = Name.stringValue;

                    rect.y += 2;

                    EditorGUI.PrefixLabel(
                        new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight),
                        1, new GUIContent(name));
                }
            };


        }

    }




    void OnGUI() {


        if (abilityCont != null) {

            #region setup



            // formats for UI
            GUI.skin.button.wordWrap = true;
            GUI.skin.label.wordWrap = true;
            EditorStyles.textField.wordWrap = true;
            EditorGUIUtility.labelWidth = 110;
            EditorGUIUtility.fieldWidth = 35;
            


            EditorGUILayout.Space();

            #endregion

            #region Top Bar

                EditorGUILayout.BeginHorizontal();
            //GUILayout.Label(Resources.Load("ABC-EditorIcons/logo", typeof(Texture2D)) as Texture2D, GUILayout.MaxWidth(4));
            toolbarABCSelection = GUILayout.Toolbar(toolbarABCSelection, toolbarABC);
            EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();


            #endregion


            // *************************************** Abilities start here


            #region Body



            switch ((int)toolbarABCSelection) {
             
                case 0:

                    EditorGUILayout.BeginHorizontal();

                    #region Controls
                     EditorGUILayout.BeginVertical(GUILayout.MaxWidth(settingButtonsWidth));


                    #region Selected Group Controls

                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }


                    EditorGUILayout.BeginVertical("Box");


                    GUI.color = Color.white;


                    EditorGUILayout.Space();

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


                    generalSettingsToolbarSelection = GUILayout.SelectionGrid(generalSettingsToolbarSelection, generalSettingsToolbar, 1);

                    GUI.backgroundColor = Color.white;
                    GUI.contentColor = Color.white;


                    EditorGUILayout.Space();


                    EditorGUILayout.EndVertical();
                    #endregion

                    #region UI Icon list
                    if (generalSettingsToolbarSelection == 4) {
                        InspectorHeader("UI Icons List", false);
                        InspectorHelpBox("Select an UI Icon below to configure it.", false);

                        //InspectorHeader("Ability Controls");
                        if (EditorGUIUtility.isProSkin)
                        {
                            GUI.color = inspectorSectionBoxProColor;
                        }
                        else
                        {
                            GUI.color = inspectorSectionBoxColor;
                        }


                        EditorGUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true));



                        GUI.color = Color.white;

                        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos,
                                                                         false,
                                                                         false);



                        reorderableListIconUIs.DoLayoutList();
                        EditorGUILayout.EndScrollView();

                        EditorGUILayout.EndVertical();

                        #region Selected UI Icon Group Controls

                        //InspectorHeader("Ability Controls");
                        if (EditorGUIUtility.isProSkin)
                        {
                            GUI.color = inspectorSectionBoxProColor;
                        }
                        else
                        {
                            GUI.color = inspectorSectionBoxColor;
                        }


                        EditorGUILayout.BeginVertical("Box");


                        GUI.color = Color.white;

                        EditorGUILayout.Space();

                        if (GUILayout.Button(new GUIContent(" Add UI Icon", AddIcon, "Add New UI Icon"))) {

                            // add standard defaults here
                            ABC_IconUI newIcon = new ABC_IconUI();
                            newIcon.iconID = ABC_Utilities.GenerateUniqueID();
                            abilityCont.IconUIs.Add(newIcon);
                        }




                        if (GUILayout.Button(new GUIContent(" Copy UI Icon", CopyIcon, "Copy UI Icon"))) {


                            ABC_IconUI clone = abilityCont.IconUIs[abilityCont.CurrentIconUI];

                            ABC_IconUI newIconUI = new ABC_IconUI();

                            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(clone), newIconUI);

                            newIconUI.iconName = "Copy Of " + newIconUI.iconName;
                            newIconUI.iconID = ABC_Utilities.GenerateUniqueID();


                            abilityCont.IconUIs.Add(newIconUI);


                        }

                        //Delete Ability 
                        if (GUILayout.Button(new GUIContent(" Delete UI Icon", RemoveIcon, "Delete UI Icon")) && EditorUtility.DisplayDialog("Delete UI Icon?", "Are you sure you want to delete " + abilityCont.IconUIs[abilityCont.CurrentIconUI].iconName, "Yes", "No")) {

                            int removeAtPosition = abilityCont.CurrentIconUI;
                            abilityCont.CurrentIconUI = 0;
                            abilityCont.IconUIs.RemoveAt(removeAtPosition);
                        }


                        if (GUILayout.Button(new GUIContent("Load Icon Toolbar", ImportIcon, "Load Default Icon Toolbar"))) {

                            //Clone new bar
                            GameObject iconBar = Instantiate(Resources.Load("ABC-GUIs/ABC_UIIconToolbar", typeof(GameObject))) as GameObject;
                            iconBar.name = "ABC_UIIconToolbar";
                            //Find Icon toolbar
                            GameObject iconToolbar = iconBar.transform.Find("ABC_Toolbar").gameObject;
                            //Find tooltip
                            GameObject toolTip = iconBar.transform.Find("ABC_Tooltip").gameObject;

                            //Load icon toolbar
                            int counter = 0;
                            foreach (Transform instance in iconToolbar.transform) {
                                GameObject iconObj = instance.Find("ABC_Icon"+counter.ToString()).gameObject;
                         

                                if (iconObj == null)
                                    continue;

                                ABC_IconUI iconUI = new ABC_IconUI();
                                //Set as an empty icon
                                iconUI.iconName = "Default Icon";
                                iconUI.iconID = ABC_Utilities.GenerateUniqueID();
                                iconUI.iconType = IconType.EmptyIcon;
                                iconUI.actionType = ActionType.Dynamic;
                                
                                //Attach icon object
                                iconUI.iconObject.GameObject = iconObj;

                                //attach key text 
                                iconUI.keyText.Text = iconObj.transform.Find("ABC_txtKey" + counter.ToString()).GetComponent<Text>();

                                //attach countdown information
                                GameObject iconCoolDown = iconObj.transform.Find("ABC_Countdown" + counter.ToString()).gameObject;
                                iconUI.countdownFillOverlay.Image = iconCoolDown.GetComponent<Image>();
                                iconUI.countdownText.Text = iconCoolDown.transform.Find("ABC_txtCountDown" + counter.ToString()).GetComponent<Text>();

                                //Attach tooltip
                                iconUI.toolTip.GameObject = toolTip;
                                iconUI.toolTipNameText.Text = toolTip.transform.Find("ABC_txtIconName").GetComponent<Text>();
                                iconUI.toolTipDescriptionText.Text = toolTip.transform.Find("ABC_txtIconDescription").GetComponent<Text>();
                                iconUI.toolTipManaText.Text = toolTip.transform.Find("ABC_txtIconMana").GetComponent<Text>();
                                iconUI.toolTipRecastText.Text = toolTip.transform.Find("ABC_txtIconRecast").GetComponent<Text>();

                                abilityCont.IconUIs.Add(iconUI);
                                counter++;



                            }

                        }

                        if (GUILayout.Button(new GUIContent("Load Icon Book", ImportIcon, "Load Default Icon Book"))) {

                            //Clone new bar
                            GameObject iconBar = Instantiate(Resources.Load("ABC-GUIs/ABC_UIIconBook", typeof(GameObject))) as GameObject;
                            iconBar.name = "ABC_UIIconBook";

                            //find Icon book 
                            GameObject iconBook = iconBar.transform.Find("ABC_Book").gameObject.transform.Find("ABC_BookIcons").gameObject;
                            //Find book tooltip
                            GameObject iconBookToolTip = iconBar.transform.Find("ABC_Book").transform.Find("ABC_IconBookTooltip").gameObject;


                            //Load icon book
                            foreach (Transform instance in iconBook.transform) {
                                GameObject iconObj = instance.gameObject;

                                if (iconObj == null)
                                    continue;

                                ABC_IconUI iconUI = new ABC_IconUI();
                                //Set as an empty icon
                                iconUI.iconName = "Default Book Icon";
                                iconUI.iconID = ABC_Utilities.GenerateUniqueID();
                                iconUI.iconType = IconType.AbilityActivation;
                                iconUI.actionType = ActionType.Source;

                                //Attach icon object
                                iconUI.iconObject.GameObject = iconObj;

                                //TODO finish tooltip
                                //Attach tooltip
                                iconUI.toolTip.GameObject = iconBookToolTip;
                                iconUI.toolTipNameText.Text = iconBookToolTip.transform.Find("ABC_txtIconBookName").GetComponent<Text>();
                                iconUI.toolTipDescriptionText.Text = iconBookToolTip.transform.Find("ABC_txtIconBookDescription").GetComponent<Text>();
                                iconUI.toolTipManaText.Text = iconBookToolTip.transform.Find("ABC_txtIconBookMana").GetComponent<Text>();
                                iconUI.toolTipRecastText.Text = iconBookToolTip.transform.Find("ABC_txtIconBookRecast").GetComponent<Text>();

                                abilityCont.IconUIs.Add(iconUI);



                            }



                        }


                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                        #endregion
                    }
                    #endregion

                    EditorGUILayout.EndVertical();

                    #endregion

                    InspectorBoldVerticleLine();

                    #region Settings



                    editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                        false,
                                                                        false);


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


                    EditorGUILayout.BeginVertical();

                    #region General Settings

                    switch ((int)generalSettingsToolbarSelection) {
                        case 0:

                            InspectorSectionHeader("General Settings & Logging");

                            #region SideBySide 

                            EditorGUILayout.BeginHorizontal();

                            #region General Settings

                            InspectorVerticalBox(true);


                            EditorGUIUtility.labelWidth = 270;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityActivationInterval"), new GUIContent("Ability Activation Interval"), GUILayout.MaxWidth(300));
                            InspectorHelpBox("The interval between ability activation.", false);
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("inputComboRecycleInterval"), new GUIContent("Input Combo Recycle Interval"), GUILayout.MaxWidth(300));
                            InspectorHelpBox("The interval between the last recorded input before the combination of recent inputs is recycled to start fresh.", false);

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("globalAbilityPrepareTimeAdjustment"), new GUIContent("Global Ability Prepare Time Adjustment (%)"), GUILayout.MaxWidth(300));
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("globalAbilityCoolDownAdjustment"), new GUIContent("Global Ability Cooldown Adjustment (%)"), GUILayout.MaxWidth(300));

                            EditorGUILayout.Space();

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("globalAbilityMissChance"), new GUIContent("Global Ability Miss Chance (%)"), GUILayout.MaxWidth(300));
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityMissMinOffset"), new GUIContent("Miss Min Offset"), GUILayout.MaxWidth(300));
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityMissMaxOffset"), new GUIContent("Miss Max Offset"), GUILayout.MaxWidth(300));
                            InspectorHelpBox("Chance that an ability can miss targets, when missing the ability will offset from the position it was intended to go to", false); 
                            ResetLabelWidth();

                          

                            ResetLabelWidth();


                            EditorGUILayout.Space();
                            EditorGUIUtility.labelWidth = 245;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("allowAbilitiesToRandomlySwapPositions"), new GUIContent("Enable Ability Random Position Swap"));
                            ResetLabelWidth();

                            if (GetTarget.FindProperty("allowAbilitiesToRandomlySwapPositions").boolValue == true) {
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityRandomPositionSwapInterval"), new GUIContent("Swap Interval"), GUILayout.MaxWidth(200));
                            }
                            InspectorHelpBox("Will randomly swap positions of abilities which are configured to do so.", false);

                            EditorGUILayout.EndVertical();

                            #endregion

                            #region Error Logging

                            InspectorVerticalBox(true);

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("entityCamera"), new GUIContent("Entities Camera"));
                            InspectorHelpBox("If no camera is set the Main one is used", false);

                            EditorGUILayout.BeginHorizontal();
                          
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityLogGUIText"), new GUIContent("GUI Log"));

                            if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                if (GameObject.Find("ABC_GUIs") == null) {
                                Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                               EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");

                                }

                                Text txt = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Text>(true).Where(i => i.name == "txtAbilityLog").FirstOrDefault();

                                if (txt != null)
                                {
                                    GetTarget.FindProperty("abilityLogGUIText").FindPropertyRelative("refVal").objectReferenceValue = txt;
                                    GetTarget.FindProperty("abilityLogGUIText").FindPropertyRelative("refName").stringValue = txt.name;
                                }

                            }

                            EditorGUILayout.EndHorizontal();


                            EditorGUIUtility.labelWidth = 120;


                            EditorGUILayout.Space();
                            if (abilityCont.abilityLogGUIText.Text != null) {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityLogUseDuration"), new GUIContent("Enable Duration"));

                                if (abilityCont.abilityLogUseDuration == true) {
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityLogDuration"), new GUIContent("Log Duration"));

                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityLogMaxLines"), new GUIContent("Max Logging Lines"), GUILayout.Width(200));
                            }


                            InspectorHelpBox("Determine below what will appear on the ability log", false);

                            EditorGUIUtility.labelWidth = 135;
                            EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logReadyToCastAgain"), new GUIContent("Ready to Activate"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logRange"), new GUIContent("Range"));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logFacingTarget"), new GUIContent("Facing Target"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logTargetSelection"), new GUIContent("Target Selection"));
                                EditorGUILayout.EndHorizontal(); 

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logSoftTargetSelection"), new GUIContent("SoftTarget Selection"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logWorldSelection"), new GUIContent("World Selection"));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logAbilityActivationError"), new GUIContent("Activation Error"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logNoMana"), new GUIContent("Not Enough Mana"));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logPreparing"), new GUIContent("Preparing Ability"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logAbilityInterruption"), new GUIContent("Ability Interrupted"));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logInitiating"), new GUIContent("Initiating Ability"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logAbilityActivation"), new GUIContent("Activating Ability"));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logScrollAbilityEquip"), new GUIContent("Scroll Ability Equip"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logAmmoInformation"), new GUIContent("Ammo Information"));
                                EditorGUILayout.EndHorizontal(); 

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logWeaponInformation"), new GUIContent("Weapon Information"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logBlockingInformation"), new GUIContent("Blocking Information"));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("logParryInformation"), new GUIContent("Parry Information"));
                                
                                EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndVertical();

                            #endregion



                            EditorGUILayout.EndHorizontal();

                            #endregion

                            InspectorSectionHeader("Ability Cancel/Interruption & Combat Toggle Settings");

                            #region SideBySide 

                            EditorGUILayout.BeginHorizontal();

                            #region AllWay 

                            #region Ability Cancel/Interruption Settings 

                            InspectorVerticalBox(true);

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("inputCancelEnabled"), new GUIContent("Input Cancel"));

                            EditorGUIUtility.labelWidth = 130;
                            if (abilityCont.inputCancelEnabled == true)
                            {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("inputCancelInputType"), new GUIContent("Input Type"), GUILayout.MaxWidth(230));

                                if (abilityCont.inputCancelInputType == InputType.Key)
                                {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("inputCancelKey"), GUILayout.MaxWidth(230));

                                }
                                else
                                {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("inputCancelButton"), GUILayout.MaxWidth(230));

                                }


                            }


                            InspectorHelpBox("If enabled then pressing the chosen input will cancel abilities and deselect targets", false);


                            EditorGUIUtility.labelWidth = 180;


                            EditorGUILayout.PropertyField(GetTarget.FindProperty("hitsInterruptCasting"), new GUIContent("Hits Can Interrupt Activation"));
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("hitsPreventCasting"), new GUIContent("Hits Can Prevent Activation"));

                            if (GetTarget.FindProperty("hitsPreventCasting").boolValue == true)
                            {
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("hitsPreventCastingDuration"), new GUIContent("Prevent Activation Duration"), GUILayout.MaxWidth(260));
                            }


           


                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            #region AllWay 

                            #region Combat Toggle Settings 

                            InspectorVerticalBox(true);

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("inIdleMode"));
                            InspectorHelpBox("If in idle mode then the entity can not activate abilities.", false);

                            EditorGUIUtility.labelWidth = 190;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("enableIdleModeToggle"));

                            ResetLabelWidth();
                            if (abilityCont.enableIdleModeToggle == true)
                            {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("idleToggleDelay"), new GUIContent("Toggle Delay"), GUILayout.MaxWidth(200));

                        

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("idleToggleInputType"), new GUIContent("Input Type"), GUILayout.Width(200));

                                if (abilityCont.idleToggleInputType == InputType.Key)
                                {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("idleToggleKey"), GUILayout.Width(200));

                                }
                                else
                                {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("idleToggleButton"), GUILayout.Width(200));

                                }
                                EditorGUILayout.Space();


                                EditorGUIUtility.labelWidth = 250;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("deactiveIdleModeOnAbilityInput"), new GUIContent("Deactivate Idle Mode on Ability Input"));
                                ResetLabelWidth();
                                InspectorHelpBox("If enabled then when in idle mode triggering an ability will quickly take entity out of idle mode", false);




                            }


                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            EditorGUILayout.EndHorizontal();

                            #endregion

                      

                            InspectorSectionHeader("Preparing Ability GUI & World Target Indicator Settings");

                            #region SideBySide 



                            EditorGUILayout.BeginHorizontal();

                            #region Preparing Ability GUI

                          
                            InspectorVerticalBox(true);
                            InspectorHelpBox("Add GUI slider and/or text below to be used to show information when entity is preparing.");

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("preparingAbilityGUIBar"), new GUIContent("Preparing Slider"));

                            if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                if (GameObject.Find("ABC_GUIs") == null) {
                                    Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                    EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                }

                                // due to the objects being unactive we have to find them using get components in children
                                Slider slider = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Slider>(true).Where(i => i.name == "sliderPreparing").FirstOrDefault();

                                if (slider != null) { 
                                    GetTarget.FindProperty("preparingAbilityGUIBar").FindPropertyRelative("refVal").objectReferenceValue = slider;
                                    GetTarget.FindProperty("preparingAbilityGUIBar").FindPropertyRelative("refName").stringValue = slider.name;
                                }

                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("preparingAbilityGUIText"), new GUIContent("Preparing Text"));

                            if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                if (GameObject.Find("ABC_GUIs") == null) {
                                    Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                    EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                }

                                // due to the objects being unactive we have to find them using get components in children
                                Slider slider = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Slider>(true).Where(i => i.name == "sliderPreparing").FirstOrDefault();
                                var txt = slider.GetComponentsInChildren<Text>(true).Where(i => i.name == "txtPreparing").FirstOrDefault();

                                if (txt != null)
                                {
                                    GetTarget.FindProperty("preparingAbilityGUIText").FindPropertyRelative("refVal").objectReferenceValue = txt;
                                    GetTarget.FindProperty("preparingAbilityGUIText").FindPropertyRelative("refName").stringValue = txt.name;
                                }

                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();


   
                        

                            EditorGUILayout.EndVertical();

                            #endregion

                            #region World Target Indicator

                            InspectorVerticalBox(true);

                            InspectorHelpBox("Indicates where ability will hit. Only works for abilities with 'Ability Before Target' set.");

                            EditorGUILayout.BeginHorizontal();
                          
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityWorldTargetIndicator"), new GUIContent("In Range"));

                          if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                GetTarget.FindProperty("abilityWorldTargetIndicator").FindPropertyRelative("refVal").objectReferenceValue = Resources.Load("ABC-TargetIndicator/ABC_WorldTargetIndicator");
                                GetTarget.FindProperty("abilityWorldTargetIndicator").FindPropertyRelative("refName").stringValue = Resources.Load("ABC-TargetIndicator/ABC_WorldTargetIndicator").name;
                            }

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("abilityWorldTargetOutRangeIndicator"), new GUIContent("Out Of Range"));
                          if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                GetTarget.FindProperty("abilityWorldTargetOutRangeIndicator").FindPropertyRelative("refVal").objectReferenceValue = Resources.Load("ABC-TargetIndicator/ABC_WorldTargetOutRangeIndicator");
                                GetTarget.FindProperty("abilityWorldTargetOutRangeIndicator").FindPropertyRelative("refName").stringValue = Resources.Load("ABC-TargetIndicator/ABC_WorldTargetOutRangeIndicator").name;
                            }

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();
                            EditorGUILayout.Space();

                            EditorGUILayout.EndVertical();

                            #endregion

                            EditorGUILayout.EndHorizontal();

                            #endregion


                            break;

                        case 1:

                            InspectorSectionHeader("Mana Settings");

                            #region AllWay 

                            #region Mana Settings 

                            InspectorVerticalBox();


                            EditorGUIUtility.labelWidth = 175;

                            InspectorHelpBox("The type of mana system to use - integrations with other assets can be selected here.");
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("manaIntergrationType"), GUILayout.Width(280));
                         

                            EditorGUIUtility.labelWidth = 150;

                            if (((string)GetTarget.FindProperty("manaIntergrationType").enumNames[GetTarget.FindProperty("manaIntergrationType").enumValueIndex]) == "ABC")
                            {
                                InspectorHelpBox("Fill out the below to add mana to the entity. Here you can set the max mana and the float value determining how much mana can regenerate each second.");
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("maxMana"), new GUIContent("Max Mana"), GUILayout.Width(210));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("manaABC"), new GUIContent("Mana"), GUILayout.Width(210));
                            }
                            else
                            {
                                InspectorHelpBox("Game Creator Integration - If enabled then mana value will be retrieved from GC Asset. To work correctly the mana value needs to be added as a GC attribute not a stat");
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("gcManaID"), new GUIContent("GC Mana ID"), GUILayout.Width(200));
                            }

                       

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("manaRegenPerSecond"), new GUIContent("Regen Per Second"), GUILayout.Width(210));
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("fullManaOnEnable"));
                            ResetLabelWidth();
                            InspectorHelpBox("Multiple Mana GUIs can be created below. GUIText will show remaining/max details and the slider will show graphically how much mana remains. You can also choose when the GUI will appear (always or when targeted). If you want to update the same Mana GUI over different entities then make them share the same slider/text making sure that 'Show When Selected' is ticked for both. ");



                            EditorGUILayout.EndVertical();

                            #endregion


                            #endregion


                            EditorGUILayout.Space();

                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button(new GUIContent(" Add Mana GUI", AddIcon, "Add New Mana GUI" ))) {
                                // add standard defaults here
                                abilityCont.ManaGUIList.Add(new ABC_Controller.ManaGUI());

                            }


                
                            EditorGUILayout.EndHorizontal();

                            for (int n = 0; n < meManaList.arraySize; n++) {

                                #region AllWay 

                                #region Mana GUI 

                                InspectorVerticalBox();

                                InspectorPropertyBox("Mana GUI", meManaList, n);

                                if (meManaList.arraySize == 0 || n > meManaList.arraySize - 1) {
                                    break;
                                }


                                SerializedProperty MyManaListRef = meManaList.GetArrayElementAtIndex(n);


                                SerializedProperty manaSlider = MyManaListRef.FindPropertyRelative("manaSlider");
                                SerializedProperty onlyShowSliderWhenSelected = MyManaListRef.FindPropertyRelative("onlyShowSliderWhenSelected");
                                SerializedProperty manaText = MyManaListRef.FindPropertyRelative("manaText");
                                SerializedProperty onlyShowTextWhenSelected = MyManaListRef.FindPropertyRelative("onlyShowTextWhenSelected");


                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(manaSlider);

                                if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                    if (GameObject.Find("ABC_GUIs") == null) {
                                        Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                        EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                    }

                                    Slider slider = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Slider>(true).Where(i => i.name == "sliderMana").FirstOrDefault();

                                    if (slider != null)
                                    {
                                        manaSlider.FindPropertyRelative("refVal").objectReferenceValue = slider;
                                        manaSlider.FindPropertyRelative("refName").stringValue = slider.name;
                                    }
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUIUtility.labelWidth = 150;
                                EditorGUILayout.PropertyField(onlyShowSliderWhenSelected, new GUIContent("Show When Selected"));
                                ResetLabelWidth();
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(manaText);

                                if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No"))
                                {
                                    if (GameObject.Find("ABC_GUIs") == null)
                                    {
                                        Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                        EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                    }



                                    Text txt = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Text>(true).Where(i => i.name == "txtManaValue").FirstOrDefault();

                                    if (txt != null)
                                    {
                                        manaText.FindPropertyRelative("refVal").objectReferenceValue = txt;
                                        manaText.FindPropertyRelative("refName").stringValue = txt.name;
                                    }

                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUIUtility.labelWidth = 150;
                                EditorGUILayout.PropertyField(onlyShowTextWhenSelected, new GUIContent("Show When Selected"));
                                ResetLabelWidth();
                                
                                EditorGUILayout.Space();


                                EditorGUILayout.EndVertical();

                                #endregion


                                #endregion

                            }

                    


                            break;

                        case 2:

                            #region Global Weapon Settings

                            InspectorSectionHeader("Weapon Input Settings");

                            #region AllWay 



                            EditorGUILayout.BeginHorizontal();

                            #region Weapon Settings


                            InspectorVerticalBox();
                            InspectorHelpBox("Setup below the Next and Previous keys which when triggered will cycle through equipping weapons that are enabled.");

                            EditorGUIUtility.labelWidth = 120;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("nextWeapon"));

                            if (abilityCont.nextWeapon == true) {

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("nextWeaponInputType"), new GUIContent("Input Type"), GUILayout.Width(270));

                                EditorGUIUtility.labelWidth = 160;
                                if (abilityCont.nextWeaponInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("nextWeaponKey"),  GUILayout.Width(270));

                                } else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("nextWeaponButton"),  GUILayout.Width(270));

                                }
                                EditorGUILayout.EndHorizontal();


                            }

                            EditorGUILayout.Space();

                            EditorGUIUtility.labelWidth = 120;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("previousWeapon"));


                            if (abilityCont.previousWeapon == true) {

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("previousWeaponInputType"), new GUIContent("Input Type"), GUILayout.Width(270));

                                EditorGUIUtility.labelWidth = 160;
                                if (abilityCont.previousWeaponInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("previousWeaponKey"), GUILayout.Width(270));

                                } else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("previousWeaponButton"), GUILayout.Width(270));

                                }
                                EditorGUILayout.EndHorizontal();


                            }


                            EditorGUILayout.Space();

                            if (GetTarget.FindProperty("nextWeapon").boolValue == true || GetTarget.FindProperty("previousWeapon").boolValue == true) {

                                EditorGUIUtility.labelWidth = 230;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("cycleWeaponsUsingMouseScroll"), GUILayout.Width(270));
                                InspectorHelpBox("If ticked then scrolling the mouse wheel will cycle through equipping weapons. Next Weapon/Previous Weapon has to be ticked to work. Scroll up for next, down for previous");
                            }

                            EditorGUIUtility.labelWidth = 150;

                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("reloadWeaponInputType"), new GUIContent("Reload Input Type"), GUILayout.MaxWidth(250));

                            if (abilityCont.reloadScrollAbilityInputType == InputType.Key) {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("reloadWeaponKey"), new GUIContent("Reload Key"), GUILayout.MaxWidth(250));

                            } else {
                                
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("reloadWeaponButton"), new GUIContent("Reload Button"), GUILayout.MaxWidth(250));

                            }

                            EditorGUILayout.EndHorizontal();

                            InspectorHelpBox("Determine the trigger to reload the current equipped weapon");


                            EditorGUILayout.EndVertical();

                            ResetLabelWidth();

                            #endregion


                            EditorGUILayout.EndHorizontal();

                            #endregion

                            InspectorSectionHeader("Weapon UI Settings");

                            #region AllWay 

                            EditorGUILayout.BeginHorizontal();

                            #region Weapon UI


                            InspectorVerticalBox();

                            InspectorHelpBox("Add an image and text object below to show during play which weapon is equipped and how much ammo it currently has.", false);

                            EditorGUIUtility.labelWidth = 140;

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponImageGUI"), new GUIContent("Image GUI"), GUILayout.MaxWidth(450));

                            if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                if (GameObject.Find("ABC_GUIs") == null) {
                                    Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                    EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                }

                                RawImage texture = GameObject.Find("ABC_GUIs").GetComponentsInChildren<RawImage>(true).Where(i => i.name == "equippedWeaponImage").FirstOrDefault();

                                if (texture != null) {
                                    GetTarget.FindProperty("weaponImageGUI").FindPropertyRelative("refVal").objectReferenceValue = texture;
                                    GetTarget.FindProperty("weaponImageGUI").FindPropertyRelative("refName").stringValue = texture.name;
                                }
                            }

                            EditorGUILayout.EndHorizontal();


                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponAmmoGUIText"), new GUIContent("Ammo GUI"), GUILayout.MaxWidth(450));

                            if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                if (GameObject.Find("ABC_GUIs") == null) {
                                    Instantiate(Resources.Load("ABC/ABC_Default/ABC_GUIs")).name = "ABC_GUIs";
                                    EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                }

                                Text txt = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Text>(true).Where(i => i.name == "equippedWeaponAmmo").FirstOrDefault();

                                if (txt != null) {
                                    GetTarget.FindProperty("weaponAmmoGUIText").FindPropertyRelative("refVal").objectReferenceValue = txt;
                                    GetTarget.FindProperty("weaponAmmoGUIText").FindPropertyRelative("refName").stringValue = txt.name;
                                }

                            }

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();



                            EditorGUILayout.EndVertical();


                            #endregion


                            EditorGUILayout.EndHorizontal();

                            #endregion



                            InspectorSectionHeader("Weapon Block Settings");

                            #region AllWay 



                            EditorGUILayout.BeginHorizontal();

                            #region Weapon Settings


                            InspectorVerticalBox();
                            InspectorHelpBox("Setup below what input needs to be held down to block using the current weapon");

                            EditorGUIUtility.labelWidth = 150;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("enableWeaponBlock"));

                            if (abilityCont.enableWeaponBlock == true) {

                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponBlockInputType"), new GUIContent("Input Type"), GUILayout.Width(240));


                                if (abilityCont.weaponBlockInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponBlockKey"), GUILayout.Width(230));

                                } else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponBlockButton"), GUILayout.Width(230));

                                }
                                EditorGUILayout.EndHorizontal();




                                EditorGUILayout.Space();

                                EditorGUIUtility.labelWidth = 215;

                                InspectorHelpBox("The type of block durability system to use - integrations with other assets can be selected here.");
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("blockDurabilityIntergrationType"), GUILayout.Width(350));


                                EditorGUIUtility.labelWidth = 150;

                                if (((string)GetTarget.FindProperty("blockDurabilityIntergrationType").enumNames[GetTarget.FindProperty("blockDurabilityIntergrationType").enumValueIndex]) == "ABC") {
                                    InspectorHelpBox("Fill out the below to setup block durability to the entity. The entity will stop blocking If the block durability value reaches 0 from blocking abilities.");
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("maxBlockDurability"), new GUIContent("Max Block Durability"), GUILayout.Width(210));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("blockDurabilityABC"), new GUIContent("Block Durability"), GUILayout.Width(210));
                                    EditorGUILayout.EndHorizontal();
                                } else {
                                    InspectorHelpBox("Game Creator Integration - If enabled then block durability value will be retrieved from GC Asset. To work correctly the block durability value needs to be added as a GC attribute not a stat");
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("gcBlockDurabilityID"), new GUIContent("GC Block Durability ID"), GUILayout.Width(260));
                                }


                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("blockDurabilityRegenPerSecond"), new GUIContent("Regen Every Second"), GUILayout.Width(210));
                                EditorGUIUtility.labelWidth = 190;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("blockDurabilityRegenWhenNotBlocking"), new GUIContent("Regen When Not Blocking"));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("fullBlockDurabilityOnEnable"));
                            }
                            ResetLabelWidth();


                            EditorGUILayout.EndVertical();

                            ResetLabelWidth();

                            #endregion


                            EditorGUILayout.EndHorizontal();

                            #endregion


                            InspectorSectionHeader("Weapon Parry Settings");

                            #region AllWay 


                            EditorGUILayout.BeginHorizontal();

                            #region Weapon Settings


                            InspectorVerticalBox();
                            InspectorHelpBox("Setup below what input needs to be pressed to parry using the current weapon");

                            EditorGUIUtility.labelWidth = 150;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("enableWeaponParry"));

                            if (abilityCont.enableWeaponParry == true) {

                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponParryInputType"), new GUIContent("Input Type"), GUILayout.Width(240));


                                if (abilityCont.weaponParryInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponParryKey"), GUILayout.Width(230));

                                } else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponParryButton"), GUILayout.Width(230));

                                }
                                EditorGUILayout.EndHorizontal();




                                EditorGUILayout.Space();

                            }
                            ResetLabelWidth();


                            EditorGUILayout.EndVertical();

                            ResetLabelWidth();

                            #endregion


                            EditorGUILayout.EndHorizontal();

                            #endregion

                            InspectorSectionHeader("Weapon Drop Settings");

                            #region AllWay 

                            EditorGUILayout.BeginHorizontal();

                            #region Weapon Drop Settings


                            InspectorVerticalBox();

                            InspectorHelpBox("Setup the key/button trigger to drop the entities current weapon (if weapon is configured to do so).");

                            EditorGUIUtility.labelWidth = 140;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("allowWeaponDrop"));

                            if (abilityCont.allowWeaponDrop == true) {

                                EditorGUILayout.BeginHorizontal();
                                ResetLabelWidth();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponDropInputType"), new GUIContent("Input Type"), GUILayout.Width(270));

                                EditorGUIUtility.labelWidth = 160;
                                if (abilityCont.weaponDropInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponDropKey"), GUILayout.Width(270));

                                } else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("weaponDropButton"), GUILayout.Width(270));

                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space();

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("restrictDropWeaponCount"), new GUIContent("Restrict Weapon Drop"), GUILayout.Width(270));

                                if (abilityCont.restrictDropWeaponCount == true) {
                                    EditorGUIUtility.labelWidth = 280;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("restrictDropWeaponIfWeaponCountLessOrEqualTo"), new GUIContent("Restrict Weapon Drop If Weapon Count <= to"), GUILayout.Width(350));
                                }

                                InspectorHelpBox("If enabled then the entity can not drop weapons if the entity has less then or equal to x amount of weapons enabled");
                            }

                            EditorGUILayout.Space();



                            EditorGUILayout.EndVertical();


                            #endregion


                            EditorGUILayout.EndHorizontal();

                            #endregion


                            #endregion


                            break; 

                        case 3:

                            InspectorSectionHeader("Ability Scroll Settings");

                            #region AllWay 

                            #region Ability Scroll Settings 

                            InspectorVerticalBox();

                            InspectorHelpBox("The below settings determine which keys are used to scroll through and activate abilities. Only abilities set to be scrollable will be affected.");

                            EditorGUIUtility.labelWidth = 140;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("nextScroll"));

                            if (abilityCont.nextScroll == true) {

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("nextScrollInputType"), new GUIContent("Input Type"), GUILayout.Width(270));

                                if (abilityCont.nextScrollInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("nextScrollKey"), GUILayout.Width(270));

                                }
                                else {
                                   
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("nextScrollButton"), GUILayout.Width(270));

                                }
                                EditorGUILayout.EndHorizontal();


                            }

                            EditorGUILayout.Space();

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("previousScroll"));

                            if (abilityCont.previousScroll == true) {

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("previousScrollInputType"), new GUIContent("Input Type"), GUILayout.Width(270));

                                if (abilityCont.previousScrollInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("previousScrollKey"), GUILayout.Width(270));

                                }
                                else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("previousScrollButton"), GUILayout.Width(270));

                                }
                                EditorGUILayout.EndHorizontal();


                            }

                            InspectorHelpBox("Next and Previous scroll keys when triggered will cycle through all the scroll abilities set. ");



                            EditorGUILayout.PropertyField(GetTarget.FindProperty("activateCurrentScrollAbilityInputType"), new GUIContent("Activate Input Type"), GUILayout.MaxWidth(250));

                            if (abilityCont.activateCurrentScrollAbilityInputType == InputType.Key) {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("activateCurrentScrollAbilityKey"), new GUIContent("Activate Key"), GUILayout.MaxWidth(250));

                            }
                            else {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("activateCurrentScrollAbilityButton"), new GUIContent("Activate Button"), GUILayout.MaxWidth(250));

                            }


                            InspectorHelpBox("Will activate the current scroll ability");

                            EditorGUIUtility.labelWidth = 140;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("playScrollAnimationOnEnable"), new GUIContent("Animation On Enable"));
                            ResetLabelWidth();
                            InspectorHelpBox("Animation On Enable if selected will play the equipping animation when the character is enabled. Turn off if you don't want them to equip the first default ability when respawning etc");

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("scrollAbilityImageGUI"), new GUIContent("Image GUI"), GUILayout.MaxWidth(450));

                          if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                if (GameObject.Find("ABC_GUIs") == null) {
                                    Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                    EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                }

                                RawImage texture = GameObject.Find("ABC_GUIs").GetComponentsInChildren<RawImage>(true).Where(i => i.name == "currentScrollAbilityImage").FirstOrDefault();

                                if (texture != null) { 
                                    GetTarget.FindProperty("scrollAbilityImageGUI").FindPropertyRelative("refVal").objectReferenceValue = texture;
                                    GetTarget.FindProperty("scrollAbilityImageGUI").FindPropertyRelative("refName").stringValue = texture.name;
                                }
                            }

                            EditorGUILayout.EndHorizontal();


                            EditorGUILayout.Space();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("scrollAbilityammoGUIText"), new GUIContent("Ammo GUI"), GUILayout.MaxWidth(450));

                          if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                if (GameObject.Find("ABC_GUIs") == null) {
                                    Instantiate(Resources.Load("ABC/ABC_Default/ABC_GUIs")).name = "ABC_GUIs";
                                    EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                }

                                Text txt = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Text>(true).Where(i => i.name == "currentScrollAbilityAmmo").FirstOrDefault();

                                if (txt != null)
                                {
                                    GetTarget.FindProperty("scrollAbilityammoGUIText").FindPropertyRelative("refVal").objectReferenceValue = txt;
                                    GetTarget.FindProperty("scrollAbilityammoGUIText").FindPropertyRelative("refName").stringValue = txt.name;
                                }
                               
                            }

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();


                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            InspectorSectionHeader("Ammo Reload Settings");

                            #region AllWay 

                            #region Ammo Reload Settings 

                            InspectorVerticalBox();

                            InspectorHelpBox("The below settings determine the key to reload the ammo on any scroll abilities.");

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("reloadScrollAbilityInputType"), new GUIContent("Input Type"), GUILayout.MaxWidth(250));

                            if (abilityCont.reloadScrollAbilityInputType == InputType.Key) {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("reloadScrollAbilityKey"), new GUIContent("Key"), GUILayout.MaxWidth(250));

                            }
                            else {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("reloadScrollAbilityButton"), new GUIContent("Button"), GUILayout.MaxWidth(250));

                            }


                            EditorGUILayout.Space();



                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            break;

                        case 4:


                            // display details for the selected UI
                            int ui = abilityCont.CurrentIconUI;

                            if (meIconUI.arraySize == 0 || ui > meIconUI.arraySize - 1) {
                                break;
                            }

                            #region Properties
                            SerializedProperty meIconUIRef = meIconUI.GetArrayElementAtIndex(ui);
                            SerializedProperty foldOut = meIconUIRef.FindPropertyRelative("foldOut");
                            SerializedProperty iconName = meIconUIRef.FindPropertyRelative("iconName");
                            SerializedProperty iconID = meIconUIRef.FindPropertyRelative("iconID");
                            SerializedProperty iconType = meIconUIRef.FindPropertyRelative("iconType");
                            SerializedProperty iconObject = meIconUIRef.FindPropertyRelative("iconObject");

                            SerializedProperty actionType = meIconUIRef.FindPropertyRelative("actionType");
                            SerializedProperty clickFromTrigger = meIconUIRef.FindPropertyRelative("clickFromTrigger");
                            SerializedProperty clickFromTriggerOnKeyPress = meIconUIRef.FindPropertyRelative("clickFromTriggerOnKeyPress");
                            SerializedProperty clickFromTriggerKeyDown = meIconUIRef.FindPropertyRelative("clickFromTriggerKeyDown");
                            SerializedProperty clickFromTriggerInputType = meIconUIRef.FindPropertyRelative("clickFromTriggerInputType");
                            SerializedProperty clickTriggerKey = meIconUIRef.FindPropertyRelative("clickTriggerKey"); 
                            SerializedProperty clickTriggerButton = meIconUIRef.FindPropertyRelative("clickTriggerButton");
                            SerializedProperty keyText = meIconUIRef.FindPropertyRelative("keyText");
                            SerializedProperty overrideKeyText = meIconUIRef.FindPropertyRelative("overrideKeyText");
                            SerializedProperty keyTextOverride = meIconUIRef.FindPropertyRelative("keyTextOverride");

                            SerializedProperty iconUIAbilityID = meIconUIRef.FindPropertyRelative("iconUIAbilityID");
                            SerializedProperty IconUIAbilityListChoice = meIconUIRef.FindPropertyRelative("IconUIAbilityListChoice");
                            SerializedProperty disableWithAbility = meIconUIRef.FindPropertyRelative("disableWithAbility");
                            SerializedProperty substituteAbilityWhenDisabled = meIconUIRef.FindPropertyRelative("substituteAbilityWhenDisabled");
                            SerializedProperty iconUISubstituteAbilityIDs = meIconUIRef.FindPropertyRelative("iconUISubstituteAbilityIDs");
                            SerializedProperty IconUISubstituteAbilityListChoice = meIconUIRef.FindPropertyRelative("IconUISubstituteAbilityListChoice");
                            SerializedProperty displayCountdown = meIconUIRef.FindPropertyRelative("displayCountdown");
                            SerializedProperty countdownFillOverlay = meIconUIRef.FindPropertyRelative("countdownFillOverlay");
                            SerializedProperty countdownText = meIconUIRef.FindPropertyRelative("countdownText");

                            SerializedProperty toolTip = meIconUIRef.FindPropertyRelative("toolTip");
                            SerializedProperty toolTipShowOnHover = meIconUIRef.FindPropertyRelative("toolTipShowOnHover");
                            SerializedProperty toolTipShowAtMousePosition = meIconUIRef.FindPropertyRelative("toolTipShowAtMousePosition");
                            SerializedProperty toolTipNameText = meIconUIRef.FindPropertyRelative("toolTipNameText");
                            SerializedProperty toolTipDescriptionText = meIconUIRef.FindPropertyRelative("toolTipDescriptionText");
                            SerializedProperty toolTipManaText = meIconUIRef.FindPropertyRelative("toolTipManaText");
                            SerializedProperty toolTipRecastText = meIconUIRef.FindPropertyRelative("toolTipRecastText");

                            SerializedProperty ScrollAbilityActivationTexture = meIconUIRef.FindPropertyRelative("ScrollAbilityActivationTexture");

                            SerializedProperty iconUIWeaponID = meIconUIRef.FindPropertyRelative("iconUIWeaponID");
                            SerializedProperty IconUIWeaponListChoice = meIconUIRef.FindPropertyRelative("IconUIWeaponListChoice");
                            SerializedProperty disableWithWeapon = meIconUIRef.FindPropertyRelative("disableWithWeapon");
                            SerializedProperty weaponQuickEquip = meIconUIRef.FindPropertyRelative("weaponQuickEquip");

                            SerializedProperty removeOnEmptyDrag = meIconUIRef.FindPropertyRelative("removeOnEmptyDrag");
                            SerializedProperty isClickable = meIconUIRef.FindPropertyRelative("isClickable");
                            #endregion

                    
                            string UIIconPropertyTitle = "UI Icon " + (string)iconType.enumNames[iconType.enumValueIndex];
                            InspectorSectionHeader(iconName.stringValue + " General Settings");


                            #region SideBySide 

                            EditorGUILayout.BeginHorizontal();

                            #region UI Icon Settings

                            InspectorVerticalBox(true);
                            InspectorHelpBox("Create a name for future reference, select the icon type and select a gameobject to become the icon.", false);
                            EditorGUILayout.LabelField("ID: " + iconID.intValue.ToString());
                            EditorGUILayout.PropertyField(iconName, GUILayout.MaxWidth(250));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(iconType, GUILayout.MaxWidth(250));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(iconObject, GUILayout.MaxWidth(250));


                            EditorGUILayout.EndVertical();

                            if (((string)iconType.enumNames[iconType.enumValueIndex]) == IconType.EmptyIcon.ToString()) {
                                actionType.enumValueIndex = 0;
                            }

                                #endregion

                            #region Icon Action Type

                                InspectorVerticalBox(true);
                            InspectorHelpBox("Select characteristics and action type which will determine what happens when the icon is dragged/dropped etc");
                            EditorGUILayout.PropertyField(actionType, GUILayout.MaxWidth(250));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(isClickable, new GUIContent("Clickable"));

                            if (((string)actionType.enumNames[actionType.enumValueIndex]) == ActionType.Dynamic.ToString()) {
                                EditorGUILayout.PropertyField(removeOnEmptyDrag, new GUIContent("Removeable"));
                            }

                            EditorGUILayout.EndVertical();

                            #endregion

                            EditorGUILayout.EndHorizontal();

                            #endregion

                            #region AllWay

                            #region Key Assignment
                            InspectorVerticalBox();

                            InspectorHelpBox("Assign an text object below which will show the key which will trigger the Button. You can also set to enable click from trigger, which will have the icon 'clicked' when a key or button is pressed.", false);
                                EditorGUILayout.PropertyField(keyText, GUILayout.MaxWidth(350));
                    

                            if (((string)actionType.enumNames[actionType.enumValueIndex]) != ActionType.Source.ToString()) {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUIUtility.labelWidth = 135;                    
                                EditorGUILayout.PropertyField(clickFromTrigger, new GUIContent("Click From Trigger"), GUILayout.MaxWidth(150));
                                ResetLabelWidth();
                                EditorGUILayout.Space();
                                if (clickFromTrigger.boolValue == true) {
                                    EditorGUILayout.PropertyField(clickFromTriggerOnKeyPress, new GUIContent("On Key Press"), GUILayout.MaxWidth(150));
                                    EditorGUILayout.PropertyField(clickFromTriggerKeyDown, new GUIContent("On Key Down"), GUILayout.MaxWidth(150));
                                }
                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                if (clickFromTrigger.boolValue == true) {
                                      EditorGUIUtility.labelWidth = 135;
                                    EditorGUILayout.PropertyField(clickFromTriggerInputType, new GUIContent("Input Type"), GUILayout.MaxWidth(250));
                                    ResetLabelWidth();
                                    if (((string)clickFromTriggerInputType.enumNames[clickFromTriggerInputType.enumValueIndex]) == "Key") {
                                        EditorGUILayout.PropertyField(clickTriggerKey, new GUIContent("Key"), GUILayout.MaxWidth(250));
                                    }
                                    else {
                                        EditorGUILayout.PropertyField(clickTriggerButton, new GUIContent("Button"), GUILayout.MaxWidth(250));
                                    }                              
                                 
                                }
                                EditorGUILayout.EndHorizontal();

            
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 135;
                                    EditorGUILayout.PropertyField(overrideKeyText, GUILayout.MaxWidth(150));


                                    if (overrideKeyText.boolValue == true) {
                                        EditorGUILayout.PropertyField(keyTextOverride, GUILayout.MaxWidth(250));
                                    }
                                    ResetLabelWidth();

                                    EditorGUILayout.EndHorizontal();
                                
                            }
                       

                            EditorGUILayout.EndVertical();
                            #endregion

                            #endregion                           


                            if (((string)iconType.enumNames[iconType.enumValueIndex]) == IconType.AbilityActivation.ToString()) {

                                InspectorSectionHeader("Ability Activation Settings");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Set Ability

                                InspectorVerticalBox(true);
                                InspectorHelpBox("Link an ability to activate it when the Icon is selected", false);

                                EditorGUILayout.BeginHorizontal();
                                // show popup of ability 
                                IconUIAbilityListChoice.intValue = EditorGUILayout.Popup("", IconUIAbilityListChoice.intValue, abilityList.ToArray(), GUILayout.MaxWidth(200));

                                if (GUILayout.Button("Update", GUILayout.Width(60))) {

                                    iconUIAbilityID.intValue = abilityCont.Abilities[IconUIAbilityListChoice.intValue].abilityID;


                                }
                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();

                                if (iconUIAbilityID.intValue != -1 && abilityCont.Abilities.Count > 0) {

                                    ABC_Ability ability = abilityCont.Abilities.FirstOrDefault(a => a.abilityID == iconUIAbilityID.intValue);

                                    string name = "Ability Not Set";

                                    if (ability != null)
                                        name = ability.name;

                                    EditorGUILayout.LabelField("Selected Ability: " + name, EditorStyles.boldLabel);
                                }

                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion

                                #region Substitute Ability

                                InspectorVerticalBox(true);

                                InspectorHelpBox("Link abilities to replace the main ability when it's disabled", false);

                                EditorGUIUtility.labelWidth = 220;
                                EditorGUILayout.PropertyField(substituteAbilityWhenDisabled);
                                EditorGUIUtility.labelWidth = 145;
                                if (substituteAbilityWhenDisabled.boolValue == true) {

                                    InspectorAbilityListBox("Substitute Abilities", iconUISubstituteAbilityIDs);

                                }


                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion



                                InspectorSectionHeader("Ability ToolTip, Countdown & Status Settings");
                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region UI Tooltip Settings

                                InspectorVerticalBox(true);

                                InspectorHelpBox("Attach an object to be used as a tooltip. Show on hover will temporarily display the tooltip and ability details whilst the current icon is hovered over.");
                                EditorGUILayout.PropertyField(toolTip);
                                EditorGUIUtility.labelWidth = 160;
                                EditorGUILayout.PropertyField(toolTipShowOnHover, new GUIContent("Show On Hover"));
                                ResetLabelWidth();

                                InspectorHelpBox("Attach text objects to display information about the ability.");
                                EditorGUIUtility.labelWidth = 160;
                                EditorGUILayout.PropertyField(toolTipNameText, new GUIContent("Tool Tip Name"));
                                EditorGUILayout.PropertyField(toolTipDescriptionText, new GUIContent("Tool Tip Description"));
                                EditorGUILayout.PropertyField(toolTipManaText, new GUIContent("Tool Tip Mana"));
                                EditorGUILayout.PropertyField(toolTipRecastText, new GUIContent("Tool Tip Recast"));

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();

                                #endregion

                                if (((string)actionType.enumNames[actionType.enumValueIndex]) != ActionType.Source.ToString()) {
                                    #region Countdown and Status

                                    InspectorVerticalBox(true);
                                    InspectorHelpBox("Settings to determine if the Icon is disabled when the ability is not enabled and also options to display countdown information");

                                    EditorGUIUtility.labelWidth = 140;

                                    EditorGUILayout.PropertyField(disableWithAbility);
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(displayCountdown);



                                    if (displayCountdown.boolValue == true) {


                                        EditorGUILayout.PropertyField(countdownFillOverlay, new GUIContent("Countdown Overlay"));
                                        EditorGUILayout.PropertyField(countdownText);
                                        InspectorHelpBox("Countdown fill overlay should be an image object which will fill down to show the active icon. Example grey with minor transparency which can fill clockwise");

                                    }

                                    ResetLabelWidth();

                                    EditorGUILayout.EndVertical();


                                    EditorGUILayout.EndHorizontal();
                                    #endregion
                                }
                                #endregion

                            }


                            if (((string)iconType.enumNames[iconType.enumValueIndex]) == IconType.ScrollAbilityActivation.ToString()) {

                                InspectorSectionHeader("Scroll Ability Activation Settings");

                                #region All Way
                                InspectorVerticalBox();

                                InspectorHelpBox("Add a texture to the icon. When the icon is activated it will trigger the current scroll ability that is selected.");
                                EditorGUILayout.PropertyField(ScrollAbilityActivationTexture, new GUIContent("Icon Texture"), GUILayout.MaxWidth(250));

                                EditorGUILayout.EndVertical();
                                #endregion
                            }

                            if (((string)iconType.enumNames[iconType.enumValueIndex]) == IconType.WeaponEquip.ToString()) {

                                InspectorSectionHeader("Weapon Equip Settings");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Set Weapon

                                InspectorVerticalBox(true);
                                InspectorHelpBox("Link a weapon to equip it when the Icon is selected", false);

                                EditorGUILayout.BeginHorizontal();
                                // show popup of ability 
                                IconUIWeaponListChoice.intValue = EditorGUILayout.Popup("", IconUIWeaponListChoice.intValue, weaponList.ToArray(), GUILayout.MaxWidth(200));

                                if (GUILayout.Button("Update", GUILayout.Width(60))) {

                                    iconUIWeaponID.intValue = abilityCont.Weapons[IconUIWeaponListChoice.intValue].weaponID;


                                }

                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();

                                if (iconUIWeaponID.intValue != -1 && abilityCont.Weapons.Count > 0) {

                                    ABC_Controller.Weapon weapon = abilityCont.Weapons.FirstOrDefault(w => w.weaponID == iconUIWeaponID.intValue);

                                    string name = "Weapon Not Set";

                                    if (weapon != null)
                                        name = weapon.weaponName;

                                    EditorGUILayout.LabelField("Selected Weapon: " + name, EditorStyles.boldLabel);
                                }

                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion

                                #region Fast Equip Weapon

                                InspectorVerticalBox(true);

                                InspectorHelpBox("If enabled then when the weapon icon is selected it will be equipped instantly, else it will go through normal animation.", false);
                                EditorGUILayout.PropertyField(weaponQuickEquip, new GUIContent("Instant Equip"), GUILayout.MaxWidth(250));


                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion



                                InspectorSectionHeader("Weapon ToolTip Settings");
                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region UI Tooltip Settings

                                InspectorVerticalBox(true);

                                InspectorHelpBox("Attach an object to be used as a tooltip. Show on hover will temporarily display the tooltip and weapon details whilst the current icon is hovered over.");
                                EditorGUILayout.PropertyField(toolTip);
                                EditorGUIUtility.labelWidth = 160;
                                EditorGUILayout.PropertyField(toolTipShowOnHover, new GUIContent("Show On Hover"));
                                ResetLabelWidth();

                                InspectorHelpBox("Attach text objects to display information about the weapon.");
                                EditorGUIUtility.labelWidth = 160;
                                EditorGUILayout.PropertyField(toolTipNameText, new GUIContent("Tool Tip Name"));
                                EditorGUILayout.PropertyField(toolTipDescriptionText, new GUIContent("Tool Tip Description"));

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();

                                #endregion

                                if (((string)actionType.enumNames[actionType.enumValueIndex]) != ActionType.Source.ToString()) {
                                    #region Countdown and Status

                                    InspectorVerticalBox(true);
                                    InspectorHelpBox("Settings to determine if the Icon is disabled when the weapon is not enabled");

                                    EditorGUIUtility.labelWidth = 140;

                                    EditorGUILayout.PropertyField(disableWithWeapon);

                                    ResetLabelWidth();

                                    EditorGUILayout.EndVertical();


                                    EditorGUILayout.EndHorizontal();
                                    #endregion
                                }
                                #endregion

                            }


                            break;

                        case 5:

                            EditorGUILayout.BeginHorizontal();

                            EditorGUIUtility.labelWidth = 180;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("enableDiagnosticLogging"));
                            ResetLabelWidth();

                            if (GUILayout.Button(new GUIContent(" Clear Diagnostic Log", RemoveIcon, "Clear Diagnostic Log"))) {
                                abilityCont.ClearDiagnosticLog();
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUIUtility.labelWidth = 180;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("logDiagnosticToConsole"));
                            ResetLabelWidth();

                            EditorGUILayout.Space();

                        
                            InspectorSectionHeader("Diagnostic Log");
                         
                            foreach (string item in abilityCont.diagnosticLog) {
                                #region AllWay 

                                #region Effect Watcher


                                InspectorVerticalBox();

                                EditorGUILayout.LabelField(item);

                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                            }



                            break; 


                    }


                    #endregion


                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndScrollView();
                    #endregion

                   

                    EditorGUILayout.EndHorizontal();

                    break;


                case 1:

                    EditorGUILayout.BeginHorizontal();

                    #region Controls
                     EditorGUILayout.BeginVertical(GUILayout.MaxWidth(settingButtonsWidth));


                    #region Selected Group Controls

                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }

                    EditorGUILayout.BeginVertical("Box");

                    GUI.color = Color.white;

                    EditorGUILayout.Space();

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



                    targetSettingsToolbarSelection = GUILayout.SelectionGrid(targetSettingsToolbarSelection, targetSettingsToolbar, 1);




                    EditorGUILayout.Space();

                    EditorGUILayout.EndVertical();
                    #endregion


                    EditorGUILayout.EndVertical();

                    #endregion

                    InspectorBoldVerticleLine();

                    #region Settings



                    editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                        false,
                                                                        false);

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


                    EditorGUILayout.BeginVertical();

                    #region General Settings


                    switch ((int)targetSettingsToolbarSelection) {
                        case 0:

                            InspectorSectionHeader("Target Select Settings");

                            #region SideBySide 

                            EditorGUILayout.BeginHorizontal();

                            #region Target Select Type

                            InspectorVerticalBox(true);
                     
                            InspectorHelpBox("The target select type and the method of selecting can be configured below.");


                            EditorGUILayout.PropertyField(GetTarget.FindProperty("targetSelectType"), new GUIContent("Select Type"));


                           EditorGUILayout.BeginHorizontal();                        
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("targetSelectInterval"), new GUIContent("Interval"));
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("targetSelectRange"), new GUIContent("Max Range"));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("targetSelectLayerMask"), new GUIContent("Layer Mask"), GUILayout.Width(250));


                            EditorGUILayout.PropertyField(GetTarget.FindProperty("targetSelectRaiseEvent"), new GUIContent("Raise Event"));
                            InspectorHelpBox("If Interval is 0  then will run with Update. increase for performance but less checks. Max determines how far a target can be selected. This also affects auto targeting.");

                            abilityCont.disableDeselect = EditorGUILayout.Toggle("Disable Deselect", abilityCont.disableDeselect);

                            EditorGUILayout.EndVertical();

                            #endregion



                            #region Target Select Config 

                            InspectorVerticalBox(true);


                            if (abilityCont.targetSelectType.ToString() != "None") {
                                GUILayout.BeginHorizontal();
                                abilityCont.hoverForTarget = EditorGUILayout.Toggle("Hover for Target", abilityCont.hoverForTarget);
                                abilityCont.clickForTarget = EditorGUILayout.Toggle("Click for Target", abilityCont.clickForTarget);
                                GUILayout.EndHorizontal();

                                EditorGUILayout.Space();
                                if (abilityCont.clickForTarget == true) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("clickForTargetInputType"), new GUIContent("Input Type"));

                                    if (abilityCont.clickForTargetInputType == InputType.Key) {
                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("clickForTargetKey"), new GUIContent("Key"));
                                    }   else {

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("clickForTargetButton"), new GUIContent("Button"));
                                    }

                                }


                            
                                EditorGUILayout.Space();

                                EditorGUIUtility.labelWidth = 125;
                                abilityCont.selectSoftTarget = EditorGUILayout.Toggle("Soft Target Override", abilityCont.selectSoftTarget);
 
                                


                                GUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("targetSelectLeeway"), new GUIContent("Select Leeway"));

                                if (abilityCont.targetSelectLeeway == true) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("targetSelectLeewayRadius"), new GUIContent("Leeway Range"));
                                }

                                GUILayout.EndHorizontal();


                                InspectorHelpBox("Higher the leeway range the less accurate the user needs to be to select targets");

                                EditorGUIUtility.labelWidth = 130;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("selectTargetTagOnly"), new GUIContent("Target Tags Only"));
                                ResetLabelWidth();


                                if (abilityCont.selectTargetTagOnly == true) {
                                    InspectorListBox("Target Tags", GetTarget.FindProperty("selectTargetTags"), true);
                                }

                            }
                            else {

                                EditorGUILayout.HelpBox("Select Type is not enabled (None).", MessageType.Warning);

                            }

                            ResetLabelWidth();

                            EditorGUILayout.EndVertical();

                            #endregion

                            EditorGUILayout.EndHorizontal();

                            #endregion


                            #region AllWay 

                            #region Target Select Graphics 

                            InspectorVerticalBox();

                            InspectorHelpBox("The target select indicator can be configured below. This indicates to the player who is currently the target.");

                            GUILayout.BeginHorizontal();
                            EditorGUIUtility.labelWidth = 130;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("selectedTargetShader"), new GUIContent("Target Shader"));
                            ResetLabelWidth();
                            if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                GetTarget.FindProperty("selectedTargetShader").FindPropertyRelative("refVal").objectReferenceValue = (Object)Resources.Load("ABC-TargetIndicator/IndicatorShaders/OutlineGlow");
                                GetTarget.FindProperty("selectedTargetShader").FindPropertyRelative("refName").stringValue = ((Object)Resources.Load("ABC-TargetIndicator/IndicatorShaders/OutlineGlow")).name;

                            }
                            GUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            GUILayout.BeginHorizontal();
                            EditorGUIUtility.labelWidth = 130;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("selectedTargetIndicator"), new GUIContent("Target Indicator"));
                            ResetLabelWidth();
                          if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                GetTarget.FindProperty("selectedTargetIndicator").FindPropertyRelative("refVal").objectReferenceValue = (Object)Resources.Load("ABC-TargetIndicator/ABC_SelectedTarget");
                                GetTarget.FindProperty("selectedTargetIndicator").FindPropertyRelative("refName").stringValue = ((Object)Resources.Load("ABC-TargetIndicator/ABC_SelectedTarget")).name;

                            }
                            GUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            if (abilityCont.selectedTargetIndicator.GameObject != null) {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Indicator Offset", GUILayout.MaxWidth(100));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("targetIndicatorOffset"), new GUIContent(""), GUILayout.MaxWidth(360));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("targetIndicatorForwardOffset"), new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("targetIndicatorRightOffset"), new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space();
                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("targetIndicatorFreezeRotation"), new GUIContent("Stop Indicator Rotate"));

                                GUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("targetIndicatorScaleSize"), new GUIContent("Scale to Target"));
                                EditorGUILayout.Space();
                                if (abilityCont.targetIndicatorScaleSize == true) {
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("targetIndicatorScaleFactor"), new GUIContent("Indicator Scale Size"), GUILayout.MaxWidth(230));
                                }
                                EditorGUILayout.Space();
                               
                                GUILayout.EndHorizontal();
                                ResetLabelWidth();
                            }

                            ResetLabelWidth();
                            EditorGUILayout.Space();
                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion


                            InspectorSectionHeader("Soft Target Settings");

                            #region AllWay 

                            #region Soft Target Settings 

                            InspectorVerticalBox();

                            InspectorHelpBox("When a soft target has been selected pressing the confirmation key will convert them to be the selected target.", false);

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetConfirmInputType"), new GUIContent("Input Type"), GUILayout.MaxWidth(230));

                            if (abilityCont.softTargetConfirmInputType == InputType.Key) {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetConfirmKey"), new GUIContent("Confirm Key"), GUILayout.MaxWidth(230));

                            }
                            else {

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetConfirmButton"), new GUIContent("Confirm Button"), GUILayout.MaxWidth(230));

                            }

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetSetRaiseEvent"), new GUIContent("Raise Event"));
                            EditorGUILayout.EndHorizontal();

                            ResetLabelWidth();

                  

                            #endregion

                            #region Soft Target Graphics 

                            EditorGUILayout.Space();

                            GUILayout.BeginHorizontal();
                            EditorGUIUtility.labelWidth = 130;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetShader"), new GUIContent("Soft Target Shader"));
                            ResetLabelWidth();
                            if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                GetTarget.FindProperty("softTargetShader").FindPropertyRelative("refVal").objectReferenceValue = (Object)Resources.Load("ABC-TargetIndicator/IndicatorShaders/Glow");
                                GetTarget.FindProperty("softTargetShader").FindPropertyRelative("refName").stringValue = ((Object)Resources.Load("ABC-TargetIndicator/IndicatorShaders/Glow")).name;
                            }
                            GUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            GUILayout.BeginHorizontal();
                            EditorGUIUtility.labelWidth = 130;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetIndicator"), new GUIContent("Soft Target Indicator"));
                            ResetLabelWidth();
                          if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                GetTarget.FindProperty("softTargetIndicator").FindPropertyRelative("refVal").objectReferenceValue = (Object)Resources.Load("ABC-TargetIndicator/ABC_SoftTarget");
                                GetTarget.FindProperty("softTargetIndicator").FindPropertyRelative("refName").stringValue = ((Object)Resources.Load("ABC-TargetIndicator/ABC_SoftTarget")).name;
                            }
                            GUILayout.EndHorizontal();

                            EditorGUILayout.Space();


                            if (abilityCont.softTargetIndicator.GameObject != null) {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Indicator Offset", GUILayout.MaxWidth(100));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetIndicatorOffset"), new GUIContent(""), GUILayout.MaxWidth(360));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetIndicatorForwardOffset"), new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetIndicatorRightOffset"), new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space();
                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetIndicatorFreezeRotation"), new GUIContent("Stop Indicator Rotate"));

                                GUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetIndicatorScaleSize"), new GUIContent("Scale to Target"));
                                EditorGUILayout.Space();
                                if (abilityCont.targetIndicatorScaleSize == true) {
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("softTargetIndicatorScaleFactor"), new GUIContent("Indicator Scale Size"));
                                }
                                EditorGUILayout.Space();
                                GUILayout.EndHorizontal();

                            }

                            ResetLabelWidth();

                            EditorGUILayout.Space();
                            EditorGUILayout.EndVertical();



                            #endregion

                            #endregion


                            break;

                        case 1:

                            InspectorSectionHeader("Auto Target Select Settings");

                            #region AllWay

                            #region Auto Target Select Settings 

                            InspectorVerticalBox();

                            InspectorHelpBox("Auto Target will automatically target ABC objects depending on conditions set.");

                            EditorGUIUtility.labelWidth = 130;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetSelect"));

                            EditorGUILayout.Space();

                            if (abilityCont.autoTargetSelect == true) {


                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetType"), new GUIContent("Auto Target Type"), GUILayout.MaxWidth(230));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetInterval"), new GUIContent("Interval"), GUILayout.MaxWidth(230));
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("The type of auto targeting. Auto - Will target when none is selected. Press - Will toggle auto target on off. Hold - Will only have the target selected when the chosen key is held down");

                                if (abilityCont.autoTargetType.ToString() != "Auto") {

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetInputType"), new GUIContent("Input Type"), GUILayout.MaxWidth(230));

                                    if (abilityCont.autoTargetInputType == InputType.Key) {

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetKey"), new GUIContent("Key"), GUILayout.MaxWidth(230));

                                    }  else {

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetButton"), new GUIContent("Button"), GUILayout.MaxWidth(230));
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    InspectorHelpBox("The input used for click and hold target types");
                                }


                                EditorGUILayout.BeginHorizontal();
                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetSoftTarget"), new GUIContent("Soft Target Override"));
                                ResetLabelWidth();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetSwapClosest"), new GUIContent("Swap To Closest"));                         
   
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetInCamera"), new GUIContent("In Camera"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetSelfFacing"), new GUIContent("Player Facing"));
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("Will continue to change target to the closest object that meets conditions note: will decrease performance as the closest target is always being checked for. Will only target those in front of player and/or in camera view");

                                EditorGUIUtility.labelWidth = 130;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("autoTargetTagOnly"), new GUIContent("Target Tags only"));
                                ResetLabelWidth();
                        

                                if (abilityCont.autoTargetTagOnly == true) {
                                    InspectorListBox("Target Tags", GetTarget.FindProperty("autoTargetTags"), true);
                                }

                            }

                            ResetLabelWidth();

                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            InspectorSectionHeader("Tab Through Targets Settings");

                            #region AllWay 

                            #region Tab Through Target Settings 

                            InspectorVerticalBox();

                            InspectorHelpBox("The below settings will allow the user to cycle through near by targets. Requires a initial target to start cycle.");
                    
                            EditorGUIUtility.labelWidth = 150;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("tabThroughTargets"));

                            EditorGUILayout.Space();

                            if (abilityCont.tabThroughTargets == true) {

                                if (abilityCont.autoTargetSelect == false && abilityCont.targetSelectType.ToString() == "None") {
                                    EditorGUILayout.HelpBox("Auto Target is off and Settings are not configured to select targets so tab targeting will only work if an API call selects a target first.", MessageType.Warning);
                                }

                             
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetNextInputType"), new GUIContent("Tab Next Input Type"), GUILayout.MaxWidth(230));


                                if (abilityCont.tabTargetNextInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetNextKey"), new GUIContent("Key"), GUILayout.MaxWidth(250));

                                }else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetNextButton"), new GUIContent("Button"), GUILayout.MaxWidth(250));
                                }

                                EditorGUILayout.Space();
                               

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetPrevInputType"), new GUIContent("Tab Previous Input Type"), GUILayout.MaxWidth(230));

                                if (abilityCont.tabTargetPrevInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetPrevKey"), new GUIContent("Key"), GUILayout.MaxWidth(250));

                                }
                                else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetPrevButton"), new GUIContent("Button"), GUILayout.MaxWidth(250));

                                }
                                
                                InspectorHelpBox("The input used to tab to the next or previous target");

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("tabSoftTarget"), new GUIContent("Soft Target Override"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetInCamera"), new GUIContent("In Camera"));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetToSelf"), new GUIContent("Enable Tab To Self"));
                                EditorGUILayout.EndHorizontal();

            
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("tabTargetTagOnly"));
                                ResetLabelWidth();
              

                                if (abilityCont.tabTargetTagOnly == true) {
                                    InspectorListBox("Tab Target Tags", GetTarget.FindProperty("tabTargetTags"), true);
                                }

                                EditorGUILayout.Space();

                            }

                            ResetLabelWidth();

                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            break;

                        case 2:

                            InspectorSectionHeader("Crosshair Settings");

                            #region AllWay 

                            #region Crosshair Settings 

                            InspectorVerticalBox();

                            InspectorHelpBox("Set below the crosshair texture and position for in game. Crosshair position affects the center point when using FPS targeting.");

                            EditorGUILayout.BeginHorizontal();
                            EditorGUIUtility.labelWidth = 150;
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("crosshairPositionX"), GUILayout.Width(270));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("crosshairPositionY"), GUILayout.Width(270));
                            EditorGUILayout.Space();
                     
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(GetTarget.FindProperty("crosshair"), new GUIContent("Crosshair Texture"));
                            ResetLabelWidth();
                            if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                GetTarget.FindProperty("crosshair").FindPropertyRelative("refVal").objectReferenceValue = (Texture)Resources.Load("ABC-TargetIndicator/ABC_StandardCrossHair");
                                GetTarget.FindProperty("crosshair").FindPropertyRelative("refName").stringValue = ((Texture)Resources.Load("ABC-TargetIndicator/ABC_StandardCrossHair")).name;
                            }

                            GUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            InspectorSectionHeader("Crosshair Override Settings");

                            #region AllWay 

                            #region crosshair override Settings 

                            InspectorVerticalBox();

                            InspectorHelpBox("Different cross to show when triggered. Useful to change to a more accurate crosshair for focus shots etc", false);

                            EditorGUILayout.Space();

                            EditorGUIUtility.labelWidth = 270;

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("showCrossHairOnKey"), new GUIContent("Crosshair Override When Key Pressed"));
                            EditorGUIUtility.labelWidth = 140;

                            if (abilityCont.showCrossHairOnKey == true) {

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("showCrossHairInputType"), new GUIContent("Input Type"), GUILayout.Width(250));
                                EditorGUILayout.Space();
                                if (abilityCont.showCrossHairInputType == InputType.Key) {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("showCrossHairKey"), GUILayout.Width(250));

                                }
                                else {

                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("showCrossHairButton"), GUILayout.Width(250));

                                }
                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("crossHairOverride"), new GUIContent("CrossHair Override"));

                                if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                    GetTarget.FindProperty("crossHairOverride").FindPropertyRelative("refVal").objectReferenceValue = (Texture)Resources.Load("ABC-TargetIndicator/ABC_FocusCrossHair");
                                    GetTarget.FindProperty("crossHairOverride").FindPropertyRelative("refName").stringValue = ((Texture)Resources.Load("ABC-TargetIndicator/ABC_FocusCrossHair")).name;
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();


                            }


                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion


                            InspectorSectionHeader("Crosshair Override Animation & Graphics");





                            #region AllWay 

                            #region CrossHair Aesthetics

                            InspectorVerticalBox();

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("useCrossHairOverrideAesthetics"), new GUIContent("Use Aesthetics"));
                            InspectorHelpBox("Animation which activates when the crosshair override is present.");

                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            if (GetTarget.FindProperty("useCrossHairOverrideAesthetics").boolValue == true) {

                                // serialized properties for crosshair aesthetics
                                #region SP for Crosshair Aesthetics
                                SerializedProperty crossHairOverrideAnimateOnEntity = GetTarget.FindProperty("crossHairOverrideAnimateOnEntity");
                                SerializedProperty crossHairOverrideAnimateOnScrollGraphic = GetTarget.FindProperty("crossHairOverrideAnimateOnScrollGraphic");
                                SerializedProperty crossHairOverrideAnimateOnWeapon = GetTarget.FindProperty("crossHairOverrideAnimateOnWeapon");
                                SerializedProperty crossHairOverrideAnimatorParameter = GetTarget.FindProperty("crossHairOverrideAnimatorParameter");
                                SerializedProperty crossHairOverrideAnimatorParameterType = GetTarget.FindProperty("crossHairOverrideAnimatorParameterType");
                                SerializedProperty crossHairOverrideAnimatorOnValue = GetTarget.FindProperty("crossHairOverrideAnimatorOnValue");
                                SerializedProperty crossHairOverrideAnimatorOffValue = GetTarget.FindProperty("crossHairOverrideAnimatorOffValue");

                                SerializedProperty crossHairOverrideAnimationRunnerClip = GetTarget.FindProperty("crossHairOverrideAnimationRunnerClip");
                                SerializedProperty crossHairOverrideAnimationRunnerClipSpeed = GetTarget.FindProperty("crossHairOverrideAnimationRunnerClipSpeed");
                                SerializedProperty crossHairOverrideAnimationRunnerClipDelay = GetTarget.FindProperty("crossHairOverrideAnimationRunnerClipDelay");
                                SerializedProperty crossHairOverrideAnimationRunnerOnEntity = GetTarget.FindProperty("crossHairOverrideAnimationRunnerOnEntity");
                                SerializedProperty crossHairOverrideAnimationRunnerOnScrollGraphic = GetTarget.FindProperty("crossHairOverrideAnimationRunnerOnScrollGraphic");
                                SerializedProperty crossHairOverrideAnimationRunnerOnWeapon = GetTarget.FindProperty("crossHairOverrideAnimationRunnerOnWeapon");

                                #endregion

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();
                               
                                #region Crosshair Animation Runner 

                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 125;
                                EditorGUILayout.PropertyField(crossHairOverrideAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));
                                InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                if (crossHairOverrideAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null)
                                {


                                    EditorGUIUtility.labelWidth = 225;
                                    EditorGUILayout.PropertyField(crossHairOverrideAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                    EditorGUILayout.PropertyField(crossHairOverrideAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                    EditorGUILayout.PropertyField(crossHairOverrideAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                    InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                    EditorGUIUtility.labelWidth = 75;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(crossHairOverrideAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.PropertyField(crossHairOverrideAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.EndHorizontal();

                                    


                                }

                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                #endregion

                                #region Crosshair Animator
                                InspectorVerticalBox(true);

                                 EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(crossHairOverrideAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(250));
                                InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will repeat until the duration is up");
                                if (crossHairOverrideAnimatorParameter.stringValue != "") {

                                    EditorGUILayout.PropertyField(crossHairOverrideAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                    EditorGUILayout.Space();

                                    EditorGUIUtility.labelWidth = 220;
                                    EditorGUILayout.PropertyField(crossHairOverrideAnimateOnEntity, new GUIContent("Animate on Entity"));
                                    EditorGUILayout.PropertyField(crossHairOverrideAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                    EditorGUILayout.PropertyField(crossHairOverrideAnimateOnWeapon, new GUIContent("Animate on Weapon"));

                                    InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");

                                    EditorGUIUtility.labelWidth = 150;

                                    if (((string)crossHairOverrideAnimatorParameterType.enumNames[crossHairOverrideAnimatorParameterType.enumValueIndex]) != "Trigger") {

                                        // if not trigger we need to know the value to switch on and off
                                        EditorGUILayout.PropertyField(crossHairOverrideAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.PropertyField(crossHairOverrideAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));                                   

                                    }
                                   
                                }



                                EditorGUILayout.EndVertical();
                                #endregion


                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region AllWay 

                                #region crosshair override Graphics 
                                InspectorVerticalBox();

                                InspectorHelpBox("Graphic which activates when the crosshair override is present.");



                                // serialized properties for crosshair aesthetics
                                #region SP for Crosshair Aesthetics
                                SerializedProperty crossHairOverrideAestheticsPositionOffset = GetTarget.FindProperty("crossHairOverrideAestheticsPositionOffset");

                                SerializedProperty crossHairOverrideParticle = GetTarget.FindProperty("crossHairOverrideParticle");
                                SerializedProperty crossHairOverrideObject = GetTarget.FindProperty("crossHairOverrideObject");
                                SerializedProperty crossHairOverrideStartPosition = GetTarget.FindProperty("crossHairOverrideStartPosition");
                                SerializedProperty crossHairOverridePositionOnObject = GetTarget.FindProperty("crossHairOverridePositionOnObject");
                                SerializedProperty crossHairOverridePositionAuxiliarySoftTarget = GetTarget.FindProperty("crossHairOverridePositionAuxiliarySoftTarget");
                                SerializedProperty crossHairOverridePositionOnTag = GetTarget.FindProperty("crossHairOverridePositionOnTag");
                                SerializedProperty crossHairOverrideAestheticsPositionForwardOffset = GetTarget.FindProperty("crossHairOverrideAestheticsPositionForwardOffset");
                                SerializedProperty crossHairOverrideAestheticsPositionRightOffset = GetTarget.FindProperty("crossHairOverrideAestheticsPositionRightOffset");
                                #endregion


                                ResetLabelWidth();
                                EditorGUILayout.PropertyField(crossHairOverrideParticle, new GUIContent("Main Graphic"));
                                EditorGUILayout.PropertyField(crossHairOverrideObject, new GUIContent("Sub Graphic"));


                                EditorGUILayout.Space();
                                EditorGUIUtility.labelWidth = 130;
                                EditorGUILayout.PropertyField(crossHairOverrideStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));

                                if (((string)crossHairOverrideStartPosition.enumNames[crossHairOverrideStartPosition.enumValueIndex]) == "Target")
                                {
                                    EditorGUILayout.PropertyField(crossHairOverridePositionAuxiliarySoftTarget, new GUIContent("Auxiliary SoftTarget"), GUILayout.MaxWidth(350));
                                }

                                ResetLabelWidth();

                                if (((string)crossHairOverrideStartPosition.enumNames[crossHairOverrideStartPosition.enumValueIndex]) == "OnObject")
                                {
                                    EditorGUILayout.PropertyField(crossHairOverridePositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                }

                                if (((string)crossHairOverrideStartPosition.enumNames[crossHairOverrideStartPosition.enumValueIndex]) == "OnTag" || ((string)crossHairOverrideStartPosition.enumNames[crossHairOverrideStartPosition.enumValueIndex]) == "OnSelfTag")
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 10;
                                    EditorGUILayout.LabelField("Select Tag");
                                    ResetLabelWidth();
                                    crossHairOverridePositionOnTag.stringValue = EditorGUILayout.TagField(crossHairOverridePositionOnTag.stringValue, GUILayout.MaxWidth(120));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();
                                }


                                EditorGUILayout.Space();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                EditorGUILayout.PropertyField(crossHairOverrideAestheticsPositionOffset, new GUIContent(""));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(crossHairOverrideAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                EditorGUILayout.PropertyField(crossHairOverrideAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();



                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion
                            }





                            break;

                    }


                    #endregion


                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndScrollView();
                    #endregion



                    EditorGUILayout.EndHorizontal();
                    break;
                case 2:
                    EditorGUILayout.BeginHorizontal();

                    #region Controls
                    EditorGUILayout.BeginVertical(GUILayout.MaxWidth(settingButtonsWidth));

                    #region Ability Group Settings
                    InspectorHeader("ABC Group Settings", false);

                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }

                    EditorGUILayout.BeginVertical("Box");

                    GUI.color = Color.white;

                    EditorGUIUtility.labelWidth = 110;
                    SerializedProperty showAbilitiesInGroup = GetTarget.FindProperty("showAbilitiesInGroup");

                    EditorGUILayout.PropertyField(showAbilitiesInGroup, new GUIContent("Show Assigned"));

                    if (showAbilitiesInGroup.boolValue == true) {

                        listScrollPosShowAssigned = EditorGUILayout.BeginScrollView(listScrollPosShowAssigned,
                                                                  false,
                                                                  false);

                        EditorGUILayout.LabelField("---- Abilities ----");

                        foreach (string abilityName in abilityCont.Abilities.Where(item => item.allowAbilityGroupAssignment == true && item.assignedAbilityGroupIDs.Contains(abilityCont.AbilityGroups[abilityCont.CurrentAbilityGroup].groupID)).Select(value => value.name)) {

                            EditorGUILayout.LabelField("- " + abilityName);

                        }

                        EditorGUILayout.LabelField("---- Weapons ----");

                        foreach (string weaponName in abilityCont.Weapons.Where(item => item.allowWeaponGroupAssignment == true && item.assignedGroupIDs.Contains(abilityCont.AbilityGroups[abilityCont.CurrentAbilityGroup].groupID)).Select(value => value.weaponName)) {

                            EditorGUILayout.LabelField("- " + weaponName);

                        }

                        EditorGUILayout.EndScrollView();

                    }


                    ResetLabelWidth();

                    EditorGUILayout.EndVertical();


                    #endregion

                    #region Ability Groups Selection List


                    InspectorHeader("ABC Groups", false);
                    InspectorHelpBox("Manages groups which link together ABC abilities and weapons to add functionality like enabling all abilities/weapons in a group at once.", false);


                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }

                    EditorGUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true));

                    GUI.color = Color.white;

                    listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos,
                                                                     false,
                                                                     false);



                    reorderableListAbilityGroups.DoLayoutList();
                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.EndVertical();
                    #endregion

                    #region Selected Ability Group Controls

                    //InspectorHeader("Ability Controls");
                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }

                    EditorGUILayout.BeginVertical("Box");

                    GUI.color = Color.white;
                    EditorGUILayout.Space();

                    if (GUILayout.Button(new GUIContent(" Add New Group", AddIcon, "Add New Group"))) {


                        // add standard defaults here
                        ABC_Controller.AbilityGroup newGroup = new ABC_Controller.AbilityGroup();
                        newGroup.groupID = ABC_Utilities.GenerateUniqueID();
                        abilityCont.AbilityGroups.Add(newGroup);
                    }


                    if (GUILayout.Button(new GUIContent(" Copy Selected Group", CopyIcon, "Copy Selected Group"))) {


                        ABC_Controller.AbilityGroup clone = abilityCont.AbilityGroups[abilityCont.CurrentAbilityGroup];

                        ABC_Controller.AbilityGroup newGroup = new ABC_Controller.AbilityGroup();


                        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(clone), newGroup);

                        newGroup.groupName = "Copy Of " + newGroup.groupName;
                        newGroup.groupID = ABC_Utilities.GenerateUniqueID();

                        abilityCont.AbilityGroups.Add(newGroup);


                    }

                    //Delete Ability 
                    if (GUILayout.Button(new GUIContent(" Delete Selected Group", RemoveIcon, "Delete Selected Group")) && EditorUtility.DisplayDialog("Delete Ability Group?", "Are you sure you want to delete " + abilityCont.AbilityGroups[abilityCont.CurrentAbilityGroup].groupName, "Yes", "No")) {

                        int removeAtPosition = abilityCont.CurrentAbilityGroup;
                        abilityCont.CurrentAbilityGroup = 0;
                        abilityCont.AbilityGroups.RemoveAt(removeAtPosition);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    #endregion

                    EditorGUILayout.EndVertical();

                    #endregion

                    InspectorBoldVerticleLine();

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


                    #region Settings



                    editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                        false,
                                                                        false);

                    EditorGUILayout.BeginVertical();

                    #region General Settings



                    switch ((int)abilityGroupsSettingsToolbarSelection) {
                        case 0:


                            #region Settings


                            // display details for that spell
                            int currentAbilityGroup = abilityCont.CurrentAbilityGroup;

                            if (meAbilityGroup.arraySize > 0 && currentAbilityGroup < meAbilityGroup.arraySize) {

                                #region Properties


                                SerializedProperty MyAbilityGrouptRef = meAbilityGroup.GetArrayElementAtIndex(currentAbilityGroup);
                                SerializedProperty groupName = MyAbilityGrouptRef.FindPropertyRelative("groupName");
                                SerializedProperty groupID = MyAbilityGrouptRef.FindPropertyRelative("groupID");
                                SerializedProperty toggleGroup = MyAbilityGrouptRef.FindPropertyRelative("toggleGroup");
                                SerializedProperty groupEnabled = MyAbilityGrouptRef.FindPropertyRelative("groupEnabled");
                                SerializedProperty enableGroupOnStart = MyAbilityGrouptRef.FindPropertyRelative("enableGroupOnStart");
                                SerializedProperty disableGroupOnStart = MyAbilityGrouptRef.FindPropertyRelative("disableGroupOnStart");
                                SerializedProperty enableGroupedAbilitiesOnInput = MyAbilityGrouptRef.FindPropertyRelative("enableGroupedAbilitiesOnInput");
                                SerializedProperty enableOnInputGroupPointsLimitRequired = MyAbilityGrouptRef.FindPropertyRelative("enableOnInputGroupPointsLimitRequired");
                                SerializedProperty abilityGroupEnableInputType = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableInputType");
                                SerializedProperty abilityGroupEnableButton = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableButton");
                                SerializedProperty abilityGroupEnableKey = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableKey");

                                SerializedProperty enableOnScrollAbilitiesActivated = MyAbilityGrouptRef.FindPropertyRelative("enableOnScrollAbilitiesEnabled");
                                SerializedProperty disableOnScrollAbilitiesDectivated = MyAbilityGrouptRef.FindPropertyRelative("disableOnScrollAbilitiesDisabled");
                                SerializedProperty enableOnScrollAbilityIDsActivated = MyAbilityGrouptRef.FindPropertyRelative("enableOnScrollAbilityIDsActivated");
                          
                                SerializedProperty enableGroupViaPoints = MyAbilityGrouptRef.FindPropertyRelative("enableGroupViaPoints");
                                SerializedProperty groupPointLimit = MyAbilityGrouptRef.FindPropertyRelative("groupPointLimit");
                                SerializedProperty currentPointCount = MyAbilityGrouptRef.FindPropertyRelative("currentPointCount");
                                SerializedProperty adjustPointsOverTime = MyAbilityGrouptRef.FindPropertyRelative("adjustPointsOverTime");
                                SerializedProperty pointAdjustmentValue = MyAbilityGrouptRef.FindPropertyRelative("pointAdjustmentValue");
                                SerializedProperty pointAdjustmentInterval = MyAbilityGrouptRef.FindPropertyRelative("pointAdjustmentInterval");
                                SerializedProperty groupPointSlider = MyAbilityGrouptRef.FindPropertyRelative("groupPointSlider");
                                SerializedProperty onlyShowSliderWhenSelected = MyAbilityGrouptRef.FindPropertyRelative("onlyShowSliderWhenSelected");
                                SerializedProperty groupPointText = MyAbilityGrouptRef.FindPropertyRelative("groupPointText");
                                SerializedProperty onlyShowTextWhenSelected = MyAbilityGrouptRef.FindPropertyRelative("onlyShowTextWhenSelected");


                                SerializedProperty abilityGroupEnableDelay = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableDelay");
                                SerializedProperty abilityGroupEnableDuration = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableDuration");
                                SerializedProperty groupEnableDurationSlider = MyAbilityGrouptRef.FindPropertyRelative("groupEnableDurationSlider");
                                SerializedProperty onlyShowEnableDurationSliderWhenSelected = MyAbilityGrouptRef.FindPropertyRelative("onlyShowEnableDurationSliderWhenSelected");
                                SerializedProperty groupEnableDurationText = MyAbilityGrouptRef.FindPropertyRelative("groupEnableDurationText");
                                SerializedProperty onlyShowEnableDurationTextWhenSelected = MyAbilityGrouptRef.FindPropertyRelative("onlyShowEnableDurationTextWhenSelected");

                                SerializedProperty enableNewGroupOnDisable = MyAbilityGrouptRef.FindPropertyRelative("enableNewGroupOnDisable");
                                SerializedProperty enableGroupOnDisableIDs = MyAbilityGrouptRef.FindPropertyRelative("enableGroupOnDisableIDs");
                                SerializedProperty disableNewGroupOnEnable = MyAbilityGrouptRef.FindPropertyRelative("disableNewGroupOnEnable");
                                SerializedProperty disableGroupOnEnableIDs = MyAbilityGrouptRef.FindPropertyRelative("disableGroupOnEnableIDs");
                                SerializedProperty useabilityGroupEnableAesthetics = MyAbilityGrouptRef.FindPropertyRelative("useAbilityGroupEnableAesthetics");
                                SerializedProperty abilityGroupEnableAestheticsPositionOffset = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAestheticsPositionOffset");
                                SerializedProperty abilityGroupEnableAestheticsPositionForwardOffset = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAestheticsPositionForwardOffset");
                                SerializedProperty abilityGroupEnableAestheticsPositionRightOffset = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAestheticsPositionRightOffset");
                                SerializedProperty abilityGroupEnableAnimatorParameter = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimatorParameter");
                                SerializedProperty abilityGroupEnableAnimatorParameterType = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimatorParameterType");
                                SerializedProperty abilityGroupEnableAnimatorOnValue = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimatorOnValue");
                                SerializedProperty abilityGroupEnableAnimatorOffValue = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimatorOffValue");
                                SerializedProperty abilityGroupEnableAnimatorDuration = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimatorDuration");
                                SerializedProperty abilityGroupEnableGraphic = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableGraphic");
                                SerializedProperty abilityGroupEnableSubGraphic = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableSubGraphic");
                                SerializedProperty abilityGroupEnableAestheticDuration = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAestheticDuration"); 
                                SerializedProperty abilityGroupEnableAestheticDelay = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAestheticDelay");
                                SerializedProperty abilityGroupEnableStartPosition = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableStartPosition");
                                SerializedProperty abilityGroupEnablePositionAuxiliarySoftTarget = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnablePositionAuxiliarySoftTarget");
                                SerializedProperty abilityGroupEnablePositionOnObject = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnablePositionOnObject");
                                SerializedProperty abilityGroupEnablePositionOnTag = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnablePositionOnTag");
                                SerializedProperty abilityGroupListChoice = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupListChoice");
                                SerializedProperty weaponEquipListChoice = MyAbilityGrouptRef.FindPropertyRelative("weaponEquipListChoice");

                                SerializedProperty abilityGroupEnableAnimationRunnerClip = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimationRunnerClip");
                                SerializedProperty abilityGroupEnableAnimationRunnerClipSpeed = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimationRunnerClipSpeed");
                                SerializedProperty abilityGroupEnableAnimationRunnerClipDelay = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimationRunnerClipDelay");
                                SerializedProperty abilityGroupEnableAnimationRunnerClipDuration = MyAbilityGrouptRef.FindPropertyRelative("abilityGroupEnableAnimationRunnerClipDuration");

               
                                SerializedProperty equipWeaponOnEnable = MyAbilityGrouptRef.FindPropertyRelative("equipWeaponOnEnable");
                                SerializedProperty equipWeaponOnEnableID = MyAbilityGrouptRef.FindPropertyRelative("equipWeaponOnEnableID");
                                SerializedProperty equipWeaponOnEnableQuickToggle = MyAbilityGrouptRef.FindPropertyRelative("equipWeaponOnEnableQuickToggle");
                                SerializedProperty equipWeaponOnDisable = MyAbilityGrouptRef.FindPropertyRelative("equipWeaponOnDisable");
                                SerializedProperty equipWeaponOnDisableID = MyAbilityGrouptRef.FindPropertyRelative("equipWeaponOnDisableID");
                                SerializedProperty equipWeaponOnDisableQuickToggle = MyAbilityGrouptRef.FindPropertyRelative("equipWeaponOnDisableQuickToggle");

                                #endregion

                                InspectorSectionHeader(groupName.stringValue + " Settings");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Ability Group Settings

                                InspectorVerticalBox(true);                         

                                EditorGUILayout.LabelField("ID: " + groupID.intValue.ToString());
                                EditorGUILayout.PropertyField(groupName);
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(toggleGroup);




                                EditorGUILayout.LabelField("Enabled: " + groupEnabled.boolValue.ToString());
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("If toggle group is selected then the group will toggle on/off");

                                
                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(enableGroupOnStart, new GUIContent("Enable On Start-Up"));
                                EditorGUILayout.PropertyField(disableGroupOnStart, new GUIContent("Disable On Start-Up"));
                                ResetLabelWidth();

                                EditorGUILayout.Space();


                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(abilityGroupEnableDelay, new GUIContent("Enable Delay"));
                                EditorGUILayout.PropertyField(abilityGroupEnableDuration, new GUIContent("Enable Duration"));
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("if duration 0 is set then duration is not applied");

                          

                                EditorGUILayout.EndVertical();


                                ResetLabelWidth();

                                #endregion


                                #region Ability Group Point Settings 

                                InspectorVerticalBox(true);

                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUIUtility.labelWidth = 85;
                                EditorGUILayout.PropertyField(groupPointLimit, new GUIContent("Point Limit"), GUILayout.Width(120));                      
                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(currentPointCount, GUILayout.Width(180));
                              
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(adjustPointsOverTime, new GUIContent("Adjust Over Time"));
                      

                                if (adjustPointsOverTime.boolValue == true) {

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(pointAdjustmentValue, new GUIContent("Adjustment Value"));
                                    EditorGUIUtility.labelWidth = 65;
                                    EditorGUILayout.PropertyField(pointAdjustmentInterval, new GUIContent("Interval"));
                                    ResetLabelWidth();
                                    EditorGUILayout.EndHorizontal();

                                }
                                InspectorHelpBox("If enabled points will adjust over an interval time set. Adjust value can be both positive or negative");


                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(groupPointSlider, new GUIContent("Points Slider"));

                                if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                    if (GameObject.Find("ABC_GUIs") == null) {
                                        Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                        EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                    }

                                    Slider slider = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Slider>(true).Where(i => i.name == "sliderAbilityGroupPoints").FirstOrDefault();

                                    if (slider != null)
                                    {
                                        groupPointSlider.FindPropertyRelative("refVal").objectReferenceValue = slider;
                                        groupPointSlider.FindPropertyRelative("refName").stringValue = slider.name;
                                    }
             
                                }


                                EditorGUILayout.EndHorizontal();

                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(onlyShowSliderWhenSelected, new GUIContent("Show When Selected"));
                                ResetLabelWidth();

                                EditorGUILayout.Space();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(groupPointText, new GUIContent("Points Text"));

                                if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                    if (GameObject.Find("ABC_GUIs") == null) {
                                        Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                        EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                    }

                                    Slider slider = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Slider>(true).Where(i => i.name == "sliderAbilityGroupPoints").FirstOrDefault();
                                    var txt = slider.GetComponentsInChildren<Text>(true).Where(i => i.name == "txtAbilityGroupPointsValue").FirstOrDefault();

                                    if (txt != null)
                                    {
                                        groupPointText.FindPropertyRelative("refVal").objectReferenceValue = txt;
                                        groupPointText.FindPropertyRelative("refName").stringValue = txt.name;
                                    }

                                   
                                }

                                EditorGUILayout.EndHorizontal();

                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(onlyShowTextWhenSelected, new GUIContent("Show When Selected"));
                                ResetLabelWidth();

                     


                                ResetLabelWidth();

                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region AllWay 

                                #region Ability Groups Enable Duration and Delay

                             

                                if (abilityGroupEnableDuration.floatValue > 0) {
                                    InspectorVerticalBox();

                                    InspectorHelpBox("Slider and text will show the remaining duration for the ability group", false);

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 160;
                                    EditorGUILayout.PropertyField(groupEnableDurationSlider, new GUIContent("Enable Duration Slider"));

                                    if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                        if (GameObject.Find("ABC_GUIs") == null) {
                                            Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                            EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                        }

                                        Slider slider = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Slider>(true).Where(i => i.name == "sliderAbilityGroupDuration").FirstOrDefault();

                                        if (slider != null)
                                        {
                                            groupEnableDurationSlider.FindPropertyRelative("refVal").objectReferenceValue = slider;
                                            groupEnableDurationSlider.FindPropertyRelative("refName").stringValue = slider.name;
                                        }

                                    }


                                    EditorGUILayout.EndHorizontal();

                                    EditorGUIUtility.labelWidth = 160;
                                    EditorGUILayout.PropertyField(onlyShowEnableDurationSliderWhenSelected, new GUIContent("Show When Selected"));
                                    ResetLabelWidth();

                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 160;
                                    EditorGUILayout.PropertyField(groupEnableDurationText, new GUIContent("Enable Duration Text"));

                                    if (GUILayout.Button(new GUIContent(ImportIcon, "Load Default"), textureButton, GUILayout.Width(20)) && EditorUtility.DisplayDialog("Load Default", "Loading defaults will overwrite the current property value. Are you sure you want to continue? ", "Yes", "No")) {
                                        if (GameObject.Find("ABC_GUIs") == null) {
                                            Instantiate(Resources.Load("ABC-GUIs/ABC_GUIs")).name = "ABC_GUIs";
                                            EditorUtility.DisplayDialog("Creating ABC_GUIs", "ABC_GUIs will be added to your game. This holds all the default GUI objects used by ABC", "Ok");
                                        }

                                        Slider slider = GameObject.Find("ABC_GUIs").GetComponentsInChildren<Slider>(true).Where(i => i.name == "sliderAbilityGroupDuration").FirstOrDefault();
                                        var txt = slider.GetComponentsInChildren<Text>(true).Where(i => i.name == "txtAbilityGroupDurationValue").FirstOrDefault();

                                        if (txt != null)
                                        {
                                            groupEnableDurationText.FindPropertyRelative("refVal").objectReferenceValue = txt;
                                            groupEnableDurationText.FindPropertyRelative("refName").stringValue = txt.name;
                                        }


                                    }

                                    EditorGUILayout.EndHorizontal();

                                    EditorGUIUtility.labelWidth = 160;
                                    EditorGUILayout.PropertyField(onlyShowEnableDurationTextWhenSelected, new GUIContent("Show When Selected"));
                                    ResetLabelWidth();
                        
                                EditorGUILayout.EndVertical();
                                }


                                #endregion

                                #endregion

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Ability Groups Enable On Input/Point Reached

                                InspectorVerticalBox(true);

                                EditorGUILayout.Space();

                                EditorGUIUtility.labelWidth = 260;
                                EditorGUILayout.PropertyField(enableGroupViaPoints, new GUIContent("Enable When Group Points Reach Limit"));
                                EditorGUIUtility.labelWidth = 140;
                                InspectorHelpBox("If true then the group will be enabled when group points reach the limit. The counter can be incremented via different mechanics. ");

                                EditorGUILayout.PropertyField(enableGroupedAbilitiesOnInput, new GUIContent("Enable On Input"));

                                if (enableGroupedAbilitiesOnInput.boolValue == true) {
                                    EditorGUIUtility.labelWidth = 230;
                                    EditorGUILayout.PropertyField(enableOnInputGroupPointsLimitRequired, new GUIContent("Group Points Limit Reach Required"));
                                    ResetLabelWidth();
                                }


                                EditorGUILayout.Space();

                                if (enableGroupedAbilitiesOnInput.boolValue == true) {
                              
                                    EditorGUILayout.PropertyField(abilityGroupEnableInputType, new GUIContent("Input Type"));

                                    if (((string)abilityGroupEnableInputType.enumNames[abilityGroupEnableInputType.enumValueIndex]) == "Key") {

                                        EditorGUILayout.PropertyField(abilityGroupEnableKey, new GUIContent("Key"));

                                    }
                                    else {

                                        EditorGUILayout.PropertyField(abilityGroupEnableButton, new GUIContent("Button"));

                                    }
                                 
                                }


                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                #endregion


                                #region Ability Groups Enable On Scroll Activation

                                InspectorVerticalBox(true);

                                EditorGUILayout.Space();


                                EditorGUIUtility.labelWidth = 260;
                                EditorGUILayout.PropertyField(enableOnScrollAbilitiesActivated);
                                EditorGUILayout.PropertyField(disableOnScrollAbilitiesDectivated);
                                ResetLabelWidth();
                                InspectorScrollAbilityListBox("Scroll Ability", enableOnScrollAbilityIDsActivated);


                          
                                EditorGUILayout.EndVertical();

                                #endregion



                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region AllWay 

                                #region Ability Groups use aesthetics

                                InspectorVerticalBox();

                                EditorGUILayout.PropertyField(useabilityGroupEnableAesthetics, new GUIContent("Use Aesthetics"));

                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                if (useabilityGroupEnableAesthetics.boolValue == true) {

                                    InspectorSectionHeader("Group Enable Animation & Graphics");

                                    #region SideBySide 

                                    EditorGUILayout.BeginHorizontal();

                                    #region Ability Group Enable Animation Runner 

                                    InspectorVerticalBox(true);

                                    EditorGUILayout.Space();

                                    ResetLabelWidth();
                                    EditorGUILayout.PropertyField(abilityGroupEnableAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));
                                    InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                    if (abilityGroupEnableAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null){
                                                                              
                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(abilityGroupEnableAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(abilityGroupEnableAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.PropertyField(abilityGroupEnableAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.Space();

                                    }

                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();

                                    #endregion


                                    #region  Ability Group Animation Settings

                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 140;

                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(abilityGroupEnableAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(230));
                                    InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will repeat until the duration is up. If 0 Duration then animation will not end.");

                                    if (abilityGroupEnableAnimatorParameter.stringValue != "") {

                                        EditorGUILayout.PropertyField(abilityGroupEnableAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                   

                                        if (((string)abilityGroupEnableAnimatorParameterType.enumNames[abilityGroupEnableAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                        
                                            // if not trigger we need to know the value to switch on and off
                                            EditorGUILayout.PropertyField(abilityGroupEnableAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(abilityGroupEnableAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));
                                        
                                            EditorGUILayout.Space();
                                          
                                            EditorGUILayout.PropertyField(abilityGroupEnableAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));
                                            ResetLabelWidth();                                       

                                        }
                                        
                                    }







                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();



                                    #endregion

           
                                    EditorGUILayout.EndHorizontal();

                                    #endregion

                                    #region AllWay 

                                    #region  Ability Group Graphic Settings

                                    InspectorVerticalBox();

                                    InspectorHelpBox("Define a graphic below which will show when the ability group is enabled");

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(abilityGroupEnableGraphic, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.PropertyField(abilityGroupEnableSubGraphic, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.EndHorizontal();


                                    EditorGUIUtility.labelWidth = 110;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(abilityGroupEnableStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));


                                    if (((string)abilityGroupEnableStartPosition.enumNames[abilityGroupEnableStartPosition.enumValueIndex]) == "Target")
                                    {
                                        EditorGUILayout.PropertyField(abilityGroupEnablePositionAuxiliarySoftTarget, new GUIContent("Auxiliary SoftTarget"), GUILayout.MaxWidth(350));
                                    }

                                    ResetLabelWidth();

                                    if (((string)abilityGroupEnableStartPosition.enumNames[abilityGroupEnableStartPosition.enumValueIndex]) == "OnObject")
                                    {
                                        EditorGUILayout.PropertyField(abilityGroupEnablePositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                    }

                                    if (((string)abilityGroupEnableStartPosition.enumNames[abilityGroupEnableStartPosition.enumValueIndex]) == "OnTag" || ((string)abilityGroupEnableStartPosition.enumNames[abilityGroupEnableStartPosition.enumValueIndex]) == "OnSelfTag")
                                    {

                                        abilityGroupEnablePositionOnTag.stringValue = EditorGUILayout.TagField(abilityGroupEnablePositionOnTag.stringValue, GUILayout.MaxWidth(150));

                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 120;
                                    EditorGUILayout.PropertyField(abilityGroupEnableAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.PropertyField(abilityGroupEnableAestheticDelay, new GUIContent("Graphic Delay"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.EndHorizontal();
                                    ResetLabelWidth();

                                    EditorGUILayout.Space();

                                    EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                    EditorGUILayout.PropertyField(abilityGroupEnableAestheticsPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(abilityGroupEnableAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.PropertyField(abilityGroupEnableAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();




                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();




                                    #endregion

                                    #endregion
                                }

                                InspectorSectionHeader("Enable / Disable Other Groups");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();


                                #region Disable Trigger Ability Group Settings

                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 230;

                                EditorGUILayout.PropertyField(disableNewGroupOnEnable, new GUIContent("Disable Groups When Enabled"));

                                EditorGUIUtility.labelWidth = 140;

                                if (disableNewGroupOnEnable.boolValue == true) {

                                    InspectorAbilityGroupListBox(disableGroupOnEnableIDs, abilityGroupListChoice);

                                }

                                InspectorHelpBox("If ticked then another group can be set to be disabled once this group has been enabled. Allows entity to turn off from normal mode when in power mode etc");

                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();




                                #endregion

                                #region Enable Trigger Ability Group Settings

                                InspectorVerticalBox(true);


                                EditorGUIUtility.labelWidth = 230;
                                EditorGUILayout.PropertyField(enableNewGroupOnDisable, new GUIContent("Enable Groups When Disabled"));

                                EditorGUIUtility.labelWidth = 140;
                                if (enableNewGroupOnDisable.boolValue == true) {

                                    InspectorAbilityGroupListBox(enableGroupOnDisableIDs, abilityGroupListChoice);

                                }


                                EditorGUIUtility.labelWidth = 140;
                                InspectorHelpBox("If ticked then another group can be set to be enabled once this group has been disabled. Allows entity to switch back from power mode to normal etc");

                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();



                                #endregion



                                EditorGUILayout.EndHorizontal();

                                #endregion

                                InspectorSectionHeader("Equip Weapon");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Equip Weapon On Enable

                                InspectorVerticalBox(true);


                                EditorGUIUtility.labelWidth = 230;
                                EditorGUILayout.PropertyField(equipWeaponOnEnable, new GUIContent("Equip Weapon When Enabled"));

                                EditorGUIUtility.labelWidth = 140;
                                if (equipWeaponOnEnable.boolValue == true) {

                                    EditorGUILayout.PropertyField(equipWeaponOnEnableQuickToggle, new GUIContent("Quick Toggle"));

                                    // show popup of weapon 
                                    weaponEquipListChoice.intValue = EditorGUILayout.Popup("Select Weapon:", weaponEquipListChoice.intValue, abilityCont.Weapons.Select(item => item.weaponName).ToArray());

                                    if (GUILayout.Button("Update", GUILayout.Width(60))) {

                                        equipWeaponOnEnableID.intValue = abilityCont.Weapons[weaponEquipListChoice.intValue].weaponID;


                                    }

                                    EditorGUILayout.Space();
                                    if (equipWeaponOnEnableID.intValue != -1 && abilityCont.Weapons.Count > 0) {

                                         ABC_Controller.Weapon weapon = abilityCont.Weapons.FirstOrDefault(w => w.weaponID == equipWeaponOnEnableID.intValue);

                                        string name = "Weapon Not Set";

                                        if (weapon != null)
                                            name = weapon.weaponName;

                                        EditorGUILayout.LabelField("Equipping: " + name, EditorStyles.boldLabel);
                                    }

                                }


                                EditorGUIUtility.labelWidth = 140;
                                InspectorHelpBox("If ticked then a weapon will be equipped when this group is enabled. If Quick Toggle is enabled then the weapon will be equipped instantly without running animations etc");

                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();



                                #endregion


                                #region Equip Weapon On Disable

                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 230;

                                EditorGUILayout.PropertyField(equipWeaponOnDisable, new GUIContent("Equip Weapon When Disabled"));

                                EditorGUIUtility.labelWidth = 140;

                                if (equipWeaponOnDisable.boolValue == true) {


                                    EditorGUILayout.PropertyField(equipWeaponOnDisableQuickToggle, new GUIContent("Quick Toggle"));

                                    // show popup of weapon 
                                    weaponEquipListChoice.intValue = EditorGUILayout.Popup("Select Weapon:", weaponEquipListChoice.intValue, abilityCont.Weapons.Select(item => item.weaponName).ToArray());

                                    if (GUILayout.Button("Update", GUILayout.Width(60))) {

                                        equipWeaponOnDisableID.intValue = abilityCont.Weapons[weaponEquipListChoice.intValue].weaponID;


                                    }

                                    EditorGUILayout.Space();
                                    if (equipWeaponOnDisableID.intValue != -1 && abilityCont.Weapons.Count > 0) {

                                        ABC_Controller.Weapon weapon = abilityCont.Weapons.FirstOrDefault(w => w.weaponID == equipWeaponOnDisableID.intValue);

                                        string name = "Weapon Not Set";

                                        if (weapon != null)
                                            name = weapon.weaponName;

                                        EditorGUILayout.LabelField("Equipping: " + name, EditorStyles.boldLabel);
                                    }


                                }

                                InspectorHelpBox("If ticked then a weapon will be equipped when this group is disabled. If Quick Toggle is enabled then the weapon will be equipped instantly without running animations etc");

                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();




                                #endregion



                                EditorGUILayout.EndHorizontal();

                                #endregion

                            }


                            #endregion

                            break;


                    }


                    #endregion


                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndScrollView();
                    #endregion



                    EditorGUILayout.EndHorizontal();

                    break;
                case 3:
                    EditorGUILayout.BeginHorizontal();

                    #region Controls
                    EditorGUILayout.BeginVertical(GUILayout.MaxWidth(settingButtonsWidth));

                    #region Weapon Global Settings



                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }

                    EditorGUILayout.BeginVertical("Box");

                    GUI.color = Color.white;

                    EditorGUILayout.Space();

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


                    weaponSettingsToolbarSelection = GUILayout.SelectionGrid(weaponSettingsToolbarSelection, weaponSettingsToolbar, 1);

                    GUI.backgroundColor = Color.white;
                    GUI.contentColor = Color.white;

                    EditorGUILayout.Space();

                    EditorGUILayout.EndVertical();


                    #endregion

                    #region Weapons Selection List


                    InspectorHeader("Weapons", false);
                    InspectorHelpBox("Setup and manage Weapons.", false);

                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }

                    EditorGUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true));

                    GUI.color = Color.white;

                    listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos,
                                                                     false,
                                                                     false);



                    reorderableListWeapons.DoLayoutList();
                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.EndVertical();
                    #endregion

                    #region Selected Weapon Controls

                    //InspectorHeader("Ability Controls");


                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }

                    EditorGUILayout.BeginVertical("Box");

                    GUI.color = Color.white;

                    EditorGUILayout.Space();

                    if (GUILayout.Button(new GUIContent(" Add New Weapon", AddIcon, "Add New Weapon")))
                    {

                        ABC_Controller.Weapon newWeapon = new ABC_Controller.Weapon();
                        newWeapon.weaponName = "New Weapon";
                        newWeapon.weaponID = ABC_Utilities.GenerateUniqueID();
                        // add standard defaults here
                        abilityCont.Weapons.Add(newWeapon);

                                       
                    }


                    if (GUILayout.Button(new GUIContent(" Copy Selected Weapon", CopyIcon, "Copy Selected Weapon")))
                    {


                        ABC_Controller.Weapon clone = abilityCont.Weapons[abilityCont.CurrentWeapon];

                        ABC_Controller.Weapon newWeapon = new ABC_Controller.Weapon();

                        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(clone), newWeapon);

                        newWeapon.weaponName = "Copy Of " + newWeapon.weaponName;
                        newWeapon.weaponID = ABC_Utilities.GenerateUniqueID();


                        abilityCont.Weapons.Add(newWeapon);


                    }

                    //Delete Ability 
                    if (GUILayout.Button(new GUIContent(" Delete Selected Weapon", RemoveIcon, "Delete Selected Weapon")) && EditorUtility.DisplayDialog("Delete Weapon?", "Are you sure you want to delete " + abilityCont.Weapons[abilityCont.CurrentWeapon].weaponName, "Yes", "No"))
                    {

                        int removeAtPosition = abilityCont.CurrentWeapon;
                        abilityCont.CurrentWeapon = 0;
                        abilityCont.Weapons.RemoveAt(removeAtPosition);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    #endregion

                    EditorGUILayout.EndVertical();

                    #endregion

                    InspectorBoldVerticleLine();

                    #region Settings



                    editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                        false,
                                                                        false);

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


                    EditorGUILayout.BeginVertical();

                    #region General Settings

                    // display details for that weapon
                    int currentWeapon = abilityCont.CurrentWeapon;

                    if (meWeaponList.arraySize > 0 && currentWeapon < meWeaponList.arraySize)
                    {

                        #region Properties


                        SerializedProperty MyWeaponRef = meWeaponList.GetArrayElementAtIndex(currentWeapon);
                        SerializedProperty weaponName = MyWeaponRef.FindPropertyRelative("weaponName");
                        SerializedProperty weaponID = MyWeaponRef.FindPropertyRelative("weaponID");
                        SerializedProperty weaponDescription = MyWeaponRef.FindPropertyRelative("weaponDescription");
                        SerializedProperty weaponIconImage = MyWeaponRef.FindPropertyRelative("weaponIconImage");

                        SerializedProperty abilitiesListChoice = MyWeaponRef.FindPropertyRelative("abilitiesListChoice");
                        SerializedProperty enableAbilitiesWhenEquipped = MyWeaponRef.FindPropertyRelative("enableAbilitiesWhenEquipped");
                        SerializedProperty disableAllOtherAbilitiesNotListed = MyWeaponRef.FindPropertyRelative("disableAllOtherAbilitiesNotListed");
                        SerializedProperty enableAbilityIDs = MyWeaponRef.FindPropertyRelative("enableAbilityIDs");

                        SerializedProperty disableAbilitiesWhenEquipped = MyWeaponRef.FindPropertyRelative("disableAbilitiesWhenEquipped");
                        SerializedProperty enableAllOtherAbilitiesNotListed = MyWeaponRef.FindPropertyRelative("enableAllOtherAbilitiesNotListed");
                        SerializedProperty disableAbilityIDs = MyWeaponRef.FindPropertyRelative("disableAbilityIDs");


                        SerializedProperty abilityGroupListChoice = MyWeaponRef.FindPropertyRelative("abilityGroupListChoice");
                        SerializedProperty enableAbilityGroupsWhenEquipped = MyWeaponRef.FindPropertyRelative("enableAbilityGroupsWhenEquipped"); 
                        SerializedProperty disableAllOtherGroupsNotListed = MyWeaponRef.FindPropertyRelative("disableAllOtherGroupsNotListed");
                        SerializedProperty enableAbilityGroupIDs = MyWeaponRef.FindPropertyRelative("enableAbilityGroupIDs");

                        SerializedProperty disableAbilityGroupWhenEquipped = MyWeaponRef.FindPropertyRelative("disableAbilityGroupWhenEquipped");
                        SerializedProperty enableAllOtherGroupsNotListed = MyWeaponRef.FindPropertyRelative("enableAllOtherGroupsNotListed");
                        SerializedProperty disableAbilityGroupIDs = MyWeaponRef.FindPropertyRelative("disableAbilityGroupIDs"); 

                        SerializedProperty weaponEnabled = MyWeaponRef.FindPropertyRelative("weaponEnabled");
                        SerializedProperty inspectorEquipWeapon = MyWeaponRef.FindPropertyRelative("inspectorEquipWeapon");
                        SerializedProperty enableDuration = MyWeaponRef.FindPropertyRelative("enableDuration");
                        SerializedProperty disableDuration = MyWeaponRef.FindPropertyRelative("disableDuration");

                        SerializedProperty allowWeaponGroupAssignment = MyWeaponRef.FindPropertyRelative("allowWeaponGroupAssignment");
                        SerializedProperty assignedGroupIDs = MyWeaponRef.FindPropertyRelative("assignedGroupIDs");

                        SerializedProperty weaponSwitchTemporarilyDisableMovement = MyWeaponRef.FindPropertyRelative("weaponSwitchTemporarilyDisableMovement");
                        SerializedProperty weaponSwitchDisableMovementFreezePosition = MyWeaponRef.FindPropertyRelative("weaponSwitchDisableMovementFreezePosition");
                        SerializedProperty weaponSwitchDisableMovementDisableComponents = MyWeaponRef.FindPropertyRelative("weaponSwitchDisableMovementDisableComponents");
                        SerializedProperty weaponSwitchTemporarilyDisableAbilityActivation = MyWeaponRef.FindPropertyRelative("weaponSwitchTemporarilyDisableAbilityActivation");
                        SerializedProperty disableAbilityActivationDuration = MyWeaponRef.FindPropertyRelative("disableAbilityActivationDuration");

                        SerializedProperty useWeaponAnimations = MyWeaponRef.FindPropertyRelative("useWeaponAnimations");
                        SerializedProperty weaponGraphics = MyWeaponRef.FindPropertyRelative("weaponGraphics");

                        SerializedProperty enableWeaponOnCycle = MyWeaponRef.FindPropertyRelative("enableWeaponOnCycle");



                        SerializedProperty enableWeaponOnInput = MyWeaponRef.FindPropertyRelative("enableWeaponOnInput");
                        SerializedProperty weaponEnableInputType = MyWeaponRef.FindPropertyRelative("weaponEnableInputType");
                        SerializedProperty weaponEnableButton = MyWeaponRef.FindPropertyRelative("weaponEnableButton");
                        SerializedProperty weaponEnableKey = MyWeaponRef.FindPropertyRelative("weaponEnableKey");


                        SerializedProperty weaponEnableAnimationRunnerClip = MyWeaponRef.FindPropertyRelative("weaponEnableAnimationRunnerClip");
                        SerializedProperty weaponEnableAnimationRunnerClipSpeed = MyWeaponRef.FindPropertyRelative("weaponEnableAnimationRunnerClipSpeed");
                        SerializedProperty weaponEnableAnimationRunnerClipDelay = MyWeaponRef.FindPropertyRelative("weaponEnableAnimationRunnerClipDelay");
                        SerializedProperty weaponEnableAnimationRunnerClipDuration = MyWeaponRef.FindPropertyRelative("weaponEnableAnimationRunnerClipDuration");
                        SerializedProperty weaponEnableAnimationRunnerOnEntity = MyWeaponRef.FindPropertyRelative("weaponEnableAnimationRunnerOnEntity");
                        SerializedProperty weaponEnableAnimationRunnerOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponEnableAnimationRunnerOnScrollGraphic");
                        SerializedProperty weaponEnableAnimationRunnerOnWeapon = MyWeaponRef.FindPropertyRelative("weaponEnableAnimationRunnerOnWeapon");

                        SerializedProperty weaponEnableAnimatorParameter = MyWeaponRef.FindPropertyRelative("weaponEnableAnimatorParameter");
                        SerializedProperty weaponEnableAnimatorParameterType = MyWeaponRef.FindPropertyRelative("weaponEnableAnimatorParameterType");
                        SerializedProperty weaponEnableAnimatorOnValue = MyWeaponRef.FindPropertyRelative("weaponEnableAnimatorOnValue");
                        SerializedProperty weaponEnableAnimatorOffValue = MyWeaponRef.FindPropertyRelative("weaponEnableAnimatorOffValue");
                        SerializedProperty weaponEnableAnimatorDuration = MyWeaponRef.FindPropertyRelative("weaponEnableAnimatorDuration");
                        SerializedProperty weaponEnableAnimateOnEntity = MyWeaponRef.FindPropertyRelative("weaponEnableAnimateOnEntity");
                        SerializedProperty weaponEnableAnimateOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponEnableAnimateOnScrollGraphic");
                        SerializedProperty weaponEnableAnimateOnWeapon = MyWeaponRef.FindPropertyRelative("weaponEnableAnimateOnWeapon");


                        SerializedProperty weaponDisableAnimationRunnerClip = MyWeaponRef.FindPropertyRelative("weaponDisableAnimationRunnerClip");
                        SerializedProperty weaponDisableAnimationRunnerClipSpeed = MyWeaponRef.FindPropertyRelative("weaponDisableAnimationRunnerClipSpeed");
                        SerializedProperty weaponDisableAnimationRunnerClipDelay = MyWeaponRef.FindPropertyRelative("weaponDisableAnimationRunnerClipDelay");
                        SerializedProperty weaponDisableAnimationRunnerClipDuration = MyWeaponRef.FindPropertyRelative("weaponDisableAnimationRunnerClipDuration");
                        SerializedProperty weaponDisableAnimationRunnerOnEntity = MyWeaponRef.FindPropertyRelative("weaponDisableAnimationRunnerOnEntity");
                        SerializedProperty weaponDisableAnimationRunnerOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponDisableAnimationRunnerOnScrollGraphic");
                        SerializedProperty weaponDisableAnimationRunnerOnWeapon = MyWeaponRef.FindPropertyRelative("weaponDisableAnimationRunnerOnWeapon");


                        SerializedProperty weaponDisableAnimatorParameter = MyWeaponRef.FindPropertyRelative("weaponDisableAnimatorParameter");
                        SerializedProperty weaponDisableAnimatorParameterType = MyWeaponRef.FindPropertyRelative("weaponDisableAnimatorParameterType");
                        SerializedProperty weaponDisableAnimatorOnValue = MyWeaponRef.FindPropertyRelative("weaponDisableAnimatorOnValue");
                        SerializedProperty weaponDisableAnimatorOffValue = MyWeaponRef.FindPropertyRelative("weaponDisableAnimatorOffValue");
                        SerializedProperty weaponDisableAnimatorDuration = MyWeaponRef.FindPropertyRelative("weaponDisableAnimatorDuration");
                        SerializedProperty weaponDisableAnimateOnEntity = MyWeaponRef.FindPropertyRelative("weaponDisableAnimateOnEntity");
                        SerializedProperty weaponDisableAnimateOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponDisableAnimateOnScrollGraphic");
                        SerializedProperty weaponDisableAnimateOnWeapon = MyWeaponRef.FindPropertyRelative("weaponDisableAnimateOnWeapon");


                        SerializedProperty enableWeaponDrop = MyWeaponRef.FindPropertyRelative("enableWeaponDrop");
                        SerializedProperty weaponDropDuration = MyWeaponRef.FindPropertyRelative("weaponDropDuration");
                        SerializedProperty weaponDropAction = MyWeaponRef.FindPropertyRelative("weaponDropAction");
                        SerializedProperty weaponDropActionApplyToWeaponEnableAbilities = MyWeaponRef.FindPropertyRelative("weaponDropActionApplyToWeaponEnableAbilities");
                        SerializedProperty weaponDropActionApplyToAssignedGroups = MyWeaponRef.FindPropertyRelative("weaponDropActionApplyToAssignedGroups");
                        SerializedProperty weaponDropActionApplyToAssignedUI = MyWeaponRef.FindPropertyRelative("weaponDropActionApplyToAssignedUI");

                        SerializedProperty UseWeaponAmmo = MyWeaponRef.FindPropertyRelative("UseWeaponAmmo"); 
                        SerializedProperty weaponAmmoCount = MyWeaponRef.FindPropertyRelative("weaponAmmoCount");
                        SerializedProperty useWeaponReload = MyWeaponRef.FindPropertyRelative("useWeaponReload");
                        SerializedProperty weaponClipSize = MyWeaponRef.FindPropertyRelative("weaponClipSize");
                        SerializedProperty weaponReloadDuration = MyWeaponRef.FindPropertyRelative("weaponReloadDuration");
                        SerializedProperty weaponReloadRestrictAbilityActivationDuration = MyWeaponRef.FindPropertyRelative("weaponReloadRestrictAbilityActivationDuration");
                        SerializedProperty autoReloadWeaponWhenRequired = MyWeaponRef.FindPropertyRelative("autoReloadWeaponWhenRequired");
                        SerializedProperty weaponReloadFillClip = MyWeaponRef.FindPropertyRelative("weaponReloadFillClip");
                        SerializedProperty weaponReloadFillClipRepeatGraphic = MyWeaponRef.FindPropertyRelative("weaponReloadFillClipRepeatGraphic");

                        SerializedProperty useReloadWeaponAesthetics = MyWeaponRef.FindPropertyRelative("useReloadWeaponAesthetics");

                   
                        SerializedProperty reloadWeaponAnimationRunnerClip = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimationRunnerClip");
                        SerializedProperty reloadWeaponAnimationRunnerClipSpeed = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimationRunnerClipSpeed");
                        SerializedProperty reloadWeaponAnimationRunnerClipDelay = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimationRunnerClipDelay");
                        SerializedProperty reloadWeaponAnimationRunnerOnEntity = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimationRunnerOnEntity");
                        SerializedProperty reloadWeaponAnimationRunnerOnScrollGraphic = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimationRunnerOnScrollGraphic");
                        SerializedProperty reloadWeaponAnimationRunnerOnWeapon = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimationRunnerOnWeapon");


                        SerializedProperty reloadWeaponAnimatorParameter = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimatorParameter");
                        SerializedProperty reloadWeaponAnimatorParameterType = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimatorParameterType");
                        SerializedProperty reloadWeaponAnimatorOnValue = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimatorOnValue");
                        SerializedProperty reloadWeaponAnimatorOffValue = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimatorOffValue");
                        SerializedProperty reloadWeaponAnimateOnEntity = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimateOnEntity");
                        SerializedProperty reloadWeaponAnimateOnScrollGraphic = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimateOnScrollGraphic");
                        SerializedProperty reloadWeaponAnimateOnWeapon = MyWeaponRef.FindPropertyRelative("reloadWeaponAnimateOnWeapon");

                        SerializedProperty reloadWeaponGraphic = MyWeaponRef.FindPropertyRelative("reloadWeaponGraphic");
                        SerializedProperty reloadWeaponSubGraphic = MyWeaponRef.FindPropertyRelative("reloadWeaponSubGraphic");
                        SerializedProperty reloadWeaponAestheticDuration = MyWeaponRef.FindPropertyRelative("reloadWeaponAestheticDuration");
                        SerializedProperty reloadWeaponAestheticDelay = MyWeaponRef.FindPropertyRelative("reloadWeaponAestheticDelay");
                        SerializedProperty reloadWeaponStartPosition = MyWeaponRef.FindPropertyRelative("reloadWeaponStartPosition");
                        SerializedProperty reloadWeaponPositionOnObject = MyWeaponRef.FindPropertyRelative("reloadWeaponPositionOnObject");
                        SerializedProperty reloadWeaponPositionOnTag = MyWeaponRef.FindPropertyRelative("reloadWeaponPositionOnTag");
                        SerializedProperty reloadWeaponAestheticsPositionOffset = MyWeaponRef.FindPropertyRelative("reloadWeaponAestheticsPositionOffset");
                        SerializedProperty reloadWeaponAestheticsPositionForwardOffset = MyWeaponRef.FindPropertyRelative("reloadWeaponAestheticsPositionForwardOffset");
                        SerializedProperty reloadWeaponAestheticsPositionRightOffset = MyWeaponRef.FindPropertyRelative("reloadWeaponAestheticsPositionRightOffset");

                        SerializedProperty enableWeaponParry = MyWeaponRef.FindPropertyRelative("enableWeaponParry");
                        SerializedProperty weaponParryDelay = MyWeaponRef.FindPropertyRelative("weaponParryDelay");
                        SerializedProperty weaponParryDuration = MyWeaponRef.FindPropertyRelative("weaponParryDuration");
                        SerializedProperty weaponParryCooldown = MyWeaponRef.FindPropertyRelative("weaponParryCooldown"); 
                        SerializedProperty weaponParryFaceAbilityRequired = MyWeaponRef.FindPropertyRelative("weaponParryFaceAbilityRequired");
                        SerializedProperty weaponParryTurnToAbilityHitPoint = MyWeaponRef.FindPropertyRelative("weaponParryTurnToAbilityHitPoint");

                        SerializedProperty enableAbilitiesAfterParrying = MyWeaponRef.FindPropertyRelative("enableAbilitiesAfterParrying");
                        SerializedProperty enableAbilitiesAfterParryingDuration = MyWeaponRef.FindPropertyRelative("enableAbilitiesAfterParryingDuration");
                        SerializedProperty abilityIDsToEnableAfterParrying = MyWeaponRef.FindPropertyRelative("abilityIDsToEnableAfterParrying");

                        SerializedProperty activateAbilityAfterParrying = MyWeaponRef.FindPropertyRelative("activateAbilityAfterParrying");
                        SerializedProperty abilityIDToActivateAfterParrying = MyWeaponRef.FindPropertyRelative("abilityIDToActivateAfterParrying");
                        SerializedProperty activateAbilityAfterParryListChoice = MyWeaponRef.FindPropertyRelative("activateAbilityAfterParryListChoice");

                        SerializedProperty useWeaponParryAesthetics = MyWeaponRef.FindPropertyRelative("useWeaponParryAesthetics");

                        SerializedProperty weaponParryEffectGraphic = MyWeaponRef.FindPropertyRelative("weaponParryEffectGraphic");
                        SerializedProperty weaponParryEffectSubGraphic = MyWeaponRef.FindPropertyRelative("weaponParryEffectSubGraphic");
                        SerializedProperty weaponParryEffectAestheticDuration = MyWeaponRef.FindPropertyRelative("weaponParryEffectAestheticDuration");
                        SerializedProperty weaponParryEffectAestheticDelay = MyWeaponRef.FindPropertyRelative("weaponParryEffectAestheticDelay");
                        SerializedProperty weaponParryEffectStartPosition = MyWeaponRef.FindPropertyRelative("weaponParryEffectStartPosition");
                        SerializedProperty weaponParryEffectPositionOnObject = MyWeaponRef.FindPropertyRelative("weaponParryEffectPositionOnObject");
                        SerializedProperty weaponParryEffectPositionOnTag = MyWeaponRef.FindPropertyRelative("weaponParryEffectPositionOnTag");
                        SerializedProperty weaponParryEffectAestheticsPositionOffset = MyWeaponRef.FindPropertyRelative("weaponParryEffectAestheticsPositionOffset");
                        SerializedProperty weaponParryEffectAestheticsPositionForwardOffset = MyWeaponRef.FindPropertyRelative("weaponParryEffectAestheticsPositionForwardOffset");
                        SerializedProperty weaponParryEffectAestheticsPositionRightOffset = MyWeaponRef.FindPropertyRelative("weaponParryEffectAestheticsPositionRightOffset");


                        SerializedProperty weaponParryAnimationRunnerClip = MyWeaponRef.FindPropertyRelative("weaponParryAnimationRunnerClip");
                        SerializedProperty weaponParryAnimationRunnerClipSpeed = MyWeaponRef.FindPropertyRelative("weaponParryAnimationRunnerClipSpeed");
                        SerializedProperty weaponParryAnimationRunnerClipDelay = MyWeaponRef.FindPropertyRelative("weaponParryAnimationRunnerClipDelay");
                        SerializedProperty weaponParryAnimationRunnerClipDuration = MyWeaponRef.FindPropertyRelative("weaponParryAnimationRunnerClipDuration");
                        SerializedProperty weaponParryAnimationRunnerOnEntity = MyWeaponRef.FindPropertyRelative("weaponParryAnimationRunnerOnEntity");
                        SerializedProperty weaponParryAnimationRunnerOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponParryAnimationRunnerOnScrollGraphic");
                        SerializedProperty weaponParryAnimationRunnerOnWeapon = MyWeaponRef.FindPropertyRelative("weaponParryAnimationRunnerOnWeapon");


                        SerializedProperty weaponParryAnimatorParameter = MyWeaponRef.FindPropertyRelative("weaponParryAnimatorParameter");
                        SerializedProperty weaponParryAnimatorParameterType = MyWeaponRef.FindPropertyRelative("weaponParryAnimatorParameterType");
                        SerializedProperty weaponParryAnimatorOnValue = MyWeaponRef.FindPropertyRelative("weaponParryAnimatorOnValue");
                        SerializedProperty weaponParryAnimatorOffValue = MyWeaponRef.FindPropertyRelative("weaponParryAnimatorOffValue");
                        SerializedProperty weaponParryAnimatorDuration = MyWeaponRef.FindPropertyRelative("weaponParryAnimatorDuration");
                        SerializedProperty weaponParryAnimateOnEntity = MyWeaponRef.FindPropertyRelative("weaponParryAnimateOnEntity");
                        SerializedProperty weaponParryAnimateOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponParryAnimateOnScrollGraphic");
                        SerializedProperty weaponParryAnimateOnWeapon = MyWeaponRef.FindPropertyRelative("weaponParryAnimateOnWeapon");





                        SerializedProperty enableWeaponBlock = MyWeaponRef.FindPropertyRelative("enableWeaponBlock");
                        SerializedProperty weaponBlockFaceAbilityRequired = MyWeaponRef.FindPropertyRelative("weaponBlockFaceAbilityRequired");
                        SerializedProperty weaponBlockTurnToAbilityHitPoint = MyWeaponRef.FindPropertyRelative("weaponBlockTurnToAbilityHitPoint");
                        SerializedProperty interruptBlockedMeleeAttack = MyWeaponRef.FindPropertyRelative("interruptBlockedMeleeAttack");

                        SerializedProperty weaponBlockMitigateProjectileDamagePercentage = MyWeaponRef.FindPropertyRelative("weaponBlockMitigateProjectileDamagePercentage");
                        SerializedProperty weaponBlockMitigateMeleeDamagePercentage = MyWeaponRef.FindPropertyRelative("weaponBlockMitigateMeleeDamagePercentage");


                        SerializedProperty weaponBlockDurabilityReduction = MyWeaponRef.FindPropertyRelative("weaponBlockDurabilityReduction");

                        SerializedProperty weaponBlockPreventMeleeEffects = MyWeaponRef.FindPropertyRelative("weaponBlockPreventMeleeEffects");
                        SerializedProperty weaponBlockPreventProjectileEffects = MyWeaponRef.FindPropertyRelative("weaponBlockPreventProjectileEffects");
                        SerializedProperty weaponBlockModifyStats = MyWeaponRef.FindPropertyRelative("weaponBlockModifyStats");
                        SerializedProperty weaponBlockStatModifications = MyWeaponRef.FindPropertyRelative("weaponBlockStatModifications");

                        SerializedProperty enableAbilitiesAfterBlocking = MyWeaponRef.FindPropertyRelative("enableAbilitiesAfterBlocking");
                        SerializedProperty enableAbilitiesAfterBlockingDuration = MyWeaponRef.FindPropertyRelative("enableAbilitiesAfterBlockingDuration");
                        SerializedProperty abilityIDsToEnableAfterBlocking = MyWeaponRef.FindPropertyRelative("abilityIDsToEnableAfterBlocking");

                        SerializedProperty activateAbilityAfterBlocking = MyWeaponRef.FindPropertyRelative("activateAbilityAfterBlocking");
                        SerializedProperty abilityIDToActivateAfterBlocking = MyWeaponRef.FindPropertyRelative("abilityIDToActivateAfterBlocking");
                        SerializedProperty activateAbilityAfterBlockListChoice = MyWeaponRef.FindPropertyRelative("activateAbilityAfterBlockListChoice");


                        SerializedProperty useWeaponBlockAesthetics = MyWeaponRef.FindPropertyRelative("useWeaponBlockAesthetics");

                        SerializedProperty weaponBlockEffectGraphic = MyWeaponRef.FindPropertyRelative("weaponBlockEffectGraphic");
                        SerializedProperty weaponBlockEffectSubGraphic = MyWeaponRef.FindPropertyRelative("weaponBlockEffectSubGraphic");
                        SerializedProperty weaponBlockEffectAestheticDuration = MyWeaponRef.FindPropertyRelative("weaponBlockEffectAestheticDuration");
                        SerializedProperty weaponBlockEffectAestheticDelay = MyWeaponRef.FindPropertyRelative("weaponBlockEffectAestheticDelay");
                        SerializedProperty weaponBlockEffectStartPosition = MyWeaponRef.FindPropertyRelative("weaponBlockEffectStartPosition");
                        SerializedProperty weaponBlockEffectPositionOnObject = MyWeaponRef.FindPropertyRelative("weaponBlockEffectPositionOnObject");
                        SerializedProperty weaponBlockEffectPositionOnTag = MyWeaponRef.FindPropertyRelative("weaponBlockEffectPositionOnTag");
                        SerializedProperty weaponBlockEffectAestheticsPositionOffset = MyWeaponRef.FindPropertyRelative("weaponBlockEffectAestheticsPositionOffset");
                        SerializedProperty weaponBlockEffectAestheticsPositionForwardOffset = MyWeaponRef.FindPropertyRelative("weaponBlockEffectAestheticsPositionForwardOffset");
                        SerializedProperty weaponBlockEffectAestheticsPositionRightOffset = MyWeaponRef.FindPropertyRelative("weaponBlockEffectAestheticsPositionRightOffset");

;

                        SerializedProperty weaponBlockAnimationRunnerClip = MyWeaponRef.FindPropertyRelative("weaponBlockAnimationRunnerClip");
                        SerializedProperty weaponBlockAnimationRunnerClipSpeed = MyWeaponRef.FindPropertyRelative("weaponBlockAnimationRunnerClipSpeed");
                        SerializedProperty weaponBlockAnimationRunnerClipDelay = MyWeaponRef.FindPropertyRelative("weaponBlockAnimationRunnerClipDelay");
                        SerializedProperty weaponBlockAnimationRunnerClipDuration = MyWeaponRef.FindPropertyRelative("weaponBlockAnimationRunnerClipDuration");
                        SerializedProperty weaponBlockAnimationRunnerOnEntity = MyWeaponRef.FindPropertyRelative("weaponBlockAnimationRunnerOnEntity");
                        SerializedProperty weaponBlockAnimationRunnerOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponBlockAnimationRunnerOnScrollGraphic");
                        SerializedProperty weaponBlockAnimationRunnerOnWeapon = MyWeaponRef.FindPropertyRelative("weaponBlockAnimationRunnerOnWeapon");


                        SerializedProperty weaponBlockAnimatorParameter = MyWeaponRef.FindPropertyRelative("weaponBlockAnimatorParameter");
                        SerializedProperty weaponBlockAnimatorParameterType = MyWeaponRef.FindPropertyRelative("weaponBlockAnimatorParameterType");
                        SerializedProperty weaponBlockAnimatorOnValue = MyWeaponRef.FindPropertyRelative("weaponBlockAnimatorOnValue");
                        SerializedProperty weaponBlockAnimatorOffValue = MyWeaponRef.FindPropertyRelative("weaponBlockAnimatorOffValue");
                        SerializedProperty weaponBlockAnimateOnEntity = MyWeaponRef.FindPropertyRelative("weaponBlockAnimateOnEntity");
                        SerializedProperty weaponBlockAnimateOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponBlockAnimateOnScrollGraphic");
                        SerializedProperty weaponBlockAnimateOnWeapon = MyWeaponRef.FindPropertyRelative("weaponBlockAnimateOnWeapon");

                        SerializedProperty weaponBlockReactionAnimationRunnerClip = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimationRunnerClip");
                        SerializedProperty weaponBlockReactionAnimationRunnerClipSpeed = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimationRunnerClipSpeed");
                        SerializedProperty weaponBlockReactionAnimationRunnerClipDelay = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimationRunnerClipDelay");
                        SerializedProperty weaponBlockReactionAnimationRunnerClipDuration = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimationRunnerClipDuration");
                        SerializedProperty weaponBlockReactionAnimationRunnerOnEntity = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimationRunnerOnEntity");
                        SerializedProperty weaponBlockReactionAnimationRunnerOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimationRunnerOnScrollGraphic");
                        SerializedProperty weaponBlockReactionAnimationRunnerOnWeapon = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimationRunnerOnWeapon");


                        SerializedProperty weaponBlockReactionAnimatorParameter = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimatorParameter");
                        SerializedProperty weaponBlockReactionAnimatorParameterType = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimatorParameterType");
                        SerializedProperty weaponBlockReactionAnimatorOnValue = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimatorOnValue");
                        SerializedProperty weaponBlockReactionAnimatorOffValue = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimatorOffValue");
                        SerializedProperty weaponBlockReactionAnimatorDuration = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimatorDuration");
                        SerializedProperty weaponBlockReactionAnimateOnEntity = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimateOnEntity");
                        SerializedProperty weaponBlockReactionAnimateOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimateOnScrollGraphic");
                        SerializedProperty weaponBlockReactionAnimateOnWeapon = MyWeaponRef.FindPropertyRelative("weaponBlockReactionAnimateOnWeapon");


                        SerializedProperty useWeaponMeleeAttackReflectedAnimations = MyWeaponRef.FindPropertyRelative("useWeaponMeleeAttackReflectedAnimations");

                        SerializedProperty weaponMeleeAttackReflectedAnimationRunnerClip = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimationRunnerClip");
                        SerializedProperty weaponMeleeAttackReflectedAnimationRunnerClipSpeed = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimationRunnerClipSpeed");
                        SerializedProperty weaponMeleeAttackReflectedAnimationRunnerClipDelay = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimationRunnerClipDelay");
                        SerializedProperty weaponMeleeAttackReflectedAnimationRunnerClipDuration = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimationRunnerClipDuration");
                        SerializedProperty weaponMeleeAttackReflectedAnimationRunnerOnEntity = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimationRunnerOnEntity");
                        SerializedProperty weaponMeleeAttackReflectedAnimationRunnerOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimationRunnerOnScrollGraphic");
                        SerializedProperty weaponMeleeAttackReflectedAnimationRunnerOnWeapon = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimationRunnerOnWeapon");


                        SerializedProperty weaponMeleeAttackReflectedAnimatorParameter = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimatorParameter");
                        SerializedProperty weaponMeleeAttackReflectedAnimatorParameterType = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimatorParameterType");
                        SerializedProperty weaponMeleeAttackReflectedAnimatorOnValue = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimatorOnValue");
                        SerializedProperty weaponMeleeAttackReflectedAnimatorOffValue = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimatorOffValue");
                        SerializedProperty weaponMeleeAttackReflectedAnimatorDuration = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimatorDuration");
                        SerializedProperty weaponMeleeAttackReflectedAnimateOnEntity = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimateOnEntity");
                        SerializedProperty weaponMeleeAttackReflectedAnimateOnScrollGraphic = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimateOnScrollGraphic");
                        SerializedProperty weaponMeleeAttackReflectedAnimateOnWeapon = MyWeaponRef.FindPropertyRelative("weaponMeleeAttackReflectedAnimateOnWeapon");



                        SerializedProperty useWeaponDropObject = MyWeaponRef.FindPropertyRelative("useWeaponDropObject");
                        SerializedProperty weaponDropObject = MyWeaponRef.FindPropertyRelative("weaponDropObject");
                        SerializedProperty weaponDropObjectDelay = MyWeaponRef.FindPropertyRelative("weaponDropObjectDelay");
                        SerializedProperty weaponDropObjectDuration = MyWeaponRef.FindPropertyRelative("weaponDropObjectDuration");
                        SerializedProperty updateWeaponDropAmmo = MyWeaponRef.FindPropertyRelative("updateWeaponDropAmmo");

                        SerializedProperty weaponDropAnimationRunnerClip = MyWeaponRef.FindPropertyRelative("weaponDropAnimationRunnerClip");
                        SerializedProperty weaponDropAnimationRunnerClipSpeed = MyWeaponRef.FindPropertyRelative("weaponDropAnimationRunnerClipSpeed");
                        SerializedProperty weaponDropAnimationRunnerClipDelay = MyWeaponRef.FindPropertyRelative("weaponDropAnimationRunnerClipDelay");
                        SerializedProperty weaponDropAnimationRunnerClipDuration = MyWeaponRef.FindPropertyRelative("weaponDropAnimationRunnerClipDuration");

                        SerializedProperty weaponDropAnimatorParameter = MyWeaponRef.FindPropertyRelative("weaponDropAnimatorParameter");
                        SerializedProperty weaponDropAnimatorParameterType = MyWeaponRef.FindPropertyRelative("weaponDropAnimatorParameterType");
                        SerializedProperty weaponDropAnimatorOnValue = MyWeaponRef.FindPropertyRelative("weaponDropAnimatorOnValue");
                        SerializedProperty weaponDropAnimatorOffValue = MyWeaponRef.FindPropertyRelative("weaponDropAnimatorOffValue");
                        SerializedProperty weaponDropAnimatorDuration = MyWeaponRef.FindPropertyRelative("weaponDropAnimatorDuration");


                        #endregion

                        switch ((int)weaponSettingsToolbarSelection)
                        { 

                             case 0:


                            #region General & Graphics


                                InspectorSectionHeader(weaponName.stringValue + " - General & Graphic Settings");


                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Weapon Settings

                                InspectorVerticalBox(true);

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("ID: " + weaponID.intValue.ToString());
                                EditorGUIUtility.labelWidth = 130;
                                EditorGUILayout.PropertyField(inspectorEquipWeapon, new GUIContent("Equip Weapon"));
                                EditorGUILayout.EndHorizontal();

                                ResetLabelWidth();              

                                EditorGUIUtility.labelWidth = 125;
                                EditorGUILayout.PropertyField(weaponName);

                                //-- Uncomment to show weapon icon
                                //if (abilityCont.Weapons[currentWeapon].weaponIconImage != null) {
                                //    Texture2D iconImage = null;
                                //    iconImage = abilityCont.Weapons[currentWeapon].weaponIconImage.Texture2D;

                                //    if (iconImage != null)
                                //        GUILayout.Label(iconImage, GUILayout.MaxWidth(windowWidth - 20), GUILayout.Height(75));
                                //}

                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(weaponIconImage, new GUIContent("Icon Image"), GUILayout.MaxWidth(270));

                                EditorGUILayout.Space();

                                EditorGUILayout.LabelField("Description:");
                                weaponDescription.stringValue = EditorGUILayout.TextArea(weaponDescription.stringValue, GUILayout.MaxHeight(40f));
                                ResetLabelWidth();

                                EditorGUILayout.Space();
                              

                                EditorGUIUtility.labelWidth = 185;
                                EditorGUILayout.PropertyField(weaponSwitchTemporarilyDisableMovement, new GUIContent("Disable Movement On Switch"));
                                
                                if (weaponSwitchTemporarilyDisableMovement.boolValue == true)
                                {

                                    EditorGUIUtility.labelWidth = 100;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(weaponSwitchDisableMovementFreezePosition, new GUIContent("Freeze Position"));
                                    EditorGUIUtility.labelWidth = 130;
                                    EditorGUILayout.PropertyField(weaponSwitchDisableMovementDisableComponents, new GUIContent("Disable Components"));
                                    EditorGUILayout.EndHorizontal();
                                }


                                EditorGUILayout.Space();

                                EditorGUIUtility.labelWidth = 220;

                                EditorGUILayout.PropertyField(weaponSwitchTemporarilyDisableAbilityActivation, new GUIContent("Disable Ability Activation On Switch"));

                                if (weaponSwitchTemporarilyDisableAbilityActivation.boolValue == true)
                                {
                                    EditorGUIUtility.labelWidth = 125;
                                    EditorGUILayout.PropertyField(disableAbilityActivationDuration, new GUIContent("Disable Duration"), GUILayout.MaxWidth(200));
                                }


                                EditorGUILayout.Space();
                               


                                EditorGUILayout.EndVertical();


                                ResetLabelWidth();

                                #endregion


                                #region Weapon Trigger

                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 140;

                                EditorGUIUtility.labelWidth = 130;
                                //If application is playing then show a disable, enable button to call right method
                                if (Application.isPlaying)
                                {
                                    if (GUILayout.Button((weaponEnabled.boolValue) ? "Disable" : "Enable"))
                                    {

                                        if (weaponEnabled.boolValue)
                                        {
                                            abilityCont.DisableWeapon(weaponID.intValue);
                                        }
                                        else
                                        {
                                            abilityCont.EnableWeapon(weaponID.intValue);
                                        }
                                    }


                                }
                                else
                                {
                                    EditorGUILayout.PropertyField(weaponEnabled);
                                }


                                EditorGUILayout.PropertyField(enableDuration, new GUIContent("Equip Duration"), GUILayout.MaxWidth(200));
                                EditorGUILayout.PropertyField(disableDuration, new GUIContent("Unequip Duration"), GUILayout.MaxWidth(200));
                                InspectorHelpBox("How long it takes for the weapon to equip/unequip. This should be greater then how long it takes for the graphic and animation to show.");


                                EditorGUILayout.PropertyField(enableWeaponOnCycle, new GUIContent("Equip On Cycle"), GUILayout.MaxWidth(250));
                                InspectorHelpBox("If ticked then the weapon will equip on being cycled through if next/previous weapon trigger is setup");

                                EditorGUILayout.PropertyField(enableWeaponOnInput, new GUIContent("Equip On Input"), GUILayout.MaxWidth(250));

                                if (enableWeaponOnInput.boolValue == true)
                                {
                                    EditorGUILayout.PropertyField(weaponEnableInputType, new GUIContent("Input Type"), GUILayout.MaxWidth(250));

                                    if (((string)weaponEnableInputType.enumNames[weaponEnableInputType.enumValueIndex]) == "Key")
                                    {

                                        EditorGUILayout.PropertyField(weaponEnableKey, new GUIContent("Key"), GUILayout.MaxWidth(250));

                                    }
                                    else
                                    {

                                        EditorGUILayout.PropertyField(weaponEnableButton, new GUIContent("Button"), GUILayout.MaxWidth(250));

                                    }
                                }

                                InspectorHelpBox("Defines what button/key press will equip the weapon");



                                EditorGUILayout.EndVertical();


                                ResetLabelWidth();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion
                            


                                #region Weapon Graphics


                                InspectorSectionHeader("Weapon Graphics", "Setup graphics for the weapon. More then one weapon graphic can be setup to allow for situations where the character may dual wield swords etc." +
                                  " For each graphic define the position it sits when enabled/disabled and if it should still show if disabled. You can also define the delay before it enables/disables");



                                if (GUILayout.Button(new GUIContent(" Add Weapon Graphic", AddIcon, "Add Additional Starting Position"), GUILayout.MaxWidth(minimumAllWaySectionWidth)))
                                {
                                    var weaponGraphicsIndex = weaponGraphics.arraySize;
                                    weaponGraphics.InsertArrayElementAtIndex(weaponGraphicsIndex);

                                }

                                for (int n = 0; n < weaponGraphics.arraySize; n++)
                                {

                                    #region AllWay 

                                    InspectorVerticalBox();

                                    InspectorPropertyBox("Weapon Graphic", weaponGraphics, n, true);



                                    if (weaponGraphics.arraySize == 0 || n > weaponGraphics.arraySize - 1)
                                    {
                                        break;
                                    }


                                    SerializedProperty meWeaponGraphic = weaponGraphics.GetArrayElementAtIndex(n);
                                    SerializedProperty weaponObjMainGraphic = meWeaponGraphic.FindPropertyRelative("weaponObjMainGraphic");
                                    SerializedProperty foldOut = meWeaponGraphic.FindPropertyRelative("foldOut");
                                    SerializedProperty weaponObjSubGraphic = meWeaponGraphic.FindPropertyRelative("weaponObjSubGraphic");
                                    SerializedProperty equipDelay = meWeaponGraphic.FindPropertyRelative("equipDelay");
                                    SerializedProperty weaponObjEnabledPositionType = meWeaponGraphic.FindPropertyRelative("weaponObjEnabledPositionType");
                                    SerializedProperty weaponEnabledPositionOnObject = meWeaponGraphic.FindPropertyRelative("weaponEnabledPositionOnObject");
                                    SerializedProperty weaponEnabledPositionOnTag = meWeaponGraphic.FindPropertyRelative("weaponEnabledPositionOnTag");
                                    SerializedProperty weaponEnabledPositionOffset = meWeaponGraphic.FindPropertyRelative("weaponEnabledPositionOffset");
                                    SerializedProperty weaponEnabledPositionForwardOffset = meWeaponGraphic.FindPropertyRelative("weaponEnabledPositionForwardOffset");
                                    SerializedProperty weaponEnabledPositionRightOffset = meWeaponGraphic.FindPropertyRelative("weaponEnabledPositionRightOffset");
                                    SerializedProperty weaponEnabledStartingRotation = meWeaponGraphic.FindPropertyRelative("weaponEnabledStartingRotation");
                                    SerializedProperty weaponEnabledSetEulerRotation = meWeaponGraphic.FindPropertyRelative("weaponEnabledSetEulerRotation");
                                    SerializedProperty unequipDelay = meWeaponGraphic.FindPropertyRelative("unequipDelay");
                                    SerializedProperty showWeaponWhenDisabled = meWeaponGraphic.FindPropertyRelative("showWeaponWhenDisabled");
                                    SerializedProperty weaponObjDisabledPositionType = meWeaponGraphic.FindPropertyRelative("weaponObjDisabledPositionType");
                                    SerializedProperty weaponDisabledPositionOnObject = meWeaponGraphic.FindPropertyRelative("weaponDisabledPositionOnObject");
                                    SerializedProperty weaponDisabledPositionOnTag = meWeaponGraphic.FindPropertyRelative("weaponDisabledPositionOnTag");
                                    SerializedProperty weaponDisabledPositionOffset = meWeaponGraphic.FindPropertyRelative("weaponDisabledPositionOffset");
                                    SerializedProperty weaponDisabledPositionForwardOffset = meWeaponGraphic.FindPropertyRelative("weaponDisabledPositionForwardOffset");
                                    SerializedProperty weaponDisabledPositionRightOffset = meWeaponGraphic.FindPropertyRelative("weaponDisabledPositionRightOffset");
                                    SerializedProperty weaponDisabledStartingRotation = meWeaponGraphic.FindPropertyRelative("weaponDisabledStartingRotation");
                                    SerializedProperty weaponDisabledSetEulerRotation = meWeaponGraphic.FindPropertyRelative("weaponDisabledSetEulerRotation");

                                    EditorGUIUtility.labelWidth = 140;

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(weaponObjMainGraphic, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.PropertyField(weaponObjSubGraphic, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(equipDelay, GUILayout.MaxWidth(230));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(unequipDelay, GUILayout.MaxWidth(230));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();
                                    foldOut.boolValue = EditorGUILayout.Foldout(foldOut.boolValue, "Position Settings");

                                    if (foldOut.boolValue == true)
                                    {
                                        #region SideBySide 


           

                                        EditorGUILayout.BeginHorizontal();



                                        #region Weapon Enable Position Settings

                                        EditorGUILayout.BeginVertical("Box", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                        InspectorSectionHeader("Equip Position", "The position for when the weapon is equipped");

                                        // enabled position


                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(weaponObjEnabledPositionType, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));

                                        if (((string)weaponObjEnabledPositionType.enumNames[weaponObjEnabledPositionType.enumValueIndex]) == "OnObject")
                                        {
                                            EditorGUILayout.PropertyField(weaponEnabledPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                        }

                                        if (((string)weaponObjEnabledPositionType.enumNames[weaponObjEnabledPositionType.enumValueIndex]) == "OnTag" || ((string)weaponObjEnabledPositionType.enumNames[weaponObjEnabledPositionType.enumValueIndex]) == "OnSelfTag")
                                        {
                                            EditorGUILayout.LabelField("Select Tag");
                                            weaponEnabledPositionOnTag.stringValue = EditorGUILayout.TagField(weaponEnabledPositionOnTag.stringValue, GUILayout.MaxWidth(150));

                                        }

                                        ResetLabelWidth();
                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Position Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(weaponEnabledPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();

                                        EditorGUILayout.PropertyField(weaponEnabledPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(weaponEnabledPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(weaponEnabledStartingRotation, new GUIContent("Starting Rotation"));
                                        EditorGUILayout.Space();
                                        EditorGUIUtility.labelWidth = 130;
                                        EditorGUILayout.PropertyField(weaponEnabledSetEulerRotation, new GUIContent("Set Euler Rotation"));
                                        ResetLabelWidth();

                                        EditorGUILayout.EndVertical();

                                        #endregion

                                        #region Weapon Disable Position Settings

                                        EditorGUILayout.BeginVertical("Box", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                        InspectorSectionHeader("Unequipped Position", "The position for when the weapon is not equipped");

                                        EditorGUIUtility.labelWidth = 220;
                                        EditorGUILayout.PropertyField(showWeaponWhenDisabled, new GUIContent("Show Weapon When Unequipped"));
                                        ResetLabelWidth();


                                        if (showWeaponWhenDisabled.boolValue == true)
                                        {
                                            // disabled position

                                            EditorGUIUtility.labelWidth = 125;
                                            EditorGUILayout.PropertyField(weaponObjDisabledPositionType, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));

                                            if (((string)weaponObjDisabledPositionType.enumNames[weaponObjDisabledPositionType.enumValueIndex]) == "OnObject")
                                            {
                                                EditorGUILayout.PropertyField(weaponDisabledPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                            }

                                            if (((string)weaponObjDisabledPositionType.enumNames[weaponObjDisabledPositionType.enumValueIndex]) == "OnTag" || ((string)weaponObjDisabledPositionType.enumNames[weaponObjDisabledPositionType.enumValueIndex]) == "OnSelfTag")
                                            {
                                               
                                                EditorGUILayout.LabelField("Select Tag");
                                                weaponDisabledPositionOnTag.stringValue = EditorGUILayout.TagField(weaponDisabledPositionOnTag.stringValue, GUILayout.MaxWidth(150));
    

                                            }
                                            ResetLabelWidth();
                                            EditorGUILayout.Space();
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("Position Offset", GUILayout.MaxWidth(100));

                                            EditorGUILayout.PropertyField(weaponDisabledPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.Space();

                                            EditorGUILayout.PropertyField(weaponDisabledPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                            EditorGUILayout.PropertyField(weaponDisabledPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.Space();
                                            EditorGUILayout.PropertyField(weaponDisabledStartingRotation, new GUIContent("Starting Rotation"));
                                            EditorGUILayout.Space();
                                            EditorGUIUtility.labelWidth = 130;
                                            EditorGUILayout.PropertyField(weaponDisabledSetEulerRotation, new GUIContent("Set Euler Rotation"));
                                            ResetLabelWidth();

                                            EditorGUILayout.EndVertical();
                                        }

                                        #endregion

                                        EditorGUILayout.EndHorizontal();

                                        #endregion
                                    }


                                    EditorGUILayout.EndVertical();

                                    #endregion


                                }


                                #endregion


                                #endregion

                                break;
                            case 1:

                                #region Animation Settings

                                InspectorSectionHeader(weaponName.stringValue + " - Animation Settings");

                                #region Equip/UnEquip Animations

                                InspectorVerticalBox();
                                EditorGUIUtility.labelWidth = 145;
                                EditorGUILayout.PropertyField(useWeaponAnimations, new GUIContent("Use Equip Animations"));
                                InspectorHelpBox("If disabled then animations will not activate when equipping/deactivating weapons.");
                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                if (useWeaponAnimations.boolValue == true)
                                {
                               
                                    #region SideBySide

                                    EditorGUILayout.BeginHorizontal();


                                    EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                    InspectorSectionHeader("Equip Animations");

                                    #region Enable Animation Runner
                                    InspectorVerticalBox(true);

                                    EditorGUILayout.PropertyField(weaponEnableAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                    if (weaponEnableAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null)
                                    {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponEnableAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponEnableAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponEnableAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponEnableAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(weaponEnableAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.PropertyField(weaponEnableAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.Space();

                                    }

                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    #region Enable Animation 
                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 145;
                                    EditorGUILayout.PropertyField(weaponEnableAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                    if (weaponEnableAnimatorParameter.stringValue != "")
                                    {

                                        InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                        EditorGUILayout.PropertyField(weaponEnableAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponEnableAnimateOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponEnableAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponEnableAnimateOnWeapon, new GUIContent("Animate on Weapon"));
                                        InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, current weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");
                                        EditorGUIUtility.labelWidth = 150;



                                        if (((string)weaponEnableAnimatorParameterType.enumNames[weaponEnableAnimatorParameterType.enumValueIndex]) != "Trigger")
                                        {
                                            //EditorGUILayout.BeginHorizontal();
                                            // if not trigger we need to know the value to switch on and off
                                            EditorGUILayout.PropertyField(weaponEnableAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponEnableAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponEnableAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));

                                        }
                                    }


                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                    InspectorSectionHeader("Unequip Animations");

                                    #region Disable Animation Runner
                                    InspectorVerticalBox(true);

                                    EditorGUILayout.PropertyField(weaponDisableAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                    if (weaponDisableAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null)
                                    {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponDisableAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponDisableAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponDisableAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponDisableAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(weaponDisableAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.PropertyField(weaponDisableAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));

                                        EditorGUILayout.Space();


                                    }

                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    #region Disable Animation
                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 145;
                                    EditorGUILayout.PropertyField(weaponDisableAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                    if (weaponDisableAnimatorParameter.stringValue != "")
                                    {

                                        InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                        EditorGUILayout.PropertyField(weaponDisableAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponDisableAnimateOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponDisableAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponDisableAnimateOnWeapon, new GUIContent("Animate on Weapon"));
                                        InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");
                                        EditorGUIUtility.labelWidth = 150;


                                        if (((string)weaponDisableAnimatorParameterType.enumNames[weaponDisableAnimatorParameterType.enumValueIndex]) != "Trigger")
                                        {
                                            //EditorGUILayout.BeginHorizontal();
                                            // if not trigger we need to know the value to switch on and off
                                            EditorGUILayout.PropertyField(weaponDisableAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponDisableAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponDisableAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));

                                        }
                                    }



                                    ResetLabelWidth();



                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.EndHorizontal();

                                    #endregion

                               
                                }

                                #endregion

                                InspectorSectionHeader("Melee Attack Reflected Animations");

                                #region AllWay 

                                EditorGUILayout.BeginHorizontal();

                                #region Weapon Attack Reflected Animation Settings


                                InspectorVerticalBox();

                                EditorGUIUtility.labelWidth = 220;

                                EditorGUILayout.PropertyField(useWeaponMeleeAttackReflectedAnimations, new GUIContent("Use Melee Attack Reflect Animations"));

                                InspectorHelpBox("If disabled then animations will not activate when melee attacks are reflected from an melee ability being blocked or parried.");

                                ResetLabelWidth();

                                EditorGUILayout.EndVertical();


                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                if (useWeaponMeleeAttackReflectedAnimations.boolValue == true) {

                                    #region SideBySide

                                    EditorGUILayout.BeginHorizontal();


                                    EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));


                                    #region Attack Reflected Animation Runner
                                    InspectorVerticalBox(true);

                                    EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                    if (weaponMeleeAttackReflectedAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null) {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.Space();

                                    }

                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion

         

                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                    #region something
                                    InspectorVerticalBox(true);  

                                    #region Attack reflected Animation 
                                    InspectorVerticalBox(true); 

                                    EditorGUIUtility.labelWidth = 145;
                                    EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                    if (weaponMeleeAttackReflectedAnimatorParameter.stringValue != "") {

                                        InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimateOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimateOnWeapon, new GUIContent("Animate on Weapon"));
                                        InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, current weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");
                                        EditorGUIUtility.labelWidth = 150;



                                        if (((string)weaponMeleeAttackReflectedAnimatorParameterType.enumNames[weaponMeleeAttackReflectedAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                            //EditorGUILayout.BeginHorizontal();
                                            // if not trigger we need to know the value to switch on and off
                                            EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponMeleeAttackReflectedAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));

                                        }
                                    }


                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion


                                    EditorGUILayout.EndHorizontal();

                                    #endregion


                                }

                                #endregion

                                break;
                            case 2:

                                #region Ammo & Reload Settings

                                InspectorSectionHeader(weaponName.stringValue + " - Ammo & Reload Settings");

                                #region Ammo & Reload

                                InspectorVerticalBox();


                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(UseWeaponAmmo, new GUIContent("Use Ammo"));
                                if (UseWeaponAmmo.boolValue == true) {
                                    EditorGUILayout.PropertyField(weaponAmmoCount, new GUIContent("Ammo Count"), GUILayout.MaxWidth(180));
                                    EditorGUIUtility.labelWidth = 190;
                                    ResetLabelWidth();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();

                                    InspectorHelpBox("Amount of ammo which can be consumed by abilities, if abilities require ammo and there is none left ability can not activate");

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(useWeaponReload, new GUIContent("Use Reload"));

                                    if (useWeaponReload.boolValue == true) {                            
                                        EditorGUILayout.PropertyField(weaponClipSize, new GUIContent("Clip Size"), GUILayout.MaxWidth(180));

                                        if (weaponClipSize.intValue == 0) {
                                            weaponClipSize.intValue = 1;
                                        }

                                        EditorGUILayout.Space();

                                

                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(weaponReloadDuration, new GUIContent("Reload Duration"), GUILayout.MaxWidth(180));
                                        EditorGUILayout.Space();                                        

                                        EditorGUILayout.EndHorizontal();

                                        InspectorHelpBox("If reload is enabled then the weapon will need to be reloaded when the clip is empty, abilities that use ammo can not activate until clip is refilled. Duration determines how long it takes for the weapon to reload.");


                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponReloadFillClip, new GUIContent("Reload Fill Clip"));
                                        if (weaponReloadFillClip.boolValue == true) {
                                            EditorGUIUtility.labelWidth = 220;
                                            EditorGUILayout.PropertyField(weaponReloadFillClipRepeatGraphic, new GUIContent("Fill Clip Repeat Graphic/Animation"));
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        InspectorHelpBox("Reload fill clip will not waste the remaining clip on reload but instead will repeatedly add ammo to the clip by 1 using the duration as an interval. Graphic/animation can also be set to repeat each time.");


                                        EditorGUIUtility.labelWidth = 285;
                                        EditorGUILayout.PropertyField(weaponReloadRestrictAbilityActivationDuration, new GUIContent("Reload Restrict Ability Activation Duration"), GUILayout.MaxWidth(330));

                                        InspectorHelpBox("How long to restrict ability activation whilst reloading");

                                        EditorGUIUtility.labelWidth = 180;

                                        EditorGUILayout.PropertyField(autoReloadWeaponWhenRequired, new GUIContent("Auto Reload When Required"));
                                        InspectorHelpBox("If enabled then the weapon will automatically reload if empty when equipping the weapon or when the clip hits 0", false);
                                        ResetLabelWidth();

                                    } else {
                                        EditorGUILayout.EndHorizontal();
                                    }

                                } else {
                                    EditorGUILayout.EndHorizontal();
                                    InspectorHelpBox("Amount of ammo which can be consumed by abilities, if abilities require ammo and there is none left ability can not activate", false);
                                }
                                EditorGUILayout.Space();
                                EditorGUILayout.EndVertical();


                                #endregion

                                if (UseWeaponAmmo.boolValue == true && useWeaponReload.boolValue == true) {

                                    InspectorSectionHeader("Reload Animation/Graphic Settings");

                                    #region AllWay 

                                    EditorGUILayout.BeginHorizontal();

                                    #region Weapon Drop Settings


                                    InspectorVerticalBox();

                                    EditorGUILayout.PropertyField(useReloadWeaponAesthetics, new GUIContent("Use Aesthetics"));

                                    EditorGUILayout.EndVertical();


                                    #endregion

                                    EditorGUILayout.EndHorizontal();

                                    #endregion

                                    if (useReloadWeaponAesthetics.boolValue == true && useWeaponReload.boolValue == true) {

                                        #region SideBySide 


                                        EditorGUILayout.BeginHorizontal();

                                        #region Weapon Reload Animations

                                        EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));


                                        #region weapon Reload Animation Runner 

                                        InspectorVerticalBox(true);

                                        EditorGUIUtility.labelWidth = 140;
                                        EditorGUILayout.PropertyField(reloadWeaponAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(325));

                                        if (reloadWeaponAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null) {
                                            InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation Runner and does not use Unity's Animator.");


                                            EditorGUIUtility.labelWidth = 225;
                                            EditorGUILayout.PropertyField(reloadWeaponAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                            EditorGUILayout.PropertyField(reloadWeaponAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                            EditorGUILayout.PropertyField(reloadWeaponAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                            InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");


                                            EditorGUIUtility.labelWidth = 75;
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(reloadWeaponAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                            EditorGUILayout.PropertyField(reloadWeaponAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                            EditorGUILayout.EndHorizontal();




                                        }

                                        ResetLabelWidth();
                                        EditorGUILayout.EndVertical();

                                        #endregion

                                        #region weapon reload Animation 

                                        InspectorVerticalBox(true);

                                        EditorGUIUtility.labelWidth = 145;
                                        EditorGUILayout.PropertyField(reloadWeaponAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                        if (reloadWeaponAnimatorParameter.stringValue != "") {

                                            InspectorHelpBox("Enter in the name of the animation in your animator. Then the Animator type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                            EditorGUILayout.PropertyField(reloadWeaponAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                            EditorGUILayout.Space();

                                            EditorGUIUtility.labelWidth = 225;
                                            EditorGUILayout.PropertyField(reloadWeaponAnimateOnEntity, new GUIContent("Animate on Entity"));
                                            EditorGUILayout.PropertyField(reloadWeaponAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                            EditorGUILayout.PropertyField(reloadWeaponAnimateOnWeapon, new GUIContent("Animate on Weapon"));

                                            InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");


                                            EditorGUIUtility.labelWidth = 150;

                                            if (((string)weaponDropAnimatorParameterType.enumNames[reloadWeaponAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                                //EditorGUILayout.BeginHorizontal();
                                                // if not trigger we need to know the value to switch on and off
                                                EditorGUILayout.PropertyField(reloadWeaponAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                                EditorGUILayout.PropertyField(reloadWeaponAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                            }
                                        }


                                        ResetLabelWidth();




                                        EditorGUILayout.EndVertical();

                                        #endregion

                                        EditorGUILayout.EndVertical();

                                        #endregion


                                        #region Weapon Reload Graphic Object

                                        InspectorVerticalBox(true);

                                        InspectorHelpBox("Settings below for the graphic to show when scrollable ability reloads");


                                        ResetLabelWidth();

                                        EditorGUILayout.PropertyField(reloadWeaponGraphic, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));

                                        EditorGUILayout.PropertyField(reloadWeaponSubGraphic, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));


                                        EditorGUILayout.Space();


                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(reloadWeaponStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));
                                        ResetLabelWidth();

                                        if (((string)reloadWeaponStartPosition.enumNames[reloadWeaponStartPosition.enumValueIndex]) == "OnObject") {
                                            EditorGUILayout.PropertyField(reloadWeaponPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                        }

                                        if (((string)reloadWeaponStartPosition.enumNames[reloadWeaponStartPosition.enumValueIndex]) == "OnTag" || ((string)reloadWeaponStartPosition.enumNames[reloadWeaponStartPosition.enumValueIndex]) == "OnSelfTag") {
                                            EditorGUILayout.LabelField("Select Tag");
                                            reloadWeaponPositionOnTag.stringValue = EditorGUILayout.TagField(reloadWeaponPositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                            EditorGUILayout.Space();
                                        }


                                        EditorGUILayout.Space();

                                   
                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(reloadWeaponAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(reloadWeaponAestheticDelay, new GUIContent("Graphic Delay"), GUILayout.MaxWidth(230));
                                        ResetLabelWidth();
                                  
                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(reloadWeaponAestheticsPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(reloadWeaponAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(reloadWeaponAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        ResetLabelWidth();


                                        EditorGUILayout.EndVertical();

                                        #endregion

                                        EditorGUILayout.EndHorizontal();

                                        #endregion
                                    }
                                }

                                #endregion

                                break;
                            case 3:

                                #region Weapon Ability Settings

                                InspectorSectionHeader(weaponName.stringValue + " - Abilities Settings");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Enable Abilities When Equipped

                                InspectorVerticalBox(true);


                                EditorGUIUtility.labelWidth = 230;
                                EditorGUILayout.PropertyField(enableAbilitiesWhenEquipped, new GUIContent("Enable Abilities When Equipped"));
                                InspectorHelpBox("If ticked then abilities can be set to be enabled once this weapon is equipped", false);


                                if (enableAbilitiesWhenEquipped.boolValue == true) {

                                    EditorGUIUtility.labelWidth = 170;
                                    EditorGUILayout.PropertyField(disableAllOtherAbilitiesNotListed, new GUIContent("Disable All Other Abilities"));
                                    InspectorHelpBox("If ticked then any ability not listed below will be disabled when weapon is equipped", false);

                                    InspectorAbilityListBox("Enable Abilities", enableAbilityIDs);

                                }


                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();


                                #endregion


                                #region Disable Abilities When Equipped

                                InspectorVerticalBox(true);


                                EditorGUIUtility.labelWidth = 230;
                                EditorGUILayout.PropertyField(disableAbilitiesWhenEquipped, new GUIContent("Disable Abilities When Equipped"));
                                InspectorHelpBox("If ticked then abilities can be set to be disabled once this weapon is equipped", false);

                                if (disableAbilitiesWhenEquipped.boolValue == true) {

                                    EditorGUIUtility.labelWidth = 170;
                                    EditorGUILayout.PropertyField(enableAllOtherAbilitiesNotListed, new GUIContent("Enable All Other Abilities"));
                                    InspectorHelpBox("If ticked then any ability not listed below will be enabled when weapon is equipped", false);


                                    InspectorAbilityListBox("Disable Abilities", disableAbilityIDs);

                                }


                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();


                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #endregion

                                EditorGUILayout.Space();
                                EditorGUILayout.Space();

                                #region Weapon Ability Group Settings

                                InspectorSectionHeader(weaponName.stringValue + " - Ability Group Settings");

                                #region AllWay 

                                #region Group Assignment 

                                InspectorVerticalBox();


                                EditorGUIUtility.labelWidth = 170;

                                EditorGUILayout.PropertyField(allowWeaponGroupAssignment, new GUIContent("Allow Group Assignment"));
                                ResetLabelWidth();

                                InspectorHelpBox("Assign this weapon to a group. Grouped weapons can be set to become enabled all at the same time.", false);
                                if (abilityCont.AbilityGroups.Count > 0) {

                                    if (allowWeaponGroupAssignment.boolValue == true) {
                                        InspectorAbilityGroupListBox(assignedGroupIDs, abilityGroupListChoice);
                                    }
                                } else {
                                    EditorGUILayout.HelpBox("Unable to assign weapon as no groups have been setup. Please add a new group in the ABC Settings. ", MessageType.Warning);
                                }


                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Enable Groups When Equipped

                                InspectorVerticalBox(true);


                                EditorGUIUtility.labelWidth = 230;
                                EditorGUILayout.PropertyField(enableAbilityGroupsWhenEquipped, new GUIContent("Enable Groups When Equipped"));
                                InspectorHelpBox("If ticked then ABC groups can be set to be enabled once this weapon is equipped", false);

                              
                                if (enableAbilityGroupsWhenEquipped.boolValue == true) {

                                    EditorGUIUtility.labelWidth = 170;
                                    EditorGUILayout.PropertyField(disableAllOtherGroupsNotListed, new GUIContent("Disable All Other Groups"));
                                    InspectorHelpBox("If ticked then any ABC groups not listed below will be disabled when weapon is equipped", false);

                                    InspectorAbilityGroupListBox(enableAbilityGroupIDs, abilityGroupListChoice);

                                }


                                ResetLabelWidth();
                               

                                EditorGUILayout.EndVertical();


                                #endregion


                                #region Disable Groups When Equipped

                                InspectorVerticalBox(true);


                                EditorGUIUtility.labelWidth = 230;
                                EditorGUILayout.PropertyField(disableAbilityGroupWhenEquipped, new GUIContent("Disable Groups When Equipped"));
                                InspectorHelpBox("If ticked then ABC groups can be set to be disabled once this weapon is equipped", false);
                               
                                if (disableAbilityGroupWhenEquipped.boolValue == true) {

                                    EditorGUIUtility.labelWidth = 170;
                                    EditorGUILayout.PropertyField(enableAllOtherGroupsNotListed, new GUIContent("Enable All Other Groups"));
                                    InspectorHelpBox("If ticked then any ABC groups not listed below will be enabled when weapon is equipped", false);
                                   

                                    InspectorAbilityGroupListBox(disableAbilityGroupIDs, abilityGroupListChoice);

                                }


                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();


                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #endregion

                                break;

                            case 4:


                                InspectorSectionHeader(weaponName.stringValue + " - Block General Settings");

                                #region AllWay 

                                EditorGUILayout.BeginHorizontal();

                                #region Enable Setting

                                InspectorVerticalBox();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(enableWeaponBlock);

                                if (enableWeaponBlock.boolValue == true) {
                                    EditorGUIUtility.labelWidth = 190;
                                    EditorGUILayout.PropertyField(useWeaponBlockAesthetics);
                                }
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("Will enable weapon block functionality, allowing for the entity to block abilities");


                                EditorGUILayout.EndVertical();


                                #endregion
                                 
                                EditorGUILayout.EndHorizontal();

                                #endregion

                                if (enableWeaponBlock.boolValue == true) {

                                    #region SideBySide 

                                    EditorGUILayout.BeginHorizontal();

                                    #region weapon block settings

                                    InspectorVerticalBox(true);
                                     
                                    ResetLabelWidth();                                 

                                    EditorGUIUtility.labelWidth = 240;
                                    
                                    EditorGUILayout.PropertyField(weaponBlockDurabilityReduction, new GUIContent("Durability Reduction On Ability Block"));
                                    InspectorHelpBox("Amount to reduce entities Block Durability after an ability is blocked. If durability reaches 0 the entity will stop blocking");

                                    EditorGUILayout.PropertyField(weaponBlockPreventMeleeEffects, new GUIContent("Prevent Melee Effects"));
                                    EditorGUILayout.PropertyField(weaponBlockPreventProjectileEffects, new GUIContent("Prevent Projectile/Raycast Effects"));
                                    InspectorHelpBox("If ticked then no effects can be applied whilst blocking. You can select to prevent one or both melee and projectile/raycast abilities");

                                    if (weaponBlockPreventMeleeEffects.boolValue == false) {
                                        EditorGUILayout.PropertyField(weaponBlockMitigateMeleeDamagePercentage, new GUIContent("Mitigate Melee Damage (%)"), GUILayout.MaxWidth(280));
                                    }

                                    if (weaponBlockPreventProjectileEffects.boolValue == false) {
                                        EditorGUILayout.PropertyField(weaponBlockMitigateProjectileDamagePercentage, new GUIContent("Mitigate Projectile/Raycast Damage (%)"), GUILayout.MaxWidth(280));
                                    }

                                    if (weaponBlockPreventProjectileEffects.boolValue == false || weaponBlockPreventMeleeEffects.boolValue == false) {
                                        InspectorHelpBox("The percentage of damage to mitigate when blocking for both melee and non melee abilities");
                                    }


                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #region Weapon Block settings 2

                                    InspectorVerticalBox(true);

                                    EditorGUILayout.PropertyField(interruptBlockedMeleeAttack);
                                    InspectorHelpBox("If true then melee attacks will be interrupted when blocked playing the entities melee attack reflected animation if setup");
                                    
                                    EditorGUILayout.PropertyField(weaponBlockFaceAbilityRequired, new GUIContent("Face Ability Required"));
                                    InspectorHelpBox("If ticked then the entity needs to be facing the incoming ability to block successfully");

                                    if (weaponBlockFaceAbilityRequired.boolValue == false) {
                                        EditorGUILayout.PropertyField(weaponBlockTurnToAbilityHitPoint, new GUIContent("Turn to Hit Point"));
                                        InspectorHelpBox("If ticked then the entity will turn to face the direction of the incoming ability when blocking");
                                    }


                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();


                                    #endregion
                                

                                EditorGUILayout.EndHorizontal();

                                #endregion


                                #region StatModification

                                #region All Way
                                #region Block Stat Modification Settings


                                InspectorVerticalBox();

                                EditorGUIUtility.labelWidth = 190;
                                EditorGUILayout.PropertyField(weaponBlockModifyStats);
                                InspectorHelpBox("Increase or decrease stats when blocking by either a base or percentage value");


                                if (weaponBlockModifyStats.boolValue == true) {



                                    GUI.color = new Color32(208, 212, 211, 255);
                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Box("Blocking - Stat Modifications", new GUILayoutOption[] { GUILayout.MinWidth(minimumAllWaySectionWidth - 50), GUILayout.Height(20) });
                                    GUI.color = Color.white;
                                    GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
                                    if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(30))) {
                                        weaponBlockStatModifications.InsertArrayElementAtIndex(weaponBlockStatModifications.arraySize);
                                    }
                                    GUILayout.EndHorizontal();
                                        Color originalTextColor = GUI.skin.button.normal.textColor;

                                        if (weaponBlockStatModifications.arraySize > 0) {
                                        EditorGUILayout.BeginVertical("box");
                                        for (int i = 0; i < weaponBlockStatModifications.arraySize; i++) {
                                            SerializedProperty element = weaponBlockStatModifications.GetArrayElementAtIndex(i);
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(element.FindPropertyRelative("arithmeticOperator"), new GUIContent(""), GUILayout.Width(80));
                                            EditorGUILayout.PropertyField(element.FindPropertyRelative("statIntegrationType"), new GUIContent(""), GUILayout.Width(110));
                                            EditorGUIUtility.labelWidth = 30;
                                            EditorGUILayout.PropertyField(element.FindPropertyRelative("statName"), new GUIContent("Stat:"), GUILayout.Width(140));
                                            EditorGUIUtility.labelWidth = 20;
                                            EditorGUILayout.PropertyField(element.FindPropertyRelative("modificationValue"), new GUIContent("By"), GUILayout.Width(60));
                                            EditorGUIUtility.labelWidth = 40;
                                            EditorGUILayout.PropertyField(element.FindPropertyRelative("modifyByPercentOrBaseValue"), new GUIContent(""), GUILayout.Width(80));
                                            ResetLabelWidth();
                                            GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
                                            if (GUILayout.Button(UpArrowSymbol.ToString(), GUILayout.Width(40))) {
                                                weaponBlockStatModifications.MoveArrayElement(i, i - 1);
                                            }
                                            if (GUILayout.Button(DownArrowSymbol.ToString(), GUILayout.Width(40))) {
                                                weaponBlockStatModifications.MoveArrayElement(i, i + 1);
                                            }


                                            GUI.skin.button.normal.textColor = Color.red;
                                            if (GUILayout.Button("X", GUILayout.Width(40))) {
                                                weaponBlockStatModifications.DeleteArrayElementAtIndex(i);
                                            }

                                            GUILayout.EndHorizontal();

                                        }


                                        EditorGUILayout.EndVertical();

                                    }

                                    GUI.skin.button.normal.textColor = originalTextColor;


                                }

                                    EditorGUILayout.Space();

                                ResetLabelWidth();

                                EditorGUILayout.EndVertical();
                                #endregion


                                #endregion

                                #endregion

                                #region Ability Activation/Enable

                                #region Side by Side

                                EditorGUILayout.BeginHorizontal();


                                #region Ability Activation


                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 190;

                                EditorGUILayout.PropertyField(activateAbilityAfterBlocking, GUILayout.MaxWidth(350));
                                 InspectorHelpBox("Will activate an ability after sucessfully blocking");

                                ResetLabelWidth();

                                if (activateAbilityAfterBlocking.boolValue == true) {

                                    EditorGUILayout.BeginHorizontal();

                                    // show popup of ability 
                                    activateAbilityAfterBlockListChoice.intValue = EditorGUILayout.Popup("Select Ability:", activateAbilityAfterBlockListChoice.intValue, abilityList.ToArray());

                                    if (GUILayout.Button("Update", GUILayout.Width(60))) {

                                        abilityIDToActivateAfterBlocking.intValue = abilityCont.Abilities[activateAbilityAfterBlockListChoice.intValue].abilityID;


                                    }

                                    EditorGUILayout.EndHorizontal();


                                    if (abilityIDToActivateAfterBlocking.intValue != -1 && abilityCont.Abilities.Count > 0) {

                                        ABC_Ability ability = abilityCont.Abilities.FirstOrDefault(a => a.abilityID == abilityIDToActivateAfterBlocking.intValue);

                                        string name = "Ability Not Set";

                                        if (ability != null)
                                            name = ability.name;

                                        EditorGUILayout.LabelField("Activating: " + name, EditorStyles.boldLabel);
                                    }

                                }





                                ResetLabelWidth();

                                EditorGUILayout.EndVertical();
                                #endregion

                                #region Ability Enable


                                InspectorVerticalBox(true);


                                EditorGUIUtility.labelWidth = 190;
                                EditorGUILayout.PropertyField(enableAbilitiesAfterBlocking);
                                InspectorHelpBox("Will enable an ability for a duration after sucessfully blocking");

                                    EditorGUIUtility.labelWidth = 120;
                                if (enableAbilitiesAfterBlocking.boolValue == true) {
                                    EditorGUILayout.PropertyField(enableAbilitiesAfterBlockingDuration, new GUIContent("Enable Duration"), GUILayout.MaxWidth(170));

                                    this.InspectorAbilityListBox("Abilities To Enable", abilityIDsToEnableAfterBlocking);
                                }




                                ResetLabelWidth();

                                EditorGUILayout.EndVertical();
                                #endregion

                                EditorGUILayout.EndHorizontal();


                                #endregion

                                #endregion

                                if (useWeaponBlockAesthetics.boolValue == true) {

                                    #region Animation Settings


                                    #region Weapon block Animations


                                    #region SideBySide

                                    EditorGUILayout.BeginHorizontal();


                                    EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                    InspectorSectionHeader("Blocking Animations");

                                    #region Blocking Animation Runner
                                    InspectorVerticalBox(true);

                                    EditorGUILayout.PropertyField(weaponBlockAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                    if (weaponBlockAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null) {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponBlockAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponBlockAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponBlockAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponBlockAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(weaponBlockAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.PropertyField(weaponBlockAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.Space();

                                    }

                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    #region Blocking Animation 
                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 145;
                                    EditorGUILayout.PropertyField(weaponBlockAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                    if (weaponBlockAnimatorParameter.stringValue != "") {

                                        InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                        EditorGUILayout.PropertyField(weaponBlockAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponBlockAnimateOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponBlockAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponBlockAnimateOnWeapon, new GUIContent("Animate on Weapon"));
                                        InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, current weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");
                                        EditorGUIUtility.labelWidth = 150;



                                        if (((string)weaponBlockAnimatorParameterType.enumNames[weaponBlockAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                            //EditorGUILayout.BeginHorizontal();
                                            // if not trigger we need to know the value to switch on and off
                                            EditorGUILayout.PropertyField(weaponBlockAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponBlockAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                        }
                                    }


                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                    InspectorSectionHeader("Block Reaction Animations");

                                    #region Block Reaction Animation Runner
                                    InspectorVerticalBox(true);

                                    EditorGUILayout.PropertyField(weaponBlockReactionAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                    if (weaponBlockReactionAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null) {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));

                                        EditorGUILayout.Space();


                                    }

                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    #region Block Reaction Animation
                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 145;
                                    EditorGUILayout.PropertyField(weaponBlockReactionAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                    if (weaponBlockReactionAnimatorParameter.stringValue != "") {

                                        InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimateOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(weaponBlockReactionAnimateOnWeapon, new GUIContent("Animate on Weapon"));
                                        InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");
                                        EditorGUIUtility.labelWidth = 150;


                                        if (((string)weaponBlockReactionAnimatorParameterType.enumNames[weaponBlockReactionAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                            //EditorGUILayout.BeginHorizontal();
                                            // if not trigger we need to know the value to switch on and off
                                            EditorGUILayout.PropertyField(weaponBlockReactionAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponBlockReactionAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponBlockReactionAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));

                                        }
                                    }



                                    ResetLabelWidth();



                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.EndHorizontal();

                                    #endregion


                                    #endregion

                                    #endregion

                                    #region Weapon Block Effect Settings

                                    InspectorSectionHeader("Blocked Graphic Settings");

                                    #region AllWay 

                                    EditorGUILayout.BeginHorizontal();

                                    #region Graphic Settings


                                    InspectorVerticalBox();

                                        InspectorHelpBox("Will activate an graphic when an ability is successfully blocked");

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(weaponBlockEffectGraphic, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.PropertyField(weaponBlockEffectSubGraphic, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 125;
                                    EditorGUILayout.PropertyField(weaponBlockEffectStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));
                                    ResetLabelWidth();

                                    if (((string)weaponBlockEffectStartPosition.enumNames[weaponBlockEffectStartPosition.enumValueIndex]) == "OnObject") {
                                        EditorGUILayout.PropertyField(weaponBlockEffectPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                    }

                                    if (((string)weaponBlockEffectStartPosition.enumNames[weaponBlockEffectStartPosition.enumValueIndex]) == "OnTag" || ((string)weaponBlockEffectStartPosition.enumNames[weaponBlockEffectStartPosition.enumValueIndex]) == "OnSelfTag") {
                                        EditorGUILayout.LabelField("Select Tag");
                                        weaponBlockEffectPositionOnTag.stringValue = EditorGUILayout.TagField(weaponBlockEffectPositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                        EditorGUILayout.Space();
                                    }

                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 125;
                                    EditorGUILayout.PropertyField(weaponBlockEffectAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.PropertyField(weaponBlockEffectAestheticDelay, new GUIContent("Graphic Delay"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.EndHorizontal();
                                    ResetLabelWidth();

                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                    EditorGUILayout.PropertyField(weaponBlockEffectAestheticsPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(weaponBlockEffectAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.PropertyField(weaponBlockEffectAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();
                                    ResetLabelWidth();



                                    ResetLabelWidth();



                                    EditorGUILayout.EndVertical();

                                    #endregion


                                    #endregion


                                    #endregion

                                }

                                }

                                break;
                            case 5:


                                InspectorSectionHeader(weaponName.stringValue + " - Parry General Settings");

                                #region AllWay 

                                EditorGUILayout.BeginHorizontal();

                                #region Enable Setting

                                InspectorVerticalBox();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUIUtility.labelWidth = 140;
                                EditorGUILayout.PropertyField(enableWeaponParry);

                                if (enableWeaponParry.boolValue == true) {
                                    EditorGUIUtility.labelWidth = 190;
                                    EditorGUILayout.PropertyField(useWeaponParryAesthetics);
                                }
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("Will enable weapon parry functionality, allowing for the entity to parry melee abilities");


                                EditorGUILayout.EndVertical();


                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                if (enableWeaponParry.boolValue == true) {

                                    #region Side By Side

                                    EditorGUILayout.BeginHorizontal();

                                    #region weapon parry settings

                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 140;

                                    EditorGUILayout.PropertyField(weaponParryDelay, new GUIContent("Parry Status Delay"), GUILayout.MaxWidth(210));
                                    InspectorHelpBox("Delay till the parry status is in effect");

                                    EditorGUILayout.PropertyField(weaponParryDuration, new GUIContent("Parry Status Duration"), GUILayout.MaxWidth(210));
                                    InspectorHelpBox("How long the parry status is active for");


                                    EditorGUILayout.PropertyField(weaponParryCooldown, new GUIContent("Parry Cooldown"), GUILayout.MaxWidth(210));
                                    InspectorHelpBox("How long till the entity can parry again");


                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #region Weapon parry settings 2

                                    InspectorVerticalBox(true);



                                    EditorGUILayout.PropertyField(weaponParryFaceAbilityRequired, new GUIContent("Face Ability Required"));
                                    InspectorHelpBox("If ticked then the entity needs to be facing the incoming ability to parry successfully");

                                    if (weaponParryFaceAbilityRequired.boolValue == false) {
                                        EditorGUILayout.PropertyField(weaponParryTurnToAbilityHitPoint, new GUIContent("Turn to Hit Point"));
                                        InspectorHelpBox("If ticked then the entity will turn to face the direction of the incoming ability when parrying");
                                    }


                                    ResetLabelWidth();


                                    EditorGUILayout.EndVertical();


                                    #endregion


                                    EditorGUILayout.EndHorizontal();

                                    #endregion


                                    #region Ability Activation/Enable

                                    #region Side by Side

                                    EditorGUILayout.BeginHorizontal();


                                    #region Ability Activation


                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 190;

                                    EditorGUILayout.PropertyField(activateAbilityAfterParrying, GUILayout.MaxWidth(350));
                                    InspectorHelpBox("Will activate an ability after sucessfully parrying");

                                    ResetLabelWidth();

                                    if (activateAbilityAfterParrying.boolValue == true) {

                                        EditorGUILayout.BeginHorizontal();

                                        // show popup of ability 
                                        activateAbilityAfterParryListChoice.intValue = EditorGUILayout.Popup("Select Ability:", activateAbilityAfterParryListChoice.intValue, abilityList.ToArray());

                                        if (GUILayout.Button("Update", GUILayout.Width(60))) {

                                            abilityIDToActivateAfterParrying.intValue = abilityCont.Abilities[activateAbilityAfterParryListChoice.intValue].abilityID;


                                        }

                                        EditorGUILayout.EndHorizontal();


                                        if (abilityIDToActivateAfterParrying.intValue != -1 && abilityCont.Abilities.Count > 0) {

                                            ABC_Ability ability = abilityCont.Abilities.FirstOrDefault(a => a.abilityID == abilityIDToActivateAfterParrying.intValue);

                                            string name = "Ability Not Set";

                                            if (ability != null)
                                                name = ability.name;

                                            EditorGUILayout.LabelField("Activating: " + name, EditorStyles.boldLabel);
                                        }

                                    }





                                    ResetLabelWidth();

                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    #region Ability Enable


                                    InspectorVerticalBox(true);


                                    EditorGUIUtility.labelWidth = 190;
                                    EditorGUILayout.PropertyField(enableAbilitiesAfterParrying);
                                    InspectorHelpBox("Will enable an ability for a duration after sucessfully parrying");

                                    EditorGUIUtility.labelWidth = 120;
                                    if (enableAbilitiesAfterParrying.boolValue == true) {
                                        EditorGUILayout.PropertyField(enableAbilitiesAfterParryingDuration, new GUIContent("Enable Duration"), GUILayout.MaxWidth(170));

                                        this.InspectorAbilityListBox("Abilities To Enable", abilityIDsToEnableAfterParrying);
                                    }




                                    ResetLabelWidth();

                                    EditorGUILayout.EndVertical();
                                    #endregion

                                    EditorGUILayout.EndHorizontal();


                                    #endregion

                                    #endregion

                                    if (useWeaponParryAesthetics.boolValue == true) {

                                        #region Animation Settings


                                        #region Weapon parry Animations


                                        InspectorSectionHeader("Parry Animations");

                                        #region SideBySide

                                        EditorGUILayout.BeginHorizontal();


                                        EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));


                                        #region Parrying Animation Runner
                                        InspectorVerticalBox(true);

                                        EditorGUILayout.PropertyField(weaponParryAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                        if (weaponParryAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null) {
                                            InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                            EditorGUIUtility.labelWidth = 225;
                                            EditorGUILayout.PropertyField(weaponParryAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                            EditorGUILayout.PropertyField(weaponParryAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                            EditorGUILayout.PropertyField(weaponParryAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                            InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                            EditorGUIUtility.labelWidth = 75;
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(weaponParryAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                            EditorGUILayout.PropertyField(weaponParryAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.PropertyField(weaponParryAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                            EditorGUILayout.Space();

                                        }

                                        ResetLabelWidth();


                                        EditorGUILayout.EndVertical();
                                        #endregion
                  

                                        EditorGUILayout.EndVertical();

                                        EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                        #region Parrying Animation 
                                        InspectorVerticalBox(true);

                                        EditorGUIUtility.labelWidth = 145;
                                        EditorGUILayout.PropertyField(weaponParryAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                        if (weaponParryAnimatorParameter.stringValue != "") {

                                            InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                            EditorGUILayout.PropertyField(weaponParryAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                            EditorGUILayout.Space();

                                            EditorGUIUtility.labelWidth = 225;
                                            EditorGUILayout.PropertyField(weaponParryAnimateOnEntity, new GUIContent("Animate on Entity"));
                                            EditorGUILayout.PropertyField(weaponParryAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                            EditorGUILayout.PropertyField(weaponParryAnimateOnWeapon, new GUIContent("Animate on Weapon"));
                                            InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, current weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");
                                            EditorGUIUtility.labelWidth = 150;



                                            if (((string)weaponParryAnimatorParameterType.enumNames[weaponParryAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                                //EditorGUILayout.BeginHorizontal();
                                                // if not trigger we need to know the value to switch on and off
                                                EditorGUILayout.PropertyField(weaponParryAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                                EditorGUILayout.PropertyField(weaponParryAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                                EditorGUILayout.PropertyField(weaponParryAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));

                                            }
                                        }


                                        ResetLabelWidth();


                                        EditorGUILayout.EndVertical();
                                        #endregion

                                        EditorGUILayout.EndVertical();

                                        EditorGUILayout.EndHorizontal();

                                        #endregion


                                        #endregion

                                        #endregion

                                        #region Weapon Parry Effect Settings

                                        InspectorSectionHeader("On Parry Graphic Settings");

                                        #region AllWay 

                                        EditorGUILayout.BeginHorizontal();

                                        #region Graphic Settings


                                        InspectorVerticalBox();

                                        InspectorHelpBox("Will activate an graphic when an ability is successfully parried");

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponParryEffectGraphic, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                        EditorGUILayout.PropertyField(weaponParryEffectSubGraphic, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(weaponParryEffectStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));
                                        ResetLabelWidth();

                                        if (((string)weaponParryEffectStartPosition.enumNames[weaponParryEffectStartPosition.enumValueIndex]) == "OnObject") {
                                            EditorGUILayout.PropertyField(weaponParryEffectPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                        }

                                        if (((string)weaponParryEffectStartPosition.enumNames[weaponParryEffectStartPosition.enumValueIndex]) == "OnTag" || ((string)weaponParryEffectStartPosition.enumNames[weaponParryEffectStartPosition.enumValueIndex]) == "OnSelfTag") {
                                            EditorGUILayout.LabelField("Select Tag");
                                            weaponParryEffectPositionOnTag.stringValue = EditorGUILayout.TagField(weaponParryEffectPositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(weaponParryEffectAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(weaponParryEffectAestheticDelay, new GUIContent("Graphic Delay"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.EndHorizontal();
                                        ResetLabelWidth();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(weaponParryEffectAestheticsPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponParryEffectAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(weaponParryEffectAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        ResetLabelWidth();



                                        ResetLabelWidth();



                                        EditorGUILayout.EndVertical();

                                        #endregion


                                        #endregion


                                        #endregion

                                    }

                                }

                                break; 
                            case 6:

                                #region Weapon Drop Settings

                                InspectorSectionHeader("Weapon Drop Settings");

                                #region AllWay 

                                EditorGUILayout.BeginHorizontal();

                                #region Weapon Drop Settings


                                InspectorVerticalBox();


                                ResetLabelWidth();
                                EditorGUIUtility.labelWidth = 160;
                                EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(enableWeaponDrop, GUILayout.MaxWidth(270));

                                    if (enableWeaponDrop.boolValue == true) {
                                        EditorGUIUtility.labelWidth = 160;
                                        EditorGUILayout.PropertyField(weaponDropAction, new GUIContent("Weapon Drop Action") ,GUILayout.MaxWidth(270));                                
                                    }

                                EditorGUILayout.EndHorizontal();

                                InspectorHelpBox("Set if the weapon should be disabled for the entity or if the weapon should be deleted when the weapon is dropped", false);

                                if (enableWeaponDrop.boolValue == true) {
                                    ResetLabelWidth();
                                    EditorGUILayout.PropertyField(weaponDropDuration, new GUIContent("Drop Duration"), GUILayout.MaxWidth(230));
                                    InspectorHelpBox("How long it takes for the weapon to drop, this should be higher then any animations etc and lets ABC know how long to wait before equipping another weapon after drop");
                                    EditorGUIUtility.labelWidth = 290;
                                    EditorGUILayout.PropertyField(weaponDropActionApplyToWeaponEnableAbilities, new GUIContent("Apply Action To Weapons Enable Abilities"), GUILayout.MaxWidth(390));

                                    InspectorHelpBox("If ticked then the weapon drop action (Disable or Delete) will also be applied to the abilities that are enabled when this weapon is equipped");

                                    EditorGUILayout.PropertyField(weaponDropActionApplyToAssignedGroups, new GUIContent("Apply Action To Weapons Assigned Groups"), GUILayout.MaxWidth(390));

                                    InspectorHelpBox("If ticked then the weapon drop action (Disable or Delete) will also be applied to the groups the weapon is assigned too");


                                    EditorGUILayout.PropertyField(weaponDropActionApplyToAssignedUI, new GUIContent("Apply Action To Weapons Assigned UI"), GUILayout.MaxWidth(390));
                                    InspectorHelpBox("If ticked then the weapon drop action (Disable or Delete) will also be applied to the UI the weapon is assigned too");
                                }

                                    EditorGUILayout.EndVertical();


                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                if (enableWeaponDrop.boolValue == true) {

                                    InspectorSectionHeader("Animation / Weapon Drop Object Settings");


                                    #region SideBySide 


                                    EditorGUILayout.BeginHorizontal();

                                    #region Weapon Drop Animations


                                    EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));


                                    #region weapon drop Animation Runner 

                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 140;
                                    EditorGUILayout.PropertyField(weaponDropAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(325));

                                    if (weaponDropAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null) {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation Runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(weaponDropAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(weaponDropAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.PropertyField(weaponDropAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));


                                    }

                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #region weapon drop Animation 

                                    InspectorVerticalBox(true);

                                    EditorGUIUtility.labelWidth = 145;
                                    EditorGUILayout.PropertyField(weaponDropAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                    if (weaponDropAnimatorParameter.stringValue != "") {

                                        InspectorHelpBox("Enter in the name of the animation in your animator. Then the Animator type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                        EditorGUILayout.PropertyField(weaponDropAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                        EditorGUILayout.Space();


                                        EditorGUIUtility.labelWidth = 150;

                                        if (((string)weaponDropAnimatorParameterType.enumNames[weaponDropAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                            //EditorGUILayout.BeginHorizontal();
                                            // if not trigger we need to know the value to switch on and off
                                            EditorGUILayout.PropertyField(weaponDropAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(weaponDropAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                        }
                                    }


                                    ResetLabelWidth();




                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    EditorGUILayout.EndVertical();

                                    #endregion



                                    #region Weapon Drop Object


                                    InspectorVerticalBox(true);


                                    EditorGUIUtility.labelWidth = 180;
                                    EditorGUILayout.PropertyField(useWeaponDropObject, GUILayout.MaxWidth(270));
                                    InspectorHelpBox("If enabled then a gameobject will be dropped during play when the weapon is dropped");
                                    ResetLabelWidth();

                                    if (useWeaponDropObject.boolValue == true) {
                                        EditorGUILayout.PropertyField(weaponDropObject, new GUIContent("Drop Object"), GUILayout.MaxWidth(270));
                                        InspectorHelpBox("Object to drop when weapon is dropped");

                                        EditorGUIUtility.labelWidth = 150;
                                        EditorGUILayout.PropertyField(weaponDropObjectDelay, new GUIContent("Object Drop Delay"), GUILayout.MaxWidth(230));
                                        InspectorHelpBox("The delay before the weapon drop object appears in game");

                                        EditorGUILayout.PropertyField(weaponDropObjectDuration, new GUIContent("Object Drop Duration"), GUILayout.MaxWidth(230));
                                        InspectorHelpBox("The duration the drop object will remain in game, if 0 then duration is unlimited");

                                        EditorGUIUtility.labelWidth = 200;
                                        EditorGUILayout.PropertyField(updateWeaponDropAmmo, new GUIContent("Update Weapon Drop Ammo"));
                                        InspectorHelpBox("If true then the weapon drop object will have it's pick up ammo updated to match the current ammo on the weapon");
                                        ResetLabelWidth();
                                    }


                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    EditorGUILayout.EndHorizontal();

                                    #endregion


                                }
                                #endregion

                                break;
                        }
                    }


                    #endregion


                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndScrollView();
                    #endregion



                    EditorGUILayout.EndHorizontal();

                    break;

                case 4:
                    EditorGUILayout.BeginHorizontal();

                    #region Controls
                     EditorGUILayout.BeginVertical(GUILayout.MaxWidth(settingButtonsWidth));

                    #region Selected Group Controls

                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.color = inspectorSectionBoxProColor;
                    }
                    else
                    {
                        GUI.color = inspectorSectionBoxColor;
                    }

                    EditorGUILayout.BeginVertical("Box");

                    GUI.color = Color.white;

                    EditorGUILayout.Space();

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

                    aiSettingsToolbarSelection = GUILayout.SelectionGrid(aiSettingsToolbarSelection, aiSettingsToolbar, 1);

                    EditorGUILayout.Space();

                    EditorGUILayout.EndVertical();
                    #endregion



                    if (aiSettingsToolbarSelection == 1) {
                        #region AI Rule Selection List


                        InspectorHeader("Rule List", false);
                        InspectorHelpBox("Select an AI rule below to configure it. After the intermission if all conditions are met then the ability will fire.", false);

                        if (EditorGUIUtility.isProSkin)
                        {
                            GUI.color = inspectorSectionBoxProColor;
                        }
                        else
                        {
                            GUI.color = inspectorSectionBoxColor;
                        }

                        EditorGUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true));

                        GUI.color = Color.white;

                        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos,
                                                                         false,
                                                                         false);



                        reorderableListAIRules.DoLayoutList();
                        EditorGUILayout.EndScrollView();

                        EditorGUILayout.EndVertical();
                        #endregion

                        #region Selected AI Rule Controls

                        //InspectorHeader("Ability Controls");

                        if (EditorGUIUtility.isProSkin)
                        {
                            GUI.color = inspectorSectionBoxProColor;
                        }
                        else
                        {
                            GUI.color = inspectorSectionBoxColor;
                        }

                        EditorGUILayout.BeginVertical("Box");

                        GUI.color = Color.white;

                        EditorGUILayout.Space();
                            if (GUILayout.Button(new GUIContent(" Add New AI Rule", AddIcon, "Add New AI Rule"))) {

                            // add standard defaults here
                            abilityCont.AIRules.Add(new ABC_Controller.AIRule());
                        }


                        if (GUILayout.Button(new GUIContent(" Copy Selected Rule", CopyIcon, "Copy Selected Rule"))) {


                            ABC_Controller.AIRule clone = abilityCont.AIRules[abilityCont.CurrentAI];

                            ABC_Controller.AIRule newAI = new ABC_Controller.AIRule();

                            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(clone), newAI);

                            newAI.RuleName = "Copy Of " + newAI.RuleName;


                            abilityCont.AIRules.Add(newAI);


                        }

                        //Delete Ability 
                        if (GUILayout.Button(new GUIContent(" Delete Selected Rule", RemoveIcon, "Delete Selected Rule")) && EditorUtility.DisplayDialog("Delete Ability?", "Are you sure you want to delete " + abilityCont.AIRules[abilityCont.CurrentAI].RuleName, "Yes", "No")) {

                            int removeAtPosition = abilityCont.CurrentAI;
                            abilityCont.CurrentAI = 0;
                            abilityCont.AIRules.RemoveAt(removeAtPosition);
                        }

                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                        #endregion
                    }


                    EditorGUILayout.EndVertical();



                    #endregion

                    InspectorBoldVerticleLine();

                    #region Settings


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




                    editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                        false,
                                                                        false);

                    EditorGUILayout.BeginVertical();

                    #region General Settings



                    switch ((int)aiSettingsToolbarSelection) {
                        case 0:
                            #region All Way
                                #region AI Settings
                                InspectorSectionHeader("AI Settings");

                        
                                InspectorVerticalBox();
                                SerializedProperty enableAI = GetTarget.FindProperty("enableAI");
                                EditorGUIUtility.labelWidth = 120;
                                EditorGUILayout.PropertyField(enableAI);

                                EditorGUILayout.Space();
                                SerializedProperty maxAIRange = GetTarget.FindProperty("maxAIRange");
                                SerializedProperty aiPotentialTargetRetrievalIntermission = GetTarget.FindProperty("aiPotentialTargetRetrievalIntermission");
                                SerializedProperty aiRandomizePotentialTargets = GetTarget.FindProperty("aiRandomizePotentialTargets");
                                SerializedProperty minimumAICheckIntermission = GetTarget.FindProperty("minimumAICheckIntermission");
                                SerializedProperty maximumAICheckIntermission = GetTarget.FindProperty("maximumAICheckIntermission");
                                SerializedProperty randomizeAIRules = GetTarget.FindProperty("randomizeAIRules");
                                SerializedProperty aiRestrictAbilityTriggers = GetTarget.FindProperty("aiRestrictAbilityTriggers");

                                EditorGUIUtility.labelWidth = 120;

                                EditorGUILayout.PropertyField(minimumAICheckIntermission, new GUIContent("Minimum Interval"), GUILayout.MaxWidth(230));
                                EditorGUILayout.PropertyField(maximumAICheckIntermission, new GUIContent("Maximum Interval"), GUILayout.MaxWidth(230));
                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(maxAIRange, new GUIContent("Max Range"), GUILayout.MaxWidth(230));
                                EditorGUIUtility.labelWidth = 210;
                                EditorGUILayout.PropertyField(aiPotentialTargetRetrievalIntermission, new GUIContent("Potential Target Retrieval Interval"), GUILayout.MaxWidth(270));
                                EditorGUILayout.PropertyField(aiRandomizePotentialTargets, new GUIContent("Randomise Potential Targets"), GUILayout.MaxWidth(270));
                                EditorGUIUtility.labelWidth = 120;
                                InspectorHelpBox("Will shuffle the order of the potential targets in the list before checking through them");
                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(randomizeAIRules, new GUIContent("Randomise Rules"));

                                InspectorHelpBox("Will shuffle the order of the rules in the list before checking through them");

                                EditorGUIUtility.labelWidth = 160;
                                EditorGUILayout.PropertyField(aiRestrictAbilityTriggers, new GUIContent("Restrict Ability Triggers"));
                                ResetLabelWidth();
                                InspectorHelpBox("If enabled then ability key/button triggers will no longer activate abilities for as long as AI is enabled");

                          


                                ResetLabelWidth();

                                EditorGUILayout.EndVertical();
                            #endregion


                            #endregion


                            break;
                        case 1:

                            // display details for that spell
                            int ai = abilityCont.CurrentAI;

                            if (meAIRule.arraySize > 0 && ai < meAIRule.arraySize) {
                                #region Properties

                                SerializedProperty MyAIRuleListRef = meAIRule.GetArrayElementAtIndex(ai);


                                SerializedProperty RuleName = MyAIRuleListRef.FindPropertyRelative("RuleName");
                                SerializedProperty AIActive = MyAIRuleListRef.FindPropertyRelative("AIActive");


                                SerializedProperty selectedAIAction = MyAIRuleListRef.FindPropertyRelative("selectedAIAction");
                                SerializedProperty AIAbilityID = MyAIRuleListRef.FindPropertyRelative("AIAbilityID");
                                SerializedProperty AIAbilityListChoice = MyAIRuleListRef.FindPropertyRelative("AIAbilityListChoice");
                                SerializedProperty AIActivateAbilityGroupIDs = MyAIRuleListRef.FindPropertyRelative("AIActivateAbilityGroupIDs");
                                SerializedProperty AIActivateGroupListChoice = MyAIRuleListRef.FindPropertyRelative("AIActivateGroupListChoice");
                                SerializedProperty AIWeaponID = MyAIRuleListRef.FindPropertyRelative("AIWeaponID");
                                SerializedProperty AIWeaponListChoice = MyAIRuleListRef.FindPropertyRelative("AIWeaponListChoice");

                                SerializedProperty AIBlockDurationMin = MyAIRuleListRef.FindPropertyRelative("AIBlockDurationMin");
                                SerializedProperty AIBlockDurationMax = MyAIRuleListRef.FindPropertyRelative("AIBlockDurationMax");
                                SerializedProperty AIBlockCooldownMin = MyAIRuleListRef.FindPropertyRelative("AIBlockCooldownMin");
                                SerializedProperty AIBlockCooldownMax = MyAIRuleListRef.FindPropertyRelative("AIBlockCooldownMax");

                                SerializedProperty probabilityMinValue = MyAIRuleListRef.FindPropertyRelative("probabilityMinValue");
                                SerializedProperty probabilityMaxValue = MyAIRuleListRef.FindPropertyRelative("probabilityMaxValue");


                                SerializedProperty AITargets = MyAIRuleListRef.FindPropertyRelative("AITargets");
                                SerializedProperty AITags = MyAIRuleListRef.FindPropertyRelative("AITags");

                                SerializedProperty AIRangeMax = MyAIRuleListRef.FindPropertyRelative("AIRangeMax");
                                SerializedProperty AIRangeMin = MyAIRuleListRef.FindPropertyRelative("AIRangeMin");


                                SerializedProperty updateTarget = MyAIRuleListRef.FindPropertyRelative("updateTarget");
                                SerializedProperty ignoreCaster = MyAIRuleListRef.FindPropertyRelative("ignoreCaster");
                                SerializedProperty castWithoutConditions = MyAIRuleListRef.FindPropertyRelative("castWithoutConditions");
                                SerializedProperty activateWithoutTarget = MyAIRuleListRef.FindPropertyRelative("activateWithoutTarget");
                                SerializedProperty bypassTargetLimitations = MyAIRuleListRef.FindPropertyRelative("bypassTargetLimitations");

                                SerializedProperty stateEffectConditions = MyAIRuleListRef.FindPropertyRelative("stateEffectConditions");
                                SerializedProperty stateEffectAbsentConditions = MyAIRuleListRef.FindPropertyRelative("stateEffectAbsentConditions");


                                SerializedProperty AIHP = MyAIRuleListRef.FindPropertyRelative("AIHP");
                                SerializedProperty AIHealthGreaterThan = MyAIRuleListRef.FindPropertyRelative("AIHealthGreaterThan");
                                SerializedProperty AIHealthLessThan = MyAIRuleListRef.FindPropertyRelative("AIHealthLessThan");



                                SerializedProperty AIMP = MyAIRuleListRef.FindPropertyRelative("AIMP");
                                SerializedProperty AIManaGreaterThan = MyAIRuleListRef.FindPropertyRelative("AIManaGreaterThan");
                                SerializedProperty AIManaLessThan = MyAIRuleListRef.FindPropertyRelative("AIManaLessThan");


                                SerializedProperty AIAbilityTarget = MyAIRuleListRef.FindPropertyRelative("AIAbilityTarget");
                                SerializedProperty AINearestTargets = MyAIRuleListRef.FindPropertyRelative("AINearestTargets");
                                SerializedProperty AINearestTags = MyAIRuleListRef.FindPropertyRelative("AINearestTags");

                                SerializedProperty AIAreaActivationLimit = MyAIRuleListRef.FindPropertyRelative("AIAreaActivationLimit");
                                SerializedProperty AIAreaActivationLimitRange = MyAIRuleListRef.FindPropertyRelative("AIAreaActivationLimitRange");
                                SerializedProperty AIAreaActivationLimitMax = MyAIRuleListRef.FindPropertyRelative("AIAreaActivationLimitMax");
                                SerializedProperty AIAreaActivationLimitTags = MyAIRuleListRef.FindPropertyRelative("AIAreaActivationLimitTags");

                                SerializedProperty AIFacingTarget = MyAIRuleListRef.FindPropertyRelative("AIFacingTarget");

                                #endregion


                                InspectorSectionHeader(RuleName.stringValue + " General Settings");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region AI Rule General Settings

                                InspectorVerticalBox(true);
                   
                               
                                ResetLabelWidth();
                   
                                EditorGUILayout.PropertyField(AIActive);
                      
  

                                EditorGUILayout.PropertyField(selectedAIAction, new GUIContent("AI Action"), GUILayout.Width(260));

                                EditorGUILayout.Space();

                                if (((string)selectedAIAction.enumNames[selectedAIAction.enumValueIndex]) == "ActivateAbility") {
                                
                                    // show popup of ability 
                                    AIAbilityListChoice.intValue = EditorGUILayout.Popup("Select Ability:", AIAbilityListChoice.intValue, abilityList.ToArray());

                                    if (GUILayout.Button("Update", GUILayout.Width(60))) {

                                        AIAbilityID.intValue = abilityCont.Abilities[AIAbilityListChoice.intValue].abilityID;


                                    }

                                    EditorGUILayout.Space();
                                    if (AIAbilityID.intValue != -1 && abilityCont.Abilities.Count > 0) {

                                        ABC_Ability ability = abilityCont.Abilities.FirstOrDefault(a => a.abilityID == AIAbilityID.intValue);

                                        string name = "Ability Not Set";

                                        if (ability != null)
                                            name = ability.name;

                                        EditorGUILayout.LabelField("Auto activating: " + name, EditorStyles.boldLabel);
                                    }

                                } else if (((string)selectedAIAction.enumNames[selectedAIAction.enumValueIndex]) == "ActivateAbilityGroup") {

                                    InspectorAbilityGroupListBox(AIActivateAbilityGroupIDs, AIActivateGroupListChoice);

                                } else if (((string)selectedAIAction.enumNames[selectedAIAction.enumValueIndex]) == "ActivateBlocking") {

                                    EditorGUIUtility.labelWidth = 130;
                                    EditorGUILayout.PropertyField(AIBlockDurationMin, new GUIContent("Block Duration Min"));
                                    EditorGUILayout.PropertyField(AIBlockDurationMax, new GUIContent("Block Duration Max"));

                                    InspectorHelpBox("Minimum/Maximum duration the entity will block for");

                                    EditorGUILayout.PropertyField(AIBlockCooldownMin, new GUIContent("Block Cooldown Min"));
                                    EditorGUILayout.PropertyField(AIBlockCooldownMax, new GUIContent("Block Cooldown Max"));

                                    InspectorHelpBox("Minimum/Maximum cooldown till the entity is able to block again");

                                } else if (((string)selectedAIAction.enumNames[selectedAIAction.enumValueIndex]) == "EquipWeapon") {

                                    // show popup of ability 
                                    AIWeaponListChoice.intValue = EditorGUILayout.Popup("Select Weapon:", AIWeaponListChoice.intValue, weaponList.ToArray());

                                    if (GUILayout.Button("Update", GUILayout.Width(60))) {

                                        AIWeaponID.intValue = abilityCont.Weapons[AIWeaponListChoice.intValue].weaponID;


                                    }

                                    EditorGUILayout.Space();
                                    if (AIWeaponID.intValue != -1 && abilityCont.Weapons.Count > 0) {

                                        ABC_Controller.Weapon weapon = abilityCont.Weapons.FirstOrDefault(w => w.weaponID == AIWeaponID.intValue);

                                        string name = "Weapon Not Set";

                                        if (weapon != null)
                                            name = weapon.weaponName;

                                        EditorGUILayout.LabelField("Auto activating: " + name, EditorStyles.boldLabel);
                                    }

                                }



                                ResetLabelWidth();


                                EditorGUILayout.PropertyField(probabilityMinValue, new GUIContent("Probability Min"));
                                EditorGUILayout.PropertyField(probabilityMaxValue, new GUIContent("Probability Max"));
                                InspectorHelpBox("Chance of AI Rule being checked");


                                EditorGUILayout.EndVertical();

                                #endregion



                                #region AI Rule Probability 

                                InspectorVerticalBox(true);

                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(RuleName);
                                EditorGUILayout.Space();
                                EditorGUIUtility.labelWidth = 190;
                                EditorGUILayout.PropertyField(AIRangeMin, new GUIContent("Range Greater Than"));
                                EditorGUILayout.PropertyField(AIRangeMax, new GUIContent("Range Less Than"));
                              
                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(ignoreCaster, new GUIContent("Ignore Activating Entity"));
                                EditorGUILayout.PropertyField(castWithoutConditions, new GUIContent("Activate Without Conditions"));

                                if (castWithoutConditions.boolValue == true)  {
                                    EditorGUILayout.PropertyField(activateWithoutTarget);
                                }

                                InspectorHelpBox("If bypass is enabled then the AI will always activate on the target even if there is no open targeting slots due to the targeter limits");
                                EditorGUILayout.PropertyField(bypassTargetLimitations);
                                ResetLabelWidth();
                                EditorGUILayout.Space();

                                

                                ResetLabelWidth();

                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region AllWay 

                                #region AI Update Target

                                InspectorVerticalBox();

                                  EditorGUILayout.PropertyField(updateTarget, new GUIContent("Update Target:"));
                                InspectorHelpBox("If the rule condition has been met then the below will change which target the ability is fired on");

                            
                                if (updateTarget.boolValue == true) {
                                    EditorGUIUtility.labelWidth = 120;
                                    EditorGUILayout.PropertyField(AIAbilityTarget, GUILayout.Width(230));
                                    ResetLabelWidth();

                                    if (((string)AIAbilityTarget.enumNames[AIAbilityTarget.enumValueIndex]) == "Nearest" || ((string)AIAbilityTarget.enumNames[AIAbilityTarget.enumValueIndex]) == "TargetOfNearest") {


                                        EditorGUILayout.BeginHorizontal();


                                        InspectorListBox("Nearest Tags", AINearestTags);

                                        InspectorListBox("Nearest Target", AINearestTargets);
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }


                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                InspectorSectionHeader("Conditions");

                                #region AllWay 

                                #region AI Condition Target

                                InspectorVerticalBox();

                                InspectorHelpBox("The below section sets the rules which are checked against. If all conditions are met then the ability will fire.");

                                if (castWithoutConditions.boolValue == false) {

                                    EditorGUILayout.LabelField(DiamondSymbol.ToString() + " If Tag/Target is in range:");
                                    EditorGUILayout.BeginHorizontal();
                                    InspectorListBox("Tags:", AITags);

                                    InspectorListBox("Target:", AITargets);
                                    EditorGUILayout.EndHorizontal();                                 

                                    EditorGUILayout.Space();


                                } else {
                                    EditorGUILayout.HelpBox("AI Rule is set to cast without checking conditions", MessageType.Warning);
                                }


                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                #region AllWay 

                                if (castWithoutConditions.boolValue == false) {
                                    #region AI facing target

                                    InspectorVerticalBox();

                                    EditorGUIUtility.labelWidth = 210;
                                    EditorGUILayout.PropertyField(AIFacingTarget, new GUIContent(DiamondSymbol.ToString() + " And entity is facing target:"));
                                    ResetLabelWidth();

                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndVertical();

                                    #endregion
                                }

                                #endregion

                                #region SideBySide 

                                if (castWithoutConditions.boolValue == false) {
                                    EditorGUILayout.BeginHorizontal();

                                    #region AI Condition Health 

                                    InspectorVerticalBox(true);

                                    EditorGUILayout.PropertyField(AIHP, new GUIContent(DiamondSymbol.ToString() + " And Health is:"));
                                    EditorGUILayout.Space();
                                    if (AIHP.boolValue == true) {
                                        EditorGUILayout.BeginVertical();
                                        EditorGUILayout.PropertyField(AIHealthGreaterThan, new GUIContent("> Then (%):"));
                                        EditorGUILayout.PropertyField(AIHealthLessThan, new GUIContent("< Then (%):"));
                                        EditorGUILayout.EndVertical();

                                    }

                                    EditorGUILayout.EndVertical();

                                    #endregion



                                    #region AI Condition Mana

                                    InspectorVerticalBox(true);

                                    EditorGUILayout.PropertyField(AIMP, new GUIContent(DiamondSymbol.ToString() + " And Mana is:"));
                                    EditorGUILayout.Space();
                                    if (AIMP.boolValue == true) {
                                        EditorGUILayout.BeginVertical();
                                        EditorGUILayout.PropertyField(AIManaGreaterThan, new GUIContent("> Then (%):"));
                                        EditorGUILayout.PropertyField(AIManaLessThan, new GUIContent("< Then (%):"));
                                        EditorGUILayout.EndVertical();
                                    }

                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    EditorGUILayout.EndHorizontal();
                                }

                                #endregion

                                #region AllWay 

                                if (castWithoutConditions.boolValue == false) {
                                    #region AI Condition Effect Status

                                    InspectorVerticalBox();

                          

                                    EditorGUILayout.LabelField(DiamondSymbol.ToString() + " And Tag/Target has the following Effect Status:");
                                    EditorGUILayout.BeginHorizontal();
                                    InspectorListBox("Currently Effected By:", stateEffectConditions);
                                    InspectorListBox("Not Effected By:", stateEffectAbsentConditions);
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space();


                          


                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                    #endregion
                                }

                                #endregion



                                #region AllWay 

                                if (castWithoutConditions.boolValue == false) {
                                    #region AI surrounding activation Status

                                    InspectorVerticalBox();

                                    EditorGUIUtility.labelWidth = 480;
                                    EditorGUILayout.PropertyField(AIAreaActivationLimit, new GUIContent(DiamondSymbol.ToString() + " And number of surrounding tags activating are equal to or below limit:"));
                                    ResetLabelWidth();


                                    if (AIAreaActivationLimit.boolValue == true) {
                                        EditorGUILayout.BeginHorizontal();
                                        InspectorListBox("Tags:", AIAreaActivationLimitTags);
                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 50;
                                        EditorGUILayout.PropertyField(AIAreaActivationLimitMax, new GUIContent("Limit"), GUILayout.MaxWidth(100));
                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(AIAreaActivationLimitRange, new GUIContent("Range"), GUILayout.MaxWidth(100));
                                        ResetLabelWidth();

                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                    }

                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndVertical();

                                    #endregion
                                }

                                #endregion

                            }




                            break;

                        case 2:

                            #region All Way
                            #region AI Navigation Settings

                            InspectorVerticalBox();


                            ResetLabelWidth();

                            EditorGUIUtility.labelWidth = 160;

                            EditorGUILayout.PropertyField(GetTarget.FindProperty("navAIEnabled"), new GUIContent("Enable AI Navigation"));
                            InspectorHelpBox("Enable AI Navigation - Requires for the entity to have a NavMeshAgent Component");

                            if (GetTarget.FindProperty("navAIEnabled").boolValue == true) {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("navAIToggleIdleMode"), new GUIContent("Toggle Idle Mode"));
                                if (GetTarget.FindProperty("navAIToggleIdleMode").boolValue == true) {
                                    EditorGUIUtility.labelWidth = 280;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("navAIPreventMovementWhenSwitchingIdleMode"), new GUIContent("Prevent Movement When Switching Idle Mode"));
                                }
                                EditorGUILayout.EndHorizontal();

                                InspectorHelpBox("If Toggle Idle mode is ticked then the entity will switch in/out of idle mode when the destination is set/unset. You can also set to prevent movement when switching.");
                            }
                    

                           

                            EditorGUILayout.EndVertical();

                            #endregion

                            #endregion

                            if (GetTarget.FindProperty("navAIEnabled").boolValue == true) {

                                InspectorSectionHeader("Destination/Wander Settings");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Destination Settings

                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 100;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("destinationTag"), GUILayout.MaxWidth(250));

                                InspectorHelpBox("Enter the tag of the entity to navigate towards ");

                             
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("destinationSearchRadius"), new GUIContent("Search Radius"), GUILayout.MaxWidth(160));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("clearDestinationInterval"), new GUIContent("Clear Interval"), GUILayout.MaxWidth(160));
                                InspectorHelpBox("The radius to search for entities with the matching tag. The clear interval determines how long before destination is cleared for a new one. if 0 then clear interval is turned off");

                                EditorGUIUtility.labelWidth = 60;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("speed"), GUILayout.MaxWidth(120));

                                EditorGUIUtility.labelWidth = 130;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("minimumStopDistance"), new GUIContent("Min Stop Distance"), GUILayout.MaxWidth(280));
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("maximumStopDistance"), new GUIContent("Max Stop Distance"), GUILayout.MaxWidth(280));

                                InspectorHelpBox("The speed and min/max distance that the entity will stop from the destination.");




                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();

                                #endregion


                                EditorGUILayout.BeginVertical();

                                #region Stop Distance Settings 

                                InspectorVerticalBox(true);


                                EditorGUIUtility.labelWidth = 150;

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("alwaysFaceDestination"));

                                ResetLabelWidth();
                                if (GetTarget.FindProperty("alwaysFaceDestination").boolValue == true) {
                                    EditorGUIUtility.labelWidth = 80;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("faceDestinationTurnSpeed"), new GUIContent("Turn Speed"), GUILayout.MaxWidth(120));
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUIUtility.labelWidth = 150;

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("lineOfSightRequired"));

                                if (GetTarget.FindProperty("lineOfSightRequired").boolValue == true) {
                                    EditorGUIUtility.labelWidth = 80;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("lineOfSightRange"), new GUIContent("Sight Range"), GUILayout.MaxWidth(120));
                                }
                                EditorGUILayout.EndHorizontal();

                                InspectorHelpBox("If ticked then the destination has to be in the sight range for the entity to start travelling");

                                EditorGUIUtility.labelWidth = 190;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("stopDistanceCheckVelocity"), GUILayout.MaxWidth(220));

                                InspectorHelpBox("If true then stopping distance checks will take into account nav obstacles, movement won't stop if agent velocity is > 0");

                                EditorGUILayout.EndVertical();

                                #endregion

                                #region Wander Settings 

                                InspectorVerticalBox(true);

                                ResetLabelWidth();

                                EditorGUIUtility.labelWidth = 170;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderTillDestinationSet"), GUILayout.MaxWidth(250));

                                if (GetTarget.FindProperty("wanderTillDestinationSet").boolValue == true) {
                                    EditorGUIUtility.labelWidth = 60;

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderInterval"), new GUIContent("Interval"), GUILayout.MaxWidth(120));
                                    EditorGUIUtility.labelWidth = 80;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderAreaRange"), new GUIContent("Area Range"), GUILayout.MaxWidth(140));
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUIUtility.labelWidth = 60;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderSpeed"), new GUIContent("Speed"), GUILayout.MaxWidth(120));
                                }

                                InspectorHelpBox("If ticked entity will wander until a destination is found.");

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndVertical();


                                EditorGUILayout.EndHorizontal();

                                #endregion

                                InspectorSectionHeader("Rotation & Distance Change Settings");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Rotation Behaviour Settings

                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 160;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("enableRotationBehaviour"), GUILayout.MaxWidth(250));
                                InspectorHelpBox("If enabled then once destination is reached entity will per interval rotate around the destination");

                                if (GetTarget.FindProperty("enableRotationBehaviour").boolValue == true) {



                                    EditorGUIUtility.labelWidth = 60;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotationInterval"), new GUIContent("Interval"), GUILayout.MaxWidth(110));                                                                        
                                    InspectorHelpBox("The minimum time which must pass between rotations.");


                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotationSpeed"), new GUIContent("Speed"), GUILayout.MaxWidth(110));
                                    EditorGUIUtility.labelWidth = 130;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotationMinDuration"), GUILayout.MaxWidth(270));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotationMaxDuration"), GUILayout.MaxWidth(270));

                                    InspectorHelpBox("speed and min/max duration of the rotation");
                                }
                                ResetLabelWidth();



                                EditorGUILayout.EndVertical();

                                #endregion



                                #region Distance Change Settinngs 

                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 220;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("enableDistanceBehaviour"), new GUIContent("Enable Distance Change Behaviour"), GUILayout.MaxWidth(250));
                                InspectorHelpBox("If enabled then once destination is reached entity will per interval move towards or away from the destination");

                                if (GetTarget.FindProperty("enableDistanceBehaviour").boolValue == true) {
                                    EditorGUIUtility.labelWidth = 60;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("distanceChangeInterval"), new GUIContent("Interval"), GUILayout.MaxWidth(110));
                                    InspectorHelpBox("The minimum time which must pass between distance changes");

                                    EditorGUIUtility.labelWidth = 170;
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("moveBackSpeed"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("moveForwardSpeed"), GUILayout.MaxWidth(230));                                    
                                    InspectorHelpBox("The speed of the entity moving back and forward");
                                }





                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion
                            }


                            break;

                        case 3:

                            if (GetTarget.FindProperty("navAIEnabled").boolValue == true) {

                                #region Navigational Animation Settings

                                InspectorSectionHeader("AI Navigation Animation Settings");

                                InspectorVerticalBox();
                                EditorGUIUtility.labelWidth = 145;
                                //EditorGUILayout.PropertyField(useWeaponAnimations, new GUIContent("Use Animations"));
                                InspectorHelpBox("Animations can be setup here to activate when the entity performs different AI Navigation (Walking/Wandering/Rotating Around etc).");
                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                #region To Destination/Wander Animations

                                #region SideBySide

                                EditorGUILayout.BeginHorizontal();


                                EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                InspectorSectionHeader("To Destination Animations");

                                #region To Destination Animation Runner
                                InspectorVerticalBox(true);

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("toDestinationAnimationRunnerClip"), new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("toDestinationAnimationRunnerClip").FindPropertyRelative("refVal").objectReferenceValue != null) {
                                    InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                    EditorGUIUtility.labelWidth = 75;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("toDestinationAnimationRunnerClipDelay"), new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("toDestinationAnimationRunnerClipSpeed"), new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.EndHorizontal();
                                   

                                    EditorGUILayout.Space();


                                }

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();
                                #endregion

                                #region To Destination Animation
                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 145;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("toDestinationAnimatorParameter"), new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("toDestinationAnimatorParameter").stringValue != "") {

                                    InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("toDestinationAnimatorParameterType"), new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                    EditorGUILayout.Space();

                                    EditorGUIUtility.labelWidth = 150;


                                    if (((string)GetTarget.FindProperty("toDestinationAnimatorParameterType").enumNames[GetTarget.FindProperty("toDestinationAnimatorParameterType").enumValueIndex]) != "Trigger") {
                                        //EditorGUILayout.BeginHorizontal();
                                        // if not trigger we need to know the value to switch on and off
                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("toDestinationAnimatorOnValue"), new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("toDestinationAnimatorOffValue"), new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                      

                                    }
                                }



                                ResetLabelWidth();



                                EditorGUILayout.EndVertical();
                                #endregion

                                EditorGUILayout.EndVertical();

                                EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                InspectorSectionHeader("Wander Animations");

                                #region Wander Animation Runner
                                InspectorVerticalBox(true);

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderAnimationRunnerClip"), new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("wanderAnimationRunnerClip").FindPropertyRelative("refVal").objectReferenceValue != null) {
                                    InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                    EditorGUIUtility.labelWidth = 75;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderAnimationRunnerClipDelay"), new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderAnimationRunnerClipSpeed"), new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.EndHorizontal();
                                    

                                    EditorGUILayout.Space();


                                }

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();
                                #endregion

                                #region Wander Animation
                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 145;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderAnimatorParameter"), new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("wanderAnimatorParameter").stringValue != "") {

                                    InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderAnimatorParameterType"), new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                    EditorGUILayout.Space();

                                    EditorGUIUtility.labelWidth = 150;


                                    if (((string)GetTarget.FindProperty("wanderAnimatorParameterType").enumNames[GetTarget.FindProperty("wanderAnimatorParameterType").enumValueIndex]) != "Trigger") {
                                        //EditorGUILayout.BeginHorizontal();
                                        // if not trigger we need to know the value to switch on and off
                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderAnimatorOnValue"), new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("wanderAnimatorOffValue"), new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                        

                                    }
                                }



                                ResetLabelWidth();



                                EditorGUILayout.EndVertical();
                                #endregion

                                EditorGUILayout.EndVertical();

                                EditorGUILayout.EndHorizontal();

                                #endregion


                                #endregion

                                #region Rotate Animations

                                #region SideBySide

                                EditorGUILayout.BeginHorizontal();


                                EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                InspectorSectionHeader("Rotate Around Left Animations");

                                #region Rotate Around Left Animation Runner
                                InspectorVerticalBox(true);

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundLeftAnimationRunnerClip"), new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("rotateAroundLeftAnimationRunnerClip").FindPropertyRelative("refVal").objectReferenceValue != null) {
                                    InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                    EditorGUIUtility.labelWidth = 75;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundLeftAnimationRunnerClipDelay"), new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundLeftAnimationRunnerClipSpeed"), new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.EndHorizontal();
                                    

                                    EditorGUILayout.Space();


                                }

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();
                                #endregion

                                #region Rotate Around Left  Animation
                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 145;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundLeftAnimatorParameter"), new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("rotateAroundLeftAnimatorParameter").stringValue != "") {

                                    InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundLeftAnimatorParameterType"), new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                    EditorGUILayout.Space();

                                    EditorGUIUtility.labelWidth = 150;


                                    if (((string)GetTarget.FindProperty("rotateAroundLeftAnimatorParameterType").enumNames[GetTarget.FindProperty("rotateAroundLeftAnimatorParameterType").enumValueIndex]) != "Trigger") {
                                        //EditorGUILayout.BeginHorizontal();
                                        // if not trigger we need to know the value to switch on and off
                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundLeftAnimatorOnValue"), new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundLeftAnimatorOffValue"), new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                        

                                    }
                                }



                                ResetLabelWidth();



                                EditorGUILayout.EndVertical();
                                #endregion

                                EditorGUILayout.EndVertical();

                                EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                InspectorSectionHeader("Rotate Around Right Animations");

                                #region Rotate Around Right Runner
                                InspectorVerticalBox(true);

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundRightAnimationRunnerClip"), new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("rotateAroundRightAnimationRunnerClip").FindPropertyRelative("refVal").objectReferenceValue != null) {
                                    InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                    EditorGUIUtility.labelWidth = 75;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundRightAnimationRunnerClipDelay"), new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundRightAnimationRunnerClipSpeed"), new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.EndHorizontal();
                                    

                                    EditorGUILayout.Space();


                                }

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();
                                #endregion

                                #region Rotate Around Right Animation
                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 145;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundRightAnimatorParameter"), new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("rotateAroundRightAnimatorParameter").stringValue != "") {

                                    InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundRightAnimatorParameterType"), new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                    EditorGUILayout.Space();

                                    EditorGUIUtility.labelWidth = 150;


                                    if (((string)GetTarget.FindProperty("rotateAroundRightAnimatorParameterType").enumNames[GetTarget.FindProperty("rotateAroundRightAnimatorParameterType").enumValueIndex]) != "Trigger") {
                                        //EditorGUILayout.BeginHorizontal();
                                        // if not trigger we need to know the value to switch on and off
                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundRightAnimatorOnValue"), new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("rotateAroundRightAnimatorOffValue"), new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                        

                                    }
                                }



                                ResetLabelWidth();



                                EditorGUILayout.EndVertical();
                                #endregion

                                EditorGUILayout.EndVertical();

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #endregion

                                #region Walk Back/Forward Animations

                                #region SideBySide

                                EditorGUILayout.BeginHorizontal();


                                EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                InspectorSectionHeader("Move Forward Animations");

                                #region Move Forward Animation Runner
                                InspectorVerticalBox(true);

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("moveForwardAnimationRunnerClip"), new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("moveForwardAnimationRunnerClip").FindPropertyRelative("refVal").objectReferenceValue != null) {
                                    InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                    EditorGUIUtility.labelWidth = 75;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("moveForwardAnimationRunnerClipDelay"), new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("moveForwardAnimationRunnerClipSpeed"), new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.EndHorizontal();
                                    

                                    EditorGUILayout.Space();


                                }

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();
                                #endregion

                                #region Move Forward Animation
                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 145;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("moveForwardAnimatorParameter"), new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("moveForwardAnimatorParameter").stringValue != "") {

                                    InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("moveForwardAnimatorParameterType"), new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                    EditorGUILayout.Space();

                                    EditorGUIUtility.labelWidth = 150;


                                    if (((string)GetTarget.FindProperty("moveForwardAnimatorParameterType").enumNames[GetTarget.FindProperty("moveForwardAnimatorParameterType").enumValueIndex]) != "Trigger") {
                                        //EditorGUILayout.BeginHorizontal();
                                        // if not trigger we need to know the value to switch on and off
                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("moveForwardAnimatorOnValue"), new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("moveForwardAnimatorOffValue"), new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                        

                                    }
                                }



                                ResetLabelWidth();



                                EditorGUILayout.EndVertical();
                                #endregion

                                EditorGUILayout.EndVertical();

                                EditorGUILayout.BeginVertical("Label", GUILayout.MinHeight(minimumSectionHeight), GUILayout.MinWidth(minimumSideBySideSectionWidth));

                                InspectorSectionHeader("Move Back Animations");

                                #region Move Back Animation Runner
                                InspectorVerticalBox(true);

                                EditorGUILayout.PropertyField(GetTarget.FindProperty("moveBackAnimationRunnerClip"), new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("moveBackAnimationRunnerClip").FindPropertyRelative("refVal").objectReferenceValue != null) {
                                    InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                    EditorGUIUtility.labelWidth = 75;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("moveBackAnimationRunnerClipDelay"), new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("moveBackAnimationRunnerClipSpeed"), new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                    EditorGUILayout.EndHorizontal();
                                    

                                    EditorGUILayout.Space();


                                }

                                ResetLabelWidth();


                                EditorGUILayout.EndVertical();
                                #endregion

                                #region Move Back Animation
                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 145;
                                EditorGUILayout.PropertyField(GetTarget.FindProperty("moveBackAnimatorParameter"), new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                if (GetTarget.FindProperty("moveBackAnimatorParameter").stringValue != "") {

                                    InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                    EditorGUILayout.PropertyField(GetTarget.FindProperty("moveBackAnimatorParameterType"), new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                    EditorGUILayout.Space();

                                    EditorGUIUtility.labelWidth = 150;


                                    if (((string)GetTarget.FindProperty("moveBackAnimatorParameterType").enumNames[GetTarget.FindProperty("moveBackAnimatorParameterType").enumValueIndex]) != "Trigger") {
                                        //EditorGUILayout.BeginHorizontal();
                                        // if not trigger we need to know the value to switch on and off
                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("moveBackAnimatorOnValue"), new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.PropertyField(GetTarget.FindProperty("moveBackAnimatorOffValue"), new GUIContent("Off Value"), GUILayout.MaxWidth(230));

                                        

                                    }
                                }



                                ResetLabelWidth();



                                EditorGUILayout.EndVertical();
                                #endregion

                                EditorGUILayout.EndVertical();

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #endregion


                                #endregion

                            } else {
                                EditorGUILayout.HelpBox("AI Navigation Not Enabled, enable to set navigation animations.", MessageType.Warning);
                            }

                            break; 


                    }


                    #endregion


                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndScrollView();
                    #endregion



                    EditorGUILayout.EndHorizontal(); 

                    break;


            }


           

            #endregion


        }

    }


    public void OnEnable() {

        toolbarABC = new GUIContent[] { new GUIContent(" Settings", Resources.Load("ABC-EditorIcons/Settings", typeof(Texture2D)) as Texture2D, "  Settings"),
        new GUIContent(" Target Settings", Resources.Load("ABC-EditorIcons/Target", typeof(Texture2D)) as Texture2D, "Target Settings"),
        new GUIContent(" ABC Groups", Resources.Load("ABC-EditorIcons/Groups", typeof(Texture2D)) as Texture2D, "ABC Groups"),
        new GUIContent(" Weapon Settings", Resources.Load("ABC-EditorIcons/Weapons", typeof(Texture2D)) as Texture2D, "Weapon Settings"),
        new GUIContent(" AI", Resources.Load("ABC-EditorIcons/AI", typeof(Texture2D)) as Texture2D, "AI")};


        AddIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Add");
        RemoveIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Remove");
        CopyIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Copy");
        ExportIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Export");
        ImportIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Import");

        //setup styles 
        textureButton.alignment = TextAnchor.MiddleCenter;
        
    }

    //Target update and applymodifiedproperties are in the inspector update call to reduce lag. 
    public void OnInspectorUpdate() {

        if (abilityCont != null) { 
            //Apply the changes to our list if an update has been made
                //take current state of the SerializedObject, and updates the real object.
                GetTarget.ApplyModifiedProperties();

            //Double check any list edits will get saved
            if (GUI.changed)
                EditorUtility.SetDirty(abilityCont);

            //Update our list (takes the current state of the real object, and updates the SerializedObject)
            GetTarget.Update();
        

            //Will update values in editor at runtime
            if (abilityCont.updateEditorAtRunTime == true) {
                Repaint();
            }
        }

    }
}