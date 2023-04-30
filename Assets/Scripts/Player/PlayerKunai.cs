using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KUNAI.Player {
    public class PlayerKunai : MonoBehaviour
    {        
        InputManager inputManager;
        PlayerMovement playerMovement;
        public PlayerCamera playerCam;

        public GameObject kunaiPrefab;
        GameObject activeKunai;

        public float spawnDistance;

        void Start()
        {
            inputManager = InputManager.GetInstance();
            playerMovement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if(inputManager.yeet.down) {
                Throw();
            }

            if(inputManager.recall.down) {
                Substitute();
            }
        }

        void Throw() {
            activeKunai = Instantiate(kunaiPrefab);
            activeKunai.transform.position = transform.position + (playerCam.transform.forward * spawnDistance);

            activeKunai.transform.rotation = playerCam.transform.rotation;
            
            Projectile akp = activeKunai.GetComponent<Projectile>();

        }

        void Substitute() {
            Rigidbody arg = activeKunai.GetComponent<Rigidbody>();

            Vector3 oldVel = playerMovement.rb.velocity;

            playerMovement.rb.velocity = arg.velocity;

            Vector3 oldPos = transform.position;
            transform.position = activeKunai.transform.position;

            activeKunai.transform.position = oldPos;
            arg.velocity = oldVel;
        }
    }
}
