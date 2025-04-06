using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    using bt = BorderType;
    using Filter = AdjacentFilter;

    public enum AdjacentFilter {
        None,
        StickyCrate,
        Temperature,
        Merge,
    }

    public class GridManager : MonoBehaviour {
        Grid grid;
        Dictionary<Vector2Int, BaseCrate> crateCells;
        HashSet<Vector2Int> groundCells;

        void Awake() {
            grid = GetComponent<Grid>();
            MoveSystem.Instance.GM = this;
            crateCells = new();
            groundCells = new();
        }

        public void DebugPrint() {
            Debug.Log("crateCells");
            foreach (var pos in crateCells.Keys) {
                Debug.Log(pos + " " + crateCells[pos]);
            }
        }

        /*─────────────────────────────────────┐
        │             Grid Buffer              │
        └──────────────────────────────────────*/

        public void RegisterCrate(BaseCrate crate) {
            foreach (var pos in crate.GetUnitsPosition()) {
                if (crateCells.ContainsKey(pos)) {
                    Debug.LogError("GridSystem: crate unit already exists at " + pos);
                }
                crateCells[pos] = crate;
            }
        }

        public void UnRegisterCrate(BaseCrate crate) {
            foreach (var pos in crate.GetUnitsPosition()) {
                if (crateCells.ContainsKey(pos)) {
                    crateCells.Remove(pos);
                }
            }
        }

        public void RegisterGrounds(List<Vector2Int> grounds) {
            foreach (var ground in grounds) {
                groundCells.Add(ground);
            }
        }

        public void Clear() {
            crateCells.Clear();
            groundCells.Clear();
        }

        /*─────────────────────────────────────┐
        │              Grid Info               │
        └──────────────────────────────────────*/

        public Vector3 CellToWorld(Vector2Int pos) => grid.CellToWorld(new(pos.x, pos.y, 0));

        public bool IsWall(Vector2Int pos) => !groundCells.Contains(pos);

        public bool IsCrateCellOccupied(Vector2Int position) => crateCells.ContainsKey(position);

        public bool Get<T>(Vector2Int pos, out T result) where T : class {
            if (crateCells.TryGetValue(pos, out BaseCrate crate)) {
                result = crate as T;
                return result != null;
            }
            result = null;
            return false;
        }

        /*
        public bool Get<T>(Vector2Int pos, out T res) where T : class {
            res = crateCells.TryGetValue(pos, out var crate) ? crate as T : null;
            return res != null;
        }
        */

        public HashSet<T> GetAdjacent<T>(BaseCrate crate, Filter filter = Filter.None) where T : class {
            var res = new HashSet<T>(); // unique adjacent crates
            var pos = crate.position;
            foreach (var offset in crate.shape) {
                foreach (var dir in Defs.directions) {
                    var dest = pos + offset + dir;
                    if (Get<Crate>(dest, out var adjacent) && adjacent != crate && adjacent is T type) {
                        var a = crate.units[offset].borders[dir].type;
                        var b = adjacent.units[dest - adjacent.position].borders[-dir].type;
                        // feat: heat shield
                        if (filter == Filter.Temperature) {
                            if (a != bt.shield && b != bt.shield) { // check my and neighbor's heat shield
                                res.Add(type);
                            }
                        }
                        // feat: sticky border
                        else if (filter == Filter.StickyCrate) {
                            if (a != bt.none && (a == bt.sticky || b == bt.sticky)) {
                                res.Add(type);
                            }
                        }
                        // feat: merge
                        else if (filter == Filter.Merge) {
                            if ((a == bt.sticky || b == bt.sticky) && a != bt.shield && b != bt.shield) {
                                res.Add(type);
                            }
                        }
                        // normal: get crate
                        else {
                            res.Add(type);
                        }
                    } // adjacent
                } // inner loop
            } // outer loop
            return res;
        }
    }
}
