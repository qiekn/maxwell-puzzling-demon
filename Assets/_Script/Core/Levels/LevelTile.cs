using UnityEngine;
using UnityEngine.Tilemaps;

namespace qiekn.core {
  [CreateAssetMenu(fileName = "LevelTile", menuName = "Level Tile", order = 1)]
  public class CKTile : Tile {
    public TileType Type;
  }

  public enum TileType {
    Ground = 0,
    Spawn = 1,
    Target = 2,
    HotCrate = 10,
    ColdCrate = 11,
    NeutralCrate = 12,
  }
}
