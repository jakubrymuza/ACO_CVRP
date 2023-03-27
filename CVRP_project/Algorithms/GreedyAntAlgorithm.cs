using CVRP_project.Structures;

namespace CVRP_project.Algorithms
{
    internal class GreedyAntAlgorithm : BasicAntAlgorithm, IAlgorithm
    {

        public GreedyAntAlgorithm(ProblemInstance cities, int iterationsCount, int antsCount, double beta, double overLimitPenaltyFactor) : base(cities, iterationsCount, antsCount, 0, beta, 1, 1, overLimitPenaltyFactor)
        { }

        public override String Name() => "Greedy Ant Algorithm";


        protected override double CalculateProbability(int lastCity, int city) => Math.Pow(1.0 / Cities.GetDistance(lastCity, city), Beta);
    }
}
