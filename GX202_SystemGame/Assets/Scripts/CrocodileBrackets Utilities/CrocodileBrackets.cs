using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrocodileBrackets
{
    public class Utilities
    {
        private static Vector3 _cameraForward;
        private static Vector3 _cameraRight;
        private static Vector3 _cameraBasedDirection;
        private static Vector3 worldPos;
        private static int _frameRate;

        // Update inputs based on camera view
        public static Vector3 CameraBasedInput(Transform _incomingCamera, Vector2 _incomingInput)
        {
            _cameraForward = _incomingCamera.forward;
            _cameraRight = _incomingCamera.right;
            _cameraForward.y = 0;
            _cameraRight.y = 0;
            _cameraForward = _cameraForward.normalized;
            _cameraRight = _cameraRight.normalized;
            _cameraBasedDirection = _cameraForward * _incomingInput.y + _cameraRight * _incomingInput.x;
            return _cameraBasedDirection;
        }

        // Set cursor settings
        public static void CursorSettings(bool _cursorVisibility, CursorLockMode _lockState)
        {
            Cursor.lockState = _lockState;
            Cursor.visible = _cursorVisibility;
        }

        // screen to world position 3D
        public static Vector3 ScreenToWorldPosition3D(Camera _camera, Vector2 _mouse)
        {
            Ray ray = _camera.ScreenPointToRay(_mouse);
            Debug.DrawRay(ray.origin, ray.direction * 500, Color.blue);
            if (Physics.Raycast(ray, out RaycastHit _hit))
            {
                worldPos = _hit.point;
            }
            return worldPos;
        }
        // Set Framerate limit
        public static void FrameRateLimiter(int _incomingframeRate)
        {
            _frameRate = _incomingframeRate;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _frameRate;
        }
    }
}