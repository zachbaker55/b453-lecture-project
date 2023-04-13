using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace BillionGame {
    public class GameManager : MonoBehaviour {
        //Singleton
        public static GameManager Instance;

        //Fields
        [SerializeField] private GameObject flagInstance;
        [SerializeField] private RadialBar _radialBar;

        //Properties
        [SerializeField] private List<Team> _teamList;
        public List<Team> TeamList {
            get { return _teamList; }
        }


        [SerializeField] private List<Sprite> _billionSpriteList;
        public List<Sprite> BillionSpriteList {
            get { return _billionSpriteList; }
        }
        [SerializeField] private List<Sprite> _levelSpriteList;
        public List<Sprite> LevelSpriteList {
            get { return _levelSpriteList; }
        }

        [SerializeField] private Teams _playerTeam;
        public Teams PlayerTeam {
            get { return _playerTeam; }
            set { _playerTeam = value; }
        }

        [SerializeField] private int _flagLimit;
        public int FlagLimit {
            get { return _flagLimit; }
            set { _flagLimit = value; }
        }

        [SerializeField] private float _billionShootSpeed;
        public float BillionShootSpeed {
            get { return _billionShootSpeed; }
            set { _billionShootSpeed = value; }
        }

        [SerializeField] private float _baseShootSpeed;
        public float BaseShootSpeed {
            get { return _baseShootSpeed; }
            set { _baseShootSpeed = value; }
        }

        private int _levelCap;
        public int LevelCap {
            get { return _levelCap; }
            set { _levelCap = _levelSpriteList.Count - 1; }
        }

        private Vector2 _mousePosition = Vector2.zero;
        public Vector2 MousePosition {
            get { return _mousePosition; }
        }

        //Events
        public static event Action BillionShootEvent;
        public static event Action BaseShootEvent;


        //Methods
        private void Awake() { 
            //Singleton check
            if (GameManager.Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            LevelCap = _levelSpriteList.Count - 1;
            InvokeRepeating("StartBillionShootEvent",_billionShootSpeed,_billionShootSpeed);
            InvokeRepeating("StartBaseShootEvent",_baseShootSpeed,_baseShootSpeed);
        }

        private void Update() {
            CheckInputs();
        }


        public Color GetTeamBackgroundColor(Teams teams) {
            Color color = Color.white;
            if (_teamList.Count > (int) teams) {
                color = _teamList[(int) teams].BackgroundColor;
            }
            return color;
        }

        public Color GetTeamMainColor(Teams teams) {
            Color color = Color.white;
            if (_teamList.Count > (int) teams) {
                color = _teamList[(int) teams].MainColor;
            }
            return color;
        }

        public List<Entity> GetTeamEntityList(Teams teams) {
            List<Entity> list = new List<Entity>();
            if (_teamList.Count > (int) teams) {
                list = _teamList[(int) teams].EntityList;
            }
            return list;
        }
        public List<Flag> GetTeamFlagList(Teams teams) {
            List<Flag> list = new List<Flag>();
            if (_teamList.Count > (int) teams) {
                list = _teamList[(int) teams].FlagList;
            }
            return list;
        }

        public void AddToEntityList(Teams teams, Entity entity) {
            if (_teamList.Count > (int) teams) {
                _teamList[(int) teams].AddEntity(entity);
            }
        }
        public void RemoveFromEntityList(Teams teams, Entity entity) {
            if (_teamList.Count > (int) teams) {
                _teamList[(int) teams].RemoveEntity(entity);
            }
        }

        public void AddToFlagList(Teams teams, Flag flag) {
            if (_teamList.Count > (int) teams) {
                _teamList[(int) teams].AddFlag(flag);
            }
        }
        public void RemoveFromFlagList(Teams teams, Flag flag) {
            if (_teamList.Count > (int) teams) {
                _teamList[(int) teams].RemoveFlag(flag);
            }
        }

        public Shader GetRadialShader() {
            return _radialBar.Shader;
        }

        public Color GetXPColor() {
            return _radialBar.XPColor;
        }

        //Checks for inputs, switches team or places flag as requested
        private void CheckInputs() {
            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetButtonDown("SwitchTeam1")){
                PlayerTeam = Teams.red;
                Debug.Log("Dev-switched to Red team.");
            }
            if (Input.GetButtonDown("SwitchTeam2")){
                PlayerTeam = Teams.blue;
                Debug.Log("Dev-switched to Blue team.");
            }
            if (Input.GetButtonDown("SwitchTeam3")){
                PlayerTeam = Teams.yellow;
                Debug.Log("Dev-switched to Yellow team.");
            }
            if (Input.GetButtonDown("SwitchTeam4")){
                PlayerTeam = Teams.green;
                Debug.Log("Dev-switched to Green team.");
            }
            if (Input.GetButtonDown("RightClick")){
                CreateFlag();
            }
        }

        //Creates a flag of PlayerTeam at mouse position
        //If Flag Limit is hit, takes closest flag and moves it to mouse position
        private void CreateFlag() {
            List<Flag> flagList = GetTeamFlagList(PlayerTeam);
            if (flagList.Count < FlagLimit) {  
                GameObject flagObject = Instantiate(flagInstance, _mousePosition, Quaternion.identity);
                Flag flag = flagObject.GetComponent<Flag>();
                flag.Team = PlayerTeam;
            } else {
                if (flagList.Count == 0) { return; }
                float smallestDistance = Mathf.Infinity;
                Flag closestFlag = flagList[0];
                foreach (Flag flag in flagList) {
                    float distance = Vector2.Distance(_mousePosition,flag.transform.position);
                        if (distance < smallestDistance) {
                        smallestDistance = distance;
                        closestFlag = flag;
                    }
                }
                closestFlag.transform.position = _mousePosition;
            }
        }

        private void StartBillionShootEvent() {
            BillionShootEvent?.Invoke();
        }

        private void StartBaseShootEvent() {
            BaseShootEvent?.Invoke();
        }



    }
}
