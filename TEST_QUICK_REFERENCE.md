# 🧪 Unity Testing Quick Reference

## 🚀 Quick Commands

### Command Line (Recommended)
```bash
# Run Entity_Stats tests only (fast, batch mode)
./run_tests.sh entity-stats

# Run all tests (batch mode)
./run_tests.sh all

# Interactive mode (opens Unity Editor) - Use if batch mode fails
./run_tests.sh interactive

# Diagnose issues with Unity testing setup
./run_tests.sh diagnose

# View latest results
./run_tests.sh results

# Clean output files
./run_tests.sh clean

# Check Unity installation
./run_tests.sh unity-info
```

### VS Code Tasks
- `Cmd+Shift+P` → "Tasks: Run Task" → Select:
  - **Unity: Run Entity Stats Tests** (batch mode)
  - **Unity: Run All Tests** (batch mode)  
  - **Unity: Run Tests (Interactive)** (opens Unity)
  - **Unity: View Test Results** (show XML)

### Unity Editor Menus
- `Tools > Testing > Run Entity Stats Tests`
- `Tools > Testing > Run All Tests`
- `Tools > Testing > Open Test Runner Window`

## 📁 Test Files Structure

```
Assets/Scripts/
├── Tests.asmdef                          # Test assembly config
├── Entity_StatsTests.cs                  # Main test suite (26 tests)
├── TestFrameworkVerificationTests.cs     # Setup verification (5 tests)
└── Editor/
    └── TestRunnerUtility.cs              # Menu utilities
```

## 🎯 Test Coverage

| System | Methods | Tests | Status |
|--------|---------|-------|--------|
| Physical Damage | `GetPhysicalDamage()` | 6 tests | ✅ Complete |
| Elemental Damage | `GetElementalDamage()` | 4 tests | ✅ Complete |
| Elemental Resistance | `GetElementalResistance()` | 4 tests | ✅ Complete |
| Armor Mitigation | `GetArmorMitigation()` | 6 tests | ✅ Complete |
| Health & Evasion | `GetMaxHealth()`, `GetEvasion()` | 3 tests | ✅ Complete |
| Armor Penetration | `GetArmorPenetration()` | 3 tests | ✅ Complete |

## 🔍 Test Results Locations

```
BraveryRPG/
├── TestResults/
│   ├── EntityStats_Results.xml
│   └── AllTests_Results.xml
└── TestLogs/
    ├── EntityStats_Log.txt
    └── AllTests_Log.txt
```

## ⚡ Quick Troubleshooting

| Issue | Solution |
|-------|----------|
| "Unity not found" | Script auto-detects Unity. Set `UNITY_PATH` if needed |
| "Assembly not found" | Check `Tests.asmdef` configuration |
| Tests won't run | Run `./run_tests.sh diagnose` for detailed analysis |
| Certificate errors | Use `./run_tests.sh interactive` instead of batch mode |
| Unity Editor running | Close Unity Editor before running batch tests |
| Permission errors | Run `chmod +x run_tests.sh` |
| Package manager issues | Try interactive mode or check network connectivity |

## 🧪 Adding New Tests

### Two Testing Modes

**Batch Mode (Faster)**
- Runs tests without opening Unity Editor GUI
- Faster execution, good for CI/CD
- May fail with package manager or certificate issues
- Use: `./run_tests.sh entity-stats`

**Interactive Mode (More Reliable)**
- Opens Unity Editor with Test Runner window
- Visual feedback and debugging capabilities
- Works around package manager issues
- Use: `./run_tests.sh interactive`

### Adding Test Methods

1. **Edit** `Entity_StatsTests.cs`
2. **Add method** with `[Test]` attribute
3. **Use format**: `Method_Scenario_ExpectedBehavior`
4. **Follow AAA**: Arrange → Act → Assert
5. **Use helper**: `SetStatValue(stat, value)`

```csharp
[Test]
public void GetNewStat_WithCondition_ReturnsExpected()
{
    // Arrange
    SetStatValue(entityStats.someStat, 50f);
    
    // Act  
    float result = entityStats.GetNewStat();
    
    // Assert
    Assert.AreEqual(75f, result, 0.01f, "Should include bonus");
}
```

## 📊 Expected Test Results

**Entity_Stats Tests**: 26 tests should pass  
**Framework Verification**: 5 tests should pass  
**Total**: 31 tests passing with 0 failures

### If Batch Mode Fails
- Certificate/package manager errors are common
- Use interactive mode: `./run_tests.sh interactive`
- Run diagnostics: `./run_tests.sh diagnose`
- Check that Unity Editor is closed before batch testing

---
*💡 Pro Tips:*
- *Use `./run_tests.sh diagnose` to troubleshoot issues*
- *Interactive mode is more reliable but slower*
- *Batch mode is faster but may fail with network/certificate issues*
