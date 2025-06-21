using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Simple verification tests to ensure the Unity Test Framework is working correctly.
/// These tests validate basic functionality and can be used to troubleshoot testing setup.
/// </summary>
public class TestFrameworkVerificationTests
{
    [Test]
    public void TestFramework_IsWorking_PassesSimpleAssertion()
    {
        // Arrange
        int expected = 42;
        int actual = 42;

        // Act & Assert
        Assert.AreEqual(expected, actual, "Basic equality assertion should pass");
    }

    [Test]
    public void TestFramework_CanCreateGameObjects_CreatesSuccessfully()
    {
        // Arrange & Act
        GameObject testObject = new GameObject("TestObject");

        // Assert
        Assert.IsNotNull(testObject, "Should be able to create GameObjects in tests");
        Assert.AreEqual("TestObject", testObject.name, "GameObject should have correct name");

        // Cleanup
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void TestFramework_CanAddComponents_AddsSuccessfully()
    {
        // Arrange
        GameObject testObject = new GameObject("TestWithComponent");

        // Act
        Entity_Stats stats = testObject.AddComponent<Entity_Stats>();

        // Assert
        Assert.IsNotNull(stats, "Should be able to add Entity_Stats component");
        Assert.IsTrue(testObject.TryGetComponent<Entity_Stats>(out var component), 
            "Should be able to retrieve added component");
        Assert.AreSame(stats, component, "Retrieved component should be the same instance");

        // Cleanup
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void TestFramework_FloatComparison_WorksWithDelta()
    {
        // Arrange
        float expected = 1.0f / 3.0f;  // 0.333...
        float actual = 0.333333f;

        // Act & Assert
        Assert.AreEqual(expected, actual, 0.001f, "Float comparison should work with appropriate delta");
    }

    [Test]
    public void TestFramework_CanTestEnums_WorksCorrectly()
    {
        // Arrange
        ElementType expectedElement = ElementType.Fire;

        // Act
        ElementType actualElement = ElementType.Fire;

        // Assert
        Assert.AreEqual(expectedElement, actualElement, "Enum comparison should work correctly");
        Assert.AreNotEqual(ElementType.None, actualElement, "Different enum values should not be equal");
    }
}
