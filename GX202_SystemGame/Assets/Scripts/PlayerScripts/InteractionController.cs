using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour {

    // [SerializeField] Animator selectAnimator;

    [SerializeField] List<Toolbelt> toolbelt = new List<Toolbelt> ();
    [SerializeField] Toolbelt currentTool = null;

    [SerializeField] MeshRenderer toolRangeRenderer;
    Material toolRangeMaterial;
    [SerializeField] float toolRangeMaxDistanceMultiplier = 4f;
    [SerializeField] float toolRangeDistanceFade = 0.01f;
    Vector4 rangeMaterialFade = new Vector4 ();

    bool toolEquipped = false;
    bool toolValid = false;
    bool toolInUse = false;
    bool toolActive = false;
    bool distCheck;
    float toolRange;
    Collider[] overlapBoxTest = new Collider[1];
    List<Material> materials = new List<Material> ();
    Vector3 snapPosition;
    float cameraDistSqr;

    InteractableBase validInteractable;

    void Start () {
        currentTool = null;
        toolEquipped = false;

        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer> (true);
        foreach (MeshRenderer mesh in meshes) {
            materials.AddRange (mesh.materials);
        }

        if (toolRangeRenderer != null) {
            toolRangeMaterial = toolRangeRenderer.material;
        }

        cameraDistSqr = (Camera.main.transform.position - PlayerController.instance.transform.position).sqrMagnitude;
    }

    void OnEnable () {
        PlayerToolBelt.OnToolSelected.AddListener (ToolSelected);
        InputController.OnInteractStarted.AddListener (InteractStarted);
        InputController.OnInteractCanceled.AddListener (InteractCanceled);
    }

    void OnDisable () {
        PlayerToolBelt.OnToolSelected.RemoveListener (ToolSelected);
        InputController.OnInteractStarted.RemoveListener (InteractStarted);
        InputController.OnInteractCanceled.RemoveListener (InteractCanceled);
    }

    void ToolSelected (ToolObject toolObject) {
        if (currentTool != null) {
            currentTool?.toolGameObject?.SetActive (false);
            currentTool = null;
        }

        if (toolObject != null) {
            if (!toolEquipped) {
                toolEquipped = true;
                // selectAnimator.SetTrigger ("StartTool");
            }
            currentTool = toolbelt.FindTool (toolObject);
            currentTool?.toolGameObject?.SetActive (true);

            toolRangeRenderer.enabled = true;
            toolRangeRenderer.transform.localScale = new Vector3 ((currentTool.toolObject.range * 2) + 0.5f, 4f, (currentTool.toolObject.range * 2) + 0.5f);
        } else {
            if (toolEquipped) {
                // selectAnimator.SetTrigger ("EndTool");
                toolRangeRenderer.enabled = false;
                toolEquipped = false;
            }
        }
    }

    void InteractStarted () {
        if (toolEquipped && toolValid) {
            toolActive = true;
            toolInUse = true;
            if (validInteractable == null) {
                currentTool.toolObject.UseTool (transform.position, InteractComplete);
            } else {
                currentTool.toolObject.UseTool (validInteractable, InteractComplete);
            }
        }
    }

    void InteractComplete () {
        toolInUse = false;
    }

    void InteractCanceled () {
        toolActive = false;
    }

    void Update () {
        SetSelectorPosition ();
    }

    void SetSelectorPosition () {
        if (toolEquipped && !toolInUse) {
            snapPosition = InputController.groundHitPoint.SnapVector ();
            transform.position = snapPosition;
            toolRange = (PlayerController.instance.transform.position - snapPosition).sqrMagnitude;

            rangeMaterialFade.x = Mathf.Lerp (rangeMaterialFade.x, ((toolRange / currentTool.toolObject.rangeSqd) * cameraDistSqr) * toolRangeMaxDistanceMultiplier, Time.deltaTime * 5f);
            rangeMaterialFade.y = toolRangeDistanceFade;
            toolRangeMaterial.SetVector ("_DistanceFade", rangeMaterialFade);
            toolRangeRenderer.transform.position = PlayerController.instance.transform.position;

            distCheck = toolRange <= currentTool.toolObject.rangeSqd;
            bool toolValid = false;

            if (distCheck) {
                overlapBoxTest = Physics.OverlapBox (snapPosition, Vector3.one / 4, Quaternion.identity, 1 << LayerMask.NameToLayer ("Interactable"), QueryTriggerInteraction.Collide);
                validInteractable = null;
                if (currentTool.toolObject.interactionType == InteractionType.Ground) {
                    if (overlapBoxTest.Length == 0) toolValid = true;
                } else if (currentTool.toolObject.interactionType == InteractionType.Interactable) {
                    if (overlapBoxTest.Length > 0) {
                        for (var i = 0; i < overlapBoxTest.Length; i++) {
                            if (overlapBoxTest[i].TryGetComponent<InteractableBase> (out InteractableBase _interactable)) {
                                if (_interactable.Validate (currentTool.toolObject)) {
                                    validInteractable = _interactable;
                                    break;
                                }
                            }
                        }
                        toolValid = validInteractable != null;
                    }
                }
            }

            SetToolValid (toolValid);

            if (toolActive) {
                InteractStarted ();
            }
        }
    }

    void SetToolValid (bool valid) {
        toolValid = valid;
        materials.ForEach (x => x.SetInt ("_VALID", valid ? 1 : 0));
    }

}

public static class InteractionExtensions {
    public static Vector3 SnapVector (this Vector3 position, float multiplier = 1) {
        Vector3 snappedVector = position * multiplier;
        snappedVector.x = Mathf.Round (snappedVector.x);
        snappedVector.z = Mathf.Round (snappedVector.z);
        snappedVector.y = Terrain.activeTerrain.SampleHeight (snappedVector);
        snappedVector /= multiplier;

        return snappedVector;
    }

    public static void Upright (this Transform transform) {
        transform.localEulerAngles = new Vector3 (0, transform.localEulerAngles.y, 0);
    }

}