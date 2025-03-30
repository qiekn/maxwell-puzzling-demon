using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace qiekn.core {
    public static class Utils {

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
