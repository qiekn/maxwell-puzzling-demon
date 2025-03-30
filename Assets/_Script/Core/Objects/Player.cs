using System.Collections.Generic;
using Codice.Client.Common.FsNodeReaders;
using UnityEngine;
using UnityEngine.UIElements;

namespace qiekn.core {
    public class Player : MonoBehaviour, IMovable {

        [SerializeField] SpriteRenderer decorationSpriteRenderer;
        [SerializeField] Vector2Int position;
        GridManager gm;

        void Start() {
            gm = FindFirstObjectByType<GridManager>().GetComponent<GridManager>();
        }

        void Update() {
            UpdatePlayerInput();
        }

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
            var other = gm.GetCrate(dest);
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
    }
}
