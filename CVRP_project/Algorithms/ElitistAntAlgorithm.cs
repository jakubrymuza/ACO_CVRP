using CVRP_project.Structures;

namespace CVRP_project.Algorithms
{
    internal class ElitistAntAlgorithm : BasicAntAlgorithm, IAlgorithm
    {
        private int ElitistAntsCount;
        public ElitistAntAlgorithm(ProblemInstance cities, int iterationsCount, int antsCount, double alpha, double beta, double evaporation, double pheromoneStrength, double overLimitPenaltyFactor, int elitistAntsCount) : base(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, overLimitPenaltyFactor)
        {
            ElitistAntsCount = elitistAntsCount;
        }

        public override String Name() => "Elitist Ant Algorithm";

        protected override void PostAddNewPheromone()
        {
            var route = GlobalBestRoute;

            for (int i = 1; i < route.Count; ++i)
            {
                int lastCity = route[i - 1];
                int city = route[i];

                double newPheromone = ElitistAntsCount * (PheromoneStrength / GlobalBestRouteLength);

                if (lastCity == BaseStation || city == BaseStation)
                {
                    newPheromone = 0;
                }

                PheromoneMatrix[lastCity, city] += newPheromone;
                PheromoneMatrix[city, lastCity] += newPheromone;
            }
        }
    }
}
