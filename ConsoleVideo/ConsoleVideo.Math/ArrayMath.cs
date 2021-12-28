namespace ConsoleVideo.Math; 

public static class ArrayMath {
    public static int GetIndex(int y,
                               int x,
                               int xSize) =>
        ((xSize * y) + x);
}