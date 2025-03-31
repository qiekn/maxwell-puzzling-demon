using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class MergeSystem {
        private static MergeSystem instance;

        public static MergeSystem Instance {
            get {
                instance ??= new MergeSystem();
                return instance;
            }
        }

        public void KissKiss(HashSet<Crate> set, Crate me) {
            foreach (var lover in set) {
                Debug.Log("crate_" + me.position + " kisskiss " + "crate_" + lover.position);
                foreach (var offset in lover.offsets) {
                    me.offsets.Add(lover.position + offset - me.position);
                }
                lover.UnRegister();
                lover.Destory();
                me.UnRegister();
                me.Register();
                me.UpdateSprites();
            }
        }
    }
}
