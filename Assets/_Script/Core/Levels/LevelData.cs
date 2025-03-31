using UnityEngine;
using System.Collections.Generic;
using System;

namespace qiekn.core {

  [CreateAssetMenu(fileName = "NewLevel", menuName = "Level Data", order = 1)]
  public class LevelData : ScriptableObject {
    public string LevelName;
    public Vector2Int LevelSize;

    // public List<Vector2Int> Grounds;

    public Vector2Int SpawnPoints;
    public List<CrateData> Crates;
    public List<Vector2Int> Targets;

    /*
    private void OnEnable() {
      if (Crates == null) Crates = new List<CrateData>();
      // if (Grounds == null) Grounds = new List<Vector2Int>();
      if (Targets == null) Targets = new List<Vector2Int>();
    }
    */
  }

  [Serializable]
  public class CrateData {
    public Temperature Temperature;
    public Vector2Int Position;
    public List<Vector2Int> Shape;

    public CrateData() {
      Shape = new List<Vector2Int>{
        new(0, 0)
      };
    }
  }
}
