using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHungerBar : MonoBehaviour
{
    public Image hungerBar;
    public bool coolingDown;
    public float waitTime = 30.0f;
    public float cooldownTime = 30.0f;

    private void Start() {
        coolingDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolingDown == true)
        {
            //Reduce fill amount over 30 seconds
            waitTime -= 1.0f * Time.deltaTime;
            if (waitTime <= 0)
            {
                waitTime = 0;
            }
            hungerBar.fillAmount -= 1.0f / cooldownTime * Time.deltaTime;
        }
    }
}
