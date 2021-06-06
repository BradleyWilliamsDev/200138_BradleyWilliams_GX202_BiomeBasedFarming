using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodIncreaseHunger : MonoBehaviour
{
    private UIHungerBar uIHunger;

    private void Start() {
        uIHunger = FindObjectOfType<UIHungerBar>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uIHunger.waitTime += 5f;
            uIHunger.hungerBar.fillAmount = uIHunger.waitTime / uIHunger.cooldownTime;
            Destroy(this.gameObject);
        }
    }
}
