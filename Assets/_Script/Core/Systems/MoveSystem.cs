using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class MoveSystem {
        private static MoveSystem instance;
        public static MoveSystem Instance => instance ??= new MoveSystem();

        private GridManager gm;
        public GridManager GM { set => gm = value; }

        public bool TryMove(Vector2Int dir, BaseCrate crate) {
            return TryMove(dir, GetStickyGroup(crate));
            // feat: sticky
            bool TryMove(Vector2Int dir, HashSet<BaseCrate> stickyGroup) {
                if (stickyGroup.Count > 0) {
                    var hits = new HashSet<BaseCrate>();
                    // loop all units of sticky group & gather objs that need to be pushed
                    foreach (var stickyObj in stickyGroup) {
                        foreach (var pos in stickyObj.GetUnitsPosition()) {
                            var dest = pos + dir;
                            // destination out of range (out of ground/map)
                            if (gm.IsWall(dest)) {
                                return false;
                            }
                            // hit other, gather other
                            if (gm.Get<BaseCrate>(dest, out var other) && !stickyGroup.Contains(other)) {
                                hits.Add(other);
                            }
                        }
                    }
                    // check chain move
                    if (!TryMove(dir, hits)) {
                        return false;
                    }
                }
                // OK, let's go!
                Move(dir, stickyGroup);
                return true;
            }
        }

        // dfs
        private HashSet<BaseCrate> GetStickyGroup(BaseCrate crate) {
            var res = new HashSet<BaseCrate>();
            var q = new Queue<BaseCrate>();
            q.Enqueue(crate);
            while (q.Count > 0) {
                var cur = q.Dequeue();
                if (res.Contains(cur)) continue; // deduplicate
                res.Add(cur);
                // gather
                var adjacentSticky = gm.GetAdjacent<BaseCrate>(cur, AdjacentFilter.StickyCrate);
                foreach (var adj in adjacentSticky) {
                    if (!res.Contains(adj)) {
                        q.Enqueue(adj);
                    }
                }
            }
            return res;
        }

        // move the entire sticky group
        private void Move(Vector2Int dir, HashSet<BaseCrate> stickyGroup) {
            foreach (var x in stickyGroup) x.UnRegister();
            foreach (var x in stickyGroup) x.Move(dir);
            foreach (var x in stickyGroup) x.Register();
        }
    }
}
