﻿using ImGuiNET;

using PalettePlus.Structs;
using PalettePlus.Palettes;
using PalettePlus.Services;
using PalettePlus.Interface.Components;

namespace PalettePlus.Interface.Windows.Tabs {
	internal class PaletteEdit {
		private Palette DefaultPalette = new();

		private PaletteList PaletteList = new();

		// "Saved Palettes" Tab

		internal unsafe void Draw(bool firstFrame = false) {
			var select = PaletteList.Draw();
			if (select || firstFrame) {
				var local = PluginServices.ClientState.LocalPlayer;
				if (local != null)
					PaletteService.GetCharaPalette(local, out var _, out DefaultPalette, true);

				if (PaletteList.Selected != null) {
					var model = (object)PaletteService.ParamContainer.Model;
					PaletteList.Selected.ApplyShaderParams(ref model);
					PaletteService.ParamContainer.Model = (ModelParams)model;

					var decal = (object)PaletteService.ParamContainer.Decal;
					PaletteList.Selected.ApplyShaderParams(ref decal);
					PaletteService.ParamContainer.Decal = (DecalParams)decal;
				}
			}

			ImGui.SameLine();

			ImGui.BeginGroup();

			if (PaletteList.Selected != null) {
				if (ImGui.Button("Apply to Self")) {
					var self = PluginServices.ObjectTable[201] ?? PluginServices.ClientState.LocalPlayer;
					if (self != null)
						PaletteList.Selected.Apply(self);
				}

				ImGui.SameLine();

				var tar = PluginServices.Targets->GPoseTarget != null ? PluginServices.Targets->GPoseTarget : PluginServices.Targets->Target;

				ImGui.BeginDisabled(tar == null);
				if (ImGui.Button("Apply to Target") && tar != null) {
					var obj = PluginServices.ObjectTable.CreateObjectReference((nint)tar);
					if (obj != null) PaletteList.Selected.Apply(obj);
				}
				ImGui.EndDisabled();

				PaletteEditor.Draw(DefaultPalette, ref PaletteList.Selected, ref PaletteService.ParamContainer, true);
			}

			ImGui.EndGroup();
		}
	}
}