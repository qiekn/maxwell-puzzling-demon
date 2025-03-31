using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class Player : MonoBehaviour, IMovable, ITemperature {

        [SerializeField] SpriteRenderer circleSpriteRenderer;
        [SerializeField] SpriteRenderer decorationSpriteRenderer;
        [SerializeField] Vector2Int position;
        [SerializeField] Temperature temperature;
        HeatSystem hm;
        GridManager gm;

        void Start() {
            gm = FindFirstObjectByType<GridManager>().GetComponent<GridManager>();
            hm = HeatSystem.Instance;
            temperature = Temperature.Neutral;
        }

        void Update() {
            UpdatePlayerInput();
        }

        // used for level manager
        public void Init(Vector2Int position_) {
            position = position_;
        }

        void UpdatePlayerInput() {
            if (Input.GetKeyDown(KeyCode.W)) {
                TryMove(Vector2Int.up);
            } else if (Input.GetKeyDown(KeyCode.S)) {
                TryMove(Vector2Int.down);
            } else if (Input.GetKeyDown(KeyCode.A)) {
                TryMove(Vector2Int.left);
                Turn(true);
            } else if (Input.GetKeyDown(KeyCode.D)) {
                TryMove(Vector2Int.right);
                Turn(false);
            }
        }


        // handle collision detection
        bool TryMove(Vector2Int dir) {
            var dest = position + dir;
            // no obsticle
            if (!gm.IsCrateCellOccupied(dest)) {
                Move(dest);
                return true;
            }
            Debug.Log("player move: try push other");
            // can push others
            var other = gm.GetMovable(dest);
            if (gm.CanMove(dir, other)) {
                other.BePushed(dir);
                Move(dest);
                return true;
            }
            Debug.Log("player move: can't push other");
            return false;
        }

        // handle move vfx, teleportation or movement animations
        void Move(Vector2Int dest) {
            position = dest;
            transform.position = gm.CellToWorld(dest) + Defs.playerOffset;
            temperature = Temperature.Neutral;
            hm.Register(this);
            hm.Register(gm.GetAdjacent<ITemperature>(position));
            hm.Balance();
        }

        void Turn(bool left) {
            if (left) {
                decorationSpriteRenderer.flipX = true;
            } else {
                decorationSpriteRenderer.flipX = false;
            }
        }

        /*─────────────────────────────────────┐
        │          IMovable Interface          │
        └──────────────────────────────────────*/

        public List<Vector2Int> GetUnitsPosition() {
            return new List<Vector2Int>{
                position
            };
        }

        public bool BePushed(Vector2Int dir) {
            return TryMove(dir);
        }

        /*─────────────────────────────────────┐
        │        ITemperature Interface        │
        └──────────────────────────────────────*/

        public int GetTemperature() {
            return (int)temperature;
        }

        public void SetTemperature(Temperature t) {
            temperature = t;
        }

        public void UpdateColor() {
            Utils.UpdateSpritesColor(circleSpriteRenderer, temperature);
            // temp solution:
            // my demon's gray color is bad,
            // so reset neutral color to midgray
            if (temperature == Temperature.Neutral) {
                circleSpriteRenderer.color = Defs.MIDGRAY;
            }

        }
    }
}
