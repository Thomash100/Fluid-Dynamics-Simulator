# Projektzusammenfassung

Stand: 2026-06-19

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

Die Issue-Bereinigung nach PR #18 wurde am 2026-06-19 durchgefuehrt und in `docs/ISSUE_REVIEW_2026-06.md` dokumentiert.

Geschlossene Alt-Issues:

- #2 Repository-Grundstruktur erstellen
- #3 Solution und Projekte anlegen
- #4 Fluid-Klasse entwickeln
- #5 Netzwerkmodell entwickeln
- #6 Hydraulik-Solver entwickeln
- #7 Pumpenmodell entwickeln
- #8 Armaturenmodell entwickeln
- #10 Ergebnisvisualisierung entwickeln
- #13 Beispielprojekte erstellen
- #14 Iterative hydraulic network solver

Bewusst offene Roadmap-Issues:

- #1 Rollout SYSTEMMEDIA-DevOps Standard
- #9 Thermisches Modell entwickeln
- #11 IFC-Schnittstelle entwickeln
- #12 Revit-Schnittstelle entwickeln

Neue Folge-Issues fuer `v0.3.0-hydraulics`:

| Issue | Titel | Zweck |
| --- | --- | --- |
| #19 | Solver-Validierungsnetze und Referenzfaelle ergaenzen | Naechster fachlicher Fokus fuer validierte hydraulische Referenznetze. |
| #20 | Hydraulischen Betriebspunkt fuer einfache Strangnetze validieren | Betriebspunktbegriff klaeren, ohne allgemeinen Netzsolver. |
| #21 | Pumpenkennlinien und Betriebspunktlogik spezifizieren | Pumpen-Folgearbeit fachlich vorbereiten. |
| #22 | Ventil-/Armaturenkennwerte fuer spaetere Regulierlogik vorbereiten | Armaturen- und Ventil-Folgearbeit fachlich vorbereiten. |
| #23 | Ergebnisvisualisierung fuer Solver- und Referenzfaelle vorbereiten | Ergebnisdarstellung vorbereiten, ohne produktive UI. |
| #24 | Beispielnetze und Beispielprojekte fuer v0.3.0-hydraulics definieren | Beispielnetze und Testdaten fuer die Hydraulikphase strukturieren. |

## Naechster sinnvoller fachlicher Schritt

Naechster fachlicher Codex-Auftrag: #19 `Solver-Validierungsnetze und Referenzfaelle ergaenzen`.

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
