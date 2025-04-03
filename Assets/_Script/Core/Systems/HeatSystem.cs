using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class HeatSystem {
        readonly HashSet<ITemperature> buffer = new();
        readonly List<Crate> crates = new();

        private static HeatSystem instance;

        public static HeatSystem Instance {
            get {
                instance ??= new HeatSystem();
                return instance;
            }
        }

        /* TODO: player as a special crate <2025-04-03 22:04, @qiekn> */
        public void Register(Player player) {
            if (player.GM == null) {
                Debug.Log("gm is null");
            }
            player.SetTemperature(Temperature.Neutral);
            buffer.Add(player);
            buffer.UnionWith(player.GM.GetAdjacent<ITemperature>(player.Position, AdjacentFilter.Temperature));
        }

        public void Register(Crate crate) {
            crates.Add(crate);
        }

        /* TODO: fix sticky merge <2025-04-03 23:03, @qiekn> */
        public void Process() {
            // gather
            foreach (var crate in crates) {
                buffer.UnionWith(crate.GM.GetAdjacent<ITemperature>(crate, AdjacentFilter.Temperature));
            }
            // calculate heat
            int res = 0;
            foreach (var crate in buffer) {
                res += crate.GetTemperature();
            }
            // apply heat
            var t = Utils.GetTemperature(res);
            foreach (var crate in buffer) {
                crate.SetTemperature(t);
                crate.UpdateColor();
            }
            // reset buffers
            buffer.Clear();
            crates.Clear();
        }
    }
}
