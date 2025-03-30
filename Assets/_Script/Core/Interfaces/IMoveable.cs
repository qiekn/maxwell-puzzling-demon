using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public interface IMovable {
        bool BePushed(Vector2Int dir);
        List<Vector2Int> GetUnitsPosition();
        /*
        bool CanMove(Vector2Int direction);
        void Move(Vector2Int direction);
        */
    }
}
