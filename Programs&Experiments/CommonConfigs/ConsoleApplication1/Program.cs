using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			const string sectionName = "testSection";
			const string sectionName2 = "testSection2";
			const string sectionName3 = "testSection3";

			var ccs = new CommonConfigSection<TestConfigElement>();
			ccs.ConfigElements.Add(new TestConfigElement() { Name = "qqqqqqq", FileName = "wwwwwwwwwww" });
			ccs.ConfigElements.Add(new TestConfigElement() { Name = "qqqqqqq2", FileName = "wwwwwwwwwww2" });

			var ccs2 = new CommonConfigSection<TestConfigElement2>();
			ccs2.ConfigElements.Add(new TestConfigElement2() { NameID = "colorOne", Color = "red" });
			ccs2.ConfigElements.Add(new TestConfigElement2() { NameID = "colorTwo", Color = "green" });

			var ccs3 = new CommonConfigSection<TestConfigElement3>();
			ccs3.ConfigElements.Add(new TestConfigElement3()
			{
				NameID = "colorOne",
				SubItems = new CommonConfigCollection<TestConfigElement2>() 
			    { 
			        new TestConfigElement2() { NameID = "colorOneOne", Color = "red" },
			        new TestConfigElement2() { NameID = "colorTwoTwo", Color = "blue" }
			    }
			});
			ccs3.ConfigElements.Add(new TestConfigElement3() { NameID = "colorTwo" });

			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			//config.Sections.Remove(sectionName);
			//config.Sections.Remove(sectionName2);
			//config.Sections.Remove(sectionName3);

			//config.Sections.Add(sectionName, ccs);
			//config.Sections.Add(sectionName2, ccs2);
			//config.Sections.Add(sectionName3, ccs3);

			//config.Save();

			var testSection1 = (CommonConfigSection<TestConfigElement>)config.GetSection(sectionName);
			var testSection2 = (CommonConfigSection<TestConfigElement2>)config.GetSection(sectionName2);
			var testSection3 = (CommonConfigSection<TestConfigElement3>)config.GetSection(sectionName3);
		}
	}


	public class TestConfigElement : CommonConfigElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
			set
			{
				this["name"] = value;
			}
		}

		[ConfigurationProperty("filename", IsRequired = true)]
		public string FileName
		{
			get
			{
				return (string)this["filename"];
			}
			set
			{
				this["filename"] = value;
			}
		}

		public override object getElementKey()
		{
			return this;
		}
	}

	public class TestConfigElement2 : CommonConfigElement
	{
		[ConfigurationProperty("id", IsRequired = true)]
		public string NameID
		{
			get
			{
				return (string)this["id"];
			}
			set
			{
				this["id"] = value;
			}
		}

		[ConfigurationProperty("color", IsRequired = true)]
		public string Color
		{
			get
			{
				return (string)this["color"];
			}
			set
			{
				this["color"] = value;
			}
		}

		public override object getElementKey()
		{
			return this;
		}
	}

	public class TestConfigElement3 : CommonConfigElement
	{
		[ConfigurationProperty("id", IsRequired = true)]
		public string NameID
		{
			get
			{
				return (string)this["id"];
			}
			set
			{
				this["id"] = value;
			}
		}

		[ConfigurationProperty("subitems")]
		public CommonConfigCollection<TestConfigElement2> SubItems
		{
			get { return (CommonConfigCollection<TestConfigElement2>)this["subitems"]; }
			set { this["subitems"] = value; }
		}

		public override object getElementKey()
		{
			return this;
		}
	}


	public abstract class CommonConfigElement : ConfigurationElement
	{
		public abstract object getElementKey();
	}

	public class CommonConfigCollection<T> : ConfigurationElementCollection,
		IEnumerable<T>, ICollection<T>, IList<T>
		where T : CommonConfigElement, new()
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new T();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((T)element).getElementKey();
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}


		#region ctors

		public CommonConfigCollection()
		{
			//
		}

		public CommonConfigCollection(IEnumerable<T> collection)
			: this()
		{
			setFrom(collection);
		}

		#endregion


		#region IEnumerable<T> Members

		public new IEnumerator<T> GetEnumerator()
		{
			return this.OfType<T>().GetEnumerator();
		}

		#endregion


		#region ICollection<T> Members

		public void Add(T item)
		{
			BaseAdd(item);
		}

		public void Clear()
		{
			BaseClear();
		}

		public bool Contains(T item)
		{
			return BaseIndexOf(item) != -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			base.CopyTo(array, arrayIndex);
		}

		public new bool IsReadOnly
		{
			get { return IsReadOnly(); }
		}

		public bool Remove(T item)
		{
			object key = item.getElementKey();
			BaseRemove(key);
			return BaseIsRemoved(key);
		}

		#endregion


		#region IList<T> Members

		public int IndexOf(T item)
		{
			return BaseIndexOf(item);
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		public T this[int index]
		{
			get { return (T)BaseGet(index); }
			set { throw new NotImplementedException(); }
		}

		#endregion


		#region Helpers

		public void setFrom(IEnumerable<T> collection)
		{
			BaseClear();
			foreach (var item in collection)
			{
				BaseAdd(item);
			}
		}

		#endregion
	}

	public sealed class CommonConfigSection<T> : ConfigurationSection
		where T : CommonConfigElement, new()
	{
		private ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
		private ConfigurationProperty _configProp = new ConfigurationProperty(
			null, typeof(CommonConfigCollection<T>), null, ConfigurationPropertyOptions.IsDefaultCollection);

		public CommonConfigSection()
		{
			_properties.Add(_configProp);
			base[_configProp] = new CommonConfigCollection<T>();
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get { return _properties; }
		}

		[ConfigurationProperty("", IsDefaultCollection = true)]
		public CommonConfigCollection<T> ConfigElements
		{
			get { return (CommonConfigCollection<T>)base[_configProp]; }
		}
	}



	public static class StringUtilities
	{
		public static string FormatStr(this string str, params object[] args)
		{
			return string.Format(str, args);
		}

		public static bool IsNullOrEmptyStr(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static string findDuplicatedChars(string str)
		{
			if (str.IsNullOrEmptyStr()) return null;

			var chars = new HashSet<char>();

			string duplicatedChars = null;
			foreach (var ch in str)
			{
				if (chars.Contains(ch))
				{
					if (duplicatedChars.IsNullOrEmptyStr() || duplicatedChars.Contains(ch) == false)
					{
						duplicatedChars += ch;
					}
				}
				else
				{
					chars.Add(ch);
				}
			}

			return duplicatedChars;
		}

		public static int CountChar(this string str, char c)
		{
			int pos = 0, count = 0;

			while ((pos = str.IndexOf(c, pos)) != -1)
			{
				count++;
				pos++;
			}

			return count;
		}
	}
}
