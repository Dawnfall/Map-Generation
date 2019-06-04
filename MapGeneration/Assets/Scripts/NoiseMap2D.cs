namespace mesh
{
    public class NoiseMap2D //TODO:... generic 2D Data
    {
        public float[] m_noiseMap;
        public int m_width;
        public int m_height;

        public NoiseMap2D(float[] noiseMap, int width, int height)
        {
            m_noiseMap = noiseMap;
            m_width = width;
            m_height = height;
        }
    }
}