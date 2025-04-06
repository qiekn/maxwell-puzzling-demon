using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace qiekn.core {
    // base crate
    // no sprite for renderer
    // just basic data structure
    public class BaseCrate : MonoBehaviour, ITemperature {

        #region Field

        public Temperature temperature;

        [Tooltip("base position, usually bottom left of the crate")]
        public Vector2Int position;

        [Tooltip("offset list, define the shape of the crate")]
        public List<Vector2Int> shape;

        [Tooltip("data structure for borders")]
        public Dictionary<Vector2Int, Unit> units;

        [Tooltip("references of outline borders on the crate, used to update sprite")]
        public List<Border> borders;

        protected GridManager gm;

        private bool registered = false;

        public GridManager GM => gm;

        #endregion

        #region Unity

        protected virtual void Awake() {
            gm = FindFirstObjectByType<GridManager>();
        }

        protected virtual void Start() {
            Register();
        }

        #endregion

        public void InitCrate(CrateData data) {
            name = "crate_" + data.Position;
            temperature = data.Temperature;
            position = data.Position;
            shape = new List<Vector2Int>(data.Shape);
            borders = data.BordersOverride.Select(b => b.DeepCopy()).ToList();
            units = new();
            InitUnits();
            InitBorders();

            void InitUnits() {
                // generate units by crate shape
                foreach (var offset in shape) {
                    units[offset] = new Unit(offset);
                }
                // override borders by level data
                foreach (var border in borders) {
                    if (!units.ContainsKey(border.pos)) {
                        Debug.Log("GridSystem: border pos not found at " + border.pos);
                    } else if (!units[border.pos].borders.ContainsKey(border.dir)) {
                        Debug.Log("GridSystem: border dir not found at " + border.pos + border.dir);
                    }
                    units[border.pos].borders[border.dir] = border;
                }
            }

            void InitBorders() {
                // gather references of outer borders
                // check neighbor units and disable inner border
                // e.g. if has up direction neighbor, disable up border
                foreach (var pos in shape) {
                    var unit = units[pos];
                    foreach (var dir in Defs.directions) {
                        // if not default type, this border is a override border, skip
                        if (unit.borders[dir].type != BorderType.conductive) {
                            continue;
                        }
                        // disable inner border
                        else if (units.ContainsKey(pos + dir)) {
                            unit.borders[dir].type = BorderType.none;
                        }
                        // normal case
                        else {
                            borders.Add(unit.borders[dir]);
                        }
                    }
                }
            }
        }

        public void DisableInnerPairedStickyBorders() {
            var map = new Dictionary<Vector2Int, Vector2Int>();
            foreach (var pos in shape) {
                foreach (var dir in Defs.directions) {
                    var dest = pos + dir;
                    // disable when two sticky border stay together
                    if (units.ContainsKey(pos + dir) &&
                            units[pos].borders[dir].type == BorderType.sticky &&
                            units[dest].borders[-dir].type == BorderType.sticky) {
                        units[pos].borders[dir].type = BorderType.none;
                        units[dest].borders[-dir].type = BorderType.none;
                        map[pos] = dir;
                        map[dest] = -dir;
                    }
                }
            }
            borders.RemoveAll(border => map.ContainsKey(border.pos) && border.dir == map[border.pos]);
        }

        public void Register() {
            registered = true;
            gm.RegisterCrate(this);
        }

        public void UnRegister() {
            registered = false;
            gm.UnRegisterCrate(this);
        }

        public void Destroy() {
            if (registered) {
                UnRegister();
            }
            Destroy(gameObject);
        }

        #region IMovable

        public virtual void Move(Vector2Int dir) {
            position += dir;
            transform.position = gm.CellToWorld(position);

            HeatSystem.Instance.Register(this);
        }

        // unit position -> crate's base pos + unit's offset
        public virtual List<Vector2Int> GetUnitsPosition() => shape.Select(x => x + position).ToList();

        #endregion

        #region ITemperature

        public virtual int GetTemperature() {
            return (int)temperature * shape.Count;
        }

        public virtual void SetTemperature(Temperature t) {
            temperature = t;
            UpdateColor();
        }

        public virtual void UpdateColor() {
            // sprite in this base class
            return;
        }

        #endregion

    }
}
