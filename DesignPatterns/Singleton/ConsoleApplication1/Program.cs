using System;
using System.Collections.Generic;
using System.Text;

//design teplates: Singleton
namespace ConsoleApplication1
{
    public class Singleton // normal
    {
        private static Singleton instance = null;
        
        private Singleton()
        {
            //
        }

        public static Singleton getInstance()
        {
            if (instance == null)
            {
                instance = new Singleton();
            }

            return instance;
        }
    }

    public class Singleton2 // property
    {
        private static Singleton2 instance = null;
        public static Singleton2 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton2();
                }

                return instance;
            }
        }
        
        private Singleton2()
        {
            //
        }
    }

    class Program
	{
		static void Main(string[] args)
		{
            //
		}
	}
}
