using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class HeatSystem {
        readonly HashSet<ITemperature> buffer;

        private static HeatSystem instance;

        public static HeatSystem Instance {
            get {
                instance ??= new HeatSystem();
                return instance;
            }
        }

        HeatSystem() {
            buffer = new HashSet<ITemperature>();
        }

        public void Register(ITemperature obj) {
            buffer.Add(obj);
        }

        public void Register(HashSet<ITemperature> set) {
            buffer.UnionWith(set);
        }

        public void Balance() {
            // calculate heat
            int res = 0;
            foreach (var crate in buffer) {
                res += crate.GetTemperature();
            }
            // apply heat
            var t = Utils.GetTemperature(res);
            Debug.Log("HeatSystem: value = " + res + ", result=" + t);
            foreach (var crate in buffer) {
                crate.SetTemperature(t);
                crate.UpdateColor();
            }
            buffer.Clear();
        }
    }
}
