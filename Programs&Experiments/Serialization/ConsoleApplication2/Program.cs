using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace ConsoleApplication2
{
	[Serializable]
	public class Aa
	{
		public int i = 5;

		public Bb er;

		public Aa()
		{
			er = new Bb();
		}
	}
	
	[Serializable]
	public class Bb
	{
		public int u = 10;
		public int j = 20;
		public string wer = "asdf";
	}

	public class Copier
	{
		private static readonly BinaryFormatter _formatter = new BinaryFormatter();

		public static T GetDeepCopy<T>(T obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");
			using (MemoryStream source = new MemoryStream())
			{
				_formatter.Serialize(source, obj);
				source.Position = 0;
				return (T) _formatter.Deserialize(source);
			}
		}

		
	}


	class Program
	{
		static volatile Object obj;

		static MemoryStream memstream = new MemoryStream();

		static void Main(string[] args)
		{
			Aa x = new Aa();

			Aa y = Copier.GetDeepCopy<Aa>(x);

			y.i = 12;

			Console.WriteLine(x.i);

		}
	}
}
