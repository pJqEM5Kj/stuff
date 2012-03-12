using System;
using System.Collections.Generic;
using System.Text;

//design teplates: Proxy
namespace ConsoleApplication1
{
    //lazy initialization features can be used
    
    class MyClass
    {
        public void method1() { }
        public void method2() { }
    }

    public sealed class MyClassProxy
    {
        // or not sealed and protected
        private MyClass myClass2 = null;

        public MyClassProxy()
        {
            myClass2 = new MyClass();
        }

        public void method2()
        {
            myClass2.method2();
        }
    }

    //----------------------------------
    
    class MyClass2
    {
        private MyClass2() { }

        public static MyClass2Proxy getProxy()
        {
            //return new MyClass2Proxy(new MyClass2());
            return new MyClass2Proxy();
        }

        public MyClass2Proxy getProxyForThis()
        {
            return new MyClass2Proxy(this);
        }

        private void method1() { }
        private void method2() { }

        public sealed class MyClass2Proxy
        {
            private MyClass2 parent = null;

            public MyClass2Proxy()
            {
                parent = new MyClass2();
            }
            
            public MyClass2Proxy(MyClass2 myClass2)
            {
                parent = myClass2;
            }

            public void method2()
            {
                parent.method2();
            }
        }
    }


    class Program
	{
		static void Main(string[] args)
		{
            MyClassProxy myClassProxy = new MyClassProxy();
            myClassProxy.method2();

            //----------------------------------

            MyClass2.getProxy().method2();
        }
	}
}
