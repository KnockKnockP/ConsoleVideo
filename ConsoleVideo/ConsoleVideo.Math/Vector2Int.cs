namespace ConsoleVideo.Math {
    public struct Vector2Int {
        public int x,
                   y;

        public Vector2Int(int _x, int _y) {
            x = _x;
            y = _y;
            return;
        }

        public override string ToString() => $"({x}, {y})";
    }
}