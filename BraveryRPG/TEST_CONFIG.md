# Unity Test Configuration

## Test Execution Settings

### Unity Editor Path
# Update this path if you're using a different Unity version
UNITY_EDITOR_PATH="/Applications/Unity/Hub/Editor/2022.3.50f1/Unity.app/Contents/MacOS/Unity"

### Project Path
PROJECT_PATH="./BraveryRPG"

### Test Output Directories
TEST_RESULTS_DIR="./BraveryRPG/TestResults"
TEST_LOGS_DIR="./BraveryRPG/TestLogs"

## Available Test Filters

### Run all Entity_Stats tests
TEST_FILTER_ENTITY_STATS="Entity_StatsTests"

### Run all Edit Mode tests
TEST_FILTER_EDIT_MODE="EditMode"

### Run specific test methods (examples)
# TEST_FILTER_SPECIFIC="Entity_StatsTests.GetPhysicalDamage_WithBaseDamageOnly_ReturnsCorrectValue"
# TEST_FILTER_SPECIFIC="Entity_StatsTests.GetElementalDamage_WithMultipleElements_ReturnsHighestAsPrimaryWithBonuses"

## Test Categories (for future organization)
# [Test, Category("PhysicalDamage")]
# [Test, Category("ElementalDamage")]  
# [Test, Category("ArmorCalculations")]
# [Test, Category("StatBonuses")]

## Quick Test Commands

### Run Entity_Stats tests only
```bash
$UNITY_EDITOR_PATH -batchmode -quit \
  -projectPath $PROJECT_PATH \
  -runTests \
  -testPlatform EditMode \
  -testFilter $TEST_FILTER_ENTITY_STATS \
  -testResults "$TEST_RESULTS_DIR/EntityStats_Results.xml" \
  -logFile "$TEST_LOGS_DIR/EntityStats_Log.txt"
```

### Run all Edit Mode tests
```bash
$UNITY_EDITOR_PATH -batchmode -quit \
  -projectPath $PROJECT_PATH \
  -runTests \
  -testPlatform EditMode \
  -testResults "$TEST_RESULTS_DIR/AllTests_Results.xml" \
  -logFile "$TEST_LOGS_DIR/AllTests_Log.txt"
```

## Test Development Guidelines

### Naming Conventions
- Test Class: `[ComponentName]Tests` (e.g., `Entity_StatsTests`)
- Test Method: `[MethodName]_[Scenario]_[ExpectedBehavior]`
- Test Categories: Use logical groupings for related functionality

### Test Data Management
- Use realistic stat values in tests (avoid extreme edge cases unless testing limits)
- Create test data that represents actual gameplay scenarios
- Document any magic numbers or specific test values

### Assertion Guidelines
- Always include descriptive failure messages
- Use appropriate delta values for float comparisons (typically 0.01f)
- Test both positive and negative scenarios
- Verify all output parameters (like `out bool isCrit`)

### Performance Tips
- Keep test setup minimal - only initialize what's needed
- Reuse test fixtures when possible
- Avoid expensive operations in test setup
- Group related tests in the same test file
