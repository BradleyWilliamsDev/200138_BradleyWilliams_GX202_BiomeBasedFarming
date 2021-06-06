using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHungerBar : MonoBehaviour
{
    public Image hungerBar;
    public bool coolingDown;
    public float waitTime = 30.0f;

    private void Start() {
        coolingDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolingDown == true)
        {
            //Reduce fill amount over 30 seconds
            hungerBar.fillAmount -= 1.0f / waitTime * Time.deltaTime;
        }
    }
}
