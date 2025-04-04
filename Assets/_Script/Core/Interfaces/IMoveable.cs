using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public interface IMovable {
        void Move(Vector2Int dir);
        List<Vector2Int> GetUnitsPosition();
    }
}
