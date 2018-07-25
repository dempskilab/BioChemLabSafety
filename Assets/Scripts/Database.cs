using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour {

    public static Database instance;
    public List<Equipment> Equipment_List = new List<Equipment>();
    public List<Event> Event_List = new List<Event>();
    /* Audio Elements
     * 0: Instruction; 
     * 1: Trainee Instruction First; 
     * 2: Trainee Instruction Second; 
     * 3: Trainer Instruction First; 
     * 4: Trainer INsturction Second; 
     * 5: Conclusion;
     * 6: Instruction Attention
    */
    public List<AudioClip> Audio_List = new List<AudioClip>();
    public List<GameObject> Effect_list = new List<GameObject>();
    public Dictionary<string, Equipment> Equip_Map = new Dictionary<string, Equipment>();
    public Dictionary<string, Event> Event_Map = new Dictionary<string, Event>();
    public string[] numbers = new string[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
    // Use this for initialization
    void Start () {
        instance = this;
		foreach(Equipment temp in Equipment_List)
            Equip_Map.Add(temp.name, temp);
        foreach (Event temp in Event_List)
            Event_Map.Add(temp.name, temp);
    }
	
}

public class Node
{
    public string name;
    public string description;
    public int id;
}

[System.Serializable]
public class Equipment : Node
{
    public GameObject narrative;
    public Color32 color;
    public string colorString;
    public MovieTexture video;
    public AudioClip audio;
}

[System.Serializable]
public class Event : Node
{
    public GameObject effect;
}
