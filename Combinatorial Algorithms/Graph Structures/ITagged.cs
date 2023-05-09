namespace Combinatorial_Algorithms.Graph_Structures
{
    public interface ITagged<T>
    {
        T? Tag { get; set; }
    }
    public interface IDistance
    {
        double Distance { get; }
    }

    public struct DistanceStruct : IDistance
    {
        public double Distance { get; }

        public DistanceStruct(double dist)
        {
            Distance = dist;
        }

        public static implicit operator double(DistanceStruct d) => d.Distance;
        public static implicit operator DistanceStruct(double distance) => new DistanceStruct(distance);
    }


}