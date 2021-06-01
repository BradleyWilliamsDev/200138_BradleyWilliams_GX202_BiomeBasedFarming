using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;
    public static PlayerToolBelt playerToolBelt;

    [Header ("References")]
    // public Animator animator;
    [HideInInspector] public new Rigidbody rigidbody;

    [Header ("Config")]
    [SerializeField] float lookSpeed = 5;
    [SerializeField] float acceleration = 2;
    [SerializeField] float moveSpeed = 20;
    [SerializeField] float sprintMultiplier = 1.5f;

    Quaternion lookRotation;
    float inputMoveSpeed;
    float targetMoveSpeed;
    float currentSpeed;
    bool sprint = false;
    bool moveEnabled = true;

    void Awake () {
        instance = this;
        playerToolBelt = GetComponent<PlayerToolBelt> ();

        rigidbody = GetComponent<Rigidbody> ();
    }

    void OnEnable () {
        InputController.OnLookVectorUpdated.AddListener (LookUpdated);
        InputController.OnMoveVectorUpdated.AddListener (MoveUpdated);
        InputController.OnSprintUpdated.AddListener (SprintUpdated);
    }

    void OnDisable () {
        InputController.OnLookVectorUpdated.RemoveListener (LookUpdated);
        InputController.OnMoveVectorUpdated.RemoveListener (MoveUpdated);
        InputController.OnSprintUpdated.RemoveListener (SprintUpdated);
    }

    void Update () {

        if (moveEnabled) {
            if (InputController.moveVector != Vector2.zero) {
                lookRotation = Quaternion.LookRotation (new Vector3 (InputController.moveVector.x, 0, InputController.moveVector.y));
            }

            //Look
            transform.rotation = Quaternion.Lerp (transform.rotation, lookRotation.normalized, Time.deltaTime * lookSpeed);
        }

        //Move Animate
        targetMoveSpeed = inputMoveSpeed * (sprint ? sprintMultiplier : 1f) * (moveEnabled ? 1f : 0f);

        if (currentSpeed != targetMoveSpeed) {
            currentSpeed = Mathf.MoveTowards (currentSpeed, targetMoveSpeed, Time.deltaTime * acceleration);

            //animator.SetFloat ("Speed", (currentSpeed));
        }

        if (currentSpeed != 0) {
            Vector3 targetMoveVelocity = transform.forward * currentSpeed * moveSpeed;
            targetMoveVelocity.y = rigidbody.velocity.y;
            rigidbody.velocity = targetMoveVelocity;
        }
    }

    void LookUpdated (Vector2 lookVector) {

    }

    void MoveUpdated (Vector2 moveVector) {
        inputMoveSpeed = moveVector.magnitude;
    }

    void SprintUpdated (bool sprint) {
        this.sprint = sprint;
    }

    public void EnableMove (bool enable) {
        moveEnabled = enable;
    }

}