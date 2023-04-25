using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BillionGame {
    public class BillionBase : Entity {
        //Child Object Elements
        private SpriteRenderer xpFillSR;
        private SpriteRenderer teamSR;
        private SpriteRenderer hpFillSR;
        private SpriteRenderer hpBackSR;

        //Fields
        [SerializeField] private GameObject billionInstance;
        [SerializeField] private float rotationSpeed;
        private List<GameObject> childBillions = new List<GameObject>();

        private float currentRotationTime = 0.0f;
        private Quaternion cannonRotation;
        private Entity previousEntityTarget;

        //Properties
        [SerializeField] private int _maxBillions;
        public int MaxBillions {
            get { return _maxBillions; }
            set { _maxBillions = value; }
        }

        [SerializeField] private float _startSpawnTime;
        public float StartSpawnTime {
            get { return _startSpawnTime; }
            set { 
                _startSpawnTime = value;
                StartSpawning();
                }
        }

        [SerializeField] private float _spawnTimer;
        public float SpawnTimer {
            get { return _spawnTimer; }
            set { 
                _spawnTimer = value;
                StartSpawning();
                }
        }
        
        [SerializeField] [Range(0,360)] private float _spawnAngle;
        public float SpawnAngle {
            get { return _spawnAngle; }
            set { _spawnAngle = value; }
        }

        [SerializeField] private int _xpToLevel;
        public int XPToLevel {
            get { return _xpToLevel; }
            set { _xpToLevel = value; }
        }

        [SerializeField] private int _increasedXPReq;
        public int IncreasedXPReq {
            get { return _increasedXPReq; }
            set { _increasedXPReq = value; }
        }

        private int _currentXP = 0;
        public int CurrentXP {
            get { return _currentXP; }
            set { 
                _currentXP = value;
                CheckLevelUp();
                }
        }

        //Methods
        protected override void OnStart() {
            GameManager.BaseShootEvent += Shoot;
            SetMaterials();
            StartSpawning();
        }

        protected override void OnUpdate() {
            if (previousEntityTarget != EntityTarget) {
                cannonRotation = WeaponObject.transform.rotation;
                currentRotationTime = 0.0f;
            }
            previousEntityTarget = EntityTarget;
        }

        protected override void OnEnd() {
            GameManager.BaseShootEvent -= Shoot;
            foreach (GameObject billion in childBillions) {
                Destroy(billion);
            }

        }

        protected override void ColorSprites() {
            teamSR.color = MainColor;
            hpBackSR.color = BackgroundColor;
        }

        protected override void SetChildren(List<Transform> childObjects) {
            xpFillSR = childObjects[0].gameObject.GetComponent<SpriteRenderer>();
            teamSR = childObjects[1].gameObject.GetComponent<SpriteRenderer>();
            hpFillSR = childObjects[2].gameObject.GetComponent<SpriteRenderer>();
            hpBackSR = childObjects[3].gameObject.GetComponent<SpriteRenderer>();
            LevelSR = childObjects[4].gameObject.GetComponent<SpriteRenderer>();
            WeaponObject = childObjects[5].gameObject;
        }

        protected override void UpdateHealth() {
            float health = (float) CurrentHealth/(MaxHealthMultiplier*Level);
            hpFillSR.material.SetFloat("_Arc1",Mathf.Lerp(360,0,health));
        }

        protected override void Aim(Quaternion facing) {
            WeaponObject.transform.rotation = Quaternion.Slerp(cannonRotation,facing,currentRotationTime);
            currentRotationTime = currentRotationTime + Time.deltaTime * rotationSpeed;
        }

        //Setting fill materials
        //Using shader from https://github.com/Nrjwolf/unity-shader-sprite-radial-fill
        private void SetMaterials() {
            xpFillSR.material = new Material(GameManager.Instance.GetRadialShader());
            xpFillSR.material.color = GameManager.Instance.GetXPColor();
            xpFillSR.material.SetFloat("_Angle",90);
            xpFillSR.material.SetFloat("_Arc1",0);
            xpFillSR.material.SetFloat("_Arc2",360);
            hpFillSR.material = new Material(GameManager.Instance.GetRadialShader());
            hpFillSR.material.color = MainColor;
            hpFillSR.material.SetFloat("_Angle",90);
            hpFillSR.material.SetFloat("_Arc1",0);
            hpFillSR.material.SetFloat("_Arc2",0);
        }

        //Start spawning billions
        private void StartSpawning() {
            CancelInvoke();
            InvokeRepeating("SpawnBillion",StartSpawnTime,SpawnTimer);
        }

        //Spawn billion
        private void SpawnBillion() {
                Vector3 offset = Quaternion.AngleAxis(SpawnAngle, Vector3.forward) * Vector3.right;
                if (childBillions.Count < MaxBillions) {
                GameObject billion = Instantiate(billionInstance, transform.position + offset, Quaternion.identity);
                billion.transform.parent = gameObject.transform;
                Billion billComp = billion.GetComponent<Billion>();
                billComp.Team = Team;
                billComp.OriginBase = this;
                billComp.Level = Level;
                childBillions.Add(billion);
            } 
        }

        //Checks for level up, and levels up if needed
        private void CheckLevelUp() {
            if (_currentXP >= _xpToLevel) {
                Level++;
                _currentXP = _currentXP - _xpToLevel;
                CurrentHealth = MaxHealthMultiplier*Level;
                _xpToLevel += _increasedXPReq;
            }
            float xpFill = (float) _currentXP/_xpToLevel;
            xpFillSR.material.SetFloat("_Arc2",Mathf.Lerp(360,0,xpFill));
        }

        public override void FeedXP(int xpDropped) {
            CurrentXP = _currentXP + xpDropped;
        }

        //Remove Billion from list
        public void RemoveFromList(Billion billion) {
            if (childBillions.Contains(billion.gameObject)) {
                childBillions.Remove(billion.gameObject);
            }
        }
    }
}
