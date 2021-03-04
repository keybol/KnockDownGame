using System.Collections.Generic;

namespace Lexic
{
    //A dictionary containing syllables that will form semi-historical names of kings, along with their titles. Fits any "alternate-history" RPG or strategy game.
    public class OnlineNames : BaseNames
    {
        private static Dictionary<string, List<string>> syllableSets = new Dictionary<string, List<string>>()
            {
                {
                    "firstnames",    new List<string>(){
                                                    "Alexander", "August", "Mike",
                                                    "Henry", "John", "Louis",
                                                    "Sigmund", "Steph", "Stephen",
                                                    "Wenceslaus", "Edward", "Alfred", "Charles",
                                                    "Edga", "Harold", "William", "Richard", "Philip",
                                                    "James", "Dagobert", "Theuderic", "Robert",
                                                    "Rudolf", "Lothar", "Hugo", "Francis", 
                                                    "Edmund", "Ragnvald", "Magnus", "Albert", 
                                                    "Sigmund", "Gustav", "Frederick", "Oscar",
                                                    "Lech", "Boleslaw", "Flaming", "Burning", "Electric",
													"Ice", "Dark","Death",
													"Light", "Heavy",
													"Poisoned", "Toxic", "Druid",
													"Priest", "Warrior", "Archer",
													"Wizard", "Dragon", 
													"Divine", "Enchanted", "Magic",
													"Barbarian", "Gladiator", "Unholy",
													"Holy", "Cursed", "Blessed", "Silver",
													"Iron", "Steel", "Golden", "Cutie",
													"Skeletal", "Bone", "Soul", "Master",
													"Frost", "Storm", "Thunder", "Fierce",
													"Lucky","Axe","Great", "Dagger", 
													"Zweihander", "Clay",
													"Crossbow", "Mace", "Hammer", "Lucerne",
													"Katana", "Bludgeon",
													"Morningstar", "Flail", "Spellblade", "Knife",
													"Reaver", "Swiftblade", "Saber", "Rapier",
													 "Shiv", "Dirk",
													"Spike", "Lance", 
													}
                },
                {
                    "numbers",   new List<string>(){
                                                    "69", "123", "888", "23", "6", "7", "9",
                                                    "111", "88", "007", "143", "222", "81",
                                                    "2000", "3000", "777", "333"
                                                    }
                },
                {
                    "titles",      new List<string>(){
												   "Guy","Gal","Maul","Scythe","Blade","Bathory", "Herman", "Jogaila", "Lambert", 
                                                    "Tanglefoot", "theBearded", "theBlack", "theBold", "theBrave",
                                                    "theChaste", "theCurly", "theExile", "theGreat",
                                                    "theJust", "theOld", "thePious", "theRestorer", "theSaxon",
                                                    "theStrong", "theWhite", "Vasa", "Wrymouth", "theElder",
                                                    "thePeaceful", "theMartyr", "theUnready", "Forkbeard", "Ironside",
                                                    "Harefoot", "theConfessor", "theYoung", "Victor", "theOld",
                                                    "theRed", "theYounger", "theLame", "Barnlock", 
                                                    "theTyrant", "Bourbon", "Savoy", "Habsburg"
                                                    }
                },
            };

        private static List<string> rules = new List<string>()
            {
                "%100firstnames%100numbers", "%100firstnames%50numbers%100titles", "%100firstnames%100titles",
            };

        public new static List<string> GetSyllableSet(string key) { return syllableSets[key]; }

        public new static List<string> GetRules() { return rules; }
    }
}
