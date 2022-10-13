namespace VD
{
	using UnityEngine;

	public static class MathHelper 
	{
		// create zero Vector only once because when use Vector3.zero or Vector2.zero, Unity return new Vector3(0, 0, 0)
		public static readonly Vector3 zeroVector3 = Vector3.zero;
		public static readonly Vector2 zeroVector2 = Vector2.zero;

		public static bool IsZero(Vector3 thisVector)
		{
			return thisVector == zeroVector3;
		}

		public static bool IsZero(Vector2 thisVector)
        {
			return thisVector == zeroVector2;
        }
	}
}
