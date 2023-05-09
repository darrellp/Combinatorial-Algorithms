namespace Combinatorial_Algorithms.Graph_Algorithms
{
    public interface ITagged<T>
    {
        T? Tag { get; set; }
    }
    public interface IDistance
    {
        double Distance { get; }
    }

    public struct DstStr : IDistance
    {
        public double Distance { get; }

        public DstStr(double dist)
        {
            Distance = dist;
        }

        public static implicit operator double(DstStr d) => d.Distance;
        public static implicit operator DstStr(double distance) => new DstStr(distance);
    }


}