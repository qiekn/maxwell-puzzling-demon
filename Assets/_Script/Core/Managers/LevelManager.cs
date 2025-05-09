using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace qiekn.core {
    public class LevelManager : MonoBehaviour {

        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject cratePrefab;

        [SerializeField] GridManager gm;
        [SerializeField] Tilemap groundMap, crateMap, spawnMap, targetMap;
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] World world;
        [SerializeField] int levelIndex;
        [SerializeField] List<CrateData> crateDatas;

        LevelData level;
        new Camera camera;
        bool prevLevelExit;
        int prevLevelIndex;
        World prevWorld;
        readonly List<GameObject> players = new();
        readonly List<GameObject> crates = new();

        void Start() {
            camera = Camera.main;
            prevLevelIndex = levelIndex;
            prevWorld = world;
            PlayerEnter();
        }

        public void SaveLevel() {
            var level = ScriptableObject.CreateInstance<LevelData>();

            level.name = $"{world}-{levelIndex}";
            var bounds = groundMap.cellBounds;
            level.MapSize = new(bounds.size.x, bounds.size.y);
            level.LevelIndex = levelIndex;
            level.GroundTiles = GetGroundTiles(groundMap).ToList();
            level.CrateDatas = crateDatas; // save changes from lm inspector
            GetSpawnsAndTargets();

            ScriptableObjectUtility.SaveLevelFile(level);

            IEnumerable<SavedRuleTile> GetGroundTiles(Tilemap map) {
                foreach (var pos in map.cellBounds.allPositionsWithin) {
                    if (map.HasTile(pos)) {
                        var tile = map.GetTile<CKRuleTile>(pos);
                        level.Grounds.Add(new(pos.x, pos.y));
                        yield return new SavedRuleTile() {
                            Position = pos,
                            Tile = tile,
                        };
                    }
                }
            }

            void GetSpawnsAndTargets() {
                var maps = new List<Tilemap> { spawnMap, targetMap };
                foreach (var map in maps) {
                    foreach (var pos in map.cellBounds.allPositionsWithin) {
                        if (map.HasTile(pos)) {
                            var tile = map.GetTile<CKTile>(pos);
                            if (tile.Type == TileType.Spawn) {
                                Debug.Log("LevelManager: add spawn " + pos);
                                level.SpawnTiles.Add(new SavedTile() { Position = pos, Tile = tile });
                                level.Spawns.Add(new(pos.x, pos.y));
                            } else if (tile.Type == TileType.Target) {
                                Debug.Log("LevelManager: add target " + pos);
                                level.TargetTiles.Add(new SavedTile() { Position = pos, Tile = tile });
                                level.Targets.Add(new(pos.x, pos.y));
                            }
                        }
                    }
                }
            }
        }

        public void ClearLevel() {
            var maps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
            foreach (var tilemap in maps) {
                tilemap.ClearAllTiles();
            }
            foreach (var player in players) {
                if (player != null) {
                    Destroy(player);
                } else {
                    Debug.Log("LevelManager: player is null");
                }
            }
            foreach (var crate in crates) {
                if (crate != null) {
                    crate.GetComponent<BaseCrate>().Destroy();
                }
            }
            crateDatas = new();
            players.Clear();
            crates.Clear();
        }

        public int LoadLevel() {
            level = Resources.Load<LevelData>($"Levels/{world}-{levelIndex}");
            if (level == null) {
                Debug.LogError($"Level {world}_{levelIndex} does not exist.");
                levelIndex = prevLevelIndex;
                world = prevWorld;
                if (prevLevelExit) {
                    return 1;
                }
                return -1;
            }
            prevLevelExit = true;
            ClearLevel();
            crateDatas = level.CrateDatas; // references

            // grounds
            foreach (var tile in level.GroundTiles) {
                switch (tile.Tile.Type) {
                    case TileType.Ground:
                    case TileType.Spawn:
                    case TileType.Target:
                        SetRuleTile(groundMap, tile);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // crates
            foreach (var crateData in level.CrateDatas) {
                var crate = Instantiate(cratePrefab, gm.CellToWorld(crateData.Position), Quaternion.identity);
                crate.GetComponent<BaseCrate>().InitCrate(crateData);
                crates.Add(crate);
            }

            // spawns
            if (level.Spawns.Count > 0) {
                foreach (var tile in level.SpawnTiles) {
                    SetTile(spawnMap, tile);
                }
            } else {
                Debug.LogError("LevelManager: no spawn point");
            }

            // targets
            foreach (var tile in level.TargetTiles) {
                SetTile(targetMap, tile);
            }

            return 0;

            // local func
            void SetRuleTile(Tilemap map, SavedRuleTile tile) => map.SetTile(tile.Position, tile.Tile);
            void SetTile(Tilemap map, SavedTile tile) => map.SetTile(tile.Position, tile.Tile);
        }

        public void PlayerEnter() {
            PlayerExit();
            int returnCode = LoadLevel();
            if (returnCode == -1) {
                return;
            } else if (returnCode == 1) {
                // new level doesn't exit
                // jump back to prev level
                LoadLevel();
            } else if (returnCode == 0) {
                levelText.text = $"World {world.ToString().Replace("w", "")} / Level {levelIndex}";
            }

            gm.RegisterGrounds(level.Grounds);

            // camera
            var center = groundMap.cellBounds.center;
            if (camera == null) {
                camera = Camera.main;
            }
            camera.transform.position = new(center.x * 0.5f, center.y * 0.5f, -10f);
            // camera.orthographicSize = level.MapSize.y * 0.5f + 1;

            // instantiate maxwell's demon
            foreach (var pos in level.Spawns) {
                // player sprite's pivot is center, so we need some offset
                var playerPosition = gm.CellToWorld(pos) + Defs.playerOffset;
                var player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
                player.GetComponent<Player>().Init(pos);
                players.Add(player);
            }
        }

        public void PlayerExit() {
            ClearLevel();
            gm.Clear();
        }

        #region Switch Level
        // switch level

        public void SwitchToLevelInThisWorld(int index) {
            prevLevelIndex = index;
            Debug.Log("Switch to level: " + index);
            PlayerEnter();
        }

        public void NextLevelInThisWorld() {
            SwitchToLevelInThisWorld(levelIndex++);
        }

        public void PrevLevelInThisWorld() {
            SwitchToLevelInThisWorld(levelIndex--);
        }

        // switch world

        public void SwitchToWorld(World world) {
            prevWorld = this.world;
            this.world = world;
            levelIndex = 0;
            PlayerEnter();
        }

        public void PrevWorld() {
            SwitchToWorld(world - 1);
        }

        public void NextWorld() {
            SwitchToWorld(world + 1);
        }

        #endregion
    } // class

#if UNITY_EDITOR

    public static class ScriptableObjectUtility {
        public static void SaveLevelFile(LevelData level) {
            AssetDatabase.CreateAsset(level, $"Assets/Resources/Levels/{level.name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

#endif
} // namespace
