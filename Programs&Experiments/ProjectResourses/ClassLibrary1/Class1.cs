using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace ClassLibrary1
{
	public class Class1
	{
		public int i = 9;

		public Image getImage()
		{
			
			//Image wer = new Bitmap(ClassLibrary1."ascent.jpg"); 
			//Image tyu = new Bitmap(typeof(Class1), "ascent.jpg");

			//ClassLibrary1.Class1.
			//return tyu;
			//return new Bitmap(23, 23);
			return ResourceHelper.GetImage("ClassLibrary1.Resourses.ascent.jpg");
			//typeof(ClassLibrary1).Assembly.GetManifestResourceNames
		}

		static class ResourceHelper
		{
			private static Stream GetStream(string resourceName)
			{
				return typeof(ResourceHelper).Assembly.GetManifestResourceStream(resourceName);
			}

			public static string GetText(string resourceName)
			{
				using (StreamReader reader = new StreamReader(GetStream(resourceName)))
				{
					return reader.ReadToEnd();
				}
			}

			public static Image GetImage(string resourceName)
			{
				using (Stream imageStream = GetStream(resourceName))
				{
					return Image.FromStream(imageStream);
				}
			}
		}
	}
}
