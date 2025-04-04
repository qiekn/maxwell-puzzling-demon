namespace qiekn.core {
    public class GameManager {
        private static GameManager instance;

        public static GameManager Instance {
            get {
                instance ??= new GameManager();
                return instance;
            }
        }

        public void UpdateGame() {
            HeatSystem.Instance.Process();
            MergeSystem.Instance.Process();
        }
    }
}
