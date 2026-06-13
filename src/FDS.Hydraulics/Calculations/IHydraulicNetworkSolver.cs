using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Calculations;

public interface IHydraulicNetworkSolver
{
    HydraulicSolverResult Solve(HydraulicSolverInput input);
}
