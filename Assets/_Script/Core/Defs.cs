using UnityEngine;

namespace qiekn.core {

    public enum Temperature {
        Cold = -10,
        Neutral = 0,
        Magic = 1, // green temperature
        Hot = 10,
    }

    public static class Defs {

        public static float Unit = 0.50f;
        public static int CellSize = 50; // pixel count

        /*─────────────────────────────────────┐
        │                Colors                │
        └──────────────────────────────────────*/

        // basic
        public static Color GRAY = new Color32(229, 229, 229, 255);
        public static Color DARKGRAY = new Color32(90, 90, 90, 255);

        // crate
        public static Color RED = new Color32(241, 185, 168, 255);
        public static Color DARKRED = new Color32(220, 113, 81, 255);

        public static Color BLUE = new Color32(177, 209, 252, 255);
        public static Color DARKBLUE = new Color32(100, 155, 236, 255);

        public static Color GREEN = new Color32(188, 238, 160, 255);
        public static Color DARKGREEN = new Color32(188, 238, 160, 255);
    }
}
