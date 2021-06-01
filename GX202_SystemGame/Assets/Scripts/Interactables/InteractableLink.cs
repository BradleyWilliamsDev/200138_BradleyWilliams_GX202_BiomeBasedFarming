using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLink : InteractableBase {

    InteractableBase interactable;

    public void SetInteractable (InteractableBase interactable) {
        this.interactable = interactable;
    }

    public override bool Interact () {
        return interactable.Interact ();
    }

    public override bool Validate (ToolObject toolObject) {
        return interactable.Validate (toolObject);
    }

}