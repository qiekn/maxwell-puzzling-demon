using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class GridManager : MonoBehaviour {
        public Grid grid;

        Dictionary<Vector2Int, Crate> crateCells;
        HashSet<Vector2Int> groundCells;

        void Awake() {
            if (grid == null) {
                grid = GetComponent<Grid>();
                if (grid == null) {
                    Debug.LogError("GridManager: Grid component is null");
                } else {
                    Debug.Log("GridManager: You should say thanks.");
                }
            }
            crateCells = new Dictionary<Vector2Int, Crate>();
            groundCells = new HashSet<Vector2Int>();
        }

        /*─────────────────────────────────────┐
        │            Info Maintenan            │
        └──────────────────────────────────────*/

        public void RegisterCrate(Crate crate) {
            var pos = crate.position;
            foreach (var offset in crate.offsets) {
                if (crateCells.ContainsKey(pos + offset)) {
                    Debug.LogError("GridSystem: crate unit already exists at " + (pos + offset));
                }
                crateCells[pos + offset] = crate;
            }
        }

        public void UnRegisterCrate(Crate crate) {
            var pos = crate.position;
            foreach (var offset in crate.offsets) {
                crateCells.Remove(pos + offset);
            }
        }

        /*─────────────────────────────────────┐
        │             Info Provide             │
        └──────────────────────────────────────*/

        public Vector3 CellToWorld(Vector2Int pos) {
            return grid.CellToWorld(new(pos.x, pos.y, 0));
        }

        public bool IsObstacle(Vector2Int pos) {
            return false;
            // to-do
            // return !groundCells.Contains(pos);
        }

        public bool IsCrateCellOccupied(Vector2Int position) {
            return crateCells.ContainsKey(position);
        }

        public IMovable GetCrate(Vector2Int position) {
            return crateCells[position];
        }

        /*─────────────────────────────────────┐
        │               Physics                │
        └──────────────────────────────────────*/

        public bool CanMove(Vector2Int dir, IMovable obj) {
            foreach (var pos in obj.GetUnitsPosition()) {
                var dest = pos + dir;
                if (IsObstacle(dest)) {
                    Debug.Log("GridSystem: hit obstacle");
                    return false;
                }
                if (IsCrateCellOccupied(dest) && GetCrate(dest) != GetCrate(pos)) {
                    if (!GetCrate(dest).BePushed(dir)) {
                        Debug.Log("GridSystem: chain push failed");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
