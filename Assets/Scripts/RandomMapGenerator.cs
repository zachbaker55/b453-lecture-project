using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BillionGame {
    public class RandomMapGenerator : MonoBehaviour {

        //Components
        [SerializeField] private GameObject baseObject;
        private Tilemap worldGrid;
        private Tilemap blockingGrid;

        //Fields
        [SerializeField] private Tile floor;
        [SerializeField] private RuleTile wall;
        [SerializeField] private Vector2Int size;
        [SerializeField] private int wallSpace;
        [SerializeField] private Vector2Int edgeArea;
        [SerializeField] private Vector2Int minHoleSize;
        [SerializeField] private Vector2Int maxHoleSize;
        [SerializeField, Range(2,14)] private int minBaseDistance;

        //Methods
        private void Awake() {
            GetTilemaps();
            GenerateHoles();
            GenerateBlockingMap();
        }

        //Puts Tilemap components into fields
        private void GetTilemaps() {
            int index = 0;
            foreach (Transform child in transform) {
                if (index == 0) { worldGrid = child.gameObject.GetComponent<Tilemap>(); }
                if (index == 1) { blockingGrid = child.gameObject.GetComponent<Tilemap>(); }
                index++;
            }
        }

        //Generates a map completely full of walls
        private void GenerateBlockingMap() {
            for (int x = -size.x-wallSpace; x < size.x+wallSpace; x++) {
                for (int y = -size.y-wallSpace; y < size.y+wallSpace; y++) {
                    Vector3Int thisVector = new Vector3Int(x,y,0);
                    bool outerWalls = true;
                    if ((-size.x <= x) && (x < size.x) && (-size.y <= y) && (y < size.y)) {
                        outerWalls = false;
                    }

                    if ((worldGrid.GetTile<Tile>(thisVector) != floor) || (outerWalls)) {
                        blockingGrid.SetTile(thisVector, wall);
                    }
                }
            }
        }

        private void GenerateHoles() {

            List<Team> teamList = GameManager.Instance.TeamList;
            List<Team> shuffledList = teamList.OrderBy( x => Random.value ).ToList();
            List<Vector2Int> spawnsList = new List<Vector2Int>();
            int index = 0;
            foreach (Team team in shuffledList) {
                Vector2Int spawnPos = new Vector2Int(0,0);
                while (true) {
                    if (index == 0) {
                        spawnPos = new Vector2Int(Random.Range(-size.x+1,-size.x+edgeArea.x),Random.Range(-size.y+1,size.y));
                    } else if (index == 1) {
                        spawnPos = new Vector2Int(Random.Range(size.x-edgeArea.x,size.x),Random.Range(-size.y+1,size.y)); 
                    } else if (index == 2) {
                        spawnPos = new Vector2Int(Random.Range(-size.x+1,size.x),Random.Range(size.y-edgeArea.y,size.y));
                    } else if (index == 3) {
                        spawnPos = new Vector2Int(Random.Range(-size.x+1,size.x),Random.Range(-size.y+1,-size.y+edgeArea.y));
                    }
                    //Break out if not in minBaseDistance
                    bool outOfRange = true;
                    foreach (Vector2Int otherPos in spawnsList) {
                        if (Vector2Int.Distance(spawnPos,otherPos) <= minBaseDistance) {
                            outOfRange = false;
                        }
                    }
                    if (outOfRange) {
                        break;
                    }
                }
                MakeTunnel(Vector2Int.zero,spawnPos);
                SpawnHole(spawnPos);
                GameObject spawnedBase = Instantiate(baseObject, new Vector2(spawnPos.x,spawnPos.y), transform.rotation);
                BillionBase baseComp = spawnedBase.GetComponent<BillionBase>();
                baseComp.Team = team.TeamEnum;
                Vector2 targetPos = Vector2.zero;
                targetPos.x = targetPos.x - spawnPos.x;
                targetPos.y = targetPos.y - spawnPos.y;
                baseComp.SpawnAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
                spawnsList.Add(spawnPos);
                index++;
            }
        }

        //Spawns random hole at position
        private Vector2Int SpawnHole(Vector2Int position) {
            Vector2Int holeSize = new Vector2Int(Random.Range(minHoleSize.x,maxHoleSize.x), Random.Range(minHoleSize.y,maxHoleSize.y));
            for (int x = position.x-holeSize.x; x < position.x+holeSize.x; x++) {
                for (int y = position.y-holeSize.y; y < position.y+holeSize.y; y++) {
                    worldGrid.SetTile(new Vector3Int(x,y,0), floor);
                }
            }
            return holeSize;
        }


        //Makes tunnels between a starting position and an ending position
        private void MakeTunnel(Vector2Int startingPos, Vector2Int endPos) {
            Vector2Int holeSize = SpawnHole(startingPos);
            int horizontalShifted = endPos.x;
            int verticalShifted = endPos.y;

            if (endPos.x > startingPos.x+holeSize.x) {
                horizontalShifted = startingPos.x+holeSize.x;
            } else if (endPos.x < startingPos.x-holeSize.x) {
                horizontalShifted = startingPos.x-holeSize.x;
            }

            if (endPos.y > startingPos.y+holeSize.y) {
                verticalShifted = startingPos.y+holeSize.y;
            } else if (endPos.y < startingPos.y-holeSize.y) {
                verticalShifted = startingPos.y-holeSize.y;
            }


            Vector2Int newPos = new Vector2Int(horizontalShifted, verticalShifted);
            if (newPos != endPos) {
                MakeTunnel(newPos, endPos);
            }
        }
    }
}