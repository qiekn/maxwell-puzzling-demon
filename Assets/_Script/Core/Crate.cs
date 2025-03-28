using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class Crate : MonoBehaviour, IMoveable, ITemperature {
        [SerializeField] int val; // temperature
        [SerializeField] SpriteRenderer background;
        [SerializeField] SpriteRenderer border;

        [SerializeField] List<Vector3> positions;

        void Start() {
            UpdateColor();
        }

        void Update() {
        }

        void UpdateColor() {
            if (val > 0) {
                background.color = Defs.RED;
                border.color = Defs.DARKRED;
            } else if (val < 0) {
                background.color = Defs.BLUE;
                border.color = Defs.DARKBLUE;
            } else if (val == 0) {
                background.color = Defs.GRAY;
                border.color = Defs.DARKGRAY;
            }
        }

        /*─────────────────────────────────────┐
        │               Movable                │
        └──────────────────────────────────────*/

        public bool BePushed(Vector2Int dir) {
            var dests = Utils.CalculateDests(positions, dir);
            return true;
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

    }
}
