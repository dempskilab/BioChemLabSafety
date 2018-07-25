using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoPlayer : MonoBehaviour {

    bool start;
    MovieTexture video;
	// Use this for initialization
	void Start () {
        
		
	}
	
	// Update is called once per frame
	void Update () {

        if (start && !video.isPlaying)
        {
            StopVideo();
        }

    }

    public void StopVideo()
    {
        video.Stop();
        BehaviorManager.instance.videoPlaying = false;
        Destroy(this.gameObject);

    }

    public void Initialize(string inputName)
    {
        Vector3 lookPos = Camera.main.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
        transform.Rotate(Vector3.right, 90);
        Renderer r = GetComponent<Renderer>();
        r.material.mainTexture = Database.instance.Equip_Map[inputName].video;
        video = (MovieTexture)r.material.mainTexture;
        video.Play();
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = Database.instance.Equip_Map[inputName].audio;
        audio.Play();
        start = true;
    }
}
