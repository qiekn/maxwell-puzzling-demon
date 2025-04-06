using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class HeatSystem {
        private static HeatSystem instance;

        HashSet<BaseCrate> requesters;
        HashSet<BaseCrate> buffer; // used for one epoch calculation

        public static HeatSystem Instance => instance ??= new HeatSystem() {
            requesters = new(),
            buffer = new(),
        };

        public void Register(BaseCrate crate) {
            requesters.Add(crate);
        }

        public void Process() {
            Debug.Log("HeatSystem: Starting process");
            foreach (var crate in requesters) {
                Debug.Log("HeatSystem: Processing requester " + crate);
                buffer.Clear();
                buffer.Add(crate);
                GatherAdjacentDFS(crate);
                ApplyTemperature(CalculateHeatSum());
                Debug.Log("buffer are:");
                foreach (var x in buffer) {
                    Debug.Log(x.position);
                }
            }
            buffer.Clear();
            requesters.Clear();

            void GatherAdjacentDFS(BaseCrate crate) {
                var adjacents = crate.GM.GetAdjacent<BaseCrate>(crate, AdjacentFilter.Temperature);
                foreach (var obj in adjacents) {
                    if (!buffer.Contains(obj)) {
                        buffer.Add(obj);
                        GatherAdjacentDFS(obj);
                    }
                }
            }

            int CalculateHeatSum() {
                int sum = 0;
                foreach (var obj in buffer) {
                    sum += obj.GetTemperature();
                }
                return sum;
            }

            void ApplyTemperature(int heatSum) {
                var t = Utils.GetTemperature(heatSum);
                foreach (var obj in buffer) {
                    obj.SetTemperature(t);
                    obj.UpdateColor();
                }
            }
        }
    }
}
