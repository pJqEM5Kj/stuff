using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1
{
	//Events
	// ������� - ����� ��������-��������� //
	
	//���������� ������� ��� �������� �� �������
	public delegate void MyDelegat();

	//����� ��������
	class Aa
	{
		//����������� ������� ������� ����� ���������
		public event MyDelegat MyEvent;

		//���-�� ���������� � ���� ����� ������� ���� ������������� 
		//� ��� ��� ��� ������� ���������
		public void somethisHappensAndMyEventOccurs()
		{
			MyEvent();
		}
	}

	//���������
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

			//��������� ������ Bb ��� �������� �� ������� ����������� ������� Aa
			//�.�. ��� ������������� ������� ��������� ������ Bb ����� ��������� � 
			//��� � ������� ������ ������ �� ������
			x.MyEvent += new MyDelegat(y.method);

		}
	}
}
