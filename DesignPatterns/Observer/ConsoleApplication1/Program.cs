using System;
using System.Collections.Generic;
using System.Text;

//design teplates: observer

namespace ConsoleApplication1
{
    public abstract class Observer
    {
        public abstract void handleEvent(object sender, object[] eventArguments);
    }

    public class Eventt
    {
        List<Observer> _observers = new List<Observer>();

        public void addObserver(Observer observer)
        {
            _observers.Add(observer);
        }

        public void delObserver(Observer observer)
        {
            _observers.Remove(observer);
        }

        public void raiseEvent(object sender, object[] arguments)
        {
            foreach (Observer observer in _observers)
            {
                observer.handleEvent(sender, arguments);
            }
        }
    }

    //---------------------------
    public class MyClass1
    {
        public Eventt Click = new Eventt();

        void method1()
        {
            Click.raiseEvent(this, new object[] { 12, 4, "re" });
        }
    }

    public class MyClass2
    {
        //concrete event handler
        public class HandlerForMyClass1ClickEvent : Observer
        {
            public override void handleEvent(object sender, object[] eventArguments)
            {
                //handling event
                Console.WriteLine("event handled");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MyClass1 publisher = new MyClass1();
            MyClass2 observingClass = new MyClass2();

            Observer observer = new MyClass2.HandlerForMyClass1ClickEvent();

            publisher.Click.addObserver(observer);
            publisher.Click.raiseEvent(publisher, new object[] { 1, 2, "sdf" });

        }
    }
}

namespace ConsoleApplication2 //with delegates
{
    public class Observer
    {
        public Observer()
        {
            //
        }

        public Observer(HandleEvent eventHandler)
        {
            HandleThisEvent += eventHandler;
        }

        public delegate void HandleEvent(object sender, object[] parameters);

        public HandleEvent HandleThisEvent;
    }

    public class Eventt
    {
        List<Observer> _observers = new List<Observer>();

        public void addObserver(Observer observer)
        {
            _observers.Add(observer);
        }

        public void delObserver(Observer observer)
        {
            _observers.Remove(observer);
        }

        public void raiseEvent(object sender, object[] arguments)
        {
            foreach (Observer observer in _observers)
            {
                observer.HandleThisEvent(sender, arguments);
            }
        }
    }
    //----------------------------

    public class MyClass1
    {
        public Eventt Click = new Eventt();

        void method1()
        {
            Click.raiseEvent(this, new object[] { 12, 4, "re" });
        }
    }

    public class MyClass2
    {
        //concrete event handler
        public void MyClass1ClickEventHandler(object sender, object[] parameters)
        {
            //handling event
            Console.WriteLine("event handled");
        }
    }

    class Program
    {
        static void Main2(string[] args)
        {
            MyClass1 publisher = new MyClass1();
            MyClass2 observingClass = new MyClass2();

            Observer observer = new Observer(observingClass.MyClass1ClickEventHandler);

            publisher.Click.addObserver(observer);
            publisher.Click.raiseEvent(publisher, new object[] { 1, 2, "sdf" });
        }
    }
}