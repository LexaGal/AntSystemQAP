using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Grsu.Lab.Aoc.Contracts;

namespace TestAntSystem1.Classes
{
    [Serializable]
    public class Graph : IGraph
    {
        public IList<IEdge> Edges { get; set; }
        
        public IList<INode> Nodes { get; set; }
        
        public int NForBig { get; set; }
        
        public int[,] FlowMatrix { get; set; }
        
        public int[,] DistanceMatrix { get; set; }
        
        public Tuple<int, int, int, int, int> Info { get; set; }

        public IList<INode> GetSibilings(INode node)
        {
            List<INode> retNodes = new List<INode>();

            if (Nodes.Count < NForBig)
            {
                for (int currentStep = 0; currentStep < Nodes.Count; currentStep++)
                {
                    if (DistanceMatrix[node.Id, currentStep] != int.MaxValue)
                    {
                        retNodes.Add(new Node {Id = currentStep});
                    }
                }
            }
            else
            {
                retNodes.AddRange(Edges.Where(edge => edge.Begin == node.Id).Select(edge => new Node {Id = edge.End}));
            }
            return retNodes;
        }

        public void LoadGraph(Stream stream)
        {
            int pheromone = 0;
            int extraPheromone = 0;
            int noUpdatesLimit = 0;
            int nIterations = 0;
            int nAnts = 0;
            NForBig = 50;

            using (TextReader textReader = new StreamReader(stream))
            {
                textReader.ReadLine();
                string s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    pheromone = int.Parse(s);
                }

                textReader.ReadLine();
                s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    extraPheromone = int.Parse(s);
                }

                textReader.ReadLine();
                s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    noUpdatesLimit = int.Parse(s);
                }

                textReader.ReadLine();
                s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    nIterations = int.Parse(s);
                }

                textReader.ReadLine();
                s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    nAnts = int.Parse(s);
                }

                Info = new Tuple<int, int, int, int, int>(pheromone,
                    extraPheromone, nAnts, noUpdatesLimit, nIterations);
                
                DistanceMatrix = new int[nAnts, nAnts];
                FlowMatrix = new int[nAnts, nAnts];
                Nodes = new List<INode>();
                Edges = new List<IEdge>();

                foreach (var id in Enumerable.Range(0, nAnts))
                {
                    Nodes.Add(new Node {Id = id});
                }

                int i = 0;
                while (!String.IsNullOrEmpty(s))
                {
                    s = textReader.ReadLine();

                    if (!String.IsNullOrEmpty(s))
                    {
                        if (s == "Distances Matrix:" || s == "Flows Matrix:")
                        {
                            continue;
                        }

                        string[] strings = Regex.Split(s, @"\D+");

                        if (i < nAnts)
                        {
                            for (int j = 0; j < strings.Length; j++)
                            {
                                DistanceMatrix[i, j] = int.Parse(strings[j]);

                                Edges.Add(new Edge {Begin = i, End = j, HeuristicInformation = DistanceMatrix[i, j], Pheromone = pheromone});
                            }
                        }
                        else
                        {
                            for (int j = 0; j < strings.Length; j++)
                            {
                                FlowMatrix[i - nAnts, j] = int.Parse(strings[j]);
                            }
                        }
                    }
                    i++;
                }
            }
        }
    }
}

    
