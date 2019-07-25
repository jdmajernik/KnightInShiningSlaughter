using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour {

    //So, basically I have a function I already want/like in player movement, but I want to call it in an animation, which is a part of the player's child (playerBody)
    //so I wrote a script that just attatches to playerBody and plays on the animation but calls from the orginal playerMovement. And all that sounds kind of roundabout, but it works.

    //Further Note: I absolutely should have named this PlayerAnimationController, because that's what it does, but it originally only controlled the jump, so it is what it is, I guess

    private GameObject player;
    private PlayerMovement move;
    public GameObject smallSword;
    public GameObject bigSword;
    public GameObject torsoUpper;
    public GameObject bigSwordPosition;
    public GameObject shockwave;
    public ParticleSystem charged;

    private Vector3 bigSwordStartPos;
    private Quaternion bigSwordStartRot;
    private bool firstCharge = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        move = player.GetComponent<PlayerMovement>();
        bigSwordPosition = GameObject.Find("BigSwordPosition");
        //charged = GameObject.Find("Hit").GetComponent<ParticleSystem>();
    }
    public void jump()
    {
        StartCoroutine(move.jump());
    }
    public void stepCamerShake()
    {
        StartCoroutine(Camera.main.GetComponent<CameraController>().shakeCameraUp(.6f, .1f));
    }
    public void bigAttackCharged()
    {
        if (firstCharge)
        {
            charged.Play();
            firstCharge = false;
        }
        bigSwordStartPos = bigSword.transform.position - player.transform.position;
        bigSwordStartRot = bigSword.transform.rotation;
        bigSword.transform.parent = smallSword.transform;
        GetComponent<Animator>().SetBool("AttackFullyCharged", true);
        
    }
    public void bigAttackDone()
    {
        firstCharge = true;
        Debug.Log(bigSwordPosition.transform.position);
        Debug.Log(bigSwordPosition.transform.localPosition);
        Debug.Log(bigSword.transform.localPosition);
        bigSword.transform.parent = torsoUpper.transform;
        bigSword.transform.localPosition = bigSwordPosition.transform.localPosition;
        bigSword.transform.localRotation = bigSwordPosition.transform.localRotation;
        GetComponent<Animator>().SetBool("AttackFullyCharged", false);
        //bigSword.transform.position = bigSwordPosition.transform.position;
        //bigSword.transform.rotation = bigSwordPosition.transform.rotation;
    }
    public void bigAttackStart()
    {
        StartCoroutine(Camera.main.GetComponent<CameraController>().zoomOut(1.5f));
    }
    public void bigAttack()
    {
        StartCoroutine(Camera.main.GetComponent<CameraController>().shakeCamera(35f,2f));
        GameObject newShockwave = Instantiate(shockwave);
        newShockwave.transform.position = bigSword.transform.position - new Vector3(3,0,0);
        newShockwave.GetComponent<ShockWaveScript>().moveDir = gameObject.GetComponentInParent<Transform>().lossyScale.x;
    }
}
