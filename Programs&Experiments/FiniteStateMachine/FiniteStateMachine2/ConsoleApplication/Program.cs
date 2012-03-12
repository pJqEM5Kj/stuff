using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace ConsoleApplication
{
	class Program
	{
		public static void Main(string[] args)
		{
			object result = null;

			bool error = true;
			try
			{
				result = FiniteStateMachine.process(args.Cast<object>().ToArray());
				error = false;
			}
			catch (FiniteStateMachineException ex)
			{
				Console.WriteLine(string.Format("Error while processing: {0}", ex.Message));
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("FiniteStateMachine error: {0}", ex));
			}

			if (error)
			{
				return;
			}

			if (result != null)
			{
				Console.WriteLine("Result is " + result.ToString());
			}
			else
			{
				Console.WriteLine("Result is null.");
			}
		}
	}


	interface IState {}

	interface INormalState : IState
	{
		IState execute(FiniteStateMachineVariables variables);
	}

	interface IErrorState : IState
	{
		string getErrorMessage(FiniteStateMachineVariables variables);
	}

	interface IFinishState : IState
	{
		object getResult(FiniteStateMachineVariables variables);
	}



	class FiniteStateMachine
	{
		public static object process(object[] args)
		{
			FiniteStateMachineVariables variables = new FiniteStateMachineVariables(args);
			IState state = new StartState();
			List<IState> statesStack = new List<IState>();

			while (true)
			{
				statesStack.Add(state);

				if (state is INormalState)
				{
					state = ((INormalState)state).execute(variables);
				}
				else if (state is IFinishState)
				{
					return ((IFinishState)state).getResult(variables);
				}
				else if (state is IErrorState)
				{
					throw new FiniteStateMachineException(((IErrorState)state).getErrorMessage(variables));
				}
				else
				{
					throw new Exception(string.Format("Unknown kind of state '{0}'", state.GetType().Name));
				}
			}
		}
	}

	class FiniteStateMachineVariables
	{
		public int i;
		public string defaultErrorMessage;
		public object result;

		public List<object> input;

		public FiniteStateMachineVariables(object[] input)
		{
			i = 10;
			defaultErrorMessage = string.Empty;
			result = null;
			this.input = input.ToList();
		}

		private int currentInputIndx = -1;
		public object NextInput
		{
			get
			{
				currentInputIndx++;
				return input[currentInputIndx];
			}
		}

		public object CurrentInput
		{
			get
			{
				return input[currentInputIndx];
			}
		}

		public bool inputIsOver
		{
			get
			{
				return currentInputIndx + 1 < input.Count;
			}
		}
	}
	
	class FiniteStateMachineException : Exception
	{
		public FiniteStateMachineException() : base() {}

		public FiniteStateMachineException(string message) : base(message) {}

		protected FiniteStateMachineException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public FiniteStateMachineException(string message, Exception innerException) : base(message, innerException) {}
	}


	
	class StartState : INormalState
	{
		public IState execute(FiniteStateMachineVariables variables)
		{
			if (new Random().NextDouble() > 0.5)
			{
				return new State2();
			}
			else
			{
				return new State3();
			}
		}
	}

	class State2 : INormalState
	{
		public IState execute(FiniteStateMachineVariables variables)
		{
			variables.defaultErrorMessage = "Error message " + new Random().Next();

			return new BlaBlaError();
		}
	}

	class State3 : INormalState
	{
		public IState execute(FiniteStateMachineVariables variables)
		{
			variables.result = new Random().Next();

			return new FinishStateDefault();
		}
	}

	class BlaBlaError : IErrorState
	{
		public string getErrorMessage(FiniteStateMachineVariables variables)
		{
			return "Shit happened - " + variables.defaultErrorMessage + " - be happy!";
		}
	}

	class FinishStateDefault : IFinishState
	{
		public object getResult(FiniteStateMachineVariables variables)
		{
			if (variables.result is int && ((int)variables.result) % 2 == 0)
			{
				return string.Format("{0} :-)", variables.result);
			}
			else
			{
				return variables.result;
			}
		}
	}
}