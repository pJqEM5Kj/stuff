using System;
using System.Collections;
using System.Collections.Generic;

namespace Chain_of_responsibility
{
	public interface IChain
	{
		bool canExecute(object command);

		void Execute(object command);
	}

	public class Chain
	{
		private List<IChain> list;

		public List<IChain> List
		{
			get
			{
				return this.list;
			}
		}

		public Chain()
		{
			this.list = new List<IChain>();
		}

		public void Message(object command)
		{
			foreach (IChain item in this.list)
			{
				if (item.canExecute(command))
				{
					item.Execute(command);
					break;
				}
			}
		}

		public void Add(IChain handler)
		{
			this.list.Add(handler);
		}
	}

	public class StringHandler : IChain
	{
		public bool canExecute(object command)
		{
			return command is string;
		}

		public void Execute(object command)
		{
			Console.WriteLine("StringHandler handle this message : {0}", (string)command);
		}
	}

	public class IntegerHandler : IChain
	{
		public bool canExecute(object command)
		{
			return command is int;
		}

		public void Execute(object command)
		{
			Console.WriteLine("IntegerHandler handle this message : {0}", (int)command);
		}
	}

	public class NullHandler : IChain
	{
		public bool canExecute(object command)
		{
			return command == null;
		}

		public void Execute(object command)
		{
			Console.WriteLine("NullHandler handle this message.");
		}
	}

	public class DefaultHandler : IChain
	{
		public bool canExecute(object command)
		{
			return true;
		}

		public void Execute(object command)
		{
			Console.WriteLine("DefaultHandler handle this message {0}", command.ToString());
		}
	}

	class TestMain
	{
		static void Main(string[] args)
		{
			Chain chain = new Chain();

			chain.Add(new StringHandler());
			chain.Add(new IntegerHandler());
			chain.Add(new NullHandler());
			chain.Add(new DefaultHandler());

			chain.Message("1st string value");
			chain.Message(100);
			chain.Message("2nd string value");
			chain.Message(4.7f);
			chain.Message(null);
		}
	}
}