using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class GridManager : MonoBehaviour {
        Grid grid;

        Dictionary<Vector2Int, Crate> occupiedCells;
        HashSet<Vector2Int> groundCells;

        void Start() {
            grid = GetComponent<Grid>();
        }

        public void RegisterCrate(Crate crate) {
            occupiedCells[crate.position] = crate;
        }

        public void UnRegisterCrate(Crate crate) {
            occupiedCells.Remove(crate.position);
        }

        void Generate() { }

        public bool IsCellOccupied(Vector2Int pos) {
            return occupiedCells.ContainsKey(pos);
        }

        public bool IsObstacle(Vector2Int pos) {
            return !groundCells.Contains(pos);
        }

        public bool CanMoveTo(Vector2Int pos, Crate crate) {
            foreach (var offset in crate.shape) {
                var dest = pos + offset;
                if (IsObstacle(dest) || (IsCellOccupied(dest) && occupiedCells[dest] != crate)) {
                    return false;
                }
            }
            return true;
        }

    }
}
