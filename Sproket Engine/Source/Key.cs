using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SproketEngine {

	class Key {

		private List<string> m_keyStrings;

		public Key(string keyString) {
			m_keyStrings = new List<string>();
			addKeyString(keyString);
		}

		public Key(string keyString, string altKeyString) {
			m_keyStrings = new List<string>();
			addKeyString(keyString);
			addKeyString(altKeyString);
		}

		public Key(string keyString, string altKeyString, string altKeyString2) {
			m_keyStrings = new List<string>();
			addKeyString(keyString);
			addKeyString(altKeyString);
			addKeyString(altKeyString2);
		}

		public Key(string keyString, string altKeyString, string altKeyString2, string altKeyString3) {
			m_keyStrings = new List<string>();
			addKeyString(keyString);
			addKeyString(altKeyString);
			addKeyString(altKeyString2);
			addKeyString(altKeyString3);
		}

		public Key(string keyString, string altKeyString, string altKeyString2, string altKeyString3, string altKeyString4) {
			m_keyStrings = new List<string>();
			addKeyString(keyString);
			addKeyString(altKeyString);
			addKeyString(altKeyString2);
			addKeyString(altKeyString3);
			addKeyString(altKeyString4);
		}

		public Key(string keyString, string altKeyString, string altKeyString2, string altKeyString3, string altKeyString4, string altKeyString5) {
			m_keyStrings = new List<string>();
			addKeyString(keyString);
			addKeyString(altKeyString);
			addKeyString(altKeyString2);
			addKeyString(altKeyString3);
			addKeyString(altKeyString4);
			addKeyString(altKeyString5);
		}

		public int size() { return m_keyStrings.Count(); }

		public string getKeyString(int index) {
			if(index < 0 || index >= m_keyStrings.Count()) { return null; }
			return m_keyStrings[index];
		}

		public bool addKeyString(string keyString) {
			if(keyString == null) { return false; }
			string formattedKeyString = keyString.Trim();
			if(!m_keyStrings.Contains(formattedKeyString)) {
				m_keyStrings.Add(formattedKeyString);
				return true;
			}
			return false;
		}

	}

}
