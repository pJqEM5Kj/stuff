using System;
using System.Drawing;

namespace YOUR_NAMESPACE
{
	public class myColor
	{
		/* Class myColor for keeping and managing colors 
		 * both in RGB (red, blue, green) and 
		 * HSV (hue, saturation, brightness) color spaces
		 * A - transparency or alha component.
			 
		 * * INPUT/OUTPUT: 
		 *		A   : 0 - 255
		 *		RGB : 0 - 255
		 *		H   : 0 - 360
		 *		SV  : 0 - 100
		 * ALL ARE INTEGERS
		/**/

		//-------variables-----------
		//internal variables are double for more convenience and
		//precision, see "make_RGB_from_HSV" and "make_HSV_from_RGB"
		//methods for more details
		private double a, r, g, b, h, s, v;

		//-------constructors--------
		/*default*/
		public myColor()
		{
			//default initialization
			a = r = g = b = h = s = v = 0;
		}
		/*from usual color*/
		public myColor(Color color)
		{
			SetColorRGB(color);
		}
		/*from myColor*/
		public myColor(myColor color)
		{
			SetColorMy(color);
		}
		/*from numbers in HSV color system*/
		public myColor(int A, int H, int S, int V)
		{
			SetColorHSV(A, H, S, V);
		}
		/**/
		//-------interface----------
		//--setting-color
		/**/
		public void SetColorMy(myColor color)
		{
			a = color.A;
			r = color.R;
			g = color.G;
			b = color.B;
			h = color.H;
			s = color.S;
			v = color.V;
		}
		/**/
		public void SetColorRGB(Color color)
		{
			a = color.A;
			r = color.R;
			g = color.G;
			b = color.B;

			make_HSV_from_RGB();
		}
		/**/
		public void SetColorRGB(int A, int R, int G, int B)
		{
			a = A;
			r = R;
			g = G;
			b = B;

			if (a < 0) a = 0;
			if (a > 255) a = 255;

			if (r < 0) r = 0;
			if (r > 255) r = 255;

			if (g < 0) g = 0;
			if (g > 255) g = 255;

			if (b < 0) b = 0;
			if (b > 255) b = 255;

			make_HSV_from_RGB();
		}
		/**/
		public void SetColorHSV(int A, int H, int S, int V)
		{
			a = A;
			h = H;
			s = S;
			v = V;

			if (a < 0) a = 0;
			if (a > 255) a = 255;

			if (h < 0) h = 0;
			if (h > 360) h = 360;

			if (s < 0) s = 0;
			if (s > 100) s = 100;

			if (v < 0) v = 0;
			if (v > 100) v = 100;

			s = s / 100f;
			v = v / 100f;

			make_RGB_from_HSV();
		}
		//--getting-color
		/**/
		public Color GetColorRGB()
		{
			return Color.FromArgb(
							(int)Math.Round(a),
							(int)Math.Round(r),
							(int)Math.Round(g),
							(int)Math.Round(b));
		}
		/**/
		//---class-properties--------
		/*
		 * direct getting and setting paramenters of the color
		 * both RGB and HSV 
		 */
		public int A
		{
			set
			{
				a = value;
				if (a < 0) a = 0;
				if (a > 255) a = 255;

				make_HSV_from_RGB();
			}

			get { return (int)Math.Round(a); }
		}

		public int R
		{
			set
			{
				r = value;
				if (r < 0) r = 0;
				if (r > 255) r = 255;

				make_HSV_from_RGB();
			}

			get { return (int)Math.Round(r); }
		}

		public int G
		{
			set
			{
				g = value;
				if (g < 0) g = 0;
				if (g > 255) g = 255;

				make_HSV_from_RGB();
			}

			get { return (int)Math.Round(g); }
		}

		public int B
		{
			set
			{
				b = value;
				if (b < 0) b = 0;
				if (b > 255) b = 255;

				make_HSV_from_RGB();
			}

			get { return (int)Math.Round(b); }
		}

		public int H
		{
			set
			{
				h = value;
				if (h < 0) a = 0;
				if (h > 360) a = 360;

				make_RGB_from_HSV();
			}

			get { return (int)Math.Round(h); }
		}

		public int S
		{
			set
			{
				s = value;
				if (s < 0) s = 0;
				if (s > 100) s = 100;
				s = s / 100f;

				make_RGB_from_HSV();
			}

			get { return (int)Math.Round(100 * s); }
		}

		public int V
		{
			set
			{
				v = value;
				if (v < 0) v = 0;
				if (v > 100) v = 100;
				v = v / 100f;

				make_RGB_from_HSV();
			}

			get { return (int)Math.Round(100 * v); }
		}
		/**/
		//------implementation------
		/*convert HSV -> RGB*/
		private void make_RGB_from_HSV()
		{
			//we work with r, g, b as 0-1 then *255 for normalization
			// s, v : 0 - 1;  h : 0 - 360

			// achromatic (grey)
			if (s == 0)
			{
				r = g = b = v * 255;
				return;
			}

			double tmp_h = h / 60f;
			int i = (int)Math.Floor(tmp_h);
			double f = tmp_h - i;

			double p = v * (1 - s);
			double q = v * (1 - s * f);
			double t = v * (1 - s * (1 - f));

			switch (i)
			{
				case 0:
					r = v;
					g = t;
					b = p;
					break;

				case 1:
					r = q;
					g = v;
					b = p;
					break;

				case 2:
					r = p;
					g = v;
					b = t;
					break;

				case 3:
					r = p;
					g = q;
					b = v;
					break;

				case 4:
					r = t;
					g = p;
					b = v;
					break;

				// case 5:
				default:
					r = v;
					g = p;
					b = q;
					break;
			}

			r *= 255;
			g *= 255;
			b *= 255;
		}
		/*convert RGB -> HSV*/
		private void make_HSV_from_RGB()
		{
			//we work with s, v : 0 - 1;  h : 0 - 360 
			//r, g, b as 0-255;
			// !!!IMPORTANT!!!
			//s,v - always 0-1 we *100 only when sending them out of this class

			double min, max, delta;

			min = Math.Min(r, Math.Min(g, b));
			max = Math.Max(r, Math.Max(g, b));
			delta = max - min;

			v = max / 255f;

			// achromatic (grey)
			if (delta == 0 || max == 0)
			{
				h = 0;
				s = 0;
				return;
			}

			s = delta / max;

			if (r == max) h = (g - b) / delta;
			else
				if (g == max) h = 2 + (b - r) / delta;
				else
					h = 4 + (r - g) / delta;

			h *= 60;

			if (h < 0) h += 360;
		}
		/**/
	} //class myColor end
}