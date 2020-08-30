using Terraria.ModLoader;

namespace Limelight
{
	public class Limelight : Mod
	{
        #region Fields
        public static bool Autosize;
        #endregion

        public override void LoadResources()
        {
            base.LoadResources();
            Autosize = false;
        }

        public override void PostAddRecipes()
        {
            base.PostAddRecipes();
            Autosize = true;
        }
    }
}