using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grsu.Lab.Aoc.Contracts;

namespace TestAntSystem1.Classes
{
    public class AlgorithmCreator
    {
        public IGraph CreatorGraph;

        public IList<IAnt> CreatorAnts;

        public IList<Node> CreatorMinPath;

        public int CreatorNForBig;

        public AlgorithmCreator(Stream stream)
        {
            CreatorGraph = new Graph();
            CreatorGraph.LoadGraph(stream);
            
            CreatorAnts = new List<IAnt>();
            CreatorMinPath = new List<Node>();
        }

        public IAlgorithm CreateStandartAlgorithm()
        {
            IList<INode> tMinPath = (IList<INode>) StandartAntAlgorithm.DeepObjectClone(CreatorGraph.Nodes);
            tMinPath.Add(tMinPath.First());

            return new StandartAntAlgorithm
            {
                Graph = CreatorGraph,
                Ants = CreatorAnts,
                Pheromone = ((Graph) CreatorGraph).Info.Item1,
                ExtraPheromone = ((Graph) CreatorGraph).Info.Item2,
                CurrentIteration = 0,
                CurrentIterationNoChanges = 0,
                MaxIterationsNoChanges = ((Graph) CreatorGraph).Info.Item4,
                MaxIterations = ((Graph) CreatorGraph).Info.Item5,
                MinPath = CreatorGraph.Nodes,
                NForBig = 50,
                BestAnt = new Ant {PathCost = int.MaxValue, VisitedNodes = CreatorGraph.Nodes}
            };
        }
    }
}
