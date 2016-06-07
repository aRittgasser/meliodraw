using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace Melio.Draw.Models
{
	class Font
	{
		private void foo()
		{
			//Graphics fx;
			FontFamily fontFamily = new FontFamily(GenericFontFamilies.SansSerif);
			FontStyle style = FontStyle.Regular;
			string glyph = "A";
			float flyphFontSize = 60.0f;

			PointF[] pts = null;
			byte[] ptsType = null;

			using (var path = new GraphicsPath())
			{
				path.AddString(glyph, fontFamily, (int)style, flyphFontSize,
					new PointF(0f, 0f), StringFormat.GenericDefault);

				path.Flatten();

				pts = path.PathPoints;
				ptsType = path.PathTypes;
			}
		}
	}
}
