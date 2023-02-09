using Kitchen;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHassleFreeShop
{
    public class LettersStartOpen : NightSystem
    {
        EntityQuery lettersQuery;
        protected override void Initialise()
        {
            base.Initialise();
            lettersQuery = GetEntityQuery(new QueryHelper()
                .All(typeof(CLetterBlueprint), typeof(CPosition)));
        }

        protected override void OnUpdate()
        {
            if (Main.Mod_Enabled)
            {
                NativeArray<Entity> letters = lettersQuery.ToEntityArray(Allocator.Temp);
                foreach (Entity letter in letters)
                {
                    PostHelpers.OpenBlueprintLetter(new EntityContext(EntityManager), letter);
                    EntityManager.DestroyEntity(letter);
                    Main.LogInfo("Opened letter.");
                }
            }
        }
    }
}
