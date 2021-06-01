using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour {

	public Image progressBar;

	Transform targetTransform;
	Vector3 targetPosition;
	Vector3 targetOffset;

	Camera camera;

	void OnEnable () {
		progressBar.fillAmount = 0;

		if (camera == null) camera = Camera.main;
	}

	void OnDisable() {
		targetPosition = Vector3.zero;
		targetTransform = null;
		targetOffset = Vector3.zero;	
	}

	void LateUpdate () {
		transform.position = camera.WorldToScreenPoint ((targetTransform == null ? targetPosition : targetTransform.position) + targetOffset);
	}

	public void SetFollowTarget (Vector3 position, Vector3 offset) {
		targetPosition = position;
		targetOffset = offset;
	}

	public void SetFollowTarget (Transform target, Vector3 offset) {
		targetTransform = target;
		targetOffset = offset;
	}

	public void SetProgress (float percentage) {
		progressBar.fillAmount = percentage;
	}
}