using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class Player : BaseCrate {

        [SerializeField] SpriteRenderer circleSpriteRenderer;
        [SerializeField] SpriteRenderer decorationSpriteRenderer;

        [SerializeField] Sprite deadSprite;

        Sprite normalSprite;
        bool isDead = false;

        protected override void Start() {
            base.Start();
            normalSprite = decorationSpriteRenderer.sprite;
            temperature = Temperature.Neutral;
        }

        private void Update() {
            if (temperature == Temperature.Hot) {
                isDead = true;
                decorationSpriteRenderer.sprite = deadSprite;
            } else if (isDead && temperature != Temperature.Hot) {
                isDead = false;
                decorationSpriteRenderer.sprite = normalSprite;
            }

            if (!isDead) {
                UpdatePlayerInput();
            }
        }

        // used for level manager
        public void Init(Vector2Int position_) {
            name = "player";
            position = position_;
            shape = new List<Vector2Int> { new(0, 0) };
            units = new Dictionary<Vector2Int, Unit> {
                { Vector2Int.zero, new Unit(Vector2Int.zero, BorderType.none) }
            };
            borders = new();
            foreach (var dir in Defs.directions) {
                borders.Add(units[Vector2Int.zero].borders[dir]);
            }
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
            return MoveSystem.Instance.TryMove(dir, this);
        }

        void Turn(bool left) {
            if (left) {
                decorationSpriteRenderer.flipX = true;
            } else {
                decorationSpriteRenderer.flipX = false;
            }
        }

        public override void Move(Vector2Int dir) {
            position += dir;
            transform.position = gm.CellToWorld(position) + Defs.playerOffset;

            HeatSystem.Instance.Register(this);
            // every time player moved
            // update game state
            GameManager.Instance.UpdateGame();
        }

        public override int GetTemperature() {
            return 0;
        }

        public override void UpdateColor() {
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
