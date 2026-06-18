# Projektzusammenfassung

Stand: 2026-06-18

Repository: https://github.com/Thomash100/Fluid-Dynamics-Simulator

Aktueller Release: https://github.com/Thomash100/Fluid-Dynamics-Simulator/releases/tag/v0.1.0-alpha

## Projektziel

Fluid Dynamics Simulator ist eine Open-Source-Simulationsplattform fuer technische Gebaeudeausruestung, Hydraulik, Thermodynamik, Luftstroemung und spaetere BIM-basierte Analyse mit IFC- und Revit-Integration.

Der Kurzname `FDS` und die technischen Namespaces `FDS.*` bleiben vorerst bestehen. In README, Issues und PRs soll der volle Name `Fluid Dynamics Simulator` verwendet werden, um Verwechslungen mit Fire Dynamics Simulator zu vermeiden.

## Architekturstand

- Zielarchitektur ist ein modularer Monolith als .NET-Solution.
- Aktuelle Fachbibliotheken: `FDS.Core`, `FDS.Hydraulics`.
- Geplante Fachbibliotheken: `FDS.Thermal`, `FDS.Airflow`, `FDS.Ifc`, `FDS.Revit`.
- Keine Service-Architektur und keine Repository-Aufteilung in der aktuellen Phase.
- `samples/FDS.WindowsApp` ist ein lokales Test-Harness, keine produktive UI.
- .NET 8 ist die aktuelle Zielplattform; eine spaetere .NET-10-Migration ist vorgemerkt.

## Aktueller technischer Stand

- Repository ist oeffentlich.
- `version.json` steht auf `0.1.0-alpha`.
- CI baut Solution und Windows-App-Smoke-Test.
- Release-Erzeugung erfolgt bei Tags `v*`.
- `FDS.Core` enthaelt `Node`, `Edge`, `Fluid`, `Network` und `Temperature`.
- `FDS.Hydraulics` enthaelt Rohr-, Einzelwiderstands-, Ventil-, Pumpen-, Strang- und Netzwerkauswertungen.
- Ein erster kleiner iterativer Referenzsolver ist vorhanden.
- Der Referenzsolver dokumentiert Status, Iterationsverlauf, finale Fluesse, Knotenbilanzen und Druckresiduen.
- Die WinForms-App kann den Referenzsolver als Smoke-Test und lokales Test-Harness ausfuehren.

## Governance-Ergaenzungen

Dieser Stand ergaenzt:

- `AGENTS.md`
- `docs/PROJECT_RULES.md`
- `docs/CODEX_WORKFLOW.md`
- `docs/ARCHITECTURE_DECISIONS.md`
- `docs/TECHNICAL_DEBT.md`
- `docs/TESTING.md`
- `docs/RELEASE_PROCESS.md`
- `docs/SOLVER_VALIDATION.md`
- `docs/UNITS_AND_ASSUMPTIONS.md`
- `.github/pull_request_template.md`
- `.github/ISSUE_TEMPLATE/bug_report.yml`
- `.github/ISSUE_TEMPLATE/feature_request.yml`
- `.github/ISSUE_TEMPLATE/codex_task.yml`
- `global.json`
- `.editorconfig`
- `Directory.Build.props`

## Build- und CI-Standard

Lokale Standardpruefung:

```bash
dotnet restore FluidDynamicsSimulator.sln
dotnet build FluidDynamicsSimulator.sln --configuration Release
dotnet test FluidDynamicsSimulator.sln --configuration Release
dotnet format FluidDynamicsSimulator.sln --verify-no-changes --verbosity minimal
dotnet run --project samples/FDS.WindowsApp/FDS.WindowsApp.csproj --configuration Release -- --smoke-test
```

GitHub Actions enthalten Restore, Build, Test, Formatcheck, Windows-App-Smoke-Test, Artefakterzeugung und Release-Erzeugung bei Tags.

## Issue- und Roadmap-Abgleich

Die offenen GitHub-Issues wurden gegen den aktuellen Stand geprueft. Es wurden keine Issues automatisch geschlossen.

| Issue | Bewertung | Empfehlung |
| --- | --- | --- |
| #1 Rollout SYSTEMMEDIA-DevOps Standard | teilweise adressiert | Nach Merge dieses PR manuell pruefen. |
| #2 Repository-Grundstruktur erstellen | vermutlich erledigt | Manuell schliessen, falls kein Project-Board-Punkt offen bleibt. |
| #3 Solution und Projekte anlegen | vermutlich erledigt | Manuell schliessen. |
| #4 Fluid-Klasse entwickeln | erledigt | Manuell schliessen. |
| #5 Netzwerkmodell entwickeln | erledigt | Manuell schliessen. |
| #6 Hydraulik-Solver entwickeln | zu breit | In Komponentenstand, Referenzsolver und allgemeinen Solver aufteilen. |
| #7 Pumpenmodell entwickeln | Basis erledigt | Betriebspunkt und Pumpenauswahl separat schneiden. |
| #8 Armaturenmodell entwickeln | Basis erledigt | Regelventil-Auslegung separat schneiden. |
| #9 Thermisches Modell entwickeln | offen | Spaeterer eigener Scope. |
| #10 Ergebnisvisualisierung entwickeln | teilweise offen | Windows-App ist nur Test-Harness, produktive Visualisierung separat planen. |
| #11 IFC-Schnittstelle entwickeln | offen | Noch nicht starten. |
| #12 Revit-Schnittstelle entwickeln | offen | Noch nicht starten. |
| #13 Beispielprojekte erstellen | teilweise offen | Fachliche Beispielnetze und Testdaten separat ausbauen. |
| #14 Iterative hydraulic network solver | teilweise erledigt | Referenzsolver ist vorhanden; allgemeiner Netzwerksolver bleibt offen oder wird neu geschnitten. |

## Naechster sinnvoller fachlicher Schritt

Vor einem allgemeinen Newton-, Hardy-Cross- oder Gradient-Solver sollten weitere hydraulische Referenznetze mit erwarteten Residuen und Grenzfaellen dokumentiert und getestet werden.

## Nicht enthalten

- Keine neue Solverlogik.
- Kein Newton-, Hardy-Cross- oder Gradient-Solver.
- Keine automatische Pumpenauswahl.
- Keine thermische Kopplung.
- Keine IFC-/Revit-Anbindung.
- Keine produktive GUI.
- Keine Namespace-Umbenennung.
- Keine Repository-Aufteilung.
