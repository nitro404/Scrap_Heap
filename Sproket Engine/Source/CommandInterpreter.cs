using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SproketEngine {

	enum BoolChange { None, Toggle, Enable, Disable }

	class CommandInterpreter {

		private ScrapHeap m_game;
		private ScreenManager m_screenManager;
		private ControlSystem m_controlSystem;
		private GameSettings m_settings;
		private GameConsole m_console;

		public CommandInterpreter() { }

		public void initialize(ScrapHeap game, ScreenManager screenManager, ControlSystem controlSystem, GameSettings settings, GameConsole console) {
			m_game = game;
			m_screenManager = screenManager;
			m_controlSystem = controlSystem;
			m_settings = settings;
			m_console = console;
		}

		public void execute(string cmd) {
			if(cmd == null) { return; }
				 if(matchCommand(cmd, "quit") || matchCommand(cmd, "exit")) { m_game.Exit(); }
			else if(matchCommand(cmd, "clear") || matchCommand(cmd, "cls")) { m_console.clear(); }
			else if(matchCommand(cmd, "echo")) { m_console.writeLine(getStringValue(cmd)); }
			else if(matchCommand(cmd, "menu")) { m_screenManager.set(ScreenType.Menu, getScreenVisibilityChange(cmd)); }
			else if(matchCommand(cmd, "console")) { m_screenManager.set(ScreenType.Console, getScreenVisibilityChange(cmd)); }
			else if(matchCommand(cmd, "noclip")) {
				BoolChange change = getBoolChange(cmd);
				if(change == BoolChange.Enable) {
					m_settings.clipping = false;
				}
				else if(change == BoolChange.Disable) {
					m_settings.clipping = true;
				}
				else if(change == BoolChange.Toggle) {
					m_settings.clipping = !m_settings.clipping;
				}
			}
			else if(matchCommand(cmd, "map")) {
				string levelName = getStringValue(cmd);
				bool levelLoaded = m_game.loadLevel(levelName);
				if(!levelLoaded) {
					m_console.writeLine("Unable to load map: " + levelName);
				}
			}
			else if(matchCommand(cmd, "bind")) {
				string bind = getStringValue(cmd);
				if(bind.Length == 0) {
					m_console.writeLine("Unable to bind key");
					return;
				}
				int spaceIndex = bind.IndexOf(" ");
				if(spaceIndex < 0) {
					m_console.writeLine("Unable to bind key");
					return;
				}

				string keyString = bind.Substring(0, spaceIndex);
				string keyCommand = bind.Substring(spaceIndex + 1, bind.Length - spaceIndex - 1);

				if(m_controlSystem.createKeyBind(keyString, keyCommand)) {
					m_console.writeLine("Successfully bound key \"" + keyString + "\" to command \"" + keyCommand + "\"");
				}
				else {
					m_console.writeLine("Unable to bind key \"" + keyString + "\" to command \"" + keyCommand + "\"");
				}
			}
			else if(matchCommand(cmd, "unbind")) {
				string keyString = getStringValue(cmd);
				if(keyString.Length == 0) {
					m_console.writeLine("Unable to unbind key \"" + keyString + "\"");
					return;
				}

				if(m_controlSystem.removeKeyBind(keyString)) {
					m_console.writeLine("Successfully unbound key \"" + keyString + "\"");
				}
				else {
					m_console.writeLine("Unable to unbind key \"" + keyString + "\"");
				}
			}
			else if(matchCommand(cmd, "unbindall")) {
				m_controlSystem.removeAllKeyBinds();
				m_console.writeLine("Successfully unbound all keys");
			}
			else { m_console.writeLine("Unknown command: " + cmd); }
		}

		private static bool matchCommand(string input, string command) {
			if(input == null || command == null) { return false; }

			string temp = input.Trim();
			int spaceIndex = temp.IndexOf(" ");

			string inputCmd;
			if(spaceIndex < 0) { inputCmd = temp; }
			else { inputCmd = temp.Substring(0, spaceIndex); }

			if(inputCmd.Length != command.Length) { return false; }
			return inputCmd.Equals(command, StringComparison.OrdinalIgnoreCase);
		}

		private static string getStringValue(string data) {
			if(data == null) { return ""; }
			int spaceIndex = data.IndexOf(" ");
			if(spaceIndex < 0) { return ""; }
			return data.Substring(spaceIndex + 1, data.Length - spaceIndex - 1);
		}

		private static int getIntValue(string data) {
			return int.Parse(getStringValue(data));
		}

		private static float getFloatValue(string data) {
			return float.Parse(getStringValue(data));
		}

		private static bool getBoolValue(string data) {
			return bool.Parse(getStringValue(data));
		}

		public static ScreenVisibilityChange getScreenVisibilityChange(string data) {
			string temp = getStringValue(data).Trim().ToLower();

			if(temp.Equals("toggle", StringComparison.OrdinalIgnoreCase)) {
				return ScreenVisibilityChange.Toggle;
			}

			if(temp.Equals("1", StringComparison.OrdinalIgnoreCase) || 
			   temp.Equals("on", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("enable", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("true", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("show", StringComparison.OrdinalIgnoreCase) ||
               temp.Equals("open", StringComparison.OrdinalIgnoreCase)){
				return ScreenVisibilityChange.Show;
			}

			if(temp.Equals("0", StringComparison.OrdinalIgnoreCase) || 
			   temp.Equals("off", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("disable", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("false", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("hide", StringComparison.OrdinalIgnoreCase) ||
               temp.Equals("close", StringComparison.OrdinalIgnoreCase)) {
				return ScreenVisibilityChange.Hide;
			}

			return ScreenVisibilityChange.None;
		}

		public static BoolChange getBoolChange(string data) {
			string temp = getStringValue(data).Trim().ToLower();

			if(temp.Equals("toggle", StringComparison.OrdinalIgnoreCase)) {
				return BoolChange.Toggle;
			}

			if(temp.Equals("1", StringComparison.OrdinalIgnoreCase) || 
			   temp.Equals("on", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("enable", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("true", StringComparison.OrdinalIgnoreCase)) {
				return BoolChange.Enable;
			}

			if(temp.Equals("0", StringComparison.OrdinalIgnoreCase) || 
			   temp.Equals("off", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("disable", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("false", StringComparison.OrdinalIgnoreCase)) {
				return BoolChange.Disable;
			}

			return BoolChange.None;
		}

	}

}
