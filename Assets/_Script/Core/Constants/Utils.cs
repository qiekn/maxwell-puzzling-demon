using UnityEngine;

namespace qiekn.core {
    public static class Utils {

        #region SpriteTexture

        public static void DrawHorizontalBorder(Texture2D texture, Border border, Vector2Int offset) {
            var color = Color.white;
            switch (border.type) {
                case BorderType.shield:
                    color = Defs.DARKGRAY;
                    break;
                case BorderType.none:
                    return;
                default:
                    break;
            }

            var cellSize = Defs.CellSize;
            var borderSize = Defs.BorderSize;
            var pos = border.pos + offset;
            float yOffset = border.dir == Vector2Int.up ? 1f : -1f;
            float yCenter = (pos.y + 0.5f) * cellSize + yOffset * (cellSize - borderSize) * 0.5f;

            int xMin = pos.x * cellSize;
            int xMax = (pos.x + 1) * cellSize;
            int yMin = Mathf.RoundToInt(yCenter - borderSize / 2f);
            int yMax = Mathf.RoundToInt(yCenter + borderSize / 2f);

            // limiter
            xMin = Mathf.Max(0, xMin);
            xMax = Mathf.Min(texture.width, xMax);
            yMin = Mathf.Max(0, yMin);
            yMax = Mathf.Min(texture.height, yMax);

            for (int y = yMin; y < yMax; y++) {
                for (int x = xMin; x < xMax; x++) {
                    if (border.type == BorderType.sticky &&
                            x > Defs.BorderSize &&
                            x < texture.width - Defs.BorderSize &&
                            (x - Defs.BorderSize) % 10 < Defs.DashedLineGap - 1) { // used to draw dashed line
                        continue;
                    }
                    texture.SetPixel(x, y, color);
                }
            }
        }

        public static void DrawVerticalBorder(Texture2D texture, Border border, Vector2Int offset) {
            var color = Color.white;
            switch (border.type) {
                case BorderType.shield:
                    color = Defs.DARKGRAY;
                    break;
                case BorderType.none:
                    return;
                default:
                    break;
            }
            var cellSize = Defs.CellSize;
            var borderSize = Defs.BorderSize;
            var pos = border.pos + offset;
            float xOffset = border.dir == Vector2Int.right ? 1f : -1f;
            float xCenter = (pos.x + 0.5f) * cellSize + xOffset * (cellSize - borderSize) * 0.5f;

            int yMin = pos.y * cellSize;
            int yMax = (pos.y + 1) * cellSize;
            int xMin = Mathf.RoundToInt(xCenter - borderSize / 2f);
            int xMax = Mathf.RoundToInt(xCenter + borderSize / 2f);

            // limiter
            yMin = Mathf.Max(0, yMin);
            yMax = Mathf.Min(texture.height, yMax);
            xMin = Mathf.Max(0, xMin);
            xMax = Mathf.Min(texture.width, xMax);

            for (int y = yMin; y < yMax; y++) {
                for (int x = xMin; x < xMax; x++) {
                    if (border.type == BorderType.sticky &&
                            y > Defs.BorderSize &&
                            y < texture.height - Defs.BorderSize &&
                            (y - Defs.BorderSize) % 10 < Defs.DashedLineGap - 1) { // used to draw dashed line
                        continue;
                    }
                    texture.SetPixel(x, y, color);
                }
            }
        }

        #endregion

        #region SpriteColor

        public static void UpdateSpritesColor(SpriteRenderer spriteRenderer, Temperature t) {
            switch (t) {
                case Temperature.Hot:
                    spriteRenderer.color = Defs.RED;
                    break;
                case Temperature.Cold:
                    spriteRenderer.color = Defs.BLUE;
                    break;
                case Temperature.Neutral:
                    spriteRenderer.color = Defs.GRAY;
                    break;
                case Temperature.Magic:
                    spriteRenderer.color = Defs.GREEN;
                    break;
            }
        }

        public static void UpdateSpritesColor(SpriteRenderer[] spriteRenderers, Temperature t) {
            foreach (var sprite in spriteRenderers) {
                UpdateSpritesColor(sprite, t);
            }
        }

        public static void UpdateBordersColor(SpriteRenderer border, Temperature t) {
            switch (t) {
                case Temperature.Hot:
                    border.color = Defs.DARKRED;
                    break;
                case Temperature.Cold:
                    border.color = Defs.DARKBLUE;
                    break;
                case Temperature.Neutral:
                    border.color = Defs.MIDGRAY;
                    break;
                case Temperature.Magic:
                    border.color = Defs.DARKGREEN;
                    break;
            }

        }

        public static void UpdateBordersColor(SpriteRenderer[] borders, Temperature t) {
            foreach (var border in borders) {
                UpdateBordersColor(border, t);
            }
        }

        public static void UpdateSpritesColorWithBorder(SpriteRenderer[] backgrounds, SpriteRenderer[] borders, Temperature t) {
            UpdateSpritesColor(backgrounds, t);
            UpdateBordersColor(borders, t);
        }

        #endregion

        #region Miscellaneous

        public static Temperature GetTemperature(int t) => t switch {
            > 9999 => Temperature.Magic,
            > 0 => Temperature.Hot,
            < 0 => Temperature.Cold,
            _ => Temperature.Neutral
        };

        public static bool IsGround(Vector3 pos, LayerMask layerMask) {
            var hit = Physics2D.OverlapPoint(pos, layerMask);
            if (hit != null) {
                return true;
            }
            return false;
        }

        #endregion

    }
}
