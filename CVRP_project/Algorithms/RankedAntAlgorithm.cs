using CVRP_project.Structures;

namespace CVRP_project.Algorithms
{
    internal class RankedAntAlgorithm : ElitistAntAlgorithm, IAlgorithm
    {
        private int RankingSize;

        public RankedAntAlgorithm(ProblemInstance cities, int iterationsCount, int antsCount, double alpha, double beta, double evaporation, double pheromoneStrength, double overLimitPenaltyFactor, int elitistAntsCount, int rankingSize) : base(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, overLimitPenaltyFactor, elitistAntsCount)
        {
            RankingSize = rankingSize;
        }

        public override String Name() => "Ranked Ant Algorithm";

        protected override void AddNewPheromone()
        {
            var comparer = new AntSolutionComparer<Ant>();
            Array.Sort(Ants, comparer);

            for (int rank = 0; rank < RankingSize; rank++)
            {
                if (!Ants[rank].RouteFinished())
                    continue;

                double pheromoneFactor = 1.0;
                if (!Ants[rank].WithinTrucksLimit())
                {
                    pheromoneFactor = OverLimitPenaltyFactor;
                }

                var route = Ants[rank].GetRoute();

                for (int i = 1; i < route.Count; ++i)
                {
                    int lastCity = route[i - 1];
                    int city = route[i];

                    double newPheromone = pheromoneFactor * (RankingSize - rank) * (PheromoneStrength / Ants[rank].GetRouteLength());

                    if (lastCity == BaseStation || city == BaseStation)
                    {
                        newPheromone = 0;
                    }

                    PheromoneMatrix[lastCity, city] += newPheromone;
                    PheromoneMatrix[city, lastCity] += newPheromone;
                }
            }

            PostAddNewPheromone();
        }

        private class AntSolutionComparer<T> : IComparer<Ant>
        {
            public int Compare(Ant? x, Ant? y) => x!.GetRouteLength().CompareTo(y!.GetRouteLength());
        }
    }
}
