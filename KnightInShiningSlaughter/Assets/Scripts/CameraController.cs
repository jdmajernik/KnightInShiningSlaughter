using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //public float trauma; //dictates how much screen shake to use
    public float rot, offsetX, offsetY; //end result variables (for use to store calculations)
    public float maxX = 0.5f; //the max X that the screen can shake
    public float maxY = 0.5f;
    public float traumaDegrade = 0.2f;
    public float traumaX;
    public float traumaY;
    public Camera baseCamera;
    public GameObject player;
    public GameObject background;
    public float shake; //the amount of screen shake (equals trauma^3)
    public bool zoomingIn = false; // to test if the zoom in coroutine is running (I know there are better ways of doing this)

    private static float orthoSize; //the starting orthographic size of the camera;
    private static float startTimeScale;
	// Use this for initialization
	void Start () {
        orthoSize = GetComponent<Camera>().orthographicSize;
        startTimeScale = Time.timeScale;
	}

    private void Update()
    {
        background.transform.position = new Vector3(baseCamera.transform.position.x * 0.1f, background.transform.position.y, background.transform.position.z);
    }

    public IEnumerator shakeCamera ( float trauma, float degrade) {
        while (trauma > 0)
        {
            offsetY = trauma * (Mathf.PerlinNoise(Random.Range(-500, 500), Time.time) - .5f);
            offsetX = trauma * (Mathf.PerlinNoise(Random.Range(-50, 50), Time.time) - .5f);
            trauma -= degrade;
            transform.position = baseCamera.transform.position + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }
        if (trauma>0)
        {
            trauma -=degrade;
        }
        else if(trauma<0)
        {
            trauma = 0;
        }
        transform.position = baseCamera.transform.position + new Vector3(offsetX, offsetY, 0);
        yield return null;
        //transform.eulerAngles = baseCamera.transform.eulerAngles + new Vector3(0,0,rot);
	}
    public IEnumerator shakeCameraUp(float trauma, float degrade)
    {
        while (trauma>0)
        {
            offsetY = trauma * (Mathf.PerlinNoise(Random.Range(-50, 50), Time.time) - .5f);
            trauma -= degrade;
            transform.position = baseCamera.transform.position + new Vector3(0, offsetY, 0);
            yield return null;
        }
        offsetY = 0;
        yield return null;
    }
    public IEnumerator zoomIn (float duration, float slowdown, float zoom)
    {
        zoomingIn = true;
        float endTime = Time.time + duration;
        while(Time.time<endTime)
        {
            float step = 1 - ((endTime - Time.time) / duration);
            GetComponent<Camera>().orthographicSize = orthoSize + ((zoom - orthoSize) * step);
            Time.timeScale = startTimeScale + ((slowdown - startTimeScale)*step);
            yield return null;
        }
        //gameObject.GetComponent<Camera>().orthographicSize = orthoSize;
        //Time.timeScale = startTimeScale;
        yield return new WaitForSeconds(5);
    }
    public IEnumerator zoomOut (float duration)
    {
        zoomingIn = false;
        float endTime = Time.time + duration;
        float startTimeScale = 1;
        while (Time.time < endTime)
        {
            float startOrtho = GetComponent<Camera>().orthographicSize;
            float startSlowdown = Time.timeScale;
            float step = ((endTime - Time.time) / duration);
            GetComponent<Camera>().orthographicSize = orthoSize + ((startOrtho- orthoSize) * step);
            Time.timeScale = startTimeScale + ((startSlowdown - startTimeScale) * step);
            yield return null;
        }
        Time.timeScale = 1.0f;
        zoomingIn = false;
        yield return null;
    }
}
