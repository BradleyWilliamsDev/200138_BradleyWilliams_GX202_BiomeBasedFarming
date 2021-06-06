using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController3rdPerson : MonoBehaviour
{

    [SerializeField] float followSpeed = 5;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform gripTransform;
    [SerializeField] Vector3 cameraTiltBack = new Vector3(10f, 0, 0);
    [SerializeField] Vector3 cameraTiltSide = new Vector3(0, 10f, 0);
    Vector3 cameraTiltSideLeft;

    Quaternion startRotGrip;
    Quaternion startRotCam;

    void Start()
    {
        startRotCam = cameraTransform.localRotation;
        startRotGrip = gripTransform.localRotation;

        cameraTiltSide.x = gripTransform.localEulerAngles.x;
        cameraTiltSideLeft = new Vector3(cameraTiltSide.x, -cameraTiltSide.y, cameraTiltSide.z);
    }

    void OnEnable()
    {
        InputController.OnMoveVectorUpdated.AddListener(MoveUpdated);
    }

    void OnDisable()
    {
        InputController.OnMoveVectorUpdated.RemoveListener(MoveUpdated);
    }

    void MoveUpdated(Vector2 moveVector) { }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerController.instance.rigidbody.position, Time.fixedDeltaTime * followSpeed);

        if (InputController.moveVector.y < 0)
        {
            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(cameraTiltBack * Mathf.Abs(InputController.moveVector.y)), Time.fixedDeltaTime * 1f);
        }
        else if (cameraTransform.localRotation != Quaternion.Euler(Vector3.zero))
        {
            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, startRotCam, Time.fixedDeltaTime * 0.5f);
        }

        if (InputController.moveVector.x != 0)
        {
            gripTransform.localRotation = Quaternion.Lerp(gripTransform.localRotation,
                (InputController.moveVector.x > 0 ? Quaternion.Euler(cameraTiltSide * Mathf.Abs(InputController.moveVector.x)) : Quaternion.Euler(cameraTiltSideLeft * Mathf.Abs(InputController.moveVector.x))), Time.fixedDeltaTime * 1f);
        }
        else if (gripTransform.localRotation != Quaternion.Euler(Vector3.zero))
        {
            gripTransform.localRotation = Quaternion.Lerp(gripTransform.localRotation, startRotGrip, Time.fixedDeltaTime * 0.5f);
        }
    }

}