using CVRP_project.Structures;

namespace CVRP_project.Algorithms
{
    internal class GreedyAntAlgorithm : BasicAntAlgorithm, IAlgorithm
    {
        // w algorytmie zachłannym wystarczy ustawić parametr alfa na 0, wówczas miasta będą wybierane zachłannie
        public GreedyAntAlgorithm(ProblemInstance cities, int iterationsCount, int antsCount) : base(cities, iterationsCount, antsCount, 0, 1, 0, 0)
        { }

        public override String Name() => "Greedy Ant Algorithm";
    }
}
