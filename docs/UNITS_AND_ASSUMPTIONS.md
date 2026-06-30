# Einheiten und Annahmen

Stand: 2026-06-18

## Einheitensystem

Intern werden SI-Einheiten verwendet.

| Groesse | Einheit | Hinweis |
| --- | --- | --- |
| Druck | Pa | Druckverluste und Pumpendruckerhoehungen. |
| Temperatur | degC und K | Engineering-/Anzeige-Werte in degC, absolute Werte in K. |
| Volumenstrom | m3/s | Edge-, Branch- und Solver-Fluesse. |
| Massenstrom | kg/s | Optionales Core-Feld. |
| Laenge | m | Kanten- und Rohrlaengen. |
| Durchmesser | m | Kanten- und Rohrinnendurchmesser. |
| Dichte | kg/m3 | Fluidmodell und hydraulische Druckverluste. |
| Dynamische Viskositaet | Pa*s | Reynoldszahl. |
| Geschwindigkeit | m/s | Aus Volumenstrom und Querschnitt. |
| Reynoldszahl | dimensionslos | Stroemungskennzahl. |
| Reibungszahl | dimensionslos | Darcy-Reibungszahl. |
| Zeta-Wert | dimensionslos | Einzelwiderstand. |
| Kv/Kvs | m3/h | Metrische Ventilkonvention. |
| Foerderhoehe | m | Pumpenkennlinie und optionale Ausgabe. |
| Leistung | W | Hydraulische Leistung und Wellenleistung. |
| Residual Knotenbilanz | m3/s | Flow-Residual. |
| Residual Druck | Pa | Druckangebot minus Druckbedarf. |

## Temperaturannahmen

- Celsius-Werte sind fuer Eingabe, Anzeige und gebaeudetechnische Interpretation geeignet.
- Kelvin-Werte werden verwendet, wenn absolute thermodynamische Werte benoetigt werden.
- Kelvin unter 0 ist ungueltig.

## Hydraulische Annahmen

- Aktuelle Hydraulikbausteine behandeln stationaere Faelle.
- Fluide werden fuer die vorhandenen Druckverlustrechnungen mit konstanter Dichte betrachtet.
- Negative Laengen, negative Dichten, negative Rauheiten und Durchmesser kleiner oder gleich 0 sind ungueltig.
- Strang- und Netzwerkauswertungen arbeiten aktuell mit nichtnegativen Volumenstroemen.
- Der kleine Referenzsolver ist fuer vorbereitete kleine Netze gedacht und nicht fuer allgemeine Netze freigegeben.

## Druck- und Pumpenannahmen

- Rohr- und Einzelwiderstandsverluste werden als positive Verlustgroessen betrachtet.
- Pumpendruckerhoehung wird als positives Druckangebot betrachtet.
- Die Strangbilanz lautet:

```text
dp_net = dp_pump - dp_pipe - dp_local
```

- Die optionale Foerderhoehe wird berechnet als:

```text
H = dp / (rho * g)
```

