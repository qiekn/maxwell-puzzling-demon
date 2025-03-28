using UnityEngine;

namespace qiekn.core {
    public interface IMoveable {
        bool CanMove(Vector2Int direction);
        void Move(Vector2Int direction);
    }
}
