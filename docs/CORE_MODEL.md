# FDS.Core Modellgrundlage

Stand: 2026-06-10

`FDS.Core` enthält die ersten fachlichen Basismodelle für eine spätere hydraulische, thermische und BIM-basierte Simulation. Die Modelle bilden nur Datenstruktur und Validierung ab. Druckverlustberechnung, hydraulische Solver, Pumpenmodelle, UI sowie IFC- und Revit-Schnittstellen sind bewusst nicht im Core-Modul enthalten.

## Modelle

| Modell | Verantwortung |
| --- | --- |
| `Node` | Knoten eines technischen Netzwerks mit ID, optionalem Druck und optionaler Temperatur. |
| `Edge` | Gerichtete Verbindung zwischen zwei Knoten mit Länge, Durchmesser und optionalen Strömungsgrößen. |
| `Fluid` | Fluiddaten mit Dichte und optionaler Referenztemperatur. |
| `Network` | Validierte Sammlung aus Knoten, Kanten und optionalem Fluid. |
| `Temperature` | Explizite Trennung von Celsius-Werten und Kelvin-Werten. |

## Einheiten

| Größe | Einheit | Code |
| --- | --- | --- |
| Druck | Pa | `PressurePascals` |
| Temperatur | °C | `Temperature.Celsius` |
| Temperatur | K | `Temperature.Kelvin` |
| Volumenstrom | m³/s | `VolumetricFlowRateCubicMetersPerSecond` |
| Massenstrom | kg/s | `MassFlowRateKilogramsPerSecond` |
| Länge | m | `LengthMeters` |
| Durchmesser | m | `DiameterMeters` |
| Dichte | kg/m³ | `DensityKilogramsPerCubicMeter` |

## Validierungsregeln

- Leere IDs und Namen werden zurückgewiesen.
- Negative Dichte wird zurückgewiesen.
- Negative Kantenlänge wird zurückgewiesen.
- Durchmesser kleiner oder gleich 0 wird zurückgewiesen.
- Doppelte IDs innerhalb eines Netzwerks werden zurückgewiesen.
- Kanten mit unbekannten Start- oder Zielknoten werden zurückgewiesen.
- Temperaturen unter absolutem Nullpunkt werden zurückgewiesen.

## Testabdeckung

Das Testprojekt `FDS.Core.Tests` deckt die Basismodelle `Node`, `Edge`, `Fluid`, `Network` sowie das Temperatur-Value-Object ab.
