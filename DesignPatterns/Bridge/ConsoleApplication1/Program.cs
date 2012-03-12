using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

//design teplates: Bridge
namespace ConsoleApplication1 //structural example
{
	//abstraction
	class Abstraction
	{
        protected Implementor implementor;
        public Implementor Implementor
		{
			set 
			{ 
				implementor = value; 
			}
		}

		public virtual void Operation()
		{
			implementor.Operation();
		}
	}

	//implementor
    abstract class Implementor
    {
        public abstract void Operation();
    }

	//refinedAbstraction
	class RefinedAbstraction : Abstraction
	{
        public override void Operation()
        {
            implementor.Operation();
        }
	}

	//concreteImplementorA
    class ConcreteImplementorA : Implementor
	{
        public override void Operation()
		{
			Console.WriteLine("ConcreteImplementorA Operation");
		}
	}

	//concreteImplementorB
    class ConcreteImplementorB : Implementor
	{
        public override void Operation()
		{
			Console.WriteLine("ConcreteImplementorB Operation");
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			Abstraction ab = new RefinedAbstraction();

			ab.Implementor = new ConcreteImplementorA();
			ab.Operation();

			ab.Implementor = new ConcreteImplementorB();
			ab.Operation();
		}
	}
}

namespace ConsoleApplication2 //real example
{
    //abstraction
    class CustomersBase
    {
        protected string group;

        private DataObject dataObject;
        public DataObject Data
        {
            set
            {
                dataObject = value;
            }

            get
            {
                return dataObject;
            }
        }

        public CustomersBase(string group)
        {
            this.group = group;
        }

        public virtual void Next()
        {
            dataObject.NextRecord();
        }

        public virtual void Prior()
        {
            dataObject.PriorRecord();
        }

        public virtual void New(string name)
        {
            dataObject.NewRecord(name);
        }

        public virtual void Delete(string name)
        {
            dataObject.DeleteRecord(name);
        }

        public virtual void Show()
        {
            dataObject.ShowRecord();
        }

        public virtual void ShowAll()
        {
            Console.WriteLine("Customer Group: " + group);
            dataObject.ShowAllRecords();
        }
    }

    //refinedAbstraction
    class Customers : CustomersBase
    {
        // Constructor 
        public Customers(string group)
            : base(group)
        {
        }

        public override void ShowAll()
        {
            // Add separator lines 
            Console.WriteLine();
            Console.WriteLine("------------------------");
            base.ShowAll();
            Console.WriteLine("------------------------");
        }
    }

    //implementor
    abstract class DataObject
    {
        public abstract void NextRecord();
        public abstract void PriorRecord();
        public abstract void NewRecord(string name);
        public abstract void DeleteRecord(string name);
        public abstract void ShowRecord();
        public abstract void ShowAllRecords();
    }

    //concreteImplementor
    class CustomersData : DataObject
    {
        private ArrayList customers = new ArrayList();
        private int current = 0;

        public CustomersData()
        {
            // Loaded from a database 
            customers.Add("Jim Jones");
            customers.Add("Samual Jackson");
            customers.Add("Allen Good");
            customers.Add("Ann Stills");
            customers.Add("Lisa Giolani");
        }

        public override void NextRecord()
        {
            if (current <= customers.Count - 1)
            {
                current++;
            }
        }

        public override void PriorRecord()
        {
            if (current > 0)
            {
                current--;
            }
        }

        public override void NewRecord(string name)
        {
            customers.Add(name);
        }

        public override void DeleteRecord(string name)
        {
            customers.Remove(name);
        }

        public override void ShowRecord()
        {
            Console.WriteLine(customers[current]);
        }

        public override void ShowAllRecords()
        {
            foreach (string name in customers)
            {
                Console.WriteLine(" " + name);
            }
        }
    }

    class Program
    {
        static void Main2()
        {
            // Create RefinedAbstraction 
            Customers customers = new Customers("Chicago");

            // Set ConcreteImplementor 
            customers.Data = new CustomersData();

            // Exercise the bridge 
            customers.Show();
            customers.Next();
            customers.Show();
            customers.Next();
            customers.Show();
            customers.New("Henry Velasquez");

            customers.ShowAll();
        }
    }
}
