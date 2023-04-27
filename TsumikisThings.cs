using log4net;
using log4net.Repository.Hierarchy;
using Terraria.ModLoader;

namespace TsumikisThings
{
	public class TsumikisThings : Mod
	{
		public static void LogDebug(object o)
		{
			ModContent.GetInstance<TsumikisThings>().Logger.Debug(o.ToString());
		}

		internal static TsumikisThings GetMod()
		{
			return ModContent.GetInstance<TsumikisThings>();
		}

		internal static ILog GetLogger()
		{
			return GetMod().Logger;
		}
	}
}