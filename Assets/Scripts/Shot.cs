using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BillionGame {
    public class Shot : MonoBehaviour {
        //Components
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rigidBody2D;
        private BoxCollider2D boxCollider2D;

        //Fields
        private Vector2 spawnLocation;

        //Properties
        [SerializeField] private Teams _team;
        public Teams Team {
            get { return _team; }
            set { 
                _team = value; 
                UpdateColor();
            }
        }
        private Entity _originEntity;
        public Entity OriginEntity {
            get { return _originEntity; }
            set { _originEntity = value; }
        }

        [SerializeField] private float _speed;
        public float Speed {
            get { return _speed; }
            set { _speed = value; }
        }

        [SerializeField] private int _damage;
        public int Damage {
            get { return _damage; }
            set { _damage = value; }
        }

        [SerializeField] private float _despawnDistance;
        public float DespawnDistance {
            get { return _despawnDistance; }
            set { _despawnDistance = value;}
        }

        //Methods

        private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void Start() {
            UpdateColor();
            spawnLocation = transform.position;
        }

        private void Update() {
            rigidBody2D.AddForce(transform.right * Speed);
            if (Vector2.Distance(spawnLocation,transform.position) > _despawnDistance) {
                Despawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            Entity entity = other.gameObject.GetComponent<Entity>();
            if (entity != null) {
                if (entity.Team != _team) {
                    if (entity.CurrentHealth <= _damage) {
                        OriginEntity.FeedXP(entity.XPDropAmount);
                    }
                    entity.TakeDamage(_damage);
                    Despawn();
                }
            } else {
                TilemapCollider2D collider = other.gameObject.GetComponent<TilemapCollider2D>();
                if (collider != null) {
                    Despawn();
                }
            }
        }

        //Updates color of shot
        private void UpdateColor() {
            Color shotColor = GameManager.Instance.GetTeamMainColor(_team);
            spriteRenderer.color = shotColor;
        }

        private void Despawn(){
            Destroy(this.gameObject);
        }

    }
}