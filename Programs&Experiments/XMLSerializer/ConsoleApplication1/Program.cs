using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ConsoleApplication1
{
	public class MyClass
	{
		public int i;
		public int[] arrayy;


		public MyClass()
		{
			i = 7;
			arrayy = new int[5];
			Random rnd = new Random();


			for (int ii = 0; ii < 5; ii++)
			{
				arrayy[ii] = rnd.Next();
			}
		}

		public void method1()
		{
			int r = 5;
			r++;
		}

		public override string ToString()
		{
			return " i = " + i.ToString();
		}

	}

	static class WorkWithXML
	{
		public static void write2file(Object x, string name)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(MyClass));
			StreamWriter writer = new StreamWriter(name, false);

			serializer.Serialize(writer, x);
			writer.Close();
		}

		public static void readFromFile(string name, ref MyClass x)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(MyClass));
			StreamReader reader = new StreamReader(name);

			x = (MyClass)serializer.Deserialize(reader);
			reader.Close();
		}

	}
	
	class Program
	{
		static void Main(string[] args)
		{

			MyClass xxx = new MyClass();
			xxx.i = 56;

			WorkWithXML.write2file(xxx, @"d:\qwe.xml");


			MyClass yyy = new MyClass();
			
			WorkWithXML.readFromFile(@"d:\qwe.xml", ref yyy);

			Console.WriteLine(yyy.ToString());
		}
	}
}
