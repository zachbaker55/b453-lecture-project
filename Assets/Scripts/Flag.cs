using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BillionGame {
    public class Flag : MonoBehaviour {
        private SpriteRenderer srenderer;
        private Animator animator;
        private LineRenderer lineRenderer;

        public Team Team;

        private Vector2 mousePos;
        private State flagState;

        private void Awake() {
            srenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start() {
            if (animator != null) {
                switch (Team) {
                    case Team.red:
                        animator.SetTrigger("setRed");
                    break;
                    case Team.blue:
                        animator.SetTrigger("setBlue");
                    break;
                    case Team.yellow:
                        animator.SetTrigger("setYellow");
                    break;
                    case Team.green:
                        animator.SetTrigger("setGreen");
                    break;
                }
            }
        }

        private void Update() {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((GameManager.Instance.state == State.drawing) && (flagState == State.drawing)) {
                lineRenderer.SetPosition(0,transform.position);
                lineRenderer.SetPosition(1,mousePos);
            }
        }

        private void OnMouseDown() {
            if ((GameManager.Instance.state == State.free) && (flagState == State.free) && (GameManager.Instance.PlayerTeam == Team)) {
                GameManager.Instance.state = State.drawing;
                flagState = State.drawing;
                lineRenderer.enabled = true;
            }
        }

        private void OnMouseUp() {
            if ((GameManager.Instance.state == State.drawing) && (flagState == State.drawing)) {
                transform.position = mousePos;
                lineRenderer.enabled = false;
                flagState = State.free;
                GameManager.Instance.state = State.free;
            }
        }

    }
}
