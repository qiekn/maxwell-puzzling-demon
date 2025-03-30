using UnityEngine;

namespace qiekn.core {
    public class LevelManager : MonoBehaviour {

        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject cratePrefab;

        [SerializeField] LevelData data;

        GridManager gm;


        void Start() {
            gm = FindFirstObjectByType<GridManager>();
            if (gm == null) {
                Debug.LogError("LevelManager: GridManager component is null");
            }
            InitLevel();
        }

        void InitLevel() {
            // crates
            foreach (var crateData in data.Crates) {
                var obj = Instantiate(cratePrefab, gm.CellToWorld(crateData.Position), Quaternion.identity);
                obj.GetComponent<Crate>().Init(crateData);
                obj.name = "crate_" + crateData.Position;
            }
            // player
            // player sprite's pivot is center, so we need some offset
            var playerPosition = gm.CellToWorld(data.SpawnPoints) + new Vector3(0.25f, 0.25f, 0f);
            var player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
            player.name = "player";
        }

        void ResetLevel() {
            InitLevel();
        }

    }
}
