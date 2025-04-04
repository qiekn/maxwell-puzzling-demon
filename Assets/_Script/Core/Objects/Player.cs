using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class Player : MonoBehaviour, IMovable, ITemperature {

        [SerializeField] SpriteRenderer circleSpriteRenderer;
        [SerializeField] SpriteRenderer decorationSpriteRenderer;
        [SerializeField] Vector2Int position;
        [SerializeField] Temperature temperature;

        GridManager gm;

        public GridManager GM => gm;
        public Vector2Int Position => position;

        void Start() {
            gm = FindFirstObjectByType<GridManager>().GetComponent<GridManager>();
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

        public bool TryMove(Vector2Int dir) {
            var dest = position + dir;
            if (gm.IsObstacle(dest)) {
                return false;
            }
            // try push others
            if (gm.Get<Crate>(dest, out var other) && !MoveSystem.Instance.TryMove(dir, other)) {
                return false;
            }
            Move(dir);
            return true;
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

        public void Move(Vector2Int dir) {
            position += dir;
            transform.position = gm.CellToWorld(position) + Defs.playerOffset;

            // heat system
            temperature = Temperature.Neutral;
            HeatSystem.Instance.Register(this);
            UpdateColor();

            // every time player moved
            // update game state
            GameManager.Instance.UpdateGame();
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
