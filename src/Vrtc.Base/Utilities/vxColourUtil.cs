using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VerticesEngine.Utilities
{
    public static class vxColourUtil
    {
        /// <summary>
        /// Convertes a Hex Value to a RGB Colour
        /// </summary>
        /// <param name="hexValue">a 6 character hex value</param>
        /// <returns></returns>
        public static Color HexToRGB(string hexValue)
        {
            if (hexValue.Length != 6)
            {
                vxConsole.WriteError(string.Format("Hex Value '{0}' is not 6 characters long", hexValue));
                return Color.Magenta;
            }
            var r = int.Parse(hexValue.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            var g = int.Parse(hexValue.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            var b = int.Parse(hexValue.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            return new Color(r, g, b);
        }


        /// <summary>
        /// Converts a Colour to a hex code
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string RGBToHex(Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
