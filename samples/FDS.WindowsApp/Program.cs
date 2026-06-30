using FDS.Hydraulics.Models;

namespace FDS.WindowsApp;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Contains("--smoke-test", StringComparer.OrdinalIgnoreCase))
        {
            TryConfigureConsoleOutputEncoding();

            SolverScenarioParameters parameters = SolverScenarioParameters.Default;
            HydraulicSolverResult result = SolverScenarioRunner.RunParallelBranchScenario(parameters);
            Console.WriteLine(SolverScenarioRunner.FormatResult(result, parameters));
            Console.WriteLine();
            Console.WriteLine(SolverScenarioRunner.FormatPresetComparison());

            int exitCode = result.Status == HydraulicSolverStatus.Converged ? 0 : 1;
            Environment.ExitCode = exitCode;
            return exitCode;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());

        return 0;
    }

    private static void TryConfigureConsoleOutputEncoding()
    {
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch (IOException)
        {
            // WinExe smoke tests can run without a normal console handle.
        }
    }
}
