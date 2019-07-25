using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDeathController : MonoBehaviour {

    private ParticleSystem part;
    float startTime;

	// Use this for initialization
	void Start () {
        part = gameObject.GetComponentInChildren<ParticleSystem>();
        part.Play();
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(part.main.duration+startTime<Time.time)
        {
            Destroy(gameObject);//destroys itself once it has finished playing
        }
	}
}
