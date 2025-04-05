using UnityEngine;
using UnityEngine.Tilemaps;

namespace qiekn.core {
  [CreateAssetMenu(fileName = "LevelRuleTile", menuName = "Level Rule Tile", order = 1)]
  public class CKRuleTile : RuleTile {
    public TileType Type;
  }
}
