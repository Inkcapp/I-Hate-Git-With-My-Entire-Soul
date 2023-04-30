using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KUNAI {
    public class Projectile : MonoBehaviour
    {
        public Rigidbody rb;
        public float speed;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate() {

        }
    }
}
