namespace ConsoleVideo.Media {
    public interface IFrame<T> {
        public T GetPixel(int index);

        public T GetPixel(int y, int x);

        public void SetPixel(int index, T value);

        public void SetPixel(int y,
                             int x,
                             T value);
    }
}