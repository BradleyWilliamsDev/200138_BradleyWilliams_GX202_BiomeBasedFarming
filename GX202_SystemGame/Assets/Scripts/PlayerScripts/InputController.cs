using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    public static UnityEvent<Vector2> OnMoveVectorUpdated = new UnityEvent<Vector2>();
    public static UnityEvent<Vector2> OnLookVectorUpdated = new UnityEvent<Vector2>();
    public static UnityEvent<bool> OnSprintUpdated = new UnityEvent<bool>();
    public static UnityEvent<Vector3> OnGroundHit = new UnityEvent<Vector3>();
    public static UnityEvent<InteractableBase> OnInteractableHit = new UnityEvent<InteractableBase>();
    public static UnityEvent OnInteractStarted = new UnityEvent();
    public static UnityEvent OnInteractCanceled = new UnityEvent();

    PlayerInput playerInput;

    public static Vector2 moveVector { get; private set; }
    public static Vector2 lookVector { get; private set; }
    public static bool sprint { get; private set; }
    public static Vector3 groundHitPoint { get; private set; }
    public static InteractableBase interactable { get; private set; }

    [SerializeField] LayerMask placementMask;
    [SerializeField] LayerMask interactMask;

    Ray mouseRay;
    RaycastHit mouseRayHit;
    new Camera camera;

    void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();

        InputSetup();
    }

    void Start()
    {
        camera = Camera.main;
    }

    void OnDestroy()
    {
        playerInput.Dispose();
    }

    void InputSetup()
    {
        playerInput.Gameplay.Move.performed += x =>
        {
            moveVector = x.ReadValue<Vector2>();
            OnMoveVectorUpdated.Invoke(moveVector);
        };

        playerInput.Gameplay.Look.performed += x =>
        {
            lookVector = x.ReadValue<Vector2>();
            OnLookVectorUpdated.Invoke(lookVector);
        };

        playerInput.Gameplay.Sprint.started += x =>
        {
            sprint = true;
            OnSprintUpdated.Invoke(true);
        };

        playerInput.Gameplay.Sprint.canceled += x =>
        {
            sprint = false;
            OnSprintUpdated.Invoke(false);
        };

        playerInput.Gameplay.Interact.performed += x =>
        {
            Interact();
        };

        playerInput.Gameplay.Interact.started += x =>
        {
            if (!HitUI())
            {
                OnInteractStarted.Invoke();
            }
        };

        playerInput.Gameplay.Interact.canceled += x =>
        {
            if (!HitUI())
            {
                OnInteractCanceled.Invoke();
            }
        };
    }

    void Update()
    {
        if (HitUI()) return;

        mouseRay = camera.ScreenPointToRay(lookVector);

        if (Physics.Raycast(mouseRay, out mouseRayHit, 150, interactMask, QueryTriggerInteraction.Collide))
        {
            InteractableBase _interactable;
            if (mouseRayHit.collider.gameObject.TryGetComponent<InteractableBase>(out _interactable) ||
                (mouseRayHit.collider.attachedRigidbody != null && mouseRayHit.collider.attachedRigidbody.gameObject.TryGetComponent<InteractableBase>(out _interactable)))
            {
                interactable = _interactable;
            }
            else
            {
                interactable = null;
            }
        }
        else
        {
            interactable = null;
        }

        if (Physics.Raycast(mouseRay, out mouseRayHit, 150, placementMask, QueryTriggerInteraction.Ignore))
        {
            groundHitPoint = mouseRayHit.point;
        }
    }

    void Interact()
    {
        if (!HitUI())
        {
            if (interactable != null)
            {
                OnInteractableHit.Invoke(interactable);
            }
            else
            {
                OnGroundHit.Invoke(groundHitPoint);
            }
        }
    }

    public static bool HitUI()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = lookVector;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            return true;
        }
        return false;
    }

}