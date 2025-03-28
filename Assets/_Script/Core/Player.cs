using UnityEngine;
using UnityEngine.UI;

namespace qiekn.core {
    public class Player : MonoBehaviour {

        [SerializeField] SpriteRenderer decorationSpriteRenderer;

        LayerMask groundLayerMask;

        void Start() {
            groundLayerMask = LayerMask.GetMask("Ground");
        }

        void Update() {
            UpdatePlayerInput();
        }


        void UpdatePlayerInput() {
            if (Input.GetKeyDown(KeyCode.W)) {
                Move(Vector2Int.up);
            } else if (Input.GetKeyDown(KeyCode.S)) {
                Move(Vector2Int.down);
            } else if (Input.GetKeyDown(KeyCode.A)) {
                Move(Vector2Int.left);
                Turn(true);
            } else if (Input.GetKeyDown(KeyCode.D)) {
                Move(Vector2Int.right);
                Turn(false);
            }
        }

        void Move(Vector2Int dir) {
            var step = Defs.Unit;
            var pos = transform.position;
            var dest = new Vector3(pos.x + dir.x * step, pos.y + dir.y * step, pos.z);
            if (CanMove(dest)) {
                transform.position = dest;
            }
        }

        bool CanMove(Vector2 dest) {
            // is ground?
            // is dest reachable?
            var hit = Physics2D.OverlapPoint(dest, groundLayerMask);
            if (hit != null) {
                return true;
            }
            return false;
        }

        void Turn(bool left) {
            if (left) {
                decorationSpriteRenderer.flipX = true;
            } else {
                decorationSpriteRenderer.flipX = false;
            }
        }
    }
}
