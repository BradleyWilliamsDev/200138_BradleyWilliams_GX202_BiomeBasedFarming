using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Toolbelt {
    public ToolObject toolObject;
    public GameObject toolGameObject;
}

public class PlayerToolBelt : MonoBehaviour {

    public static UnityEvent<ToolObject> OnToolSelected = new UnityEvent<ToolObject> ();

    [SerializeField] List<Toolbelt> toolbelt = new List<Toolbelt> ();
    [SerializeField] Toolbelt currentTool = null;

    void Start () {
        currentTool = null;
    }

    public void ActivateTool (ToolObject toolObject = null) {
        if (currentTool != null) {
            currentTool?.toolGameObject?.SetActive (false);
            currentTool = null;
            if (toolObject == null) OnToolSelected.Invoke (null);
        }
        if (toolObject != null) {
            currentTool = toolbelt.FindTool (toolObject);
            OnToolSelected.Invoke (toolObject);
            currentTool?.toolGameObject?.SetActive (true);
        }
    }

}

public static class ToolbeltExtensions {
    public static Toolbelt FindTool (this List<Toolbelt> toolbelt, ToolObject toolObject) {
        for (var i = 0; i < toolbelt.Count; i++) {
            if (toolbelt[i].toolObject == toolObject) {
                return toolbelt[i];
            }
        }
        return null;
    }
}