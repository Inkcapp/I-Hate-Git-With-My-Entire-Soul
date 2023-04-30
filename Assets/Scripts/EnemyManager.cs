using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KUNAI.Enemy {
    public class EnemyManager : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public Transform[] spawnPoints;
        public int enemyCount;
        [HideInInspector]
        public List<Enemy> enemies;

        private static EnemyManager instance;

        public static EnemyManager GetInstance() {
            return instance;
        }

        void Awake() {
            if (instance != null) {
                Destroy(GetInstance().gameObject);
                Debug.Log("Deleted Old InputManager Instance");
            }

            instance = this;
        }

        public GameObject player;
        public Vector3 lastKnownLocation;

        void Start()
        {
            
        }

        void Update()
        {
            for (int i = 0; i < enemies.Count; i++) {
                if (!enemies[i]) {
                    enemies.Remove(enemies[i]);
                }
            }

            if (enemies.Count < enemyCount) {
                

                int r = (int)Random.Range(0, spawnPoints.Length - 1);

                Debug.Log(r);

                GameObject go = Instantiate(enemyPrefab, spawnPoints[r].position, Quaternion.identity);

                //go.transform.position = spawnPoints[r].position;

                enemies.Add(go.GetComponent<Enemy>());
            }
        }
    }
}