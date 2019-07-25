using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class healthBarController : MonoBehaviour {

    public Image healthBar;
    
    public void updateHealthBar (float value)
    {
        healthBar.fillAmount = value;
    } 
}
