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

		private static string getStringValue(string data) {
			if(data == null) { return ""; }
			int spaceIndex = data.IndexOf(" ");
			if(spaceIndex < 0) { return ""; }
			return data.Substring(spaceIndex + 1, data.Length - spaceIndex - 1);
		}

		public bool getOnOffValue(string data) {
			string temp = getStringValue(data).Trim().ToLower();
				 if(temp.Equals("1", StringComparison.OrdinalIgnoreCase)) { return true; }
			else if(temp.Equals("on", StringComparison.OrdinalIgnoreCase)) { return true; }
			else if(temp.Equals("enable", StringComparison.OrdinalIgnoreCase)) { return true; }
			else if(temp.Equals("true", StringComparison.OrdinalIgnoreCase)) { return true; }
			else if(temp.Equals("show", StringComparison.OrdinalIgnoreCase)) { return true; }
			return false;
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

		public void execute(string command) {
			string cmd = command.Trim().ToLower();
				 if(cmd.StartsWith("quit")) { m_game.Exit(); }
			else if(cmd.StartsWith("exit")) { m_game.Exit(); }
			else if(cmd.StartsWith("clear")) { m_console.clear(); }
			else if(cmd.StartsWith("cls")) { m_console.clear(); }
			else if(cmd.StartsWith("echo")) { m_console.writeLine(getStringValue(command)); }
			else if(cmd.StartsWith("togglemenu")) { m_screenManager.toggle(ScreenType.Menu); }
			else if(cmd.StartsWith("menu")) { m_screenManager.show(ScreenType.Menu, getOnOffValue(command)); }
			else if(cmd.StartsWith("toggleconsole")) { m_screenManager.toggle(ScreenType.Console); }
			else if(cmd.StartsWith("console")) { m_screenManager.show(ScreenType.Console, getOnOffValue(command)); }
		}
	}

}
