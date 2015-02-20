namespace MathLib
{
    public class Box<T>
    {
        public Box(T item)
        {
            Item = item;
        }

        public T Item { get; set; }
    }
}
