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

        public ITemperature GetTemperature(Vector2Int position) => crateCells[position];

        public IMovable GetMovable(Vector2Int position) => crateCells[position];

        public bool GetCrate(Vector2Int position, out Crate crate) {
            return crateCells.TryGetValue(position, out crate);
        }

        // return adjacent crates around given position
        // adjacent crates should be unique
        // however, we do not check for duplicates with the given position
        // used by player Move()
        public HashSet<T> GetAdjacent<T>(Vector2Int pos) where T : class {
            var set = new HashSet<T>();
            foreach (var dir in Defs.directions) {
                var dest = pos + dir;
                if (GetCrate(dest, out var adjacent) && adjacent is T type) {
                    // feat: heat shield
                    if (typeof(ITemperature).IsAssignableFrom(typeof(T)) &&
                    adjacent.units[dest - adjacent.position].borders[-dir].type != BorderType.shield) {
                        continue;
                    }
                    set.Add(type);
                }
            }
            return set;
        }

        // used by crate Move()
        public HashSet<T> GetAdjacent<T>(Crate crate) where T : class {
            var set = new HashSet<T>(); // unique adjacent crates
            var pos = crate.position;
            foreach (var offset in crate.offsets) {
                foreach (var dir in Defs.directions) {
                    var dest = pos + offset + dir;
                    if (GetCrate(dest, out var adjacent) && // get neighbor crate
                            adjacent != crate && // deduplicate
                            adjacent is T type) { // convert to T
                        // feat: heat shield
                        if (typeof(ITemperature).IsAssignableFrom(typeof(T)) &&
                                crate.units[offset].borders[dir].type != BorderType.shield && // check my heat shield
                                adjacent.units[dest - adjacent.position].borders[-dir].type != BorderType.shield) { // check neighbor's heat shield
                            continue;
                        }
                        set.Add(type);
                    }
                }
            }
            return set;
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
                if (IsCrateCellOccupied(dest) && GetMovable(dest) != GetMovable(pos)) {
                    if (!GetMovable(dest).BePushed(dir)) {
                        Debug.Log("GridSystem: chain push failed");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
