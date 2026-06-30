# Codex Workflow

Stand: 2026-06-18

## Standardablauf

1. Arbeitsstand pruefen:

```bash
git status --short --branch
git fetch origin
```

2. Basisbranch klaeren:

- Ohne Sondervorgabe von `main` starten.
- Bei Folgearbeiten auf einem Feature-Branch nur dort weiterarbeiten, wenn der User den Branch nennt.

3. Branch erstellen:

```bash
git switch main
git pull --ff-only origin main
git switch -c feature/<scope>
```

4. Umfang begrenzen:

- Nur Dateien aendern, die zum Auftrag gehoeren.
- Keine versteckten Refactorings.
- Keine numerische Solverlogik in Governance- oder Dokumentationsaufgaben.

5. Verifizieren:

```bash
dotnet restore FluidDynamicsSimulator.sln
dotnet build FluidDynamicsSimulator.sln --configuration Release
dotnet test FluidDynamicsSimulator.sln --configuration Release
dotnet format FluidDynamicsSimulator.sln --verify-no-changes --verbosity minimal
dotnet run --project samples/FDS.WindowsApp/FDS.WindowsApp.csproj --configuration Release -- --smoke-test
```

6. Diff pruefen:

```bash
git status --short
git diff --stat
git diff --check
```

7. Commit und Push:

```bash
git add <dateien>
git commit -m "<englischer Commit-Titel>"
git push -u origin <branch>
```

8. PR oeffnen:

- Titel kurz und fachlich.
- Beschreibung auf Deutsch, wenn der Auftrag deutsch ist.
- Testergebnisse konkret nennen.
- Manuellen Stopppunkt nennen, falls Issues, Releases oder Merge bewusst offen bleiben.

## Manueller Stopppunkt

Stoppen, wenn:

- PR geoeffnet ist.
- Lokale Pruefungen erfolgreich sind oder Fehler dokumentiert sind.
- GitHub Actions gruen sind oder ein klarer CI-Fehler dokumentiert ist.
- Eine Entscheidung ueber Issue-Schliessungen, Release-Tags oder Merge erforderlich ist.

