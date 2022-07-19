using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Handy2DTools.Utils
{
    public static class Math
    {
        public static float Binary(float directionSign)
        {
            return directionSign != 0 ? Mathf.Sign(directionSign) : 0;
        }

        public static Vector2 Binary(Vector2 vector)
        {
            float x = vector.x != 0 ? MathF.Sign(vector.x) : 0;
            float y = vector.y != 0 ? Mathf.Sign(vector.y) : 0;
            return new Vector2(x, y);
        }

        public static Vector2 RoundDirections(Vector2 directions, float limit = 0.4f)
        {
            float x = Mathf.Abs(directions.x) > limit ? MathF.Sign(directions.x) : directions.x;
            float y = Mathf.Abs(directions.y) > limit ? Mathf.Sign(directions.y) : directions.y;
            return new Vector2(x, y);
        }
    }
}