using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterModel", menuName = "Character")]
public class CharacterModel : ScriptableObject
{
	public GameObject[] characterModel;
	public string[] Name;
	public float HP;
	public float Speed;
	public float Power;
	public float Special;
	public enum Gauntlets { Bomb, Speedrun, Seeker, Glitch, Lag, Fansign, Raid, Hunter, Shoutcast, Hurricane, Hail, Star, Magic, Poison, Dwarf, Giant, Death, Meteor}
	public Gauntlets Gauntlet = Gauntlets.Bomb;
	public float armLength;
	public string winAnimation = "Test";
	public string poseAnimation = "Test";
}
