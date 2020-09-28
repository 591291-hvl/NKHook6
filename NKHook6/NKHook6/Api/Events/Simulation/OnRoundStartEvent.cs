﻿using Assets.Scripts.Simulation;

namespace NKHook6.Api.Events._Simulation
{
    public partial class SimulationEvents
    {
        public class OnRoundStartEvent
        {
            public class Pre : EventBaseCancellable
            {
                public Simulation instance;
                public int roundArrayIndex;

                public Pre(ref Simulation __instance, ref int roundArrayIndex) : base("Simulation.OnRoundStartEvent.Pre")
                {
                    this.instance = __instance;
                    this.roundArrayIndex = roundArrayIndex;
                }
            }

            public class Post : EventBase
            {
                public Simulation instance;
                public int roundArrayIndex;

                public Post(ref Simulation __instance, ref int roundArrayIndex) : base("Simulation.OnRoundStartEvent.Post")
                {
                    this.instance = __instance;
                    this.roundArrayIndex = roundArrayIndex;
                }
            }
        }
    }
}