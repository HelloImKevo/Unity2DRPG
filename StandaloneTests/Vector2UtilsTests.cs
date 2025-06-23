using NUnit.Framework;
using System;

namespace StandaloneTests
{
    /// <summary>
    /// Mock-friendly version of Vector2 for testing without Unity dependencies
    /// </summary>
    public struct MockVector2 : IEquatable<MockVector2>
    {
        public float x;
        public float y;

        public MockVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(MockVector2 other)
        {
            return Math.Abs(x - other.x) < 0.0001f && Math.Abs(y - other.y) < 0.0001f;
        }

        public override bool Equals(object obj)
        {
            return obj is MockVector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public static bool operator ==(MockVector2 left, MockVector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MockVector2 left, MockVector2 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

    /// <summary>
    /// Testable version of Vector2Utils using MockVector2 instead of Unity's Vector2
    /// </summary>
    public static class MockVector2Utils
    {
        public static MockVector2[] DeepCopy(MockVector2[] originalArray)
        {
            if (originalArray == null) return null;

            MockVector2[] copiedArray = new MockVector2[originalArray.Length];
            Array.Copy(originalArray, copiedArray, originalArray.Length);
            return copiedArray;
        }
    }

    /// <summary>
    /// Unit tests for Vector2Utils functionality using mocking approach.
    /// Tests the deep copy functionality without Unity dependencies.
    /// </summary>
    [TestFixture]
    public class Vector2UtilsTests
    {
        [Test]
        public void DeepCopy_WithValidArray_ReturnsIdenticalCopy()
        {
            // Arrange
            var originalArray = new MockVector2[]
            {
                new MockVector2(1.0f, 2.0f),
                new MockVector2(3.5f, 4.7f),
                new MockVector2(-1.2f, 0.0f)
            };

            // Act
            var copiedArray = MockVector2Utils.DeepCopy(originalArray);

            // Assert
            Assert.That(copiedArray, Is.Not.Null);
            Assert.That(copiedArray.Length, Is.EqualTo(originalArray.Length));
            
            for (int i = 0; i < originalArray.Length; i++)
            {
                Assert.That(copiedArray[i], Is.EqualTo(originalArray[i]), 
                    $"Element at index {i} should be equal");
            }
        }

        [Test]
        public void DeepCopy_WithValidArray_ReturnsNewArrayInstance()
        {
            // Arrange
            var originalArray = new MockVector2[]
            {
                new MockVector2(10.0f, 20.0f),
                new MockVector2(30.0f, 40.0f)
            };

            // Act
            var copiedArray = MockVector2Utils.DeepCopy(originalArray);

            // Assert
            Assert.That(copiedArray, Is.Not.SameAs(originalArray), 
                "Copied array should be a different object instance");
        }

        [Test]
        public void DeepCopy_WithValidArray_ModificationsDontAffectOriginal()
        {
            // Arrange
            var originalArray = new MockVector2[]
            {
                new MockVector2(5.0f, 6.0f),
                new MockVector2(7.0f, 8.0f)
            };
            var originalFirstElement = originalArray[0];

            // Act
            var copiedArray = MockVector2Utils.DeepCopy(originalArray);
            copiedArray[0] = new MockVector2(999.0f, 888.0f);

            // Assert
            Assert.That(originalArray[0], Is.EqualTo(originalFirstElement), 
                "Original array should not be affected by modifications to the copy");
            Assert.That(copiedArray[0], Is.EqualTo(new MockVector2(999.0f, 888.0f)), 
                "Copy should reflect the modification");
        }

        [Test]
        public void DeepCopy_WithNullArray_ReturnsNull()
        {
            // Arrange
            MockVector2[] originalArray = null;

            // Act
            var copiedArray = MockVector2Utils.DeepCopy(originalArray);

            // Assert
            Assert.That(copiedArray, Is.Null, "Copying null array should return null");
        }

        [Test]
        public void DeepCopy_WithEmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            var originalArray = new MockVector2[0];

            // Act
            var copiedArray = MockVector2Utils.DeepCopy(originalArray);

            // Assert
            Assert.That(copiedArray, Is.Not.Null);
            Assert.That(copiedArray.Length, Is.EqualTo(0), "Empty array copy should also be empty");
            Assert.That(copiedArray, Is.Not.SameAs(originalArray), "Should be a different instance");
        }

        [Test]
        public void DeepCopy_WithSingleElementArray_CopiesCorrectly()
        {
            // Arrange
            var originalArray = new MockVector2[] { new MockVector2(42.5f, -17.3f) };

            // Act
            var copiedArray = MockVector2Utils.DeepCopy(originalArray);

            // Assert
            Assert.That(copiedArray.Length, Is.EqualTo(1));
            Assert.That(copiedArray[0], Is.EqualTo(originalArray[0]));
            Assert.That(copiedArray, Is.Not.SameAs(originalArray));
        }

        [Test]
        public void DeepCopy_WithLargeArray_PerformanceTest()
        {
            // Arrange
            const int arraySize = 10000;
            var originalArray = new MockVector2[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                originalArray[i] = new MockVector2(i * 0.1f, i * 0.2f);
            }

            // Act
            var startTime = DateTime.Now;
            var copiedArray = MockVector2Utils.DeepCopy(originalArray);
            var endTime = DateTime.Now;

            // Assert
            Assert.That(copiedArray.Length, Is.EqualTo(arraySize));
            Assert.That((endTime - startTime).TotalMilliseconds, Is.LessThan(100), 
                "Large array copy should complete within 100ms");
        }
    }
}
