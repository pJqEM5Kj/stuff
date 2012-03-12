using System;
using System.Collections.Generic;

namespace ConsoleApplication
{
	class Program
	{
		public static void Main(string[] args)
		{
			//
			FiniteStateMachine machine = new FiniteStateMachine(args);

			machine.Start();

			if (machine.Result.HasValue)
			{
				Console.WriteLine(machine.Result);
			}
			else
			{
				Console.WriteLine("No result.");
			}
		}
	}

	class FiniteStateMachine
	{
		public readonly State state;
		public Stack<State> states = new Stack<State>();

		public readonly string[] input;

		public int? Result = null;

		public FiniteStateMachine(string[] input)
		{
			this.input = input;
			state = new StartState(this);
		}

		public void Start()
		{
			state.Operation();
		}

		private int currentInputIndx = 0;

		public string getCurrentInput()
		{
			if (input.Length >= currentInputIndx)
			{
				return null;
			}
			else
			{
				return input[currentInputIndx];
			}
		}

		public string getNextInput()
		{
			if (currentInputIndx >= input.Length)
			{
				return null;
			}

			do
			{
				currentInputIndx++;

				if (currentInputIndx >= input.Length)
				{
					return null;
				}
			}
			while (string.IsNullOrEmpty(input[currentInputIndx]));

			return getCurrentInput();
		}

		public bool isInputOver
		{
			get
			{
				return getCurrentInput() == null;
			}
		}
	}

	abstract class State
	{
		public readonly FiniteStateMachine machine;

		public State(FiniteStateMachine finiteStateMachine)
		{
			machine = finiteStateMachine;
		}

		public void Operation()
		{
			machine.states.Push(this);

			OperationImpl();
		}

		protected abstract void OperationImpl();
	}

	class StartState : State
	{
		public StartState(FiniteStateMachine finiteStateMachine) : base(finiteStateMachine) { }

		protected override void OperationImpl()
		{
			if (machine.isInputOver || machine.getCurrentInput() == "")
			{
				machine.Result = 1;
				new State2(machine).Operation();
			}
			else
			{
				machine.Result = 2;
				new State3(machine).Operation();
			}
		}
	}

	class State2 : State
	{
		public State2(FiniteStateMachine machine) : base(machine) { }

		protected override void OperationImpl()
		{
			//throw new NotImplementedException();
		}
	}

	class State3 : State
	{
		public State3(FiniteStateMachine machine) : base(machine) { }

		protected override void OperationImpl()
		{
			//throw new NotImplementedException();
		}
	}
}