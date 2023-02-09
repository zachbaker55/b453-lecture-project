using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BillionGame {
    public class Billion : MonoBehaviour {
        private SpriteRenderer srenderer;
        [SerializeField] private Sprite billionRed;
        [SerializeField] private Sprite billionBlue;
        [SerializeField] private Sprite billionYellow;
        [SerializeField] private Sprite billionGreen;

        public Team Team;

        private void Awake() {
            srenderer = GetComponent<SpriteRenderer>();
        }

        private void Start() {
            switch (Team) {
                case Team.red:
                    srenderer.sprite = billionRed;
                break;
                case Team.blue:
                    srenderer.sprite = billionBlue;
                break;
                case Team.yellow:
                    srenderer.sprite = billionYellow;
                break;
                case Team.green:
                    srenderer.sprite = billionGreen;
                break;
            }

            //Fixes overlapping when spawned
            Invoke("FixOverlap",0.5f);
        }

        private void FixOverlap() {
            transform.position = transform.position + new Vector3(0.01f,0.01f,0);
        }
    }
}
