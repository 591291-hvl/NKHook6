﻿using Assets.Scripts.Simulation.Bloons;
using Harmony;
using NKHook6.Api.Events;
using NKHook6.Api.Events.Bloons;

namespace NKHook6.Patches.Bloons
{
    [HarmonyPatch(typeof(Bloon), "OnDestroy")]
    class BloonPoppedHook
    {
        private static bool sendPrefixEvent = true;
        private static bool sendPostfixEvent = true;

        [HarmonyPrefix]
        internal static bool Prefix(ref Bloon __instance)
        {
            bool allowOriginalMethod = true;
            if (sendPrefixEvent)
            {
                var o = new BloonPoppedEvent.Prefix(ref __instance);
                EventRegistry.subscriber.dispatchEvent(ref o);
                __instance = o.bloon;
                allowOriginalMethod = !o.replaceMethod;
            }

            sendPrefixEvent = !sendPrefixEvent;

            return allowOriginalMethod;
        }

        [HarmonyPostfix]
        internal static void Postfix(ref Bloon __instance)
        {
            if (sendPostfixEvent)
            {
                var o = new BloonPoppedEvent.Postfix(ref __instance);
                EventRegistry.subscriber.dispatchEvent(ref o);
                __instance = o.bloon;
            }

            sendPostfixEvent = !sendPostfixEvent;
        }
    }
}
