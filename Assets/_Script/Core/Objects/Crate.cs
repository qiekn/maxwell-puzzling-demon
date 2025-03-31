using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class Crate : MonoBehaviour, IMovable, ITemperature {

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
        bool registered = false;

        #endregion

        #region Unity

        void Start() {
            gm = FindFirstObjectByType<GridManager>();
            Register();
            UpdateSprites();
            UpdateColor();
        }

        #endregion

        // used for level manager
        public void InitCrate(CrateData data) {
            temperature = data.Temperature;
            position = data.Position;
            offsets = data.Shape;
        }

        public void InitBorders() {
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

        struct Bounds {
            public Vector2Int size; // square size (pixel)
            public Vector2Int min; // bottom left pos (grid)
            public Vector2Int max; // top right pos
            public Vector2 pivot;

            public Bounds(Vector2Int size_, Vector2Int min_, Vector2Int max_, Vector2 pivot_) {
                size = size_;
                min = min_;
                max = max_;
                pivot = pivot_;
            }
        }

        Bounds CalculateBounds() {
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            foreach (var offset in offsets) {
                minX = Mathf.Min(minX, offset.x);
                maxX = Mathf.Max(maxX, offset.x);
                minY = Mathf.Min(minY, offset.y);
                maxY = Mathf.Max(maxY, offset.y);
            }
            var size = new Vector2Int {
                x = (maxX - minX + 1) * Defs.CellSize,
                y = (maxY - minY + 1) * Defs.CellSize
            };
            var pivot = new Vector2(-1.0f * minX / (maxX - minX + 1), -1.0f * minY / (maxY - minY + 1));
            return new Bounds(size, new(minX, minY), new(maxX, maxY), pivot);
        }

        Texture2D GenerateTexture(out Bounds res) {
            var bounds = CalculateBounds();
            int witdh = bounds.size.x;
            int height = bounds.size.y;

            var texture = new Texture2D(witdh, height);

            // init texture color --> transparent
            var pixels = new Color32[witdh * height];
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = Color.clear;
            }
            texture.SetPixels32(pixels);
            res = bounds;
            return texture;
        }

        public void UpdateSprites() {
            InitBorders();
            GenerateBackgroundSprite();
            GenerateBorderSprite();
        }

        public void GenerateBackgroundSprite() {
            var texture = GenerateTexture(out var bounds);
            foreach (var offset in offsets) {
                var startX = (offset.x - bounds.min.x) * Defs.CellSize;
                var startY = (offset.y - bounds.min.y) * Defs.CellSize;
                for (int y = startY; y < startY + Defs.CellSize; y++) {
                    for (int x = startX; x < startX + Defs.CellSize; x++) {
                        texture.SetPixel(x, y, Color.white);
                    }
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), bounds.pivot);
            sprite.name = "background";
            backgroundSR.sprite = sprite;
        }

        public void GenerateBorderSprite() {
            var texture = GenerateTexture(out var bounds);
            var cellSize = Defs.CellSize;
            var borderSize = Defs.BorderSize;
            foreach (var border in borders) {
                if (border.dir == Vector2Int.up || border.dir == Vector2Int.down) {
                    Utils.DrawHorizontalBorder(texture, border, -bounds.min);
                } else if (border.dir == Vector2Int.left || border.dir == Vector2Int.right) {
                    Utils.DrawVerticalBorder(texture, border, -bounds.min);
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
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), bounds.pivot);
            sprite.name = "borders";
            BorderSR.sprite = sprite;
        }

        public void Register() {
            gm.RegisterCrate(this);
            registered = true;
        }

        public void UnRegister() {
            gm.UnRegisterCrate(this);
            registered = false;
        }

        public void Destory() {
            if (registered) {
                UnRegister();
            }
            Destroy(gameObject);
        }

        /*─────────────────────────────────────┐
        │          IMovable Interface          │
        └──────────────────────────────────────*/

        public List<Vector2Int> GetUnitsPosition() {
            var res = new List<Vector2Int>();
            foreach (var offset in offsets) {
                res.Add(position + offset);
            }
            return res;
        }

        public bool BePushed(Vector2Int dir) {
            Debug.Log("crate_" + position + " try to pushed to " + (position + dir));
            if (gm.CanMove(dir, this)) {
                Move(dir);
                return true;
            }
            Debug.Log("crate can't be pushed");
            return false;
        }

        private void Move(Vector2Int dir) {
            /* TODO: Move System <2025-04-01 03:13, @qiekn> */
            gm.UnRegisterCrate(this);
            position += dir;
            transform.position = gm.CellToWorld(position); // move
            gm.RegisterCrate(this);

            HeatSystem.Instance.Register(gm.GetAdjacent<ITemperature>(this));
            MergeSystem.Instance.KissKiss(gm.GetAdjacent<Crate>(this, AdjacentFilter.StickyCrate), this);
        }

        /*─────────────────────────────────────┐
        │        ITemperature Interface        │
        └──────────────────────────────────────*/

        public int GetTemperature() {
            return (int)temperature * offsets.Count;
        }

        public void SetTemperature(Temperature t) {
            temperature = t;
            UpdateColor();
        }

        public void UpdateColor() {
            Utils.UpdateSpritesColor(backgroundSR, temperature);
            Utils.UpdateBordersColor(BorderSR, temperature);
        }
    }
}
