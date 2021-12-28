namespace ConsoleVideo.Media; 

public struct Bgr {
    public byte R, G, B;
    
    public Bgr(byte r, byte g, byte b) => (R, G, B) = (r, g, b);
}