using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public interface IMovable {
        bool BePushed(Vector2Int dir);
        List<Vector2Int> GetUnitsPosition();
    }
}
