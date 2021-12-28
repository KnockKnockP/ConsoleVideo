namespace ConsoleVideo.Media; 

public interface IFrame {
    public char GetPixel(int y, int x);

    public void SetPixel(int y,
                         int x,
                         object value);
}