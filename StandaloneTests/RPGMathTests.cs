using NUnit.Framework;

namespace StandaloneTests
{
    /// <summary>
    /// Unit tests for RPGMath utility class.
    /// These tests run with standard .NET NUnit, no Unity required.
    /// </summary>
    [TestFixture]
    public class RPGMathTests
    {
        [Test]
        public void CalculateDamageWithBonus_BasicTest_ReturnsCorrectValue()
        {
            // Arrange
            float baseDamage = 100f;
            float bonusPercent = 20f;
            float expected = 120f;

            // Act
            float actual = RPGMath.CalculateDamageWithBonus(baseDamage, bonusPercent);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Damage calculation should include 20% bonus");
        }

        [Test]
        public void CalculateDamageWithBonus_ZeroDamage_ReturnsZero()
        {
            // Arrange
            float baseDamage = 0f;
            float bonusPercent = 50f;
            float expected = 0f;

            // Act
            float actual = RPGMath.CalculateDamageWithBonus(baseDamage, bonusPercent);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Zero damage should remain zero");
        }

        [Test]
        public void CalculateDamageWithBonus_NegativeDamage_ReturnsZero()
        {
            // Arrange
            float baseDamage = -50f;
            float bonusPercent = 25f;
            float expected = 0f;

            // Act
            float actual = RPGMath.CalculateDamageWithBonus(baseDamage, bonusPercent);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Negative damage should return zero");
        }

        [Test]
        public void CalculatePercentage_BasicTest_ReturnsCorrectPercentage()
        {
            // Arrange
            float current = 75f;
            float maximum = 100f;
            float expected = 75f;

            // Act
            float actual = RPGMath.CalculatePercentage(current, maximum);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Should return 75%");
        }

        [Test]
        public void CalculatePercentage_ZeroMaximum_ReturnsZero()
        {
            // Arrange
            float current = 50f;
            float maximum = 0f;
            float expected = 0f;

            // Act
            float actual = RPGMath.CalculatePercentage(current, maximum);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Zero maximum should return zero percentage");
        }

        [Test]
        public void ApplyArmorMitigation_BasicTest_ReducesDamage()
        {
            // Arrange
            float incomingDamage = 100f;
            float armorValue = 100f;
            float expected = 50f; // 100 armor should reduce damage by 50%

            // Act
            float actual = RPGMath.ApplyArmorMitigation(incomingDamage, armorValue);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "100 armor should reduce 100 damage to 50");
        }

        [Test]
        public void ApplyArmorMitigation_ZeroArmor_ReturnOriginalDamage()
        {
            // Arrange
            float incomingDamage = 100f;
            float armorValue = 0f;
            float expected = 100f;

            // Act
            float actual = RPGMath.ApplyArmorMitigation(incomingDamage, armorValue);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Zero armor should not reduce damage");
        }

        [Test]
        public void CalculateCriticalDamage_DefaultMultiplier_DoubleseDamage()
        {
            // Arrange
            float baseDamage = 50f;
            float expected = 100f;

            // Act
            float actual = RPGMath.CalculateCriticalDamage(baseDamage);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Default critical should double damage");
        }

        [Test]
        public void CalculateCriticalDamage_CustomMultiplier_AppliesCorrectly()
        {
            // Arrange
            float baseDamage = 40f;
            float criticalMultiplier = 2.5f;
            float expected = 100f;

            // Act
            float actual = RPGMath.CalculateCriticalDamage(baseDamage, criticalMultiplier);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Should apply custom critical multiplier");
        }

        [Test]
        public void Clamp_ValueWithinRange_ReturnsOriginalValue()
        {
            // Arrange
            float value = 50f;
            float min = 0f;
            float max = 100f;
            float expected = 50f;

            // Act
            float actual = RPGMath.Clamp(value, min, max);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Value within range should be unchanged");
        }

        [Test]
        public void Clamp_ValueBelowMin_ReturnsMin()
        {
            // Arrange
            float value = -10f;
            float min = 0f;
            float max = 100f;
            float expected = 0f;

            // Act
            float actual = RPGMath.Clamp(value, min, max);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Value below min should be clamped to min");
        }

        [Test]
        public void Clamp_ValueAboveMax_ReturnsMax()
        {
            // Arrange
            float value = 150f;
            float min = 0f;
            float max = 100f;
            float expected = 100f;

            // Act
            float actual = RPGMath.Clamp(value, min, max);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f), "Value above max should be clamped to max");
        }
    }
}
