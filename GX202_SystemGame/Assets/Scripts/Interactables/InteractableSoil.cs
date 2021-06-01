using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSoil : InteractableBase {

    bool occupied = false;
    [SerializeField] List<ToolObject> validTools = new List<ToolObject> ();

    void Awake () {
        SetupLinks ();
    }

    public override bool Interact () {
        if (!occupied) {
            occupied = true;
            return true;
        } else {
            return false;
        }
    }

    public override bool Validate (ToolObject toolObject) {
        if (validTools.Contains (toolObject)) {
            return !occupied;
        } else {
            return false;
        }
    }
}