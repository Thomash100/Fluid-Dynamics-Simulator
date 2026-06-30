# Releaseprozess

Stand: 2026-06-18

## Versionsbasis

- Die Projektversion steht in `version.json`.
- Aktueller Release-Stand: `v0.1.0-alpha`.
- Pre-Releases enthalten einen Bindestrich im Tag, zum Beispiel `v0.1.0-alpha`.

## Standardablauf

1. Feature-Branch in `main` mergen, nachdem CI gruen ist.
2. `version.json` in einem separaten Release-PR aktualisieren, falls sich die Version aendert.
3. Tag setzen:

```bash
git tag v<version>
git push origin v<version>
```

4. GitHub Actions erzeugen bei Tags `v*` ein Release-Archiv.

## CI-Verhalten

Der Workflow `.github/workflows/ci-cd.yml` fuehrt aus:

- Restore
- Build
- Test
- Formatcheck
- Windows-App-Smoke-Test
- Artefakterzeugung
- Release-Erzeugung bei Tags
- optionales Raspberry-Pi-Deployment bei Tags, sofern Secrets gesetzt sind

## Abgrenzung

Dieser Governance-Schritt erzeugt keinen Release-Tag. Ein Release wird erst vorbereitet, wenn fachlicher Scope, Versionsnummer und Release Notes bewusst festgelegt sind.

