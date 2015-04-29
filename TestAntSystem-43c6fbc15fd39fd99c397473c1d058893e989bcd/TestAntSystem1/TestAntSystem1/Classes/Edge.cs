using System;
using Grsu.Lab.Aoc.Contracts;

namespace TestAntSystem1.Classes
{
    [Serializable]
    public class Edge : IEdge
    {
        public int Begin { get; set; }

        public int End { get; set; }

        public double HeuristicInformation { get; set; }

        public double Pheromone { get; set; }
    }
}