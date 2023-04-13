using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BillionGame {
    public class Billion : Entity {
        //Components
        private Rigidbody2D rigidBody2D;

        //Child Object Elements
        private SpriteRenderer fillSR;
        private SpriteRenderer backSR;

        //Fields
        private List<Sprite> billionSpriteList;
        private Flag flagTarget = null;
        private Vector3 lastFlagPosition;
        private float decelTimer = 1.0f;
        private bool reached = false;

        //Properties
        private BillionBase _originBase;
        public BillionBase OriginBase {
            get { return _originBase; }
            set { _originBase = value; }
        }

        [SerializeField] [Range(0.0f, 10.0f)] private float _moveSpeed;
        public float MoveSpeed {
            get { return _moveSpeed; }
            set { _moveSpeed = value; }
        }

        [SerializeField] [Range(0.0f, 1.0f)] private float _slowDownSpeed;
        public float SlowDownSpeed {
            get { return _slowDownSpeed; }
            set { _slowDownSpeed = value; }
        }

        [SerializeField] [Range(0.0f, 5.0f)] private float _slowDownRange;
        public float SlowDownRange {
            get { return _slowDownRange; }
            set { _slowDownRange = value; }
        }

        //Methods
        protected override void OnAwake() {
            billionSpriteList = GameManager.Instance.BillionSpriteList;
            rigidBody2D = GetComponent<Rigidbody2D>();
        }

        protected override void OnStart() {
            GameManager.BillionShootEvent += Shoot;
        }

        protected override void OnUpdate() {
            flagTarget = GetFlagTarget();
        }

        protected override void OnFixedUpdate() {
            MoveToFlag();
        }
        
        protected override void OnEnd() {
            GameManager.BillionShootEvent -= Shoot;
            if (_originBase != null) {
                _originBase.RemoveFromList(this);
            }
        } 

        protected override void ColorSprites() {
            fillSR.color = MainColor;
            backSR.color = BackgroundColor;
        }

        protected override void SetChildren(List<Transform> childObjects) {
            fillSR = childObjects[0].gameObject.GetComponent<SpriteRenderer>();
            backSR = childObjects[1].gameObject.GetComponent<SpriteRenderer>();
            LevelSR = childObjects[2].gameObject.GetComponent<SpriteRenderer>();
            WeaponObject = childObjects[3].gameObject;
        }

        protected override void UpdateHealth() {
            float health = (float) CurrentHealth/(MaxHealthMultiplier*Level);
            if (health >= 1.0f) {
                fillSR.sprite = billionSpriteList[0];
            } else if (health > 0.65f) {
                fillSR.sprite = billionSpriteList[1]; 
            } else if (health > 0.35f) {
                fillSR.sprite = billionSpriteList[2]; 
            } else {
                fillSR.sprite = billionSpriteList[3]; 
            }
        }

        public override void FeedXP(int xpDropped) {
            OriginBase.FeedXP(xpDropped);
        }

        private Flag GetFlagTarget() {
            List<Flag> flagList = GameManager.Instance.GetTeamFlagList(Team);
            float smallestDistance = Mathf.Infinity;
            if ( !flagList.Any() ) {return null;}
            Flag closestFlag = flagList[0];
            foreach (Flag flag in flagList) {
                    float distance = Vector2.Distance(transform.position,flag.transform.position);
                    if (distance < smallestDistance) {
                        smallestDistance = distance;
                        closestFlag = flag;
                    }
                }
                return closestFlag;
        }

        private void MoveToFlag() {
            if (flagTarget != null) {
                if (flagTarget.transform.position != lastFlagPosition) {
                    decelTimer = 1.0f;
                    reached = false;
                    lastFlagPosition = flagTarget.transform.position;
                }
                float distance = Vector2.Distance(transform.position,flagTarget.transform.position);
                if (distance < _slowDownRange) {
                    decelTimer -= 0.002f;
                    if (decelTimer < 0) {
                        decelTimer = 0.0f;
                    }
                    if (distance < 0.5f) {
                        reached = true;
                    }
                }
                if (!reached) {
                    rigidBody2D.velocity = (flagTarget.transform.position - transform.position).normalized * _moveSpeed * decelTimer;
                } else {
                    rigidBody2D.velocity = rigidBody2D.velocity * decelTimer;
                }
                

            }
        }
    }
}
