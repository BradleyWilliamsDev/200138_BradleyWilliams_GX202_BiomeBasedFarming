using UnityEngine;

public abstract class InteractableBase : MonoBehaviour {

    protected void SetupLinks () {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider> ();
        foreach (Collider collider in colliders) {
            collider.gameObject.AddComponent<InteractableLink> ().SetInteractable (this);
        }
    }

    public abstract bool Interact ();
    public abstract bool Validate (ToolObject toolObject);

}