using System;
using System.Collections.Generic;
using System.Text;

//design teplates: Strategy
namespace ConsoleApplication1
{
	public abstract class AbstractSortStrategy
	{
		public abstract byte[] sort(byte[] bytes);
	}

	public class BubleSortStrategy : AbstractSortStrategy
	{
		public override byte[] sort(byte[] bytes)
		{
			List<byte> list = new List<byte>(bytes);

			list.Sort();

			return list.ToArray();
		}
	}

	public class QuickSortStrategy : AbstractSortStrategy
	{
		public override byte[] sort(byte[] bytes)
		{
			List<byte> list = new List<byte>(bytes);

			list.Sort();

			return list.ToArray();
		}
	}


	class Program
	{
		static void useSomeSort(AbstractSortStrategy sortStrategy)
		{
			//just sort even without knowing how
			sortStrategy.sort(new byte[10]);
		}
		
		static void Main(string[] args)
		{
			//decide and choose strategy
			AbstractSortStrategy sortStrategy = new BubleSortStrategy();
			AbstractSortStrategy sortStrategy2 = new QuickSortStrategy();
			
			//pass chosen algorithm as parameter
			useSomeSort(sortStrategy2);
		}
	}
}
