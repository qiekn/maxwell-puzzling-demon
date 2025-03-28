using UnityEngine;

namespace qiekn.core {
    public class Player : MonoBehaviour {

        [SerializeField] SpriteRenderer decorationSpriteRenderer;

        void Update() {
            UpdatePlayerInput();
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

        void TryMove(Vector2Int dir) {
            var dest = Utils.CalculateDest(transform.position, dir);
            Move(dest);
        }

        void Move(Vector3 dest) {
            transform.position = dest;
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
