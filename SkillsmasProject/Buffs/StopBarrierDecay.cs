using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using UnityEngine;
using MysticsRisky2Utils;
using R2API;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace Skillsmas.Buffs
{
    public class StopBarrierDecay : BaseBuff
    {
        public static Dictionary<HealthComponent, float> BarrierDecayRate = [];
        public override void OnLoad() {
            buffDef.name = "Skillsmas_StopBarrierDecay";
            buffDef.buffColor = new Color32(214, 201, 58, 255);
            buffDef.iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texBuffGenericShield.tif").WaitForCompletion();
            buffDef.canStack = false;

            Overlays.CreateOverlay(SkillsmasPlugin.AssetBundle.LoadAsset<Material>("Assets/Mods/Skillsmas/Skills/Loader/StopBarrierDecay/matLoaderStopBarrierDecay.mat"), (characterModel) =>
            {
                return characterModel.body && characterModel.body.HasBuff(buffDef);
            });

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.GetBarrierDecayRate += (orig, self) =>
            {
                var r = orig(self);
                if (BarrierDecayRate.ContainsKey(self)) r *= BarrierDecayRate[self];
                return r;
            };
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                args.attackSpeedMultAdd += Skills.Loader.StopBarrierDecay.attackSpeed / 100f;
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.HasBuff(buffDef)) BarrierDecayRate[self.healthComponent] = 0;
            else BarrierDecayRate[self.healthComponent] = 1;
        }
    }
}
