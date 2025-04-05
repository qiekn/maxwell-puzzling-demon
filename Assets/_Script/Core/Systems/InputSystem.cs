using UnityEngine;

namespace qiekn.core {
    public class InputSystem : MonoBehaviour {
        LevelManager lm;

        void Start() {
            lm = FindFirstObjectByType<LevelManager>().GetComponent<LevelManager>();
            Application.targetFrameRate = 60;
        }

        /* TODO: undo system <2025-04-06 01:57, @qiekn> */
        void Update() {
            // reset current level
            if (Input.GetKeyDown(KeyCode.R)) {
                lm.PlayerEnter();
            }
            // switch level in this world
            if (Input.GetKeyDown(KeyCode.J)) {
                lm.NextLevelInThisWorld();
            }
            if (Input.GetKeyDown(KeyCode.K)) {
                lm.PrevLevelInThisWorld();
            }
            // switch world
            if (Input.GetKeyDown(KeyCode.H)) {
                lm.PrevWorld();
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                lm.NextWorld();
            }
        }
    }
}
