using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ConsoleApplication
{
	class Program
	{
		enum test { er, tr };

		static void Main(string[] args)
		{
			object obj = null;

			//B b = new B();

			//E e = new E();
			//e.b.e = e;

			//obj = DeepCopier.makeDeepCopy(new A());

			//E copyE = (E)DeepCopier.makeDeepCopy(e);

			//obj = DeepCopier.makeDeepCopy(b);
			//obj = DeepCopier.makeDeepCopy((I)(new C()));

			//D d = D.getInstance();

			int[] d = new int[3] { 1, 2, 3 };
			//List<int> d = new List<int>() { 1, 2, 3 };
			//int[] d = new int[3] { 1, 2, 3 };

			obj = DeepCopier.makeDeepCopy(d);

			//Stru sstruct = new Stru();
			//sstruct.a = new A();
			//F f = new F();
			//f.ss = sstruct;

			//obj = DeepCopier.makeDeepCopy(f);

			//object objjjj = new Func<int, int>(delegate(int i) { return i + 1; });
			//obj = DeepCopier.makeDeepCopy(objjjj);
		}
	}

	struct Stru
	{
		public A a;
	}

	interface I
	{
		void method(A a);
	}
	
	class A
	{
		int j = 66;
		string str2 = "s333333333df";
	}

	class B
	{
		//private A a = new A();
		//int i = 6;
		//public string str = "sdf";
		//char ch = 'r';

		public E e = null;
	}

	class C : I
	{
		#region I Members

		public void method(A a)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	class D
	{
		public D inst = null;

		private D()
		{
		}

		public static D getInstance()
		{
			D d = new D();
			d.inst = d;
			return d;
		}
	}

	class E
	{
		int i = 8;
		public B b = new B();
	}

	class F
	{
		public Stru ss;
	}

	static class DeepCopier
	{
		public static Object makeDeepCopy(object obj)
		{
			return makeDeepCopy(obj, new List<object>(),
				new List<object>(), true);
		}

		public static Object makeDeepCopy(object obj, bool shallowCopyForDelegates)
		{
			return makeDeepCopy(obj, new List<object>(), 
				new List<object>(), shallowCopyForDelegates);
		}

		private static Object makeDeepCopy(object obj,
			List<object> objRefFields, List<object> copyRefFields,
			bool shallowCopyForDelegates)
		{
			if (obj == null)
			{
				return null;
			}

			object copy = DeepCopier.getRawDuplicate(obj);

			if (obj is String || obj is Enum)
			{
				return copy;
			}

			if (shallowCopyForDelegates && obj is Delegate)
			{
				return copy;
			}

			objRefFields.Add(obj);
			copyRefFields.Add(copy);

			Type objType = obj.GetType();

			FieldInfo[] objFields = objType.GetFields(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			int inx = 0;
			object fieldValue = null;
			for (int i = 0; i < objFields.Length; i++)
			{
				fieldValue = objFields[i].GetValue(obj);

				if (fieldValue == null)
				{
					objFields[i].SetValue(copy, null);
					continue;
				}

				if (fieldValue.GetType().IsPrimitive)
				{
					objFields[i].SetValue(copy, fieldValue);
					continue;
				}

				if (objRefFields.Contains(fieldValue))
				{
					inx = objRefFields.IndexOf(fieldValue);
					objFields[i].SetValue(copy, copyRefFields[inx]);
					continue;
				}


				object fieldValueCopy = DeepCopier.makeDeepCopy(fieldValue,
					objRefFields, copyRefFields, shallowCopyForDelegates);

				objFields[i].SetValue(copy, fieldValueCopy);


				objRefFields.Add(fieldValue);
				copyRefFields.Add(fieldValueCopy);
			}

			return copy;
		}

		private static Object getRawDuplicate(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			#region simple cases
			if (obj is String)
			{
				return String.Copy((String)obj);
			}

			if (obj is ValueType)
			{
				return obj;
			}

			if (obj is Delegate)
			{
				return ((Delegate)obj).Clone();
			}

			if (obj is Array)
			{
				//
			}
			#endregion

			Type type = obj.GetType();

			#region 1 way
			try
			{
				return System.Runtime.Serialization.FormatterServices.
					GetUninitializedObject(type);
			}
			catch
			{
			}
			#endregion

			#region 2 way
			try
			{
				return System.Activator.CreateInstance(type, true);
			}
			catch
			{
			}
			#endregion

			#region non documented opportunity
			try
			{
				RuntimeTypeHandle typeHandle = type.TypeHandle;

				return typeHandle.GetType().GetMethod("Allocate",
					BindingFlags.Instance | BindingFlags.NonPublic).
						Invoke(typeHandle, null);
			}
			catch
			{
			}
			#endregion
	
			#region through getting default constructor
			try
			{
				ConstructorInfo[] constructors = type.GetConstructors(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				ConstructorInfo constructor = null;
				foreach (ConstructorInfo cctor in constructors)
				{
					if (cctor.GetParameters().Length == 0)
					{
						constructor = cctor;
						break;
					}
				}

				return constructor.Invoke(null);
			}
			catch
			{
			}
			#endregion

			throw new Exception("Can't create duplicate instance.");
		}
	}
}
