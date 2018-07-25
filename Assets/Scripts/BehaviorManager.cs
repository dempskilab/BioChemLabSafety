using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorManager : MonoBehaviour {

    public static BehaviorManager instance;

    Vector3 originPos, originRot;

    GameObject origin, caption, video;

    [SerializeField]
    GameObject description, videoObject, warning;

    bool repeat, end = false;
    public bool videoPlaying;
	// Use this for initialization
	void Start () {
        instance = this;
        caption = GameObject.Find("Caption");
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Color color = warning.GetComponent<UnityEngine.UI.Text>().color;
        if(color.a > 0)
        {
            color.a -= 0.002f;
            warning.GetComponent<UnityEngine.UI.Text>().color = color;
        }
        if (StateManager.instance.state == StateManager.State.Progress && StateManager.instance.mode == StateManager.Mode.Trainee && !videoPlaying)
        {
            bool looping = true;
            for (int i = origin.transform.childCount - 1; i >= 1; i--)
            {
                if (Vector3.Distance(origin.transform.GetChild(i).position, Camera.main.transform.position) < 1)
                {
                    string GoName = origin.transform.GetChild(i).name;
                    string inputName = GoName.Substring(0, GoName.Length - 2);
                    bool playVideo = false, bechecked = false;
                    if (!StateManager.instance.Equip_CheckMap[inputName] || repeat)
                    {
                        GameObject videoGo = GameObject.Instantiate(videoObject, Camera.main.transform.position + Camera.main.transform.right - Vector3.up, Quaternion.identity);
                        videoGo.name = "Video";
                        video = videoGo;
                        videoGo.transform.parent = Camera.main.transform;
                        videoGo.transform.localPosition = new Vector3(0.5f, 0, 3);
                        videoGo.GetComponent<VideoPlayer>().Initialize(inputName);
                        StateManager.instance.Equip_CheckMap[inputName] = true;
                        warning.GetComponent<UnityEngine.UI.Text>().text = inputName;
                        color = warning.GetComponent<UnityEngine.UI.Text>().color;
                        color.a = 1f;
                        warning.GetComponent<UnityEngine.UI.Text>().color = color;
                        repeat = false;
                        videoPlaying = true;
                        StateManager.instance.StopAudio();
                        playVideo = true;
                    }
                    if (!origin.transform.GetChild(i).gameObject.GetComponentInChildren<DisplayBoard>().beChecked)
                    {
                        GameObject effect = GameObject.Instantiate(Database.instance.Effect_list[0]);
                        effect.transform.parent = Camera.main.transform;
                        effect.transform.localPosition = new Vector3(0, 0, 3);
                        Destroy(effect, 2);
                        StateManager.instance.RemoveOneEquip(inputName);
                        FileController.momentToString(1,GoName);
                        //Destroy(origin.transform.GetChild(i).gameObject);
                        origin.transform.GetChild(i).gameObject.GetComponentInChildren<DisplayBoard>().Checked();
                        bechecked = true;
                    }

                    if(playVideo || bechecked)
                    {
                        looping = false;
                        break;
                    }

                }

                if (!end && looping)
                {
                    looping = origin.transform.GetChild(i).gameObject.GetComponentInChildren<DisplayBoard>().beChecked;
                }

            }

            if (!end && looping)
            {
                end = true;
                color = warning.GetComponent<UnityEngine.UI.Text>().color;
                color.a = 1f;
                warning.GetComponent<UnityEngine.UI.Text>().text = "Congrats!\nPlease go back to Origin";
                warning.GetComponent<UnityEngine.UI.Text>().color = color;
                StateManager.instance.PlayAudio(5);
                FileController.momentToString(2, "");
            }
                

        }
		
	}

    public void RepeatVideo()
    {
        repeat = true;
    }

    public void setOrigin()
    {
        GameObject originGO = InstantiateNode(0,"Origin", 0);
        foreach (Equipment temp in Database.instance.Equipment_List)
            getProperty(temp.name);
        StateManager.instance.InitializeEquipCount();
    }
    
    public void setProperty(int index)
    {
        initializeProperty(Database.instance.Equipment_List[index].name);

    }

    public void deleteEquipAll(string equip)
    {
        for(int i = origin.transform.childCount - 1; i >= 0; --i)
        {
            if (origin.transform.GetChild(i).name.Contains(equip))
                Destroy(origin.transform.GetChild(i).gameObject);
        }
    }

    public void deleteEquip(string equip, int index)
    {
        int eNum = PlayerPrefs.GetInt(equip + "Number") + 1;
        for (int i = origin.transform.childCount - 1; i >= 0; --i)
        {
            if (origin.transform.GetChild(i).name.Contains(equip + " " + index.ToString()))
                Destroy(origin.transform.GetChild(i).gameObject);
        }
        if(eNum > index)
        {
            for (int i = index + 1; i <= eNum; ++i)
                GameObject.Find(equip + " " + i.ToString()).name = equip + " " + (i-1).ToString();
        }
    }

    void initializeProperty(string prefix_key)
    {
        GameObject tempGO;
        //if (PlayerPrefs.HasKey(prefix_key + "X") && (tempGO = GameObject.Find(prefix_key)) != null)
        //    Destroy(tempGO);
        int index = PlayerPrefs.GetInt(prefix_key + "Number");
        PlayerPrefs.SetInt(prefix_key + "Number", index + 1);
        tempGO = InstantiateNode(1, prefix_key, PlayerPrefs.GetInt(prefix_key + "Number"));
    }

    void getProperty(string prefix_key)
    {
        for (int i = PlayerPrefs.GetInt(prefix_key + "Number"); i > 0; --i)
        {
            GameObject tempGO;
            tempGO = InstantiateNode(2, prefix_key, i);
        }
        

    }


    /*
     * index type
     * 0: set Origin
     * 1: set an equipment node
     * 2: get an existing equipment node
     */

    GameObject InstantiateNode(int cmd_index, string prefix_key, int index)
    {
        GameObject tempGO = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Sphere);
        string nodeName = prefix_key + " " + index.ToString();
        tempGO.transform.localScale = Vector3.one * 0.1f;
        tempGO.name = nodeName;
        switch (cmd_index)
        {
            case 0:
                originPos = Camera.main.transform.position;
                originRot = Camera.main.transform.localEulerAngles;
                originRot.x = 0;
                originRot.z = 0;
                //tempGO.GetComponent<Renderer>().material.color = Color.blue;
                tempGO.transform.position = originPos;
                tempGO.transform.localEulerAngles = originRot;
                tempGO.transform.SetParent(caption.transform, true);
                origin = tempGO;
                break;
            case 1:
                //tempGO.GetComponent<Renderer>().material.color = Color.yellow;
                Vector3 camForward = Camera.main.transform.forward * 0.5f;
                camForward.y = 0;
                tempGO.transform.position = Camera.main.transform.position + camForward;
                tempGO.transform.SetParent(origin.transform);
                Vector3 tempPos = tempGO.transform.localPosition;
                PlayerPrefs.SetFloat(nodeName + "X", tempPos.x);
                PlayerPrefs.SetFloat(nodeName + "Y", tempPos.y);
                PlayerPrefs.SetFloat(nodeName + "Z", tempPos.z);
                VoiceController.instance.resetKR();
                break;
            case 2:
                //tempGO.GetComponent<Renderer>().material.color = Color.yellow;
                tempGO.transform.SetParent(origin.transform);
                tempGO.transform.localPosition = new Vector3(PlayerPrefs.GetFloat(nodeName + "X"), PlayerPrefs.GetFloat(nodeName + "Y"), PlayerPrefs.GetFloat(nodeName + "Z"));
                break;
        }
        GameObject temp_description = GameObject.Instantiate(description);
        temp_description.transform.position = tempGO.transform.position;
        temp_description.transform.SetParent(tempGO.transform);
        temp_description.transform.localScale = Vector3.one;
        temp_description.GetComponent<UnityEngine.UI.Text>().text = nodeName;
        temp_description.GetComponent<UnityEngine.UI.Text>().fontSize = 1;
        temp_description.GetComponent<UnityEngine.UI.Text>().color = Database.instance.Equip_Map[prefix_key].color;
        return tempGO;

    }

    public void restart()
    {
        GameObject videoPlayer;
        if((videoPlayer = GameObject.Find("Video")) != null)
        {
            videoPlayer.SendMessage("StopVideo", false);
        }
        end = false;
        Destroy(origin);
    }

    public void StopVideo()
    {
        if (video)
            video.SendMessage("StopVideo", SendMessageOptions.DontRequireReceiver);
    }

    // @Test: Event Mode - Set Event Node
    public void TestSetEvent()
    {
        if (StateManager.instance.state == StateManager.State.Progress && StateManager.instance.mode == StateManager.Mode.Trainer)
        {
            GameObject tempGO = null;
            if ((tempGO = GameObject.Find("Fire Event")) != null)
            {
                Vector3 camForward = Camera.main.transform.forward * 0.5f;
                camForward.y = 0;
                tempGO.transform.position = Camera.main.transform.position - Vector3.up + camForward;
                Vector3 tempPos = tempGO.transform.localPosition;
                PlayerPrefs.SetFloat(tempGO.name + "X", tempPos.x);
                PlayerPrefs.SetFloat(tempGO.name + "Y", tempPos.y);
                PlayerPrefs.SetFloat(tempGO.name + "Z", tempPos.z);

            }
            else
            {
                tempGO = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Sphere);
                string nodeName = "Fire Event";
                tempGO.transform.localScale = Vector3.one * 0.1f;
                tempGO.name = nodeName;

                tempGO.transform.position = Camera.main.transform.position - Vector3.up * 0.5f + Camera.main.transform.forward * 0.5f;
                tempGO.transform.SetParent(origin.transform);
                Vector3 tempPos = tempGO.transform.localPosition;
                PlayerPrefs.SetFloat(nodeName + "X", tempPos.x);
                PlayerPrefs.SetFloat(nodeName + "Y", tempPos.y);
                PlayerPrefs.SetFloat(nodeName + "Z", tempPos.z);

                GameObject temp_description = GameObject.Instantiate(description);
                temp_description.transform.position = tempGO.transform.position + Vector3.up * 0.5f;
                temp_description.transform.SetParent(tempGO.transform);
                temp_description.transform.localScale = Vector3.one;
                temp_description.GetComponent<UnityEngine.UI.Text>().text = nodeName;
                temp_description.GetComponent<UnityEngine.UI.Text>().fontSize = 1;

            }
        }
    }

}
