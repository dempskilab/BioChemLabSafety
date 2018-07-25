using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBoard : MonoBehaviour {

    [SerializeField]
    UnityEngine.UI.Text text;

    public bool beChecked;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 lookPos = Camera.main.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
        transform.Rotate(Vector3.up, 180);
        text.text = transform.parent.name;
    }

    public void Checked()
    {
        beChecked = true;
        text.color = Color.white;
    }
}
