﻿// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>
/// Localized message formatting class for a numeric value that represents a <i>distance</i>.
/// </summary>
/// <remarks>
/// The class uses the unit name localizations from the stock module <c>ModuleEnviroSensor</c>. In
/// case of this module is deprecated or the tags are changed, the default English values will be
/// used for the unit names.
/// </remarks>
/// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatDefault"/></example>
/// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatWithScale"/></example>
/// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatFixed"/></example>
public sealed class DistanceType {
  /// <summary>Localized suffix for the "meter" units. Scale x1.</summary>
  public static readonly Message meter = new Message(
      "#autoLOC_7001411", defaultTemplate: "\u0020m",
      description: "Meter unit for a distance value");

  /// <summary>Localized suffix for the "kilometer" untis. Scale x1000</summary>
  public static readonly Message kilometer = new Message(
      "#autoLOC_7001405", defaultTemplate: "\u0020km",
      description: "Kilometer unit for a distance value");

  /// <summary>A wrapped numeric value.</summary>
  /// <remarks>This is the original non-rounded and unscaled value.</remarks>
  public readonly double value;

  /// <summary>Constructs a distance type object.</summary>
  /// <param name="value">The numeric value in meters.</param>
  /// <seealso cref="Format"/>
  /// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatWithScale"/></example>
  /// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatFixed"/></example>
  public DistanceType(double value) {
    this.value = value;
  }

  /// <summary>Coverts a numeric value into a distance type object.</summary>
  /// <param name="value">The numeric value to convert.</param>
  /// <returns>A distance type object.</returns>
  public static implicit operator DistanceType(double value) {
    return new DistanceType(value);
  }

  /// <summary>Converts a distance type object into a numeric value.</summary>
  /// <param name="distanceObj">The object type to convert.</param>
  /// <returns>A numeric value.</returns>
  public static implicit operator double(DistanceType distanceObj) {
    return distanceObj.value;
  }

  /// <summary>Formats the distance to a human friendly string.</summary>
  /// <remarks>
  /// <para>
  /// The method tries to keep the resulted string meaningful and as short as possible. For this
  /// reason the big values may be scaled down and/or rounded.
  /// </para>
  /// </remarks>
  /// <param name="value">The numeric value to format.</param>
  /// <param name="scale">
  /// The fixed scale to apply to the value before formatting. The formatting method can uderstand
  /// only a few scales: metters (x1) and kilometers (x1000). Unknown scales will be rounded to the
  /// closest known scale. If this parameter is omitted, then the best scale for the value will be
  /// choosen automatically.
  /// </param>
  /// <param name="format">
  /// The specific float number format to use. If this parameter is specified, then the method
  /// doesn't try to guess the right scale. Instead, it uses either the provided scale, or <c>x1</c>
  /// if nothing is provided.
  /// </param>
  /// <returns>A formatted and localized string</returns>
  /// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatWithScale"/></example>
  /// <example><code source="Examples/GUIUtils/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatFixed"/></example>
  public static string Format(double value, double? scale = null, string format = null) {
    // For the fixed format only deal with the scale. 
    if (format != null) {
      if (scale.HasValue && scale.Value >= 1000.0) {
        return (value / 1000.0).ToString(format) + kilometer.Format();
      }
      return value.ToString(format) + meter.Format();
    }
    // For the unspecified format try detecting the scale.
    if (!scale.HasValue || scale < 1000.0) {
      if (value < 0.1) {
        return value.ToString("0.00#") + meter.Format();
      }
      if (value < 1.0) {
        return value.ToString("0.00") + meter.Format();
      }
      if (value < 10.0) {
        return value.ToString("0.0#") + meter.Format();
      }
      if (value < 100.0) {
        return value.ToString("0.#") + meter.Format();
      }
      if (scale < 1000.0 || value < 10000.0) {
        return value.ToString("0") + meter.Format();
      }
    }
    var scaled = value / 1000.0;
    if (scaled < 100.0) {
      return scaled.ToString("0.#") + kilometer.Format();
    }
    return scaled.ToString("0") + kilometer.Format();
  }

  /// <summary>Returns a string formatted as a human friendly distance specification.</summary>
  /// <returns>A string representing the value.</returns>
  /// <seealso cref="Format"/>
  public override string ToString() {
    return Format(value);
  }
}

}  // namespace
