using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace qiekn.core {
    public class Crate : BaseCrate {

        #region Field
        // caches
        [SerializeField] SpriteRenderer backgroundSR; // background sprite
        [SerializeField] SpriteRenderer BorderSR; // border sprite

        #endregion

        #region Unity

        protected override void Start() {
            base.Start();
            UpdateSprites();
            UpdateColor();
        }

        #endregion

        #region Render

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

        public void UpdateSprites() {
            GenerateBackgroundSprite();
            GenerateBorderSprite();

            Bounds CalculateBounds() {
                int minX = 0, maxX = 0, minY = 0, maxY = 0;
                foreach (var offset in shape) {
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

            void GenerateBackgroundSprite() {
                var texture = GenerateTexture(out var bounds);
                foreach (var offset in shape) {
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

            void GenerateBorderSprite() {
                var texture = GenerateTexture(out var bounds);
                var borderSize = Defs.BorderSize;
                foreach (var border in borders) {
                    // Debug.Log("Generate Border Sprite: " + border.pos);
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
        }

        #endregion

        public override void Move(Vector2Int dir) {
            base.Move(dir);
            MergeSystem.Instance.Register(this);
        }

        public override void UpdateColor() {
            Utils.UpdateSpritesColor(backgroundSR, temperature);
            Utils.UpdateBordersColor(BorderSR, temperature);
        }
    }
}
