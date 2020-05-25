using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Utility.ColorUtilities
{
	public static class ColorUtilities
	{
        public static Color ToColor(this string self)
        {
            Color color;
            if (!ColorUtility.TryParseHtmlString(self, out color))
            {
                throw new Exception(self + ": is not an actual hexadecimal colors!");
            }

            return color;
        }
        public static string ToStringRGB(this Color self)
        {
            return ColorUtility.ToHtmlStringRGB(self);
        }
        public static string ToStringRGBA(this Color self)
        {
            return ColorUtility.ToHtmlStringRGBA(self);
        }

		public static Color ColorBlend(Color color1, Color color2,Vector2 specificValue)
		{
			if (specificValue.x == 0)
			{
				return color2;
			}
			if (specificValue.y == 0)
			{
				return color1;
			}

			if (specificValue.x < specificValue.y)
			{
				return ColorBlend(color2,color1, specificValue.x / specificValue.y);
			}
			else
			{
				return ColorBlend(color1, color2, specificValue.y / specificValue.x);
			}
		}

		public static Color ColorBlend(Color color1, Color color2, float difference)
		{
			return Color.Lerp(color1, color2, difference);
		}
	}
}