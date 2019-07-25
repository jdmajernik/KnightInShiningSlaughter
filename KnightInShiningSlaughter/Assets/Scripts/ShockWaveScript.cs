using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveScript : MonoBehaviour {

    private float KillTimer = 8; //lives for this many seconds
    private float KillTime; //the killtimer added to Time.time

    [SerializeField]
    private float moveSpeed = 8f; //the x move speed

    public float moveDir;//the x move direction

	void Start () {
        KillTime = Time.time + KillTimer;
	}
	
	void Update () {
        transform.position += new Vector3(moveDir * moveSpeed, 0);
		if(Time.time>=KillTime)
        {
            Destroy(gameObject);
        }
	}
}
