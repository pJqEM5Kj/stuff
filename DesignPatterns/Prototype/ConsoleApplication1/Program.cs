using System;
using System.Collections.Generic;
using System.Text;

//design teplates: Prototype
namespace ConsoleApplication1
{
    public abstract class ClonnedClass
    {
        public abstract object clone();
    }
    
    public class MyClass : ClonnedClass
    {
        int i;
        string str;
        public int id;

        public override object clone()
        {
            MyClass clon = new MyClass();
            clon.i = this.i;
            clon.str = this.str;
            
            //clon.id = this.id;          // passive clonning
            clon.id = clon.GetHashCode(); // smart clonning

            return clon;

            #region another way
            MyClass clon2 = (MyClass)this.MemberwiseClone();
            clon2.id = clon2.GetHashCode();
            return clon2;

            //or
            //return this.MemberwiseClone();
            #endregion
        }
    }

    //interface implementation
    public interface IClonAvailable
    {
        object clone();
    }

    public class MyClass2 : IClonAvailable
    {
        int i;
        string str;
        public int id;

        public object clone()
        {
            MyClass2 clon = new MyClass2();
            clon.i = this.i;
            clon.str = this.str;

            //clon.id = this.id;          // passive clonning
            clon.id = clon.GetHashCode(); // smart clonning

            return clon;

            #region another way
            MyClass2 clon2 = (MyClass2)this.MemberwiseClone();
            clon2.id = clon2.GetHashCode();
            return clon2;

            //or
            //return this.MemberwiseClone();
            #endregion
        }
    }

    //using C# build-in interface
    public class MyClass3 : ICloneable
    {
        int i;
        string str;
        public int id;

        #region ICloneable Members

        public object Clone()
        {
            MyClass3 clon = new MyClass3();
            clon.i = this.i;
            clon.str = this.str;

            //clon.id = this.id;          // passive clonning
            clon.id = clon.GetHashCode(); // smart clonning

            return clon;

            #region another way
            MyClass3 clon2 = (MyClass3)this.MemberwiseClone();
            clon2.id = clon2.GetHashCode();
            return clon2;

            //or
            //return this.MemberwiseClone();
            #endregion
        }

        #endregion
    }

	class Program
	{
		static void Main(string[] args)
		{
            MyClass m1 = new MyClass();
            MyClass2 m2 = new MyClass2();
            MyClass3 m3 = new MyClass3();

            Console.WriteLine(m1.GetHashCode() + "    " + m1.id);
            Console.WriteLine(m2.GetHashCode() + "    " + m2.id);
            Console.WriteLine(m3.GetHashCode() + "    " + m3.id);

            Console.WriteLine("-----------------------");
            m1 = (MyClass)m1.clone();
            m2 = (MyClass2)m2.clone();
            m3 = (MyClass3)m3.Clone();
            Console.WriteLine("-----------------------");

            Console.WriteLine(m1.GetHashCode() + "    " + m1.id);
            Console.WriteLine(m2.GetHashCode() + "    " + m2.id);
            Console.WriteLine(m3.GetHashCode() + "    " + m3.id);
        }
	}
}
