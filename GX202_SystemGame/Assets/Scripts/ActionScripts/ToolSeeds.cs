using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (fileName = "ToolSeeds", menuName = "Items/Tool/Seeds", order = 0)]
public class ToolSeeds : ToolObject {

    [SerializeField] GameObject seedlingPrefab;

    public override void UseTool (InteractableBase interactable, UnityAction interactionComplete) {
        if (interactable.Interact ()) {
            //PlayerController.instance.animator.SetTrigger ("ToolSeedsPlant");
            GameObject newSeedling = Instantiate (seedlingPrefab, interactable.transform.position, Quaternion.identity, Terrain.activeTerrain.transform);
            PlayerController.instance.transform.LookAt (interactable.transform.position);
            PlayerController.instance.transform.Upright ();
        }
        interactionComplete.Invoke ();
    }

}