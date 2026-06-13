namespace FDS.Hydraulics.Models;

public enum HydraulicSolverStatus
{
    NotStarted,
    Prepared,
    Converged,
    MaxIterationsReached,
    NotConverged,
    InvalidInput
}
