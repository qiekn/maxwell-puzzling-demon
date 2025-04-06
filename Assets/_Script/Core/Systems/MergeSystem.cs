using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class MergeSystem {
        private static MergeSystem instance;
        private readonly Queue<Crate> queue = new();

        public static MergeSystem Instance => instance ??= new MergeSystem();

        public void Register(Crate crate) {
            queue.Enqueue(crate);
        }

        public void Process() {
            while (queue.Count > 0) {
                var crate = queue.Dequeue();
                KissKiss(crate, crate.GM.GetAdjacent<Crate>(crate, AdjacentFilter.Merge));
            }

            // merge function
            void KissKiss(Crate me, HashSet<Crate> set) {
                foreach (var lover in set) {
                    // private dating
                    lover.UnRegister();
                    me.UnRegister();
                    // merge offset & units
                    foreach (var old_offset in lover.shape) {
                        var new_offset = lover.position + old_offset - me.position;
                        me.shape.Add(new_offset);
                        var unit = lover.units[old_offset];
                        unit.position = new_offset;
                        foreach (var pair in unit.borders) {
                            pair.Value.pos = new_offset;
                        }
                        me.units.Add(new_offset, unit);
                    }
                    // merge the list of borders' reference
                    foreach (var border in lover.borders) {
                        me.borders.Add(border);
                    }
                    // submit
                    lover.Destroy();
                    me.Register();
                    me.DisableInnerPairedStickyBorders();
                    me.UpdateSprites();
                }
            }
        }
    } // class
}
