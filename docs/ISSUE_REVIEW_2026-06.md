# Issue Review 2026-06

Stand: 2026-06-19

Basis:

- PR #18 wurde nach `main` gemergt.
- Merge-Commit: `651d36c Merge PR #18 project governance and architecture docs`.
- Dokumentationscommit: `8ec1178 Add governance issue review`.
- Issue-Bereinigung wurde am 2026-06-19 ueber GitHub Issues durchgefuehrt.
- Es wurden keine fachlichen Solverfunktionen umgesetzt.

## Ergebnis

Die Alt-Issues #1 bis #14 wurden gegen den aktuellen `main`-Stand und die Governance-Dokumentation bewertet.

- Geschlossen als erledigt: #2, #3, #4, #5.
- Geschlossen mit engerem Folge-Issue: #6, #7, #8, #10, #13, #14.
- Bewusst offen gelassen: #1, #9, #11, #12.
- Neue Folge-Issues angelegt: #19, #20, #21, #22, #23, #24.

## Geschlossene Alt-Issues

| Issue | Titel | Aktion | Begruendung | Folge-Issue |
| --- | --- | --- | --- | --- |
| #2 | Repository-Grundstruktur erstellen | Geschlossen | Repository-Grundstruktur, Dokumentation, CI, Templates und Governance-Grundlagen sind auf `main`. | Kein Folge-Issue |
| #3 | Solution und Projekte anlegen | Geschlossen | Solution enthaelt Core, Hydraulics, Tests und Windows-Test-Harness. | Neue Fachmodule bei Bedarf separat |
| #4 | Fluid-Klasse entwickeln | Geschlossen | `Fluid` ist in `FDS.Core` implementiert und getestet. | Spaetere Stoffdaten separat |
| #5 | Netzwerkmodell entwickeln | Geschlossen | `Node`, `Edge`, `Network` und Topologievalidierung sind implementiert und getestet. | Erweiterte Topologieanalyse separat |
| #6 | Hydraulik-Solver entwickeln | Geschlossen | Breiter Alt-Scope wurde in kleinere Folge-Issues geschnitten. | #19, #20 |
| #7 | Pumpenmodell entwickeln | Geschlossen | Pumpen-Basismodell ist vorhanden; weiterfuehrende Betriebspunktlogik wurde separiert. | #21 |
| #8 | Armaturenmodell entwickeln | Geschlossen | LocalResistance, Fitting, Valve, Zeta und Kv/Kvs sind vorhanden; Regulierlogik wurde separiert. | #22 |
| #10 | Ergebnisvisualisierung entwickeln | Geschlossen | Breiter Visualisierungs-Scope wurde auf Solver- und Referenzfaelle eingegrenzt. | #23 |
| #13 | Beispielprojekte erstellen | Geschlossen | Breiter Beispiel-Scope wurde auf Hydraulik-Beispielnetze und Referenzfaelle geschnitten. | #19, #24 |
| #14 | Iterative hydraulic network solver | Geschlossen | Der kleine Referenzsolver ist vorhanden; weitere Arbeit wurde auf Validierung und Betriebspunktdefinition eingegrenzt. | #19, #20 |

Alle geschlossenen Issues wurden vor dem Schliessen kommentiert.

## Bewusst offene Issues

| Issue | Titel | Status | Begruendung |
| --- | --- | --- | --- |
| #1 | Rollout: SYSTEMMEDIA-DevOps Standard fuer Fluid-Dynamics-Simulator uebernehmen | Offen | Bleibt offen, bis eine konkrete externe DevOps-Checkliste final abgeglichen ist. |
| #9 | Thermisches Modell entwickeln | Offen | `FDS.Thermal` ist ein spaeterer Roadmap-Punkt. |
| #11 | IFC-Schnittstelle entwickeln | Offen | IFC/BIM-Anbindung ist ein spaeterer Roadmap-Punkt. |
| #12 | Revit-Schnittstelle entwickeln | Offen | Revit-Anbindung haengt von einem stabilen Austauschmodell ab. |

Diese Issues wurden kommentiert und bewusst offen gelassen.

## Neue Folge-Issues

| Issue | Titel | Milestone | Zweck |
| --- | --- | --- | --- |
| #19 | Solver-Validierungsnetze und Referenzfaelle ergaenzen | `v0.3.0-hydraulics` | Naechster fachlicher Fokus fuer validierte Referenznetze, Residuen und Grenzfaelle. |
| #20 | Hydraulischen Betriebspunkt fuer einfache Strangnetze validieren | `v0.3.0-hydraulics` | Betriebspunktbegriff fuer einfache Strangnetze klaeren, ohne allgemeinen Netzsolver. |
| #21 | Pumpenkennlinien und Betriebspunktlogik spezifizieren | `v0.3.0-hydraulics` | Pumpenmodell fachlich fuer spaetere Betriebspunktlogik vorbereiten. |
| #22 | Ventil-/Armaturenkennwerte fuer spaetere Regulierlogik vorbereiten | `v0.3.0-hydraulics` | Zeta-, Kv- und Kvs-Konventionen fuer spaetere Regulierlogik konsolidieren. |
| #23 | Ergebnisvisualisierung fuer Solver- und Referenzfaelle vorbereiten | `v0.3.0-hydraulics` | Ergebnisdarstellung vorbereiten, ohne produktive UI einzufuehren. |
| #24 | Beispielnetze und Beispielprojekte fuer v0.3.0-hydraulics definieren | `v0.3.0-hydraulics` | Beispielnetze, Testdaten und Beispielprojekte fuer den Hydraulikstand definieren. |

## Aktuelle Roadmap-Lage

Der aktive fachliche Fokus fuer die naechste Entwicklungsphase liegt auf `v0.3.0-hydraulics`.

Prioritaet:

1. #19 Solver-Validierungsnetze und Referenzfaelle ergaenzen.
2. #20 Hydraulischen Betriebspunkt fuer einfache Strangnetze validieren.
3. #21 und #22 Pumpen- und Armaturen-Folgearbeit fachlich vorbereiten.
4. #23 und #24 Ergebnisdarstellung und Beispielnetze fuer die Hydraulikphase strukturieren.

## Naechster Codex-Auftrag

Empfohlen:

```text
Repository: Thomash100/Fluid-Dynamics-Simulator

Aufgabe:
Ergaenze Solver-Validierungsnetze und Referenzfaelle fuer FDS.Hydraulics, ohne neue Solverlogik.

Issue:
#19 Solver-Validierungsnetze und Referenzfaelle ergaenzen

Nicht umsetzen:
- kein Newton-Solver
- kein Hardy-Cross-Solver
- kein allgemeiner Netzwerksolver
- keine automatische Pumpenauswahl
- keine produktive UI
```
