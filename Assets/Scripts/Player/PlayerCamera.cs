using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KUNAI.Player {
    public class PlayerCamera : MonoBehaviour
    {
        InputManager inputManager;

        // X is X sensitivity, Y is Y sensitivity
        public Vector2 sensitivity = new Vector2(1,1);

        public Transform orientation;

        Vector2 rotation = new Vector2(0,0);

        void Start()
        {
            inputManager = InputManager.GetInstance();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            float mouseX = inputManager.cameraMovement.x * sensitivity.x;
            float mouseY = inputManager.cameraMovement.y * sensitivity.y;

            // Rotation works weird. I really could not tell you why it works like this.

            rotation.x -= mouseY;
            rotation.y += mouseX;

            // Clamps X rotation to prevent looking past up and pitching the head backwards.

            rotation.x = Mathf.Clamp(rotation.x, -90, 90);

            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
            orientation.rotation = Quaternion.Euler(0, rotation.y, 0);
        }
    }
}
