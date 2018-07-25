using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;

public class VoiceController : MonoBehaviour {

    public static VoiceController instance;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> cmds = new Dictionary<string, System.Action>();
    // Use this for initialization
    void Start () {
        instance = this;
        InitializeKR();
    }

    void OnDestroy()
    {
        RemoveKR();
    }

    void InitializeKR()
    {
        foreach (Equipment temp in Database.instance.Equipment_List)
            AddCmds(temp);
        // general commands
        cmds.Add("Trainer Mode", () => setMode(1));
        cmds.Add("Begin Program", () => setMode(2));
        cmds.Add("Start Training", () => setProperty(0));
        cmds.Add("Replay Video", () => RepeatVideo());
        cmds.Add("Stop Video", () => BehaviorManager.instance.StopVideo());
        cmds.Add("Replay Audio", () => StateManager.instance.ReplayAudio());
        cmds.Add("Stop Audio", () => StateManager.instance.StopAudio());
        cmds.Add("Reset", () => restart());
        cmds.Add("Save Room", () => restart());
        // @Test: Event Mode - Commands
        cmds.Add("Set Fire Event", () => TestFire(1));
        cmds.Add("Delete Fire Event", () => TestFire(2));
        cmds.Add("Enter Event Test", () => TestFire(3));
        cmds.Add("Quit Event Test", () => TestFire(4));
        keywordRecognizer = new KeywordRecognizer(cmds.Keys.ToArray());
        keywordRecognizer.Start();
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

    }

    // @Test: Event Mode - Execute Test Commands
    void TestFire(int index)
    {
        switch (index)
        {
            case 1:
                BehaviorManager.instance.TestSetEvent();
                break;
            case 2:
                GameObject temp = GameObject.Find("Fire Event");
                Destroy(temp);
                break;
            case 3:
                if (StateManager.instance.mode == StateManager.Mode.Trainee && StateManager.instance.state == StateManager.State.Progress && index == 3)
                {
                    StateManager.instance.mode = (StateManager.Mode)index;
                    TestFire(5);
                }
                break;
            case 4:
                if (StateManager.instance.mode == StateManager.Mode.Trainee_Event && StateManager.instance.state == StateManager.State.Progress)
                {
                    StateManager.instance.mode = StateManager.Mode.Trainee;
                    TestFire(2);
                }
                break;
            case 5:
                string nodeName = "Fire Event";
                if (PlayerPrefs.HasKey(nodeName + "X"))
                {
                    GameObject tempGO = GameObject.Instantiate(Database.instance.Effect_list[2]);
                    tempGO.name = nodeName;
                    tempGO.transform.SetParent(GameObject.Find("Origin 0").transform);
                    tempGO.transform.localPosition = new Vector3(PlayerPrefs.GetFloat(nodeName + "X"), PlayerPrefs.GetFloat(nodeName + "Y") - 2, PlayerPrefs.GetFloat(nodeName + "Z"));
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // @Test: Event Mode - Exit Found
        if (StateManager.instance.state == StateManager.State.Progress && StateManager.instance.mode == StateManager.Mode.Trainee_Event)
        {
            GameObject origin = GameObject.Find("Origin 0");
            for (int i = origin.transform.childCount - 1; i >= 1; i--)
            {
                if (Vector3.Distance(origin.transform.GetChild(i).position, Camera.main.transform.position) < 1)
                {
                    string GoName = origin.transform.GetChild(i).name;
                    string inputName = GoName.Substring(0, GoName.Length - 2);
                    if (inputName.Equals("Exit"))
                    {
                        GameObject effect = GameObject.Instantiate(Database.instance.Effect_list[0]);
                        effect.transform.parent = Camera.main.transform;
                        effect.transform.localPosition = new Vector3(0, 0, 3);
                        Destroy(effect, 2);
                        TestFire(4);
                    }
                    break;
                }
            }

        }




    }

    void RemoveKR()
    {
        if (keywordRecognizer != null)
        {
            cmds.Clear();
            keywordRecognizer.Stop();
            keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Dispose();
        }

    }

    public void resetKR()
    {
        RemoveKR();
        InitializeKR();
    }

    void AddCmds(Node node)
    {
        if(!PlayerPrefs.HasKey(node.name + "Number"))
        {
            PlayerPrefs.SetInt(node.name + "Number", 0);
        }
        else
        {
            for(int i = PlayerPrefs.GetInt(node.name + "Number"); i > 0; --i)
            {
                int tempindex = i;
                cmds.Add("Delete " + node.name + " "+ Database.instance.numbers[i], () => deleteEquip(node.name, tempindex));
            }
        }
        cmds.Add("Set " + node.name, () => setProperty(node.id));
        cmds.Add("Delete All " + node.name, () => deleteEquipAll(node.name));
    }
	


    string hexConverter(Color32 color)
    {
        int r = (int)(color.r * 256), g = (int)(color.g * 256), b = (int)(color.b * 256), a = (int)(color.a * 256);
        return "#" + color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2") + color.a.ToString("x2");
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordResponse;
        print(args.text);
        // Check to make sure the recognized keyword exists in the methods dictionary, then invoke the corresponding method.
        if (cmds.TryGetValue(args.text, out keywordResponse))
        {
            keywordResponse.Invoke();
        }

    }

    public void setMode(int index)
    {
        if(StateManager.instance.mode == StateManager.Mode.None && StateManager.instance.state == StateManager.State.Initialize)
        {
            StateManager.instance.mode = (StateManager.Mode)index;
            StateManager.instance.state = StateManager.State.SetOrigin;
            if (index == 1)
                StateManager.instance.PlayAudio(3);
            else
                StateManager.instance.PlayAudio(1);
        }
    }

    public void setOrigin()
    {
        if (StateManager.instance.mode != StateManager.Mode.None && StateManager.instance.state == StateManager.State.SetOrigin)
        {
            StateManager.instance.state = StateManager.State.Progress;
            BehaviorManager.instance.setOrigin();
            if (StateManager.instance.mode == StateManager.Mode.Trainer)
            {
                StateManager.instance.PlayAudio(4);
            }
            else
            {
                StateManager.instance.PlayAudio(2);
                FileController.momentToString(0, "");
            }
        }
    }

    public void deleteEquipAll(string equip)
    {
        if (StateManager.instance.mode == StateManager.Mode.Trainer && StateManager.instance.state == StateManager.State.Progress)
        {
            for(int i = PlayerPrefs.GetInt(equip+"Number"); i > 0; --i)
            {
                string prefix = equip + " " + i.ToString();
                PlayerPrefs.DeleteKey(prefix + "X");
                PlayerPrefs.DeleteKey(prefix + "Y");
                PlayerPrefs.DeleteKey(prefix + "Z");
            }
            PlayerPrefs.SetFloat(equip + "Number", 0);
            resetKR();
            BehaviorManager.instance.deleteEquipAll(equip);
        }
    }

    public void deleteEquip(string equip, int index)
    {
        if (StateManager.instance.mode == StateManager.Mode.Trainer && StateManager.instance.state == StateManager.State.Progress)
        {
            int eNum = PlayerPrefs.GetInt(equip + "Number");
            if(index < eNum)
            {
                for (int i = index; i < eNum; ++i)
                {
                    PlayerPrefs.SetFloat(equip + " " + index.ToString() + "X", PlayerPrefs.GetFloat(equip + " " + (index + 1).ToString() + "X"));
                    PlayerPrefs.SetFloat(equip + " " + index.ToString() + "Y", PlayerPrefs.GetFloat(equip + " " + (index + 1).ToString() + "Y"));
                    PlayerPrefs.SetFloat(equip + " " + index.ToString() + "Z", PlayerPrefs.GetFloat(equip + " " + (index + 1).ToString() + "Z"));
                }
            }
            string prefix = equip + " " + eNum.ToString();
            PlayerPrefs.DeleteKey(prefix + "X");
            PlayerPrefs.DeleteKey(prefix + "Y");
            PlayerPrefs.DeleteKey(prefix + "Z");
            PlayerPrefs.SetInt(equip + "Number", eNum - 1);
            resetKR();
            BehaviorManager.instance.deleteEquip(equip, index);
        }

    }

    public void setProperty(int index)
    {
        if (index == 0)
            setOrigin();
        else
        {
            if (StateManager.instance.mode == StateManager.Mode.Trainer && StateManager.instance.state == StateManager.State.Progress)
            {
                BehaviorManager.instance.setProperty(index);
            }

        }
    }

    void RepeatVideo()
    {
        BehaviorManager.instance.RepeatVideo();
    }

    public void restart()
    {
        StateManager.instance.mode = StateManager.Mode.None;
        StateManager.instance.state = StateManager.State.Initialize;
        StateManager.instance.ResetCheckMap();
        BehaviorManager.instance.restart();

    }
}
