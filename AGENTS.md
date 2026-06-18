# Codex Arbeitsregeln

Diese Datei gilt fuer Arbeiten im Repository `Thomash100/Fluid-Dynamics-Simulator`.

## Projektgrenze

- Zielarchitektur ist aktuell ein modularer Monolith als .NET-Solution.
- Neue Fachmodule werden als Projekte unter `src/` angelegt, zum Beispiel `FDS.Thermal`, `FDS.Airflow`, `FDS.Ifc` und `FDS.Revit`.
- Es werden keine Services, keine getrennten Repositories und keine produktive UI eingefuehrt, solange kein expliziter Auftrag dazu vorliegt.
- `samples/FDS.WindowsApp` ist ein lokales Test-Harness fuer Solver- und Ergebnispruefung, keine produktive Anwendung.
- Technische Namespaces `FDS.*` bleiben vorerst bestehen. In Dokumentation und PRs ist der volle Name `Fluid Dynamics Simulator` zu verwenden, um Verwechslungen mit Fire Dynamics Simulator zu vermeiden.

## Arbeitsweise

1. Vor Aenderungen `git status --short --branch` pruefen.
2. Von `main` oder vom explizit genannten Basisbranch abzweigen.
3. Scope eng halten. Keine Solverlogik in Dokumentations-, Governance- oder CI-Aufgaben.
4. Bestehende Modelle und Tests nicht umbauen, wenn der Auftrag nur Projektstandard betrifft.
5. Offene Issues nicht automatisch schliessen, wenn der Auftrag nur Analyse und Empfehlung verlangt.

## Architekturleitplanken

- `FDS.Core` enthaelt gemeinsame Basismodelle, Einheiten-Value-Objects und Topologiegrundlagen.
- `FDS.Hydraulics` enthaelt Hydraulikmodelle, Einzelkomponentenberechnungen, feste Netzwerkauswertungen und kleine Referenzsolver.
- Solver muessen Residuen, Statuswerte, Eingaben, Optionen und Iterationsverlauf nachvollziehbar ausgeben.
- Neue numerische Verfahren brauchen eigene Tests mit Referenzfaellen und klare Dokumentation der Grenzen.
- Thermik, Luft, IFC und Revit werden erst als eigene Module begonnen, wenn der jeweilige Scope geschnitten ist.

## Einheiten

- Druck: Pa
- Temperatur: Anzeige/Engineering in degC, intern bei absoluter Temperatur K
- Volumenstrom: m3/s
- Massenstrom: kg/s
- Laenge und Durchmesser: m
- Dichte: kg/m3
- Dynamische Viskositaet: Pa*s

## Pflichtpruefungen

Vor Commit oder PR, soweit in der Umgebung moeglich:

```bash
dotnet restore FluidDynamicsSimulator.sln
dotnet build FluidDynamicsSimulator.sln --configuration Release
dotnet test FluidDynamicsSimulator.sln --configuration Release
dotnet format FluidDynamicsSimulator.sln --verify-no-changes --verbosity minimal
dotnet run --project samples/FDS.WindowsApp/FDS.WindowsApp.csproj --configuration Release -- --smoke-test
```

Wenn ein Check nicht ausgefuehrt werden kann, muss die Ursache im PR dokumentiert werden.

