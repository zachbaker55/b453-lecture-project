using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BillionGame {

    public enum Teams {
            red,
            blue,
            yellow,
            green
    }


    [System.Serializable]
    public class Team {
        //Fields
        [SerializeField] private Teams _teamEnum;
        public Teams TeamEnum  {
            get { return _teamEnum; }
        }

        [SerializeField] private Color _backgroundColor;
        public Color BackgroundColor {
            get { return _backgroundColor; }
        }
        [SerializeField] private Color _mainColor;
        public Color MainColor {
            get { return _mainColor; }
        }

        private List<Entity> _entityList = new List<Entity>();
        public List<Entity> EntityList {
            get { return _entityList; }
        }

        private List<Flag> _flagList = new List<Flag>();
        public List<Flag> FlagList {
            get { return _flagList; }
        }

        //Methods
        public void AddEntity(Entity entity) {
            _entityList.Add(entity);
        }

        public void RemoveEntity(Entity entity) {
            if (_entityList.Contains(entity)) {
                _entityList.Remove(entity);
            }
        }

        public void AddFlag(Flag flag) {
            _flagList.Add(flag);
        }

        public void RemoveFlag(Flag flag) {
            if (_flagList.Contains(flag)) {
                _flagList.Remove(flag);
            }
        }
    }
}
