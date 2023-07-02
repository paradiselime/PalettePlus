using ImGuiNET;

using System;
using System.Linq;
using System.Reflection;

using Dalamud.Plugin;
using Dalamud.Logging;
using Dalamud.Game.Command;

using Penumbra.PlayerWatch;

using PalettePlus.Palettes;
using PalettePlus.Extensions;
using PalettePlus.Interface.Components;

namespace PalettePlus.Interface.Windows.Tabs {
	internal class CommandEdit {
		private const string HelpString = "Apply,Name or PlaceHolder,Palette Name";

        if (PaletteList.Selected != null) {
				if (ImGui.Button("Apply to Self")) {
					var self = PluginServices.ObjectTable[201] ?? PluginServices.ClientState.LocalPlayer;
					if (self != null && self is Character chara)
						PaletteList.Selected.Apply(chara, true);
				}
        
        Dalamud.Initialize(pluginInterface);
            Version       = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
            Config        = GlamourerConfig.Load();
            Customization = CustomizationManager.Create(Dalamud.PluginInterface, Dalamud.GameData, Dalamud.ClientState.ClientLanguage);
            Designs       = new DesignManager();
            Penumbra      = new PenumbraAttach(Config.AttachToPenumbra);
            PlayerWatcher = PlayerWatchFactory.Create(Dalamud.Framework, Dalamud.ClientState, Dalamud.Objects);
            GlamourerIpc  = new GlamourerIpc(Dalamud.ClientState, Dalamud.Objects, Dalamud.PluginInterface);
            if (!Config.ApplyFixedDesigns)
                PlayerWatcher.Disable();

            FixedDesigns  = new FixedDesigns(Designs);

        Dalamud.Commands.AddHandler("/papply", new CommandInfo(OnPapply)
            {
                HelpMessage = $"Use Palette Functions: {HelpString}",
            });

        public void ApplyCommand(Character player)
        {
            CharacterSave? save = null;
            {
				var validCharacter = persist.Character.Split(' ').Length == 2;
			    var validWorld = string.IsNullOrEmpty(persist.CharaWorld) || persist.CharaWorld.Split(' ').Length == 1;
			    var validPalette = !string.IsNullOrEmpty(persist.PaletteId);
                var self = PluginServices.ObjectTable[201] ?? PluginServices.ClientState.LocalPlayer;
				if (self != null && self is Character chara)
					persist.PaletteId.Apply(chara, true);
			}
            save?.Apply(player);
            Penumbra.UpdateCharacters(player);
        }

        public void OnPapply(string command, string arguments)
        {
            static void PrintHelp()
            {
                Dalamud.Chat.Print("Usage:");
                Dalamud.Chat.Print($"    {HelpString}");
            }

            arguments = arguments.Trim();
            if (!arguments.Any())
            {
                PrintHelp();
                return;
            }

            var split = arguments.Split(new[]
            {
                ',',
            }, 3, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length < 2)
            {
                PrintHelp();
                return;
            }

            var player = GetPlayer(split[1]) as Character;
            if (player == null)
            {
                Dalamud.Chat.Print($"Could not find object for {split[1]} or it was not a Character.");
                return;
            }

            switch (split[0].ToLowerInvariant())
            {
                
                case "apply":
                {
                    if (split.Length < 3)
                    {
                        Dalamud.Chat.Print("Applying requires a name for the save to be applied or 'clipboard'.");
                        return;
                    }

                    ApplyCommand(player, split[2]);

                    return;
                }
                
                }
                default:
                    PrintHelp();
                    return;
            }
        }
        public void Dispose()
        {
            FixedDesigns.Dispose();
            Penumbra.Dispose();
            PlayerWatcher.Dispose();
            _interface.Dispose();
            GlamourerIpc.Dispose();
            Dalamud.Commands.RemoveHandler("/papply");
        }
    }
}
