using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SproketEngine {

	class CommandInterpreter {

		private ScrapHeap m_game;
		private ScreenManager m_screenManager;
		private GameSettings m_settings;
		private GameConsole m_console;

		public CommandInterpreter() { }

		public void initialize(ScrapHeap game, ScreenManager screenManager, GameSettings settings, GameConsole console) {
			m_game = game;
			m_screenManager = screenManager;
			m_settings = settings;
			m_console = console;
		}

		public void execute(string command) {
			string cmd = command.Trim().ToLower();
			if(cmd.StartsWith("quit") || cmd.StartsWith("exit")) { m_game.Exit(); }
			else if(cmd.StartsWith("clear") || cmd.StartsWith("cls")) { m_console.clear(); }
			else if(cmd.StartsWith("echo")) { m_console.writeLine(getStringValue(command)); }
			else if(cmd.StartsWith("menu")) { m_screenManager.set(ScreenType.Menu, getScreenVisibilityChange(command)); }
			else if(cmd.StartsWith("console")) { m_screenManager.set(ScreenType.Console, getScreenVisibilityChange(command)); }
			else { m_console.writeLine("Unknown Command: " + command); }
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
			   temp.Equals("show", StringComparison.OrdinalIgnoreCase)) {
				return ScreenVisibilityChange.Show;
			}

			if(temp.Equals("0", StringComparison.OrdinalIgnoreCase) || 
			   temp.Equals("off", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("disable", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("false", StringComparison.OrdinalIgnoreCase) ||
			   temp.Equals("hide", StringComparison.OrdinalIgnoreCase)) {
				return ScreenVisibilityChange.Hide;
			}

			return ScreenVisibilityChange.None;
		}

	}

}
