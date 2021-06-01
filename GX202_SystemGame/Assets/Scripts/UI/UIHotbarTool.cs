using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIHotbarTool : MonoBehaviour {

    public static UnityEvent OnSetAllInactive = new UnityEvent ();

    [SerializeField] ToolObject toolObject;

    bool active = false;

    public void ActivateTool () {
        active = !active;
        if (active) {
            PlayerController.playerToolBelt.ActivateTool (toolObject);
            OnSetAllInactive.Invoke ();
            OnSetAllInactive.AddListener (SetInactive);
        } else {
            PlayerController.playerToolBelt.ActivateTool ();
            OnSetAllInactive.RemoveListener (SetInactive);
        }
    }

    void SetInactive () {
        active = false;
        OnSetAllInactive.RemoveListener (SetInactive);
    }

}