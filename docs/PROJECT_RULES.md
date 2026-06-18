# Projektregeln

Stand: 2026-06-18

## Ziel

Der Fluid Dynamics Simulator ist eine modulare .NET-Simulationsplattform fuer TGA, Hydraulik, Thermik, Luftstroemung und spaetere BIM-Integration.

Der Projektstandard soll weitere Codex- und GitHub-Aufgaben stabil machen: eindeutige Branches, klare Scope-Grenzen, reproduzierbare Tests und nachvollziehbare Architekturentscheidungen.

## Grundregeln

- `main` muss jederzeit baubar und testbar bleiben.
- Fachliche Aenderungen und Governance-Aenderungen werden getrennt.
- Keine neue Solverlogik ohne eigene Tests, Dokumentation und klare Abgrenzung.
- Keine produktive UI, solange die WinForms-App nur als Test-Harness dient.
- Keine Repository-Aufteilung und keine Service-Architektur in der aktuellen Phase.
- Offene Issues werden nicht automatisch geschlossen, wenn nur eine Roadmap-Analyse beauftragt ist.

## Naming und Kommunikation

- Technische Namespaces bleiben vorerst `FDS.*`.
- In README, PRs und Issues wird der vollstaendige Name `Fluid Dynamics Simulator` verwendet.
- Der Kurzname `FDS` ist intern akzeptiert, muss kommunikativ aber klar von Fire Dynamics Simulator getrennt bleiben.

## Branch- und PR-Regeln

- Feature-Branches folgen `feature/<kurzer-scope>`.
- PRs gehen standardmaessig gegen `main`, ausser ein Vorbereitungsbranch ist explizit Ziel.
- PR-Beschreibungen muessen Scope, Nicht-Scope und Testergebnisse enthalten.
- Draft-PRs sind sinnvoll, wenn fachliche Pruefung oder CI-Ergebnis noch offen ist.
- Merges nach `main` erfolgen erst nach gruenen Checks oder bewusst dokumentierter Ausnahme.

## Dokumentationspflicht

Bei fachlichen Aenderungen sind mindestens README oder passende Datei unter `docs/` zu pruefen. Bei Solverarbeiten muessen Residuen, Einheiten, Grenzen und Referenzfaelle dokumentiert werden.

