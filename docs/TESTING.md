# Testing

Stand: 2026-06-18

## Lokale Standardpruefung

```bash
dotnet restore FluidDynamicsSimulator.sln
dotnet build FluidDynamicsSimulator.sln --configuration Release
dotnet test FluidDynamicsSimulator.sln --configuration Release
dotnet format FluidDynamicsSimulator.sln --verify-no-changes --verbosity minimal
dotnet run --project samples/FDS.WindowsApp/FDS.WindowsApp.csproj --configuration Release -- --smoke-test
```

## Testschichten

| Schicht | Ort | Zweck |
| --- | --- | --- |
| Core Unit Tests | `tests/FDS.Core.Tests` | Basismodelle, Einheiten, Topologievalidierung. |
| Hydraulics Unit Tests | `tests/FDS.Hydraulics.Tests` | Rohr, Widerstand, Ventil, Pumpe, Strang, Netzwerk, Solver-Residuals. |
| Windows-App-Smoke-Test | `samples/FDS.WindowsApp -- --smoke-test` | Startbarkeit und Ergebnisweg des Test-Harness ohne manuelle UI-Interaktion. |
| CI | `.github/workflows/ci-cd.yml` | Restore, Build, Test, Formatcheck, Windows-App-Smoke-Test, Release bei Tags. |

## Akzeptanzregeln fuer Solverarbeiten

- Jede neue Berechnungsregel braucht Unit Tests mit erwarteten Werten.
- Iterative Solver muessen Status, Iterationszahl, finale Residuen und Iterationsverlauf ausgeben.
- Konvergenz darf nicht nur ueber Iterationsende behauptet werden.
- `MaxIterationsReached` und `InvalidInput` muessen explizit testbar sein.
- Referenzwerte sind in SI-Einheiten zu dokumentieren.

## Akzeptanzregeln fuer Dokumentations- und Governance-Arbeiten

- Keine Aenderung an Solvererwartungswerten.
- Keine neuen produktiven Features.
- `dotnet format --verify-no-changes` muss lokal und in CI laufen.
- Diff muss frei von Build-Artefakten bleiben.

