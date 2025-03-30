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
        }

        public void RegisterCompositeCrate(Crate crate) {
            var pos = crate.position;
            foreach (var offset in crate.offsets) {
                if (crateCells.ContainsKey(pos + offset)) {
                    Debug.LogError("GridSystem: crate unit already exists at " + (pos + offset));
                }
                crateCells[pos + offset] = crate;
            }
        }

        public void UnRegisterCompositeCrate(Crate crate) {
            var pos = crate.position;
            foreach (var offset in crate.offsets) {
                crateCells.Remove(pos + offset);
            }
        }

        public bool IsCrateCellOccupied(Vector2Int position) {
            return crateCells.ContainsKey(position);
        }

        /*─────────────────────────────────────┐
        │         Cell / World Positon         │
        └──────────────────────────────────────*/
        public Vector3 CellToWorld(Vector2Int pos) {
            return grid.CellToWorld(new(pos.x, pos.y, 0));
        }

        /*
        public bool IsObstacle(Vector2Int pos) {
            return !groundCells.Contains(pos);
        }

        public bool CanMoveTo(Vector2Int pos, CompositeCrate crate) {
            foreach (var offset in crate.shape) {
                var dest = pos + offset;
                if (IsObstacle(dest) || (IsCellOccupied(dest) && occupiedCells[dest] != crate)) {
                    return false;
                }
            }
            return true;
        }
        */

    }
}
