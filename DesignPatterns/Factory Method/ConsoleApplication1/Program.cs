using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

//design teplates: Factory Method
namespace ConsoleApplication1
{
	class Program
	{
		//static 
			void Main(string[] args)
		{
			// An array of creators 
			const int N = 2;
			Creator[] creators = new Creator[N];
			creators[0] = new ConcreteCreatorA();
			creators[1] = new ConcreteCreatorB();

			// Iterate over creators and create products 
			Product[] products = new Product[N];
			for (int i = 0; i < N; i++)
			{
				products[i] = creators[i].FactoryMethod();
				Console.WriteLine("Created {0}", products[i].GetType().Name);
			}
        }
	}

	// "Product" 
	abstract class Product
	{
	}

	// "ConcreteProductA" 
	class ConcreteProductA : Product
	{
	}

	// "ConcreteProductB" 
	class ConcreteProductB : Product
	{
	}

	// "Creator" 
	abstract class Creator
	{
		public abstract Product FactoryMethod();
	}

	// "ConcreteCreator" 
	class ConcreteCreatorA : Creator
	{
		public override Product FactoryMethod()
		{
			return new ConcreteProductA();
		}
	}

	// "ConcreteCreator" 
	class ConcreteCreatorB : Creator
	{
		public override Product FactoryMethod()
		{
			return new ConcreteProductB();
		}
	}
}

namespace ConsoleApplication2 // real variant
{
	class Program
	{
		static 
			void Main()
		{
			// Note: constructors call Factory Method 
			Document[] documents = new Document[2];
			documents[0] = new Resume();
			documents[1] = new Report();

			// Display document pages 
			foreach (Document document in documents)
			{
				Console.WriteLine("\n" + document.GetType().Name + "--");
				foreach (Page page in document.Pages)
				{
					Console.WriteLine(" " + page.GetType().Name);
				}
			}
		}
	}

	// "Product" 
	abstract class Page
	{
	}

	// "ConcreteProduct" 
	class SkillsPage : Page
	{
	}

	// "ConcreteProduct" 
	class EducationPage : Page
	{
	}

	// "ConcreteProduct" 
	class ExperiencePage : Page
	{
	}

	// "ConcreteProduct" 
	class IntroductionPage : Page
	{
	}

	// "ConcreteProduct" 
	class ResultsPage : Page
	{
	}

	// "ConcreteProduct" 
	class ConclusionPage : Page
	{
	}

	// "ConcreteProduct" 
	class SummaryPage : Page
	{
	}

	// "ConcreteProduct" 
	class BibliographyPage : Page
	{
	}

	// "Creator" 
	abstract class Document
	{
		private ArrayList pages = new ArrayList();

		// Constructor calls abstract Factory method 
		public Document()
		{
			this.CreatePages();
		}

		public ArrayList Pages
		{
			get { return pages; }
		}

		// Factory Method 
		public abstract void CreatePages();
	}

	// "ConcreteCreator" 
	class Resume : Document
	{
		// Factory Method implementation 
		public override void CreatePages()
		{
			Pages.Add(new SkillsPage());
			Pages.Add(new EducationPage());
			Pages.Add(new ExperiencePage());
		}
	}

	// "ConcreteCreator" 
	class Report : Document
	{
		// Factory Method implementation 
		public override void CreatePages()
		{
			Pages.Add(new IntroductionPage());
			Pages.Add(new ResultsPage());
			Pages.Add(new ConclusionPage());
			Pages.Add(new SummaryPage());
			Pages.Add(new BibliographyPage());
		}
	}
}
