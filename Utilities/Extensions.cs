using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Limelight.Utilities
{
    public static class Extensions
    {
        public static bool TryModify<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, Action<T> modification)
        {
            T target = enumerable.FirstOrDefault(predicate);
            if (target == null) return false;

            modification.Invoke(target);
            return true;
        }

        public static Vector2 DirectionTo(this Vector2 origin, Vector2 target) => Vector2.Normalize(target - origin);
    }
}
