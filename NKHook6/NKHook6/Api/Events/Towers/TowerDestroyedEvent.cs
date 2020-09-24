﻿using Assets.Scripts.Simulation.Towers;

namespace NKHook6.Api.Events.Towers
{
    public class TowerDestroyedEvent
    {
        public class Prefix : EventBase
        {
            public Tower tower;
            public bool replaceMethod { get; set; }

            public Prefix(ref Tower __instance) : base("TowerDestroyedEvent.Pre")
            {
                this.tower = __instance;
            }
        }

        public class Postfix : EventBase
        {
            public Tower tower;

            public Postfix(ref Tower __instance) : base("TowerDestroyedEvent.Post")
            {
                this.tower = __instance;
            }
        }
    }
}
