using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BillionGame {
    //Fields
    public abstract class Entity : MonoBehaviour {

        //Child Object Elements
        private GameObject _weaponObject;
        protected GameObject WeaponObject {
            get { return _weaponObject; }
            set { _weaponObject = value; }
        }

        [SerializeField] private SpriteRenderer _levelSR;
        protected SpriteRenderer LevelSR {
            get { return _levelSR; }
            set { _levelSR = value; }
        }

        //Fields
        private List<Transform> _childObjects = new List<Transform>();
        [SerializeField] private GameObject shotInstance;

        //Properties
        [SerializeField] private Teams _team;
        public Teams Team {
            get { return _team; }
            set { 
                _team = value; 
                UpdateColors();
            }
        }
        [SerializeField] private int _level;
        public int Level {
            get { return _level; }
            set { 
                _level = Mathf.Clamp(value,0,GameManager.Instance.LevelCap);
                UpdateLevel();
                }
        }

        [SerializeField] private int _maxHealthMultiplier;
        public int MaxHealthMultiplier {
            get { return _maxHealthMultiplier; }
            set { 
                _maxHealthMultiplier = value;
                UpdateHealth();
                }
        }

        private int _currentHealth = 3;
        public int CurrentHealth {
            get { return _currentHealth; }
            set { 
                _currentHealth = value;
                UpdateHealth();
                }
        }
        [SerializeField] private int _damageMultiplier;
        public int DamageMultiplier {
            get { return _damageMultiplier; }
            set { _damageMultiplier = value; }
        }

        private Entity _entityTarget;
        protected Entity EntityTarget {
            get { return _entityTarget; }
            set { _entityTarget = value; }
        }

        [SerializeField] private float _shootingRange;
        public float ShootingRange {
            get { return _shootingRange; }
            set { _shootingRange = value; }
        }

        [SerializeField] private int _xpDropAmount;
        public int XPDropAmount {
            get { return _xpDropAmount; }
        }

        private Color _backgroundColor;
        protected Color BackgroundColor {
            get { return _backgroundColor; }
        }

        private Color _mainColor;
        protected Color MainColor {
            get { return _mainColor; }
        }

        //Methods
        private void Awake() {
            foreach (Transform child in transform) {
                _childObjects.Add(child);
            }
            SetChildren(_childObjects);
            OnAwake();
        }

        private void Start() {
            CurrentHealth = MaxHealthMultiplier*Level;
            GameManager.Instance.AddToEntityList(_team,this);
            UpdateColors();
            UpdateLevel();
            OnStart();
        }

        private void Update() {
            _entityTarget = GetEntityTarget();
            OnUpdate();
        }

        private void FixedUpdate() {
            OnFixedUpdate();
            AimAngle();
        }

        private void OnEnable() {
        }

        private void OnDisable() {
            GameManager.Instance.RemoveFromEntityList(_team,this);
            OnEnd();
        }

        public void TakeDamage(int damage) {
            CurrentHealth = CurrentHealth - damage;
            if (CurrentHealth <= 0) {
                Destroy(gameObject);
            }
        }

        //Puts Team colors into variables
        private void UpdateColors() {
            _backgroundColor = GameManager.Instance.GetTeamBackgroundColor(_team);
            _mainColor = GameManager.Instance.GetTeamMainColor(_team);
            ColorSprites();
        }

        //Updates level sprite, and forces level cap
        private void UpdateLevel() {
            _levelSR.sprite = GameManager.Instance.LevelSpriteList[_level];
        }

        //Gets nearest billion that is not on the same team
        private Entity GetEntityTarget() {
            float smallestDistance = Mathf.Infinity;
            Entity closestEntity = null;
            foreach (Team team in GameManager.Instance.TeamList) {
                if (team.TeamEnum != _team) {
                    foreach (Entity entity in team.EntityList) {
                        float distance = Vector2.Distance(transform.position,entity.transform.position);
                        if (distance < smallestDistance) {
                            smallestDistance = distance;
                            closestEntity = entity;
                        }
                    }
                }
            }
            return closestEntity;
        }

        //Aim at Entity Target
        private void AimAngle() {
            if (_entityTarget != null) {
                Vector3 targetPos = _entityTarget.transform.position;
                targetPos.z = 0;
                targetPos.x = targetPos.x - transform.position.x;
                targetPos.y = targetPos.y - transform.position.y;
                
                float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;

                Quaternion Facing = Quaternion.Euler(new Vector3(0, 0, angle));
                Aim(Facing);
            }
        }

        protected void Shoot() {
            if (_entityTarget != null) {
                float distance = Vector2.Distance(transform.position,_entityTarget.transform.position);
                if (distance <= _shootingRange) {
                    GameObject shotObject = Instantiate(shotInstance, _weaponObject.transform.position, _weaponObject.transform.rotation);
                    Shot shot = shotObject.GetComponent<Shot>();
                    shot.transform.parent = gameObject.transform;
                    shot.Team = _team;
                    shot.Damage = DamageMultiplier*Level;
                    shot.OriginEntity = this;
                }
            }
        }

        //Color all sprites with _backgroundColor and _mainColor
        protected abstract void ColorSprites();

        //Set all children Game Objects in list to variables
        protected abstract void SetChildren(List<Transform> childObjects);

        //Update Health displays
        protected abstract void UpdateHealth();

        //Feed XP from shot back to base
        public abstract void FeedXP(int xpDropped);

        //Aim at Facing
        protected virtual void Aim(Quaternion facing) {
            _weaponObject.transform.rotation = facing;
        }

        //Awake
        protected virtual void OnAwake() {
        }

        //Start
        protected virtual void OnStart() {
        }
        
        //Update
        protected virtual void OnUpdate() {
        }
        
        //FixedUpdate
        protected virtual void OnFixedUpdate() {
        }

        //OnDisable
        protected virtual void OnEnd() {
        }
    }
}