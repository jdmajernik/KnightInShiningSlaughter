using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public GameObject player;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");//auto imports player object
	}
	
	// Update is called once per frame
	void Update () {
        
        float newY = (transform.position.y * 0.93f) + (player.transform.position.y * 0.07f);//moves towards the player y every frame
        float newX = (transform.position.x * 0.93f) + (player.transform.position.x * 0.07f);//moves towards the player x every frame
        gameObject.transform.position = new Vector3(newX,newY,transform.position.z);//sets position to calculated position
	}
}
