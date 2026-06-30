# Technische Schulden und Roadmap-Abgleich

Stand: 2026-06-18

## Grundsatz

Diese Datei dokumentiert offene Punkte und Issue-Empfehlungen. Sie schliesst keine GitHub-Issues automatisch.

## Aktuelle technische Schulden

| Bereich | Punkt | Empfehlung |
| --- | --- | --- |
| Issue-Hygiene | Fruehe Issues #2 bis #8 bilden teilweise bereits erledigte Arbeiten ab. | Nach Merge dieses PR manuell pruefen und erledigte Issues schliessen oder mit Abschlusskommentar versehen. |
| Hydrauliksolver | `SmallHydraulicNetworkSolver` ist ein Referenzsolver, kein allgemeiner Netzwerksolver. | Issue #14 nicht pauschal schliessen, sondern in Referenzsolver-Abschluss und allgemeinen Solver-Folgeauftrag trennen. |
| Validierung | Referenzfaelle sind noch klein und fachlich synthetisch. | Mehr validierte hydraulische Referenznetze mit erwarteten Residuen, Druckverlusten und Grenzfaellen anlegen. |
| Pumpen | Pumpenkennlinie und Leistung sind vorhanden, aber keine Betriebspunktbestimmung. | Separates Issue fuer Betriebspunkt und Pumpenauswahl schneiden. |
| Armaturen | Zeta und Kv/Kvs sind vorbereitet, aber keine Regelventil-Auslegung. | Separates Issue fuer Ventilautoritaet und Regelventil-Design. |
| Windows-App | App ist Test-Harness und keine produktive UI. | Produktive GUI erst nach Solver-Stabilisierung entscheiden. |
| Projektkommunikation | `FDS` kann mit Fire Dynamics Simulator verwechselt werden. | Extern immer `Fluid Dynamics Simulator` verwenden. |
| Plattform | .NET 8 ist Basis, .NET 10 steht spaeter an. | Migration nicht mit Solverentwicklung vermischen. |

## Offene Issues gegen aktuellen Stand

| Issue | Statusbewertung | Empfehlung |
| --- | --- | --- |
| #1 Rollout SYSTEMMEDIA-DevOps Standard | teilweise adressiert | Dieser PR deckt Projektregeln, Templates, CI-Formatcheck und Governance ab. Nach Merge manuell pruefen, ob weitere DevOps-Standards fehlen. |
| #2 Repository-Grundstruktur erstellen | vermutlich erledigt | Nach Merge manuell schliessen, falls keine Project-Board-Pflicht offen ist. |
| #3 Solution und Projekte anlegen | vermutlich erledigt | Solution enthaelt Core, Hydraulics, Tests und Windows-Test-Harness. |
| #4 Fluid-Klasse entwickeln | erledigt | `Fluid` ist in `FDS.Core` implementiert und getestet. |
| #5 Netzwerkmodell entwickeln | erledigt | `Network`, `Node` und `Edge` sind implementiert und getestet. |
| #6 Hydraulik-Solver entwickeln | zu breit | In kleinere Issues teilen: Komponentenstand abgeschlossen, Referenzsolver vorhanden, allgemeiner Solver offen. |
| #7 Pumpenmodell entwickeln | Basis erledigt | Grundmodell ist vorhanden. Betriebspunkt, Auswahl und Regelung separat schneiden. |
| #8 Armaturenmodell entwickeln | Basis erledigt | LocalResistance, Fitting, Valve und Kv/Kvs sind vorhanden. Regelventil-Auslegung separat schneiden. |
| #9 Thermisches Modell entwickeln | offen | Noch nicht starten, bis Hydraulik-Validierung stabiler ist oder Scope explizit gesetzt wird. |
| #10 Ergebnisvisualisierung entwickeln | teilweise offen | Windows-App ist nur Test-Harness. Produktive Visualisierung separat planen. |
| #11 IFC-Schnittstelle entwickeln | offen | Noch nicht starten. Vorher JSON-/Domänenmodell stabilisieren. |
| #12 Revit-Schnittstelle entwickeln | offen | Noch nicht starten. Abhaengig von IFC-/Austauschmodell. |
| #13 Beispielprojekte erstellen | teilweise offen | Smoke-Test und Samples existieren. Fachliche Beispielnetze und Testdaten separat ausbauen. |
| #14 Iterative hydraulic network solver | teilweise erledigt | Referenzsolver ist gemergt. Allgemeiner iterativer Netzwerksolver bleibt offen oder sollte neu geschnitten werden. |

## Naechster fachlicher Solver-Schritt

Empfohlen wird kein sofortiger allgemeiner Newton-, Hardy-Cross- oder Gradient-Solver. Der naechste fachliche Schritt sollte sein:

1. Mehr kleine hydraulische Referenznetze definieren.
2. Erwartete Knotenbilanz- und Druckresiduen dokumentieren.
3. Grenzfaelle fuer ungueltige Eingaben, nicht konvergierende Faelle und Pumpendruckvorgaben absichern.
4. Erst danach eine zweite Solvervariante entwerfen.

