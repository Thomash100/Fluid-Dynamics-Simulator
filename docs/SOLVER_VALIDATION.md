# Solvervalidierung

Stand: 2026-06-18

## Aktueller Solverstatus

`FDS.Hydraulics` enthaelt:

- Einzelrohrberechnungen.
- Zeta- und Kv/Kvs-basierte Einzelwiderstaende.
- Pumpenkennlinien und Pumpenleistung.
- Strangberechnung bei bekanntem Volumenstrom.
- Feste Netzwerkauswertung bei bekannten Strangvolumenstroemen.
- Residualvorbereitung fuer Knotenbilanzen und Druckgleichungen.
- Einen kleinen iterativen Referenzsolver fuer vorbereitete Netze.

Der vorhandene Solver ist kein allgemeiner hydraulischer Netzwerksolver.

## Residualkonventionen

Knotenbilanz:

```text
Q_res = Q_in + Q_source - Q_out - Q_sink
```

Druckresidual:

```text
dp_res = dp_available - dp_required
```

Konvergenz wird nur akzeptiert, wenn Flow-Residual und Druck-Residual innerhalb der Toleranzen aus `HydraulicSolverOptions` liegen.

## Validierungsfaelle

Vor einem allgemeinen Solver sollten diese Faelle stabil dokumentiert und getestet sein:

| Fall | Erwartung |
| --- | --- |
| Einstrangnetz mit bekannter Druckdifferenz | Konvergenter Volumenstrom und Druckresidual nahe 0. |
| Zwei parallele Straenge | Knotenbilanz nahe 0 und nachvollziehbare Verteilung. |
| Feste Pumpendruckerhoehung | Druckangebot wird gegen Strangverluste geprueft. |
| Zu kleine Iterationszahl | Status `MaxIterationsReached`. |
| Ungueltige Eingabe | Status `InvalidInput` ohne verdeckte Ausnahme. |
| Nullfluss-Grenzfall | Kein Division-by-zero und nachvollziehbarer Status. |

## Grenzen

- Kein Newton-Verfahren.
- Kein Hardy-Cross-Verfahren als allgemeiner Solver.
- Kein Gradient-Solver.
- Keine automatische Pumpenauswahl.
- Keine Regelstrategie.
- Keine thermische Kopplung.
- Keine IFC-/BIM-Anbindung.

