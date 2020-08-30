using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limelight.Utilities
{
    public static class Helpers
    {
		public static float GetLerpValue(float from, float to, float t, bool clamped = false)
		{
			if (clamped)
			{
				if (from < to)
				{
					if (t < from)
						return 0f;

					if (t > to)
						return 1f;
				}
				else
				{
					if (t < to)
						return 1f;

					if (t > from)
						return 0f;
				}
			}

			return (t - from) / (to - from);
		}
	}
}
