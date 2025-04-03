using System.Buffers;
using System.Collections.Generic;
using NUnit.Framework;
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

        public void Register(Player player) {
            if (player.GM == null) {
                Debug.Log("gm is null");
            }
            buffer.UnionWith(player.GM.GetAdjacent<ITemperature>(player.Position));
        }

        public void Register(Crate crate) {
            crates.Add(crate);
        }

        public void Process() {
            // gather
            foreach (var crate in crates) {
                buffer.UnionWith(crate.GM.GetAdjacent<ITemperature>(crate));
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
