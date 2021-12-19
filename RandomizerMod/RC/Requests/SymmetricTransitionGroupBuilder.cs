﻿using RandomizerCore;
using RandomizerCore.Randomization;

namespace RandomizerMod.RC
{
    public class SymmetricTransitionGroupBuilder : GroupBuilder
    {
        public readonly Bucket<string> Group1 = new();
        public readonly Bucket<string> Group2 = new();
        public bool coupled;
        public string reverseLabel;

        public override void Apply(List<RandomizationGroup> groups, RandoFactory factory)
        {
            if (Group1.GetTotal() != Group2.GetTotal())
            {
                throw new InvalidOperationException($"Failed to build group {label} due to unbalanced counts.");
            }

            List<IRandoCouple> t1s = new();
            foreach (string s in Group1.EnumerateWithMultiplicity())
            {
                t1s.Add(factory.MakeTransition(s));
            }

            List<IRandoCouple> t2s = new();
            foreach (string s in Group2.EnumerateWithMultiplicity())
            {
                t2s.Add(factory.MakeTransition(s));
            }

            if (coupled)
            {
                CoupledRandomizationGroup g1 = new()
                {
                    Items = t1s.ToArray<IRandoItem>(),
                    Locations = t2s.ToArray<IRandoLocation>(),
                    Label = label,
                    Strategy = strategy ?? new DefaultGroupPlacementStrategy(0),
                };
                CoupledRandomizationGroup g2 = new()
                {
                    Items = t2s.ToArray<IRandoItem>(),
                    Locations = t1s.ToArray<IRandoLocation>(),
                    Label = reverseLabel,
                    Strategy = strategy ?? new DefaultGroupPlacementStrategy(0),
                };
                g1.Dual = g2;
                g2.Dual = g1;

                groups.Add(g1);
                groups.Add(g2);
            }
            else
            {
                RandomizationGroup g1 = new()
                {
                    Items = t1s.ToArray<IRandoItem>(),
                    Locations = t2s.ToArray<IRandoLocation>(),
                    Label = label,
                    Strategy = strategy ?? new DefaultGroupPlacementStrategy(0),
                };
                RandomizationGroup g2 = new()
                {
                    Items = t2s.ToArray<IRandoItem>(),
                    Locations = t1s.ToArray<IRandoLocation>(),
                    Label = reverseLabel,
                    Strategy = strategy ?? new DefaultGroupPlacementStrategy(0),
                };

                groups.Add(g1);
                groups.Add(g2);
            }
        }
    }
}
