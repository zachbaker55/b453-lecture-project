using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BillionGame {

    public enum Team {
        red,
        blue,
        yellow,
        green
    }

    public class BillionBase : MonoBehaviour {
        
        public GameObject Billion;
        private SpriteRenderer srenderer;
        [SerializeField] private Sprite baseRed;
        [SerializeField] private Sprite baseBlue;
        [SerializeField] private Sprite baseYellow;
        [SerializeField] private Sprite baseGreen;

        public Team Team;
        public float StartTime;
        public float SpawnTime;
        public int MaxBillions;
        private List<GameObject> childBillions = new List<GameObject>();

        private void Awake() {
            srenderer = GetComponent<SpriteRenderer>();
        }

        private void Start() {
            switch (Team) {
                case Team.red:
                    srenderer.sprite = baseRed;
                break;
                case Team.blue:
                    srenderer.sprite = baseBlue;
                break;
                case Team.yellow:
                    srenderer.sprite = baseYellow;
                break;
                case Team.green:
                    srenderer.sprite = baseGreen;
                break;
            }
            InvokeRepeating("SpawnBillion",StartTime, SpawnTime);
        }

        private void SpawnBillion() {
            if (childBillions.Count < MaxBillions) {
                GameObject billion = Instantiate(Billion, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
                billion.transform.parent = gameObject.transform;
                billion.GetComponent<Billion>().Team = Team;
                childBillions.Add(billion);
            } 
        }


    }
}
