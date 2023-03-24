using CVRP_project.Structures;

namespace CVRP_project.Algorithms
{
    internal class ElitistAntAlgorithm : BasicAntAlgorithm, IAlgorithm
    {
        private int ElitistAntsCount;
        public ElitistAntAlgorithm(ProblemInstance cities, int iterationsCount, int antsCount, double alpha, double beta, double evaporation, double pheromoneStrength, int elitistAntsCount) : base(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength)
        {
            ElitistAntsCount = elitistAntsCount;
        }

        public new String Name() => "Elitist Ant Algorithm";

        protected new void PostAddNewPheromone()
        {
            // TODO
        }
    }
}
