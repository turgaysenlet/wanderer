using Intel.RealSense;

namespace Wanderer.Software.ImageProcessing
{
    public class RealSense
    {
        public D435 D435 { get; private set; }
        public T264 T264 { get; private set; }

        public RealSense()
        {
            D435 = new D435();
            T264 = new T264();
        }

        public void Start()
        {
            D435.Start();
            T264.Start();
        }
    }
}