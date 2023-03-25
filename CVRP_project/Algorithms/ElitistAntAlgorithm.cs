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
            int lastCity = BaseStation;

            foreach (int city in LocalBestRoute)
            {
                double newPheromone = Cities.GetPheromone(lastCity, city) + (PheromoneStrength / LocalBestRouteLength);
                newPheromone *= ElitistAntsCount;

                Cities.SetPheromone(lastCity, city, newPheromone);
                Cities.SetPheromone(city, lastCity, newPheromone);
            }
        }
    }
}
