using UnityEngine;
using System.Collections.Generic;



namespace BillionGame {
    public enum Team {
            red,
            blue,
            yellow,
            green
    }

    public enum State {
            free,
            drawing
    }


    //Singleton Component. Controls universal aspects of the game.
    public class GameManager : MonoBehaviour {

        //Singleton
        public static GameManager Instance;


        public GameObject flagPrefab;
        public Team PlayerTeam;
        public int FlagLimit;

        [HideInInspector] public State state = State.free;

        [HideInInspector] public List<GameObject> redFlags = new List<GameObject>();
        [HideInInspector] public List<GameObject> blueFlags = new List<GameObject>();
        [HideInInspector] public List<GameObject> yellowFlags = new List<GameObject>();
        [HideInInspector] public List<GameObject> greenFlags = new List<GameObject>();

        private Vector2 mousePos;   

        private void Awake() { 
            //Singleton check
            if (GameManager.Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Update() {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetButtonDown("SwitchTeam1")){
                PlayerTeam = Team.red;
            }
            if (Input.GetButtonDown("SwitchTeam2")){
                PlayerTeam = Team.blue;
            }
            if (Input.GetButtonDown("SwitchTeam3")){
                PlayerTeam = Team.yellow;
            }
            if (Input.GetButtonDown("SwitchTeam4")){
                PlayerTeam = Team.green;
            }
            if (Input.GetButtonDown("Click")){
                if (state == State.free) {
                    switch (PlayerTeam) {
                        case Team.red:
                            CreateFlag(redFlags);
                        break;
                        case Team.blue:
                            CreateFlag(blueFlags);
                        break;
                        case Team.yellow:
                            CreateFlag(yellowFlags);
                        break;
                        case Team.green:
                            CreateFlag(greenFlags);
                        break;
                    }
                }
                
            }
            
        }

        private void CreateFlag(List<GameObject> flagArray) {
            if (flagArray.Count < FlagLimit) { 
                GameObject flagObject = Instantiate(flagPrefab, mousePos, Quaternion.identity);
                Flag flag = flagObject.GetComponent<Flag>();
                flag.Team = PlayerTeam;
                flagArray.Add(flagObject);
            } else {
                float smallestDistance = Mathf.Infinity;
                GameObject closestFlag = flagArray[0];
                foreach (GameObject f in flagArray) {
                    float distance = Vector2.Distance(mousePos,f.transform.position);
                    if (distance < smallestDistance) {
                        smallestDistance = distance;
                        closestFlag = f;
                    }
                }
                closestFlag.transform.position = mousePos;
            }
        }


    }
}