using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Media;
using System.Windows.Input;

namespace ConsoleApplication1
{
	class Program
	{
		public static List<int> list = new List<int>() { 0, 1, 2, 3, 4, 5, };

		static void Main(string[] args)
		{
			//
		}
	}

	/// Clone strategy
	
	// 1 only clone
	class A : ICloneable
	{
		public int III;

		public A()
		{
			//
		}

		public object Clone()
		{
			return Clone(null);
		}

		protected virtual object Clone(object clone)
		{
			if (clone == null) clone = new A();

			var a = (A)clone;
			a.III = III;
			return a;
		}
	}

	class B : A
	{
		public int JJJ;

		public B()
		{
			//
		}

		protected override object Clone(object clone)
		{
			if (clone == null) clone = new B();

			base.Clone(clone);

			var b = (B)clone;
			b.JJJ = JJJ;
			return b;
		}
	}

	class C : B
	{
		public int KKK;

		public C()
		{
			//
		}

		protected override object Clone(object clone)
		{
			if (clone == null) clone = new C();

			base.Clone(clone);

			var c = (C)clone;
			c.KKK = KKK;
			return c;
		}
	}

	// 2 use copy ctor
	class A2 : ICloneable
	{
		public int III;

		public A2()
		{
			//
		}

		public A2(A2 copy)
		{
			if (copy == null) throw new ArgumentNullException();

			copy.III = III;
		}

		public virtual object Clone()
		{
			return new A2(this);
		}
	}

	class B2 : A2
	{
		public int JJJ;

		public B2()
		{
			//
		}

		public B2(B2 copy)
			: base(copy)
		{
			copy.JJJ = JJJ;
		}

		public override object Clone()
		{
			return new B2(this);
		}
	}

	class C2 : B2
	{
		public int KKK;

		public C2()
		{
			//
		}

		public C2(C2 copy)
			: base(copy)
		{
			copy.KKK = KKK;
		}

		public override object Clone()
		{
			return new C2(this);
		}
	}
}
