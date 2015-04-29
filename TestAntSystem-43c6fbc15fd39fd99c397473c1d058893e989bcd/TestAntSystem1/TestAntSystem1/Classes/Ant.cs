using System.Collections.Generic;
using Grsu.Lab.Aoc.Contracts;

namespace TestAntSystem1.Classes
{
    public class Ant : IAnt
    {
        public IList<INode> VisitedNodes { get; set; }
        public int PathCost { get; set; }
    }
}
