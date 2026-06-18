# Architekturentscheidungen

Stand: 2026-06-18

## ADR-001: Modularer Monolith als .NET-Solution

Status: akzeptiert

Der aktuelle Architekturansatz ist ein modularer Monolith in einer gemeinsamen .NET-Solution. Die fachlichen Grenzen werden ueber Projekte und Namespaces gezogen, nicht ueber Services oder getrennte Repositories.

Begruendung:

- Der Solver- und Modellkern ist noch klein und stark gekoppelt.
- Tests, Referenzfaelle und Dokumentation lassen sich in einem Repository einfacher konsistent halten.
- Eine Service-Architektur wuerde aktuell mehr Integrationsaufwand als Nutzen erzeugen.

Konsequenz:

- Neue Fachmodule werden als Projekte in `src/` angelegt.
- Gemeinsame Testprojekte liegen in `tests/`.
- Beispiel- und Test-Harness-Projekte liegen in `samples/`.

## ADR-002: Fachbibliotheken als Projektgrenzen

Status: akzeptiert

Aktuelle und geplante Projektgrenzen:

- `FDS.Core`: Basismodelle, Topologie, Einheiten-Value-Objects.
- `FDS.Hydraulics`: Hydraulikmodelle, Komponentenberechnungen, Referenzsolver.
- `FDS.Thermal`: spaetere thermische Modelle.
- `FDS.Airflow`: spaetere lufttechnische Netzmodelle.
- `FDS.Ifc`: spaetere IFC-Anbindung.
- `FDS.Revit`: spaetere Revit-Anbindung.

## ADR-003: WinForms-App bleibt Test-Harness

Status: akzeptiert

`samples/FDS.WindowsApp` ist eine lokale Test-App fuer Referenzsolver und Ergebnisdarstellung. Sie ist keine produktive Benutzeroberflaeche.

Konsequenz:

- UI-Aenderungen duerfen keine Solverlogik veraendern.
- Die App darf Smoke-Tests, Vergleichsfaelle und Ergebnispruefung unterstuetzen.
- Produktive GUI-Architektur wird spaeter separat entschieden.

## ADR-004: .NET 8 als Basis, .NET 10 als vorgemerkter Migrationspunkt

Status: akzeptiert

Die Solution zielt aktuell auf .NET 8. Eine spaetere Migration auf .NET 10 wird als Architekturpunkt vorgemerkt, aber nicht vorgezogen.

Konsequenz:

- `global.json` pinnt ein .NET 8 SDK mit Roll-forward innerhalb der 8.0-Familie.
- CI nutzt weiterhin `8.0.x`.
- Migration auf .NET 10 braucht einen eigenen Branch, eigene CI-Pruefung und Release-Hinweis.

## ADR-005: Technische Namespaces bleiben `FDS.*`

Status: akzeptiert

Die bestehenden Namespaces bleiben technisch bestehen. Kommunikativ wird der volle Projektname `Fluid Dynamics Simulator` verwendet, um Verwechslung mit Fire Dynamics Simulator zu vermeiden.

Konsequenz:

- Keine Umbenennung bestehender Namespaces in diesem Schritt.
- README, PRs und Issues sollen den vollen Projektnamen bevorzugen.

