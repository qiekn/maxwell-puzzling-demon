using UnityEngine;
using System.Collections.Generic;
using System;

namespace qiekn.core {

  [CreateAssetMenu(fileName = "New Level", menuName = "Level Data", order = 1)]
  public class LevelData : ScriptableObject {
    public World World;
    public int LevelIndex;
    public Vector2Int MapSize;

    // tiles
    public List<SavedRuleTile> GroundTiles = new();
    public List<SavedTile> SpawnTiles = new();
    public List<SavedTile> TargetTiles = new();

    // datas
    public List<Vector2Int> Grounds = new();
    public List<CrateData> CrateDatas = new();
    public List<Vector2Int> Spawns = new();
    public List<Vector2Int> Targets = new();
  }

  [Serializable]
  public class SavedTile {
    public Vector3Int Position;
    public CKTile Tile;
  }

  [Serializable]
  public class SavedRuleTile {
    public Vector3Int Position;
    public CKRuleTile Tile;
  }

  [Serializable]
  public class CrateData {
    public Temperature Temperature;
    public Vector2Int Position;
    public List<Vector2Int> Shape;
    public List<Border> BordersOverride;
  }

  [Serializable] public enum World { w0, w1, w2, w3, w4, w5, w6, w7, w8, w9 }
}
