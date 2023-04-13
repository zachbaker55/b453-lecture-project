using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BillionGame {
    public class Flag : MonoBehaviour {
        //Child Object Elements
        private SpriteRenderer flagSR;

        //Fields
        private LineRenderer lineRenderer;
        private bool isDrawing = false;
        private Vector2 mousePosition;

        //Properties
        [SerializeField] private Teams _team;
        public Teams Team {
            get { return _team; }
            set { 
                _team = value; 
                UpdateColor();
            }
        }

        private void Awake() {
            flagSR = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start() {
            GameManager.Instance.AddToFlagList(_team,this);
            UpdateColor();
        }

        private void Update() {
            mousePosition = GameManager.Instance.MousePosition;
            if (isDrawing) {
                lineRenderer.SetPosition(0,transform.position);
                lineRenderer.SetPosition(1,mousePosition);
            }
        }

        private void OnDisable() {
            GameManager.Instance.RemoveFromFlagList(_team,this);
        }

        //Colors sprites
        private void UpdateColor() {
            Color flagColor = GameManager.Instance.GetTeamMainColor(_team);
            Color trailColor = GameManager.Instance.GetTeamBackgroundColor(_team);
            flagSR.color = flagColor;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(flagColor, 0.0f), new GradientColorKey(trailColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;
        }

        private void OnMouseDown() {
            if ((!isDrawing) && (GameManager.Instance.PlayerTeam == _team)) {
                isDrawing = true;
                lineRenderer.enabled = true;
            }
        }

        private void OnMouseUp() {
            if (isDrawing) {
                transform.position = mousePosition;
                lineRenderer.enabled = false;
                isDrawing = false;
            }
        }
        





    }
}
