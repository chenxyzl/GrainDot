using System;

namespace Base.Helper
{
	public static class TimeHelper
	{
		private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

		public static long NowSeconds()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000000;
		}

		public static long Now()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000; ;
		}
    }
}