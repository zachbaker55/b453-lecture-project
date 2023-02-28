using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BillionGame {
    public class Billion : MonoBehaviour {
        public Team Team;
        private Transform healthTransform;
        private SpriteRenderer healthSprite;
        private Rigidbody2D rigidBody;
        [SerializeField] private Sprite billionRed;
        [SerializeField] private Sprite billionBlue;
        [SerializeField] private Sprite billionYellow;
        [SerializeField] private Sprite billionGreen;
        
        private GameObject target;
        private Vector3 oldTarget;

        [HideInInspector] public BillionBase originBase;
        [HideInInspector] public int maxHealth;
        public int CurrentHealth {get; set;}

        [Range(0.0f, 10.0f)] public float walkSpeed;
        [Range(0.0f, 10.0f)] public float slowRange;
        [Range(0.00f, 0.02f)] public float slowDown;
        private float deceleration = 1.0f;
        private bool isOver = false;


        private void Awake() {
            healthTransform = this.transform.GetChild(0);
            healthSprite = healthTransform.GetComponent<SpriteRenderer>();
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            CurrentHealth = maxHealth;
            switch (Team) {
                case Team.red:
                    healthSprite.sprite = billionRed;
                break;
                case Team.blue:
                    healthSprite.sprite = billionBlue;
                break;
                case Team.yellow:
                    healthSprite.sprite = billionYellow;
                break;
                case Team.green:
                    healthSprite.sprite = billionGreen;
                break;
            }

            //Fixes overlapping when spawned
            Invoke("FixOverlap",0.5f);
        }

        private void Update() {
            switch (Team) {
                case Team.red:
                    target = getTarget(GameManager.Instance.redFlags);
                break;
                case Team.blue:
                    target = getTarget(GameManager.Instance.blueFlags);
                break;
                case Team.yellow:
                    target = getTarget(GameManager.Instance.yellowFlags);
                break;
                case Team.green:
                    target = getTarget(GameManager.Instance.greenFlags);
                break;
            }
            if (Input.GetButtonDown("TempDamage") && isOver) { //Temporary damage
                takeDamage(1);
            }
        }

        private void FixedUpdate() {
            moveTowardsTarget(target);
        }

        private void OnMouseEnter() {
            isOver = true;
        }
        private void OnMouseExit() {
            isOver = false;
        }


        private void FixOverlap() {
            transform.position = transform.position + new Vector3(0.01f,0.01f,0);
        }


        private GameObject getTarget(List<GameObject> flagArray) {
            float smallestDistance = Mathf.Infinity;
            if ( !flagArray.Any() ) {return null;}
            GameObject closestFlag = flagArray[0];
            foreach (GameObject f in flagArray) {
                    float distance = Vector2.Distance(transform.position,f.transform.position);
                    if (distance < smallestDistance) {
                        smallestDistance = distance;
                        closestFlag = f;
                    }
                }
                return closestFlag;
        }
        
        private void moveTowardsTarget(GameObject target) {
            if (target != null) {
                if (target.transform.position != oldTarget) {
                    deceleration = 1.0f;
                    oldTarget = target.transform.position;
                }
                rigidBody.velocity = (target.transform.position - transform.position).normalized * walkSpeed * deceleration;
                float distance = Vector2.Distance(transform.position,target.transform.position);
                if (distance < slowRange) { 
                    deceleration -= slowDown;
                    if (deceleration < 0) {deceleration = 0;}
                } 
                
            }
        }

        private void takeDamage(int damage) {
            int prevHealth = CurrentHealth;
            CurrentHealth -= damage;
            if (CurrentHealth <= 0) {
                Destroy(gameObject);
            } else {
                float healthPercent = (float) CurrentHealth/maxHealth;
                Debug.Log(healthPercent);
                healthTransform.localScale = Vector3.Lerp(new Vector3(0.3f,0.3f,0.3f), new Vector3(1,1,1), healthPercent);
            }
        }

        private Vector3 healthToScale(int health) {
            return healthTransform.localScale;
        }

        private void OnDestroy() {
            originBase.childBillions.Remove(this.gameObject);
        }
    }
}
