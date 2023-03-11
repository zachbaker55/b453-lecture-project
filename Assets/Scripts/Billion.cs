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
        private Billion billionTarget;

        [HideInInspector] public BillionBase originBase;
        [HideInInspector] public int maxHealth;
        public int CurrentHealth {get; set;}
        public Quaternion Facing {get; set;}

        [Range(0.0f, 10.0f)] public float walkSpeed;
        [Range(0.0f, 10.0f)] public float slowRange;
        [Range(0.00f, 0.02f)] public float slowDown;
        private float deceleration = 1.0f;
        private bool isOver = false;

        private static List<Billion> billionList = new List<Billion>(); 

        private void Awake() {
            healthTransform = this.transform.GetChild(0);
            healthSprite = healthTransform.GetComponent<SpriteRenderer>();
            rigidBody = GetComponent<Rigidbody2D>();
            billionList.Add(this);
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
                    target = GetTarget(GameManager.Instance.redFlags);
                break;
                case Team.blue:
                    target = GetTarget(GameManager.Instance.blueFlags);
                break;
                case Team.yellow:
                    target = GetTarget(GameManager.Instance.yellowFlags);
                break;
                case Team.green:
                    target = GetTarget(GameManager.Instance.greenFlags);
                break;
            }
            if (Input.GetButtonDown("TempDamage") && isOver) { //Temporary damage
                takeDamage(1);
            }
            billionTarget = GetBillionTarget();
        }

        private void FixedUpdate() {
            moveTowardsTarget(target);
            AimAngle(billionTarget);
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


        private GameObject GetTarget(List<GameObject> flagArray) {
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

        private Billion GetBillionTarget() {
            //Find nearest billion that is not on the same team
            float smallestDistance = Mathf.Infinity;
            Billion closestBillion = null;
            foreach (Billion b in billionList) {
                float distance = Vector2.Distance(transform.position,b.transform.position);
                if ((distance < smallestDistance) && (b.Team != Team)) {
                    smallestDistance = distance;
                    closestBillion = b;
                }
            }
            return closestBillion;
        }

        private void AimAngle(Billion target) {
            //used: 
            //https://answers.unity.com/questions/1350050/how-do-i-rotate-a-2d-object-to-face-another-object.html
            //Aims at target gameObject
            if (target != null) {
                Vector3 targetPos = target.transform.position;
                targetPos.z = 0;
                targetPos.x = targetPos.x - transform.position.x;
                targetPos.y = targetPos.y - transform.position.y;
                
                float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;

                Facing = Quaternion.Euler(new Vector3(0, 0, angle));
                transform.rotation = Facing;
            }
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

        private void OnDisable() {
            originBase.childBillions.Remove(this.gameObject);
            billionList.Remove(this);
        }
    }
}
