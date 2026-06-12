using FDS.Hydraulics.Models;

namespace FDS.WindowsApp;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Contains("--smoke-test", StringComparer.OrdinalIgnoreCase))
        {
            HydraulicSolverResult result = SolverScenarioRunner.RunParallelBranchScenario();
            Console.WriteLine(SolverScenarioRunner.FormatResult(result));

            int exitCode = result.Status == HydraulicSolverStatus.Converged ? 0 : 1;
            Environment.ExitCode = exitCode;
            return exitCode;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());

        return 0;
    }
}
