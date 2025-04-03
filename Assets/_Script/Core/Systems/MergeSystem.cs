using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class MergeSystem {
        private static MergeSystem instance;
        readonly Queue<Crate> queue = new();

        public static MergeSystem Instance {
            get {
                instance ??= new MergeSystem();
                return instance;
            }
        }

        public void Register(Crate crate) {
            queue.Enqueue(crate);
        }

        public void Process() {
            while (queue.Count > 0) {
                var crate = queue.Dequeue();
                KissKiss(crate, crate.GM.GetAdjacent<Crate>(crate, AdjacentFilter.StickyCrate));
            }
        }

        // merge function
        void KissKiss(Crate me, HashSet<Crate> set) {
            foreach (var lover in set) {
                // private dating
                lover.UnRegister();
                me.UnRegister();
                // merge shape & units
                foreach (var offset in lover.offsets) {
                    var me_offset = lover.position + offset - me.position;
                    me.offsets.Add(me_offset);

                    var unit = lover.units[offset];
                    unit.position = me_offset;
                    foreach (var pair in unit.borders) {
                        pair.Value.pos = me_offset;
                    }
                    me.units.Add(me_offset, unit);
                }
                // merge borders
                foreach (var border in lover.borders) {
                    me.borders.Add(border);
                }
                // submit
                lover.Destory();
                me.Register();
                me.DisableInnerPairedStickyBorders();
                me.UpdateSprites();
            }
        }
    }
}
