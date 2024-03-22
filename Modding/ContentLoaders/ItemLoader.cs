using Block2D.Common;
using Block2D.Modding.DataStructures;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Block2D.Modding.ContentLoaders
{
    public class ItemLoader : ContentLoader
    {
        public ItemLoader(Mod mod) : base(mod)
        {
            FilesPath = Main.ModsDirectory + "/" + Mod.InternalName + "/ItemData";
        }

        public bool TryLoadItems(out ModItem[] items)
        {
            items = null;

            if (!ModContains())
            {
                return false;
            }

            string[] itemFilePaths = GetFilePaths();
            List<ModItem> modItemList = new();

            for (int i = 0; i < itemFilePaths.Length; i++)
            {
                if (!itemFilePaths[i].Contains(LUA_FILE_EXTENSION))
                {
                    return false;
                }

                Script script = new();
                Main.SetupScript(script);

                ModItem item = new();

                script.DoFile(itemFilePaths[i]);
                SetScript(script);

                DynValue nameVal = GetGlobal("Name");
                item.Name = nameVal.String;

                DynValue damageVal = GetGlobal("Damage");
                item.Damage = (int)damageVal.Number;

                DynValue typeval = GetGlobal("Type");

                item.Type = (int)typeval.Number;

                DynValue heightVal = GetGlobal("Height");
                DynValue widthVal = GetGlobal("Width");

                item.Size = new((int)widthVal.Number, (int)heightVal.Number);

                DynValue stackableVal = GetGlobal("Stackable");
                item.Stackable = stackableVal.Boolean;

                DynValue maxStackSizeVal = GetGlobal("MaxStackSize");
                item.MaxStackSize = (int)maxStackSizeVal.Number;

                modItemList.Add(item);
            }

            items = modItemList.ToArray();

            return false;
        }
    }
}
