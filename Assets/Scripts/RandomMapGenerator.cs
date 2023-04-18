using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BillionGame {
    public class RandomMapGenerator : MonoBehaviour {

        //Components
        [SerializeField] private GameObject testObject;
        private Tilemap worldGrid;
        private Tilemap blockingGrid;

        //Fields
        [SerializeField] private Tile floor;
        [SerializeField] private Tile wall;
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
                    if ((-size.x <= x) && (x < size.x) && (-size.y <= y) && (y < size.y)) outerWalls = false;

                    if ((worldGrid.GetTile<Tile>(thisVector) != floor) || (outerWalls)) {
                        worldGrid.SetTile(thisVector, wall);
                        blockingGrid.SetTile(thisVector, wall);
                    }
                }
            }
        }

        private void GenerateHoles() {
            Vector2Int leftSpawn = new Vector2Int(Random.Range(-size.x+1,-size.x+edgeArea.x),Random.Range(-size.y+1,size.y));
            Vector2Int rightSpawn = new Vector2Int(Random.Range(size.x-edgeArea.x,size.x),Random.Range(-size.y+1,size.y)); 
            Vector2Int upSpawn;
            while (true) {
                upSpawn = new Vector2Int(Random.Range(-size.x+1,size.x),Random.Range(size.y-edgeArea.y,size.y));
                if (Vector2Int.Distance(upSpawn,leftSpawn) >= minBaseDistance &&
                    Vector2Int.Distance(upSpawn,rightSpawn) >= minBaseDistance) break;
            }
            Vector2Int downSpawn;
            while (true) {
                downSpawn = new Vector2Int(Random.Range(-size.x+1,size.x),Random.Range(-size.y+1,-size.y+edgeArea.y));
                if (Vector2Int.Distance(downSpawn,leftSpawn) >= minBaseDistance &&
                    Vector2Int.Distance(downSpawn,rightSpawn) >= minBaseDistance) break;
            }

            SpawnHole(new Vector2Int(0,0));
            SpawnHole(leftSpawn);
            SpawnHole(rightSpawn);
            SpawnHole(upSpawn);
            SpawnHole(downSpawn);


            //Then connect holes from spawns to 0,0
            //Randomly select which spawn is which team's base
            //Change team's base spawn angle depending on the side of the arena they spawn
            //Add dynamic wall tiles? Solidify the games' resolution?
            

            Instantiate(testObject, new Vector2(leftSpawn.x,leftSpawn.y), transform.rotation);
            Instantiate(testObject, new Vector2(rightSpawn.x,rightSpawn.y), transform.rotation);
            Instantiate(testObject, new Vector2(upSpawn.x,upSpawn.y), transform.rotation);
            Instantiate(testObject, new Vector2(downSpawn.x,downSpawn.y), transform.rotation);
        }

        private Vector2Int SpawnHole(Vector2Int position) {
            Vector2Int holeSize = new Vector2Int(Random.Range(minHoleSize.x,maxHoleSize.x), Random.Range(minHoleSize.y,maxHoleSize.y));

            for (int x = position.x-holeSize.x; x < position.x+holeSize.x; x++) {
                for (int y = position.y-holeSize.y; y < position.y+holeSize.y; y++) {
                    worldGrid.SetTile(new Vector3Int(x,y,0), floor);
                }
            }


            return holeSize;
        }

    }
}