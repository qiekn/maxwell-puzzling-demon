using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public enum AdjacentFilter {
        None,
        StickyCrate,
        Temperature,
        Merge,
    }

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
            MoveSystem.Instance.GM = this;
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

        public bool Get<T>(Vector2Int pos, out T result) where T : class {
            if (crateCells.TryGetValue(pos, out Crate crate)) {
                result = crate as T;
                return result != null;
            }
            result = null;
            return false;
        }

        // return adjacent crates around given position
        // adjacent crates should be unique
        // however, we do not check for duplicates with the given position
        // used by player case
        public HashSet<T> GetAdjacent<T>(Vector2Int pos, AdjacentFilter filter = AdjacentFilter.None) where T : class {
            var set = new HashSet<T>();
            foreach (var dir in Defs.directions) {
                var dest = pos + dir;
                if (Get<Crate>(dest, out var adjacent) && adjacent is T type) {
                    // feat: heat shield
                    if (filter == AdjacentFilter.Temperature &&
                            typeof(ITemperature).IsAssignableFrom(typeof(T)) &&
                            adjacent.units[dest - adjacent.position].borders[-dir].type == BorderType.shield) {
                        continue;
                    }
                    set.Add(type);
                }
            }
            return set;
        }

        // used by crate Move()
        public HashSet<T> GetAdjacent<T>(Crate crate, AdjacentFilter filter = AdjacentFilter.None) where T : class {
            var set = new HashSet<T>(); // unique adjacent crates
            var pos = crate.position;
            foreach (var offset in crate.offsets) {
                foreach (var dir in Defs.directions) {
                    var dest = pos + offset + dir;
                    if (Get<Crate>(dest, out var adjacent) && adjacent != crate && adjacent is T type) {
                        var crateBorderType = crate.units[offset].borders[dir].type;
                        var adjacentBorderType = adjacent.units[dest - adjacent.position].borders[-dir].type;
                        // feat: heat shield
                        if (filter == AdjacentFilter.Temperature &&
                                typeof(ITemperature).IsAssignableFrom(typeof(T)) && (
                                crate.units[offset].borders[dir].type == BorderType.shield || // check myself heat shield
                                adjacent.units[dest - adjacent.position].borders[-dir].type == BorderType.shield)) { // check neighbor's heat shield
                        }
                        // feat: sticky border
                        else if (filter == AdjacentFilter.StickyCrate) {
                            if (crateBorderType == BorderType.sticky || adjacentBorderType == BorderType.sticky) {
                                set.Add(type);
                            }
                        }
                        // feat: merge
                        else if (filter == AdjacentFilter.Merge) {
                            if ((crateBorderType == BorderType.sticky || adjacentBorderType == BorderType.sticky) &&
                                    crateBorderType != BorderType.shield &&
                                    adjacentBorderType != BorderType.shield) {
                                set.Add(type);
                            }
                        }
                        // normal: get crate
                        else {
                            set.Add(type);
                        }
                    }
                }
            }
            return set;
        }
    }
}
