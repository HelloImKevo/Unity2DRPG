using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using System.Collections.Generic;

/// <summary>
/// Unity Editor utility for running unit tests from the menu or scripts.
/// This provides easy access to test execution without opening the Test Runner window.
/// </summary>
public static class TestRunnerUtility
{
    /// <summary>
    /// Runs all unit tests in the project.
    /// Accessible via Unity menu: Tools/Run All Tests
    /// </summary>
    [MenuItem("Tools/Testing/Run All Tests")]
    public static void RunAllTests()
    {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.EditMode | TestMode.PlayMode
        };
        
        testRunnerApi.Execute(new ExecutionSettings(filter));
        Debug.Log("Running all tests...");
    }
    
    /// <summary>
    /// Runs only Edit Mode tests (unit tests that don't require play mode).
    /// Accessible via Unity menu: Tools/Run Edit Mode Tests
    /// </summary>
    [MenuItem("Tools/Testing/Run Edit Mode Tests")]
    public static void RunEditModeTests()
    {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.EditMode
        };
        
        testRunnerApi.Execute(new ExecutionSettings(filter));
        Debug.Log("Running Edit Mode tests...");
    }
    
    /// <summary>
    /// Runs only tests for Entity_Stats component.
    /// Accessible via Unity menu: Tools/Run Entity Stats Tests
    /// </summary>
    [MenuItem("Tools/Testing/Run Entity Stats Tests")]
    public static void RunEntityStatsTests()
    {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.EditMode,
            testNames = new[] { "Entity_StatsTests" }
        };
        
        testRunnerApi.Execute(new ExecutionSettings(filter));
        Debug.Log("Running Entity_Stats tests...");
    }
    
    /// <summary>
    /// Opens the Test Runner window for manual test execution and monitoring.
    /// Accessible via Unity menu: Tools/Open Test Runner
    /// </summary>
    [MenuItem("Tools/Testing/Open Test Runner Window")]
    public static void OpenTestRunner()
    {
        EditorWindow.GetWindow(typeof(UnityEditor.TestTools.TestRunner.TestRunnerWindow));
    }
}
