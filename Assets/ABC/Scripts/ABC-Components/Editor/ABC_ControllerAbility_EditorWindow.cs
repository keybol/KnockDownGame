using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;

public class ABC_ControllerAbility_EditorWindow : EditorWindow {
    [MenuItem("Window/ABC/Ability Manager")]

    public static void ShowWindow() {
        EditorWindow window = EditorWindow.GetWindow(typeof(ABC_ControllerAbility_EditorWindow));
        window.maxSize = new Vector2(windowWidth, windowHeight);
        window.minSize = window.maxSize;
    }



    #region Window Sizes

     static  float windowHeight = 660f;
     static  float windowWidth = 1025f;


    //Width of ability list in left side 
    int abilityListWidth = 160;

    //Width of first column in left part of main body 
    int abilityInfoWidth = 170;

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


    #region Design Functions

    public void InspectorToolbarAndAbilityInfo(ref int toolbarSelection, ref string[] toolbar) {

        //return if no abilities or we just deleted an ability (current index is greater then list count)
        if (AbilityCount == 0 || currentAbilityIndex > abilityCont.Abilities.Count() - 1)
            return;


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

        toolbarSelection = GUILayout.SelectionGrid(toolbarSelection, toolbar, 1);


        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();




        InspectorHeader(meAbilityList.GetArrayElementAtIndex(currentAbilityIndex).FindPropertyRelative("name").stringValue);


        Texture2D iconImage = abilityCont.Abilities[currentAbilityIndex].iconImage.Texture2D;
        string description = meAbilityList.GetArrayElementAtIndex(currentAbilityIndex).FindPropertyRelative("description").stringValue;



        if (description == "" || description == " ") {
            description = "Ability Description \n";
        }

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

        GUILayout.Label(description, GUILayout.MinWidth(abilityInfoWidth));
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();


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

        //uncomment below to modify ID
        //EditorGUILayout.PropertyField(abilityID, GUILayout.MaxWidth(180));
        EditorGUILayout.LabelField("ID: " + abilityID.intValue.ToString(), GUILayout.MaxWidth(abilityInfoWidth));


        if (GUILayout.Button("Activate")) {
            abilityCont.TriggerAbility(abilityID.intValue);
        }


        


        //If application is playing then show a disable, enable button to call right method
        if (Application.isPlaying) {
            EditorGUILayout.LabelField("Enabled: " + abilityEnabled.boolValue.ToString(), GUILayout.MaxWidth(abilityInfoWidth));

            if (GUILayout.Button((abilityEnabled.boolValue) ? "Disable" : "Enable")) {

                if (abilityEnabled.boolValue) {
                    abilityCont.DisableAbility(abilityID.intValue);
                }  else {
                    abilityCont.EnableAbility(abilityID.intValue);
                }
            }


        } else {
            EditorGUILayout.PropertyField(abilityEnabled, new GUIContent("Enabled"));
            EditorGUILayout.PropertyField(autoCast, new GUIContent("Auto Cast"));
        }

      

            EditorGUILayout.PropertyField(meAbilityList.GetArrayElementAtIndex(currentAbilityIndex).FindPropertyRelative("enableExport"), new GUIContent("Export"));
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();




        #region Parent Group Assignment / Show Child

        InspectorHelpBox("Parent assignment is for list grouping only and does not affect game mechanics", false, true);

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

        //Assign ability to a parent ability to sit under (will only show child abilities in inspector when parent is selected) used for UI grouping only
        List<string> parentAbilitiesList = new List<string>();
        //Add default none
        parentAbilitiesList.Add("None");
        //Get list of ability names
        parentAbilitiesList.AddRange(abilityCont.Abilities.Select(item => item.name).ToList());

        if (parentAbilityID.intValue > 0 && abilityCont.FindAbility(parentAbilityID.intValue) != null) {
            //Track which ability is currently selected as a parent
            EditorGUILayout.LabelField("Parent: " + abilityCont.FindAbility(parentAbilityID.intValue).name);
        } else {
            EditorGUILayout.LabelField("Parent: None ");
        }

        EditorGUILayout.BeginHorizontal();
        inspectorParentAbilityListChoice.intValue = EditorGUILayout.Popup(inspectorParentAbilityListChoice.intValue, parentAbilitiesList.ToArray(), GUILayout.Width(110));
        if (GUILayout.Button(new GUIContent("Update"))) {
            //Record the ID of the parent if greater then 0 
            if (inspectorParentAbilityListChoice.intValue == 0) {
                parentAbilityID.intValue = 0;
            }
            else {
                parentAbilityID.intValue = abilityCont.Abilities[inspectorParentAbilityListChoice.intValue - 1].abilityID;
            }

            EditorUtility.SetDirty(abilityCont);
            CreateAbilityReorderableList();
        }
        EditorGUILayout.EndHorizontal();





        if (abilityCont.Abilities.Where(a => a.parentAbilityID == abilityID.intValue).Count() > 0) {
            if (GUILayout.Button((showChildrenInInspector.boolValue) ? "Hide Child Abilities" : "Show Child Abilities")) {

                if (showChildrenInInspector.boolValue) {
                    abilityCont.Abilities[currentAbilityIndex].showChildrenInInspector = false;
                }
                else {
                    abilityCont.Abilities[currentAbilityIndex].showChildrenInInspector = true;
                }

                EditorUtility.SetDirty(abilityCont);
                CreateAbilityReorderableList();
            }

        }

   

        EditorGUILayout.EndVertical();

        #endregion

 


    }

    public void InspectorHeader(string text) {
        EditorGUILayout.Space();
        Color originalTextColor = GUI.skin.button.normal.textColor;


        GUIStyle myStyle = new GUIStyle("Box");
        if (EditorGUIUtility.isProSkin)
        {
            myStyle.normal.textColor = inspectorSectionHeaderTextProColor;
        } else
        {
            myStyle.normal.textColor = inspectorSectionHeaderTextColor;
        }
        myStyle.alignment = TextAnchor.MiddleCenter;
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
        GUI.contentColor = Color.white;
        GUI.skin.box.normal.textColor = Color.white;
        GUILayout.Box(" " + text, myStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.MinHeight(30)});
        GUI.color = Color.white;
        GUI.skin.box.normal.textColor = originalTextColor;
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
        GUILayout.Box(" " + text, myStyle, new GUILayoutOption[] { GUILayout.MaxWidth(minimumAllWaySectionWidth) });

        GUI.color = Color.red;
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
        } else
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
            myStyle.fontSize = 10;
            myStyle.fontStyle = FontStyle.Normal;

            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = inspectorSectionHelpProColor;
            } else
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

    public void InspectorPropertyBox(string header, SerializedProperty list, int listIndex) {
        Color originalTextColor = GUI.skin.button.normal.textColor;

        EditorGUILayout.BeginVertical();
        GUI.color = new Color32(208, 212, 211, 255);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box(header, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(19) });
        GUI.skin.button.normal.textColor = Color.red;

        GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
        if (GUILayout.Button(UpArrowSymbol.ToString()))
        {
            list.MoveArrayElement(listIndex, listIndex - 1);
        }
        if (GUILayout.Button(DownArrowSymbol.ToString()))
        {
            list.MoveArrayElement(listIndex, listIndex + 1);
        }


        GUI.skin.button.normal.textColor = Color.red;
        if (GUILayout.Button("X", GUILayout.Width(40)))
        {
            list.DeleteArrayElementAtIndex(listIndex);
        }

        GUILayout.EndHorizontal();
        GUI.color = Color.white;
        GUI.skin.button.normal.textColor = originalTextColor;
        EditorGUILayout.EndVertical();

    }

    public void InspectorBoldVerticleLine() {
        GUI.color = Color.white;
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.Width(1f), GUILayout.ExpandHeight(true) });
        GUI.color = Color.white;


    }

    public void ResetLabelWidth() {

        EditorGUIUtility.labelWidth = 110;

    }

    public void InspectorHorizontalSpace(int width) {
        EditorGUILayout.LabelField("", GUILayout.Width(width));
    }

    public void InspectorListBox(string title, SerializedProperty list ) {
        Color originalTextColor = GUI.skin.button.normal.textColor;

        EditorGUILayout.BeginVertical();
        GUI.color = new Color32(208, 212, 211, 255);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box(title, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(19) });
        GUI.color = Color.white;
        GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
        if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(30))) {
            list.InsertArrayElementAtIndex(list.arraySize);
            if (list.type == "String") {
                list.GetArrayElementAtIndex(list.arraySize - 1).stringValue = "";
            }
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

    // places an ability select list 
    public void InspectorAbilityListBox(string title, SerializedProperty list) {
        Color originalTextColor = GUI.skin.button.normal.textColor;


        EditorGUILayout.BeginVertical();
        GUI.color = new Color32(208, 212, 211, 255);
        GUILayout.BeginHorizontal("box");


        //List<string> abilityList = new List<string>();


        GUI.color = Color.white;
        abilityListChoice = EditorGUILayout.Popup(title, abilityListChoice, abilityCont.Abilities.Select(item => item.name).ToArray(), GUILayout.MinWidth(250));
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
                ABC_Ability a = abilityCont.Abilities.Where(item => item.abilityID == element.intValue).FirstOrDefault();

                if (a != null)
                EditorGUILayout.LabelField(a.name);

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

    // places an group select list 
    public void InspectorAbilityGroupListBox(string title, SerializedProperty list, SerializedProperty listChoice) {
        Color originalTextColor = GUI.skin.button.normal.textColor;

        EditorGUILayout.BeginVertical();
        GUI.color = new Color32(208, 212, 211, 255);
        GUILayout.BeginHorizontal("box");


        GUI.color = Color.white;
        listChoice.intValue = EditorGUILayout.Popup(title, listChoice.intValue, abilityCont.AbilityGroups.Select(item => item.groupName).ToArray());
  
        GUI.color = Color.white;
        if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(40))) {

            var stateIndex = list.arraySize;
            list.InsertArrayElementAtIndex(stateIndex);

            SerializedProperty groupID = list.GetArrayElementAtIndex(stateIndex);

            groupID.intValue = abilityCont.AbilityGroups[listChoice.intValue].groupID;


        }

        GUILayout.EndHorizontal();

        GUI.color = Color.white;

    // 3 verticle lists
        GUILayout.BeginHorizontal();

        if (list.arraySize > 0) {
        EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < list.arraySize; i += 3) {


                SerializedProperty element = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(abilityCont.AbilityGroups.Where(item => item.groupID == element.intValue).FirstOrDefault().groupName);

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

        if (list.arraySize > 1) { 
        EditorGUILayout.BeginVertical("box");
            for (int i = 1; i < list.arraySize; i += 3) {


                SerializedProperty element = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(abilityCont.AbilityGroups.Where(item => item.groupID == element.intValue).FirstOrDefault().groupName);

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

        if (list.arraySize > 2) {
            EditorGUILayout.BeginVertical("box");
            for (int i = 2; i < list.arraySize; i += 3) {


                SerializedProperty element = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(abilityCont.AbilityGroups.Where(item => item.groupID == element.intValue).FirstOrDefault().groupName);

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

        GUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }




    // symbols used for aesthetics only
    char SmallRightArrowSymbol = '\u25B8';
    char SmallDownArrowSymbol = '\u25BE';
    char BranchedSymbol = '\u251C';
    char DiamondSymbol = '\u25C6';
    char UpArrowSymbol = '\u2191';
    char DownArrowSymbol = '\u2193';
    // used to space out button text 
    char buttonSpace = '\u0590';

    // Button Icons
    Texture AddIcon;
    Texture SortIcon;
    Texture RemoveIcon;
    Texture ExportIcon;
    Texture CopyIcon;
    Texture ImportIcon; 


    Vector2 editorScrollPos;
    Vector2 listScrollPos;

    #endregion


    private ReorderableList reorderableListAbilities;

    ABC_Controller abilityCont;
    SerializedObject GetTarget;
    SerializedProperty meAbilityList;


    //Used for selecting abilities
    List<ABC_Ability> reorderableAbilitylist = new List<ABC_Ability>();
    //Used for filtering abilities
    private string abilitySearchString = "";
    //Used to keep track of what we last filtered
    private string previousAbilitySearchString = "";


    public int toolbarABCSelection;

    public GUIContent[] toolbarABC;

    public int generalSettingsToolbarSelection;
    public string[] generalSettingsToolbar = new string[] { "Settings", "Key & Combo", "Effects", "Additional Settings", };


    public int positiontravelSettingsToolbarSelection;
    public string[] positiontravelSettingsToolbar = new string[] { "Position & Travel Type", "Travelling", "Additional Starting Positions"};


    public int collisionimpactSettingsToolbarSelection;
    public string[] collisionimpactSettingsToolbar = new string[] { "Collider", "Collision", "Impact"};


    public int animationSettingsToolbarSelection;
    public string[] animationSettingsToolbar = new string[] { "Preparing", "Initiating", "Ability End", "Graphic To Start", "Scrollable Ability", "Reload"};


    // ENUM Used for clicing effects in Inspector () 

    enumEffects effects; 

    // ************************* Settings / Serialized Properties ***********************************

    #region Settings (Serialized Properties)

    int currentAbilityIndex = 0;
    // which is currently selected from list
    int abilityListChoice;

    //Which is currently selected from the side list filter
    List<string> sideListFilterOptions = new List<string>();
    int sideListFilterChoice = 0;
    int sideListPreviousFilterChoice = 0; 


    int AbilityCount;


    SerializedProperty MyListRef;
    new SerializedProperty name;
    SerializedProperty abilityID;
    SerializedProperty allowAbilityGroupAssignment;
    SerializedProperty assignedAbilityGroupIDs;
    SerializedProperty abilityGroupListChoice;
    SerializedProperty inspectorParentAbilityListChoice;
    SerializedProperty parentAbilityID;
    SerializedProperty showChildrenInInspector;
    SerializedProperty description;
    SerializedProperty iconImage;
    SerializedProperty iconUIs;
    SerializedProperty autoCast;
    SerializedProperty abilityEnabled;
    SerializedProperty enableDuration;
    SerializedProperty enableAfterEvent;
    SerializedProperty enableAfterAbilityIDsActivated;
    SerializedProperty enableAfterAbilityIDsCollided;
    SerializedProperty enableAbilityActivationLinks;
    SerializedProperty enableAbilityTriggerLinks;
    SerializedProperty triggerLinkAbilityIDs;
    SerializedProperty activationLinkAbilityIDs;
    SerializedProperty manaCost;
    SerializedProperty reduceManaWhenActive;
    SerializedProperty neverSleep;
    SerializedProperty isKinematic;
    SerializedProperty mass;
    SerializedProperty particle;
    SerializedProperty chooseLayer;
    SerializedProperty abLayer;
    SerializedProperty childParticle;
    SerializedProperty useColliderTrigger;
    SerializedProperty applyGravity;
    SerializedProperty applyGravityDelay;
    SerializedProperty travelType;
    SerializedProperty travelPhysics;
    SerializedProperty travelDrag;
    SerializedProperty customTravelScript;
    SerializedProperty crossHairRaycastRadius;
    SerializedProperty crossHairRecordTargetOnActivation;
    SerializedProperty crossHairRaycastReturnDistancePointOnly;
    SerializedProperty crossHairRaycastReturnedDistance;
    SerializedProperty targetTravel;
    SerializedProperty travelWithCaster;
    SerializedProperty boomerangMode;
    SerializedProperty boomerangDelay;
    SerializedProperty requireCrossHair;
    SerializedProperty scrollSetUnsetRaiseEvent;
    SerializedProperty scrollQuickInputType;
    SerializedProperty scrollQuickButton;
    SerializedProperty scrollQuickKey;
    SerializedProperty scrollSwapDuration;
    SerializedProperty enableAbilityEffectLinks;
    SerializedProperty effectLinkAbilityIDs;

    SerializedProperty targetFacing;
    SerializedProperty noTargetStillTravel;
    SerializedProperty auxiliarySoftTarget;
    SerializedProperty startingPositionAuxiliarySoftTarget;
    SerializedProperty abilityBeforeSoftTarget;
    SerializedProperty seekTargetDelay;
    SerializedProperty selectedTargetRestrictTargets;
    SerializedProperty selectedTargetOnlyCastOnTag;
    SerializedProperty continuouslyTurnToDestination;
    SerializedProperty affectOnlyTarget;
    SerializedProperty scrollAbility;



    SerializedProperty tempAbilityActivationIntervalAdjustment;
    SerializedProperty neverResetOtherCombos;
    SerializedProperty abilityCombo;
    SerializedProperty comboNextActivateTime;
    SerializedProperty comboHitRequired;

    SerializedProperty randomlySwapAbilityPosition;
    SerializedProperty LandOrAir;
    SerializedProperty castableDuringHitPrevention;
    SerializedProperty hitPreventionWontInterruptActivation;
    SerializedProperty forceActivation;
    SerializedProperty forceActivationInterruptCurrentActivation;
    SerializedProperty airAbilityDistanceFromGround;

    SerializedProperty triggerType;
    SerializedProperty inputCombo;

    SerializedProperty keyInputType;
    SerializedProperty keyButton;
    SerializedProperty key;
    SerializedProperty onKeyPress;
    SerializedProperty onKeyDown;

    SerializedProperty requireAdditionalKeyInput;
    SerializedProperty additionalKeyInputType;
    SerializedProperty additionalKeyButton;
    SerializedProperty additionalKey;
    SerializedProperty additionalOnKeyPress;
    SerializedProperty additionalOnKeyDown;

    SerializedProperty abilityRecast;
    SerializedProperty startRecastAfterAbilityEnd;
    SerializedProperty travelSpeed;
    SerializedProperty travelDelay;
    SerializedProperty applyTravelDuration;
    SerializedProperty travelDurationTime;
    SerializedProperty travelDurationStopSuddenly;
    SerializedProperty duration;
    SerializedProperty abilityToggle;
    SerializedProperty canCastWhenToggled;
    SerializedProperty repeatInitiatingAnimationWhilstToggled;
    SerializedProperty stateEffects;
    SerializedProperty additionalStartPositions;
    SerializedProperty onEnter;
    SerializedProperty onStay;
    SerializedProperty onExit;
    SerializedProperty particleCollision;

    SerializedProperty enableCollisionAfterKeyPress;
    SerializedProperty enableCollisionAfterKeyInputType;
    SerializedProperty enableCollisionAfterKeyButton;
    SerializedProperty enableCollisionAfterKey;

    SerializedProperty abilityRequiredTag;
    SerializedProperty abilityIgnoreTag;
    SerializedProperty startingPosition;
    SerializedProperty affectOriginObject;
    SerializedProperty affectLayer;

    SerializedProperty startingPositionOnObject;
    SerializedProperty startingPositionOnTag;
    SerializedProperty startingPositionOffset;
    SerializedProperty startingPositionForwardOffset;
    SerializedProperty startingPositionRightOffset;

    SerializedProperty startingRotation;
    SerializedProperty setEulerRotation;
    SerializedProperty prepareTime;
    SerializedProperty ignorePrepareTimeAdjustments;
    SerializedProperty preparingAestheticsPositionOffset;
    SerializedProperty moveInteruptPreparation;
    SerializedProperty waitForKeyBeforeInitiating;
    SerializedProperty waitBeforeInitiatingInputType;
    SerializedProperty waitBeforeInitiatingButton;
    SerializedProperty waitBeforeInitiatingKey;
    SerializedProperty waitForKeyBeforeInitiatingDelay;
    SerializedProperty waitForKeyAllowChangeOfTarget;
    SerializedProperty distanceInteruptPreparation;
    SerializedProperty abilityCollisionIgnores;
    SerializedProperty impactDestroy;
    SerializedProperty destroyDelay;
    SerializedProperty destroyIgnoreTag;

    SerializedProperty includeSurroundingObject;
    SerializedProperty surroundingObjectTarget;
    SerializedProperty lockSurroundingObject;
    SerializedProperty minimumScatterRange;
    SerializedProperty maximumScatterRange;
    SerializedProperty surroundingObjectTargetRange;
    SerializedProperty destroySurroundingObject;
    SerializedProperty projectileAffectSurroundingObject;
    SerializedProperty surroundingObjectTargetRequired;
    SerializedProperty surroundingObjectTargetRestrict;
    SerializedProperty surroundingObjectTargetAffectTag;
    SerializedProperty surroundingObjectAuxiliarySoftTarget;

    SerializedProperty surroundingObjectTags;
    SerializedProperty surroundingObjectTagRequired;
    SerializedProperty surroundingObjectTagAffectTag;
    SerializedProperty surroundingObjectTagLimit;
    SerializedProperty surroundingObjectTagsRange;
    SerializedProperty applyColliderToSurroundingObjects;
    SerializedProperty applyColliderWhenProjectileReached;

    SerializedProperty sendObjectToProjectile;
    SerializedProperty objectToProjectileDuration;
    SerializedProperty useScrollAbilityAesthetics;
    SerializedProperty scrollAbilityAnimateOnScrollGraphic;
    SerializedProperty scrollAbilityAnimateOnEntity;
    SerializedProperty scrollAbilityAnimateOnWeapon;



    SerializedProperty useScrollAbilityAestheticsAnimationWhenDeactivating;



    SerializedProperty scrollAbilityAestheticsPositionOffset;
    SerializedProperty scrollAbilityAnimatorParameter;
    SerializedProperty scrollAbilityAnimatorParameterType;
    SerializedProperty scrollAbilityAnimatorOnValue;
    SerializedProperty scrollAbilityAnimatorOffValue;
    SerializedProperty scrollAbilityAnimatorDuration;
    SerializedProperty scrollAbilityParticle;
    SerializedProperty scrollAbilityObject;
    SerializedProperty scrollAbilityGraphicActivateDelay;
    SerializedProperty scrollAbilityGraphicDeactivateDelay;

    SerializedProperty scrollAbilityStartPosition;
    SerializedProperty scrollAbilityPositionOnObject;
    SerializedProperty scrollAbilityPositionOnTag;
    SerializedProperty scrollAbilityAestheticUseDuration;
    SerializedProperty scrollAbilityAestheticDuration;
    SerializedProperty scrollAbilityPersistantAestheticInactivePosition;
    SerializedProperty scrollAbilityPersistantAestheticInactivePositionOnObject;
    SerializedProperty scrollAbilityPersistantAestheticInactivePositionOnTag;

    SerializedProperty scrollAbilityAnimationRunnerClip;
    SerializedProperty scrollAbilityAnimationRunnerClipSpeed;
    SerializedProperty scrollAbilityAnimationRunnerClipDelay;
    SerializedProperty scrollAbilityAnimationRunnerClipDuration;
    SerializedProperty scrollAbilityAnimationRunnerOnEntity;
    SerializedProperty scrollAbilityAnimationRunnerOnScrollGraphic;
    SerializedProperty scrollAbilityAnimationRunnerOnWeapon;


    SerializedProperty scrollAbilityDeactivateAnimationRunnerClip;
    SerializedProperty scrollAbilityDeactivateAnimationRunnerClipSpeed;
    SerializedProperty scrollAbilityDeactivateAnimationRunnerClipDelay;
    SerializedProperty scrollAbilityDeactivateAnimationRunnerClipDuration;
    SerializedProperty scrollAbilityDeactivateAnimationRunnerOnEntity;
    SerializedProperty scrollAbilityDeactivateAnimationRunnerOnScrollGraphic;
    SerializedProperty scrollAbilityDeactivateAnimationRunnerOnWeapon;


    SerializedProperty scrollAbilityDeactivateAnimateOnScrollGraphic;
    SerializedProperty scrollAbilityDeactivateAnimateOnEntity;
    SerializedProperty scrollAbilityDeactivateAnimateOnWeapon;
    SerializedProperty scrollAbilityDeactivateAnimatorParameter;
    SerializedProperty scrollAbilityDeactivateAnimatorParameterType;
    SerializedProperty scrollAbilityDeactivateAnimatorOnValue;
    SerializedProperty scrollAbilityDeactivateAnimatorOffValue;
    SerializedProperty scrollAbilityDeactivateAnimatorDuration;

    SerializedProperty useReloadAbilityAesthetics;
    SerializedProperty reloadAbilityAnimateOnScrollGraphic;
    SerializedProperty reloadAbilityAnimateOnEntity;
    SerializedProperty reloadAbilityAnimateOnWeapon;
    SerializedProperty reloadAbilityAestheticsPositionOffset;
    SerializedProperty reloadAbilityAnimatorParameter;
    SerializedProperty reloadAbilityAnimatorParameterType;
    SerializedProperty reloadAbilityAnimatorOnValue;
    SerializedProperty reloadAbilityAnimatorOffValue;
    SerializedProperty reloadAbilityParticle;
    SerializedProperty reloadAbilityObject;
    SerializedProperty reloadAbilityAestheticDuration;
    SerializedProperty reloadAbilityStartPosition;
    SerializedProperty reloadAbilityPositionOnObject;
    SerializedProperty reloadAbilityPositionOnTag;

    SerializedProperty reloadAbilityAnimationRunnerClip;
    SerializedProperty reloadAbilityAnimationRunnerClipSpeed;
    SerializedProperty reloadAbilityAnimationRunnerClipDelay;
    SerializedProperty reloadAbilityAnimationRunnerOnEntity;
    SerializedProperty reloadAbilityAnimationRunnerOnScrollGraphic;
    SerializedProperty reloadAbilityAnimationRunnerOnWeapon;


    SerializedProperty useProjectileToStartPosition;
    SerializedProperty useOriginalProjectilePTS;
    SerializedProperty projToStartPosParticle;
    SerializedProperty projToStartPosObject;
    SerializedProperty projToStartPosDuration;
    SerializedProperty projToStartStartingPosition;
    SerializedProperty projToStartRotateToTarget;
    SerializedProperty projToStartRotation;
    SerializedProperty projToStartSetEulerRotation;
    SerializedProperty projToStartPositionOnObject;
    SerializedProperty projToStartPositionOnTag;
    SerializedProperty projToStartPositionOffset;
    SerializedProperty projToStartPositionForwardOffset;
    SerializedProperty projToStartPositionRightOffset;
    SerializedProperty projToStartHoverOnSpot;
    SerializedProperty projToStartHoverDistance;
    SerializedProperty projToStartmoveWithTarget;
    SerializedProperty projToStartReachPositionTime;
    SerializedProperty projToStartTravelDelay;

    SerializedProperty UseAmmo;
    SerializedProperty useEquippedWeaponAmmo;
    SerializedProperty ammoCount;
    SerializedProperty reduceAmmoWhilstActive;
    SerializedProperty useReload;
    SerializedProperty reloadDuration;
    SerializedProperty reloadRestrictAbilityActivationDuration;
    SerializedProperty clipSize;
    SerializedProperty autoReloadWhenRequired;
    SerializedProperty reloadFillClip;
    SerializedProperty reloadFillClipRepeatGraphic;


    SerializedProperty bounceMode;
    SerializedProperty bounceAmount;
    SerializedProperty bounceTarget;
    SerializedProperty bounceTag;
    SerializedProperty bounceRange;
    SerializedProperty bounceOnCaster;
    SerializedProperty enableRandomBounce;
    SerializedProperty startBounceTagRequired;
    SerializedProperty startBounceRequiredTags;
    SerializedProperty bouncePositionOffset;
    SerializedProperty bouncePositionForwardOffset;
    SerializedProperty bouncePositionRightOffset;


    SerializedProperty useGraphicRadius;
    SerializedProperty colliderRadius;
    SerializedProperty applyColliderSettingsToParent;
    SerializedProperty applyColliderSettingsToChildren;

    SerializedProperty meleeKeepRotatingToSelectedTarget;
    SerializedProperty rotateToSelectedTarget;
    SerializedProperty rotateToCameraCenter;
    SerializedProperty hitsStopMeleeAttack;

    SerializedProperty colliderOffset;
    SerializedProperty useDestroySplashEffect;
    SerializedProperty destroySplashRadius;
    SerializedProperty destroySplashExplosion;
    SerializedProperty destroySplashExplosionPower;
    SerializedProperty destroySplashExplosionTagLimit;
    SerializedProperty destroySplashExplosionAffectTag;
    SerializedProperty destroySplashExplosionRadius;
    SerializedProperty destroySplashExplosionUplift;

    SerializedProperty modifyGameSpeedOnInitiation;
    SerializedProperty modifyGameSpeedOnInitiationSpeedFactor;
    SerializedProperty modifyGameSpeedOnInitiationDelay;
    SerializedProperty modifyGameSpeedOnInitiationDuration;

    SerializedProperty modifyGameSpeedOnImpact;
    SerializedProperty modifyGameSpeedOnImpactSpeedFactor;
    SerializedProperty modifyGameSpeedOnImpactDuration;

    SerializedProperty shakeCameraOnImpact;
    SerializedProperty shakeCameraOnImpactDuration;
    SerializedProperty shakeCameraOnImpactAmount;
    SerializedProperty shakeCameraOnImpactSpeed;

    SerializedProperty attachToObjectOnImpact;
    SerializedProperty attachToObjectProbabilityMinValue;
    SerializedProperty attachToObjectProbabilityMaxValue;
    SerializedProperty attachToObjectStickOutFactor;
    SerializedProperty attachToObjectNearestBone;

    SerializedProperty useInitiatingAesthetics;
    SerializedProperty initiatingAnimateOnEntity;
    SerializedProperty initiatingAnimateOnScrollGraphic;
    SerializedProperty initiatingAnimateOnWeapon;
    SerializedProperty defyGravityInitiating;
    SerializedProperty defyGravityInitiatingDuration;
    SerializedProperty defyGravityInitiatingRaiseEvent;
    SerializedProperty initiatingAestheticsPositionOffset;
    SerializedProperty initiatingAnimatorParameter;
    SerializedProperty initiatingParticle;
    SerializedProperty initiatingObject;
    SerializedProperty initiatingAestheticDuration;
    SerializedProperty initiatingAestheticDelay;
    SerializedProperty delayBetweenInitiatingAndProjectile;
    SerializedProperty initiatingStartPosition;
    SerializedProperty initiatingPositionOnObject;
    SerializedProperty initiatingPositionOnTag;
    SerializedProperty initiatingAnimatorParameterType;
    SerializedProperty initiatingAnimatorOnValue;
    SerializedProperty initiatingAnimatorOffValue;
    SerializedProperty initiatingAnimatorDuration;

    SerializedProperty initiatingAnimationRunnerClip;
    SerializedProperty initiatingAnimationRunnerClipSpeed;
    SerializedProperty initiatingAnimationRunnerClipDelay;
    SerializedProperty initiatingAnimationRunnerClipDuration;
    SerializedProperty initiatingAnimationRunnerOnEntity;
    SerializedProperty initiatingAnimationRunnerOnScrollGraphic;
    SerializedProperty initiatingAnimationRunnerOnWeapon;

    SerializedProperty moveSelfWhenInitiating;
    SerializedProperty moveSelfInitiatingOffset;
    SerializedProperty moveSelfInitiatingForwardOffset;
    SerializedProperty moveSelfInitiatingRightOffset;
    SerializedProperty moveSelfInitiatingDelay;
    SerializedProperty moveSelfInitiatingDuration;

    SerializedProperty moveSelfToTargetWhenInitiating;
    SerializedProperty moveSelfToTargetInitiatingDelay;
    SerializedProperty moveSelfToTargetInitiatingDuration;
    SerializedProperty moveSelfToTargetInitiatingStopDistance;
    SerializedProperty moveSelfToTargetInitiatingOffset;
    SerializedProperty moveSelfToTargetInitiatingForwardOffset;
    SerializedProperty moveSelfToTargetInitiatingRightOffset;



    SerializedProperty initiatingAestheticsPositionForwardOffset;
    SerializedProperty initiatingAestheticsPositionRightOffset;
    SerializedProperty preparingAestheticsPositionForwardOffset;
    SerializedProperty preparingAestheticsPositionRightOffset;
    SerializedProperty scrollAbilityAestheticsPositionForwardOffset;
    SerializedProperty scrollAbilityAestheticsPositionRightOffset;
    SerializedProperty reloadAbilityAestheticsPositionForwardOffset;
    SerializedProperty reloadAbilityAestheticsPositionRightOffset;
    SerializedProperty selectedTargetForwardOffset;
    SerializedProperty selectedTargetRightOffset;
    SerializedProperty abilityCanMiss;


    SerializedProperty useAbilityEndAesthetics;
    SerializedProperty abEndParticle;
    SerializedProperty abEndObject;
    SerializedProperty abEndAestheticDuration;

    SerializedProperty showPrepareTimeOnGUI;
    SerializedProperty defyGravityPreparing;
    SerializedProperty defyGravityPreparingDuration;
    SerializedProperty defyGravityPreparingRaiseEvent;
    SerializedProperty usePreparingAesthetics;
    SerializedProperty preparingAnimateOnEntity;
    SerializedProperty preparingAnimateOnScrollGraphic;
    SerializedProperty preparingAnimateOnWeapon;
    SerializedProperty preparingAnimatorParameter;
    SerializedProperty preparingAnimatorParameterType;
    SerializedProperty preparingAnimatorOnValue;
    SerializedProperty preparingAnimatorOffValue;
    SerializedProperty preparingParticle;
    SerializedProperty preparingObject;
    SerializedProperty preparingAestheticDurationUsePrepareTime;
    SerializedProperty preparingAestheticDuration;
    SerializedProperty preparingStartPosition;
    SerializedProperty preparingPositionOnObject;
    SerializedProperty preparingPositionOnTag;


    SerializedProperty moveSelfToTargetWhenPreparing;
    SerializedProperty moveSelfToTargetPreparingDelay;
    SerializedProperty moveSelfToTargetPreparingDuration;
    SerializedProperty moveSelfToTargetPreparingStopDistance;
    SerializedProperty moveSelfToTargetActivatePreparingAnimationOnlyWhenMoving;
    SerializedProperty moveSelfToTargetPreparingOffset;
    SerializedProperty moveSelfToTargetPreparingForwardOffset;
    SerializedProperty moveSelfToTargetPreparingRightOffset;

    SerializedProperty preparingAnimationRunnerClip;
    SerializedProperty preparingAnimationRunnerClipSpeed;
    SerializedProperty preparingAnimationRunnerClipDelay;
    SerializedProperty preparingAnimationRunnerOnEntity;
    SerializedProperty preparingAnimationRunnerOnScrollGraphic;
    SerializedProperty preparingAnimationRunnerOnWeapon;

    SerializedProperty moveSelfWhenPreparing;
    SerializedProperty moveSelfPreparingOffset;
    SerializedProperty moveSelfPreparingForwardOffset;
    SerializedProperty moveSelfPreparingRightOffset;
    SerializedProperty moveSelfPreparingDelay;
    SerializedProperty moveSelfPreparingDuration;

    SerializedProperty colliderTimeDelay;
    SerializedProperty colliderDelayTime;
    SerializedProperty colliderKeyPressDelay;
    SerializedProperty colliderDelayInputType;
    SerializedProperty colliderDelayButton;
    SerializedProperty colliderDelayKey;
    SerializedProperty ignoreActiveTerrain;
    SerializedProperty overrideIgnoreAbilityCollision;
    SerializedProperty overrideWeaponParrying;
    SerializedProperty overrideWeaponBlocking;
    SerializedProperty reduceWeaponBlockDurability;


    SerializedProperty activateAnimationFromHit;
    SerializedProperty activateSpecificHitAnimation;
    SerializedProperty hitAnimationToActivate;

    SerializedProperty spawnObject;
    SerializedProperty spawningObject;
    SerializedProperty spawnObjectOnDestroy;
    SerializedProperty spawnObjectOnCollide;

    SerializedProperty useRange;
    SerializedProperty selectedTargetRangeGreaterThan;
    SerializedProperty selectedTargetRangeLessThan;
    SerializedProperty selectedTargetOffset;
    SerializedProperty addAbilityCollider;
    SerializedProperty abilityType;
    SerializedProperty rayCastRadius;
    SerializedProperty raycastHitAmount;
    SerializedProperty raycastBlockable;
    SerializedProperty raycastIgnoreTerrain;
    SerializedProperty abilityBeforeTarget;
    SerializedProperty loopTillTargetFound;
    SerializedProperty abilityBeforeTargetIndicatorScaleToEffectRadius;

    SerializedProperty rayCastLength;
    SerializedProperty maxActiveAtOnce;
    SerializedProperty limitActiveAtOnce;
    SerializedProperty abilityActivationRaiseEvent;
    SerializedProperty abilityActivationCompleteRaiseEvent;
    SerializedProperty abilityActivationCompleteEventType;
    SerializedProperty loggingEnabled;

    SerializedProperty stopMovementOnInitiate;
    SerializedProperty stopMovementOnInitiateDuration;
    SerializedProperty stopMovementOnInitiateFreezePosition;
    SerializedProperty stopMovementOnInitiateDisableComponents;
    SerializedProperty stopMovementOnInitiateRaiseEvent;

    SerializedProperty stopMovementOnPreparing;
    SerializedProperty stopMovementOnPreparingDuration;
    SerializedProperty stopMovementOnPreparingFreezePosition;
    SerializedProperty stopMovementOnPreparingDisableComponents;
    SerializedProperty stopMovementOnPreparingRaiseEvent;

    SerializedProperty mouseForwardLockX;
    SerializedProperty mouseForwardLockY;
    SerializedProperty mouseForwardLockZ;
    SerializedProperty mouseFrontOnly;

    SerializedProperty travelNearestTagList;
    SerializedProperty travelNearestTagRange;
    SerializedProperty travelNearestTagIgnoreOriginator;
    SerializedProperty travelNearestTagRandomiseSearch;

    #endregion

    void GetProperties(int elementIteration) {

        // return if no abilities exist
        if (abilityCont.Abilities.Count == 0)
            return;


        if (meAbilityList.arraySize == 0)
            return; 

        // remembers what spell we are currently displaying details display details for
        currentAbilityIndex = elementIteration;

        MyListRef = meAbilityList.GetArrayElementAtIndex(elementIteration);
        name = MyListRef.FindPropertyRelative("name");
        abilityID = MyListRef.FindPropertyRelative("abilityID");
        allowAbilityGroupAssignment = MyListRef.FindPropertyRelative("allowAbilityGroupAssignment");
        assignedAbilityGroupIDs = MyListRef.FindPropertyRelative("assignedAbilityGroupIDs");
        abilityGroupListChoice = MyListRef.FindPropertyRelative("abilityGroupListChoice");
        inspectorParentAbilityListChoice = MyListRef.FindPropertyRelative("inspectorParentAbilityListChoice");
        parentAbilityID = MyListRef.FindPropertyRelative("parentAbilityID");
        showChildrenInInspector = MyListRef.FindPropertyRelative("showChildrenInInspector");
        description = MyListRef.FindPropertyRelative("description");
        iconImage = MyListRef.FindPropertyRelative("iconImage");
        autoCast = MyListRef.FindPropertyRelative("autoCast");
        abilityEnabled = MyListRef.FindPropertyRelative("abilityEnabled");
        enableDuration = MyListRef.FindPropertyRelative("enableDuration");
        enableAfterEvent = MyListRef.FindPropertyRelative("enableAfterEvent");
        enableAfterAbilityIDsActivated = MyListRef.FindPropertyRelative("enableAfterAbilityIDsActivated");
        enableAfterAbilityIDsCollided = MyListRef.FindPropertyRelative("enableAfterAbilityIDsCollide");
        enableAbilityActivationLinks = MyListRef.FindPropertyRelative("enableAbilityActivationLinks");
        activationLinkAbilityIDs = MyListRef.FindPropertyRelative("activationLinkAbilityIDs");
        enableAbilityTriggerLinks = MyListRef.FindPropertyRelative("enableAbilityTriggerLinks");
        triggerLinkAbilityIDs = MyListRef.FindPropertyRelative("triggerLinkAbilityIDs");

        manaCost = MyListRef.FindPropertyRelative("manaCost");
        reduceManaWhenActive = MyListRef.FindPropertyRelative("reduceManaWhenActive");
        neverSleep = MyListRef.FindPropertyRelative("neverSleep");
        isKinematic = MyListRef.FindPropertyRelative("isKinematic");
        mass = MyListRef.FindPropertyRelative("mass");
        particle = MyListRef.FindPropertyRelative("mainGraphic");
        chooseLayer = MyListRef.FindPropertyRelative("chooseLayer");
        abLayer = MyListRef.FindPropertyRelative("abLayer");
        childParticle = MyListRef.FindPropertyRelative("subGraphic");
        useColliderTrigger = MyListRef.FindPropertyRelative("useColliderTrigger");
        applyGravity = MyListRef.FindPropertyRelative("applyGravity");
        applyGravityDelay = MyListRef.FindPropertyRelative("applyGravityDelay");
        travelType = MyListRef.FindPropertyRelative("travelType");
        travelPhysics = MyListRef.FindPropertyRelative("travelPhysics");
        travelDrag = MyListRef.FindPropertyRelative("travelDrag");
        customTravelScript = MyListRef.FindPropertyRelative("customTravelScript");
        crossHairRaycastRadius = MyListRef.FindPropertyRelative("crossHairRaycastRadius");
        crossHairRecordTargetOnActivation = MyListRef.FindPropertyRelative("crossHairRecordTargetOnActivation");
        crossHairRaycastReturnDistancePointOnly = MyListRef.FindPropertyRelative("crossHairRaycastReturnDistancePointOnly");
        crossHairRaycastReturnedDistance = MyListRef.FindPropertyRelative("crossHairRaycastReturnedDistance");
        targetTravel = MyListRef.FindPropertyRelative("targetTravel");
        travelWithCaster = MyListRef.FindPropertyRelative("travelWithCaster");
        boomerangMode = MyListRef.FindPropertyRelative("boomerangMode");
        boomerangDelay = MyListRef.FindPropertyRelative("boomerangDelay");
        requireCrossHair = MyListRef.FindPropertyRelative("requireCrossHairOverride");

        scrollSetUnsetRaiseEvent = MyListRef.FindPropertyRelative("scrollSetUnsetRaiseEvent");
        scrollQuickInputType = MyListRef.FindPropertyRelative("scrollQuickInputType");
        scrollQuickButton = MyListRef.FindPropertyRelative("scrollQuickButton");
        scrollQuickKey = MyListRef.FindPropertyRelative("scrollQuickKey");
        scrollSwapDuration = MyListRef.FindPropertyRelative("scrollSwapDuration");

        enableAbilityEffectLinks = MyListRef.FindPropertyRelative("enableAbilityEffectLinks");
        effectLinkAbilityIDs = MyListRef.FindPropertyRelative("effectLinkAbilityIDs");

        targetFacing = MyListRef.FindPropertyRelative("targetFacing");
        noTargetStillTravel = MyListRef.FindPropertyRelative("noTargetStillTravel");
        auxiliarySoftTarget = MyListRef.FindPropertyRelative("auxiliarySoftTarget");
        startingPositionAuxiliarySoftTarget = MyListRef.FindPropertyRelative("startingPositionAuxiliarySoftTarget");
        abilityBeforeSoftTarget = MyListRef.FindPropertyRelative("abilityBeforeSoftTarget");
        seekTargetDelay = MyListRef.FindPropertyRelative("seekTargetDelay");
        selectedTargetRestrictTargets = MyListRef.FindPropertyRelative("selectedTargetRestrictTargets");
        selectedTargetOnlyCastOnTag = MyListRef.FindPropertyRelative("selectedTargetOnlyCastOnTag");
        continuouslyTurnToDestination = MyListRef.FindPropertyRelative("continuouslyTurnToDestination");
        affectOnlyTarget = MyListRef.FindPropertyRelative("affectOnlyTarget");
        scrollAbility = MyListRef.FindPropertyRelative("scrollAbility");
        randomlySwapAbilityPosition = MyListRef.FindPropertyRelative("randomlySwapAbilityPosition");
        LandOrAir = MyListRef.FindPropertyRelative("LandOrAir");
        castableDuringHitPrevention = MyListRef.FindPropertyRelative("castableDuringHitPrevention");
        hitPreventionWontInterruptActivation = MyListRef.FindPropertyRelative("hitPreventionWontInterruptActivation");
        forceActivation = MyListRef.FindPropertyRelative("forceActivation");
        forceActivationInterruptCurrentActivation = MyListRef.FindPropertyRelative("forceActivationInterruptCurrentActivation");
        airAbilityDistanceFromGround = MyListRef.FindPropertyRelative("airAbilityDistanceFromGround");

        tempAbilityActivationIntervalAdjustment = MyListRef.FindPropertyRelative("tempAbilityActivationIntervalAdjustment");
        neverResetOtherCombos = MyListRef.FindPropertyRelative("neverResetOtherCombos"); 
        abilityCombo = MyListRef.FindPropertyRelative("abilityCombo"); 
        comboNextActivateTime = MyListRef.FindPropertyRelative("comboNextActivateTime");
        comboHitRequired = MyListRef.FindPropertyRelative("comboHitRequired");

        triggerType = MyListRef.FindPropertyRelative("triggerType");
        inputCombo = MyListRef.FindPropertyRelative("keyInputCombo");

        keyInputType = MyListRef.FindPropertyRelative("keyInputType");
        keyButton = MyListRef.FindPropertyRelative("keyButton");
        key = MyListRef.FindPropertyRelative("key");

        onKeyPress = MyListRef.FindPropertyRelative("onKeyPress");
        onKeyDown = MyListRef.FindPropertyRelative("onKeyDown");

        requireAdditionalKeyInput = MyListRef.FindPropertyRelative("requireAdditionalKeyInput");
        additionalKeyInputType = MyListRef.FindPropertyRelative("additionalKeyInputType");
        additionalKeyButton = MyListRef.FindPropertyRelative("additionalKeyButton");
        additionalKey = MyListRef.FindPropertyRelative("additionalKey");
        additionalOnKeyPress = MyListRef.FindPropertyRelative("additionalOnKeyPress");
        additionalOnKeyDown  = MyListRef.FindPropertyRelative("additionalOnKeyDown");

        abilityRecast = MyListRef.FindPropertyRelative("abilityRecast");
        startRecastAfterAbilityEnd = MyListRef.FindPropertyRelative("startRecastAfterAbilityEnd");
        travelSpeed = MyListRef.FindPropertyRelative("travelSpeed");
        travelDelay = MyListRef.FindPropertyRelative("travelDelay");
        applyTravelDuration = MyListRef.FindPropertyRelative("applyTravelDuration");
        travelDurationTime = MyListRef.FindPropertyRelative("travelDurationTime");
        travelDurationStopSuddenly = MyListRef.FindPropertyRelative("travelDurationStopSuddenly");
        duration = MyListRef.FindPropertyRelative("duration");
        abilityToggle = MyListRef.FindPropertyRelative("abilityToggle");
        canCastWhenToggled = MyListRef.FindPropertyRelative("canCastWhenToggled");
        repeatInitiatingAnimationWhilstToggled = MyListRef.FindPropertyRelative("repeatInitiatingAnimationWhilstToggled");
        stateEffects = MyListRef.FindPropertyRelative("effects");
        additionalStartPositions = MyListRef.FindPropertyRelative("additionalStartingPositions");
        onEnter = MyListRef.FindPropertyRelative("onEnter");
        onStay = MyListRef.FindPropertyRelative("onStay");
        onExit = MyListRef.FindPropertyRelative("onExit");
        particleCollision = MyListRef.FindPropertyRelative("particleCollision");

        enableCollisionAfterKeyPress = MyListRef.FindPropertyRelative("enableCollisionAfterKeyPress");
        enableCollisionAfterKeyInputType = MyListRef.FindPropertyRelative("enableCollisionAfterKeyInputType");
        enableCollisionAfterKeyButton = MyListRef.FindPropertyRelative("enableCollisionAfterKeyButton");
        enableCollisionAfterKey = MyListRef.FindPropertyRelative("enableCollisionAfterKey");

        abilityRequiredTag = MyListRef.FindPropertyRelative("abilityRequiredTag");
        abilityIgnoreTag = MyListRef.FindPropertyRelative("abilityIgnoreTag");
        startingPosition = MyListRef.FindPropertyRelative("startingPosition");
        affectOriginObject = MyListRef.FindPropertyRelative("affectOriginObject");
        affectLayer = MyListRef.FindPropertyRelative("affectLayer");

        startingPositionOnObject = MyListRef.FindPropertyRelative("startingPositionOnObject");
        startingPositionOnTag = MyListRef.FindPropertyRelative("startingPositionOnTag");
        startingPositionOffset = MyListRef.FindPropertyRelative("startingPositionOffset");
        startingPositionForwardOffset = MyListRef.FindPropertyRelative("startingPositionForwardOffset");
        startingPositionRightOffset = MyListRef.FindPropertyRelative("startingPositionRightOffset");

        startingRotation = MyListRef.FindPropertyRelative("startingRotation");
        setEulerRotation = MyListRef.FindPropertyRelative("setEulerRotation");
        prepareTime = MyListRef.FindPropertyRelative("prepareTime");
        ignorePrepareTimeAdjustments = MyListRef.FindPropertyRelative("ignorePrepareTimeAdjustments");
        preparingAestheticsPositionOffset = MyListRef.FindPropertyRelative("preparingAestheticsPositionOffset");
        moveInteruptPreparation = MyListRef.FindPropertyRelative("moveInteruptPreparation");
        waitForKeyBeforeInitiating = MyListRef.FindPropertyRelative("waitForKeyBeforeInitiating");
        waitBeforeInitiatingInputType = MyListRef.FindPropertyRelative("waitBeforeInitiatingInputType");
        waitBeforeInitiatingButton = MyListRef.FindPropertyRelative("waitBeforeInitiatingButton");
        waitBeforeInitiatingKey = MyListRef.FindPropertyRelative("waitBeforeInitiatingKey");
        waitForKeyBeforeInitiatingDelay = MyListRef.FindPropertyRelative("waitForKeyBeforeInitiatingDelay");
        waitForKeyAllowChangeOfTarget = MyListRef.FindPropertyRelative("waitForKeyAllowChangeOfTarget");
        distanceInteruptPreparation = MyListRef.FindPropertyRelative("distanceInteruptPreparation");
        abilityCollisionIgnores = MyListRef.FindPropertyRelative("abilityCollisionIgnores");
        impactDestroy = MyListRef.FindPropertyRelative("impactDestroy");
        destroyDelay = MyListRef.FindPropertyRelative("destroyDelay");
        destroyIgnoreTag = MyListRef.FindPropertyRelative("destroyIgnoreTag");

        includeSurroundingObject = MyListRef.FindPropertyRelative("includeSurroundingObject");
        surroundingObjectTarget = MyListRef.FindPropertyRelative("surroundingObjectTarget");
        lockSurroundingObject = MyListRef.FindPropertyRelative("lockSurroundingObject");
        minimumScatterRange = MyListRef.FindPropertyRelative("minimumScatterRange");
        maximumScatterRange = MyListRef.FindPropertyRelative("maximumScatterRange");
        surroundingObjectTargetRange = MyListRef.FindPropertyRelative("surroundingObjectTargetRange");
        destroySurroundingObject = MyListRef.FindPropertyRelative("destroySurroundingObject");
        projectileAffectSurroundingObject = MyListRef.FindPropertyRelative("projectileAffectSurroundingObject");
        surroundingObjectTargetRequired = MyListRef.FindPropertyRelative("surroundingObjectTargetRequired");
        surroundingObjectTargetRestrict = MyListRef.FindPropertyRelative("surroundingObjectTargetRestrict");
        surroundingObjectTargetAffectTag = MyListRef.FindPropertyRelative("surroundingObjectTargetAffectTag");
        surroundingObjectAuxiliarySoftTarget = MyListRef.FindPropertyRelative("surroundingObjectAuxiliarySoftTarget");

        surroundingObjectTags = MyListRef.FindPropertyRelative("surroundingObjectTags");
        surroundingObjectTagRequired = MyListRef.FindPropertyRelative("surroundingObjectTagRequired");
        surroundingObjectTagAffectTag = MyListRef.FindPropertyRelative("surroundingObjectTagAffectTag");
        surroundingObjectTagLimit = MyListRef.FindPropertyRelative("surroundingObjectTagLimit");
        surroundingObjectTagsRange = MyListRef.FindPropertyRelative("surroundingObjectTagsRange");
        applyColliderToSurroundingObjects = MyListRef.FindPropertyRelative("applyColliderToSurroundingObjects");
        applyColliderWhenProjectileReached = MyListRef.FindPropertyRelative("applyColliderWhenProjectileReached");

        sendObjectToProjectile = MyListRef.FindPropertyRelative("sendObjectToProjectile");
        objectToProjectileDuration = MyListRef.FindPropertyRelative("objectToProjectileDuration");
        useScrollAbilityAesthetics = MyListRef.FindPropertyRelative("useScrollAbilityAesthetics");
        scrollAbilityAnimateOnEntity = MyListRef.FindPropertyRelative("scrollAbilityAnimateOnEntity");
        scrollAbilityAnimateOnScrollGraphic = MyListRef.FindPropertyRelative("scrollAbilityAnimateOnScrollGraphic");
        scrollAbilityAnimateOnWeapon = MyListRef.FindPropertyRelative("scrollAbilityAnimateOnWeapon");



        scrollAbilityAestheticsPositionOffset = MyListRef.FindPropertyRelative("scrollAbilityAestheticsPositionOffset");
        scrollAbilityAnimatorParameter = MyListRef.FindPropertyRelative("scrollAbilityAnimatorParameter");
        scrollAbilityAnimatorParameterType = MyListRef.FindPropertyRelative("scrollAbilityAnimatorParameterType");
        scrollAbilityAnimatorOnValue = MyListRef.FindPropertyRelative("scrollAbilityAnimatorOnValue");
        scrollAbilityAnimatorOffValue = MyListRef.FindPropertyRelative("scrollAbilityAnimatorOffValue");
        scrollAbilityAnimatorDuration = MyListRef.FindPropertyRelative("scrollAbilityAnimatorDuration");

        scrollAbilityAnimationRunnerClip = MyListRef.FindPropertyRelative("scrollAbilityAnimationRunnerClip");
        scrollAbilityAnimationRunnerClipSpeed = MyListRef.FindPropertyRelative("scrollAbilityAnimationRunnerClipSpeed");
        scrollAbilityAnimationRunnerClipDelay = MyListRef.FindPropertyRelative("scrollAbilityAnimationRunnerClipDelay");
        scrollAbilityAnimationRunnerClipDuration = MyListRef.FindPropertyRelative("scrollAbilityAnimationRunnerClipDuration");
        scrollAbilityAnimationRunnerOnEntity = MyListRef.FindPropertyRelative("scrollAbilityAnimationRunnerOnEntity");
        scrollAbilityAnimationRunnerOnScrollGraphic = MyListRef.FindPropertyRelative("scrollAbilityAnimationRunnerOnScrollGraphic");
        scrollAbilityAnimationRunnerOnWeapon = MyListRef.FindPropertyRelative("scrollAbilityAnimationRunnerOnWeapon");

        scrollAbilityDeactivateAnimationRunnerClip = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimationRunnerClip");
        scrollAbilityDeactivateAnimationRunnerClipSpeed = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimationRunnerClipSpeed");
        scrollAbilityDeactivateAnimationRunnerClipDelay = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimationRunnerClipDelay");
        scrollAbilityDeactivateAnimationRunnerClipDuration = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimationRunnerClipDuration");
        scrollAbilityDeactivateAnimationRunnerOnEntity = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimationRunnerOnEntity");
        scrollAbilityDeactivateAnimationRunnerOnScrollGraphic = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimationRunnerOnScrollGraphic");
        scrollAbilityDeactivateAnimationRunnerOnWeapon = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimationRunnerOnWeapon");


        scrollAbilityDeactivateAnimateOnEntity = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimateOnEntity");
        scrollAbilityDeactivateAnimateOnScrollGraphic = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimateOnScrollGraphic");
        scrollAbilityDeactivateAnimateOnWeapon = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimateOnWeapon");
        scrollAbilityDeactivateAnimatorParameter = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimatorParameter");
        scrollAbilityDeactivateAnimatorParameterType = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimatorParameterType");
        scrollAbilityDeactivateAnimatorOnValue = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimatorOnValue");
        scrollAbilityDeactivateAnimatorOffValue = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimatorOffValue");
        scrollAbilityDeactivateAnimatorDuration = MyListRef.FindPropertyRelative("scrollAbilityDeactivateAnimatorDuration");

        scrollAbilityParticle = MyListRef.FindPropertyRelative("scrollAbilityGraphic");
        scrollAbilityObject = MyListRef.FindPropertyRelative("scrollAbilitySubGraphic");
        scrollAbilityGraphicActivateDelay = MyListRef.FindPropertyRelative("scrollAbilityGraphicActivateDelay");
        scrollAbilityGraphicDeactivateDelay = MyListRef.FindPropertyRelative("scrollAbilityGraphicDeactivateDelay");
        scrollAbilityStartPosition = MyListRef.FindPropertyRelative("scrollAbilityStartPosition");
        scrollAbilityPositionOnObject = MyListRef.FindPropertyRelative("scrollAbilityPositionOnObject");
        scrollAbilityPositionOnTag = MyListRef.FindPropertyRelative("scrollAbilityPositionOnTag");
        scrollAbilityAestheticUseDuration = MyListRef.FindPropertyRelative("scrollAbilityAestheticDurationType");
        scrollAbilityAestheticDuration = MyListRef.FindPropertyRelative("scrollAbilityAestheticDuration");
        scrollAbilityPersistantAestheticInactivePosition = MyListRef.FindPropertyRelative("scrollAbilityPersistantAestheticInactivePosition");
        scrollAbilityPersistantAestheticInactivePositionOnObject = MyListRef.FindPropertyRelative("scrollAbilityPersistantAestheticInactivePositionOnObject");
        scrollAbilityPersistantAestheticInactivePositionOnTag = MyListRef.FindPropertyRelative("scrollAbilityPersistantAestheticInactivePositionOnTag");

        useReloadAbilityAesthetics = MyListRef.FindPropertyRelative("useReloadAbilityAesthetics");
        reloadAbilityAnimateOnEntity = MyListRef.FindPropertyRelative("reloadAbilityAnimateOnEntity");
        reloadAbilityAnimateOnScrollGraphic = MyListRef.FindPropertyRelative("reloadAbilityAnimateOnScrollGraphic");
        reloadAbilityAnimateOnWeapon = MyListRef.FindPropertyRelative("reloadAbilityAnimateOnWeapon");
        reloadAbilityAestheticsPositionOffset = MyListRef.FindPropertyRelative("reloadAbilityAestheticsPositionOffset");
        reloadAbilityAnimatorParameter = MyListRef.FindPropertyRelative("reloadAbilityAnimatorParameter");
        reloadAbilityAnimatorParameterType = MyListRef.FindPropertyRelative("reloadAbilityAnimatorParameterType");
        reloadAbilityAnimatorOnValue = MyListRef.FindPropertyRelative("reloadAbilityAnimatorOnValue");
        reloadAbilityAnimatorOffValue = MyListRef.FindPropertyRelative("reloadAbilityAnimatorOffValue");
        reloadAbilityParticle = MyListRef.FindPropertyRelative("reloadAbilityGraphic");
        reloadAbilityObject = MyListRef.FindPropertyRelative("reloadAbilitySubGraphic");
        reloadAbilityAestheticDuration = MyListRef.FindPropertyRelative("reloadAbilityAestheticDuration");
        reloadAbilityStartPosition = MyListRef.FindPropertyRelative("reloadAbilityStartPosition");
        reloadAbilityPositionOnObject = MyListRef.FindPropertyRelative("reloadAbilityPositionOnObject");
        reloadAbilityPositionOnTag = MyListRef.FindPropertyRelative("reloadAbilityPositionOnTag");

        reloadAbilityAnimationRunnerClip = MyListRef.FindPropertyRelative("reloadAbilityAnimationRunnerClip");
        reloadAbilityAnimationRunnerClipSpeed = MyListRef.FindPropertyRelative("reloadAbilityAnimationRunnerClipSpeed");
        reloadAbilityAnimationRunnerClipDelay = MyListRef.FindPropertyRelative("reloadAbilityAnimationRunnerClipDelay");        
        reloadAbilityAnimationRunnerOnEntity = MyListRef.FindPropertyRelative("reloadAbilityAnimationRunnerOnEntity");
        reloadAbilityAnimationRunnerOnScrollGraphic = MyListRef.FindPropertyRelative("reloadAbilityAnimationRunnerOnScrollGraphic");
        reloadAbilityAnimationRunnerOnWeapon = MyListRef.FindPropertyRelative("reloadAbilityAnimationRunnerOnWeapon");


        useProjectileToStartPosition = MyListRef.FindPropertyRelative("useProjectileToStartPosition");
        useOriginalProjectilePTS = MyListRef.FindPropertyRelative("useOriginalProjectilePTS");
        projToStartPosParticle = MyListRef.FindPropertyRelative("projToStartPosGraphic");
        projToStartPosObject = MyListRef.FindPropertyRelative("projToStartPosSubGraphic");
        projToStartPosDuration = MyListRef.FindPropertyRelative("projToStartPosDuration");
        projToStartStartingPosition = MyListRef.FindPropertyRelative("projToStartStartingPosition");
        projToStartRotateToTarget = MyListRef.FindPropertyRelative("projToStartRotateToTarget");
        projToStartRotation = MyListRef.FindPropertyRelative("projToStartRotation");
        projToStartSetEulerRotation = MyListRef.FindPropertyRelative("projToStartSetEulerRotation");
        projToStartPositionOnObject = MyListRef.FindPropertyRelative("projToStartPositionOnObject");
        projToStartPositionOnTag = MyListRef.FindPropertyRelative("projToStartPositionOnTag");
        projToStartPositionOffset = MyListRef.FindPropertyRelative("projToStartPositionOffset");
        projToStartPositionForwardOffset = MyListRef.FindPropertyRelative("projToStartPositionForwardOffset");
        projToStartPositionRightOffset = MyListRef.FindPropertyRelative("projToStartPositionRightOffset");
        projToStartHoverOnSpot = MyListRef.FindPropertyRelative("projToStartHoverOnSpot");
        projToStartHoverDistance = MyListRef.FindPropertyRelative("projToStartHoverDistance");
        projToStartmoveWithTarget = MyListRef.FindPropertyRelative("projToStartMoveWithTarget");
        projToStartReachPositionTime = MyListRef.FindPropertyRelative("projToStartReachPositionTime");
        projToStartTravelDelay = MyListRef.FindPropertyRelative("projToStartTravelDelay");

        UseAmmo = MyListRef.FindPropertyRelative("UseAmmo");
        useEquippedWeaponAmmo = MyListRef.FindPropertyRelative("useEquippedWeaponAmmo");
        ammoCount = MyListRef.FindPropertyRelative("ammoCount");
        reduceAmmoWhilstActive = MyListRef.FindPropertyRelative("reduceAmmoWhilstActive");
        useReload = MyListRef.FindPropertyRelative("useReload");
        reloadDuration = MyListRef.FindPropertyRelative("reloadDuration");
        reloadRestrictAbilityActivationDuration = MyListRef.FindPropertyRelative("reloadRestrictAbilityActivationDuration");
        clipSize = MyListRef.FindPropertyRelative("clipSize");
        autoReloadWhenRequired = MyListRef.FindPropertyRelative("autoReloadWhenRequired");
        reloadFillClip = MyListRef.FindPropertyRelative("reloadFillClip"); 
        reloadFillClipRepeatGraphic = MyListRef.FindPropertyRelative("reloadFillClipRepeatGraphic");

        bounceMode = MyListRef.FindPropertyRelative("bounceMode");
        bounceAmount = MyListRef.FindPropertyRelative("bounceAmount");
        startBounceTagRequired = MyListRef.FindPropertyRelative("startBounceTagRequired");
        startBounceRequiredTags = MyListRef.FindPropertyRelative("startBounceRequiredTags");
        bounceTarget = MyListRef.FindPropertyRelative("bounceTarget");
        bounceTag = MyListRef.FindPropertyRelative("bounceTag");
        bounceRange = MyListRef.FindPropertyRelative("bounceRange");
        bounceOnCaster = MyListRef.FindPropertyRelative("bounceOnCaster");
        enableRandomBounce = MyListRef.FindPropertyRelative("enableRandomBounce");
        bouncePositionOffset = MyListRef.FindPropertyRelative("bouncePositionOffset");
        bouncePositionForwardOffset = MyListRef.FindPropertyRelative("bouncePositionForwardOffset");
        bouncePositionRightOffset = MyListRef.FindPropertyRelative("bouncePositionRightOffset");


        useGraphicRadius = MyListRef.FindPropertyRelative("useGraphicRadius");
        colliderRadius = MyListRef.FindPropertyRelative("colliderRadius");
        applyColliderSettingsToParent = MyListRef.FindPropertyRelative("applyColliderSettingsToParent");
        applyColliderSettingsToChildren = MyListRef.FindPropertyRelative("applyColliderSettingsToChildren");




        meleeKeepRotatingToSelectedTarget = MyListRef.FindPropertyRelative("meleeKeepRotatingToSelectedTarget");
        rotateToSelectedTarget = MyListRef.FindPropertyRelative("rotateToSelectedTarget");
        rotateToCameraCenter = MyListRef.FindPropertyRelative("rotateToCameraCenter");
        hitsStopMeleeAttack = MyListRef.FindPropertyRelative("hitsStopMeleeAttack");

        colliderOffset = MyListRef.FindPropertyRelative("colliderOffset");
        useDestroySplashEffect = MyListRef.FindPropertyRelative("useDestroySplashEffect");
        destroySplashRadius = MyListRef.FindPropertyRelative("destroySplashRadius");
        destroySplashExplosion = MyListRef.FindPropertyRelative("destroySplashExplosion");
        destroySplashExplosionPower = MyListRef.FindPropertyRelative("destroySplashExplosionPower");
        destroySplashExplosionTagLimit = MyListRef.FindPropertyRelative("destroySplashExplosionTagLimit");
        destroySplashExplosionAffectTag = MyListRef.FindPropertyRelative("destroySplashExplosionAffectTag");
        destroySplashExplosionRadius = MyListRef.FindPropertyRelative("destroySplashExplosionRadius");
        destroySplashExplosionUplift = MyListRef.FindPropertyRelative("destroySplashExplosionUplift");

        modifyGameSpeedOnInitiation = MyListRef.FindPropertyRelative("modifyGameSpeedOnInitiation");
        modifyGameSpeedOnInitiationSpeedFactor = MyListRef.FindPropertyRelative("modifyGameSpeedOnInitiationSpeedFactor");
        modifyGameSpeedOnInitiationDelay = MyListRef.FindPropertyRelative("modifyGameSpeedOnInitiationDelay");
        modifyGameSpeedOnInitiationDuration = MyListRef.FindPropertyRelative("modifyGameSpeedOnInitiationDuration");

        modifyGameSpeedOnImpact = MyListRef.FindPropertyRelative("modifyGameSpeedOnImpact");
        modifyGameSpeedOnImpactSpeedFactor = MyListRef.FindPropertyRelative("modifyGameSpeedOnImpactSpeedFactor");
        modifyGameSpeedOnImpactDuration = MyListRef.FindPropertyRelative("modifyGameSpeedOnImpactDuration");

        shakeCameraOnImpact = MyListRef.FindPropertyRelative("shakeCameraOnImpact");
        shakeCameraOnImpactDuration = MyListRef.FindPropertyRelative("shakeCameraOnImpactDuration");
        shakeCameraOnImpactAmount = MyListRef.FindPropertyRelative("shakeCameraOnImpactAmount");
        shakeCameraOnImpactSpeed = MyListRef.FindPropertyRelative("shakeCameraOnImpactSpeed");

        attachToObjectOnImpact = MyListRef.FindPropertyRelative("attachToObjectOnImpact");
        attachToObjectProbabilityMinValue = MyListRef.FindPropertyRelative("attachToObjectProbabilityMinValue");
        attachToObjectProbabilityMaxValue = MyListRef.FindPropertyRelative("attachToObjectProbabilityMaxValue");
        attachToObjectStickOutFactor = MyListRef.FindPropertyRelative("attachToObjectStickOutFactor");
        attachToObjectNearestBone = MyListRef.FindPropertyRelative("attachToObjectNearestBone");

        useInitiatingAesthetics = MyListRef.FindPropertyRelative("useInitiatingAesthetics");
        initiatingAnimateOnEntity = MyListRef.FindPropertyRelative("initiatingAnimateOnEntity");
        initiatingAnimateOnScrollGraphic = MyListRef.FindPropertyRelative("initiatingAnimateOnScrollGraphic");
        initiatingAnimateOnWeapon = MyListRef.FindPropertyRelative("initiatingAnimateOnWeapon");
        defyGravityInitiating = MyListRef.FindPropertyRelative("defyGravityInitiating");
        defyGravityInitiatingDuration = MyListRef.FindPropertyRelative("defyGravityInitiatingDuration");
        defyGravityInitiatingRaiseEvent = MyListRef.FindPropertyRelative("defyGravityInitiatingRaiseEvent");
        initiatingAestheticsPositionOffset = MyListRef.FindPropertyRelative("initiatingAestheticsPositionOffset");
        initiatingAnimatorParameter = MyListRef.FindPropertyRelative("initiatingAnimatorParameter");
        initiatingParticle = MyListRef.FindPropertyRelative("initiatingGraphic");
        initiatingObject = MyListRef.FindPropertyRelative("initiatingSubGraphic");
        initiatingAestheticDuration = MyListRef.FindPropertyRelative("initiatingAestheticDuration");
        initiatingAestheticDelay = MyListRef.FindPropertyRelative("initiatingAestheticDelay");
        delayBetweenInitiatingAndProjectile = MyListRef.FindPropertyRelative("delayBetweenInitiatingAndProjectile");
        initiatingStartPosition = MyListRef.FindPropertyRelative("initiatingStartPosition");
        initiatingPositionOnObject = MyListRef.FindPropertyRelative("initiatingPositionOnObject");
        initiatingPositionOnTag = MyListRef.FindPropertyRelative("initiatingPositionOnTag");
        initiatingAnimatorParameterType = MyListRef.FindPropertyRelative("initiatingAnimatorParameterType");
        initiatingAnimatorOnValue = MyListRef.FindPropertyRelative("initiatingAnimatorOnValue");
        initiatingAnimatorOffValue = MyListRef.FindPropertyRelative("initiatingAnimatorOffValue");
        initiatingAnimatorDuration = MyListRef.FindPropertyRelative("initiatingAnimatorDuration");

        initiatingAnimationRunnerClip = MyListRef.FindPropertyRelative("initiatingAnimationRunnerClip");
        initiatingAnimationRunnerClipSpeed = MyListRef.FindPropertyRelative("initiatingAnimationRunnerClipSpeed");
        initiatingAnimationRunnerClipDelay = MyListRef.FindPropertyRelative("initiatingAnimationRunnerClipDelay");
        initiatingAnimationRunnerClipDuration = MyListRef.FindPropertyRelative("initiatingAnimationRunnerClipDuration");
        initiatingAnimationRunnerOnEntity = MyListRef.FindPropertyRelative("initiatingAnimationRunnerOnEntity");
        initiatingAnimationRunnerOnScrollGraphic = MyListRef.FindPropertyRelative("initiatingAnimationRunnerOnScrollGraphic");
        initiatingAnimationRunnerOnWeapon = MyListRef.FindPropertyRelative("initiatingAnimationRunnerOnWeapon");

        moveSelfWhenInitiating = MyListRef.FindPropertyRelative("moveSelfWhenInitiating");
        moveSelfInitiatingOffset = MyListRef.FindPropertyRelative("moveSelfInitiatingOffset");
        moveSelfInitiatingForwardOffset = MyListRef.FindPropertyRelative("moveSelfInitiatingForwardOffset");
        moveSelfInitiatingRightOffset = MyListRef.FindPropertyRelative("moveSelfInitiatingRightOffset");
        moveSelfInitiatingDelay = MyListRef.FindPropertyRelative("moveSelfInitiatingDelay");
        moveSelfInitiatingDuration = MyListRef.FindPropertyRelative("moveSelfInitiatingDuration");

        moveSelfToTargetWhenInitiating = MyListRef.FindPropertyRelative("moveSelfToTargetWhenInitiating");
        moveSelfToTargetInitiatingDelay = MyListRef.FindPropertyRelative("moveSelfToTargetInitiatingDelay");
        moveSelfToTargetInitiatingDuration = MyListRef.FindPropertyRelative("moveSelfToTargetInitiatingDuration");
        moveSelfToTargetInitiatingStopDistance = MyListRef.FindPropertyRelative("moveSelfToTargetInitiatingStopDistance");
        moveSelfToTargetInitiatingOffset = MyListRef.FindPropertyRelative("moveSelfToTargetInitiatingOffset");
        moveSelfToTargetInitiatingForwardOffset = MyListRef.FindPropertyRelative("moveSelfToTargetInitiatingForwardOffset");
        moveSelfToTargetInitiatingRightOffset = MyListRef.FindPropertyRelative("moveSelfToTargetInitiatingRightOffset");

        initiatingAestheticsPositionForwardOffset = MyListRef.FindPropertyRelative("initiatingAestheticsPositionForwardOffset");
        initiatingAestheticsPositionRightOffset = MyListRef.FindPropertyRelative("initiatingAestheticsPositionRightOffset");
        preparingAestheticsPositionForwardOffset = MyListRef.FindPropertyRelative("preparingAestheticsPositionForwardOffset");
        preparingAestheticsPositionRightOffset = MyListRef.FindPropertyRelative("preparingAestheticsPositionRightOffset");
        scrollAbilityAestheticsPositionForwardOffset = MyListRef.FindPropertyRelative("scrollAbilityAestheticsPositionForwardOffset");
        scrollAbilityAestheticsPositionRightOffset = MyListRef.FindPropertyRelative("scrollAbilityAestheticsPositionRightOffset");
        reloadAbilityAestheticsPositionForwardOffset = MyListRef.FindPropertyRelative("reloadAbilityAestheticsPositionForwardOffset");
        reloadAbilityAestheticsPositionRightOffset = MyListRef.FindPropertyRelative("reloadAbilityAestheticsPositionRightOffset");
        selectedTargetForwardOffset = MyListRef.FindPropertyRelative("selectedTargetForwardOffset");
        selectedTargetRightOffset = MyListRef.FindPropertyRelative("selectedTargetRightOffset");
        abilityCanMiss = MyListRef.FindPropertyRelative("abilityCanMiss");

        useAbilityEndAesthetics = MyListRef.FindPropertyRelative("useAbilityEndAesthetics");
        abEndParticle = MyListRef.FindPropertyRelative("abilityEndGraphic");
        abEndObject = MyListRef.FindPropertyRelative("abilityEndSubGraphic");
        abEndAestheticDuration = MyListRef.FindPropertyRelative("abEndAestheticDuration");

        showPrepareTimeOnGUI = MyListRef.FindPropertyRelative("showPrepareTimeOnGUI");
        defyGravityPreparing = MyListRef.FindPropertyRelative("defyGravityPreparing");
        defyGravityPreparingDuration = MyListRef.FindPropertyRelative("defyGravityPreparingDuration");
        defyGravityPreparingRaiseEvent = MyListRef.FindPropertyRelative("defyGravityPreparingRaiseEvent");
        usePreparingAesthetics = MyListRef.FindPropertyRelative("usePreparingAesthetics");
        preparingAnimatorParameter = MyListRef.FindPropertyRelative("preparingAnimatorParameter");
        preparingAnimatorParameterType = MyListRef.FindPropertyRelative("preparingAnimatorParameterType");
        preparingAnimateOnEntity = MyListRef.FindPropertyRelative("preparingAnimateOnEntity");
        preparingAnimateOnScrollGraphic = MyListRef.FindPropertyRelative("preparingAnimateOnScrollGraphic");
        preparingAnimateOnWeapon = MyListRef.FindPropertyRelative("preparingAnimateOnWeapon");
        preparingAnimatorOnValue = MyListRef.FindPropertyRelative("preparingAnimatorOnValue");
        preparingAnimatorOffValue = MyListRef.FindPropertyRelative("preparingAnimatorOffValue");
        preparingParticle = MyListRef.FindPropertyRelative("preparingGraphic");
        preparingObject = MyListRef.FindPropertyRelative("preparingSubGraphic");
        preparingAestheticDurationUsePrepareTime = MyListRef.FindPropertyRelative("preparingAestheticDurationUsePrepareTime");
        preparingAestheticDuration = MyListRef.FindPropertyRelative("preparingAestheticDuration");
        preparingStartPosition = MyListRef.FindPropertyRelative("preparingStartPosition");
        preparingPositionOnObject = MyListRef.FindPropertyRelative("preparingPositionOnObject");
        preparingPositionOnTag = MyListRef.FindPropertyRelative("preparingPositionOnTag");

        moveSelfToTargetWhenPreparing = MyListRef.FindPropertyRelative("moveSelfToTargetWhenPreparing");
        moveSelfToTargetPreparingDelay = MyListRef.FindPropertyRelative("moveSelfToTargetPreparingDelay");
        moveSelfToTargetPreparingDuration = MyListRef.FindPropertyRelative("moveSelfToTargetPreparingDuration");
        moveSelfToTargetPreparingStopDistance = MyListRef.FindPropertyRelative("moveSelfToTargetPreparingStopDistance");
        moveSelfToTargetActivatePreparingAnimationOnlyWhenMoving = MyListRef.FindPropertyRelative("moveSelfToTargetActivatePreparingAnimationOnlyWhenMoving");
        moveSelfToTargetPreparingOffset = MyListRef.FindPropertyRelative("moveSelfToTargetPreparingOffset");
        moveSelfToTargetPreparingForwardOffset = MyListRef.FindPropertyRelative("moveSelfToTargetPreparingForwardOffset");
        moveSelfToTargetPreparingRightOffset = MyListRef.FindPropertyRelative("moveSelfToTargetPreparingRightOffset");

        preparingAnimationRunnerClip = MyListRef.FindPropertyRelative("preparingAnimationRunnerClip");
        preparingAnimationRunnerClipSpeed = MyListRef.FindPropertyRelative("preparingAnimationRunnerClipSpeed");
        preparingAnimationRunnerClipDelay = MyListRef.FindPropertyRelative("preparingAnimationRunnerClipDelay");        
        preparingAnimationRunnerOnEntity = MyListRef.FindPropertyRelative("preparingAnimationRunnerOnEntity");
        preparingAnimationRunnerOnScrollGraphic = MyListRef.FindPropertyRelative("preparingAnimationRunnerOnScrollGraphic");
        preparingAnimationRunnerOnWeapon = MyListRef.FindPropertyRelative("preparingAnimationRunnerOnWeapon");


        moveSelfWhenPreparing = MyListRef.FindPropertyRelative("moveSelfWhenPreparing");
        moveSelfPreparingOffset = MyListRef.FindPropertyRelative("moveSelfPreparingOffset");
        moveSelfPreparingForwardOffset = MyListRef.FindPropertyRelative("moveSelfPreparingForwardOffset");
        moveSelfPreparingRightOffset = MyListRef.FindPropertyRelative("moveSelfPreparingRightOffset");
        moveSelfPreparingDelay = MyListRef.FindPropertyRelative("moveSelfPreparingDelay");
        moveSelfPreparingDuration = MyListRef.FindPropertyRelative("moveSelfPreparingDuration");

        colliderTimeDelay = MyListRef.FindPropertyRelative("colliderTimeDelay");
        colliderDelayTime = MyListRef.FindPropertyRelative("colliderDelayTime");
        colliderKeyPressDelay = MyListRef.FindPropertyRelative("colliderKeyPressDelay");
        colliderDelayInputType = MyListRef.FindPropertyRelative("colliderDelayInputType");
        colliderDelayButton = MyListRef.FindPropertyRelative("colliderDelayButton");
        colliderDelayKey = MyListRef.FindPropertyRelative("colliderDelayKey");
        ignoreActiveTerrain = MyListRef.FindPropertyRelative("ignoreActiveTerrain");
        overrideIgnoreAbilityCollision = MyListRef.FindPropertyRelative("overrideIgnoreAbilityCollision");
        overrideWeaponBlocking = MyListRef.FindPropertyRelative("overrideWeaponBlocking");
        overrideWeaponParrying = MyListRef.FindPropertyRelative("overrideWeaponParrying");
        reduceWeaponBlockDurability = MyListRef.FindPropertyRelative("reduceWeaponBlockDurability");
        activateAnimationFromHit = MyListRef.FindPropertyRelative("activateAnimationFromHit");
        activateSpecificHitAnimation = MyListRef.FindPropertyRelative("activateSpecificHitAnimation");
        hitAnimationToActivate = MyListRef.FindPropertyRelative("hitAnimationToActivate");

        spawnObject = MyListRef.FindPropertyRelative("spawnObject");
        spawningObject = MyListRef.FindPropertyRelative("spawningObject");
        spawnObjectOnDestroy = MyListRef.FindPropertyRelative("spawnObjectOnDestroy");
        spawnObjectOnCollide = MyListRef.FindPropertyRelative("spawnObjectOnCollide");

        useRange = MyListRef.FindPropertyRelative("useRange");
        selectedTargetRangeGreaterThan = MyListRef.FindPropertyRelative("selectedTargetRangeGreaterThan");
        selectedTargetRangeLessThan = MyListRef.FindPropertyRelative("selectedTargetRangeLessThan");
        selectedTargetOffset = MyListRef.FindPropertyRelative("selectedTargetOffset");
        addAbilityCollider = MyListRef.FindPropertyRelative("addAbilityCollider");
        abilityType = MyListRef.FindPropertyRelative("abilityType");
        rayCastRadius = MyListRef.FindPropertyRelative("rayCastRadius");
        raycastHitAmount = MyListRef.FindPropertyRelative("raycastHitAmount");
        raycastBlockable = MyListRef.FindPropertyRelative("raycastBlockable");
        raycastIgnoreTerrain = MyListRef.FindPropertyRelative("raycastIgnoreTerrain");
        abilityBeforeTarget = MyListRef.FindPropertyRelative("abilityBeforeTarget");
        loopTillTargetFound = MyListRef.FindPropertyRelative("loopTillTargetFound");
        abilityBeforeTargetIndicatorScaleToEffectRadius = MyListRef.FindPropertyRelative("abilityBeforeTargetIndicatorScaleToEffectRadius");

        rayCastLength = MyListRef.FindPropertyRelative("rayCastLength");
        maxActiveAtOnce = MyListRef.FindPropertyRelative("maxActiveAtOnce");
        limitActiveAtOnce = MyListRef.FindPropertyRelative("limitActiveAtOnce");
        abilityActivationRaiseEvent = MyListRef.FindPropertyRelative("abilityActivationRaiseEvent");
        abilityActivationCompleteRaiseEvent = MyListRef.FindPropertyRelative("abilityActivationCompleteRaiseEvent");
        abilityActivationCompleteEventType = MyListRef.FindPropertyRelative("abilityActivationCompleteEventType");
       
        loggingEnabled = MyListRef.FindPropertyRelative("loggingEnabled");
        stopMovementOnInitiate = MyListRef.FindPropertyRelative("stopMovementOnInitiate");
        stopMovementOnInitiateDuration = MyListRef.FindPropertyRelative("stopMovementOnInitiateDuration");
        stopMovementOnInitiateFreezePosition = MyListRef.FindPropertyRelative("stopMovementOnInitiateFreezePosition");
        stopMovementOnInitiateDisableComponents = MyListRef.FindPropertyRelative("stopMovementOnInitiateDisableComponents");
        stopMovementOnInitiateRaiseEvent = MyListRef.FindPropertyRelative("stopMovementOnInitiateRaiseEvent");

        stopMovementOnPreparing = MyListRef.FindPropertyRelative("stopMovementOnPreparing");
        stopMovementOnPreparingDuration = MyListRef.FindPropertyRelative("stopMovementOnPreparingDuration");
        stopMovementOnPreparingFreezePosition = MyListRef.FindPropertyRelative("stopMovementOnPreparingFreezePosition");
        stopMovementOnPreparingDisableComponents = MyListRef.FindPropertyRelative("stopMovementOnPreparingDisableComponents");
        stopMovementOnPreparingRaiseEvent = MyListRef.FindPropertyRelative("stopMovementOnPreparingRaiseEvent");


        mouseForwardLockX = MyListRef.FindPropertyRelative("mouseForwardLockX");
        mouseForwardLockY = MyListRef.FindPropertyRelative("mouseForwardLockY");
        mouseForwardLockZ = MyListRef.FindPropertyRelative("mouseForwardLockZ");
        mouseFrontOnly = MyListRef.FindPropertyRelative("mouseFrontOnly");

        travelNearestTagList = MyListRef.FindPropertyRelative("travelNearestTagList");
        travelNearestTagRange = MyListRef.FindPropertyRelative("travelNearestTagRange");
        travelNearestTagIgnoreOriginator = MyListRef.FindPropertyRelative("travelNearestTagIgnoreOriginator");
        travelNearestTagRandomiseSearch = MyListRef.FindPropertyRelative("travelNearestTagRandomiseSearch");
    }

    /// <summary>
    /// Determines if the ability can currently be displayed on the reorderable list (recursivly calls itself to check if all parent branches are set to show children)
    /// </summary>
    /// <param name="Ability">Ability to check if can be displayed</param>
    /// <returns>True if ability can be displayed, else false</returns>
    private bool CanDisplayAbilityOnList(ABC_Ability Ability) {

        if (Ability == null)
            return true;

        //If parent ability is 0 then this ability can be displayed as we are not dependant on a parent ability having show children setting set to true
        if (Ability.parentAbilityID == 0)
            return true;


        //If the parent ability is not 0 then this ability is a child of another ability in hiearchy
        //if parent ability show children setting is true then recursivley call this method again to check if the parent can also be displayed (will check parents parent etc etc)
        if (abilityCont.FindAbility(Ability.parentAbilityID) == null || abilityCont.FindAbility(Ability.parentAbilityID).showChildrenInInspector == true)
            return CanDisplayAbilityOnList(abilityCont.FindAbility(Ability.parentAbilityID));
        else
            return false; // can't be displayed as parent show children is set to false


    }

    //Creates the ability reorderable list
    private void CreateAbilityReorderableList(string searchfilter = "", int dropdownfilter = 0) {

        sideListFilterOptions.Clear();
        sideListFilterOptions.Add("All");
        sideListFilterOptions.Add("----- Weapons -----");
        sideListFilterOptions.AddRange(abilityCont.Weapons.Select(item => item.weaponName));
        sideListFilterOptions.Add("------ Groups ------");
        sideListFilterOptions.AddRange(abilityCont.AbilityGroups.Select(item => item.groupName));

        // determines if list can be moved around
        bool isDraggable = false;
        int count = 0;

        if (abilityCont.DraggableMode == true) {
            reorderableAbilitylist = abilityCont.Abilities.ToList();
            count = abilityCont.Abilities.Count;
            isDraggable = true;
        } else if (searchfilter == "*Scroll*") {

            reorderableAbilitylist = abilityCont.Abilities.Where(a => a.scrollAbility == true).ToList();
            //disable draggable functionality
            isDraggable = false;
            count = reorderableAbilitylist.Count;

        }
        else if (searchfilter.Contains("*Combo*")) {

            //Keysearch Mouse0*Combo 
            //Buttonsearch *Trigger1*Combo*

            string code = searchfilter.Split('*')[0].ToString();
          
            reorderableAbilitylist = abilityCont.Abilities.Where(a => a.abilityCombo == true && (a.keyInputType == InputType.Key && a.key.ToString() == code || a.keyInputType == InputType.Button && a.keyButton == searchfilter.Split('*')[1].ToString())).ToList();
            //disable draggable functionality
            isDraggable = false;
            count = reorderableAbilitylist.Count;

        } else if (searchfilter.Contains("*ActivationLinks*")) {

            //What abilities are setup to activate link the ability (ID) provided

            //ID*ActivationLinks* e.g 1*ActivationLinks*

            string code = searchfilter.Split('*')[0].ToString();
            ABC_Ability ability = abilityCont.Abilities.Where(a => a.abilityID.ToString() == code).FirstOrDefault();

            if (ability != null) {
                reorderableAbilitylist = abilityCont.Abilities.Where(a => a.activationLinkAbilityIDs.Contains(ability.abilityID)).ToList();
                //disable draggable functionality
                isDraggable = false;
                count = reorderableAbilitylist.Count;
            }

        }
        else if (searchfilter.Contains("*EffectLinks*")) {

            //What abilities are setup to apply effects of the ability (ID) provided 

            //ID*EffectLinks* e.g 1*EffectLinks*

            string code = searchfilter.Split('*')[0].ToString();
            ABC_Ability ability = abilityCont.Abilities.Where(a => a.abilityID.ToString() == code).FirstOrDefault();

            if (ability != null) {
                reorderableAbilitylist = abilityCont.Abilities.Where(a => a.effectLinkAbilityIDs.Contains(ability.abilityID)).ToList();
                //disable draggable functionality
                isDraggable = false;
                count = reorderableAbilitylist.Count;
            }

        } else if (searchfilter.Contains("*All*")) {

            reorderableAbilitylist = abilityCont.Abilities.ToList();
            count = abilityCont.Abilities.Count;

        }  else  if (searchfilter != "") {

            reorderableAbilitylist = abilityCont.Abilities.Where(a => a.name.Trim().ToUpper().Contains(searchfilter.Trim().ToUpper())).ToList();
            //disable draggable functionality
            isDraggable = false;
            count = reorderableAbilitylist.Count;

        }  else {

            reorderableAbilitylist = abilityCont.Abilities.Where(a => CanDisplayAbilityOnList(a) == true).ToList();
            count = abilityCont.Abilities.Count;
            //disable draggable functionality
            isDraggable = false;
        }

        //Remove those not in the dropdown filter 
        if (sideListFilterChoice == 0 || abilityCont.DraggableMode == true) {
            //remove none as ALL
        } else if (sideListFilterChoice > sideListFilterOptions.FindIndex(s => s == "----- Weapons -----") && sideListFilterChoice < sideListFilterOptions.FindIndex(s => s == "------ Groups ------")) {
            //Remove all abilities that the weapon does not enable
            reorderableAbilitylist.RemoveAll(i => abilityCont.Weapons.Where(w => w.weaponName == sideListFilterOptions[sideListFilterChoice]).FirstOrDefault().enableAbilityIDs.Contains(i.abilityID) == false);
            isDraggable = false;
        } else if (sideListFilterChoice > sideListFilterOptions.FindIndex(s => s == "------ Groups ------")) {
            //remove all abilities not linked to the group 
            reorderableAbilitylist.RemoveAll(i => i.allowAbilityGroupAssignment == false ||  i.assignedAbilityGroupIDs.Contains(abilityCont.AbilityGroups.Where(g => g.groupName == sideListFilterOptions[sideListFilterChoice]).FirstOrDefault().groupID)  == false);
            isDraggable = false;
        }

       
        reorderableListAbilities = new ReorderableList(reorderableAbilitylist,
                                                       typeof(ABC_Ability),
                                                       isDraggable, false, false, false);


        // name the header
        reorderableListAbilities.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Abilities:" + reorderableAbilitylist.Count);
        };


        // when we select any of the list then it will set the current ability to show the ability details ready to be changed
        reorderableListAbilities.onSelectCallback = (ReorderableList l) => {
            abilityCont.CurrentAbility = abilityCont.Abilities.IndexOf(reorderableAbilitylist[l.index]);

        };


        reorderableListAbilities.onReorderCallback = (ReorderableList l) => {

            //get current ability
            ABC_Ability movedElement = abilityCont.Abilities[currentAbilityIndex];
            //remove current ability
            abilityCont.Abilities.Remove(movedElement);
            //insert it back to l.index where the element was dragged to in the list
            abilityCont.Abilities.Insert(l.index, movedElement);

            EditorUtility.SetDirty(abilityCont);

        };


        //					 design of the reorderable list 
        reorderableListAbilities.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) => {
            if (index < AbilityCount) {
                ABC_Ability ability = reorderableAbilitylist[index];

                string name = "";

                //If the ability is a child to a hiearchy parent then show branched symbol
                if (ability.parentAbilityID != 0 && abilityCont.FindAbility(ability.parentAbilityID) != null && (abilityCont.FindAbility(ability.parentAbilityID).showChildrenInInspector == true || isDraggable == true)) {
                    name = BranchedSymbol.ToString() + " ";
                }

                //If the ability is a parent to any hiearchy abilities then show small or right arrow depending on if show children is true
                if (abilityCont.Abilities.Where(a => a.parentAbilityID == ability.abilityID).Count() > 0) {
                    if (ability.showChildrenInInspector == true) {
                        name += SmallDownArrowSymbol.ToString() + " ";
                    } else {
                        name += SmallRightArrowSymbol.ToString() + " ";
                    }
                }
             
   
                if (ability.enableExport == true) {
                    name += " " + DiamondSymbol.ToString() + " ";
                }


                name = name + ability.name;

                rect.y += 2;


                EditorGUI.PrefixLabel(
                    new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight),
                    1, new GUIContent(name));
            }
        };


    }




    //  ****************************************** Setup Re-Ordablelists and define abilityController************************************************************

    void OnFocus() {

        // get new target 
        GameObject sel = Selection.activeGameObject;

        abilitySearchString = "";
        previousAbilitySearchString = ""; 

        // get ABC from current target 
        if (sel != null && sel.GetComponent<ABC_Controller>() != null) {
            abilityCont = sel.GetComponent<ABC_Controller>();
            GUIContent titleContent = new GUIContent(sel.gameObject.name + "'s ABC Ability Manager");
            GetWindow<ABC_ControllerAbility_EditorWindow>().titleContent = titleContent;
        }

        //If we have controller then setup abilities 
        if (abilityCont != null) {


            //Retrieve the main serialized object. This is the main property which is updated to retrieve current state, fields changed and then modifications applied back to the real object
            GetTarget = new SerializedObject(abilityCont);
            meAbilityList = GetTarget.FindProperty("Abilities"); // Find the List in our script and create a refrence of it 

            CreateAbilityReorderableList();

          
        }



    }




    void OnGUI() {


        if (abilityCont != null) {

            #region setup
            // keep up to date with count
            AbilityCount = meAbilityList.arraySize;

            //If new ability has been picked or an abilityID property has not been retrieved yet then populate properties for the current selected ability 
            if (this.currentAbilityIndex != abilityCont.CurrentAbility || abilityID == null) {
                this.GetProperties(abilityCont.CurrentAbility);      
            }


            // formats for UI
            GUI.skin.button.wordWrap = true;
            GUI.skin.label.wordWrap = true;
            EditorStyles.textField.wordWrap = true;
            
            EditorGUIUtility.labelWidth = 110;
            EditorGUIUtility.fieldWidth = 35;
 
            EditorGUILayout.Space();

            #endregion

        


            #region Body
            EditorGUILayout.BeginHorizontal();



            #region Left Section

            if (abilityCont.showAbilityListInWindow == true) {

                if (EditorGUIUtility.isProSkin)
                {
                    GUI.color = inspectorSectionBoxProColor;
                }
                else
                {
                    GUI.color = inspectorSectionBoxColor;
                }


                EditorGUILayout.BeginVertical(GUILayout.Width(abilityListWidth));

                GUI.color = Color.white;

                #region Logo

                if (abilityCont.Abilities.Count() > 0) {

                    Texture2D iconImage = null;
                        
                    if (abilityCont.Abilities[currentAbilityIndex].iconImage != null)
                        iconImage = abilityCont.Abilities[currentAbilityIndex].iconImage.Texture2D;

                    if (iconImage != null) {
                        EditorGUILayout.BeginVertical();
                        GUILayout.Label(iconImage, GUILayout.MinWidth(abilityInfoWidth - 20), GUILayout.Height(150));
                        EditorGUILayout.EndVertical();
                    }
                    else {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(Resources.Load("ABC-EditorIcons/DiceLockLogo", typeof(Texture2D)) as Texture2D, GUILayout.MinWidth(abilityInfoWidth - 20), GUILayout.Height(150));
                        EditorGUILayout.EndHorizontal();
                    }
                }




                #endregion

             

                #region Ability Select List



                if (GUILayout.Button(new GUIContent((abilityCont.DraggableMode) ? " Disable Drag Mode" : " Enable Drag Mode", SortIcon, " Enable/Disable Drag Mode" ))) {
                    if (abilityCont.DraggableMode == false) {
                        abilityCont.DraggableMode = true;
                    } else {
                        abilityCont.DraggableMode = false;
                    }

                    EditorUtility.SetDirty(abilityCont);
                    CreateAbilityReorderableList();
                }

                sideListFilterChoice = EditorGUILayout.Popup("", sideListFilterChoice, sideListFilterOptions.ToArray());

                //Only recreate list if a new search string has been entered
                if (sideListFilterChoice != sideListPreviousFilterChoice) {
                    sideListPreviousFilterChoice = sideListFilterChoice;
                    CreateAbilityReorderableList(abilitySearchString, sideListFilterChoice);
                }


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

                GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));


                abilitySearchString = GUILayout.TextField(abilitySearchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
          

                //If filter has been cleared then get all abilities
                if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton"))) {
                    // Remove focus if cleared
                    abilitySearchString = "";
                    previousAbilitySearchString = "";
                    CreateAbilityReorderableList();
                    GUI.FocusControl(null);
                }

                //Only recreate list if a new search string has been entered
                if (abilitySearchString != previousAbilitySearchString) {
                    previousAbilitySearchString = abilitySearchString;
                    CreateAbilityReorderableList(abilitySearchString, sideListFilterChoice);
                }



     
                GUILayout.EndHorizontal();

                listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos,
                                                                     false,
                                                                     false);



                    reorderableListAbilities.DoLayoutList();
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                #endregion

                #region Selected Ability Controls

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

                    if (GUILayout.Button(new GUIContent(" Add New Ability", AddIcon, "Add New Ability"))) {

                    ABC_Ability ability = new ABC_Ability();
                    ability.abilityID = ABC_Utilities.GenerateUniqueID();
                    ability.name = "New Ability";
                    ability.affectLayer = LayerMask.NameToLayer("Everything");


                    abilityCont.Abilities.Add(ability);


                    EditorUtility.SetDirty(abilityCont);

                    // recalculate list
                    CreateAbilityReorderableList(previousAbilitySearchString, sideListPreviousFilterChoice);
                    abilityCont.CurrentAbility = abilityCont.Abilities.IndexOf(reorderableAbilitylist[reorderableAbilitylist.Count()-1]);
                    this.currentAbilityIndex = abilityCont.CurrentAbility;

                }


                if (GUILayout.Button(new GUIContent(" Copy Ability     " + buttonSpace.ToString(), CopyIcon, "Copy Selected Ability"))) {


                    ABC_Ability clone = abilityCont.Abilities[currentAbilityIndex];

                    ABC_Ability newAb = new ABC_Ability();

                    JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(clone), newAb);

                    // change name as we can't have dupes
                    newAb.name = "Copy Of " + newAb.name;


                    // once we have the latest ID then we plus 1 for a unique number 
                    newAb.abilityID = ABC_Utilities.GenerateUniqueID();


                    abilityCont.Abilities.Add(newAb);


                    EditorUtility.SetDirty(abilityCont);

                    // recalculate list
                    CreateAbilityReorderableList(previousAbilitySearchString, sideListPreviousFilterChoice);


                }

                //Delete Ability 
                if (GUILayout.Button(new GUIContent(" Delete Ability   " + buttonSpace.ToString(), RemoveIcon, "Delete Selected Ability")) && EditorUtility.DisplayDialog("Delete Ability?", "Are you sure you want to delete " + meAbilityList.GetArrayElementAtIndex(currentAbilityIndex).FindPropertyRelative("name").stringValue, "Yes", "No")) {

                        abilityCont.CurrentAbility = 0;
                        abilityCont.Abilities.RemoveAt(currentAbilityIndex);


                    EditorUtility.SetDirty(abilityCont);

                    // recalculate list
                    CreateAbilityReorderableList(previousAbilitySearchString, sideListPreviousFilterChoice);

                }

                //Export abilities
                if (GUILayout.Button(new GUIContent(" Export Abilities   " + buttonSpace.ToString(), ExportIcon, "Export Abilities"))) {


                    abilityCont.ExportAbilities();

                }


                

                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
                #endregion                

                EditorGUILayout.EndVertical();

          

            }
            #endregion

            InspectorBoldVerticleLine();


            #region Right Section


            EditorGUILayout.BeginVertical();

            #region Right Section - Top Bar

            toolbarABCSelection = GUILayout.Toolbar(toolbarABCSelection, toolbarABC);

            #endregion

            EditorGUILayout.BeginHorizontal();

           
           
            #region Right Section - Main 


            EditorGUILayout.BeginVertical();


            // if array size is bigger then we need to update
            if (AbilityCount != meAbilityList.arraySize) {
                while (AbilityCount > meAbilityList.arraySize) {
                    meAbilityList.InsertArrayElementAtIndex(meAbilityList.arraySize);
                }
                while (AbilityCount < meAbilityList.arraySize) {
                    meAbilityList.DeleteArrayElementAtIndex(meAbilityList.arraySize - 1);
                }
            }



            if (AbilityCount > 0 && currentAbilityIndex < AbilityCount) {

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


                switch ((int)toolbarABCSelection) {
                    case 0:

                        EditorGUILayout.BeginHorizontal();

              

                        #region Controls/ability info
                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(abilityInfoWidth));


                        InspectorToolbarAndAbilityInfo(ref generalSettingsToolbarSelection, ref generalSettingsToolbar);

                        EditorGUILayout.EndVertical();

                        #endregion


                        InspectorBoldVerticleLine();



                        #region General Settings
                        editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                             false,
                                                                             false);

                        EditorGUILayout.BeginVertical();


                        #region Settings


                        switch ((int)generalSettingsToolbarSelection) {
                            case 0:


                                InspectorSectionHeader("Ability Settings");

                                #region SideBySide 


                                EditorGUILayout.BeginHorizontal();

                                #region Ability Description 


                                InspectorVerticalBox(true);

                                InspectorHelpBox("Enter the name, description of the ability, the Icon which can appear in game and the particle graphics of the ability.");

                                EditorGUILayout.PropertyField(name);
                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(iconImage);
                                EditorGUILayout.Space();
                                description.stringValue = EditorGUILayout.TextArea(description.stringValue, GUILayout.MaxHeight(80f));

                                EditorGUILayout.Space();
                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {
                                    EditorGUILayout.PropertyField(particle, new GUIContent("Main Graphic"));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(childParticle, new GUIContent("Sub Graphic"));

                                }
                                else {

                                    EditorGUILayout.HelpBox("RayCast Abilities do not require graphics.", MessageType.Warning);
                                }



                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion



                                #region Ability Type 


                                InspectorVerticalBox(true);



                                EditorGUILayout.Space();

                               
                                EditorGUILayout.BeginHorizontal();
                                EditorGUIUtility.labelWidth = 60;
                                EditorGUILayout.PropertyField(abilityType, new GUIContent("Type"), GUILayout.MaxWidth(150));
                                ResetLabelWidth();
                                EditorGUILayout.PropertyField(scrollAbility);
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("If enabled the Ability has to be equipped by going through a list and activated via one button.", false);

                                EditorGUIUtility.labelWidth = 275;
                                EditorGUILayout.PropertyField(castableDuringHitPrevention, new GUIContent("Can Activate During Hit Prevention"));
                                EditorGUILayout.PropertyField(hitPreventionWontInterruptActivation, new GUIContent("Hit Prevention Won't Interrupt Activation"));
                                ResetLabelWidth();

                                EditorGUILayout.Space();

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {

                                    // InspectorHelpBox("If On/Off then ability can be switched on and off with the button and can cast other abilities. If held the ability will stay active as long as the button is held down and no other abilities can be cast");

                                    EditorGUILayout.PropertyField(abilityToggle, GUILayout.MaxWidth(250));

                                    if ((string)abilityToggle.enumNames[abilityToggle.enumValueIndex] != "Off") {
                                        EditorGUIUtility.labelWidth = 230;
                                        EditorGUILayout.PropertyField(canCastWhenToggled, new GUIContent("Can Activate Abilities When Toggled"));
                                        EditorGUIUtility.labelWidth = 250;
                                        if (canCastWhenToggled.boolValue == false) {
                                            EditorGUILayout.PropertyField(repeatInitiatingAnimationWhilstToggled);
                                        }
                                        ResetLabelWidth();

                                    }

                                }


                                EditorGUILayout.PropertyField(LandOrAir, GUILayout.MaxWidth(250));

                                if ((string)LandOrAir.enumNames[LandOrAir.enumValueIndex] == "Air") {
                                    EditorGUIUtility.labelWidth = 190;
                                    EditorGUILayout.PropertyField(airAbilityDistanceFromGround, new GUIContent("Distance From Ground"), GUILayout.MaxWidth(230));
                                    ResetLabelWidth();
                                }
                                InspectorHelpBox("Determines the elevation condition for ability to activate.", false);

                                EditorGUIUtility.labelWidth = 120;
                                EditorGUILayout.PropertyField(forceActivation);

                                if (forceActivation.boolValue == true) {
                                    EditorGUIUtility.labelWidth = 185;
                                    EditorGUILayout.PropertyField(forceActivationInterruptCurrentActivation, new GUIContent("Interrupt Current Activation"));
                                    ResetLabelWidth();
                                }
                                

                                EditorGUILayout.Space();





                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region AllWay 

                                #region Ability Preparation 

                                InspectorVerticalBox();


                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(prepareTime, GUILayout.Width(480));
                                               
                                EditorGUILayout.PropertyField(showPrepareTimeOnGUI, new GUIContent("Show on GUI"));
                                EditorGUILayout.EndHorizontal();

                                EditorGUIUtility.labelWidth = 230;

                                EditorGUILayout.PropertyField(ignorePrepareTimeAdjustments);
                                InspectorHelpBox("If ticked then the prepare time can not be modified during play due to effects etc", false);
                                EditorGUILayout.BeginHorizontal();                      
                                EditorGUILayout.PropertyField(moveInteruptPreparation, new GUIContent("Moving Interrupts Preparing"));
                                if (moveInteruptPreparation.boolValue == true) {
                                    EditorGUIUtility.labelWidth = 130;
                                    EditorGUILayout.PropertyField(distanceInteruptPreparation, new GUIContent("Distance to Interrupt"), GUILayout.MaxWidth(160));
                                    EditorGUILayout.Space();
                                }
                                ResetLabelWidth();
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();


                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                #region SideBySide

                                EditorGUILayout.BeginHorizontal();

                                #region Ability Duration and Travel 

                                InspectorVerticalBox(true);


                                ResetLabelWidth();

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "RayCast") {
                                    EditorGUILayout.PropertyField(rayCastLength, new GUIContent("Raycast Length"));
                                    EditorGUILayout.PropertyField(affectLayer, GUILayout.MaxWidth(250));
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 105;
                                    EditorGUILayout.PropertyField(rayCastRadius, new GUIContent("Raycast Radius"), GUILayout.MaxWidth(170));
                                    EditorGUILayout.PropertyField(raycastHitAmount, new GUIContent("Raycast Max Hits"), GUILayout.MaxWidth(170));
                                    EditorGUILayout.EndHorizontal();
                                    ResetLabelWidth();
                                    EditorGUILayout.BeginHorizontal();                                  
                                    EditorGUILayout.PropertyField(raycastBlockable, new GUIContent("Raycast Blockable"), GUILayout.MaxWidth(180));
                                    EditorGUIUtility.labelWidth = 155;
                                    EditorGUILayout.PropertyField(raycastIgnoreTerrain, new GUIContent("Raycast Ignore Terrain"), GUILayout.MaxWidth(180));
                                    ResetLabelWidth();
                                    EditorGUILayout.EndHorizontal();
                                    InspectorHelpBox("How many entities can the raycast affect in one shot. If the max is 0 then the effect has no limit. If Blockable is true then non ABC objects can stop the raycast. If Ignore Terrain is ticked then the raycast will ignore terrain collisions");
                                    
                                }
                                else if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {

                                    InspectorHelpBox("Duration and Travel Speed of the ability. If Duration is set to 0 then the ability will have an unlimited duration.", false);
        
                                    EditorGUILayout.PropertyField(duration, GUILayout.MaxWidth(180));

                                   


                                    if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile"){
                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(travelSpeed);
                                    }





                                }


                                EditorGUILayout.EndVertical();

                                #endregion



                                #region Ability Mana and Recast 


                                InspectorVerticalBox(true);


                                //InspectorSectionHeader("Ability details 2", "Details determine preparation time, and if any ammo should be used. If the ability is a scroll then you can also determine reload");



                                EditorGUILayout.PropertyField(manaCost, GUILayout.MaxWidth(160));
                                EditorGUIUtility.labelWidth = 220;
                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) != "RayCast") {
                                    EditorGUILayout.PropertyField(reduceManaWhenActive, new GUIContent("Reduce Mana Whilst Active"));
                                }
                                ResetLabelWidth();

                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(abilityRecast, new GUIContent("Cooldown"), GUILayout.MaxWidth(160));
                                EditorGUIUtility.labelWidth = 220;
                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) != "RayCast") {
                                    EditorGUILayout.PropertyField(startRecastAfterAbilityEnd, new GUIContent("Start Cooldown After Ability Ends"));
                                }
                                ResetLabelWidth();







                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region AllWay 

                                #region Ability Ammo 

                                InspectorVerticalBox();



                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(UseAmmo);
                                if (UseAmmo.boolValue == true) {
                                    EditorGUIUtility.labelWidth = 210;
                                    EditorGUILayout.PropertyField(useEquippedWeaponAmmo);
                                    EditorGUIUtility.labelWidth = 190;
                                    if (((string)abilityType.enumNames[abilityType.enumValueIndex]) != "RayCast") {
                                        EditorGUILayout.PropertyField(reduceAmmoWhilstActive, new GUIContent("Reduce Ammo Whilst Active"));
                                    }
                                    ResetLabelWidth();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();

                                    if (useEquippedWeaponAmmo.boolValue == false) {
                                        EditorGUILayout.PropertyField(ammoCount, GUILayout.MaxWidth(180));
                                        InspectorHelpBox("Amount of times ability can be used. Reload fill will not waste clip but repeatedly add ammo to clip by 1 using the duration as an interval, graphic will also repeat each duration", false);

                                        if (scrollAbility.boolValue == true) {

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(useReload);

                                            if (useReload.boolValue == true) {

                                                EditorGUILayout.PropertyField(clipSize, GUILayout.MaxWidth(180));

                                                if (clipSize.intValue == 0) {
                                                    clipSize.intValue = 1;
                                                }

                                                EditorGUILayout.Space();
                                                EditorGUILayout.PropertyField(reloadDuration, GUILayout.MaxWidth(180));
                                                EditorGUILayout.Space();
                                                EditorGUILayout.EndHorizontal();

                                                EditorGUILayout.Space();

                                                EditorGUIUtility.labelWidth = 180;
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(autoReloadWhenRequired);
                                                ResetLabelWidth();
                                                EditorGUILayout.PropertyField(reloadFillClip);
                                                if (reloadFillClip.boolValue == true) {
                                                    EditorGUIUtility.labelWidth = 180;
                                                    EditorGUILayout.PropertyField(reloadFillClipRepeatGraphic, new GUIContent("Fill Clip Repeat Graphic"));
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUIUtility.labelWidth = 270;
                                                EditorGUILayout.PropertyField(reloadRestrictAbilityActivationDuration, GUILayout.MaxWidth(330));

                                                ResetLabelWidth();

                                            } else {
                                                EditorGUILayout.EndHorizontal();
                                            }


                                            EditorGUILayout.Space();


                                        }
                                    }

                                } else {
                                    EditorGUILayout.EndHorizontal();

                                }

                      
                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                InspectorSectionHeader("Group Assignment");

                                #region AllWay 

                                #region Group Assignment 

                                InspectorVerticalBox();
           

                                EditorGUIUtility.labelWidth = 170;

                                EditorGUILayout.PropertyField(allowAbilityGroupAssignment, new GUIContent("Allow Group Assignment"));
                                ResetLabelWidth();

                                InspectorHelpBox("Assign this ability to a group. Grouped abilities can be set to become enabled all at the same time.", false);
                                if (abilityCont.AbilityGroups.Count > 0) {

                                    if (allowAbilityGroupAssignment.boolValue == true) {
                                        InspectorAbilityGroupListBox("Assign To Group", assignedAbilityGroupIDs, abilityGroupListChoice);
                                    }
                                } else {
                                    EditorGUILayout.HelpBox("Unable to assign abilities as no groups have been setup. Please add a new group in the ABC Settings. ", MessageType.Warning);
                                }


                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion





                                break;


                            case 1:

                                InspectorSectionHeader("Key Press Settings");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Key Press Settings

                                InspectorVerticalBox(true);


                                InspectorHelpBox("Setup what key initates the Ability. If set as a 'Scroll Ability' then it will only initate if scrolled to and the scroll key is pressed. Scroll key bindings are setup in Settings.");

                                if (scrollAbility.boolValue == false) {

                                    EditorGUILayout.PropertyField(triggerType, new GUIContent("Trigger Type"), GUILayout.MaxWidth(250));
                                }

                                if (((string)triggerType.enumNames[triggerType.enumValueIndex]) == "Input" || scrollAbility.boolValue == true) {

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(onKeyPress);
                                    EditorGUILayout.PropertyField(onKeyDown);
                                    EditorGUILayout.EndHorizontal();
                                }

                                EditorGUILayout.Space();

                                if (scrollAbility.boolValue == false) {

                                    if (((string)triggerType.enumNames[triggerType.enumValueIndex]) == "InputCombo") {

                                        InspectorHelpBox("Enter in the key combination that needs to be inputted to trigger the ability. The list order represents which order the inputs need to be pressed");

                                        InspectorListBox("Input Combination", inputCombo);
                                    } else {

                                        EditorGUILayout.PropertyField(keyInputType, new GUIContent("Input Type"), GUILayout.MaxWidth(250));

                                        if (((string)keyInputType.enumNames[keyInputType.enumValueIndex]) == "Key") {

                                            EditorGUILayout.PropertyField(key, GUILayout.MaxWidth(250));

                                        } else {

                                            EditorGUILayout.PropertyField(keyButton, GUILayout.MaxWidth(250));

                                        }

                                        EditorGUILayout.Space();
                                        InspectorHelpBox("If additional input is enabled then ability will require an additional trigger.", false);


                                        EditorGUILayout.PropertyField(requireAdditionalKeyInput, new GUIContent("Additional Input"));

                                        if (requireAdditionalKeyInput.boolValue == true) {

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(additionalOnKeyPress, new GUIContent("On Key Press"));
                                            EditorGUILayout.PropertyField(additionalOnKeyDown, new GUIContent("On Key Down"));
                                            EditorGUILayout.EndHorizontal();

                                            EditorGUILayout.PropertyField(additionalKeyInputType, new GUIContent("Input Type"), GUILayout.MaxWidth(250));

                                            if (((string)additionalKeyInputType.enumNames[additionalKeyInputType.enumValueIndex]) == "Key") {

                                                EditorGUILayout.PropertyField(additionalKey, GUILayout.MaxWidth(250));

                                            } else {

                                                EditorGUILayout.PropertyField(additionalKeyButton, GUILayout.MaxWidth(250));

                                            }


                                            EditorGUILayout.Space();
                                        }
                                    }
                                    


                                }
                                else {


                                    InspectorHelpBox("By using a quick key the player can quickly switch to this ability by pressing the key. Swap Duration stops any ability activating whilst swapping abilities");

                                    EditorGUILayout.PropertyField(scrollQuickInputType, new GUIContent("Input Type"), GUILayout.MaxWidth(250));

                                

                                    if (((string)scrollQuickInputType.enumNames[scrollQuickInputType.enumValueIndex]) == "Key") {

                                        EditorGUILayout.PropertyField(scrollQuickKey, GUILayout.MaxWidth(250));

                                    }
                                    else {

                                        EditorGUILayout.PropertyField(scrollQuickButton, GUILayout.MaxWidth(250));

                                    }


                                    EditorGUILayout.PropertyField(scrollSwapDuration, new GUIContent("Swap Duration"), GUILayout.MaxWidth(180));

                               

                                    EditorGUIUtility.labelWidth = 150;
                                    EditorGUILayout.PropertyField(scrollSetUnsetRaiseEvent, new GUIContent("Set/Unset Raise Event"), GUILayout.MaxWidth(180));
                                    ResetLabelWidth();
                                }

                                EditorGUILayout.Space();
                       

                         

                                EditorGUILayout.EndVertical();

                                #endregion

                                #region wait for key to initialise 

                                InspectorVerticalBox(true);

                            
                                EditorGUIUtility.labelWidth = 190;
                                InspectorHelpBox("After preparation Ability will wait for the second key press (after delay) to initiate. No other abilities can be used during this time. ", false);
                                EditorGUILayout.PropertyField(waitForKeyBeforeInitiating, new GUIContent("Wait For Key to Initiate"));
                                ResetLabelWidth();
                                EditorGUILayout.Space();
                                if (waitForKeyBeforeInitiating.boolValue == true) {

                                    EditorGUILayout.PropertyField(waitBeforeInitiatingInputType, new GUIContent("Input Type"), GUILayout.MaxWidth(250));


                                    if (((string)waitBeforeInitiatingInputType.enumNames[waitBeforeInitiatingInputType.enumValueIndex]) == "Key") {

                                        EditorGUILayout.PropertyField(waitBeforeInitiatingKey, new GUIContent("Key"), GUILayout.MaxWidth(250));

                                    }
                                    else {

                                        EditorGUILayout.PropertyField(waitBeforeInitiatingButton, new GUIContent("Button"), GUILayout.MaxWidth(250));

                                    }


                                    EditorGUILayout.Space();
                                    EditorGUIUtility.labelWidth = 150;
                                    EditorGUILayout.PropertyField(waitForKeyBeforeInitiatingDelay, new GUIContent("Delay Till Recognised"), GUILayout.MaxWidth(200));
                                    ResetLabelWidth();


                                    EditorGUIUtility.labelWidth = 150;
                                  
                                    EditorGUILayout.PropertyField(waitForKeyAllowChangeOfTarget, new GUIContent("Allow Target Change"));
                                    ResetLabelWidth();



                                }


                               
                                InspectorHelpBox("Ability will only activate if Override Crosshair is present on screen. ", false);
                                EditorGUIUtility.labelWidth = 190;
                                EditorGUILayout.PropertyField(requireCrossHair, new GUIContent("Require Override Crosshair"));
                                EditorGUIUtility.labelWidth = 110;
                      
                                EditorGUILayout.Space();


                                InspectorHelpBox("Any abilities linked will always have the same key/button trigger as this ability", false);

                                EditorGUIUtility.labelWidth = 210;
                                EditorGUILayout.PropertyField(enableAbilityTriggerLinks);
                                ResetLabelWidth();

                                if (enableAbilityTriggerLinks.boolValue == true) {
                                    InspectorAbilityListBox("Trigger Links:", triggerLinkAbilityIDs);
                                }

                                EditorGUILayout.EndVertical();

                       

                                #endregion


                                EditorGUILayout.EndHorizontal();

                                #endregion

                                InspectorSectionHeader("Ability Combo Settings");

                                #region AllWay 

                                #region Ability Combo 

                                InspectorVerticalBox();


                                EditorGUIUtility.labelWidth = 190;
                                if (scrollAbility.boolValue == false && ((string)triggerType.enumNames[triggerType.enumValueIndex]) == "Input") {

                                    InspectorHelpBox("Combo Next Time is the duration in which the trigger key has to be pressed again to activate the next combo ability in the list with the same trigger. If key hasn't been pressed after this time it will reset the combo. If never reset other combos is disabled then using this ability will stop and reset all other combos.", false);

                                  

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(abilityCombo);
                                    EditorGUILayout.PropertyField(comboNextActivateTime, new GUIContent("Combo Next Time"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(comboHitRequired);
                                }
                                    EditorGUILayout.PropertyField(neverResetOtherCombos);
                                    EditorGUILayout.Space();

                                if (scrollAbility.boolValue == false && ((string)triggerType.enumNames[triggerType.enumValueIndex]) == "Input") {
                                    EditorGUILayout.EndHorizontal();
                                }

                                    ResetLabelWidth();




                               
 

                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                #region AllWay 

                                #region Ability swap and activation adjustment 

                                InspectorVerticalBox();



                                    InspectorHelpBox("Swap setting will randomly mix up abilities in the same trigger group also set to be randomly swapped. Interval adjustment will on activation temporarily modify the wait between ability activations, the adjustment is reset after the interval is over.", false);

                                    EditorGUIUtility.labelWidth = 210;

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(randomlySwapAbilityPosition);
                                    EditorGUIUtility.labelWidth = 290;
                                    EditorGUILayout.PropertyField(tempAbilityActivationIntervalAdjustment, new GUIContent("Activation Interval Temporary Adjustment"), GUILayout.MaxWidth(340));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();
                                    ResetLabelWidth();




                                
           

                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion


                                #region AllWay 

                                #region Enable After Event

                                InspectorVerticalBox();


                                //InspectorSectionHeader("Enable After Event", "If enable after event has been setup then the ability will be enabled after certain ABC events have occured.");

                                EditorGUIUtility.labelWidth = 130;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(enableDuration, new GUIContent("Enabled Duration"), GUILayout.MaxWidth(180));
                                EditorGUILayout.PropertyField(enableAfterEvent, new GUIContent("Enable After Event"));
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("If enabled duration is greater then 0 the ability when enabled will become disabled after the time entered.", false);



                                EditorGUIUtility.labelWidth = 130;
                            
                               
                                ResetLabelWidth();

                            
                                if (enableAfterEvent.boolValue == true) { 
                                EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(400), GUILayout.MinHeight(minimumSectionHeight));
                                EditorGUILayout.Space();
                                InspectorAbilityListBox("Ability Activated:", enableAfterAbilityIDsActivated);
                                EditorGUILayout.Space();
                                EditorGUILayout.Space();
                                EditorGUILayout.Space();
                                InspectorAbilityListBox("Ability Collision:", enableAfterAbilityIDsCollided);
                                EditorGUILayout.Space();

                                EditorGUILayout.EndHorizontal();
                                }

                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                InspectorSectionHeader("Ability Activation Links");

                                #region AllWay 

                                #region Ability Activation Link 

                                InspectorVerticalBox();

                                InspectorHelpBox("Any abilities linked will activate when this ability activates (ignoring triggers and activation restrictions)", false);

                                EditorGUIUtility.labelWidth = 210;
                                EditorGUILayout.PropertyField(enableAbilityActivationLinks);
                                ResetLabelWidth();

                                if (enableAbilityActivationLinks.boolValue == true) {
                                    InspectorAbilityListBox("Activate Links:", activationLinkAbilityIDs);
                                }

                                EditorGUILayout.Space();

                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion


                                break;

                            case 2:




                                #region effect settings


                                InspectorHelpBox("Add ability effects. Effects will run in order shown, toggle for more settings. ", false, true);

                                #region effect picker
                                GUI.color = new Color32(208, 212, 211, 255);
                                EditorGUILayout.BeginHorizontal(); 
                                GUI.color = Color.white;
                                effects = (enumEffects)EditorGUILayout.EnumPopup("", effects, GUILayout.Width(600));
                                //GUI.color = new Color32(208, 212, 211, 255);

                                if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(30))) {
                                    var stateIndex = stateEffects.arraySize;
                                    stateEffects.InsertArrayElementAtIndex(stateIndex);

                                    SerializedProperty meStateEffect = stateEffects.GetArrayElementAtIndex(stateIndex);
                                    SerializedProperty effectID = meStateEffect.FindPropertyRelative("effectID");
                                    SerializedProperty effectAbilityID = meStateEffect.FindPropertyRelative("effectAbilityID");
                                    SerializedProperty effectName = meStateEffect.FindPropertyRelative("effectName");


                                    if (effects.ToString() != "New") {
                                        // if not new then change value to the selected enum effect
                                        effectName.stringValue = (effects).ToString();
                                    }
                                    else {
                                        // new element so blank field ready for entry 
                                        effectName.stringValue = "";
                                    }

                                    // change ID to unique 
                                    int maxID = 0;

                                    // find the max ID in the list
                                    for (int n = 0; n < stateEffects.arraySize; n++) {

                                        int ID = stateEffects.GetArrayElementAtIndex(n).FindPropertyRelative("effectID").intValue;
                                        if (ID > maxID) {
                                            maxID = ID;
                                        }

                                    }

                                    // once we have the latest ID then we plus 1 for a unique number 
                                    effectID.intValue = maxID + 1;
                                    // link effect to ability
                                    effectAbilityID.intValue = abilityID.intValue;
                                    //set some default values
                                    meStateEffect.FindPropertyRelative("effectDuration").floatValue = 0.5f;
                                    meStateEffect.FindPropertyRelative("effectGraphicDuration").floatValue = 2f;


                                }

                                EditorGUILayout.EndHorizontal();
                                #endregion

                                #region AllWay 

                                #region Ability Effect Links

                                InspectorVerticalBox();

                                InspectorHelpBox("When this ability applies effects it will also apply effects of the abilities linked below.", false);
                                EditorGUIUtility.labelWidth = 190;
                                EditorGUILayout.PropertyField(enableAbilityEffectLinks);
                                ResetLabelWidth();

                                if (enableAbilityEffectLinks.boolValue == true) {
                                 
                                    InspectorAbilityListBox("Effect Links:", effectLinkAbilityIDs);
                                }


                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                #region Effects
                                GUI.color = Color.white;


                                for (int n = 0; n < stateEffects.arraySize; n++) {

                                    EditorGUILayout.Space();

                                    SerializedProperty meStateEffect = stateEffects.GetArrayElementAtIndex(n);
                                    SerializedProperty foldOut = meStateEffect.FindPropertyRelative("foldOut");
                                    SerializedProperty effectID = meStateEffect.FindPropertyRelative("effectID");
                                    SerializedProperty effectName = meStateEffect.FindPropertyRelative("effectName");
                                    
                                    SerializedProperty abilityName = meStateEffect.FindPropertyRelative("abilityName");
                                    // record the name of the ability 
                                    abilityName.stringValue = name.stringValue;

                                    SerializedProperty allowDuplicateEffectActivation = meStateEffect.FindPropertyRelative("allowDuplicateEffectActivation");
                                    SerializedProperty limitNoOfDuplicateEffectActivations = meStateEffect.FindPropertyRelative("limitNoOfDuplicateEffectActivations");
                                    SerializedProperty maxNoDuplicateEffectActivations = meStateEffect.FindPropertyRelative("maxNoDuplicateEffectActivations");
                                    SerializedProperty potency = meStateEffect.FindPropertyRelative("potency");
                                    SerializedProperty modifyPotencyUsingStats = meStateEffect.FindPropertyRelative("modifyPotencyUsingStats");
                                    SerializedProperty potencyStatModifications = meStateEffect.FindPropertyRelative("potencyStatModifications");
                                    SerializedProperty miscellaneousProperty = meStateEffect.FindPropertyRelative("miscellaneousProperty");
                                    SerializedProperty miscellaneousAltProperty = meStateEffect.FindPropertyRelative("miscellaneousAltProperty");
                                    SerializedProperty delay = meStateEffect.FindPropertyRelative("delay");
                                    SerializedProperty activateOn = meStateEffect.FindPropertyRelative("activateOn");
                                    SerializedProperty eventType = meStateEffect.FindPropertyRelative("eventType");
                                    SerializedProperty effectIgnoreTag = meStateEffect.FindPropertyRelative("effectIgnoreTag");
                                    SerializedProperty effectIgnoreTagCheckObject = meStateEffect.FindPropertyRelative("effectIgnoreTagCheckObject");
                                    SerializedProperty effectIgnoreTagCheckEntity = meStateEffect.FindPropertyRelative("effectIgnoreTagCheckEntity");


                                    SerializedProperty requireSplashToActivate = meStateEffect.FindPropertyRelative("requireSplashToActivate");

                                    SerializedProperty effectRequiredTag = meStateEffect.FindPropertyRelative("effectRequiredTag");
                                    SerializedProperty effectRequiredTagCheckObject = meStateEffect.FindPropertyRelative("effectRequiredTagCheckObject");
                                    SerializedProperty effectRequiredTagCheckEntity = meStateEffect.FindPropertyRelative("effectRequiredTagCheckEntity");


                                    SerializedProperty effectDuration = meStateEffect.FindPropertyRelative("effectDuration");
                                    SerializedProperty playEffect = meStateEffect.FindPropertyRelative("playEffect");
                                    SerializedProperty addToEffectText = meStateEffect.FindPropertyRelative("addToEffectText");
                                    SerializedProperty effectText = meStateEffect.FindPropertyRelative("effectText");
                                    SerializedProperty effectTextDisplayTime = meStateEffect.FindPropertyRelative("effectTextDuration");
                                    SerializedProperty effectTextColour = meStateEffect.FindPropertyRelative("effectTextColour");
                                    SerializedProperty addToEffectLog = meStateEffect.FindPropertyRelative("addToEffectLog");
                                    SerializedProperty effectLogText = meStateEffect.FindPropertyRelative("effectLogText");

               
                                    SerializedProperty effectRandomChance = meStateEffect.FindPropertyRelative("effectRandomChance");
                                    SerializedProperty effectRandomChanceProbabilityMin = meStateEffect.FindPropertyRelative("effectRandomChanceProbabilityMin");
                                    SerializedProperty effectRandomChanceProbabilityMax = meStateEffect.FindPropertyRelative("effectRandomChanceProbabilityMax");

                                    SerializedProperty hitStopsMovement = meStateEffect.FindPropertyRelative("hitStopsMovement");
                                    SerializedProperty hitStopsMovementRandomChance = meStateEffect.FindPropertyRelative("hitStopsMovementRandomChance");
                                    SerializedProperty hitStopsMovementProbabilityMin = meStateEffect.FindPropertyRelative("hitStopsMovementProbabilityMin");
                                    SerializedProperty hitStopsMovementProbabilityMax = meStateEffect.FindPropertyRelative("hitStopsMovementProbabilityMax");

                                    SerializedProperty hitPreventsCasting = meStateEffect.FindPropertyRelative("hitPreventsCasting");
                                    SerializedProperty hitPreventsCastingRandomChance = meStateEffect.FindPropertyRelative("hitPreventsCastingRandomChance");                                
                                    SerializedProperty hitPreventsCastingProbabilityMin = meStateEffect.FindPropertyRelative("hitPreventsCastingProbabilityMin");
                                    SerializedProperty hitPreventsCastingProbabilityMax = meStateEffect.FindPropertyRelative("hitPreventsCastingProbabilityMax");
                                    SerializedProperty hitInterruptsCasting = meStateEffect.FindPropertyRelative("hitInterruptsCasting");
                                    SerializedProperty hitInterruptsCastingRandomChance = meStateEffect.FindPropertyRelative("hitInterruptsCastingRandomChance");
                                    SerializedProperty hitInterruptsCastingProbabilityMin = meStateEffect.FindPropertyRelative("hitInterruptsCastingProbabilityMin");
                                    SerializedProperty hitInterruptsCastingProbabilityMax = meStateEffect.FindPropertyRelative("hitInterruptsCastingProbabilityMax");
                                    SerializedProperty repeatEffect = meStateEffect.FindPropertyRelative("repeatEffect");
                                    SerializedProperty repeatDuration = meStateEffect.FindPropertyRelative("repeatDuration");
                                    SerializedProperty repeatInterval = meStateEffect.FindPropertyRelative("repeatInterval");
                                    SerializedProperty repeatAesthetics = meStateEffect.FindPropertyRelative("repeatAesthetics");

                                    SerializedProperty showNonActivationReason = meStateEffect.FindPropertyRelative("showNonActivationReason");

                                    SerializedProperty enableRemoveEffect = meStateEffect.FindPropertyRelative("enableRemoveEffect");
                                    SerializedProperty dispellable = meStateEffect.FindPropertyRelative("dispellable");
                                    SerializedProperty removeEffectAddToEffectText = meStateEffect.FindPropertyRelative("removeEffectAddToEffectText");
                                    SerializedProperty removeEffectText = meStateEffect.FindPropertyRelative("removeEffectText");
                                    SerializedProperty removeEffectTextDisplayTime = meStateEffect.FindPropertyRelative("removeEffectTextDuration");
                                    SerializedProperty removeEffectTextColour = meStateEffect.FindPropertyRelative("removeEffectTextColour");
                                    SerializedProperty removeEffectAddToEffectLog = meStateEffect.FindPropertyRelative("removeEffectAddToEffectLog");
                                    SerializedProperty removeEffectLogText = meStateEffect.FindPropertyRelative("removeEffectLogText");

                                    SerializedProperty enableEffectActivationRange = meStateEffect.FindPropertyRelative("enableEffectActivationRange");
                                    SerializedProperty effectActivationRange = meStateEffect.FindPropertyRelative("effectActivationRange");
                                    SerializedProperty useProjectileForActivationRange = meStateEffect.FindPropertyRelative("useProjectileForActivationRange");


                                    SerializedProperty effectParticle = meStateEffect.FindPropertyRelative("effectGraphic");
                                    SerializedProperty effectChildParticle = meStateEffect.FindPropertyRelative("effectChildGraphic");
                                    SerializedProperty effectGraphicOffset = meStateEffect.FindPropertyRelative("effectGraphicOffset");
                                    SerializedProperty effectGraphicForwardOffset = meStateEffect.FindPropertyRelative("effectGraphicForwardOffset");
                                    SerializedProperty effectGraphicRightOffset = meStateEffect.FindPropertyRelative("effectGraphicRightOffset");

                                    SerializedProperty effectParticleDelay = meStateEffect.FindPropertyRelative("effectGraphicDelay");
                                    SerializedProperty effectParticleDuration = meStateEffect.FindPropertyRelative("effectGraphicDuration");
                                    SerializedProperty effectFollowTarget = meStateEffect.FindPropertyRelative("effectFollowTarget");
                                    SerializedProperty effectOnHitPosition = meStateEffect.FindPropertyRelative("effectOnHitPosition");


                                    InspectorSectionHeader(effectName.stringValue);
                                    InspectorVerticalBox();

                                    foldOut.boolValue = EditorGUILayout.Foldout(foldOut.boolValue, "More Settings");

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 60;
                                    EditorGUILayout.PropertyField(potency, GUILayout.Width(100));
                                    InspectorHorizontalSpace(5);
                                    EditorGUIUtility.labelWidth = 80;
                                    EditorGUILayout.PropertyField(delay, new GUIContent("Start Delay"), GUILayout.Width(120));
                                    InspectorHorizontalSpace(5);
                                    EditorGUIUtility.labelWidth = 65;
                                    EditorGUILayout.PropertyField(effectDuration, new GUIContent("Duration"), GUILayout.Width(100));
                                    InspectorHorizontalSpace(5);
                                    EditorGUIUtility.labelWidth = 130;
                                    ResetLabelWidth();


                                    if (GUILayout.Button("Up")) {
                                        stateEffects.MoveArrayElement(n, n - 1);
                                    }


                                    if (GUILayout.Button("Down")) {
                                        stateEffects.MoveArrayElement(n, n + 1);
                                    }


                                    GUI.color = Color.red;
                                    if (GUILayout.Button("X", GUILayout.Width(40))) {
                                        stateEffects.DeleteArrayElementAtIndex(n);
                                        // we can go to next loop as this no longer exists 
                                        continue;
                                    }

                                    GUI.color = Color.white;

                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.EndVertical();

                                    if (foldOut.boolValue == true) {




                                        #region AllWay 

                                        #region Effect Properties 

                                        InspectorVerticalBox();

                                        //InspectorSectionHeader("Effect Properties", "Loop this");

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("ID: " + effectID.intValue.ToString(), GUILayout.MaxWidth(100));
                                        EditorGUILayout.PropertyField(effectName, GUILayout.Width(300));
                                        EditorGUIUtility.labelWidth = 150;
                                        EditorGUILayout.PropertyField(enableRemoveEffect);
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(dispellable);
                                        EditorGUIUtility.labelWidth = 170;
                                        EditorGUILayout.PropertyField(requireSplashToActivate);
                                        EditorGUILayout.EndHorizontal();
                                        InspectorHelpBox("The name of the effect will be used later by ABC to determine what code to run. If duration of -1 is set then effect will have an infinite duration. If dispellable is true then effects can be removed instantly from other effects. If require splash to activate" +
                                            " is ticked then the effect will only be added if the effect was applied from an ability splashing");
                                        ResetLabelWidth();

                                 
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(activateOn, GUILayout.Width(250));
                                        EditorGUIUtility.labelWidth = 120;
                                        EditorGUILayout.PropertyField(miscellaneousProperty, new GUIContent("Misc Property"), GUILayout.Width(300));
                                        EditorGUILayout.EndHorizontal();
                                        ResetLabelWidth();
                                        //InspectorHelpBox("Entity - Will apply the effect on the entity hit. Originator - Will apply the effect on the originator. Both - Will apply the effect simultaneously on both the entity hit and the originator. Misc Property is A field which can be used to transfer further information about the effect. Can be used to pass ID's etc.");
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(eventType, GUILayout.Width(250));
                                        EditorGUIUtility.labelWidth = 120;
                                        EditorGUILayout.PropertyField(miscellaneousAltProperty, new GUIContent("Alt Misc Property"), GUILayout.Width(300));
                                        EditorGUILayout.EndHorizontal();
                                        InspectorHelpBox("Determines the event type when activating and deactiving the effect. Standard will run the normal code, RaiseEvent will invoke the event delegate and both will do everything.");



                                        EditorGUIUtility.labelWidth = 190;
                                        EditorGUILayout.PropertyField(effectRandomChance);

                                        EditorGUILayout.BeginHorizontal();
                                        ResetLabelWidth();

                                        if (effectRandomChance.boolValue == true) {
                                            EditorGUILayout.PropertyField(effectRandomChanceProbabilityMin, new GUIContent("Probability Min"));
                                            EditorGUILayout.PropertyField(effectRandomChanceProbabilityMax, new GUIContent("Probability Max"));
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 190;
                                        EditorGUILayout.PropertyField(hitStopsMovement);

                                        if (hitStopsMovement.boolValue == true) {
                                            EditorGUILayout.PropertyField(hitStopsMovementRandomChance, new GUIContent("Stop Movement Chance"));
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginHorizontal();
                                        ResetLabelWidth();

                                        if (hitStopsMovement.boolValue == true && hitStopsMovementRandomChance.boolValue == true) {
                                            EditorGUILayout.PropertyField(hitStopsMovementProbabilityMin, new GUIContent("Probability Min"));
                                            EditorGUILayout.PropertyField(hitStopsMovementProbabilityMax, new GUIContent("Probability Max"));
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 190;
                                        EditorGUILayout.PropertyField(hitPreventsCasting, new GUIContent("Hit Prevent Activation"));

                                        if (hitPreventsCasting.boolValue == true) {
                                            EditorGUILayout.PropertyField(hitPreventsCastingRandomChance, new GUIContent("Prevent Activation Chance"));
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginHorizontal();
                                        ResetLabelWidth();

                                        if (hitPreventsCasting.boolValue == true && hitPreventsCastingRandomChance.boolValue == true) {
                                            EditorGUILayout.PropertyField(hitPreventsCastingProbabilityMin, new GUIContent("Probability Min"));
                                            EditorGUILayout.PropertyField(hitPreventsCastingProbabilityMax, new GUIContent("Probability Max"));
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 190;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(hitInterruptsCasting, new GUIContent("Hit Interupt Activation"));

                                        if (hitInterruptsCasting.boolValue == true) {
                                            EditorGUILayout.PropertyField(hitInterruptsCastingRandomChance, new GUIContent("Interupt Activation Chance"));
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginHorizontal();

                                        ResetLabelWidth();
                                        if (hitInterruptsCasting.boolValue == true && hitInterruptsCastingRandomChance.boolValue == true) {
                                            EditorGUILayout.PropertyField(hitInterruptsCastingProbabilityMin, new GUIContent("Probability Min"));
                                            EditorGUILayout.PropertyField(hitInterruptsCastingProbabilityMax, new GUIContent("Probability Max"));
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();


                                        EditorGUILayout.EndVertical();

                                        #endregion

                                        #endregion

                                        #region AllWay 

                                        #region Effect Stat Modifications 

                                        InspectorVerticalBox();

                                        EditorGUIUtility.labelWidth = 190;
                                        EditorGUILayout.PropertyField(modifyPotencyUsingStats);
                                        InspectorHelpBox("Use stats from either the target entity and/or the originator to modify the potency value");
                                        ResetLabelWidth();

                                        if (modifyPotencyUsingStats.boolValue == true) {


                                            
                                            GUI.color = new Color32(208, 212, 211, 255);
                                            EditorGUILayout.BeginHorizontal();
                                            GUILayout.Box("Potency - Stat Modifications", new GUILayoutOption[] { GUILayout.MinWidth(minimumAllWaySectionWidth-50), GUILayout.Height(19) });
                                            GUI.color = Color.white;
                                            GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
                                            if (GUILayout.Button(new GUIContent(AddIcon), GUILayout.Width(30))) {
                                                potencyStatModifications.InsertArrayElementAtIndex(potencyStatModifications.arraySize);
                                                potencyStatModifications.GetArrayElementAtIndex(potencyStatModifications.arraySize - 1).FindPropertyRelative("statSource").enumValueIndex = 1;
                                                potencyStatModifications.GetArrayElementAtIndex(potencyStatModifications.arraySize - 1).FindPropertyRelative("percentageValue").floatValue = 100;
                                            }
                                            GUILayout.EndHorizontal();

                                            if (potencyStatModifications.arraySize > 0) {
                                                EditorGUILayout.BeginVertical("box");
                                                for (int i = 0; i < potencyStatModifications.arraySize; i++) {
                                                    SerializedProperty element = potencyStatModifications.GetArrayElementAtIndex(i);
                                                    EditorGUILayout.BeginHorizontal();
                                                    EditorGUILayout.PropertyField(element.FindPropertyRelative("arithmeticOperator"), new GUIContent(""), GUILayout.Width(65));
                                                    EditorGUILayout.PropertyField(element.FindPropertyRelative("percentageValue"), new GUIContent(""), GUILayout.Width(50));
                                                    EditorGUIUtility.labelWidth = 40;
                                                    EditorGUILayout.PropertyField(element.FindPropertyRelative("statSource"), new GUIContent("% of"), GUILayout.Width(150));
                                                    EditorGUILayout.PropertyField(element.FindPropertyRelative("statIntegrationType"), new GUIContent(""), GUILayout.Width(90)); 
                                                    EditorGUILayout.PropertyField(element.FindPropertyRelative("statName"), new GUIContent("Stat:"), GUILayout.Width(140));
                                                    ResetLabelWidth();
                                                    GUI.skin.button.normal.textColor = new Color(0, 0.45f, 1, 1);
                                                    if (GUILayout.Button(UpArrowSymbol.ToString())) {
                                                        potencyStatModifications.MoveArrayElement(i, i - 1);
                                                    }
                                                    if (GUILayout.Button(DownArrowSymbol.ToString())) {
                                                        potencyStatModifications.MoveArrayElement(i, i + 1);
                                                    }


                                                    GUI.skin.button.normal.textColor = Color.red;
                                                    if (GUILayout.Button("X", GUILayout.Width(40))) {
                                                        potencyStatModifications.DeleteArrayElementAtIndex(i);
                                                    }
                                                    GUI.color = Color.white;
                                                    GUILayout.EndHorizontal();

                                                }
                                                EditorGUILayout.EndVertical();

                                            }


                                        }



                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndVertical();

                                        #endregion

                                        #endregion

                                        #region SideBySide

                                        EditorGUILayout.BeginHorizontal();

                                        #region Effect Activation 

                                        InspectorVerticalBox(true);

                                        EditorGUIUtility.labelWidth = 225;

                                        EditorGUILayout.PropertyField(allowDuplicateEffectActivation);
                              
                                        if (allowDuplicateEffectActivation.boolValue == true) {
                                            EditorGUILayout.PropertyField(limitNoOfDuplicateEffectActivations, new GUIContent("Limit Duplicate Effect Activation"));
                                            if (limitNoOfDuplicateEffectActivations.boolValue == true) {
                                                EditorGUIUtility.labelWidth = 175;
                                                EditorGUILayout.PropertyField(maxNoDuplicateEffectActivations, new GUIContent("Max No Of Duplicate Effects"), GUILayout.Width(230));
                                            }
                                        }


                                        InspectorHelpBox("If true then the effect can be applied multiple times. If false then the effect can only be applied once per ABC user. Duplicate effect activation" +
                                            " can be limited so only a max number of duplicate effects can activate at one time");


                                        ResetLabelWidth();



                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) != "RayCast") {

                                            EditorGUIUtility.labelWidth = 150;

                                            EditorGUILayout.PropertyField(enableEffectActivationRange, new GUIContent("Activation Range"));

                                            if (enableEffectActivationRange.boolValue == true) {


                                                EditorGUILayout.PropertyField(useProjectileForActivationRange, new GUIContent("Use Projectiles Range"));


                                                if (useProjectileForActivationRange.boolValue == false) {
                                                    EditorGUILayout.PropertyField(effectActivationRange, GUILayout.Width(190));
                                                }
                                                ResetLabelWidth();


                                            }

                                            InspectorHelpBox("Activation range if enabled will only have the effect activate if the entity hit is still in the range set");

                                        }



                                        if (effectDuration.floatValue > 0) {
                                            EditorGUILayout.PropertyField(repeatEffect);

                                            if (repeatEffect.boolValue == true) {
                                                EditorGUILayout.PropertyField(repeatAesthetics);


                                                EditorGUILayout.PropertyField(repeatDuration, GUILayout.Width(190));
                                                EditorGUILayout.PropertyField(repeatInterval, GUILayout.Width(190));



                                            }

                                        }






                                        EditorGUILayout.EndVertical();

                                        #endregion



                                        #region Effect Graphic
                                        ResetLabelWidth();

                                        InspectorVerticalBox(true);



                                        EditorGUILayout.PropertyField(playEffect, new GUIContent("Effect Graphic"));
                                        if (playEffect.boolValue == true) { 
                                        EditorGUILayout.Space();

                                        EditorGUILayout.PropertyField(effectParticle, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));

                                        EditorGUILayout.PropertyField(effectChildParticle, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 90;
                                        EditorGUILayout.PropertyField(effectParticleDelay, new GUIContent("Delay"));
                                        EditorGUILayout.PropertyField(effectParticleDuration, new GUIContent("Duration"));
                                        EditorGUILayout.EndHorizontal();
                                        ResetLabelWidth();

                                        EditorGUILayout.Space();
                                        EditorGUIUtility.labelWidth = 170;
                                        EditorGUILayout.PropertyField(effectFollowTarget, new GUIContent("Follow Affected Object"));
                                        EditorGUILayout.PropertyField(effectOnHitPosition);

                                        EditorGUILayout.Space();
                                   
                                        EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));
                                        EditorGUILayout.PropertyField(effectGraphicOffset, new GUIContent(""), GUILayout.MaxWidth(300));

                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 100;
                                        EditorGUILayout.PropertyField(effectGraphicForwardOffset, new GUIContent("Forward Offset"));
                                        EditorGUILayout.PropertyField(effectGraphicRightOffset, new GUIContent("Right Offset"));
                                        EditorGUILayout.EndHorizontal();

                                        

                                        ResetLabelWidth();
                                        EditorGUILayout.Space();
                                    }
                                        


                                        EditorGUILayout.EndVertical();

                                        #endregion

                                        EditorGUILayout.EndHorizontal();

                                        #endregion

                                        #region SideBySide

                                        EditorGUILayout.BeginHorizontal();

                                        #region Graphic Log

                                        InspectorVerticalBox(true);

                                       

                                        EditorGUILayout.PropertyField(addToEffectText, new GUIContent("Graphic Log"));
                                        InspectorHelpBox("Placeholders: #Potency# - Potency Value, #Origin# - name of GameObject Ability was fired from, #Target# - Name of GameObject hit by Ability, #Ability# - Ability Name, #Effect# - Effect Name");


                                        if (addToEffectText.boolValue == true) {
                                            EditorGUILayout.PropertyField(effectText, new GUIContent(""));
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUIUtility.labelWidth = 170;
                                            EditorGUILayout.PropertyField(effectTextDisplayTime, new GUIContent("Graphic Log Duration"), GUILayout.Width(240));
                                            EditorGUIUtility.labelWidth = 60;
                                            EditorGUILayout.PropertyField(effectTextColour, new GUIContent("Colour"));
                                            EditorGUILayout.EndHorizontal();
                                            ResetLabelWidth();
                                        }

                                        EditorGUILayout.Space();
                                        EditorGUIUtility.labelWidth = 160;
                                        EditorGUILayout.PropertyField(removeEffectAddToEffectText, new GUIContent("Remove Graphic Log"));
                                        ResetLabelWidth();
                                        if (removeEffectAddToEffectText.boolValue == true) {
                                            EditorGUILayout.PropertyField(removeEffectText, new GUIContent(""));
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUIUtility.labelWidth = 170;
                                            EditorGUILayout.PropertyField(removeEffectTextDisplayTime, new GUIContent("Graphic Log Duration"), GUILayout.Width(240));
                                            EditorGUIUtility.labelWidth = 60;
                                            EditorGUILayout.PropertyField(removeEffectTextColour, new GUIContent("Colour"));
                                            EditorGUILayout.EndHorizontal();
                                        

                                            ResetLabelWidth();
                                        }

                                        EditorGUILayout.Space();


                                        EditorGUILayout.EndVertical();

                                        #endregion



                                        #region Effect Log
                                        ResetLabelWidth();

                                        EditorGUILayout.BeginVertical("Box", GUILayout.MaxWidth(position.width / 3), GUILayout.MinHeight(minimumSectionHeight));



                                        EditorGUIUtility.labelWidth = 195;
                                        EditorGUILayout.PropertyField(addToEffectLog, new GUIContent("Effect Log"));
                                        InspectorHelpBox("Set what is shown on the effect GUI log");

                                        if (addToEffectLog.boolValue == true) {
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(effectLogText, new GUIContent(""));
                                            EditorGUILayout.EndHorizontal();



                                            EditorGUILayout.Space();
                                        }

                                   
                                        EditorGUILayout.PropertyField(removeEffectAddToEffectLog, new GUIContent("Remove Effect Log"));

                                        if (removeEffectAddToEffectLog.boolValue == true) {
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(removeEffectLogText, new GUIContent(""));
                                            EditorGUILayout.EndHorizontal();


                                            EditorGUILayout.Space();
                                        }
                                        EditorGUILayout.PropertyField(showNonActivationReason);
                                        InspectorHelpBox("If true then the effect log will notify the user why the effect didn't activate (already exists, resisted etc)");

                                        ResetLabelWidth();




                                       

                                        #endregion
                                        EditorGUILayout.EndVertical();
                                        EditorGUILayout.EndHorizontal();

                                        #endregion

                                        #region SideBySide

                                        EditorGUILayout.BeginHorizontal();

                                        #region Effect Ignore Tag

                                        InspectorVerticalBox(true);

                                        InspectorHelpBox("This effect will not activate on the tags listed", false);

                                        EditorGUIUtility.labelWidth = 170;
                                        EditorGUILayout.PropertyField(effectIgnoreTagCheckEntity, new GUIContent("Check Tags On Entity"));
                                        EditorGUILayout.PropertyField(effectIgnoreTagCheckObject, new GUIContent("Check Tags On Object Hit"));
                                        ResetLabelWidth();
                                        InspectorListBox("Effect Ignore Tags", effectIgnoreTag);



                                        EditorGUILayout.EndVertical();

                                        #endregion



                                        #region Effect Required Tags
                                        ResetLabelWidth();

                                        InspectorVerticalBox(true);
                                        InspectorHelpBox("This effect will only activate on the tags listed", false);

                                        EditorGUIUtility.labelWidth = 170;
                                        EditorGUILayout.PropertyField(effectRequiredTagCheckEntity, new GUIContent("Check Tags On Entity"));
                                        EditorGUILayout.PropertyField(effectRequiredTagCheckObject, new GUIContent("Check Tags On Object Hit"));
                                        ResetLabelWidth();
                                        InspectorListBox("Effect Required Tags", effectRequiredTag);


                                        #endregion
                                        EditorGUILayout.EndVertical();
                                        EditorGUILayout.EndHorizontal();

                                        #endregion



                                    }


                                }

                                #endregion



                                EditorGUILayout.Space();



                                EditorGUILayout.Space();

                     
                       




                                EditorGUILayout.Space();
                                #endregion


                                break;
                        
                            case 3:

                                InspectorSectionHeader("Additional Settings");

                                #region AllWay 

                                #region Ability Spawn Objects / Limit Active 

                                InspectorVerticalBox();


                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || (string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(spawnObject);

                                    if (spawnObject.boolValue == true) { 
                                    EditorGUILayout.PropertyField(spawningObject);
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(spawnObjectOnDestroy, new GUIContent("Destroy Spawn"));
                                    EditorGUILayout.PropertyField(spawnObjectOnCollide, new GUIContent("Collide Spawn"));
                                    EditorGUILayout.EndHorizontal();
                            
                                    } else {
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    InspectorHelpBox("If enabled then this will spawn the provided objects when the ability is destroyed or collides");

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(limitActiveAtOnce, new GUIContent("Limit Active"));
                                    EditorGUILayout.Space();
                                    if (limitActiveAtOnce.boolValue == true) { 
                                    EditorGUILayout.PropertyField(maxActiveAtOnce, new GUIContent("Max Active"), GUILayout.MaxWidth(180));
                                    }
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();
                                    InspectorHelpBox("Will limit the amount of the abilities currently in play by destroying the oldest active ability when the limit is reached from a new one being created.");


                                }
                                else {

                                    EditorGUILayout.HelpBox("Raycast abilities can not spawn objects or limit active.", MessageType.Warning);
                                }


                                EditorGUIUtility.labelWidth = 120;
                                EditorGUILayout.PropertyField(loggingEnabled, new GUIContent("Logging Enabled"), GUILayout.MaxWidth(180));
                                InspectorHelpBox("If enabled then the ability states (Preparing, Activating, Cooldown etc) can write to any GUI Logs setup.");

                                EditorGUIUtility.labelWidth = 210;
                                EditorGUILayout.PropertyField(abilityActivationRaiseEvent, new GUIContent("Raise Ability Activation Event"), GUILayout.MaxWidth(240));
                                InspectorHelpBox("If enabled then on ability activation an event delegate will be raised to let linked scripts know");
                                ResetLabelWidth();

                                             
                                EditorGUIUtility.labelWidth = 280;
                                EditorGUILayout.PropertyField(abilityActivationCompleteRaiseEvent, new GUIContent("Raise Ability Activation Completed Event"), GUILayout.MaxWidth(290));
                                if (abilityActivationCompleteRaiseEvent.boolValue == true){
                                    EditorGUILayout.PropertyField(abilityActivationCompleteEventType, new GUIContent("Ability Activation Completed Event Type"), GUILayout.MaxWidth(390));
                                }
                                ResetLabelWidth();
                                InspectorHelpBox("If enabled then when ability activation is completed depending on the type selected (when ability initiates or is destroyed) an event delegate will be raised to let linked scripts know");

                          
                            

                                EditorGUILayout.EndVertical();

                                #endregion

                                #endregion

                                InspectorSectionHeader("Surrounding Objects");


                                #region AllWay

                                EditorGUILayout.BeginHorizontal();

                                #region SurroundingObjects 

                                InspectorVerticalBox();

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || (string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") {

                                    InspectorHelpBox("If enabled and setup then surrounding objects configured will travel alongside the ability, like telekensis.");

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 220;
                                    EditorGUILayout.PropertyField(includeSurroundingObject);
                                    if (includeSurroundingObject.boolValue == true) {
                                        EditorGUILayout.PropertyField(lockSurroundingObject, new GUIContent("Lock To Projectile"));
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    if (includeSurroundingObject.boolValue == true) {

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(minimumScatterRange, GUILayout.MaxWidth(270));
                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(maximumScatterRange, GUILayout.MaxWidth(270));
                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndHorizontal();


                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(sendObjectToProjectile, new GUIContent("Send Object To Projectile"));
                                        EditorGUILayout.Space();
                                        if (sendObjectToProjectile.boolValue == true) {
                                            EditorGUILayout.PropertyField(objectToProjectileDuration, new GUIContent("Object To Projectile Duration"), GUILayout.MaxWidth(270));
                                        }
                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndHorizontal();
                                        InspectorHelpBox("If Send Object To Projectile is ticked the object will move towards the object location. " +
                                                                     "If Object To Projectile Duration is 0 then the object will hover on the spot", false);
                                    }

                                    ResetLabelWidth();

                                  


                                }
                                else {


                                    EditorGUILayout.HelpBox("Raycast abilities can't include surrounding objects", MessageType.Warning);
                                }
                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region AllWay

                                EditorGUILayout.BeginHorizontal();

                                #region SurroundingObjects Collision & Collider

                                EditorGUILayout.BeginVertical("Box", GUILayout.MinWidth(minimumAllWaySectionWidth));

                                EditorGUILayout.Space();

                                if (includeSurroundingObject.boolValue == false) {
                                    EditorGUILayout.HelpBox("Surrounding Objects not enabled", MessageType.Warning);
                                } else { 


                                    if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || (string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") {


                                        EditorGUIUtility.labelWidth = 220;

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(applyColliderToSurroundingObjects, new GUIContent("Enable Collider"));
                                        if (applyColliderToSurroundingObjects.boolValue == true) {
                                            EditorGUILayout.PropertyField(applyColliderWhenProjectileReached, new GUIContent("Enable When Projectile Reached"));
                                        }
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(projectileAffectSurroundingObject, new GUIContent("Effect on Collision"));
                                        EditorGUILayout.PropertyField(destroySurroundingObject, new GUIContent("Destroy on Collision"));
                                        EditorGUILayout.EndHorizontal();


                                        ResetLabelWidth();

                                        EditorGUILayout.Space();



                                    }
                                    else {


                                        EditorGUILayout.HelpBox("Raycast abilities can include surrounding objects", MessageType.Warning);
                                    }
                                }
                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Surrounding Object Tag 

                                EditorGUILayout.BeginVertical("Box", GUILayout.MinWidth(minimumSideBySideSectionWidth), GUILayout.MinHeight(minimumSectionHeight));


                                //InspectorSectionHeader("Surrounding Object Tag", "Tag Options.");

                                if (includeSurroundingObject.boolValue == false) {
                                    EditorGUILayout.HelpBox("Surrounding Objects not enabled", MessageType.Warning);
                                }
                                else {

                                    if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || (string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") {

                                        EditorGUIUtility.labelWidth = 220;

                                        EditorGUILayout.PropertyField(surroundingObjectTags, new GUIContent("Surrounding Tags"));

                                        if (surroundingObjectTags.boolValue == true) {
                                            EditorGUILayout.PropertyField(surroundingObjectTagsRange, new GUIContent("Surrounding Range"), GUILayout.MaxWidth(280));
                                            EditorGUILayout.PropertyField(surroundingObjectTagRequired, new GUIContent("Surrounding Object Required"), GUILayout.MaxWidth(280));
                                            EditorGUILayout.PropertyField(surroundingObjectTagLimit, new GUIContent("Object Limit"));

                                            EditorGUILayout.Space();


                                            InspectorListBox("Affected Tags", surroundingObjectTagAffectTag);
                                        }




                                        ResetLabelWidth();

                                        EditorGUILayout.Space();



                                    }
                                    else {


                                        EditorGUILayout.HelpBox("Raycast abilities can include surrounding objects", MessageType.Warning);
                                    }
                                }



                                EditorGUILayout.EndVertical();

                                #endregion

                                #region Surronding Object Target

                                EditorGUILayout.BeginVertical("Box", GUILayout.MinWidth(minimumSideBySideSectionWidth), GUILayout.MinHeight(minimumSectionHeight));

                                if (includeSurroundingObject.boolValue == false) {
                                    EditorGUILayout.HelpBox("Surrounding Objects not enabled", MessageType.Warning);
                                }
                                else {

                                    //InspectorSectionHeader("SurroundingObjects Target", "Todo put this at bottom");

                                    if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || (string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") {

                                        EditorGUIUtility.labelWidth = 220;



                                        EditorGUILayout.PropertyField(surroundingObjectTarget, new GUIContent("Surrounding Target Select"));

                                        if (surroundingObjectTarget.boolValue == true) {
                                            EditorGUILayout.PropertyField(surroundingObjectTargetRange, new GUIContent("Surrounding Range"), GUILayout.MaxWidth(280));
                                            EditorGUILayout.PropertyField(surroundingObjectAuxiliarySoftTarget, new GUIContent("Auxiliary SoftTarget"));
                                            EditorGUILayout.PropertyField(surroundingObjectTargetRequired, new GUIContent("Surrounding Object Required"), GUILayout.MaxWidth(280));
                                            EditorGUILayout.PropertyField(surroundingObjectTargetRestrict, new GUIContent("Only Affect Tag"));

                                            if (surroundingObjectTargetRestrict.boolValue == true) {
                                                InspectorListBox("Only Affect Targets with the following Tags", surroundingObjectTargetAffectTag);
                                            }

                                        }




                                        EditorGUIUtility.labelWidth = 110;

                                        EditorGUILayout.Space();



                                    }
                                    else {


                                        EditorGUILayout.HelpBox("Raycast abilities can include surrounding objects", MessageType.Warning);
                                    }
                                }





                                EditorGUILayout.Space();





                                EditorGUILayout.EndVertical();

                                #endregion



                                EditorGUILayout.EndHorizontal();

                                #endregion



                                break;



                        }

                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndScrollView();
                        #endregion

                        #endregion


                        EditorGUILayout.EndHorizontal();

                        break;
                  


                    case 1:

                        EditorGUILayout.BeginHorizontal();

                        #region Controls/ability info
                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(abilityInfoWidth));


                      

                        InspectorToolbarAndAbilityInfo(ref positiontravelSettingsToolbarSelection, ref positiontravelSettingsToolbar);

                        EditorGUILayout.EndVertical();

                        #endregion

                        InspectorBoldVerticleLine();


                        #region Position & Travel 



                        editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                            false,
                                                                            false);

                        EditorGUILayout.BeginVertical();

                        #region Settings


                        switch ((int)positiontravelSettingsToolbarSelection) {
                            case 0:


                                InspectorSectionHeader("Travel Type & Starting Position");

                                #region SideBySide 

                                EditorGUILayout.BeginHorizontal();

                                #region Targeting/Travel

                                InspectorVerticalBox(true);


                                InspectorHelpBox("Travel type determines how the ability reaches it's destination. From selected targets to mouse position or just to travel forward from the entity position", false);


                                if ((string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") {

                                    EditorGUILayout.HelpBox("Travel type setting does not exist for Melee Abilities", MessageType.Warning);


                                }
                                else {

                                    EditorGUILayout.PropertyField(travelType, GUILayout.MaxWidth(250));

                                    switch ((string)travelType.enumNames[travelType.enumValueIndex]) {

                                        #region CustomScript targeting Settings
                                        case "CustomScript":

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.Space();
                                                InspectorHelpBox("Custom script will be added to the Ability. Please note unless added to the custom script the Ability will lose normal features found in this section");
                                                EditorGUILayout.PropertyField(customTravelScript, new GUIContent("Custom Script"), GUILayout.MaxWidth(400));
                                                // custom script has no travel or misc settings or additional settings

                                            }
                                            else {


                                                EditorGUILayout.HelpBox(" Custom Script does not have specific settings whilst ability is a raycast.", MessageType.Warning);
                                            }



                                            break;
                                        #endregion

                                        #region NoTravel targeting Settings
                                        case "NoTravel":

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.HelpBox("No travel does not have specific settings .", MessageType.Warning);
                                                // none added yet 
                                            }
                                            else {


                                                EditorGUILayout.HelpBox("No Travel can not be used whilst ability is a Raycast. Ability will default to Forward Settings.", MessageType.Warning);

                                            }


                                            break;
                                        #endregion

                                        #region Mouse2D targeting Settings
                                        case "Mouse2D":

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));
                                            }



                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));
                                                EditorGUILayout.PropertyField(selectedTargetOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                                EditorGUILayout.EndHorizontal();

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile")
                                            {
                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(selectedTargetForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.PropertyField(selectedTargetRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();

                                                EditorGUILayout.PropertyField(abilityCanMiss, new GUIContent("Ability Can Miss"), GUILayout.MaxWidth(180));
                                                InspectorHelpBox("If true then the ability has the potential to miss the target depending on the Originators miss chance and values");

                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(targetTravel);

                                                if (targetTravel.boolValue == true) {
                                                    EditorGUILayout.Space();
                                                    EditorGUIUtility.labelWidth = 125;

                                                    EditorGUILayout.PropertyField(seekTargetDelay, GUILayout.MaxWidth(180));

                                                    EditorGUIUtility.labelWidth = 110;
                                                    EditorGUILayout.Space();
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("Does the projectile travel to target or already appear there. If seek target delay is enabled then ability will travel forward in its starting direction until the delay is over and then will start heading towards target");

                                            }


                                            break;

                                        #endregion

                                        #region Forward3D targeting Settings
                                        case "Forward":
                                            EditorGUILayout.Space();
                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));
                                            }
                                            else {


                                                EditorGUILayout.HelpBox(" Forward does not have specific settings whilst ability is a raycast.", MessageType.Warning);
                                            }

                                            break;

                                        #endregion

                                        #region Selected Target targeting Settings 
                                        case "SelectedTarget":

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));
                                            }

                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));
                                                EditorGUILayout.PropertyField(selectedTargetOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(selectedTargetForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.PropertyField(selectedTargetRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();

                                            EditorGUILayout.PropertyField(abilityCanMiss, new GUIContent("Ability Can Miss"), GUILayout.MaxWidth(180));
                                            InspectorHelpBox("If true then the ability has the potential to miss the target depending on the Originators miss chance and values");

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(targetTravel);

                                                if (targetTravel.boolValue == true) {
                                                    EditorGUILayout.Space();
                                                    EditorGUIUtility.labelWidth = 125;

                                                    EditorGUILayout.PropertyField(seekTargetDelay, GUILayout.MaxWidth(180));

                                                    EditorGUIUtility.labelWidth = 110;
                                                    EditorGUILayout.Space();
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("Does the projectile travel to target or already appear there. If seek target delay is enabled then ability will travel forward in its starting direction until the delay is over and then will start heading towards target");

                                            }
       



                                            break;

                                        #endregion

                                        #region NearestTag targeting Settings 
                                        case "NearestTag":



                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));
                                            }

                                            EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));
                                                EditorGUILayout.PropertyField(selectedTargetOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(selectedTargetForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.PropertyField(selectedTargetRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();

                                            EditorGUILayout.PropertyField(abilityCanMiss, new GUIContent("Ability Can Miss"), GUILayout.MaxWidth(180));
                                            InspectorHelpBox("If true then the ability has the potential to miss the target depending on the Originators miss chance and values");


                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(targetTravel);

                                                if (targetTravel.boolValue == true) {
                                                    EditorGUILayout.Space();
                                                    EditorGUIUtility.labelWidth = 125;

                                                    EditorGUILayout.PropertyField(seekTargetDelay, GUILayout.MaxWidth(180));

                                                    EditorGUIUtility.labelWidth = 110;
                                                    EditorGUILayout.Space();
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("Does the projectile travel to target or already appear there. If seek target delay is enabled then ability will travel forward in its starting direction until the delay is over and then will start heading towards target");

                                            }
                                           


                                            break;

                                        #endregion

                                        #region Self targeting Settings 
                                        case "Self":

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));

                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));
                                                EditorGUILayout.PropertyField(selectedTargetOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(selectedTargetForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.PropertyField(selectedTargetRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();

                                                EditorGUILayout.PropertyField(abilityCanMiss, new GUIContent("Ability Can Miss"), GUILayout.MaxWidth(180));
                                                InspectorHelpBox("If true then the ability has the potential to miss the target depending on the Originators miss chance and values");


                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(targetTravel);

                                                if (targetTravel.boolValue == true) {
                                                    EditorGUILayout.Space();
                                                    EditorGUIUtility.labelWidth = 125;

                                                    EditorGUILayout.PropertyField(seekTargetDelay, GUILayout.MaxWidth(180));

                                                    EditorGUIUtility.labelWidth = 110;
                                                    EditorGUILayout.Space();
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("Does the projectile travel to target or already appear there. If seek target delay is enabled then ability will travel forward in its starting direction until the delay is over and then will start heading towards target");

                                            }
                                            else {


                                                EditorGUILayout.HelpBox("Self Type does not have specific settings whilst ability is a raycast.", MessageType.Warning);




                                            }





                                            break;

                                        #endregion

                                        #region ToWorld targeting Settings

                                        case "ToWorld":
                                     
                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));

                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));
                                                EditorGUILayout.PropertyField(selectedTargetOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(selectedTargetForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.PropertyField(selectedTargetRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();

                                                EditorGUILayout.PropertyField(abilityCanMiss, new GUIContent("Ability Can Miss"), GUILayout.MaxWidth(180));
                                                InspectorHelpBox("If true then the ability has the potential to miss the target depending on the Originators miss chance and values");


                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(targetTravel);

                                                if (targetTravel.boolValue == true) {
                                                    EditorGUILayout.Space();
                                                    EditorGUIUtility.labelWidth = 125;

                                                    EditorGUILayout.PropertyField(seekTargetDelay, GUILayout.MaxWidth(180));

                                                    EditorGUIUtility.labelWidth = 110;
                                                    EditorGUILayout.Space();
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("Does the projectile travel to target or already appear there. If seek target delay is enabled then ability will travel forward in its starting direction until the delay is over and then will start heading towards target");

                                            }
                                            else {

                                                EditorGUILayout.HelpBox(" To World can not be used whilst ability is a Raycast. Ability will default to Forward Settings.", MessageType.Warning);

                                            }



                                            break;

                                        #endregion

                                        #region MouseTarget targeting Settings 
                                        case "MouseTarget":

                                    

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));
                                            }

                                            EditorGUILayout.Space();
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));
                                            EditorGUILayout.PropertyField(selectedTargetOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                            EditorGUILayout.EndHorizontal();

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile")  {
                                             EditorGUILayout.Space();
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(selectedTargetForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(180));
                                            EditorGUILayout.PropertyField(selectedTargetRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(180));
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.Space();

                                                EditorGUILayout.PropertyField(abilityCanMiss, new GUIContent("Ability Can Miss"), GUILayout.MaxWidth(180));
                                                InspectorHelpBox("If true then the ability has the potential to miss the target depending on the Originators miss chance and values");


                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(targetTravel);
                                                if (targetTravel.boolValue == true) {
                                                    EditorGUILayout.Space();
                                                    EditorGUIUtility.labelWidth = 125;

                                                    EditorGUILayout.PropertyField(seekTargetDelay, GUILayout.MaxWidth(180));

                                                    EditorGUIUtility.labelWidth = 110;
                                                    EditorGUILayout.Space();
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("Does the projectile travel to target or already appear there. If seek target delay is enabled then ability will travel forward in its starting direction until the delay is over and then will start heading towards target");


                                            }

                               






                                            break;
                                        #endregion

                                        #region MouseForward targeting Settings
                                        case "MouseForward":
                                            EditorGUILayout.Space();
                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));
                                            }
                                            else {


                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));
                                                EditorGUILayout.PropertyField(selectedTargetOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                                EditorGUILayout.EndHorizontal();

                                            }
                                            EditorGUILayout.Space();


                                            break;
                                        #endregion

                                        #region CameraCenterFPS targeting Settings
                                        case "Crosshair":




                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                                EditorGUILayout.PropertyField(travelPhysics, GUILayout.MaxWidth(250));
                                            }

                                                EditorGUILayout.Space();

                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));
                                                EditorGUILayout.PropertyField(selectedTargetOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                                EditorGUILayout.EndHorizontal();

                                            if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile")
                                            {
                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(targetTravel);

                                                if (targetTravel.boolValue == true)
                                                {
                                                    EditorGUILayout.Space();
                                                    EditorGUIUtility.labelWidth = 125;

                                                    EditorGUILayout.PropertyField(seekTargetDelay, GUILayout.MaxWidth(180));

                                                    EditorGUIUtility.labelWidth = 110;
                                                    EditorGUILayout.Space();
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("Does the projectile travel to target or already appear there. If seek target delay is enabled then ability will travel forward in its starting direction until the delay is over and then will start heading towards target");
                                            }               



                                            break;
                                            #endregion

                                    }


                                }


                                EditorGUILayout.EndVertical();

                                #endregion

                                #region Starting Position

                                InspectorVerticalBox(true);


                                InspectorHelpBox("Settings define where the ability will start after initiation.");



                                EditorGUIUtility.labelWidth = 130;
                                if ((string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") {

                                    // If melee then change travel script to no travel 
                                    travelType.enumValueIndex = 5;
                                }

                                EditorGUILayout.PropertyField(startingPosition, GUILayout.MaxWidth(250));



                                // get enum value index and if it's on object display new layout
                                if (((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "OnObject") {
                                   

                                    EditorGUILayout.PropertyField(startingPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(400));
                                }




                                // get enum value index and if it's on object display new layout
                                if (((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "OnTag" || ((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "OnSelfTag") {

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 10;
                                    EditorGUILayout.LabelField("Select Tag");
                                    ResetLabelWidth();
                                    startingPositionOnTag.stringValue = EditorGUILayout.TagField(startingPositionOnTag.stringValue, GUILayout.MaxWidth(120));
                                    InspectorHorizontalSpace(59);
                                    EditorGUILayout.EndHorizontal();
              

                                }


                                // get enum value index and if it's on object display new layout
                                if (((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "Target") {
                         

                                    EditorGUILayout.PropertyField(startingPositionAuxiliarySoftTarget, new GUIContent("Auxiliary SoftTarget"), GUILayout.MaxWidth(400));
                                }
                                ResetLabelWidth();

 

                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Position Offset", GUILayout.MaxWidth(100));
                                EditorGUILayout.PropertyField(startingPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(startingPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(180));
                                EditorGUILayout.PropertyField(startingPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(180));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                    EditorGUILayout.PropertyField(startingRotation, GUILayout.MaxWidth(250));
                                    EditorGUILayout.Space();
                                    EditorGUIUtility.labelWidth = 130;
                                    EditorGUILayout.PropertyField(setEulerRotation, GUILayout.MaxWidth(250));
                                }
                                ResetLabelWidth();
                                EditorGUILayout.EndVertical();

                                #endregion



                          

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                if ((string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") {
                                    InspectorSectionHeader("Melee Settings");
                                }
                                else {
                                    InspectorSectionHeader((string)travelType.enumNames[travelType.enumValueIndex] + " Settings");
                                }

                                #region AllWay 

                                InspectorVerticalBox();



                                switch ((string)travelType.enumNames[travelType.enumValueIndex]) {

                                    #region CustomScript targeting Settings
                                    case "CustomScript":

                                        EditorGUILayout.HelpBox("Custom Script does not have specific settings.", MessageType.Warning);

                                      

                                        break;
                                    #endregion

                                    #region NoTravel/Melee targeting Settings
                                    case "NoTravel":

                                 

                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                            EditorGUIUtility.labelWidth = 230;
                                            EditorGUILayout.PropertyField(travelWithCaster, new GUIContent("Travel w/ Entity"));
                                            EditorGUILayout.Space();
                                        }
                                        else    if ((string)abilityType.enumNames[abilityType.enumValueIndex] == "Melee") { // bespoke melee settings as its not a travel type enum      
                                            EditorGUIUtility.labelWidth = 210;

                                            EditorGUILayout.PropertyField(noTargetStillTravel, new GUIContent("Always Activate"));
                                            InspectorHelpBox("If always activate is enabled then a selected target is not required to activate the melee ability.");

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(rotateToSelectedTarget, new GUIContent("Rotate to Target"));
                                            EditorGUILayout.PropertyField(meleeKeepRotatingToSelectedTarget, new GUIContent("Keep Rotating to Target"));                                    
                                            EditorGUILayout.EndHorizontal();

                                            InspectorHelpBox("Rotate to target if enabled will always rotate the entity to face the current target when preparing and initiating. Keep rotating will make sure the entity is rotated towards the target for the whole melee ability");

                                            EditorGUILayout.PropertyField(auxiliarySoftTarget, new GUIContent("Auxiliary SoftTarget"));
                                            InspectorHelpBox("If no selected target then the ability will as backup - treat the current softTarget as the main target");

                                                                          
                                                
                                            EditorGUILayout.BeginHorizontal();
                                        
                                            EditorGUILayout.PropertyField(useRange);
                                            if (useRange.boolValue == true) {
                                                EditorGUIUtility.labelWidth = 140;
                                                EditorGUILayout.PropertyField(selectedTargetRangeGreaterThan, new GUIContent("Range Greater Than"));

                                                EditorGUILayout.PropertyField(selectedTargetRangeLessThan, new GUIContent("Range Less Than"));
                                                                     
                                            }
                                            EditorGUILayout.EndHorizontal();

                                            EditorGUIUtility.labelWidth = 210;
                                            EditorGUILayout.PropertyField(targetFacing, new GUIContent("Face Target Required"));

                                            InspectorHelpBox("If entity currently has a target the option is avaliable to determine a range or a face target condition to activate the melee attack");
                                            

                                            EditorGUILayout.Space();
                                            EditorGUILayout.PropertyField(rotateToCameraCenter);
                                            InspectorHelpBox("Entity will face same direction as camera when using this Ability. If rotate to  target is also active then entity will only turn if no target is selected");

                                            EditorGUIUtility.labelWidth = 290;
                                            EditorGUILayout.PropertyField(hitsStopMeleeAttack, new GUIContent("Hit Prevention Will Immediately Stop Melee Attack"));

                                            InspectorHelpBox("If true then hits received can stagger the melee attack stopping the projectile and animation immediately (after activation)");

                                            ResetLabelWidth();

                                        }
                                        else {


                                            EditorGUILayout.HelpBox("No Travel does not have specific settings whilst ability is a raycast.", MessageType.Warning);
                                        }
                                   

                                        break;
                                    #endregion

                                    #region Mouse2D targeting Settings
                                    case "Mouse2D":

                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                            EditorGUIUtility.labelWidth = 150;

                                            EditorGUILayout.PropertyField(abilityBeforeTarget);
                                            InspectorHelpBox("If ticked then the user will have to select the target after the Ability key has been pressed. If unticked then the Ability will target the current selected target.", false);


                                            EditorGUILayout.PropertyField(rotateToSelectedTarget, new GUIContent("Rotate to Target"));
                                            InspectorHelpBox("Entity will face selected target when using this Ability", false);

                                            EditorGUIUtility.labelWidth = 250;
                                            EditorGUILayout.PropertyField(continuouslyTurnToDestination);
                                            InspectorHelpBox("Does the projectile stop at target or carry on travelling", false);
                                            ResetLabelWidth();
                                        }
                                        else {


                                            EditorGUILayout.HelpBox(" Mouse2D does not have specific settings whilst ability is a raycast.", MessageType.Warning);
                                        }

                  

                                        break;

                                    #endregion

                                    #region Forward3D targeting Settings
                                    case "Forward":
                                        EditorGUIUtility.labelWidth = 170;

                                        EditorGUILayout.PropertyField(rotateToCameraCenter);
                                        InspectorHelpBox("Entity will turn to face the same direction as camera", false);

                                        EditorGUIUtility.labelWidth = 110;

                                        break;

                                    #endregion

                                    #region Selected Target targeting Settings 
                                    case "SelectedTarget":

                                    

                                            EditorGUIUtility.labelWidth = 210;
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(rotateToSelectedTarget, new GUIContent("Rotate to Target"));
                                            EditorGUILayout.PropertyField(targetFacing, new GUIContent("Face Target Required"));
                                            EditorGUILayout.EndHorizontal();

                                            EditorGUILayout.BeginHorizontal();

                                            EditorGUILayout.PropertyField(useRange);
                                            if (useRange.boolValue == true) {
                                                EditorGUIUtility.labelWidth = 140;
                                                EditorGUILayout.PropertyField(selectedTargetRangeGreaterThan, new GUIContent("Range Greater Than"));

                                                EditorGUILayout.PropertyField(selectedTargetRangeLessThan, new GUIContent("Range Less Than"));

                                            }
                                            EditorGUILayout.EndHorizontal();

                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                            EditorGUILayout.Space();
                                            EditorGUIUtility.labelWidth = 220;
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(continuouslyTurnToDestination);
                                            EditorGUILayout.PropertyField(affectOnlyTarget);
                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("Does the projectile stop at target or carry on travelling. If Affect Only is ticked then the Ability will only collide and effect the selected target. Ignoring any other entity on the way. ", false);
                                        }

                                            EditorGUIUtility.labelWidth = 210;

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(abilityBeforeTarget);
                                         

                                         

                                            if (abilityBeforeTarget.boolValue == true) {
                                                EditorGUILayout.PropertyField(abilityBeforeSoftTarget, new GUIContent("Force SoftTarget"));
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("If ticked then the user will have to select the target after the Ability key has been pressed. If softtarget is forced then it will come up to confirm target before ability is fired", false);

                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(loopTillTargetFound, new GUIContent("Loop Till Target Found"));                                   
                                                EditorGUILayout.EndHorizontal();
                                            } else {

                                                EditorGUILayout.EndHorizontal();
                                            }


                                           
                                            if (abilityBeforeTarget.boolValue == false) {
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(auxiliarySoftTarget, new GUIContent("Auxiliary SoftTarget"));
                                                EditorGUILayout.PropertyField(noTargetStillTravel, new GUIContent("Always Travel"));                                            
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("If no selected target then the ability will as backup - treat the current softTarget as the main target. If Always travel is enabled then the ability will still fire in a straight line if no target has been selected");

                                            }


                                            EditorGUILayout.PropertyField(selectedTargetRestrictTargets, new GUIContent("Restrict Targets"));
                                            if (selectedTargetRestrictTargets.boolValue == true) {
                                                InspectorListBox("Only Cast On Following Tags", selectedTargetOnlyCastOnTag);
                                            }
                                          




 


                                        ResetLabelWidth();



                                        break;

                                    #endregion

                                    #region NearestTag targeting Settings 
                                    case "NearestTag":

                                   

                                            EditorGUIUtility.labelWidth = 220;

       
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(rotateToSelectedTarget, new GUIContent("Rotate to Target"));
                                            EditorGUILayout.PropertyField(targetFacing, new GUIContent("Face Target Required"));
                                            EditorGUILayout.EndHorizontal();


                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(continuouslyTurnToDestination);
                                            EditorGUILayout.PropertyField(affectOnlyTarget);
                                            EditorGUILayout.EndHorizontal();
                                        }
                                            EditorGUILayout.PropertyField(noTargetStillTravel, new GUIContent("Always Travel"));
                                            InspectorHelpBox("Does the projectile stop at target or carry on travelling. If Affect Only is ticked then the Ability will only collide and effect the selected target. Ignoring any other entity on the way. If always travel is enabled then the ability will still fire in a straight line if no target has been selected ", false);

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(travelNearestTagRange, new GUIContent("Nearest Tag Range Amount"), GUILayout.MaxWidth(280));
                                            EditorGUILayout.PropertyField(travelNearestTagIgnoreOriginator, new GUIContent("Ignore Activating Entity"));
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.PropertyField(travelNearestTagRandomiseSearch, new GUIContent("Randomise Search"));

                                            InspectorHelpBox("The ability will find a target by searching within the range setup for any tags provided in the list below. Tags will be searched in the order listed below. Randomise search will shuffle the entities found before checking tags");
                                            

                                            InspectorListBox("Target Nearest Tag:", travelNearestTagList);
            


                                        
                                     


                                        ResetLabelWidth();



                                        break;

                                    #endregion

                                    #region Self targeting Settings 
                                    case "Self":

                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {

                                            EditorGUIUtility.labelWidth = 210;


                                          

                                            if (((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "Target" || ((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "OnWorld") {


                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(rotateToSelectedTarget, new GUIContent("Rotate to Target"));
                                                if (((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "Target") {
                                                    EditorGUILayout.PropertyField(targetFacing, new GUIContent("Face Target Required"));
                                                }
                                                EditorGUILayout.EndHorizontal();

                                                EditorGUILayout.BeginHorizontal();

                                                EditorGUILayout.PropertyField(useRange);
                                                if (useRange.boolValue == true) {
                                                    EditorGUIUtility.labelWidth = 150;
                                                    EditorGUILayout.PropertyField(selectedTargetRangeGreaterThan, new GUIContent("Range Greater Than"));

                                                    EditorGUILayout.PropertyField(selectedTargetRangeLessThan, new GUIContent("Range Less Than"));
                                                    ResetLabelWidth();
                                                }
                                                EditorGUILayout.EndHorizontal();

                                            }

                                            EditorGUIUtility.labelWidth = 220;
                                            //2 ifs used so I could add this section at same place as selected target 
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(continuouslyTurnToDestination);
                                            EditorGUILayout.PropertyField(affectOnlyTarget);
                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("Does the projectile stop at target or carry on travelling. If Affect Only is ticked then the Ability will only collide and effect the selected target. Ignoring any other entity on the way. ", false);

                                            if (((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "Target" || ((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "OnWorld") {


                                                EditorGUIUtility.labelWidth = 210;

                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(abilityBeforeTarget);




                                                if (abilityBeforeTarget.boolValue == true) {
                                                    if (((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "Target") { 
                                                    EditorGUILayout.PropertyField(abilityBeforeSoftTarget, new GUIContent("Force SoftTarget"));
                                                    }

                                                    EditorGUILayout.EndHorizontal();
                                                    InspectorHelpBox("If ticked then the user will have to select the target after the Ability key has been pressed. If softtarget is forced then it will come up to confirm target before ability is fired", false);

                                                    EditorGUILayout.BeginHorizontal();
                                                    EditorGUILayout.PropertyField(loopTillTargetFound, new GUIContent("Loop Till Target Found"));
                                                    if (((string)startingPosition.enumNames[startingPosition.enumValueIndex]) == "OnWorld") {
                                                        EditorGUILayout.PropertyField(abilityBeforeTargetIndicatorScaleToEffectRadius, new GUIContent("Scale to Effect Radius"));
                                                    }

                                                    EditorGUILayout.EndHorizontal();
                                                }
                                                else {

                                                    EditorGUILayout.EndHorizontal();
                                                }





                                            }

                                
                                        }
                                        else {


                                            EditorGUILayout.HelpBox(" self does not have have specific settings whilst ability is a raycast.", MessageType.Warning);
                                        }


                                        ResetLabelWidth();



                                        break;

                                    #endregion

                                    #region ToWorld targeting Settings

                                    case "ToWorld":

                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {

                                            EditorGUIUtility.labelWidth = 210;

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(rotateToSelectedTarget, new GUIContent("Rotate to Target"));
                                            EditorGUILayout.EndHorizontal();



                                            EditorGUILayout.BeginHorizontal();

                                            EditorGUILayout.PropertyField(useRange);
                                            if (useRange.boolValue == true) {
                                                EditorGUIUtility.labelWidth = 150;
                                                EditorGUILayout.PropertyField(selectedTargetRangeGreaterThan, new GUIContent("Range Greater Than"));

                                                EditorGUILayout.PropertyField(selectedTargetRangeLessThan, new GUIContent("Range Less Than"));
                                                ResetLabelWidth();
                                            }
                                            EditorGUILayout.EndHorizontal();

                                            EditorGUIUtility.labelWidth = 220;
                                            //2 ifs used so I could add this section at same place as selected target 
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(continuouslyTurnToDestination);
                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("Does the projectile stop at target or carry on travelling.", false);


                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(abilityBeforeTarget);

                                            if (abilityBeforeTarget.boolValue == true) {

                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("If ticked then the user will have to select the target after the Ability key has been pressed. If softtarget is forced then it will come up to confirm target before ability is fired", false);

                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(loopTillTargetFound, new GUIContent("Loop Till Target Found"));
                                                EditorGUILayout.PropertyField(abilityBeforeTargetIndicatorScaleToEffectRadius, new GUIContent("Scale to Effect Radius"));
                                                EditorGUILayout.EndHorizontal();
                                                InspectorHelpBox("If true Ability indicator will scale showing who is in the effect range");
                                            }
                                            else {

                                                EditorGUILayout.EndHorizontal();
                                            }



                                        }
                                        else {


                                            EditorGUILayout.HelpBox(" To World does not have specific settings whilst ability is a raycast.", MessageType.Warning);
                                        }

                                        ResetLabelWidth();
                                    
                                        break;

                                    #endregion

                                    #region MouseTarget targeting Settings 
                                    case "MouseTarget":

                                        EditorGUILayout.PropertyField(rotateToSelectedTarget, new GUIContent("Rotate to Target"));
                                        InspectorHelpBox("Entity will face selected target when using this Ability", false);

                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                            EditorGUIUtility.labelWidth = 220;

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(continuouslyTurnToDestination);
                                            EditorGUILayout.PropertyField(mouseFrontOnly);                                      
                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("Does the projectile stop at target or carry on travelling. If FrontOnly is enabled the target position will always be infront of the player. If false then player will activate all around in world position. ", false);

                                            EditorGUILayout.PropertyField(abilityBeforeTarget);
                                            InspectorHelpBox("If ticked then the user will have to select the target after the Ability key has been pressed. If unticked then the Ability will target the current selected target.", false);

                                            ResetLabelWidth();
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(mouseForwardLockX, new GUIContent("Lock X"));
                                            EditorGUILayout.PropertyField(mouseForwardLockY, new GUIContent("Lock Y"));
                                            EditorGUILayout.PropertyField(mouseForwardLockZ, new GUIContent("Lock Z"));
                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("By ticking the Locks the Ability will not record the corresponding co-ordinate for the target. Example: By locking Y the ability no matter where the mouse has clicked will never travel up or down.", false);

                                        }

                                        ResetLabelWidth();


                                        break;
                                    #endregion

                                    #region MouseForward targeting Settings
                                    case "MouseForward":

                                        EditorGUIUtility.labelWidth = 130;


                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {
                                            EditorGUILayout.PropertyField(mouseFrontOnly);
                                            InspectorHelpBox("If ticked then the target position will always be infront of the player. If false then player will fire all around in world position.", false);

                                            ResetLabelWidth();
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(mouseForwardLockX, new GUIContent("Lock X"));
                                            EditorGUILayout.PropertyField(mouseForwardLockY, new GUIContent("Lock Y"));
                                            EditorGUILayout.PropertyField(mouseForwardLockZ, new GUIContent("Lock Z"));
                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("By ticking the Locks the Ability will not record the corresponding co-ordinate for the target. Example: By locking Y the ability no matter where the mouse has clicked will never travel up or down.", false);


                                        } else {
                                            EditorGUILayout.HelpBox("No additional Mouse Forward settings exist when ability is set to raycast mode", MessageType.Warning);
                                        }

                                        ResetLabelWidth();



                                        break;
                                    #endregion

                                    #region CrossHair targeting Settings
                                    case "Crosshair":
                                        EditorGUIUtility.labelWidth = 220;
                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {

                                            EditorGUILayout.PropertyField(rotateToSelectedTarget, new GUIContent("Rotate to Target"));
                                            InspectorHelpBox("Entity will face selected target when using this Ability", false);

                                            EditorGUILayout.PropertyField(continuouslyTurnToDestination);
                                            InspectorHelpBox("Does the projectile stop at target or carry on travelling");


                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(affectLayer, GUILayout.MaxWidth(350));
                                            EditorGUILayout.PropertyField(crossHairRaycastRadius, GUILayout.MaxWidth(250));
                                            EditorGUILayout.EndHorizontal();

                                            EditorGUILayout.PropertyField(crossHairRaycastReturnedDistance,  new GUIContent("Returned Distance"), GUILayout.MaxWidth(250));
                                            InspectorHelpBox("The distance point along the raycast which is returned if no objects have been hit or ability has been configured to always return a distance point only");

                                            EditorGUILayout.PropertyField(crossHairRaycastReturnDistancePointOnly, new GUIContent("Distance Point Only"));
                                            InspectorHelpBox("If selected then the ray cast will always get the distance point x units away instead of looking for points on objects hit by the cast");
                                           
                                            EditorGUILayout.PropertyField(crossHairRecordTargetOnActivation, new GUIContent("Record Target On Activation"));
                                            InspectorHelpBox("If selected then ray will be casted during ability activation otherwise it is done when the ability is initiated after being prepared etc");
                                           
                                        } else {

                                            EditorGUILayout.HelpBox(" Crosshair  does not have specific settings whilst ability is a raycast.", MessageType.Warning);

                                        }



                                        ResetLabelWidth();

                                        break;
                                        #endregion

                                }

                                EditorGUILayout.EndVertical();

                                #endregion

                                break;
                            case 1:

                                InspectorSectionHeader("Travel Settings");


                                #region AllWay 

                                InspectorVerticalBox();

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {
                                    switch ((string)travelType.enumNames[travelType.enumValueIndex]) {
                                        #region NoTravel Travel & Misc Settings
                                        case "NoTravel":
                                        case "CustomScript":


                                            EditorGUIUtility.labelWidth = 130;

                                            EditorGUILayout.HelpBox("Travel Types: No Travel and Custom Script can not have travel delay or durations", MessageType.Warning);



                                            break;
                                        #endregion

                                        #region  Travel & Misc Settings for rest of travel types
                                        // these at present all share the same abilities (Except onworld/toworld which adds a little setting) this has been setup this way for future development
                                        // if more unique settings was needed for each of these scripts

                                        case "Mouse2D":
                                        case "Forward":
                                        case "SelectedTarget":
                                        case "Self":
                                        case "ToWorld":
                                        case "MouseTarget":
                                        case "MouseForward":
                                        case "Crosshair":
                                        case "NearestTag":


                                            EditorGUIUtility.labelWidth = 140;



                                            EditorGUILayout.BeginHorizontal();

                                            EditorGUILayout.PropertyField(travelDelay, GUILayout.MaxWidth(180));


                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("Delay time before ability will start travelling");
                                            EditorGUIUtility.labelWidth = 150;
                                            EditorGUILayout.PropertyField(applyTravelDuration);

                                            GUILayout.BeginHorizontal();


                                            // travel duration only applies to abilities that do not always follow a target 
                                            if (applyTravelDuration.boolValue == true) {
                                                EditorGUIUtility.labelWidth = 150;
                                                EditorGUILayout.PropertyField(travelDurationTime, GUILayout.MaxWidth(180));
                                                EditorGUILayout.Space();
                                                EditorGUILayout.PropertyField(travelDurationStopSuddenly, new GUIContent("Stop Instantly"));

                                            }
                                            ResetLabelWidth();
                                            GUILayout.EndHorizontal();

                                            InspectorHelpBox("How long the ability will travel for and if it will stop instantly or over time with physics");
                                            EditorGUILayout.Space();



                                            break;
                                            #endregion
                                    }


                                }
                                else {

                                    EditorGUILayout.HelpBox("RayCast Abilities do not currently have travel settings.", MessageType.Warning);
                                }

                   

                                EditorGUILayout.EndVertical();

                                #endregion

                                #region AllWay 

                                InspectorVerticalBox();


                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {

                                    EditorGUILayout.Space();
                                    switch ((string)travelType.enumNames[travelType.enumValueIndex]) {
                                        #region NoTravel Travel & Misc Settings
                                        case "NoTravel":
                                        case "CustomScript":


                                            EditorGUIUtility.labelWidth = 130;


                                            EditorGUILayout.BeginHorizontal();

                                            EditorGUILayout.PropertyField(mass, GUILayout.MaxWidth(180));
                                            EditorGUILayout.Space();
                                            EditorGUILayout.PropertyField(travelDrag, GUILayout.MaxWidth(180));
                                            EditorGUILayout.Space();
                                            EditorGUILayout.EndHorizontal();

                                            EditorGUILayout.Space();

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(applyGravity, GUILayout.MaxWidth(305));

                                            if (applyGravity.boolValue == true) {

                                                EditorGUILayout.PropertyField(applyGravityDelay, new GUIContent("Gravity Delay"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.Space();
                                            }

                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("Add gravity to the ability after the delay time set");

                                            EditorGUILayout.PropertyField(isKinematic, GUILayout.MaxWidth(180));

                                            EditorGUILayout.PropertyField(neverSleep, GUILayout.MaxWidth(180));
                                            InspectorHelpBox("If enabled then the ability will never sleep and always look for collisions. Sleeping occurs when a Rigidbody is moving slower than a defined minimum linear or rotational speed, the physics engine assumes it has come to a halt");
                                            break;
                                        #endregion

                                        #region  Travel & Misc Settings for rest of travel types
                                        // these at present all share the same abilities (Except onworld/toworld which adds a little setting) this has been setup this way for future development
                                        // if more unique settings was needed for each of these scripts

                                        case "Mouse2D":
                                        case "Forward":
                                        case "SelectedTarget":
                                        case "Self":
                                        case "ToWorld":
                                        case "MouseTarget":
                                        case "MouseForward":
                                        case "Crosshair":
                                        case "NearestTag":






                                            EditorGUILayout.BeginHorizontal();

                                            EditorGUILayout.PropertyField(mass, GUILayout.MaxWidth(180));
                                            EditorGUILayout.Space();
                                            EditorGUILayout.PropertyField(travelDrag, GUILayout.MaxWidth(180));
                                            EditorGUILayout.Space();
                                            EditorGUILayout.EndHorizontal();

                                            EditorGUILayout.Space();

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(applyGravity, GUILayout.MaxWidth(305));

                                            if (applyGravity.boolValue == true) {

                                                EditorGUILayout.PropertyField(applyGravityDelay, new GUIContent("Gravity Delay"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.Space();
                                            }

                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("Add gravity to the ability after the delay time set");

                                            EditorGUILayout.PropertyField(neverSleep, GUILayout.MaxWidth(180));
                                            InspectorHelpBox("If enabled then the ability will never sleep and always look for collisions. Sleeping occurs when a Rigidbody is moving slower than a defined minimum linear or rotational speed, the physics engine assumes it has come to a halt");

                                            break;
                                            #endregion
                                    }

                                } else {

                                    EditorGUILayout.HelpBox("RayCast Abilities do not currently have travel settings.", MessageType.Warning);
                                }




                                EditorGUILayout.EndVertical();

                                #endregion

                                InspectorSectionHeader("Bounce & Boomerang");

                                #region AllWay 

                                InspectorVerticalBox();

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {
                                    switch ((string)travelType.enumNames[travelType.enumValueIndex]) {
                                        #region NoTravel Travel & Misc Settings
                                        case "NoTravel":
                                        case "CustomScript":




                                            EditorGUILayout.HelpBox("Travel Types: No Travel and Custom Script can not boomerang or bounce", MessageType.Warning);



                                            break;
                                        #endregion

                                        #region  Travel & Misc Settings for rest of travel types
                                        // these at present all share the same abilities (Except onworld/toworld which adds a little setting) this has been setup this way for future development
                                        // if more unique settings was needed for each of these scripts

                                        case "Mouse2D":
                                        case "Forward":
                                        case "SelectedTarget":
                                        case "Self":
                                        case "ToWorld":
                                        case "MouseTarget":
                                        case "MouseForward":
                                        case "Crosshair":
                                        case "NearestTag":



                                            EditorGUIUtility.labelWidth = 130;

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(boomerangMode, new GUIContent("Boomerang"), GUILayout.MaxWidth(306));

                                            if (boomerangMode.boolValue == true) {
                                                EditorGUILayout.PropertyField(boomerangDelay, new GUIContent("Delay"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.Space();
                                            }

                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("Boomerang Effect - after delay the ability will return back to the caster. Once caster is reached ability will dissapear", false);

                                            EditorGUILayout.Space();

                                            EditorGUILayout.PropertyField(bounceMode, new GUIContent("Bounce"));
                                            InspectorHelpBox("Bounce Effect - After collision the ability will bounce to near targets depending on bounce targeting type.", false);

                                            if (bounceMode.boolValue == true) {
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(bounceAmount, new GUIContent("Bounce Amount"), GUILayout.MaxWidth(180));
                                                EditorGUILayout.Space();
                                                EditorGUILayout.PropertyField(bounceRange, GUILayout.MaxWidth(180));
                                                EditorGUILayout.Space();
                                                EditorGUILayout.EndHorizontal();


                                                EditorGUIUtility.labelWidth = 210;
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(enableRandomBounce, new GUIContent("Random Bounce"));
                                                EditorGUILayout.PropertyField(bounceOnCaster, new GUIContent("Bounce On Activating Entity"));
                                                EditorGUILayout.EndHorizontal();


                                                EditorGUIUtility.labelWidth = 210;
                                                EditorGUILayout.PropertyField(startBounceTagRequired, new GUIContent("Start Bounce On Specific Tag"));
                                                ResetLabelWidth();

                                                if (startBounceTagRequired.boolValue == true) {
                                                    InspectorListBox("Tag Required To Start Ability Bouncing", startBounceRequiredTags);
                                                }
                                                EditorGUILayout.Space();

                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField("Position Offset", GUILayout.MaxWidth(100));

                                                EditorGUILayout.PropertyField(bouncePositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.PropertyField(bouncePositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                                EditorGUILayout.PropertyField(bouncePositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                                EditorGUILayout.EndHorizontal();

                                                EditorGUILayout.Space();
                                                EditorGUILayout.PropertyField(bounceTarget, GUILayout.MaxWidth(310));
                                                EditorGUILayout.Space();

                                                if (((string)bounceTarget.enumNames[bounceTarget.enumValueIndex]) == "NearestTag") {

                                                    InspectorListBox("Bounce Tag", bounceTag);
                                                }
                                            }

                                            

                                            EditorGUILayout.Space();

                                            break;
                                            #endregion
                                    }

                                }
                                else {

                                    EditorGUILayout.HelpBox("RayCast Abilities can't bounce or boomerang.", MessageType.Warning);
                                }

                       


                                EditorGUILayout.EndVertical();

                                #endregion

                                break;

                            case 2:

                                InspectorSectionHeader("Additional Starting Positions", "An extra Ability object will be created for each Additional Starting Position. The object is a duplicate of the normal " +
                                    "Ability object and will operate in the same way. The only difference being it will start from another place. This functionality can be used for example " +
                                    "to have 2 fireballs fire from both hands or for melee attacks to have each fist apply damage");

                                #region Additional Starting Position

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee")
                                {

                                    if (GUILayout.Button(new GUIContent(" Add Additional Starting Position", AddIcon, "Add Additional Starting Position")))
                                    {
                                        var additionalSPIndex = additionalStartPositions.arraySize;
                                        additionalStartPositions.InsertArrayElementAtIndex(additionalSPIndex);

                                    }

                                    for (int n = 0; n < additionalStartPositions.arraySize; n++)
                                    {

                                        #region AllWay 

                                        InspectorVerticalBox();

                                        InspectorPropertyBox("Additional Start Position", additionalStartPositions, n);

                                        EditorGUILayout.Space();

                                        if (additionalStartPositions.arraySize == 0 || n > additionalStartPositions.arraySize - 1)
                                        {
                                            break;
                                        }


                                        SerializedProperty meAdditionalStartPosition = additionalStartPositions.GetArrayElementAtIndex(n);
                                        SerializedProperty addStartingPosition = meAdditionalStartPosition.FindPropertyRelative("startingPosition");
                                        SerializedProperty addStartingDelay = meAdditionalStartPosition.FindPropertyRelative("startingDelay");

                                        SerializedProperty addStartingPositionOnObject = meAdditionalStartPosition.FindPropertyRelative("startingPositionOnObject");
                                        SerializedProperty addStartingPositionOnTag = meAdditionalStartPosition.FindPropertyRelative("startingPositionOnTag");
                                        SerializedProperty addStartingPositionOffset = meAdditionalStartPosition.FindPropertyRelative("startingPositionOffset");
                                        SerializedProperty addStartingPositionForwardOffset = meAdditionalStartPosition.FindPropertyRelative("startingPositionForwardOffset");
                                        SerializedProperty addStartingPositionRightOffset = meAdditionalStartPosition.FindPropertyRelative("startingPositionRightOffset");
                                        SerializedProperty addStartingRotation = meAdditionalStartPosition.FindPropertyRelative("startingRotation");
                                        SerializedProperty addSetEulerRotation = meAdditionalStartPosition.FindPropertyRelative("setEulerRotation");



                                        EditorGUIUtility.labelWidth = 140;

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(addStartingPosition, GUILayout.MaxWidth(250));

                                        EditorGUILayout.PropertyField(addStartingDelay, GUILayout.MaxWidth(250));
                                        EditorGUILayout.EndHorizontal();

                                        // get enum value index and if it's on object display new layout
                                        if (((string)addStartingPosition.enumNames[addStartingPosition.enumValueIndex]) == "OnObject")
                                        {


                                            EditorGUILayout.PropertyField(addStartingPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(400));
                                        }




                                        // get enum value index and if it's on object display new layout
                                        if (((string)addStartingPosition.enumNames[addStartingPosition.enumValueIndex]) == "OnTag" || ((string)addStartingPosition.enumNames[addStartingPosition.enumValueIndex]) == "OnSelfTag")
                                        {

                                       
                                            EditorGUIUtility.labelWidth = 10;
                                            EditorGUILayout.LabelField("Select Tag");
                                            addStartingPositionOnTag.stringValue = EditorGUILayout.TagField(addStartingPositionOnTag.stringValue, GUILayout.MaxWidth(120));
                                          


                                        }


                                        ResetLabelWidth();



                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Position Offset", GUILayout.MaxWidth(100));
                                        EditorGUILayout.PropertyField(addStartingPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(addStartingPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(180));
                                        EditorGUILayout.PropertyField(addStartingPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(180));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();

                                        if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile")
                                        {
                                            EditorGUILayout.PropertyField(addStartingRotation, GUILayout.MaxWidth(250));
                                            EditorGUILayout.Space();
                                            EditorGUIUtility.labelWidth = 130;
                                            EditorGUILayout.PropertyField(addSetEulerRotation, GUILayout.MaxWidth(250));
                                        }
                                        ResetLabelWidth();


                                        EditorGUILayout.EndVertical();

                                        #endregion

                                       
                                    }


                                } else {

                                    EditorGUILayout.HelpBox("RayCast Abilities can't have additional starting positions.", MessageType.Warning);
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
                    case 2:

                        EditorGUILayout.BeginHorizontal();

                        #region Controls/ability info
                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(abilityInfoWidth));

                       InspectorToolbarAndAbilityInfo(ref collisionimpactSettingsToolbarSelection, ref collisionimpactSettingsToolbar);

                        EditorGUILayout.EndVertical();

                        #endregion

                        InspectorBoldVerticleLine();


                        #region Collision & Physics 

                        editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                       false,
                                                                       false);

                        EditorGUILayout.BeginVertical();

                        #region Settings

                        switch ((int)collisionimpactSettingsToolbarSelection) {
                            case 0:

                                InspectorSectionHeader("Add Collider Settings");

                                #region AllWay 



                                InspectorVerticalBox();

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {

                                    EditorGUILayout.PropertyField(addAbilityCollider, new GUIContent("Add Collider"));                                 
                                    InspectorHelpBox("If disabled then the ability will initiate without a collider being added. Any colliders already present on the object will remain.", false);

                                    if (addAbilityCollider.boolValue == true) {
                                        EditorGUILayout.PropertyField(useColliderTrigger, new GUIContent("Is Trigger"));
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(useGraphicRadius, new GUIContent("Graphic Radius"));

                                        if (useGraphicRadius.boolValue == false) {
                                            EditorGUILayout.PropertyField(colliderRadius, GUILayout.MaxWidth(180));
                                            EditorGUILayout.Space();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Collider Offset", GUILayout.MaxWidth(100));
                                        EditorGUILayout.PropertyField(colliderOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        InspectorHelpBox("If enabled then the projectile radius is calculated depending on the renderer size of the projectile. If disabled a radius can be picked");


                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 130;
                                        EditorGUILayout.PropertyField(applyColliderSettingsToParent, new GUIContent("Apply To Parent"));
                                        EditorGUILayout.PropertyField(applyColliderSettingsToChildren, new GUIContent("Apply To Children"));
                                        ResetLabelWidth();
                                        EditorGUILayout.EndHorizontal();
                                        InspectorHelpBox("You can select to apply the settings to the abilities parent collider and/or colliders found in the children of the ability");

                                        GUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(colliderTimeDelay, new GUIContent("Time Delay"));

                                        if (colliderTimeDelay.boolValue == true) {
                                            EditorGUILayout.PropertyField(colliderDelayTime, new GUIContent("Time"), GUILayout.MaxWidth(180));
                                        }
                                        EditorGUILayout.Space();
                                        GUILayout.EndHorizontal();
                                        InspectorHelpBox("Collider will come on after the delay time.");

                                        EditorGUILayout.PropertyField(colliderKeyPressDelay, new GUIContent("Key Delay"));

                                        GUILayout.BeginHorizontal();

                                        if (colliderKeyPressDelay.boolValue == true) {

                                            EditorGUILayout.PropertyField(colliderDelayInputType, new GUIContent("Input Type"), GUILayout.MaxWidth(250));

                                            EditorGUILayout.Space();


                                            if (((string)colliderDelayInputType.enumNames[colliderDelayInputType.enumValueIndex]) == "Key") {

                                                EditorGUILayout.PropertyField(colliderDelayKey, new GUIContent("Key"), GUILayout.MaxWidth(250));

                                            }
                                            else {

                                                EditorGUILayout.PropertyField(colliderDelayButton, new GUIContent("Button"), GUILayout.MaxWidth(250));

                                            }

                                            EditorGUILayout.Space();

                                        }
                                        GUILayout.EndHorizontal();
                                        InspectorHelpBox("Collider will come on after the key has been pressed.");

                                    }
                                }
                                else {


                                    EditorGUILayout.HelpBox("Collider can not be used on this ability as it's not either a Particle or Melee type.", MessageType.Warning);
                             
                                }

                                EditorGUILayout.EndVertical();

                                #endregion

                                InspectorSectionHeader("Collider Collision Type");

                                #region AllWay 



                                InspectorVerticalBox();

                                
                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {

                                    InspectorHelpBox("Any colliders attached to the ability added manually or automatically through the above settings will collide using the types selected in the boxes below. If Particle Collison is true make sure the World Collision setting is enabled on the mainGraphic.", false);

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(onEnter);
                                    EditorGUIUtility.labelWidth = 120;
                                    EditorGUILayout.PropertyField(onStay);
                                    ResetLabelWidth();
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(onExit);
                                    EditorGUIUtility.labelWidth = 120;
                                    EditorGUILayout.PropertyField(particleCollision);
                                    ResetLabelWidth();
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space();
                                    InspectorHelpBox("Settings below can be used to enable collisions after a key has been pressed, useful for setting up remote bomb type abilities.", false);
                                    EditorGUIUtility.labelWidth = 240;
                                    EditorGUILayout.PropertyField(enableCollisionAfterKeyPress);

                                    if (enableCollisionAfterKeyPress.boolValue == true) {

                                        EditorGUILayout.PropertyField(enableCollisionAfterKeyInputType, new GUIContent("Input Type"), GUILayout.MaxWidth(350));

                                        if (((string)enableCollisionAfterKeyInputType.enumNames[enableCollisionAfterKeyInputType.enumValueIndex]) == "Key") {

                                            EditorGUILayout.PropertyField(enableCollisionAfterKey, new GUIContent("Key"), GUILayout.MaxWidth(350));
                                            
                                        }
                                        else {

                                            EditorGUILayout.PropertyField(enableCollisionAfterKeyButton, new GUIContent("Button"), GUILayout.MaxWidth(350));

                                        }

                                    }
                                    ResetLabelWidth();


                                }
                                else {


                                    EditorGUILayout.HelpBox("Collider can not be used on this ability as it's not either a Particle or Melee type.", MessageType.Warning);

                                }

                                EditorGUILayout.EndVertical();

                                #endregion



                                break;

                            case 1:

                                InspectorSectionHeader("Settings");

                                #region AllWay 

                                EditorGUILayout.BeginHorizontal();

                                #region Collision Settings

                                InspectorVerticalBox();

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {


                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 200;
                                    EditorGUILayout.PropertyField(affectOriginObject, new GUIContent("Collide w/ Activating Entity"));
                                    ResetLabelWidth();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(abilityCollisionIgnores, new GUIContent("Ability Collision"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();
                                    InspectorHelpBox("How does the ability behave when colliding against other abilities and can it collide with the activating entity.");
                                    ResetLabelWidth();


                                    if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile") {

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(chooseLayer, new GUIContent("Add to Layer"));
                                        if (chooseLayer.boolValue == true) {
                                            EditorGUILayout.PropertyField(abLayer, new GUIContent("Layer"), GUILayout.MaxWidth(250));
                                        }
                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndHorizontal();


                                    }


                                    if (((string)impactDestroy.enumNames[impactDestroy.enumValueIndex]) != "DestroyOnTerrainOnly") {
                                        EditorGUILayout.PropertyField(ignoreActiveTerrain, new GUIContent("Ignore Terrain"));
                                    }
    

                                }

                                EditorGUIUtility.labelWidth = 200;
                                EditorGUILayout.PropertyField(overrideIgnoreAbilityCollision);
                         
                                InspectorHelpBox("If the entity is currently in ignore ability collision mode then this setting if true, will override that and still allow the ability to collide");

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {
                                    EditorGUILayout.PropertyField(overrideWeaponParrying);
                                }
                                EditorGUILayout.PropertyField(overrideWeaponBlocking);

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {
                                    InspectorHelpBox("If ticked then the ability will ignore any weapon blocking and/or parrying");
                                } else {
                                    InspectorHelpBox("If ticked then the ability will ignore weapon blocking");
                                }

                                if (overrideWeaponBlocking.boolValue == false) {
                                    EditorGUILayout.PropertyField(reduceWeaponBlockDurability);

                                    InspectorHelpBox("If ticked then the block durability will be decreased on the entity that blocks this ability");
                                }


                                EditorGUILayout.PropertyField(activateAnimationFromHit);

                                InspectorHelpBox("If true then then this will make the entity animate from this ability hitting if entity is setup to animate from hits");


                                if (activateAnimationFromHit.boolValue == true) {

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(activateSpecificHitAnimation); 
                                    EditorGUIUtility.labelWidth = 170;
                                    if (activateSpecificHitAnimation.boolValue == true) {
                                        EditorGUILayout.PropertyField(hitAnimationToActivate, GUILayout.MaxWidth(340));
                                    }
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();

                                    InspectorHelpBox("If ticked then then the specific animation entered will activate, If animation not found then a random hit animation will play instead");
                                }

                                ResetLabelWidth();
                                EditorGUILayout.Space();

                                InspectorListBox("Ability Collide Required Tags", abilityRequiredTag);
                                EditorGUILayout.Space();
                                InspectorListBox("Ability Collide Ignore Tags", abilityIgnoreTag);




                                EditorGUILayout.EndVertical();

                                #endregion

                                EditorGUILayout.EndHorizontal();

                                #endregion

                         

                                InspectorSectionHeader("Destroy Settings");
                            

                                #region All Way 

                                EditorGUILayout.BeginHorizontal();

                                #region Collision Settings 2

                                InspectorVerticalBox();

                                InspectorHelpBox("Settings below define the rules on destroying the ability on collision", false, true);

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {

                                    EditorGUILayout.PropertyField(destroyDelay, GUILayout.MaxWidth(180));
                                    EditorGUILayout.Space();

                                    EditorGUILayout.PropertyField(impactDestroy, GUILayout.MaxWidth(250));
                                    EditorGUILayout.Space();

                                    InspectorHelpBox("Determines what will destroy the ability. This can be used to filter out colliders which should not destroy.", false);
                                    InspectorListBox("Don't Destroy on Below Tags", destroyIgnoreTag);

                                                                       

                                    EditorGUILayout.Space();

                                }
                                else {


                                    EditorGUILayout.HelpBox("Collider can not be used on this ability as it's not either a Particle or Melee type.", MessageType.Warning);

                                }

                                EditorGUILayout.EndVertical();

                                #endregion


                                EditorGUILayout.EndHorizontal();

                                #endregion

    

                                break;
                            case 2:

                                InspectorSectionHeader("Impact Settings");

                                #region Side By Side

                                EditorGUILayout.BeginHorizontal();

                                #region BulletTime on initation Settings 

                                InspectorVerticalBox(true);

                                EditorGUIUtility.labelWidth = 210;
                                EditorGUILayout.PropertyField(modifyGameSpeedOnInitiation, new GUIContent("Modify Game Speed On Initiation"));
                                
                                EditorGUIUtility.labelWidth = 90;


                                if (modifyGameSpeedOnInitiation.boolValue == true) {

                                    EditorGUILayout.PropertyField(modifyGameSpeedOnInitiationDelay, new GUIContent("Delay"), GUILayout.MaxWidth(150));

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(modifyGameSpeedOnInitiationSpeedFactor, new GUIContent("Speed Factor"), GUILayout.MaxWidth(180));
                                    EditorGUILayout.PropertyField(modifyGameSpeedOnInitiationDuration, new GUIContent("Duration"), GUILayout.MaxWidth(180));
                                    EditorGUILayout.EndHorizontal();
                                }



                                InspectorHelpBox("Will slow down or speed up the game for a set duration when the ability collides with an entity.", false);

                                EditorGUILayout.Space();


                                EditorGUILayout.EndVertical();

                                #endregion

                                #region BulletTime on impact Settings 

                                InspectorVerticalBox(true);

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {

                                    EditorGUIUtility.labelWidth = 210;
                                    EditorGUILayout.PropertyField(modifyGameSpeedOnImpact, new GUIContent("Modify Game Speed On Impact"));
                                   
                                    EditorGUIUtility.labelWidth = 90;

                                    if (modifyGameSpeedOnImpact.boolValue == true) {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(modifyGameSpeedOnImpactSpeedFactor, new GUIContent("Speed Factor"), GUILayout.MaxWidth(180));
                                        EditorGUILayout.PropertyField(modifyGameSpeedOnImpactDuration, new GUIContent("Duration"), GUILayout.MaxWidth(180));
                                        EditorGUILayout.EndHorizontal();
                                    }

                                    InspectorHelpBox("Will slow down or speed up the game for a set duration when the ability collides with an entity.", false);

                                    EditorGUILayout.Space();



                                } else {

                                    EditorGUILayout.HelpBox("Impact Settings can not be used on this ability as it's not either a Particle or Melee type.", MessageType.Warning);

                                }

                                EditorGUILayout.EndVertical();

                                #endregion


                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region All Way 

                                EditorGUILayout.BeginHorizontal();

                                #region Camera Shake Settings 

                                InspectorVerticalBox();

                                //InspectorSectionHeader("Collision Settings", "Settings below define collision settings");

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {

                              

                                    EditorGUILayout.BeginHorizontal();

                                    EditorGUIUtility.labelWidth = 130;
                                    EditorGUILayout.PropertyField(shakeCameraOnImpact, new GUIContent("Shake Camera"));

                                    if (shakeCameraOnImpact.boolValue == true) {

                                        EditorGUILayout.PropertyField(shakeCameraOnImpactDuration, new GUIContent("Duration"), GUILayout.MaxWidth(250));
                                        EditorGUILayout.Space();
                                        ResetLabelWidth();
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 130;
                                        EditorGUILayout.PropertyField(shakeCameraOnImpactAmount, new GUIContent("Shake Amount"), GUILayout.MaxWidth(250));


                                        EditorGUILayout.PropertyField(shakeCameraOnImpactSpeed, new GUIContent("Shake Speed"), GUILayout.MaxWidth(250));
                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndHorizontal();
                                    
                                        ResetLabelWidth();
                                    } else {
                                        EditorGUILayout.EndHorizontal();
                                    }

                                    InspectorHelpBox("Will shake the camera by the amount, speed and duration provided when the ability collides with an entity.", false);

                                }
                                else {


                                    EditorGUILayout.HelpBox("Impact Settings can not be used on this ability as it's not either a Particle or Melee type.", MessageType.Warning);

                                }

                                EditorGUILayout.EndVertical();

                                #endregion


                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region All Way 

                                EditorGUILayout.BeginHorizontal();

                                #region Explosion/Splash Settings 

                                InspectorVerticalBox();

        

                                if (((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Projectile" || ((string)abilityType.enumNames[abilityType.enumValueIndex]) == "Melee") {


                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(useDestroySplashEffect, new GUIContent("Splash Effects"));
                                    if (useDestroySplashEffect.boolValue == true) {
                                        EditorGUILayout.PropertyField(destroySplashRadius, new GUIContent("Radius"), GUILayout.MaxWidth(180));
                                        EditorGUILayout.Space();
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    InspectorHelpBox("Will apply effects to entities in a set radius when the ability is destroyed.");


                                    EditorGUIUtility.labelWidth = 130;
                                    EditorGUILayout.PropertyField(destroySplashExplosion, new GUIContent("Splash Explosion"));
                                   
                                    if (destroySplashExplosion.boolValue == true) {
                                        
                                        EditorGUILayout.PropertyField(destroySplashExplosionRadius, new GUIContent("Explosion Radius"), GUILayout.MaxWidth(250));
                                        EditorGUILayout.Space();
                                        ResetLabelWidth();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 130;
                                        EditorGUILayout.PropertyField(destroySplashExplosionPower, new GUIContent("Explosion Power"), GUILayout.MaxWidth(250));
                                        
                                        
                                        EditorGUILayout.PropertyField(destroySplashExplosionUplift, new GUIContent("Uplift Power"), GUILayout.MaxWidth(250));
                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndHorizontal();
                                 
                                     
                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 150;
                                        EditorGUILayout.PropertyField(destroySplashExplosionTagLimit, new GUIContent("Only Explode on Tags"));
                                        ResetLabelWidth();

                                        if (destroySplashExplosionTagLimit.boolValue == true) {
                                            InspectorListBox("Only Explode on following tags", destroySplashExplosionAffectTag);
                                        }
                                    }
                                    InspectorHelpBox("Will add an explosive force to rigidbodies in a set radius when the ability is destroyed.");

                                    EditorGUIUtility.labelWidth = 180;
                                    EditorGUILayout.PropertyField(attachToObjectOnImpact, new GUIContent("Attach To Object On Impact"));

                                    EditorGUIUtility.labelWidth = 130;
                                    if (attachToObjectOnImpact.boolValue == true){

                                        EditorGUILayout.PropertyField(attachToObjectProbabilityMinValue, new GUIContent("Probability Min"));
                                        EditorGUILayout.PropertyField(attachToObjectProbabilityMaxValue, new GUIContent("Probability Max"));

                                        EditorGUILayout.BeginHorizontal();
                                      
                                        EditorGUILayout.PropertyField(attachToObjectStickOutFactor, new GUIContent("Stick Out Factor"), GUILayout.MaxWidth(180));
                                        EditorGUIUtility.labelWidth = 140;
                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(attachToObjectNearestBone, new GUIContent("Attach to Nearest Bone"));
                                        EditorGUILayout.EndHorizontal();
                                        
                                    }

                                    InspectorHelpBox("If enabled then ability on impact will attach to the object until duration is up");


                                }
                                else {


                                    EditorGUILayout.HelpBox("Impact Settings can not be used on this ability as it's not either a Particle or Melee type.", MessageType.Warning);

                                }

                                EditorGUILayout.EndVertical();

                                #endregion


                                EditorGUILayout.EndHorizontal();

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

                        #region Controls/ability info
                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(abilityInfoWidth));

                        InspectorToolbarAndAbilityInfo(ref animationSettingsToolbarSelection, ref animationSettingsToolbar);

                        EditorGUILayout.EndVertical();

                        #endregion

                        EditorGUIUtility.labelWidth = 160;

                        InspectorBoldVerticleLine();

                        #region Aesthetic & Animation 




                        editorScrollPos = EditorGUILayout.BeginScrollView(editorScrollPos,
                                                                     false,
                                                                     false);

                        EditorGUILayout.BeginVertical();

                        #region Settings

                        switch ((int)animationSettingsToolbarSelection) {
                            case 0:



                                #region AllWay 


                                InspectorVerticalBox();


                                if (prepareTime.floatValue <= 0) {

                                    EditorGUILayout.HelpBox("Ability is not current set to prepare. To enable this add a prepare time in the General tab.", MessageType.Warning);

                                }
                                else {
                                    // what to play Aesthetically when iniating ability 
                                    EditorGUILayout.PropertyField(usePreparingAesthetics);
                                    InspectorHelpBox("If disabled then none of the below will come into effect when preparing.", false);
                                }

                                EditorGUILayout.EndVertical();



                                #endregion

                                if (usePreparingAesthetics.boolValue == true) {

                                    InspectorSectionHeader("Animation & Movement");

                                    #region SideBySide 

                                    EditorGUILayout.BeginHorizontal();

                                    EditorGUILayout.BeginVertical();

                                    #region Preparing Animation Runner 

                                    InspectorVerticalBox(true);

                                    EditorGUILayout.Space();

                                    ResetLabelWidth();
                                    EditorGUILayout.PropertyField(preparingAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                    if (preparingAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null)
                                    {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(preparingAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(preparingAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(preparingAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(preparingAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(preparingAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();

                                    }

                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #region Preparing Animation 

                                    InspectorVerticalBox(true);


                                    if (prepareTime.floatValue <= 0) {

                                        EditorGUILayout.HelpBox("Ability is not current set to prepare. To enable this add a prepare time in the General tab.", MessageType.Warning);

                                    }
                                    else {

                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 145;
                                        EditorGUILayout.PropertyField(preparingAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));
                                        InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer preparing");

                                        if (preparingAnimatorParameter.stringValue != "") {

                                            EditorGUILayout.PropertyField(preparingAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));
                                          

                                            EditorGUILayout.Space();

                                            EditorGUIUtility.labelWidth = 225;
                                            EditorGUILayout.PropertyField(preparingAnimateOnEntity, new GUIContent("Animate on Entity"));
                                            EditorGUILayout.PropertyField(preparingAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                            EditorGUILayout.PropertyField(preparingAnimateOnWeapon, new GUIContent("Animate on Weapon"));

                                            InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");


                                            EditorGUIUtility.labelWidth = 150;


                                            if (((string)preparingAnimatorParameterType.enumNames[preparingAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                                //EditorGUILayout.BeginHorizontal();
                                                // if not trigger we need to know the value to switch on and off
                                                EditorGUILayout.PropertyField(preparingAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                                EditorGUILayout.PropertyField(preparingAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));                                           

                                            }
                                            
                                        }



                                        EditorGUILayout.Space();

                                    }

                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    EditorGUILayout.EndVertical();

                                    #region Preparing Movement

                                    InspectorVerticalBox(true);



                                    if (prepareTime.floatValue <= 0) {

                                        EditorGUILayout.HelpBox("Ability is not current set to prepare. To enable this add a prepare time in the General tab.", MessageType.Warning);

                                    }
                                    else {



                                        EditorGUIUtility.labelWidth = 210;
                                        EditorGUILayout.PropertyField(stopMovementOnPreparing, new GUIContent("Restrict Movement When Preparing"));
                                        if (stopMovementOnPreparing.boolValue == true) {

                                            ResetLabelWidth();
                                            EditorGUILayout.PropertyField(stopMovementOnPreparingDuration, new GUIContent("Duration"), GUILayout.MaxWidth(180));

                                            EditorGUILayout.BeginHorizontal();
                                           
                                            EditorGUILayout.PropertyField(stopMovementOnPreparingFreezePosition, new GUIContent("Freeze Position"));
                                            EditorGUIUtility.labelWidth = 140;
                                            EditorGUILayout.PropertyField(stopMovementOnPreparingDisableComponents, new GUIContent("Disable Components"));
                                            EditorGUILayout.EndHorizontal();
                                        }


                                        InspectorHelpBox("If enabled then entity will not be able to move when preparing, if 0 duration then movement will enable after preparation animation ends", false);

                                        EditorGUIUtility.labelWidth = 200;
                                        EditorGUILayout.PropertyField(stopMovementOnPreparingRaiseEvent, new GUIContent("Raise Restrict Movement Event"));

                                        ResetLabelWidth();
                                        if (stopMovementOnPreparingRaiseEvent.boolValue == true){
                                            EditorGUILayout.PropertyField(stopMovementOnPreparingDuration, new GUIContent("Duration"), GUILayout.MaxWidth(180));
                                        }

                                        InspectorHelpBox("If enabled then a restrict movement event delegate will be invoked to let linked scripts know to prevent movement. if 0 duration then the enable movement event will invoke after preparing animation ends");

                                        ResetLabelWidth();


                                        EditorGUIUtility.labelWidth = 190;
                                        EditorGUILayout.PropertyField(moveSelfWhenPreparing);
                                        ResetLabelWidth();

                                        if (moveSelfWhenPreparing.boolValue == true) {

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("Move Offset", GUILayout.MaxWidth(100));

                                            EditorGUILayout.PropertyField(moveSelfPreparingOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.Space();

                                            ResetLabelWidth();
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(moveSelfPreparingForwardOffset, new GUIContent("Forward Offset"));
                                            EditorGUIUtility.labelWidth = 90;
                                            EditorGUILayout.PropertyField(moveSelfPreparingRightOffset, new GUIContent("Right Offset"));
                                            ResetLabelWidth();
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.Space();

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(moveSelfPreparingDelay, new GUIContent("Start Delay"));
                                            EditorGUIUtility.labelWidth = 90;
                                            EditorGUILayout.PropertyField(moveSelfPreparingDuration, new GUIContent("Duration"));
                                            ResetLabelWidth();
                                            EditorGUILayout.EndHorizontal();

                                        }
                                        InspectorHelpBox("Move entity when preparing. Useful if animation has no rootmotion. Entity will move by an offset given below. ");

                                        EditorGUIUtility.labelWidth = 210;
                                        EditorGUILayout.PropertyField(moveSelfToTargetWhenPreparing, new GUIContent("Move To Target When Preparing"));

                                        if (moveSelfToTargetWhenPreparing.boolValue == true) {

                                            ResetLabelWidth();

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));

                                            EditorGUILayout.PropertyField(moveSelfToTargetPreparingOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.Space();

                                            ResetLabelWidth();
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(moveSelfToTargetPreparingForwardOffset, new GUIContent("Forward Offset"));
                                            EditorGUIUtility.labelWidth = 90;
                                            EditorGUILayout.PropertyField(moveSelfToTargetPreparingRightOffset, new GUIContent("Right Offset"));
                                            ResetLabelWidth();
                                            EditorGUILayout.EndHorizontal();

                                            EditorGUILayout.Space();

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(moveSelfToTargetPreparingDelay, new GUIContent("Start Delay"));
                                            EditorGUIUtility.labelWidth = 90;
                                            EditorGUILayout.PropertyField(moveSelfToTargetPreparingDuration, new GUIContent("Duration"));

                                            EditorGUILayout.EndHorizontal();
                                            EditorGUIUtility.labelWidth = 110;
                                            EditorGUILayout.PropertyField(moveSelfToTargetPreparingStopDistance, new GUIContent("Stop Distance"), GUILayout.MaxWidth(160));
                                            EditorGUIUtility.labelWidth = 230;
                                            EditorGUILayout.PropertyField(moveSelfToTargetActivatePreparingAnimationOnlyWhenMoving, new GUIContent("Only Activate Animation When Moving"));
                                            ResetLabelWidth();
                                        }

                                        InspectorHelpBox("Moves entity to the ability target during preparation. If ability doesn't use targets (Forward Travel Type etc) then entity will move towards the" +
                                            " current target or soft target", false);


                                        EditorGUILayout.Space();

                                        ResetLabelWidth();
                                        EditorGUILayout.PropertyField(defyGravityPreparing, new GUIContent("Defy Gravity"));
                                        if (defyGravityPreparing.boolValue == true) {

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(defyGravityPreparingDuration, new GUIContent("Defy Duration"), GUILayout.MaxWidth(230));
                                            EditorGUILayout.PropertyField(defyGravityPreparingRaiseEvent, new GUIContent("Raise Event"), GUILayout.MaxWidth(230));
                                            EditorGUILayout.EndHorizontal();
                                          

                                        }
                             
                                        InspectorHelpBox("If enabled then when preparing the ability the activating entity will defy gravity for a duration set when preparing. Like floating when charging an attack");

                                    }

                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    EditorGUILayout.EndHorizontal();

                                    #endregion

                                    InspectorSectionHeader("Graphic");

                                    #region AllWay 

                                    #region Preparing Graphic 

                                    InspectorVerticalBox();

                                    InspectorHelpBox("Settings below for the graphic to show when preparing.");

                                    if (prepareTime.floatValue <= 0) {

                                        EditorGUILayout.HelpBox("Ability is not current set to prepare. To enable this add a prepare time in the General tab.", MessageType.Warning);

                                    }
                                    else {


                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(preparingParticle, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                        InspectorHorizontalSpace(65);
                                        EditorGUILayout.PropertyField(preparingObject, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(preparingStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));
                                        ResetLabelWidth();
                                        InspectorHorizontalSpace(165);
                                        if (((string)preparingStartPosition.enumNames[preparingStartPosition.enumValueIndex]) == "OnObject") {
                                            EditorGUILayout.PropertyField(preparingPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                        }

                                        if (((string)preparingStartPosition.enumNames[preparingStartPosition.enumValueIndex]) == "OnTag" || ((string)preparingStartPosition.enumNames[preparingStartPosition.enumValueIndex]) == "OnSelfTag") {
                                            EditorGUILayout.LabelField("Select Tag");
                                            preparingPositionOnTag.stringValue = EditorGUILayout.TagField(preparingPositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.EndHorizontal();


                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 190;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(preparingAestheticDurationUsePrepareTime, new GUIContent("Use Prepare Time Duration"), GUILayout.MaxWidth(230));
                                        if (preparingAestheticDurationUsePrepareTime.boolValue == false){
                                            EditorGUIUtility.labelWidth = 130;
                                            EditorGUILayout.PropertyField(preparingAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                        }
                                        ResetLabelWidth();
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(preparingAestheticsPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(preparingAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(preparingAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();


                                        EditorGUILayout.Space();

                                    }

                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #endregion

                                }

                                break;

                            case 1:

                                #region AllWay 


                                InspectorVerticalBox();


                                // what to play Aesthetically when iniating ability 
                                EditorGUILayout.BeginHorizontal();
                                EditorGUIUtility.labelWidth = 200;
                                EditorGUILayout.PropertyField(useInitiatingAesthetics);
                                EditorGUIUtility.labelWidth = 250;
                                EditorGUILayout.PropertyField(delayBetweenInitiatingAndProjectile, new GUIContent("Delay Between Initiating & Projectile"), GUILayout.MaxWidth(290));
                                ResetLabelWidth();
                                EditorGUILayout.EndHorizontal();
                                InspectorHelpBox("To aid with the duration of the initiating animation. The ability will fire after the delay time.", false);

                                EditorGUILayout.EndVertical();


                                #endregion

                                if (useInitiatingAesthetics.boolValue == true) {

                                    InspectorSectionHeader("Animation & Movement");

                                    #region SideBySide 

                                    EditorGUILayout.BeginHorizontal();

                                    EditorGUILayout.BeginVertical();

                                    #region Initiating Animation Runner 

                                    InspectorVerticalBox(true);

                                    EditorGUILayout.Space();

                                    EditorGUILayout.PropertyField(initiatingAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));
                                   
                                    if (initiatingAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null)  {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(initiatingAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(initiatingAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(initiatingAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(initiatingAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(initiatingAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.PropertyField(initiatingAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                        

                                    }

                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #region Initiating Animation 

                                    InspectorVerticalBox(true);

                                    EditorGUILayout.Space();

                                    EditorGUIUtility.labelWidth = 145;
                                    EditorGUILayout.PropertyField(initiatingAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(315));

                                    if (initiatingAnimatorParameter.stringValue != "") {

                                        InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until entity is no longer initiating");


                                        EditorGUILayout.PropertyField(initiatingAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));

                                        EditorGUILayout.Space();

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(initiatingAnimateOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(initiatingAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(initiatingAnimateOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");

                                        EditorGUIUtility.labelWidth = 150;


                                        if ((string)abilityToggle.enumNames[abilityToggle.enumValueIndex] == "Off" || (string)abilityToggle.enumNames[abilityToggle.enumValueIndex] != "Off" && (repeatInitiatingAnimationWhilstToggled.boolValue == false || canCastWhenToggled.boolValue == true)) {
                                            EditorGUILayout.PropertyField(initiatingAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));
                                        }

                                        if (((string)initiatingAnimatorParameterType.enumNames[initiatingAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                            //EditorGUILayout.BeginHorizontal();
                                            // if not trigger we need to know the value to switch on and off
                                            EditorGUILayout.PropertyField(initiatingAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));

                                            EditorGUILayout.PropertyField(initiatingAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));
                                   
                                        }
                                    }


                                    ResetLabelWidth();
                                    EditorGUILayout.Space();



                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    EditorGUILayout.EndVertical();

                                    #region initiating Movement

                                    InspectorVerticalBox(true);



                                    EditorGUIUtility.labelWidth = 230;
                                    EditorGUILayout.PropertyField(stopMovementOnInitiate, new GUIContent("Restrict Movement When Initiating"));
                                    if (stopMovementOnInitiate.boolValue == true) {
                                        ResetLabelWidth();
                                        EditorGUILayout.PropertyField(stopMovementOnInitiateDuration, new GUIContent("Duration"), GUILayout.MaxWidth(180));

                                        EditorGUILayout.BeginHorizontal();
                                       
                                        EditorGUILayout.PropertyField(stopMovementOnInitiateFreezePosition, new GUIContent("Freeze Position"));
                                        EditorGUIUtility.labelWidth = 140;
                                        EditorGUILayout.PropertyField(stopMovementOnInitiateDisableComponents, new GUIContent("Disable Components"));
                                        EditorGUILayout.EndHorizontal();

                                    }

                                    InspectorHelpBox("If enabled then entity will not be able to move when initiating. if 0 duration then movement will enable after initiation animation ends", false);

                                    EditorGUIUtility.labelWidth = 230;
                                    EditorGUILayout.PropertyField(stopMovementOnInitiateRaiseEvent, new GUIContent("Raise Restrict Movement Event"));

                                    ResetLabelWidth();
                                    if (stopMovementOnInitiateRaiseEvent.boolValue == true)
                                    {
                                        EditorGUILayout.PropertyField(stopMovementOnInitiateDuration, new GUIContent("Duration"), GUILayout.MaxWidth(180));
                                    }
                                    InspectorHelpBox("If enabled then a restrict movement event delegate will be invoked to let linked scripts know to prevent movement. if 0 duration then the enable movement event will invoke after initiation animation ends");

                                    ResetLabelWidth();

                                   

                                    EditorGUIUtility.labelWidth = 200;
                                    EditorGUILayout.PropertyField(moveSelfWhenInitiating);
                                    ResetLabelWidth();
                                    
                                    if (moveSelfWhenInitiating.boolValue == true) {

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Move Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(moveSelfInitiatingOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();

                                        ResetLabelWidth();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(moveSelfInitiatingForwardOffset, new GUIContent("Forward Offset"));
                                        EditorGUIUtility.labelWidth = 90;
                                        EditorGUILayout.PropertyField(moveSelfInitiatingRightOffset, new GUIContent("Right Offset"));
                                        ResetLabelWidth();
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(moveSelfInitiatingDelay, new GUIContent("Start Delay"));
                                        EditorGUIUtility.labelWidth = 90;
                                        EditorGUILayout.PropertyField(moveSelfInitiatingDuration, new GUIContent("Duration"));
                                        ResetLabelWidth();
                                        EditorGUILayout.EndHorizontal();

                                    }

                                    InspectorHelpBox("Move entity when initiating. Useful if animation has no rootmotion. Entity will move by an offset given below. ");


                                    EditorGUIUtility.labelWidth = 230;
                                    EditorGUILayout.PropertyField(moveSelfToTargetWhenInitiating, new GUIContent("Move To Target When Initiating"));
                                                                 
                                    if (moveSelfToTargetWhenInitiating.boolValue == true) {

                                        ResetLabelWidth();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Target Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(moveSelfToTargetInitiatingOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();

                                        ResetLabelWidth();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(moveSelfToTargetInitiatingForwardOffset, new GUIContent("Forward Offset"));
                                        EditorGUIUtility.labelWidth = 90;
                                        EditorGUILayout.PropertyField(moveSelfToTargetInitiatingRightOffset, new GUIContent("Right Offset"));
                                        ResetLabelWidth();
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(moveSelfToTargetInitiatingDelay, new GUIContent("Start Delay"));
                                        EditorGUIUtility.labelWidth = 90;
                                        EditorGUILayout.PropertyField(moveSelfToTargetInitiatingDuration, new GUIContent("Duration"));
                                       
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUIUtility.labelWidth = 110;
                                        EditorGUILayout.PropertyField(moveSelfToTargetInitiatingStopDistance, new GUIContent("Stop Distance"), GUILayout.MaxWidth(160));
                                        ResetLabelWidth();
                                    }

                                    InspectorHelpBox("Moves entity to the ability target during initiation. If ability doesn't use targets (Forward Travel Type etc) then entity will move towards the" +
                                        " current target or soft target", false);

                                    EditorGUILayout.Space();

                                    ResetLabelWidth();
                                    EditorGUILayout.PropertyField(defyGravityInitiating, new GUIContent("Defy Gravity"));
                                    if (defyGravityInitiating.boolValue == true) {

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(defyGravityInitiatingDuration, new GUIContent("Defy Duration"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(defyGravityInitiatingRaiseEvent, new GUIContent("Raise Event"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.EndHorizontal();
                  

                                    }
                            
                                    InspectorHelpBox("If enabled then when initiating the ability the activating entity will defy gravity for a duration set when preparing. Like floating when charging an attack. Requires Rigidbody.");
                                


                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    EditorGUILayout.EndHorizontal();

                                    #endregion

                                    InspectorSectionHeader("Graphic");

                                    #region AllWay 

                                    #region Initiating Graphic 

                                    InspectorVerticalBox();

                                    InspectorHelpBox("Settings below for the graphic to show when initiating.");

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(initiatingParticle, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                    InspectorHorizontalSpace(65);
                                    EditorGUILayout.PropertyField(initiatingObject, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUIUtility.labelWidth = 125;
                                    EditorGUILayout.PropertyField(initiatingStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));
                                    ResetLabelWidth();
                                    InspectorHorizontalSpace(165);
                                    if (((string)initiatingStartPosition.enumNames[initiatingStartPosition.enumValueIndex]) == "OnObject") {
                                        EditorGUILayout.PropertyField(initiatingPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                    }
                                    if (((string)initiatingStartPosition.enumNames[initiatingStartPosition.enumValueIndex]) == "OnTag" || ((string)initiatingStartPosition.enumNames[initiatingStartPosition.enumValueIndex]) == "OnSelfTag") {
                                        EditorGUILayout.LabelField("Select Tag");
                                        initiatingPositionOnTag.stringValue = EditorGUILayout.TagField(initiatingPositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                        EditorGUILayout.Space();
                                    }
                                    EditorGUILayout.EndHorizontal();


                                    EditorGUILayout.Space();
                                    EditorGUIUtility.labelWidth = 125;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(initiatingAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.PropertyField(initiatingAestheticDelay, new GUIContent("Graphic Delay"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.EndHorizontal();
                                    ResetLabelWidth();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                    EditorGUILayout.PropertyField(initiatingAestheticsPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(initiatingAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.PropertyField(initiatingAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();





                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #endregion
                                }


                      
                                break;

                            case 2:

                                #region AllWay 


                                InspectorVerticalBox();

                                // what to play Aesthetically when iniating ability 
                                    EditorGUILayout.PropertyField(useAbilityEndAesthetics);
                                    InspectorHelpBox("If disabled then none of the below will come into effect when ability ends.");

                                EditorGUILayout.Space();
                                EditorGUILayout.EndVertical();


                                #endregion

                                InspectorSectionHeader("Graphic");

                                #region AllWay 

                                if (useAbilityEndAesthetics.boolValue == true) {

                                    #region Ability End Graphic 

                                    InspectorVerticalBox();

                                    InspectorHelpBox("Graphics which will display when the ability ends e.g is destroyed or duration is up.");


                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(abEndParticle, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                    InspectorHorizontalSpace(65);
                                    EditorGUILayout.PropertyField(abEndObject, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(abEndAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));

                                    EditorGUILayout.Space();




                                    EditorGUILayout.EndVertical();

                                    #endregion
                                }

                                #endregion

                                break;


                            case 3:

                                if (prepareTime.floatValue <= 0) {
                                    InspectorVerticalBox();
                                    EditorGUILayout.HelpBox("Ability is not current set to prepare. To enable this functionality add a prepare time in the General tab.", MessageType.Warning);
                                    EditorGUILayout.EndVertical();
                                }
                                else {

                                    #region AllWay 


                                    InspectorVerticalBox();

                                    EditorGUIUtility.labelWidth = 220;
                                    EditorGUILayout.PropertyField(useProjectileToStartPosition);
                                    ResetLabelWidth();
                                    InspectorHelpBox("If disabled then projectile to start position will not come into effect when preparing.");

 
                                    EditorGUILayout.EndVertical();


                                    #endregion

                                    if (useProjectileToStartPosition.boolValue == true) {

                                        InspectorSectionHeader("Projectile To Start Position");

                                        #region AllWay 



                                        InspectorVerticalBox();



                                        InspectorHelpBox("Setup a graphic which will travel to the start position during the preparation stage. This graphic will always be destroyed just before initiating unless a duration greater then 0 is given or the original projectile graphic is used. example of this implementation could be receiving lightning from the sky before sending it onwards.");



                                        EditorGUIUtility.labelWidth = 150;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(projToStartStartingPosition, new GUIContent("Graphic Start Position"), GUILayout.MaxWidth(260));
                                        EditorGUILayout.Space();

                                        if (((string)projToStartStartingPosition.enumNames[projToStartStartingPosition.enumValueIndex]) == "OnObject") {
                                            EditorGUILayout.PropertyField(projToStartPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(300));
                                        }

                                        if (((string)projToStartStartingPosition.enumNames[projToStartStartingPosition.enumValueIndex]) == "OnTag" || ((string)projToStartStartingPosition.enumNames[projToStartStartingPosition.enumValueIndex]) == "OnSelfTag") {

                                            EditorGUILayout.LabelField("Select Tag");
                                            projToStartPositionOnTag.stringValue = EditorGUILayout.TagField(projToStartPositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                            EditorGUILayout.Space();

                                        }
                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();




                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Position Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(projToStartPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(projToStartPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(projToStartPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();


                                        EditorGUILayout.PropertyField(projToStartRotateToTarget, new GUIContent("Rotate To Target"));
                                        if (projToStartRotateToTarget.boolValue == false) {
                                            EditorGUILayout.PropertyField(projToStartRotation, new GUIContent("Starting Rotation"));
                                            EditorGUILayout.Space();
                                            EditorGUIUtility.labelWidth = 130;
                                            EditorGUILayout.PropertyField(projToStartSetEulerRotation, new GUIContent("Set Euler Rotation"));
                                            ResetLabelWidth();
                                            
                                        }



                                        EditorGUILayout.EndVertical();


                                        #endregion

                                        InspectorSectionHeader("Travel");
                                        #region AllWay 


                                        InspectorVerticalBox();

                                        InspectorHelpBox("How long graphic takes to reach the starting position. If 0 then the projectile will just stay where it spawns and not travel.", false);
                                        EditorGUILayout.Space();
                                        EditorGUIUtility.labelWidth = 140;
                                        EditorGUILayout.PropertyField(projToStartReachPositionTime, new GUIContent("Seconds To Target"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(projToStartTravelDelay, new GUIContent("Travel Delay"), GUILayout.MaxWidth(230));

                                        EditorGUILayout.PropertyField(projToStartmoveWithTarget, new GUIContent("Move With Target"));

                                        EditorGUILayout.BeginHorizontal();

                                        EditorGUILayout.PropertyField(projToStartHoverOnSpot, new GUIContent("Hover On Spot"));

                                        if (projToStartHoverOnSpot.boolValue == true) {
                                            EditorGUILayout.PropertyField(projToStartHoverDistance, new GUIContent("Hover Distance"), GUILayout.MaxWidth(230));
                                            EditorGUILayout.Space();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        ResetLabelWidth();
                                        EditorGUILayout.Space();

                                        EditorGUILayout.EndVertical();


                                        #endregion

                                        InspectorSectionHeader("Graphic");

                                        #region AllWay 

                                        #region  Graphic 

                                        InspectorVerticalBox();


                                        EditorGUIUtility.labelWidth = 150;
                                        EditorGUILayout.PropertyField(useOriginalProjectilePTS, new GUIContent("Use Original Projectile"));
                                        ResetLabelWidth();
                                        if (useOriginalProjectilePTS.boolValue == false) {
                                            EditorGUILayout.Space();
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(projToStartPosParticle, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                            InspectorHorizontalSpace(65);
                                            EditorGUILayout.PropertyField(projToStartPosObject, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.Space();
                                            EditorGUIUtility.labelWidth = 120;
                                            EditorGUILayout.PropertyField(projToStartPosDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                            ResetLabelWidth();
                                            InspectorHelpBox("If graphic duration is 0 then the graphic will be destroyed on intiating. Else it will be destroyed after the duration");
                                        }



                                        EditorGUILayout.Space();

                                        EditorGUILayout.EndVertical();

                                        #endregion

                                        #endregion
                                    }

                                }

                                break;

                            case 4:

                                #region AllWay 


                                InspectorVerticalBox();

                               
                                if (scrollAbility.boolValue == false) {

                                    EditorGUILayout.HelpBox("Ability is not currently scrollable. To enable this tick the Scroll Ability box in the General tab.", MessageType.Warning);

                                } else {

                                    EditorGUIUtility.labelWidth = 190;
                                    // what to play Aesthetically when iniating ability 
                                    EditorGUILayout.PropertyField(useScrollAbilityAesthetics);
                                    ResetLabelWidth();
                                    InspectorHelpBox("If disabled then none of the below will come into effect when activating/deactivating a scrollable ability.", false);
                                }

                                EditorGUILayout.EndVertical();


                                #endregion

                                if (useScrollAbilityAesthetics.boolValue == true) {

                                    InspectorSectionHeader("Animation");

                                    InspectorHelpBox("Settings below for when entity enables (equips) a scroll ability", false, true);

                                    #region SideBySide 

                                    EditorGUILayout.BeginHorizontal();

                                    #region Scrollable Ability Activate Animation Runner

                                    InspectorVerticalBox(true);


                                    EditorGUIUtility.labelWidth = 155;
                                    EditorGUILayout.PropertyField(scrollAbilityAnimationRunnerClip, new GUIContent("Enable Animation Clip"), GUILayout.MaxWidth(335));

                                    if (scrollAbilityAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null)
                                    {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(scrollAbilityAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(scrollAbilityAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(scrollAbilityAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(scrollAbilityAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(scrollAbilityAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.PropertyField(scrollAbilityAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));


                                    }

                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();



                                    #endregion

                                    #region Scrollable Ability Activate Animation 

                                    InspectorVerticalBox(true);

                                    if (scrollAbility.boolValue == false) {

                                        EditorGUILayout.HelpBox("Ability is not currently scrollable. To enable this tick the Scroll Ability box in the General tab.", MessageType.Warning);

                                    } else {

                                        EditorGUIUtility.labelWidth = 200;
                                        EditorGUILayout.PropertyField(scrollAbilityAnimatorParameter, new GUIContent("Enable Animator Parameter"), GUILayout.Width(300));
                                        ResetLabelWidth();
                                        if (scrollAbilityAnimatorParameter.stringValue != ""){

                                            InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values.");

                                            EditorGUILayout.PropertyField(scrollAbilityAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));
                                            EditorGUILayout.Space();
     

                                            EditorGUIUtility.labelWidth = 225;
   
                                            EditorGUILayout.PropertyField(scrollAbilityAnimateOnEntity, new GUIContent("Animate on Entity"));
                                            EditorGUILayout.PropertyField(scrollAbilityAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                            EditorGUILayout.PropertyField(scrollAbilityAnimateOnWeapon, new GUIContent("Animate on Weapon"));

                                            InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");


                                            EditorGUIUtility.labelWidth = 150;

                                            EditorGUILayout.PropertyField(scrollAbilityAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));

                                            if (((string)scrollAbilityAnimatorParameterType.enumNames[scrollAbilityAnimatorParameterType.enumValueIndex]) != "Trigger") {


                                                // if not trigger we need to know the value to switch on and off
                                                EditorGUILayout.PropertyField(scrollAbilityAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));
                                                EditorGUILayout.PropertyField(scrollAbilityAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));
                                                EditorGUILayout.Space();



                                                ResetLabelWidth();
                                                EditorGUILayout.Space();



                                            }
                                        }

                                    }


                                    EditorGUILayout.EndVertical();

                                    #endregion


                                    EditorGUILayout.EndHorizontal();
                                    #endregion

                                    InspectorHelpBox("Settings below for when entity disables (unequips) a scroll ability", false, true);

                                    #region SideBySide 

                                    EditorGUILayout.BeginHorizontal();

                                    #region Scrollable Ability Activate Animation Runner

                                    InspectorVerticalBox(true);


                                    EditorGUIUtility.labelWidth = 155;
                                    EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimationRunnerClip, new GUIContent("Disable Animation Clip"), GUILayout.MaxWidth(335));

                                    if (scrollAbilityDeactivateAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null)
                                    {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimationRunnerClipDuration, new GUIContent("Duration"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));


                                    }

                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();



                                    #endregion

                                    #region Scrollable Ability deactivate Animation 

                                    InspectorVerticalBox(true);

                                    if (scrollAbility.boolValue == false){

                                        EditorGUILayout.HelpBox("Ability is not currently scrollable. To enable this tick the Scroll Ability box in the General tab.", MessageType.Warning);

                                    } else {
                                        EditorGUIUtility.labelWidth = 200;

                                        EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimatorParameter, new GUIContent("Disable Animator Parameter"), GUILayout.Width(300));
                                        ResetLabelWidth();
                                        if (scrollAbilityDeactivateAnimatorParameter.stringValue != "") {

                                            InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values.");

                                            EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));
                                            EditorGUILayout.Space();

                                            EditorGUIUtility.labelWidth = 225;

                                            EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimateOnEntity, new GUIContent("Animate on Entity"));
                                            EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                            EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimateOnWeapon, new GUIContent("Animate on Weapon"));

                                            InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");

                                            EditorGUIUtility.labelWidth = 150;

                                            EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimatorDuration, new GUIContent("Animation Duration"), GUILayout.MaxWidth(230));

                                            if (((string)scrollAbilityDeactivateAnimatorParameterType.enumNames[scrollAbilityDeactivateAnimatorParameterType.enumValueIndex]) != "Trigger"){
 

                                                // if not trigger we need to know the value to switch on and off
                                                EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));
                                                EditorGUILayout.PropertyField(scrollAbilityDeactivateAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));
                                                EditorGUILayout.Space();



                                                ResetLabelWidth();


                                            }
                                        }
                                        
                                    }


                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    EditorGUILayout.EndHorizontal();
                                    #endregion

                                    InspectorSectionHeader("Graphic");

                                    #region AllWay 

                                    #region Scrollable Ability Activate Graphic 

                                    InspectorVerticalBox();

                                    if (scrollAbility.boolValue == false) {

                                        EditorGUILayout.HelpBox("Ability is not currently scrollable. To enable this tick the Scroll Ability box in the General tab.", MessageType.Warning);

                                    }
                                    else {

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(scrollAbilityParticle, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));
                                        InspectorHorizontalSpace(65);
                                        EditorGUILayout.PropertyField(scrollAbilityObject, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(scrollAbilityStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));
                                        ResetLabelWidth();
                                        if (((string)scrollAbilityStartPosition.enumNames[scrollAbilityStartPosition.enumValueIndex]) == "OnObject") {
                                            EditorGUILayout.PropertyField(scrollAbilityPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                        }

                                        if (((string)scrollAbilityStartPosition.enumNames[scrollAbilityStartPosition.enumValueIndex]) == "OnTag" || ((string)scrollAbilityStartPosition.enumNames[scrollAbilityStartPosition.enumValueIndex]) == "OnSelfTag") {
                                            EditorGUILayout.LabelField("Select Tag");
                                            scrollAbilityPositionOnTag.stringValue = EditorGUILayout.TagField(scrollAbilityPositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                            EditorGUILayout.Space();

                                        }
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.PropertyField(scrollAbilityGraphicActivateDelay, new GUIContent("Graphic Delay"), GUILayout.MaxWidth(250));

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        ResetLabelWidth();
                                        EditorGUILayout.PropertyField(scrollAbilityAestheticUseDuration, new GUIContent("Duration Type"), GUILayout.MaxWidth(250));
                                        ResetLabelWidth();
                                        if (((string)scrollAbilityAestheticUseDuration.enumNames[scrollAbilityAestheticUseDuration.enumValueIndex]) == "Duration") {
                                            EditorGUILayout.Space();
                                            EditorGUIUtility.labelWidth = 125;
                                            EditorGUILayout.PropertyField(scrollAbilityAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                            ResetLabelWidth();
                                            EditorGUILayout.Space();
                                            EditorGUILayout.EndHorizontal();
                                            InspectorHelpBox("If no duration is set then graphic will have an infinite duration until ability is switched over");

                                        } else {
                                          
                                            EditorGUILayout.PropertyField(scrollAbilityGraphicDeactivateDelay, new GUIContent("Inactive Delay"), GUILayout.MaxWidth(332));

                                            EditorGUILayout.EndHorizontal();

                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(scrollAbilityPersistantAestheticInactivePosition, new GUIContent("Inactive Position"), GUILayout.MaxWidth(250));

                                            if (((string)scrollAbilityPersistantAestheticInactivePosition.enumNames[scrollAbilityPersistantAestheticInactivePosition.enumValueIndex]) == "OnObject") {
                                                EditorGUILayout.PropertyField(scrollAbilityPersistantAestheticInactivePositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                            }

                                            if (((string)scrollAbilityPersistantAestheticInactivePosition.enumNames[scrollAbilityPersistantAestheticInactivePosition.enumValueIndex]) == "OnTag" || ((string)scrollAbilityPersistantAestheticInactivePosition.enumNames[scrollAbilityPersistantAestheticInactivePosition.enumValueIndex]) == "OnSelfTag") {
                                                EditorGUILayout.LabelField("Select Tag");
                                                scrollAbilityPositionOnTag.stringValue = EditorGUILayout.TagField(scrollAbilityPersistantAestheticInactivePositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                                EditorGUILayout.Space();

                                            }
                                            EditorGUILayout.EndHorizontal();

                                          
                                            InspectorHelpBox("Graphic is persistant so will always show as long as ability is enabled. Inactive position is where the graphic will be placed when the ability is not 'equipped'", false);

                                        }

                                        
                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(scrollAbilityAestheticsPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(scrollAbilityAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(scrollAbilityAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();

                                    }


                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #endregion
                                }



                                break;


                            case 5:

                                #region AllWay 


                                InspectorVerticalBox();

                                if (scrollAbility.boolValue == true && useReload.boolValue == true && UseAmmo.boolValue == true && useEquippedWeaponAmmo.boolValue == false) {

                                    EditorGUIUtility.labelWidth = 210;
                                    EditorGUILayout.PropertyField(useReloadAbilityAesthetics);
                                    ResetLabelWidth();
                                    InspectorHelpBox("If disabled then none of the below will come into effect when reloading a scrollable ability.");
                                    

                                } else {

                                    EditorGUILayout.HelpBox("Ability is not currently setup to reload. To enable reloading please make sure ability is scrollable, not using equipped weapon ammo and both 'Use Reload' and 'Use Ammo' are ticked in the General tab.", MessageType.Warning);

                                }

                                EditorGUILayout.EndVertical();


                                #endregion

                                if (useReloadAbilityAesthetics.boolValue == true && scrollAbility.boolValue == true && useReload.boolValue == true && UseAmmo.boolValue == true && useEquippedWeaponAmmo.boolValue == false) {

                                    InspectorSectionHeader("Animation");

                                 

                                    #region AllWay 

                                    #region Reloading Animation Runner 

                                    InspectorVerticalBox();

                                    InspectorHelpBox("Settings below for when entity reloads a scrollable ability", false);

                                    EditorGUILayout.PropertyField(reloadAbilityAnimationRunnerClip, new GUIContent("Animation Clip"), GUILayout.MaxWidth(315));

                                    if (reloadAbilityAnimationRunnerClip.FindPropertyRelative("refVal").objectReferenceValue != null)
                                    {
                                        InspectorHelpBox("Select an animation clip to play, the duration, speed and delay. The clip is played using the ABC animation runner and does not use Unity's Animator.");

                                        EditorGUIUtility.labelWidth = 225;
                                        EditorGUILayout.PropertyField(reloadAbilityAnimationRunnerOnEntity, new GUIContent("Animate on Entity"));
                                        EditorGUILayout.PropertyField(reloadAbilityAnimationRunnerOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                        EditorGUILayout.PropertyField(reloadAbilityAnimationRunnerOnWeapon, new GUIContent("Animate on Weapon"));

                                        InspectorHelpBox("Determines if the animation clip is run on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own animation");

                                        EditorGUIUtility.labelWidth = 75;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(reloadAbilityAnimationRunnerClipDelay, new GUIContent("Delay"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.PropertyField(reloadAbilityAnimationRunnerClipSpeed, new GUIContent("Speed"), GUILayout.MaxWidth(125));
                                        EditorGUILayout.EndHorizontal();

                                        


                                    }

                                    ResetLabelWidth();
                                    EditorGUILayout.EndVertical();

                                    #endregion

                                    #endregion

                                    #region AllWay 

                                    #region Reload Animation 

                                    InspectorVerticalBox();

                                    

                                    EditorGUIUtility.labelWidth = 150;

                                    if (scrollAbility.boolValue == true && useReload.boolValue == true && UseAmmo.boolValue == true) {

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(reloadAbilityAnimatorParameter, new GUIContent("Animator Parameter"), GUILayout.MaxWidth(280));
                                        if (reloadAbilityAnimatorParameter.stringValue != "") {

                                            EditorGUILayout.Space();
                                            EditorGUILayout.PropertyField(reloadAbilityAnimatorParameterType, new GUIContent("Parameter Type"), GUILayout.MaxWidth(250));
                                            EditorGUILayout.Space();
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.Space();

                                            EditorGUIUtility.labelWidth = 225;
                                            EditorGUILayout.PropertyField(reloadAbilityAnimateOnEntity, new GUIContent("Animate on Entity"));
                                            EditorGUILayout.PropertyField(reloadAbilityAnimateOnScrollGraphic, new GUIContent("Animate on Scroll Ability Graphic"));
                                            EditorGUILayout.PropertyField(reloadAbilityAnimateOnWeapon, new GUIContent("Animate on Weapon"));

                                            InspectorHelpBox("Determines if the animation command is sent on either the entity, current scroll graphic, weapon or all of them. Animate on graphic/weapon should be used if the graphic object has it's own  animator/animation");

                                            EditorGUIUtility.labelWidth = 150;

                                            if (((string)reloadAbilityAnimatorParameterType.enumNames[reloadAbilityAnimatorParameterType.enumValueIndex]) != "Trigger") {
                                                EditorGUILayout.BeginHorizontal();
                                                // if not trigger we need to know the value to switch on and off
                                                EditorGUILayout.PropertyField(reloadAbilityAnimatorOnValue, new GUIContent("On Value"), GUILayout.MaxWidth(230));
                                                EditorGUILayout.Space();
                                                EditorGUILayout.PropertyField(reloadAbilityAnimatorOffValue, new GUIContent("Off Value"), GUILayout.MaxWidth(230));
                                                EditorGUILayout.Space();
                                                EditorGUILayout.EndHorizontal();


                                            }
                                            InspectorHelpBox("Enter in the name of the animation in your animator. Then the parameter type and the start and stop values. Note: Animation will keep repeating until reload duration is up.");
                                        }
                                        else {
                                            EditorGUILayout.EndHorizontal();
                                        }

                                    }
                                    else {

                                        EditorGUILayout.HelpBox("Ability is not currently setup to reload. To enable reloading please make sure ability is scrollable and both 'Use Reload' and 'Use Ammo' are ticked in the General tab.", MessageType.Warning);

                                    }


                                    EditorGUILayout.EndVertical();

                                    ResetLabelWidth();

                                    #endregion

                                    #endregion

                                    InspectorSectionHeader("Graphic");

                                    #region AllWay 

                                    #region Reload Graphic 

                                    InspectorVerticalBox();

                                    InspectorHelpBox("Settings below for the graphic to show when scrollable ability reloads");

                                    ResetLabelWidth();
                                    if (scrollAbility.boolValue == true && useReload.boolValue == true && UseAmmo.boolValue == true) {

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(reloadAbilityParticle, new GUIContent("Main Graphic"), GUILayout.MaxWidth(350));

                                        EditorGUILayout.PropertyField(reloadAbilityObject, new GUIContent("Sub Graphic"), GUILayout.MaxWidth(350));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(reloadAbilityStartPosition, new GUIContent("Graphic Position"), GUILayout.MaxWidth(250));
                                        ResetLabelWidth();

                                        if (((string)reloadAbilityStartPosition.enumNames[reloadAbilityStartPosition.enumValueIndex]) == "OnObject") {
                                            EditorGUILayout.PropertyField(reloadAbilityPositionOnObject, new GUIContent("Select Object"), GUILayout.MaxWidth(350));
                                        }

                                        if (((string)reloadAbilityStartPosition.enumNames[reloadAbilityStartPosition.enumValueIndex]) == "OnTag" || ((string)reloadAbilityStartPosition.enumNames[reloadAbilityStartPosition.enumValueIndex]) == "OnSelfTag") {
                                            EditorGUILayout.LabelField("Select Tag");
                                            reloadAbilityPositionOnTag.stringValue = EditorGUILayout.TagField(reloadAbilityPositionOnTag.stringValue, GUILayout.MaxWidth(150));
                                            EditorGUILayout.Space();
                                        }
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUIUtility.labelWidth = 125;
                                        EditorGUILayout.PropertyField(reloadAbilityAestheticDuration, new GUIContent("Graphic Duration"), GUILayout.MaxWidth(230));
                                        ResetLabelWidth();
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Graphic Offset", GUILayout.MaxWidth(100));

                                        EditorGUILayout.PropertyField(reloadAbilityAestheticsPositionOffset, new GUIContent(""), GUILayout.MaxWidth(500));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(reloadAbilityAestheticsPositionForwardOffset, new GUIContent("Forward Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.PropertyField(reloadAbilityAestheticsPositionRightOffset, new GUIContent("Right Offset"), GUILayout.MaxWidth(230));
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.Space();
                                        ResetLabelWidth();

                                    }
                                    else {

                                        EditorGUILayout.HelpBox("Ability is not currently setup to reload. To enable reloading please make sure ability is scrollable and both 'Use Reload' and 'Use Ammo' are ticked in the General tab.", MessageType.Warning);

                                    }



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

                        ResetLabelWidth();

                        break;

                }

                GUI.backgroundColor = inspectorBackgroundColor;

            }


            EditorGUILayout.EndVertical();
            #endregion


            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();

            #endregion


            EditorGUILayout.EndHorizontal();

            #endregion




        }

    }


    public void OnEnable() {
        //Collect any icons 
        toolbarABC = new GUIContent[] { new GUIContent(" General", Resources.Load("ABC-EditorIcons/General", typeof(Texture2D)) as Texture2D, " General Settings"),
        new GUIContent(" Position & Travel", Resources.Load("ABC-EditorIcons/Travel", typeof(Texture2D)) as Texture2D, "Position & Travel"),
        new GUIContent(" Collision & Impact", Resources.Load("ABC-EditorIcons/Collision", typeof(Texture2D)) as Texture2D, "Collider & Collision"),
        new GUIContent(" Aesthetic & Animation", Resources.Load("ABC-EditorIcons/Animation", typeof(Texture2D)) as Texture2D, "Aesthetic & Animation")};

  

        AddIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Add");
        SortIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Sort");
        RemoveIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Remove");
        CopyIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Copy");
        ExportIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Export");
        ImportIcon = (Texture2D)Resources.Load("ABC-EditorIcons/Import"); 

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
        

            //Retrieve all properties of current ability
            this.GetProperties(abilityCont.CurrentAbility);


            //Will update values in editor at runtime
            if (abilityCont.updateEditorAtRunTime == true) {
                Repaint();
            }
        }






    }
}