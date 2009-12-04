//
// Settings.cs - Settings class
//
// Copyright © 2009  Thomas Faber
//
// This file is part of Bomber Stuff.
//
// Bomber Stuff is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Bomber Stuff is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Bomber Stuff. If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using System.Collections;

namespace BomberStuff.Core
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Settings : IEnumerable<KeyValuePair<string, object>>
	{
		/// <summary>
		/// 
		/// </summary>
		public enum Types
		{
			/// <summary></summary>
			ABDirectory,

			/// <summary></summary>
			PlayerCount,

			/// <summary></summary>
			Tileset,

			/// <summary></summary>
			Scheme,

			/// <summary></summary>
			UserInterface,

			/// <summary></summary>
			WindowSize,

			/// <summary></summary>
			PlayerColor,

			/// <summary>The last type of setting</summary>
			Last = PlayerColor
		}

		private System.Type[] DataTypes =
		{
			typeof(string),
			typeof(int),
			typeof(int),
			typeof(string),
			typeof(string),
			typeof(System.Drawing.Size),
			typeof(BomberStuff.Core.Utilities.ColorRemapInfo[]),
		};

		private Dictionary<Types, object> CoreSettings;
		private Dictionary<string, object> CustomSettings;

		/// <summary>
		/// 
		/// </summary>
		public Settings()
		{
			CoreSettings = new Dictionary<Types, object>();
			CustomSettings = new Dictionary<string, object>();	
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public T Get<T>(Types type)
		{
			System.Diagnostics.Debug.Assert(DataTypes[(int)type] == typeof(T) || typeof(T) == typeof(object));
			try
			{
				return (T)CoreSettings[type];
			}
			catch (KeyNotFoundException)
			{
				return default(T);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public T Get<T>(string type)
		{
			try
			{
				return (T)CustomSettings[type];
			}
			catch (KeyNotFoundException)
			{
				return default(T);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public void Set<T>(Types type, T value)
		{
			System.Diagnostics.Debug.Assert(DataTypes[(int)type] == typeof(T));
			CoreSettings[type] = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public void Set<T>(string type, T value)
		{
			CustomSettings[type] = value;
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			foreach (KeyValuePair<Types, object> obj in CoreSettings)
				yield return new KeyValuePair<string, object>(obj.Key.ToString(), obj.Value);

			foreach (KeyValuePair<string, object> obj in CustomSettings)
				yield return obj;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
		}
	}

	
}
