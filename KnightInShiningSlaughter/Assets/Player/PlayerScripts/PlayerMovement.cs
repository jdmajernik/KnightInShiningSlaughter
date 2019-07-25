using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

    public float speed = 25.0f;
    public float jumpSpeed = 50.0f;
    public bool grounded; //if the player is touching the ground
    public Image blood;
    private Color bloodColor;
    private Animator anim;
    public GameObject pauseMenu;
    public GameObject helpMenu;
    public GameObject healthBar;
    public GameObject gameOverLose;
    public GameObject gameOverWin;

    public float gravityMult = 2.5f;
    public float jumpX;
    public float jumpXScale = 75.0f;
    private CameraController cam;
    public float moveX;
    private Rigidbody2D rb;
    private bool chargingDone = false; // if the player attack charging is done
    private float trauma = 0;
    private float bloodAlpha;
    private float alphaMax = 255f;
    private float lastTimeScale;
    private float maxHealth = 250f;
    private float health;
    private bool latch = true;
    [SerializeField]
    private int maxScenes;
    [SerializeField]
    private int thisScene;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cam = Camera.main.GetComponent<CameraController>();

        //all this sets up the blood overlay for the gui
        bloodAlpha = 0;
        updateBloodOverlay();
        maxScenes = SceneManager.sceneCountInBuildSettings;
        thisScene = SceneManager.GetActiveScene().buildIndex;

        pauseMenu.SetActive(false);
        helpMenu.SetActive(false);
        latch = true;
        health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
        if(health<=0)
        {
            if (latch)
            {
                Time.timeScale = 0;
                GameObject lose = Instantiate(gameOverLose);
                lose.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                lose.transform.parent = GameObject.Find("Canvas").transform;
                latch = false;
            }
            
        }
        healthBar.GetComponent<healthBarController>().updateHealthBar(health/maxHealth);
        if(Input.GetButtonDown("Cancel"))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            if(Time.timeScale!=0)
            {
                lastTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = lastTimeScale;
            }
        }
        if (!pauseMenu.activeSelf)
        {
            updateBloodOverlay();
            if (trauma > 0)
            {
                trauma -= 0.2f;
            }
            else
            {
                trauma = 0;
            }
            moveX = Input.GetAxis("Horizontal") * speed;
            if (moveX < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (moveX > 0)//the player faces wherever they moved last
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (Input.GetMouseButton(0))
            {
                moveX = 0;
                anim.SetBool("AttackCharging", true); // If the player holds down the mouse button, it charges a big attack and sets move to 0
                if (!cam.zoomingIn)
                {
                    StartCoroutine(cam.zoomIn(.8f, 0.1f, 10f));
                }
            }

            if (Input.GetButtonDown("Jump") && grounded)
            {
                anim.SetBool("InAir", true);
                grounded = false;
                jumpX = moveX;
                //moveX = moveX/2;//zeros out movement for jumping
            }
            if (moveX != 0 && grounded)
            {
                anim.SetBool("Walking", true);
            }
            else
            {
                anim.SetBool("Walking", false);
            }
            if (Input.GetMouseButtonUp(0))
            {
                moveX = 0;
                anim.SetBool("AttackCharging", false);
                StopAllCoroutines();
                StartCoroutine(cam.zoomOut(1));
            }
            if (Input.GetMouseButton(1))
            {
                anim.SetBool("Blocking", true);
                moveX = 0;
            }
            else
            {
                anim.SetBool("Blocking", false);
            }
            if (grounded)
            {
                transform.Translate(new Vector3(moveX, 0, 0));
            }
            else
            {
                rb.velocity += Vector2.right * moveX;
            }
        }
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            StartCoroutine(cam.shakeCameraUp(15f, 2f));
        }
        {
            grounded = true;
            anim.SetBool("InAir", false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Sword")
        {
            if (trauma < 5)
            {
                trauma += 2.4f;
            }
            if (!(bloodAlpha >= 1))
            {
                bloodAlpha += .1f;
            }
            //health -= 5;
            StartCoroutine(Camera.main.GetComponent<CameraController>().shakeCamera(trauma, 0.2f));
            //StartCoroutine(showHealthBar(2));
            //hit.Play();
            health -= 5;
        }
        if(collision.gameObject.tag == "Exit")
        {
            Debug.Log("Hit Exit");
            NextRoom();
        }
    }
    private IEnumerator waitOnAnimation(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }
    public IEnumerator jump()
    {
        rb.AddForce(new Vector2(jumpX*jumpXScale,jumpSpeed), ForceMode2D.Impulse);
        do
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityMult - 1) * Time.deltaTime;
            Debug.Log(rb.velocity.x);
            yield return null;
        } while (rb.velocity.y < 0);
        yield return null;
    }
    private void updateBloodOverlay()
    {
        bloodColor = blood.color;
        bloodColor.a = bloodAlpha;
        blood.color = bloodColor;
        if (bloodAlpha >0)
        {
            bloodAlpha -= 0.005f;
        }
    }
    public void resumeButtonClicked()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = lastTimeScale;
    }
    public void quitButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void helpButtonClicked()
    {
        helpMenu.SetActive(true);
    }
    public void helpExitButtonClicked()
    {
        helpMenu.SetActive(false);
    }
    public void NextRoom()
    {
        if(SceneManager.GetActiveScene().buildIndex>=maxScenes-1)
        {
            if (latch)
            {
                GameObject win = Instantiate(gameOverWin);
                Time.timeScale = 0;
                transform.position = transform.position;
                win.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                win.transform.parent = GameObject.Find("Canvas").transform;
                latch = false;
            }
        }
        else
        {
            health += 50;
            if(health>=maxHealth)
            {
                health = maxHealth;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }
}
