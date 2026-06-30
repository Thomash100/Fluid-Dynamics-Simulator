using FDS.Hydraulics.Models;

namespace FDS.WindowsApp;

internal static class SolverScenarioReview
{
    public static IReadOnlyList<string> CreateMessages(
        SolverScenarioParameters parameters,
        HydraulicSolverResult result)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(result);

        var messages = new List<string>();
        AddInputMessages(parameters, messages);
        AddResultMessages(result, messages);

        return messages.Count == 0
            ? ["Keine kritischen Hinweise."]
            : messages;
    }

    public static string CreateAssessment(HydraulicSolverResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.Status switch
        {
            HydraulicSolverStatus.Converged => "Referenzfall konvergiert",
            HydraulicSolverStatus.MaxIterationsReached => "Prüfung erforderlich",
            HydraulicSolverStatus.InvalidInput => "Ungültige Eingabe",
            _ => "Unbekannter Status",
        };
    }

    private static void AddInputMessages(SolverScenarioParameters parameters, List<string> messages)
    {
        if (parameters.TotalVolumeFlowRateCubicMetersPerSecond == 0
            && parameters.PressureDifferencePascals > 0)
        {
            messages.Add("Es ist Druckdifferenz gesetzt, aber kein Gesamtvolumenstrom. Das kann nur einen Nullflussfall prüfen.");
        }

        if (parameters.TotalVolumeFlowRateCubicMetersPerSecond > 0
            && parameters.PressureDifferencePascals == 0)
        {
            messages.Add("Es ist Volumenstrom gesetzt, aber keine Druckdifferenz. Druckresiduen werden voraussichtlich nicht erfüllt.");
        }

        if (parameters.BranchAZeta == 0 || parameters.BranchBZeta == 0)
        {
            messages.Add("Mindestens ein Strang hat Zeta 0. Lokale Widerstände sind dort deaktiviert.");
        }

        if (parameters.PipeInnerDiameterMeters < 0.02)
        {
            messages.Add("Der Rohrdurchmesser ist sehr klein. Erwartbar sind hohe Druckverluste oder Druckresiduen.");
        }
    }

    private static void AddResultMessages(HydraulicSolverResult result, List<string> messages)
    {
        if (result.Status == HydraulicSolverStatus.Converged)
        {
            messages.Add("Solverstatus ist konvergiert.");
        }
        else if (result.Status == HydraulicSolverStatus.MaxIterationsReached)
        {
            messages.Add("Die maximale Iterationszahl wurde erreicht. Eingaben und Residuen fachlich prüfen.");
        }
        else if (result.Status == HydraulicSolverStatus.InvalidInput)
        {
            messages.Add("Der Solver hat die Eingabe als ungültig bewertet.");
        }

        if (result.MaxNodeBalanceResidualCubicMetersPerSecond > 1e-8)
        {
            messages.Add("Das Knotenbilanz-Residuum liegt oberhalb der Referenztoleranz.");
        }

        if (result.MaxPressureResidualPascals > 0.25)
        {
            messages.Add("Das Druck-Residuum liegt oberhalb der Referenztoleranz.");
        }
    }
}
