using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Grsu.Lab.Aoc.Contracts;

namespace TestAntSystem1.Classes
{
    public class StandartAntAlgorithm : IAlgorithm
    {
        public IGraph Graph { get; set; }

        public IList<IAnt> Ants { get; set; }

        public IList<INode> MinPath { get; set; }
        
        public int Pheromone { get; set; }

        public int ExtraPheromone { get; set; }

        public int NForBig { get; set; }

        public IAnt BestAnt { get; set; }

        public IAnt CurrentAnt { get; set; }

        public int MaxIterations { get; set; }

        public int CurrentIteration { get; set; }

        public int MaxIterationsNoChanges { get; set; }

        public int CurrentIterationNoChanges { get; set; }

        public int Shift { get; set; }

        public uint Flag { get; set; }

        public static object DeepObjectClone(object obj)
        {
            object objResult;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter  = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);

                memoryStream.Position = 0;
                objResult = binaryFormatter.Deserialize(memoryStream);
            }
            return objResult;
        }

        //==================== QAP Path Generating ================

        public double GetCoefficient()
        {
            int x10 = 12345;
            int x11 = 67890;
            int x12 = 13579;
            int x20 = 24680;
            int x21 = 98765;
            int x22 = 43210;
            const int m = 2147483647;
            const int m2 = 2145483479;
            const int a12 = 63308;
            const int q12 = 33921;
            const int r12 = 12979;
            const int a13 = -183326;
            const int q13 = 11714;
            const int r13 = 2883;
            const int a21 = 86098;
            const int q21 = 24919;
            const int r21 = 7417;
            const int a23 = -539608;
            const int q23 = 3976;
            const int r23 = 2071;
            const double coeff = 4.656612873077393e-10;

            int h = x10/q13;
            int p13 = (-1)*a13*(x10 - h*q13) - h*r13;
            h = x11/q12;
            int p12 = a12*(x11 - h*q12) - h*r12;

            if (p13 < 0)
            {
                p13 = p13 + m;
            }

            if (p12 < 0)
            {
                p12 = p12 + m;
            }

            x10 = x11;
            x11 = x12;
            x12 = p12 - p13;

            if (x12 < 0)
            {
                x12 = x12 + m;
            }

            h = x20/q23;
            int p23 = (-1)*a23*(x20 - h*q23) - h*r23;
            h = x22/q21;
            int p21 = a21*(x22 - h*q21) - h*r21;

            if (p23 < 0)
            {
                p23 = p23 + m2;
            }

            if (p21 < 0)
            {
                p21 = p21 + m2;
            }

            x20 = x21;
            x21 = x22;
            x22 = p21 - p23;

            if (x22 < 0)
            {
                x22 = x22 + m2;
            }

            if (x12 < x22)
            {
                h = x12 - x22 + m;
            }

            else h = x12 - x22;

            if (h == 0)
            {
                Console.WriteLine("h == 0\n");
                return 1.0;
            }

            return h*coeff;
        }

        public int GetPermutationalIndex(int low, int high)
        {
            int index = low + (int) ((high - low + 1)*GetCoefficient());
            return index;
        }

        public void GeneratePath(int[] path)
        {
            for (int i = 0; i < Ants.Count; i++)
            {
                path[i] = i;
            }

            for (int i = 0; i < Ants.Count - 1; i++)
            {
                int x = GetPermutationalIndex(i, Ants.Count - 1);
                int y = path[i];
                path[i] = path[x];
                path[x] = y;
            }
        }
        
        //=========================================================

        public void CreateAnts()
        {
            for (int index = 0; index < Graph.Nodes.Count; index++)
            {
                Ants.Add(new Ant { VisitedNodes = new List<INode>() });

                for (int i = 0; i < Graph.Nodes.Count; i++)
                {
                    Ants.Last().VisitedNodes.Add(new Node());
                }
            }
        }

        public IList<INode> AntsTravel()
        {
            IList<INode> path = Ants.Select(ant => new Node()).Cast<INode>().ToList();

            int[] nextI = new int[Ants.Count];
            int[] nextJ = new int[Ants.Count];
            int[] sumTrace = new int[Ants.Count];

            GeneratePath(nextI);
            GeneratePath(nextJ);

            for (int i = 0; i < Ants.Count; i++)
            {
                for (int j = 0; j < Ants.Count; j++)
                {
                    sumTrace[i] += (int) Graph.Edges.Single(edge => edge.Begin == i && edge.End == j).Pheromone;
                }
            }

            for (int i = 0; i < Ants.Count; i++)
            {
                Shift = i;
                
                path[nextI[i]] = (Node) GetNextNode(new object[] {i, nextI, nextJ, sumTrace});

                for (int k = i; k < Ants.Count; k++)
                {
                    sumTrace[nextI[k]] -= (int) Graph.Edges.Single(o => o.Begin == nextI[k] && o.End == nextJ[Shift]).Pheromone;
                }

                int y = nextJ[Shift];
                nextJ[Shift] = nextJ[i];
                nextJ[i] = y;
            }
            return path;
        }
        
        public INode GetNextNode(object obj)
        {
            object[] objs = obj as object[];
            int index = 0;

            if (objs != null)
            {
                int i = int.Parse(objs[0].ToString());            
                int[] nextI = objs[1] as int[];
                int[] nextJ = objs[2] as int[];
                int[] sumTrace = objs[3] as int[];

                if (sumTrace != null && nextI != null && nextJ != null)
                {
                    int target = GetPermutationalIndex(0, sumTrace[nextI[i]] - 1);

                    double pheromone = Graph.Edges.Single(o => o.Begin == nextI[i] && o.End == nextJ[Shift]).Pheromone;

                    while (pheromone < target)
                    {
                        Shift++;
                        pheromone += Graph.Edges.Single(o => o.Begin == nextI[i] && o.End == nextJ[Shift]).Pheromone;
                    }
                    index = nextJ[Shift];
                }
            }
            return new Node {Id = index};
        }
    
        public void LocalSearch(IAnt ant)
        {
            // set of moves, numbered from 0 to index
            int[] move = new int[Ants.Count * (Ants.Count - 1) / 2];
            int nMoves = 0;

            for (int i = 0; i < Ants.Count - 1; i++)
            {
                for (int j = i + 1; j < Ants.Count; j++)
                {
                    move[nMoves++] = Ants.Count * i + j;
                }
            }

            bool isImproved = true;

            for (int scan = 0; scan < 2 && isImproved; scan++)
            {
                isImproved = false;

                for (int i = 0; i < nMoves - 1; i++)
                {
                    int x = GetPermutationalIndex(i + 1, nMoves - 1);
                    int y = move[i];
                    move[i] = move[x];
                    move[x] = y;
                }

                for (int i = 0; i < nMoves; i++)
                {
                    int r = move[i] / Ants.Count;
                    int s = move[i] % Ants.Count;
                    int moveCost = ComputeMoveCost(r, s, ant.VisitedNodes);

                    if (moveCost < 0)
                    {
                        ((Ant) ant).PathCost += moveCost;

                        int y = ant.VisitedNodes[r].Id;
                        ant.VisitedNodes[r].Id = ant.VisitedNodes[s].Id;
                        ant.VisitedNodes[s].Id = y;

                        isImproved = true;
                    }
                } 
            }
        }

        public int ComputeMoveCost(int r, int s, IList<INode> path)
        {
            int d =  (int) ((Graph.Edges.Single(e => e.Begin == r && e.End == r).HeuristicInformation
                              - 
                              Graph.Edges.Single(e => e.Begin == s && e.End == s).HeuristicInformation) 
                             *
                             (((Graph)Graph).FlowMatrix[path[s].Id, path[s].Id]
                             -
                             ((Graph)Graph).FlowMatrix[path[r].Id, path[r].Id])
                            +
                            ((Graph.Edges.Single(e => e.Begin == r && e.End == s).HeuristicInformation
                             -
                             Graph.Edges.Single(e => e.Begin == s && e.End == r).HeuristicInformation)
                            *
                            (((Graph)Graph).FlowMatrix[path[s].Id, path[r].Id]
                             -
                             ((Graph)Graph).FlowMatrix[path[r].Id, path[s].Id])));

            for (int k = 0; k < Ants.Count; k++)
            {
                if (k != r && k != s)
                {
                    d += (int) ((Graph.Edges.Single(e => e.Begin == k && e.End == r).HeuristicInformation
                                  -
                                  Graph.Edges.Single(e => e.Begin == k && e.End == s).HeuristicInformation)
                                 *
                                 (((Graph)Graph).FlowMatrix[path[k].Id, path[s].Id]
                                 -
                                 ((Graph)Graph).FlowMatrix[path[k].Id, path[r].Id])
                                +
                                ((Graph.Edges.Single(e => e.Begin == r && e.End == k).HeuristicInformation
                                 - Graph.Edges.Single(e => e.Begin == s && e.End == k).HeuristicInformation)
                                *
                                (((Graph)Graph).FlowMatrix[path[s].Id, path[k].Id]
                                -
                                ((Graph)Graph).FlowMatrix[path[r].Id, path[k].Id])));
                }
            }
            return d;
        }

        public int ComputePathCost(IAnt ant)
        {
            int cost = 0;

            for (int i = 0; i < ant.VisitedNodes.Count; i++)
            {
                for (int j = 0; j < ant.VisitedNodes.Count; j++)
                {
                    cost += (int) Graph.Edges.Single(
                        edge => edge.Begin == i &&
                                edge.End == j).HeuristicInformation*
                            ((Graph) Graph).FlowMatrix[ant.VisitedNodes[i].Id, ant.VisitedNodes[j].Id];
                }
            }
            ((Ant) ant).PathCost = cost;
            return cost;
        }

        public bool IsNewBestPath(IAnt ant)
        {
            int bestOne = ((Ant)BestAnt).PathCost;
            int newOne = ((Ant)ant).PathCost;

            if (newOne < bestOne)
            {
                Flag = 2;
                BestAnt.VisitedNodes = (IList<INode>)DeepObjectClone(ant.VisitedNodes);
                ((Ant)BestAnt).PathCost = ((Ant)ant).PathCost;

                Console.WriteLine(Result());

                return true;
            }
            if (newOne == bestOne)
            {
                Flag = 1;
            }
            else
            {
                Flag = 0;
            }
            return false;
        }

        public void UpdateGeneration()
        {
            if (Flag == 2)
            {
                Pheromone = 1;
                foreach (IEdge e in Graph.Edges)
                {
                    e.Pheromone = Pheromone;
                }
            }
            if (Flag == 1)
            {
                Pheromone++;
                foreach (IEdge e in Graph.Edges)
                {
                    e.Pheromone = Pheromone;
                }
            }
            if (Flag == 0)
            {
                for (int i = 1; i < BestAnt.VisitedNodes.Count; i++)
                {
                    Graph.Edges.Single(
                        edge => edge.Begin == i && edge.End == CurrentAnt.VisitedNodes[i].Id).Pheromone += Pheromone;
                    Graph.Edges.Single(
                        edge => edge.Begin == i && edge.End == BestAnt.VisitedNodes[i].Id).Pheromone += ExtraPheromone;
                }
            }
        }
      
        public bool IsFinished()
        {
            if (CurrentIteration == MaxIterations)
            {
                Console.WriteLine("End");
                return true;
            }
            if (CurrentIterationNoChanges == MaxIterationsNoChanges)
            {
                Console.WriteLine("End");
                return true;
            }
            return false;
        }

        public void Run()
        {
            Console.WriteLine("Start\n");

            while (!IsFinished())
            {
                CreateAnts();

                IList<INode> newPath = AntsTravel();

                CurrentAnt = new Ant {VisitedNodes = newPath, PathCost = 0};
                ComputePathCost(CurrentAnt);

                LocalSearch(CurrentAnt);
                
                if (!IsNewBestPath(CurrentAnt))
                {
                    CurrentIterationNoChanges++;
                }
                else CurrentIterationNoChanges = 0;

                UpdateGeneration();
                
                CurrentIteration++;
                
                Ants.Clear();
            }
        }

        public string Result()
        {
            StringBuilder result = new StringBuilder();
            result.Append(String.Format("----------------------\nCost: {0}", ((Ant)BestAnt).PathCost));
           
            for (int i = 0; i < BestAnt.VisitedNodes.Count - 1; i++)
            {
                result.Append(String.Format("\nIn location: {0} - ", i + 1)).Append(String.Format("item: {0}", BestAnt.VisitedNodes[i].Id + 1));
            }
            result.Append(String.Format("\nIn location: {0} - ", BestAnt.VisitedNodes.Count)).Append(String.Format("item: {0}", BestAnt.VisitedNodes.Last().Id + 1));

            result.Append(String.Format("\nIteration: {0}\n----------------------", CurrentIteration + 1)).Append(Environment.NewLine);
            
            return result.ToString();
        }
    }
}