using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (fileName = "ToolHoe", menuName = "Items/Tool/Hoe", order = 0)]
public class ToolHoe : ToolObject {

    [SerializeField] GameObject dirtPrefab;
    [SerializeField] float duration = 2;

    Coroutine hoeGroundCoroutine;
    UIProgressBar progressBar;
    UnityAction interactionCompleteCallback;

    public override void UseTool (Vector3 targetPosition, UnityAction interactionComplete) {
        hoeGroundCoroutine = PlayerController.instance.StartCoroutine (HoeGround (targetPosition));
        InputController.OnInteractCanceled.AddListener (InteractCancel);
        interactionCompleteCallback = interactionComplete;
    }

    void InteractCancel () {
        if (hoeGroundCoroutine != null) PlayerController.instance.StopCoroutine (hoeGroundCoroutine);
        InputController.OnInteractCanceled.RemoveListener (InteractCancel);
        ObjectPoolProgressBar.instance.PutIntoObjectPool (progressBar);

        //PlayerController.instance.animator.SetBool ("ToolSwing", false);
        PlayerController.instance.EnableMove (true);

        interactionCompleteCallback.Invoke ();
    }

    IEnumerator HoeGround (Vector3 targetPosition) {
        float _duration = duration;

        //Get objectpool progress bar
        progressBar = ObjectPoolProgressBar.instance.GetFromObjectPool ();
        progressBar.SetFollowTarget (targetPosition, Vector3.up);

        //PlayerController.instance.animator.SetBool ("ToolSwing", true);
        PlayerController.instance.EnableMove (false);

        while (_duration > 0) {
            _duration -= Time.deltaTime;
            progressBar.SetProgress (_duration / duration);
            PlayerController.instance.transform.LookAt (targetPosition);
            PlayerController.instance.transform.Upright ();

            yield return null;
        }

        //Create dirt block
        GameObject newDirtBlock = Instantiate (dirtPrefab, targetPosition, Quaternion.identity, Terrain.activeTerrain.transform);

        InteractCancel ();
    }

}