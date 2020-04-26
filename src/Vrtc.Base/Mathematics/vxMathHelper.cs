using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using VerticesEngine.Utilities;

namespace VerticesEngine.Mathematics
{
	/// <summary>
	/// Math Helper Library with an extension of Mathematically Helpful Functions
	/// </summary>
	public static partial class vxMathHelper
	{

		/// <summary>
		/// Rounds to nearest half.
		/// </summary>
		/// <returns>The to nearest half.</returns>
		/// <param name="value">Value.</param>
		public static float RoundToNearestHalf(float value)
		{
			return (float)Math.Round(value * 2) / 2;
		}

		/// <summary>
		/// Rounds to nearest ten.
		/// </summary>
		/// <returns>The to nearest ten.</returns>
		/// <param name="value">The index.</param>
		public static int RoundToNearestTen (float value)
		{
			return ((int)Math.Round(value / 10.0)) * 10;
		}

		/// <summary>
		/// Rounds to nearest specified number.
		/// </summary>
		/// <returns>The to nearest specified number.</returns>
		/// <param name="value">Value.</param>
		/// <param name="valueToRoundTo">Value to round to.</param>
		public static float RoundToNearestSpecifiedNumber (float value, float valueToRoundTo)
		{
			return ((float)Math.Round(value / valueToRoundTo)) * valueToRoundTo;
		}

		/// <summary>
		/// Rounds to nearest specified number.
		/// </summary>
		/// <returns>The to nearest number.</returns>
		/// <param name="value">Value.</param>
		/// <param name="valueToRoundTo">Value to round to.</param>
		public static Vector2 RoundVector2(Vector2 value, float valueToRoundTo)
		{
			return new Vector2 (
				RoundToNearestSpecifiedNumber (value.X, valueToRoundTo),
				RoundToNearestSpecifiedNumber (value.Y, valueToRoundTo)
			);
		}


		public static float Map(float s, float a1, float a2, float b1, float b2)
		{
			return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
		}

		public static int Map(int s, float a1, float a2, float b1, float b2)
		{
			return (int)(b1 + (s - a1) * (b2 - b1) / (a2 - a1));
		}


		/// <summary>
		/// Gets the vector2 string.
		/// </summary>
		/// <returns>The vector2 string.</returns>
		/// <param name="vector">Vector.</param>
		/// <param name="DecPt">Dec point.</param>
		public static string GetVector2String(Vector2 vector, float DecPt)
		{
			float x = RoundToNearestSpecifiedNumber(vector.X, DecPt);
			float y = RoundToNearestSpecifiedNumber(vector.Y, DecPt);

			return "(" + x + ", " + y + ")";
		}


        /// <summary>
        /// Gets the rotated vector.
        /// </summary>
        /// <returns>The rotated vector.</returns>
        /// <param name="rotation">Rotation.</param>
        public static Vector2 GetRotatedVector(float rotation)
        {
            return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        }

        /// <summary>
        /// Gets the rotation from direction.
        /// </summary>
        /// <returns>The rotation from direction.</returns>
        /// <param name="direction">Direction.</param>
        public static float GetRotationFromDirection(Vector2 direction)
        {
            float angle = (float)Math.Atan2(direction.X, direction.Y);

            if (direction.X < 0)
                angle += (float)Math.PI*2;
            return angle;
        }

		#region Smoothing Code

		/// <summary>
		/// Smooths a float by a stepsize.
		/// </summary>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
		static public float Smooth(float whatItIs, float whatItShouldBe, float stepSize)
		{
			return whatItIs + (whatItShouldBe - whatItIs) / stepSize;
		}

		/// <summary>
		/// Smooths a int by a stepsize.
		/// </summary>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
		static public int Smooth(int whatItIs, int whatItShouldBe, int stepSize)
		{
			return whatItIs + (whatItShouldBe - whatItIs) / stepSize;
		}

		/// <summary>
		/// Smooths a Color by a stepsize.
		/// </summary>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
		static public Color Smooth(Color whatItIs, Color whatItShouldBe, float stepSize)
		{
            return Color.Lerp(whatItIs, whatItShouldBe, 1/stepSize);

            //vxConsole.WriteToInGameDebug(whatItIs.ToString());
            //vxConsole.WriteToInGameDebug(whatItShouldBe.ToString());
            
   //         return new Color(
			//	Smooth((float)whatItIs.R, (float)whatItShouldBe.R, stepSize),
			//	Smooth((float)whatItIs.B, (float)whatItShouldBe.B, stepSize),
			//	Smooth((float)whatItIs.G, (float)whatItShouldBe.G, stepSize)
			//);
		}


		/// <summary>
		/// Smooths a 2D Vector by a stepsize.
		/// </summary>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
		static public Vector2 Smooth(Vector2 whatItIs, Vector2 whatItShouldBe, float stepSize)
		{
			whatItIs.X = Smooth(whatItIs.X, whatItShouldBe.X, stepSize);
			whatItIs.Y = Smooth(whatItIs.Y, whatItShouldBe.Y, stepSize);

			return whatItIs;
		}

		/// <summary>
		/// Smooths a 3D Vector by a stepsize.
		/// </summary>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
		static public Vector3 Smooth(Vector3 whatItIs, Vector3 whatItShouldBe, float stepSize)
		{
			whatItIs.X = Smooth(whatItIs.X, whatItShouldBe.X, stepSize);
			whatItIs.Y = Smooth(whatItIs.Y, whatItShouldBe.Y, stepSize);
			whatItIs.Z = Smooth(whatItIs.Z, whatItShouldBe.Z, stepSize);

			return whatItIs;
		}

		#endregion
	}
}
