using System.Collections.Generic;
using UnityEngine;

namespace qiekn.core {
    public class MoveSystem {
        private static MoveSystem instance;
        private GridManager gm;

        public GridManager GM { set => gm = value; }
        public static MoveSystem Instance => instance ??= new MoveSystem();

        public bool TryMove(Vector2Int dir, Crate crate) {
            return TryMove(dir, GetStickyGroup(crate));

            bool TryMove(Vector2Int dir, HashSet<Crate> stickyGroup) {
                if (stickyGroup.Count > 0) {
                    var obstacles = new HashSet<Crate>();
                    // loop all units' position & gather objs that need to be pushed
                    foreach (var stickyObj in stickyGroup) {
                        foreach (var pos in stickyObj.GetUnitsPosition()) {
                            var dest = pos + dir;
                            // destination out of range (out of ground/map)
                            if (gm.IsObstacle(dest)) {
                                return false;
                            }
                            // hit other, gather other
                            if (gm.Get<Crate>(dest, out var other) && !stickyGroup.Contains(other)) {
                                obstacles.Add(other);
                            }
                        }
                    }
                    // check if can move
                    if (!TryMove(dir, obstacles)) {
                        return false;
                    }
                }
                // let's go!
                Move(dir, stickyGroup);
                return true;
            }
        }

        private HashSet<Crate> GetStickyGroup(Crate crate) {
            var res = new HashSet<Crate>();
            var q = new Queue<Crate>();
            q.Enqueue(crate);
            while (q.Count > 0) {
                var cur = q.Dequeue();
                if (res.Contains(cur)) continue; // deduplicate
                res.Add(cur);
                // gather
                var adjacentSticky = gm.GetAdjacent<Crate>(cur, AdjacentFilter.StickyCrate);
                foreach (var adj in adjacentSticky) {
                    if (!res.Contains(adj)) {
                        q.Enqueue(adj);
                    }
                }
            }
            return res;
        }

        // move the entire sticky group
        private void Move(Vector2Int dir, HashSet<Crate> stickyGroup) {
            foreach (var x in stickyGroup) x.UnRegister();
            foreach (var stickCrate in stickyGroup) {
                stickCrate.Move(dir);
            }
            foreach (var x in stickyGroup) x.Register();
        }
    }
}
