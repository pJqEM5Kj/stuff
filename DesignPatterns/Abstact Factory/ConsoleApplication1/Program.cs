using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ConsoleApplication1
{
	class Program
	{
		public static void Main(string[] args)
		{
			Man man = new Man();
			
			IFactory factory = getFactory();

			man.getDressed(factory.getShoes(), factory.getDress());

			Console.WriteLine("Man's clothes:");
			Console.WriteLine(man.dress.GetType().Name);
			Console.WriteLine(man.shoes.GetType().Name);
        }

		private static IFactory getFactory()
		{
			if (new Random().NextDouble() > 0.5)
			{
				return new NikeFactory();
			}
			else
			{
				return new AdidasFactory();
			}
		}
	}

	class Man
	{
		public IDress dress;
		public IShoes shoes;

		public void getDressed(IShoes shoes, IDress dress)
		{
			this.dress = dress;
			this.shoes = shoes;
		}
	}


	interface IDress {}

	interface IShoes {}
	
	interface IFactory
	{
		IDress getDress();
		IShoes getShoes();
	}


	
	class AdidasFactory : IFactory
	{
		public IDress getDress()
		{
			return new AdidasShirt();
		}

		public IShoes getShoes()
		{
			return new AdidasSandals();
		}
	}

	class AdidasShirt : IDress {}

	class AdidasSandals : IShoes {}


	class NikeFactory : IFactory
	{
		public IDress getDress()
		{
			return new NikeShirt();
		}

		public IShoes getShoes()
		{
			return new NikeSandals();
		}
	}

	class NikeShirt : IDress { }

	class NikeSandals : IShoes { }

}