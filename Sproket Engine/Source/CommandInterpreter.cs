﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SproketEngine {

	enum BoolChange { None, Toggle, Enable, Disable }

	class CommandInterpreter {

		// "global" variables
		private ScrapHeap m_game;
		private Player m_player;
		private ScreenManager m_screenManager;
		private ControlSystem m_controlSystem;
		private GameSettings m_settings;
		private GameConsole m_console;

		public CommandInterpreter() { }

		// initialize the command interpreter
		public void initialize(ScrapHeap game, Player player, ScreenManager screenManager, ControlSystem controlSystem, GameSettings settings, GameConsole console) {
			m_game = game;
			m_player = player;
			m_screenManager = screenManager;
			m_controlSystem = controlSystem;
			m_settings = settings;
			m_console = console;
		}

		// execute a command
		public void execute(string command) {
			if(command == null) { return; }
			string cmd = command.Trim();

			// parse the command and execute the corresponding function
			if(matchCommand(cmd, "quit") || matchCommand(cmd, "exit")) { m_game.Exit(); } // close the game
			else if(matchCommand(cmd, "clear") || matchCommand(cmd, "cls")) { m_console.clear(); } // clear the console
			else if(matchCommand(cmd, "echo")) { m_console.writeLine(getStringValue(cmd)); } // write text to the console
			else if(matchCommand(cmd, "menu")) { m_screenManager.set(ScreenType.Menu, getScreenVisibilityChange(cmd)); } // change the menu's visibility
			else if(matchCommand(cmd, "console")) { m_screenManager.set(ScreenType.Console, getScreenVisibilityChange(cmd)); } // change the console's visibility
			else if(matchCommand(cmd, "noclip")) { // change player clipping
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
			else if(matchCommand(cmd, "map")) { // load a specified level
				string levelName = getStringValue(cmd);
				bool levelLoaded = m_game.loadLevel(levelName);
				if(!levelLoaded) {
					m_console.writeLine("Unable to load map: " + levelName);
				}
			}
			else if(matchCommand(cmd, "bind")) { // bind a key to a command
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
			else if(matchCommand(cmd, "unbind")) { // unbind a command from a key
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
			else if(matchCommand(cmd, "unbindall")) { // unbind all commands from all keys
				m_controlSystem.removeAllKeyBinds();
				m_console.writeLine("Successfully unbound all keys");
			}
			else if(matchCommand(cmd, "+moveforward")) { m_player.moveForward(); } // move the player forward
			else if(matchCommand(cmd, "+movebackward")) { m_player.moveBackward(); } // move the player backward
			else if(matchCommand(cmd, "+moveleft")) { m_player.moveLeft(); } // move the player left
			else if(matchCommand(cmd, "+moveright")) { m_player.moveRight(); } // move the player right
			else if(matchCommand(cmd, "+jump")) { m_player.jump(); } // make the player jump
			else if(matchCommand(cmd, "weapon1")) { m_player.selectWeapon(0); } // select weapon 1
			else if(matchCommand(cmd, "weapon2")) { m_player.selectWeapon(1); } // select weapon 2
			else if(matchCommand(cmd, "weapon3")) { m_player.selectWeapon(2); } // select weapon 3
			else if(matchCommand(cmd, "weapon4")) { m_player.selectWeapon(3); } // select weapon 4
            else if(matchCommand(cmd, "weapon5")) { m_player.selectWeapon(4); } // select weapon 5
			else { m_console.writeLine("Unknown command: " + cmd); }
		}

		// check to see if some input matches a specified command
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

		// parse a string value which trails after a command (delimited by whitespace)
		private static string getStringValue(string data) {
			if(data == null) { return ""; }
			int spaceIndex = data.IndexOf(" ");
			if(spaceIndex < 0) { return ""; }
			return data.Substring(spaceIndex + 1, data.Length - spaceIndex - 1);
		}

		// parse a integer value which trails after a command (delimited by whitespace)
		private static int getIntValue(string data) {
			return int.Parse(getStringValue(data));
		}

		// parse a float value which trails after a command (delimited by whitespace)
		private static float getFloatValue(string data) {
			return float.Parse(getStringValue(data));
		}

		// parse a boolean float which trails after a command (delimited by whitespace)
		private static bool getBoolValue(string data) {
			return bool.Parse(getStringValue(data));
		}

		// check to see if a string value trailing after a command is a valid screen visibility change value
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

		// check to see if a string value trailing after a command is a valid boolean change value
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
