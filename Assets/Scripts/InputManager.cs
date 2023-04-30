using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


namespace KUNAI {
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;

        public static InputManager GetInstance() {
            return instance;
        }    
        
        
        public Controls controls;

        [Header("Gameplay")]
        public Vector2 movement;
        public float movementAmount;
        public Vector2 cameraMovement;
        [SerializeField]
        public InputManagerButton attack;
        [SerializeField]
        public InputManagerButton jump;

        //I can't name the variable "throw", C# gets mad at me.
        [SerializeField]
        public InputManagerButton yeet;
        [SerializeField]
        public InputManagerButton recall;

        void Awake() {
            if (instance != null) {
                Destroy(GetInstance().gameObject);
                Debug.Log("Deleted Old InputManager Instance");
            }

            instance = this;

            controls = new Controls();
        }

        private void OnEnable() {
            controls.Gameplay.Movement.performed += ctx => movement = ctx.ReadValue<Vector2>();

            controls.Gameplay.Camera.performed += ctx => cameraMovement = ctx.ReadValue<Vector2>();

            controls.Gameplay.Attack.performed += ctx => attack.pressed = true;
            controls.Gameplay.Attack.canceled += ctx => attack.pressed = false;

            controls.Gameplay.Jump.performed += ctx => jump.pressed = true;
            controls.Gameplay.Jump.canceled += ctx => jump.pressed = false;

            controls.Gameplay.Throw.performed += ctx => yeet.pressed = true;
            controls.Gameplay.Throw.canceled += ctx => yeet.pressed = false;

            controls.Gameplay.Recall.performed += ctx => recall.pressed = true;
            controls.Gameplay.Recall.canceled += ctx => recall.pressed = false;

            
            

            controls.Enable();
        }

        private void OnDisable() {
            controls.Gameplay.Movement.performed -= ctx => movement = ctx.ReadValue<Vector2>();

            controls.Gameplay.Camera.performed -= ctx => cameraMovement = ctx.ReadValue<Vector2>();

            controls.Gameplay.Attack.performed -= ctx => attack.pressed = true;
            controls.Gameplay.Attack.canceled -= ctx => attack.pressed = false;

            controls.Gameplay.Jump.performed -= ctx => jump.pressed = true;
            controls.Gameplay.Jump.canceled -= ctx => jump.pressed = false;

            controls.Gameplay.Throw.performed -= ctx => yeet.pressed = true;
            controls.Gameplay.Throw.canceled -= ctx => yeet.pressed = false;

            controls.Gameplay.Recall.performed += ctx => recall.pressed = true;
            controls.Gameplay.Recall.canceled += ctx => recall.pressed = false;
            

            controls.Disable();
        }
        
        void Update()
        {
            attack.UpdateButton();
            jump.UpdateButton();
            yeet.UpdateButton();
            recall.UpdateButton();

            movementAmount = Vector2.Distance(Vector2.zero, movement);
        }

        

        [Serializable()]
        public class InputManagerButton {
            public bool pressed;
            public bool prev;
            public bool down;
            public bool up;

            public void UpdateButton() {

                down = UpdateDown(pressed, prev);
                up = UpdateUp(pressed, prev);

                prev = pressed;

                bool UpdateDown(bool pressed, bool prev) {
                    if (!(prev == pressed)) {
                        if (prev == false && pressed == true) {
                            return true;
                        } else {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }

                bool UpdateUp(bool pressed, bool prev) {
                    if (!(prev == pressed)) {
                        if (prev == true && pressed == false) {
                            return true;
                        } else {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }
            }
        }
    }
}



