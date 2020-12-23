using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;
using System; 

public class ABC_ExportedAbilities : ScriptableObject {

	// if help boxes are shown in editor 
	public bool showHelpInformation = true; 

	public string exportDescription; 
	public string creationDate = System.DateTime.Now.ToString(); 
	public string createdBy = Environment.UserName.ToString();

	public List <ABC_Ability> ExportedAbilities = new List<ABC_Ability>();
	
}
