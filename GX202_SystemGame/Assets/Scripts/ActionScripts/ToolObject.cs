using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (fileName = "ToolObject", menuName = "Items/Tool", order = 0)]
public class ToolObject : ScriptableObject {

    [SerializeField] protected internal float range = 2f;
    [SerializeField, HideInInspector] protected internal float rangeSqd;
    [SerializeField] protected internal InteractionType interactionType; 

    void OnValidate () {
        rangeSqd = range * range;
    }

    public virtual void UseTool (Vector3 targetPosition, UnityAction interactionComplete) {
        Debug.Log ($"No implementation for tool.");
    }

    public virtual void UseTool (InteractableBase interactable, UnityAction interactionComplete) {
        Debug.Log ($"No implementation for tool.");
    }

}

public enum InteractionType {
    Ground, Surface, Interactable, GroundOrSurface, Any
}