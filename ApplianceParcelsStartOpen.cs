using Kitchen;
using KitchenLib.Utils;
using System;
using Unity.Collections;
using Unity.Entities;

namespace KitchenHassleFreeShop
{
    public class ApplianceParcelsStartOpen : NightSystem
    {
        EntityQuery parcelsQuery;

        protected override void Initialise()
        {
            base.Initialise();
            parcelsQuery = GetEntityQuery(new QueryHelper()
                .All(typeof(CLetterAppliance), typeof(CPosition)));
        }

        protected override void OnUpdate()
        {
            if (Main.Mod_Enabled)
            {
                NativeArray<Entity> parcels = parcelsQuery.ToEntityArray(Allocator.Temp);
                foreach (Entity parcel in parcels)
                {
                    if (!Require(parcel, out CLetterAppliance letter) || !Require(parcel, out CPosition position))
                    {
                        continue;
                    }
                    CreateAppliance(letter, position);
                    EntityManager.DestroyEntity(parcel);
                    Main.LogInfo($"Destroyed Parcel.");
                }
            }
        }

        protected void CreateAppliance(CLetterAppliance letter, CPosition position)
        {
            Entity entity = EntityManager.CreateEntity();
            Set(entity, new CCreateAppliance
            {
                ID = letter.ApplianceID
            });
            Set(entity, new CPosition(position));

            string applianceName = "Unknown";
            try
            {
                applianceName = GDOUtils.GetExistingGDO(letter.ApplianceID).name;
            }
            catch (NullReferenceException) { }
            Main.LogInfo($"Created Appliance {applianceName} ({letter.ApplianceID}) from parcel.");
        }
    }
}
