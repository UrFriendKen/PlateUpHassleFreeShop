using Kitchen;
using KitchenData;
using KitchenLib.Utils;
using System;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHassleFreeShop
{
    public class IngredientParcelsStartOpen : NightSystem
    {
        EntityQuery parcelsQuery;
        //MethodInfo m_Perform;

        protected override void Initialise()
        {
            base.Initialise();
            parcelsQuery = GetEntityQuery(new QueryHelper()
                .All(typeof(CLetterIngredient), typeof(CPosition)));
            //m_Perform = typeof(OpenApplianceParcel).GetMethod("Perform", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        protected override void OnUpdate()
        {
            if (Main.Mod_Enabled)
            {
                NativeArray<Entity> parcels = parcelsQuery.ToEntityArray(Allocator.Temp);
                foreach (Entity parcel in parcels)
                {
                    if (!Require(parcel, out CLetterIngredient letter) || !Require(parcel, out CPosition position))
                    {
                        continue;
                    }
                    //m_Perform.Invoke(World.GetExistingSystem<OpenIngredientParcel>(), );
                    CreateIngredientAppliance(letter, position);
                    EntityManager.DestroyEntity(parcel);
                    Main.LogInfo($"Destroyed Parcel.");
                }
            }
        }

        protected void CreateIngredientAppliance(CLetterIngredient letter, CPosition position)
        {
            int iD = base.Data.ReferableObjects.DefaultProvider.ID;
            if (base.Data.TryGet<Item>(letter.IngredientID, out var output, warn_if_fail: true))
            {
                Appliance dedicatedProvider = output.DedicatedProvider;
                iD = ((dedicatedProvider == null) ? base.Data.ReferableObjects.DefaultProvider.ID : dedicatedProvider.ID);
            }
            Entity entity = EntityManager.CreateEntity();
            Set(entity, new CCreateAppliance
            {
                ID = iD
            });
            Set(entity, CItemProvider.InfiniteItemProvider(letter.IngredientID));
            Set(entity, new CPosition(position));

            string providerName = "Unknown";
            try
            {
                providerName = GDOUtils.GetExistingGDO(iD).name;
            } catch (NullReferenceException) { }
            Main.LogInfo($"Created Ingredient Provider {providerName} ({iD}) from parcel.");
        }
    }
}
