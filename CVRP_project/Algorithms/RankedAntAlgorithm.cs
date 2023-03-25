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

        public override String Name() => "Ranked Ant Algorithm";

        protected override void AddNewPheromone()
        {
            var comparer = new AntSolutionComparer<Ant>();
            Array.Sort(Ants, comparer);

            for (int rank = 0; rank < RankingSize; rank++)
            {
                // TODO: daj feromon tylko jak dobra trasa?

                int lastCity = BaseStation;

                foreach (int city in Ants[rank].GetRoute())
                {
                    double newPheromone = Cities.GetPheromone(lastCity, city) + (PheromoneStrength / Ants[rank].GetRouteLength());
                    newPheromone *= (RankingSize - rank);

                    Cities.SetPheromone(lastCity, city, newPheromone);
                    Cities.SetPheromone(city, lastCity, newPheromone);
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
