using System;

namespace Combinatorial_Algorithms.PriorityQueue
{
    // ReSharper disable once InconsistentNaming
	public class DeletableWrapper<BaseType> : IDeletable
	{
		public BaseType Value { get; set; }
		public int Index { get; set; }
		private readonly Func<BaseType, BaseType, int> _compare;

		public DeletableWrapper(BaseType value, Func<BaseType, BaseType, int> compare = null)
		{
			Value = value;
			Index = -1;
			_compare = compare;
		}

        public static implicit operator BaseType(DeletableWrapper<BaseType> value)
        {
            return value.Value;
        }

        public static implicit operator DeletableWrapper<BaseType>(BaseType value)
        {
            return new DeletableWrapper<BaseType>(value);
        }

        public int CompareTo(object obj)
		{
            var otherWrapper = obj as DeletableWrapper<BaseType>;
			if (_compare != null)
			{
				return _compare(Value, otherWrapper.Value);
			}
			var cmpThis = Value as IComparable;
			var cmpOther = otherWrapper.Value as IComparable;
			if (cmpThis == null || cmpOther == null)
			{
				throw new InvalidOperationException("No comparison function and Attrs are not IComparable");
			}
			return cmpThis.CompareTo(cmpOther);
		}

		public override string ToString()
		{
			return "[" + Value.ToString() + "]";
		}

        public void SetIndex(int i)
        {
            throw new NotImplementedException();
        }
    }
}

