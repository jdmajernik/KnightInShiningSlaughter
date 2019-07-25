using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {
    public ParticleSystem hit;
    public GameObject death;
    public GameObject pauseMenu;
    public enum AiState {idle, spotted, attacking, knockback };
    public AiState currentState;

    public Canvas canvas;
    public GameObject healthbar;

    public int currentWaypoint = 0;
    public float nextWaypointDistance = 0.02f;

    public float speed = 50f;
    public float jumpSpeed = .2f;
    public float colliderRadius = 1.0f;
    public float moveScale = 0.5f;

    public PlatformPath path;
    public List<Node> pathList;
    public GameObject target;
    public int Count;

    private bool start;
    private bool done;
    private Rigidbody2D rb;

    private float startTime;
    private float endTime;
    private float jumpTime = 1.5f; //the time for the jump
    private float t = 0;
    private bool startJump = false;
    private float time = 0;
    private Animator anim;
    private bool jump = false;
    private Vector3 yDist; //the y distance between the enemy center and the node center
    private Vector3 jumpStartPos;
    private Vector3 jumpEndPos;
    private float SightRadius = 50f;
    private float pathTimer = 0.5f;
    private bool latch = true;
    private float idleRange = 80f;
    private float attackRange = 10f;
    [SerializeField]
    private float health = 0f;
    [SerializeField]
    private float shockwaveVel = 50f; //the amount of force the shockwave sends the enemy knocking back
    private float maxHealth = 100f;
    private GameObject thisHealthBar;
    private Image healthBarRect;
    private float deathZone = -600f; //the y value to kill the gameobject at

    private void Start()
    {
        currentState = AiState.idle;

        canvas = Canvas.FindObjectOfType<Canvas>();
        path = gameObject.AddComponent<PlatformPath>();
        target = GameObject.FindGameObjectWithTag("Player");
        pathList = new List<Node>();
        start = false;
        done = false;
        StartCoroutine(findPath());
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        yDist = new Vector3(0, transform.position.y - path.nodePath[0].transform.position.y);
        health = maxHealth;
        thisHealthBar = Instantiate(healthbar);
        thisHealthBar.transform.SetParent(canvas.transform);
        thisHealthBar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 10, 0));
        healthbar.SetActive(false);
    }
    private void Update()
    {
        if(transform.position.y < deathZone)
        {
            Destroy(gameObject);
        }
        thisHealthBar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 7, 0));
        thisHealthBar.GetComponent<healthBarController>().updateHealthBar(health / maxHealth);
        if (health<=0)
        {
            Destroy(gameObject);
        }
        if(Time.time>path.time+(pathTimer*3))
        {
            StartCoroutine(findPath());//checks if the findPath quit for some reason
        }
        float playerDist = Vector3.Distance(transform.position, target.transform.position);
        switch (currentState)
        {
            case AiState.idle:
                if(latch)
                {
                    latch = false;
                    StartCoroutine(turn(5));
                }
                //anim.SetBool("Grounded", true);
                anim.SetBool("IsWalking", false);
                if(checkPlayerVisisble())
                {
                    currentState = AiState.spotted;
                }
                break;
            case AiState.spotted:
                latch = true;
                if(playerDist >= idleRange)
                {
                    
                    currentState = AiState.idle;
                }
                else if(playerDist <=attackRange)
                {
                    currentState = AiState.attacking;
                }
                StartCoroutine(move());
                break;
            case AiState.attacking:
                if(target.transform.position.x<transform.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                anim.SetBool("Attacking", true);
                if(playerDist>attackRange)
                {
                    anim.SetBool("Attacking", false);
                    currentState = AiState.spotted;
                }
                break;
        }
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Sword")
        {
            health -= 5;
            StartCoroutine(Camera.main.GetComponent<CameraController>().shakeCamera(1.2f,0.1f));
            StartCoroutine(showHealthBar(2));
            hit.Play();
            //health -= 10;
        }
        if(collision.gameObject.tag == "Shockwave")
        {
            Destroy(collision.gameObject);
            float velDir = collision.GetComponent<ShockWaveScript>().moveDir;
            rb.AddForce(new Vector2(velDir * shockwaveVel*4, shockwaveVel/2),ForceMode2D.Impulse);
            health -= 25;
            StartCoroutine(showHealthBar(2));
        }
    }
    private void OnDestroy()
    {
        GameObject deathPart = Instantiate(death);
        deathPart.transform.position = transform.position;
        thisHealthBar.SetActive(false);
        if (thisHealthBar != null)
        {
            Destroy(thisHealthBar);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="Ground")
        {
            anim.SetBool("Grounded", true);
        }
    }

    private IEnumerator findPath()
    {
        //Debug.Log("asking for new path");
        path.createPath(transform.position, target.transform.position);
        pathList = path.nodePath;
        yield return new WaitForSeconds(pathTimer);
        StartCoroutine(findPath());
    }
    private IEnumerator move()
    {
        if (currentWaypoint>=path.vectorPath.Length)
        {
            yield return null;
        }
        if (path.nextNode(currentWaypoint, nextWaypointDistance))
        {
            currentWaypoint++;
        }
        if (jump)
        {
            StartCoroutine(Jump());
            yield return null;
        }
        else
        {
            if (path.nodePath[currentWaypoint].thisNodeType == Node.nodeType.jump&&path.nodePath[currentWaypoint+1].location.y>path.nodePath[currentWaypoint].location.y)
            {
                jump = true;
                anim.SetBool("Jump", true);
                anim.SetBool("Grounded", false);
                yield return null;
            }
            else
            {
                startJump = true;
                Vector3 dir = (path.vectorPath[currentWaypoint]);
                Vector3 velocity = dir * moveScale;
                //Debug.Log("Velocity - " + velocity);
                if (velocity.x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (velocity.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                transform.position += velocity;
                anim.SetBool("IsWalking", true);
                anim.SetBool("Grounded", true);

            }
            
        }
        
        yield return null;
    }
    private void resetTimer()
    {
        startTime = Time.time;
        endTime = startTime + jumpTime;
        jumpStartPos = path.nodePath[currentWaypoint].location;
        jumpEndPos = path.nodePath[currentWaypoint + 1].location;
        t = 0;
    }
    private IEnumerator Jump()
    {
        anim.SetBool("Jump", true);
        StopCoroutine(findPath());
        if (startJump)
        {
            resetTimer();
        }
        startJump = false;
        t = (Time.time - startTime) / (endTime - startTime);
        time = Time.time;
        Debug.Log(t);
        Debug.Log("StartPos -" + jumpStartPos);
        Debug.Log("EndPos" + jumpEndPos);
        if (Time.time < endTime)
        {
            transform.position = path.jumpVel(t, jumpStartPos, jumpEndPos, 20) + yDist;
        }
        else
        {
            yield return null;
            jump = false;
            anim.SetBool("Jump", false);
        }
        yield return null;
    }
    private bool checkPlayerVisisble()
    {
        if(Vector3.Distance(transform.position, target.transform.position)<SightRadius)
        {
            if(transform.localScale.x*(target.transform.position.x-transform.position.x)>0)
            {
                return true;
            }
        }
        return false;
    }
    private IEnumerator turn(float timer)
    {
        transform.localScale = new Vector3(-transform.localScale.x,1,1);
        yield return new WaitForSeconds(timer);
        StartCoroutine(turn(timer));
    }
    private IEnumerator showHealthBar (int timer)
    {
        thisHealthBar.SetActive(true);
        for (int a = 0; a < timer; a++)
        {
            yield return new WaitForSeconds(1);
        }
        thisHealthBar.SetActive(false);
        yield return null;
    }
}
