using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class Crate : MonoBehaviour, IMoveable, ITemperature {

        #region Field

        // caches
        [SerializeField] SpriteRenderer backgroundSR; // background sprite
        [SerializeField] SpriteRenderer BorderSR; // border sprite

        [SerializeField] Temperature temperature;

        public Vector2Int position; // grid position
        public List<Vector2Int> offsets; // crate shape

        public Dictionary<Vector2Int, Unit> units; // use this info to disable inner border
        public List<Border> borders; // outline borders

        GridManager gm;

        #endregion

        #region Unity

        void Start() {
            gm = FindFirstObjectByType<GridManager>();
            /*gm.RegisterCrate(this);*/
            UpdateBorders();
            UpdateSprites();
            UpdateColor();
        }

        #endregion

        public void Init(CrateData data) {
            this.temperature = data.Temperature;
            this.position = data.Position;
            this.offsets = data.Shape;
        }

        public void UpdateBorders() {
            // generate units
            units = new Dictionary<Vector2Int, Unit>();
            foreach (var offset in offsets) {
                units.Add(offset, new Unit(offset));
            }
            // generate borders
            // and also check neighbors for each unit to disable inner border
            // e.g. if has up direction neighbor, disable up border
            borders = new List<Border>();
            foreach (var pos in offsets) {
                var unit = units[pos];
                foreach (var dir in Defs.directions) {
                    if (units.ContainsKey(pos + dir)) {
                        unit.borders[dir].type = BorderType.none;
                    } else {
                        borders.Add(unit.borders[dir]);
                    }
                }
            }
        }

        /*─────────────────────────────────────┐
        │               Renderer               │
        └──────────────────────────────────────*/

        Vector2Int CalculateBounds() {
            int width = 0;
            int height = 0;
            foreach (var offset in offsets) {
                width = Mathf.Max(width, offset.x + 1);
                height = Mathf.Max(height, offset.y + 1);
            }

            int texWidth = width * Defs.CellSize;
            int texHeight = height * Defs.CellSize;
            return new(texWidth, texHeight);
        }

        Texture2D GenerateTexture() {
            var bounds = CalculateBounds();
            int texWidth = bounds.x;
            int texHeight = bounds.y;

            var texture = new Texture2D(texWidth, texHeight);

            // init texture color --> transparent
            var pixels = new Color32[texWidth * texHeight];
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = Color.clear;
            }
            texture.SetPixels32(pixels);

            return texture;
        }

        public void UpdateSprites() {
            GenerateBackgroundSprite();
            GenerateBorderSprite();
        }

        public void GenerateBackgroundSprite() {
            var texture = GenerateTexture();
            foreach (var offset in offsets) {
                var startX = offset.x * Defs.CellSize;
                var startY = offset.y * Defs.CellSize;
                for (int y = startY; y < startY + Defs.CellSize; y++) {
                    for (int x = startX; x < startX + Defs.CellSize; x++) {
                        texture.SetPixel(x, y, Color.white);
                    }
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            var sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0, 0));
            sprite.name = "background";
            backgroundSR.sprite = sprite;
        }

        public void GenerateBorderSprite() {
            var texture = GenerateTexture();
            var cellSize = Defs.CellSize;
            var borderSize = Defs.BorderSize;
            foreach (var border in borders) {
                if (border.dir == Vector2Int.up || border.dir == Vector2Int.down) {
                    Utils.DrawHorizontalBorder(texture, border, cellSize, borderSize);
                } else if (border.dir == Vector2Int.left || border.dir == Vector2Int.right) {
                    Utils.DrawVerticalBorder(texture, border, cellSize, borderSize);
                }
            }



            // fix inner corner
            bool PixelExist(int x, int y) {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height) {
                    return texture.GetPixel(x, y) == Color.white;
                }
                return false;
            }
            var fixPos = new List<Vector2Int>();
            for (int y = 0; y < texture.width; y++) {
                for (int x = 0; x < texture.height; x++) {
                    foreach (var dir in Defs.crossDirections) {
                        var offsetX = dir.x * borderSize;
                        var offsetY = dir.y * borderSize;
                        if (!PixelExist(x, y) &&
                                PixelExist(x + offsetX, y) &&
                                PixelExist(x, y + offsetY) &&
                                !PixelExist(x + offsetY, y + offsetY) &&
                                PixelExist(x + offsetX * 2, y) &&
                                PixelExist(x, y + offsetY * 2)) {
                            fixPos.Add(new(x, y));
                        }
                    }
                }
            }
            foreach (var pos in fixPos) {
                texture.SetPixel(pos.x, pos.y, Color.white);
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            var sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0, 0));
            sprite.name = "borders";
            BorderSR.sprite = sprite;
        }

        /*─────────────────────────────────────┐
        │               Movable                │
        └──────────────────────────────────────*/

        /*
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
        */

        /*─────────────────────────────────────┐
        │             Temperature              │
        └──────────────────────────────────────*/

        public int GetTemperature() {
            return 0;
        }

        public void SetTemperature(int val_) {
            UpdateColor();
            return;
        }

        public void UpdateColor() {
            switch (temperature) {
                case Temperature.Hot:
                    backgroundSR.color = Defs.RED;
                    BorderSR.color = Defs.DARKRED;
                    break;
                case Temperature.Cold:
                    backgroundSR.color = Defs.BLUE;
                    BorderSR.color = Defs.DARKBLUE;
                    break;
                case Temperature.Neutral:
                    backgroundSR.color = Defs.GRAY;
                    BorderSR.color = Defs.MIDGRAY;
                    break;
                case Temperature.Magic:
                    backgroundSR.color = Defs.GREEN;
                    BorderSR.color = Defs.DARKGREEN;
                    break;
            }
        }
    }
}
