﻿// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.DebugUtils;
using KSPDev.GUIUtils;
using KSPDev.LogUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSPDev.DebugUtils {

/// <summary>Debug dialog for adjusting the part configuration.</summary>
/// <seealso cref="DebugAdjustableAttribute"/>
public sealed class PartDebugAdjustmentDialog : MonoBehaviour,
    // KSPDev interfaces
    IHasGUI {

  #region Local fields
  /// <summary>Actual screen position of the console window.</summary>
  Rect windowRect = new Rect(100, 100, 1, 1);

  /// <summary>A title bar location.</summary>
  Rect titleBarRect = new Rect(0, 0, 10000, 20);

  /// <summary>A list of actions to apply at the end of the GUI frame.</summary>
  readonly GuiActionsList guiActions = new GuiActionsList();

  /// <summary>The part being adjusted.</summary>
  Part parentPart;

  /// <summary>Tells if the parent part capture mode is enabled.</summary>
  bool parentPartTracking;

  /// <summary>Array of the moduels, available for the debug dialog.</summary>
  /// <remarks>The key of the pair is the module name.</remarks>
  KeyValuePair<string, IRenderableGUIControl[]>[] adjustableModules;

  /// <summary>The index of the selected module in <see cref="adjustableModules"/>.</summary>
  int selectedModule = -1;

  /// <summary>Scroll view for the adjustable modules.</summary>
  GUILayoutVerticalScrollView mainScrollView = new GUILayoutVerticalScrollView();

  /// <summary>Dialog title.</summary>
  internal string dialogTitle = "";

  /// <summary>Dialog width.</summary>
  internal float dialogWidth = 500.0f;

  /// <summary>Size of the controls for the values.</summary>
  internal float dialogValueSize = 250.0f;
  #endregion

  #region IHasGUI implementation
  /// <inheritdoc/>
  public void OnGUI() {
    windowRect = GUILayout.Window(
        GetInstanceID(), windowRect, ConsoleWindowFunc, dialogTitle,
        GUILayout.MaxHeight(1), GUILayout.Width(dialogWidth));
  }
  #endregion

  #region Local utility methods
  /// <summary>Shows a window that displays the winch controls.</summary>
  /// <param name="windowId">Window ID.</param>
  void ConsoleWindowFunc(int windowId) {
    if (guiActions.ExecutePendingGuiActions()) {
      if (parentPartTracking && Input.GetMouseButtonDown(0)
          && !windowRect.Contains(Mouse.screenPos)) {
        SetPart(Mouse.HoveredPart);
        parentPartTracking = false;
      }
    }

    if (GUILayout.Button(!parentPartTracking ? "Set part" : "Cancel set mode...")) {
      guiActions.Add(() => { parentPartTracking = !parentPartTracking; });
    }
    string parentPartName = parentPart != null ? DbgFormatter.PartId(parentPart) : "NONE";
    if (parentPartTracking && Mouse.HoveredPart != null) {
      parentPartName = "Select: " + DbgFormatter.PartId(Mouse.HoveredPart);
    }
    GUILayout.Label(parentPartName, new GUIStyle(GUI.skin.box) { wordWrap = true });

    // Render the adjustable fields.
    if (parentPart != null && adjustableModules != null) {
      mainScrollView.BeginView(GUI.skin.box, Screen.height - 100);
      for (var i = 0; i < adjustableModules.Length; i++) {
        var isSelected = selectedModule == i;
        var module = adjustableModules[i];
        var toggleCaption = (isSelected ? "\u25b2 " : "\u25bc ") + module.Key;
        if (GUILayout.Button(toggleCaption)) {
          var selectedModuleSnapshot = selectedModule == i ? -1 : i;  // Make a copy for lambda!
          guiActions.Add(() => selectedModule = selectedModuleSnapshot);
        }
        if (isSelected) {
          foreach (var control in module.Value) {
            control.RenderControl(
                guiActions, GUI.skin.label, new[] { GUILayout.Width(dialogValueSize) });
          }
        }
      }
      mainScrollView.EndView();
    }

    if (GUILayout.Button("Close", GUILayout.ExpandWidth(false))) {
      guiActions.Add(() => DebugGui.DestroyPartDebugDialog(this));
    }

    // Allow the window to be dragged by its title bar.
    GuiWindow.DragWindow(ref windowRect, titleBarRect);
  }

  /// <summary>Sets the part to be adjusted.</summary>
  /// <param name="part">The part to set.</param>
  void SetPart(Part part) {
    parentPart = part;
    if (part != null) {
      adjustableModules = part.Modules
          .Cast<PartModule>()
          .Select(m => new KeyValuePair<string, IRenderableGUIControl[]>(
              m.moduleName,
              DebugGui.GetAdjustableFields(m)
                  .Select(f => new StdTypesDebugGuiControl(f.attr.caption, m, f.fieldInfo))
                  .ToArray()))
          .Where(r => r.Value.Length > 0)
          .OrderBy(r => r.Key)
          .ToArray();
    } else {
      adjustableModules = null;
    }
  }
  #endregion
}

}  // namespace
