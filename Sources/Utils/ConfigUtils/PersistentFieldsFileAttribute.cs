﻿// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>A base for any persitent fields file annotation.</summary>
/// <remarks>See more details and examples in <see cref="ConfigAccessor"/> module.</remarks>
public abstract class AbstractPersitentFieldsFileAttribute : Attribute {
  /// <summary>A group tag for this file to contain.</summary>
  public readonly string group;

  /// <summary>A path to the node which will be a root for the fields in the group.</summary>
  /// <remarks>By setting different root for every group and/or type you may combine multiple
  /// settings in the same config file.</remarks>
  public readonly string[] nodePath;

  /// <summary>Relative path to the config file.</summary>
  /// <remarks>The path is relative to the game's "GameData" folder.</remarks>
  public readonly string configFilePath;

  protected AbstractPersitentFieldsFileAttribute(
      string configFilePath, string nodePath, string group) {
    this.configFilePath = configFilePath;
    this.nodePath = nodePath.Split('/');
    this.group = group;
  }
}

/// <summary>
/// Simple persitent fields file annotation that requires all arguments to be set in constructor.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class PersistentFieldsFileAttribute : AbstractPersitentFieldsFileAttribute {
  public PersistentFieldsFileAttribute(string configFilePath, string nodePath,
                                       string group = StdPersistentGroups.Default)
      : base(configFilePath, nodePath, group) {
  }
}

}  // namespace