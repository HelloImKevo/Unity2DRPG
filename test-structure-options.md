# Test Project Structure Options

## Current Structure (Option 1 - With .gitignore)
```
Unity2DRPG/
├── .gitignore                   # Now excludes build artifacts
├── StandaloneTests/             # Test project (artifacts ignored)
│   ├── *.cs                     # Test files (tracked)
│   ├── *.csproj                 # Project file (tracked)
│   ├── bin/                     # Build output (ignored)
│   └── obj/                     # Temp files (ignored)
```

## Option 2: Separate Test Directory Structure
```
Unity2DRPG/
├── src/                         # Main Unity project
│   └── BraveryRPG/
└── tests/                       # All test-related files
    ├── unit/
    │   ├── StandaloneTests.csproj
    │   └── *.cs
    ├── scripts/
    │   └── run_tests.sh
    ├── bin/                     # (ignored)
    └── obj/                     # (ignored)
```

## Option 3: External Test Repository
```
Unity2DRPG/                      # Clean Unity project
Unity2DRPG-Tests/                # Separate repository
├── StandaloneTests/
├── scripts/
└── docs/
```

## Option 4: In-Memory Testing (No Artifacts)
Use dotnet test --no-build or configure to use temp directories
```
