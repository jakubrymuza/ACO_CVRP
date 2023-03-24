using CVRP_project.Structures;

namespace CVRP_project.Algorithms
{
    internal class RankedAntAlgorithm : BasicAntAlgorithm, IAlgorithm
    {
        private int RankingSize;

        public RankedAntAlgorithm(ProblemInstance cities, int iterationsCount, int antsCount, double alpha, double beta, double evaporation, double pheromoneStrength, int rankingSize) : base(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength)
        {
            RankingSize = rankingSize;
        }

        public new String Name() => "Greedy Ant Algorithm";

        protected new void AddNewPheromone()
        {
            // TODO
        }
    }
}
