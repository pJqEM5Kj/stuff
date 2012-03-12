using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

namespace ConsoleApplication1
{
	delegate Color DelegateForColorInvokation();
	
	class Program
	{
		static void Main(string[] args)
		{
			Type type = typeof(Colors);

			DelegateForColorInvokation delegateForColorInvokation = null;
			string result = "";

			foreach (MethodInfo method in type.GetMethods())
			{
				try
				{
					delegateForColorInvokation = (DelegateForColorInvokation)Delegate.CreateDelegate(typeof(DelegateForColorInvokation), method);
				}
				catch
				{
					//get out from loop
					break;
				}

				string colorName = method.Name;
				colorName = colorName.Remove(0, 4);

				Color color = delegateForColorInvokation();

				result +=
					"<add color_name=\""
					+ colorName.ToLower()
					+ "\" color_value=\""
					+ color.ToString() + "\" />"
					+ "\n";
			}

			Console.WriteLine(result);
			File.WriteAllText("D:\\colorsss.txt", result);
		}
	}
}
