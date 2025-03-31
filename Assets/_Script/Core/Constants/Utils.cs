using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace qiekn.core {
    public static class Utils {

        #region SpriteTexture

        public static void DrawHorizontalBorder(Texture2D texture, Border border, int cellSize, int borderSize) {
            float yOffset = border.dir == Vector2Int.up ? 1f : -1f;
            float yCenter = (border.pos.y + 0.5f) * cellSize + yOffset * (cellSize - borderSize) * 0.5f;

            int xMin = border.pos.x * cellSize;
            int xMax = (border.pos.x + 1) * cellSize;
            int yMin = Mathf.RoundToInt(yCenter - borderSize / 2f);
            int yMax = Mathf.RoundToInt(yCenter + borderSize / 2f);

            // limiter
            xMin = Mathf.Max(0, xMin);
            xMax = Mathf.Min(texture.width, xMax);
            yMin = Mathf.Max(0, yMin);
            yMax = Mathf.Min(texture.height, yMax);

            var color = Color.white;
            if (border.type == BorderType.shield) {
                color = Color.black;
            }

            for (int y = yMin; y < yMax; y++) {
                for (int x = xMin; x < xMax; x++) {
                    texture.SetPixel(x, y, color);
                }
            }
        }

        public static void DrawVerticalBorder(Texture2D texture, Border border, int cellSize, int borderSize) {
            float xOffset = border.dir == Vector2Int.right ? 1f : -1f;
            float xCenter = (border.pos.x + 0.5f) * cellSize + xOffset * (cellSize - borderSize) * 0.5f;

            int yMin = border.pos.y * cellSize;
            int yMax = (border.pos.y + 1) * cellSize;
            int xMin = Mathf.RoundToInt(xCenter - borderSize / 2f);
            int xMax = Mathf.RoundToInt(xCenter + borderSize / 2f);

            // limiter
            yMin = Mathf.Max(0, yMin);
            yMax = Mathf.Min(texture.height, yMax);
            xMin = Mathf.Max(0, xMin);
            xMax = Mathf.Min(texture.width, xMax);

            var color = Color.white;
            if (border.type == BorderType.shield) {
                color = Color.black;
            }

            for (int y = yMin; y < yMax; y++) {
                for (int x = xMin; x < xMax; x++) {
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
