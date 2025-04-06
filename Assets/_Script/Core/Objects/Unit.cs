using System;
using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {

    // 1x1 crate with 4 borders
    [Serializable]
    public class Unit {
        public Vector2Int position;
        public Dictionary<Vector2Int, Border> borders;

        public Unit(Vector2Int position, BorderType bt = BorderType.conductive) {
            this.position = position;
            borders = new Dictionary<Vector2Int, Border>();
            foreach (var dir in Defs.directions) {
                borders[dir] = new Border(position, dir, bt);
            }
        }
    }

}
