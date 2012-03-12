using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1
{
	//Events
	// событи€ - схема »«ƒј“≈Ћ№-ѕќƒѕ»—„»  //
	
	//определили делегат дл€ подписки на событие
	public delegate void MyDelegat();

	//класс »«ƒј“≈Ћ№
	class Aa
	{
		//декларируем событие которое может произойти
		public event MyDelegat MyEvent;

		//что-то происходит и Ё“ќ“ класс говорит всем ѕќƒѕ»—ј¬Ў»ћ—я 
		//о том что это событие произошло
		public void somethisHappensAndMyEventOccurs()
		{
			MyEvent();
		}
	}

	//ѕќƒѕ»—„» 
	class Bb
	{
		public void method() { }
	}
	
	//------------------------------
	class Program
	{
		static void Main(string[] args)
		{
			Aa x = new Aa();
			Bb y = new Bb();

			//экземпл€р класса Bb был подписан на событие публикуемое классом Aa
			//т.е. при возникновении событи€ экземпл€р класса Bb будет уведомлен о 
			//нем с помощью вызова своего же метода
			x.MyEvent += new MyDelegat(y.method);

		}
	}
}
