using LaserCad.Core.Kerf;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Kerf;

public sealed class KerfCalibrationCalculatorTests
{
    [Test]
    public void CalculateRecommendedKerf_WithExactMeasurement_ShouldReturnLabeledKerf()
    {
        var options = new KerfCalibrationOptions(
            baseKerf: Length.FromMillimeters(0.1),
            kerfStep: Length.FromMillimeters(0.02),
            slotWidth: Length.FromMillimeters(20.0));

        var kerf = KerfCalibrationCalculator.CalculateRecommendedKerf(
            options,
            slotIndex: 2,
            measuredSlotWidth: Length.FromMillimeters(20.0));

        Assert.That(kerf.Millimeters, Is.EqualTo(0.14).Within(1e-9));
    }

    [Test]
    public void CalculateRecommendedKerf_WithNarrowMeasurement_ShouldIncreaseRecommendation()
    {
        var options = new KerfCalibrationOptions(
            baseKerf: Length.FromMillimeters(0.1),
            kerfStep: Length.FromMillimeters(0.02),
            slotWidth: Length.FromMillimeters(20.0));

        var kerf = KerfCalibrationCalculator.CalculateRecommendedKerf(
            options,
            slotIndex: 2,
            measuredSlotWidth: Length.FromMillimeters(19.95));

        Assert.That(kerf.Millimeters, Is.EqualTo(0.19).Within(1e-9));
    }

    [Test]
    public void CalculateRecommendedKerf_WithWideMeasurement_ShouldDecreaseRecommendation()
    {
        var options = new KerfCalibrationOptions(
            baseKerf: Length.FromMillimeters(0.1),
            kerfStep: Length.FromMillimeters(0.02),
            slotWidth: Length.FromMillimeters(20.0));

        var kerf = KerfCalibrationCalculator.CalculateRecommendedKerf(
            options,
            slotIndex: 2,
            measuredSlotWidth: Length.FromMillimeters(20.05));

        Assert.That(kerf.Millimeters, Is.EqualTo(0.09).Within(1e-9));
    }

    [Test]
    public void CalculateRecommendedKerf_WithMeasuredCalibrationSample_ShouldReturnExpectedKerf()
    {
        var options = new KerfCalibrationOptions(
            baseKerf: Length.FromMillimeters(0.1),
            kerfStep: Length.FromMillimeters(0.02),
            slotWidth: Length.FromMillimeters(20.0));

        var kerf = KerfCalibrationCalculator.CalculateRecommendedKerf(
            options,
            slotIndex: 3,
            measuredSlotWidth: Length.FromMillimeters(19.98));

        Assert.That(kerf.Millimeters, Is.EqualTo(0.18).Within(1e-9));
    }
}
