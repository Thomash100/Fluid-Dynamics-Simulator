# Issue Review 2026-06

Stand: 2026-06-19

Basis:

- `main` nach Merge von PR #18.
- Merge-Commit: `651d36c Merge PR #18 project governance and architecture docs`.
- Governance-Dokumente, Architekturleitplanken, Issue-Templates, PR-Template, .NET-8-SDK-Pin, EditorConfig, Directory.Build.props und CI-Formatcheck sind auf `main`.
- Es wurden keine Issues automatisch geschlossen.

## Bewertungsgrundsatz

Diese Datei ist eine Entscheidungsvorlage. Sie unterscheidet zwischen bereits umgesetztem Stand, weiter offenem Scope und empfohlenem Zuschnitt fuer Folge-Issues.

Issue-Schliessungen wurden bewusst nicht automatisch ausgefuehrt, obwohl GitHub API-Zugriff vorhanden ist. Grund: Mehrere Alt-Issues sind mit Milestones, Roadmap und teils breiten Titeln verbunden. Die endgueltige Bereinigung sollte manuell erfolgen, damit Milestones und Folge-Issues konsistent bleiben.

## Issue-Bewertung

| Issue | Titel | Aktueller Umsetzungsstand | Empfehlung | Begruendung | Moegliches Folge-Issue |
| --- | --- | --- | --- | --- | --- |
| #1 | Rollout: SYSTEMMEDIA-DevOps Standard fuer Fluid-Dynamics-Simulator uebernehmen | Teilweise umgesetzt durch PR #18: Governance-Dokumente, Templates, Formatcheck und Architekturleitplanken sind vorhanden. | Offen lassen oder nach manueller Checkliste schliessen. | Der genaue SYSTEMMEDIA-DevOps-Standard ist nicht vollstaendig als Akzeptanzkriterium im Repository abgebildet. | DevOps-Standard gegen externe Checkliste final abgleichen. |
| #2 | Repository-Grundstruktur erstellen | Erledigt. Repository-Struktur, README, Docs, CI, Templates und Governance-Grundlagen sind vorhanden. | Schliessen. | Der urspruengliche Basisumfang ist auf `main` umgesetzt. | Kein Folge-Issue erforderlich; Project-Board separat klaeren, falls weiterhin gewuenscht. |
| #3 | Solution und Projekte anlegen | Erledigt. `FluidDynamicsSimulator.sln` enthaelt `FDS.Core`, `FDS.Hydraulics`, Tests und Windows-Test-Harness. | Schliessen. | Solution- und Projektgrundlage ist vorhanden und CI-geprueft. | Neue Fachmodule jeweils als eigene Issues schneiden. |
| #4 | Fluid-Klasse entwickeln | Erledigt. `Fluid` ist in `FDS.Core` implementiert und getestet. | Schliessen. | Basismodell und Validierungen sind vorhanden. | Spaetere Stoffdatenbank oder temperaturabhaengige Eigenschaften separat schneiden. |
| #5 | Netzwerkmodell entwickeln | Erledigt. `Node`, `Edge`, `Network` und Topologievalidierungen sind implementiert und getestet. | Schliessen. | Core-Netzwerkmodell ist vorhanden. | Erweiterte Graph-/Topologieanalyse separat schneiden. |
| #6 | Hydraulik-Solver entwickeln | Zu breit und teilweise umgesetzt. Komponenten, Strang, feste Netzwerkauswertung, Residuen und kleiner Referenzsolver existieren; allgemeiner Netzwerksolver fehlt bewusst. | Neu schneiden. | Ein einziges Issue vermischt Komponentenmodelle, Referenzsolver und allgemeinen Solver. | Solver-Validierungsnetze und Referenzfaelle ergaenzen; spaeter allgemeine Solvervariante spezifizieren. |
| #7 | Pumpenmodell entwickeln | Basis erledigt. `Pump`, `PumpCurve`, Interpolation, Druckerhoehung und Leistung sind implementiert. | Schliessen und Folge-Issue anlegen. | Das Grundmodell ist erledigt; Betriebspunktlogik ist bewusst nicht enthalten. | Pumpenkennlinien und Betriebspunktlogik spezifizieren. |
| #8 | Armaturenmodell entwickeln | Basis erledigt. `LocalResistance`, `Fitting`, `Valve`, Zeta und Kv/Kvs sind implementiert. | Schliessen und Folge-Issue anlegen. | Grundmodelle sind erledigt; Regulierlogik und Ventilauslegung fehlen bewusst. | Ventil-/Armaturenkennwerte fuer spaetere Regulierlogik vorbereiten. |
| #9 | Thermisches Modell entwickeln | Offen. Es existiert noch kein `FDS.Thermal`. | Offen lassen. | Thermik ist ein spaeteres Modul und nicht Teil des aktuellen Hydraulik-/Governance-Scopes. | FDS.Thermal Grundmodell schneiden, wenn Hydraulik-Validierung stabil ist. |
| #10 | Ergebnisvisualisierung entwickeln | Teilweise offen. Die WinForms-App ist nur Test-Harness und keine produktive Visualisierung. | Neu schneiden oder offen lassen. | Der bestehende Sample deckt nur lokale Solverpruefung ab. | Ergebnisvisualisierung fuer Referenzfaelle spezifizieren. |
| #11 | IFC-Schnittstelle entwickeln | Offen. Es existiert noch kein `FDS.Ifc`. | Offen lassen. | BIM-/IFC-Anbindung ist bewusst nicht gestartet. | IFC-Domaenenmodell und Importgrenzen vorbereiten. |
| #12 | Revit-Schnittstelle entwickeln | Offen. Es existiert noch kein `FDS.Revit`. | Offen lassen. | Revit-Anbindung haengt von stabilem Austauschmodell ab. | Revit-Adapter-Scope nach IFC-/JSON-Modell schneiden. |
| #13 | Beispielprojekte erstellen | Teilweise umgesetzt. `samples/FDS.WindowsApp` und Smoke-Test existieren; fachliche Beispielnetze fehlen. | Neu schneiden. | Das Sample ist ein Test-Harness, kein kuratierter Satz fachlicher Beispiele. | Solver-Validierungsnetze und Referenzfaelle ergaenzen. |
| #14 | Iterative hydraulic network solver | Teilweise umgesetzt. Der kleine Referenzsolver ist auf `main`; ein allgemeiner Netzwerksolver fehlt bewusst. | Offen lassen oder neu schneiden. | Der aktuelle Solver ist absichtlich begrenzt. Das Issue sollte nicht als allgemeiner Solverabschluss gewertet werden. | Allgemeiner hydraulischer Netzwerksolver nach Referenzfall-Validierung. |

## Empfohlene direkte Issue-Aktionen

Sicher schliessbar nach manueller Bestaetigung:

- #2 Repository-Grundstruktur erstellen
- #3 Solution und Projekte anlegen
- #4 Fluid-Klasse entwickeln
- #5 Netzwerkmodell entwickeln

Schliessbar mit Folge-Issue:

- #7 Pumpenmodell entwickeln
- #8 Armaturenmodell entwickeln

Neu schneiden statt pauschal schliessen:

- #6 Hydraulik-Solver entwickeln
- #10 Ergebnisvisualisierung entwickeln
- #13 Beispielprojekte erstellen
- #14 Iterative hydraulic network solver

Offen lassen:

- #1 DevOps-Standard, bis externe Kriterien final abgeglichen sind
- #9 Thermisches Modell entwickeln
- #11 IFC-Schnittstelle entwickeln
- #12 Revit-Schnittstelle entwickeln

## Vorgeschlagene Folge-Issues

1. Solver-Validierungsnetze und Referenzfaelle ergaenzen
   - Kleine Einstrang-, Parallelstrang- und Pumpendruck-Netze definieren.
   - Erwartete Knotenbilanz- und Druckresiduen dokumentieren.
   - Grenzfaelle fuer `InvalidInput` und `MaxIterationsReached` absichern.

2. Hydraulischen Betriebspunkt fuer einfache Strangnetze validieren
   - Betriebspunktbegriff fuer feste Druckdifferenz und feste Pumpendruckerhoehung klaeren.
   - Keine allgemeine Netziteration in diesem Schritt.

3. Pumpenkennlinien und Betriebspunktlogik spezifizieren
   - Erwartetes Verhalten bei Kennliniengrenzen, Interpolation und fehlender Konvergenz beschreiben.
   - Automatische Pumpenauswahl weiterhin ausschliessen.

4. Ventil-/Armaturenkennwerte fuer spaetere Regulierlogik vorbereiten
   - Zeta-, Kv- und Kvs-Konventionen konsolidieren.
   - Regelventil-Auslegung bewusst als spaeteren Schritt belassen.

5. Roadmap `v0.3.0-hydraulics` nach Governance-Review aktualisieren
   - Milestone-Zuordnung und Issue-Zuschnitt an den aktuellen Implementierungsstand anpassen.

## Naechster fachlicher Codex-Auftrag

Empfohlen:

```text
Repository: Thomash100/Fluid-Dynamics-Simulator

Aufgabe:
Ergaenze Solver-Validierungsnetze und Referenzfaelle fuer FDS.Hydraulics, ohne neue Solverlogik.

Scope:
- Kleine hydraulische Referenznetze als Testdaten oder Test-Helper definieren.
- Erwartete Knotenbilanz- und Druckresiduen dokumentieren.
- Tests fuer Einstrang, parallele Straenge, bekannte Druckdifferenz und feste Pumpendruckerhoehung ergaenzen.
- `InvalidInput` und `MaxIterationsReached` als Grenzfaelle absichern.
- Dokumentation aktualisieren.

Nicht umsetzen:
- kein Newton-Solver
- kein Hardy-Cross-Solver
- kein allgemeiner Netzwerksolver
- keine automatische Pumpenauswahl
- keine produktive UI
```
