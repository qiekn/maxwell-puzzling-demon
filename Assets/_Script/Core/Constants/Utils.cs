using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace qiekn.core {
    public static class Utils {

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

            for (int y = yMin; y < yMax; y++) {
                for (int x = xMin; x < xMax; x++) {
                    texture.SetPixel(x, y, Color.white);
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

            for (int y = yMin; y < yMax; y++) {
                for (int x = xMin; x < xMax; x++) {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }

        public static Vector3 CalculateDest(Vector3 pos, Vector2Int dir) {
            var step = Defs.Unit;
            var dest = new Vector3(pos.x + dir.x * step, pos.y + dir.y * step, pos.z);
            return dest;
        }

        public static List<Vector3> CalculateDests(List<Vector3> positions, Vector2Int dir) {
            var res = new List<Vector3>();
            foreach (var pos in positions) {
                res.Append(CalculateDest(pos, dir));
            }
            return res;
        }

        public static bool IsGround(Vector3 pos, LayerMask layerMask) {
            var hit = Physics2D.OverlapPoint(pos, layerMask);
            if (hit != null) {
                return true;
            }
            return false;
        }

        public static bool IsCrate(Vector3 pos, LayerMask layerMask) {
            var hit = Physics2D.OverlapPoint(pos, layerMask);
            if (hit != null) {
                return true;
            }
            return false;
        }
        public static GameObject GetCrate(Vector3 pos, LayerMask layerMask) {
            var hit = Physics2D.OverlapPoint(pos, layerMask);
            if (hit != null) {
                return hit.gameObject;
            }
            return null;
        }
    }
}
