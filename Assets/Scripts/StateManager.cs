using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

    public static StateManager instance;

    public enum Mode
    {
        None = 0,
        Trainer,
        Trainee,
        Trainee_Event
    }

    public enum State
    {
        Initialize,
        SetOrigin,
        Progress
    }

    public Mode mode = Mode.None;
    public State state = State.Initialize;
    public Dictionary<string, bool> Equip_CheckMap = new Dictionary<string, bool>();
    public int[] Cur_EquipNum;

    // Use this for initialization
    void Start () {
        instance = this;
        Cur_EquipNum = new int[Database.instance.Equipment_List.Count];
        InvokeRepeating("Audio_Attention", 30f, 25f);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateList();
	}

    public void InitializeEquipCount()
    {
        foreach (Equipment temp in Database.instance.Equipment_List)
            Equip_CheckMap.Add(temp.name, false);
        for (int i = 0; i < Cur_EquipNum.Length; ++i)
        {
            Cur_EquipNum[i] = PlayerPrefs.GetInt(Database.instance.Equipment_List[i].name + "Number");
        }
    }

    void UpdateList()
    {
        GameObject.Find("Status").GetComponent<UnityEngine.UI.Text>().text = "Mode: " + mode + "\nState:" + state;
        string inputList = "";
        if (state == State.Progress && mode == Mode.Trainer)
        {
            foreach (Equipment temp in Database.instance.Equipment_List)
            {
                inputList += "<color=" + /*hexConverter(temp.color)*/temp.colorString + ">" + temp.name + ":  " + PlayerPrefs.GetInt(temp.name + "Number") + "</color>" + "\n";
            }
        }
        else if (state == State.Progress && mode == Mode.Trainee)
        {
            foreach (Equipment temp in Database.instance.Equipment_List)
            {
                inputList += "<color=" + /*hexConverter(temp.color)*/temp.colorString + ">" + temp.name + ":  " + Cur_EquipNum[temp.id] + "</color>" + "\n";
            }
        }
        GameObject.Find("List").GetComponent<UnityEngine.UI.Text>().text = inputList;
    }

    public void RemoveOneEquip(string input)
    {
        Cur_EquipNum[Database.instance.Equip_Map[input].id]--;
    }

    public void PlayAudio(int index)
    {
        AudioSource source = Camera.main.GetComponent<AudioSource>();
        if (source.isPlaying)
            source.Stop();
        source.clip = Database.instance.Audio_List[index];
        source.Play();
    }

    public void StopAudio()
    {
        AudioSource source = Camera.main.GetComponent<AudioSource>();
        source.Stop();
    }

    public void ReplayAudio()
    {
        AudioSource source = Camera.main.GetComponent<AudioSource>();
        source.Stop();
        source.Play();
    }

    void Audio_Attention()
    {
        if (state == State.Initialize && mode == Mode.None)
            PlayAudio(6);
    }

    public void ResetCheckMap()
    {
        Equip_CheckMap.Clear();
        PlayAudio(0);
    }
}
