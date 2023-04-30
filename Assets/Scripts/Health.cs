using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KUNAI {
    public class Health : MonoBehaviour
    {
        public int health;
        int maxHealth;
        bool dead;

        void Start() {
            maxHealth = health;
        }

        void Damage(int damage) {
            health -= damage;

            if (health <= 0) dead = true;
        }
    }
}