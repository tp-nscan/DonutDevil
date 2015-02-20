namespace NodeLib
{
    public interface INode
    {
        float Value { get; }
        int GroupIndex { get; }
    }

    public static class Node
    {
        public static INode Make(float value, int groupIndex)
        {
            return new NodeImpl(value, groupIndex);
        }
    }

    public class NodeImpl : INode
    {
        private readonly float _value;
        private readonly int _groupIndex;

        public NodeImpl(float value, int groupIndex)
        {
            _value = value;
            _groupIndex = groupIndex;
        }

        public float Value
        {
            get { return _value; }
        }

        public int GroupIndex
        {
            get { return _groupIndex; }
        }
    }
}
