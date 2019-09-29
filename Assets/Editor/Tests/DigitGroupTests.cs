using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class DigitGroupTests
{
    [Test]
    [TestCase(1, 9)]
    [TestCase(2, 99)]
    [TestCase(3, 999)]
    public void DigitGroupMaxValueTest(int places, int expectedValue)
    {
        var dg = new DigitGroup(places);
        Assert.That(dg.MaxValue, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCase(3, 999, new int[]{ 9, 9, 9})]
    [TestCase(3, 0, new int[]{ 0, 0, 0})]
    [TestCase(3, 22, new int[]{ 2, 2, 0})]
    public void DigitGroupDigitsTest(int places, int value, int[] expectedDigits)
    {
        var dg = new DigitGroup(places);
        dg.SetValue(value);
        Assert.That(dg.Digits, Is.EqualTo(expectedDigits));
    }

    [Test]
    [TestCase(3, 0, 0, false)]
    [TestCase(3, 0, 1, false)]
    [TestCase(3, 0, 2, false)]
    [TestCase(3, 22, 0, true)]
    [TestCase(3, 22, 1, true)]
    [TestCase(3, 22, 2, false)]
    [TestCase(3, 123, 0, true)]
    [TestCase(3, 123, 1, true)]
    [TestCase(3, 123, 2, true)]
    public void DigitGroupUsedTest(int places, int value, int place, bool used)
    {
        var dg = new DigitGroup(places);
        dg.SetValue(value);
        Assert.That(dg.IsDigitUsed(place) == used);
    }

    [Test]
    [TestCase(3, 16, 0, 6)]
    [TestCase(3, 16, 1, 1)]
    [TestCase(3, 16, 2, 0)]
    [TestCase(3, 234, 0, 4)]
    [TestCase(3, 234, 1, 3)]
    [TestCase(3, 234, 2, 2)]
    public void DigitValueTest(int places, int value, int place, int expected)
    {
        var dg = new DigitGroup(places);
        dg.SetValue(value);
        Assert.That(dg.GetDigitValue(place), Is.EqualTo(expected));
    }
}
