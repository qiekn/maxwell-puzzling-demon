using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class Crate : MonoBehaviour, IMoveable, ITemperature {
        [SerializeField] int val; // temperature
        public Vector2Int position;
        public List<Vector2Int> shape;

        [SerializeField] SpriteRenderer background;
        // [SerializeField] SpriteRenderer border;

        GridManager gm;

        void Start() {
            gm = FindFirstObjectByType<GridManager>();
            gm.RegisterCrate(this);
            UpdateColor();
        }

        /*─────────────────────────────────────┐
        │               Movable                │
        └──────────────────────────────────────*/

        public bool CanMove(Vector2Int direction) {
            var dest = position + direction;
            return gm.CanMoveTo(dest, this);
        }

        public void Move(Vector2Int direction) {
            if (CanMove(direction)) {
                gm.UnRegisterCrate(this);
                position += direction;
                gm.RegisterCrate(this);
                transform.position = new Vector3(position.x, position.y, 0);
            }
        }

        /*─────────────────────────────────────┐
        │             Temperature              │
        └──────────────────────────────────────*/

        public int GetTemperature() {
            return val;
        }

        public void SetTemperature(int val_) {
            if (val_ != val) {
                val = val_;
                UpdateColor();
            }
        }

        void UpdateColor() {
            if (val > 0) {
                background.color = Defs.RED;
                // border.color = Defs.DARKRED;
            } else if (val < 0) {
                background.color = Defs.BLUE;
                // border.color = Defs.DARKBLUE;
            } else if (val == 0) {
                background.color = Defs.GRAY;
                // border.color = Defs.DARKGRAY;
            }
        }

    }
}
