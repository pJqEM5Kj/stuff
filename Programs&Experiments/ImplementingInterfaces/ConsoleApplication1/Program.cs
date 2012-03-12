using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1
{
	class myClass1 : IEnumerable<int>
	{
		public int[] _mass = new int[0];

		public myClass1()
		{
		}

		IEnumerator<int> IEnumerable<int>.GetEnumerator()
		{
			return new EnumeratorForMyClass(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new Exception();
			//return new EnumeratorForMyClass(this);
		}

		class EnumeratorForMyClass : IEnumerator<int>
		{
			myClass1 _parent = null;
			int _current = -1;
			
			object IEnumerator.Current
			{
				get
				{
					throw new Exception();	
				    //return _parent._mass[_current];
				}
			}

			int IEnumerator<int>.Current
			{
				get
				{
					if (_current < 0 || _current > _parent._mass.Length - 1)
					{
						throw new System.InvalidOperationException();
					}

					return _parent._mass[_current];
				}
			}

			public bool MoveNext()
			{
				_current++;
				
				if (_current > _parent._mass.Length - 1)
				{
					return false;
				}

				return true;
			}

			public void Reset()
			{
				_current = -1;
			}

			public void Dispose() {}
			
			public EnumeratorForMyClass(myClass1 parent)
			{
				_parent = parent;
			}
		}
	}

	class myClass2
	{
		int i;

		public myClass2(int parameter)
		{
			i = parameter;
		}

		public static int comparisonXY(myClass2 x, myClass2 y)
		{
			if (x.i < y.i)
			{
				return -1;
			}
			else
			{
				if (x.i > y.i)
				{
					return 1;
				}

				return 0;
			}
		}

		public static IComparer<myClass2> getComparer()
		{
			return new myClass2Comparer();
		}

		class myClass2Comparer : IComparer<myClass2>
		{
			public int Compare(myClass2 x, myClass2 y)
			{
				if (x.i < y.i)
				{
					return -1;
				}
				else
				{
					if (x.i > y.i)
					{
						return 1;
					}

					return 0;
				}
			}
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			myClass1 qqq = new myClass1();
			qqq._mass = new int[4];

			qqq._mass[0] = 0;
			qqq._mass[1] = 1;
			qqq._mass[2] = 2;
			qqq._mass[3] = 3;

			List<int> wer = new List<int>(qqq);

			List<myClass2> asd = new List<myClass2>();

			asd.Add(new myClass2(4));
			asd.Add(new myClass2(6));
			asd.Add(new myClass2(0));
			asd.Add(new myClass2(3));

			asd.Sort(myClass2.comparisonXY);

			asd.Add(new myClass2(-4));
			asd.Add(new myClass2(-6));
			asd.Add(new myClass2(0));
			asd.Add(new myClass2(-3));

			asd.Sort(myClass2.getComparer());
		}
	}
}
