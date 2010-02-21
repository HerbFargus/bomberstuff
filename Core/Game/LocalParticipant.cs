//
// LocalParticipant.cs - LocalParticipant class
//
// Copyright © 2009-2010  Thomas Faber
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

using System;

using BomberStuff.Core.Input;

namespace BomberStuff.Core.Game
{
	/// <summary>
	/// 
	/// </summary>
	public class LocalParticipant : Participant
	{
		private PlayerControls[] Controls;

		/// <summary>
		/// 
		/// </summary>
		public LocalParticipant(PlayerControls[] controls)
			: base(false)
		{
			Controls = controls;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void StartNegotiation()
		{
			OnNegotiate(new NegotiateEventArgs(Controls.Length));
		}

		/// <summary>
		/// 
		/// </summary>
		public override void StartRound(Player[] yourPlayers)
		{
			Players = yourPlayers;
			// ... okay... so the round is starting

			// let's do something crazy... move in a random direction! :D
			//Directions direction = (Directions)Game.GetRandom(4);
			//OnMovePlayer(new MovePlayerEventArgs(Players[0].PlayerIndex, direction, true));

			foreach (PlayerControls control in Controls)
			{
				if (control != null)
				{
					control.Pressed += Control_Pressed;
					control.Released += Control_Released;
				}
			}
		}

		private void MovePressed(Player player, Directions direction)
		{
			if (direction == player.Direction)
				return;
			
			if (player.Moving)
				OnMovePlayer(new MovePlayerEventArgs(player.PlayerIndex, direction, player.Direction, true));
			else
				OnMovePlayer(new MovePlayerEventArgs(player.PlayerIndex, direction, direction, true));
		}

		private void MoveReleased(Player player, Directions direction)
		{
			if (direction == player.Direction)
			{
				if (direction != player.SecondaryDirection)
					OnMovePlayer(new MovePlayerEventArgs(player.PlayerIndex, player.SecondaryDirection, player.SecondaryDirection, true));
				else
					OnMovePlayer(new MovePlayerEventArgs(player.PlayerIndex, player.Direction, player.Direction, false));
			}
			else if (direction == player.SecondaryDirection)
				OnMovePlayer(new MovePlayerEventArgs(player.PlayerIndex, player.Direction, player.Direction, player.Moving));
		}

		private void Control_Pressed(object sender, PlayerControlEventArgs e)
		{
			for (int i = 0; i < Math.Min(Players.Length, Controls.Length); ++i)
				if (Controls[i] == sender)
				{
					switch (e.Type)
					{
						case PlayerControls.Types.Up:
							MovePressed(Players[i], Directions.Up);
							break;
						case PlayerControls.Types.Down:
							MovePressed(Players[i], Directions.Down);
							break;
						case PlayerControls.Types.Left:
							MovePressed(Players[i], Directions.Left);
							break;
						case PlayerControls.Types.Right:
							MovePressed(Players[i], Directions.Right);
							break;
						case PlayerControls.Types.Action1:
							OnPlayerAction(new PlayerActionEventArgs(Players[i].PlayerIndex, PlayerActionEventArgs.Types.Action1));
							break;
						case PlayerControls.Types.Action2:
							OnPlayerAction(new PlayerActionEventArgs(Players[i].PlayerIndex, PlayerActionEventArgs.Types.Action2));
							break;
						default:
							break;
					}
					break;
				}
		}

		private void Control_Released(object sender, PlayerControlEventArgs e)
		{
			for (int i = 0; i < Math.Min(Players.Length, Controls.Length); ++i)
				if (Controls[i] == sender)
				{
					switch (e.Type)
					{
						case PlayerControls.Types.Up:
							MoveReleased(Players[i], Directions.Up);
							break;
						case PlayerControls.Types.Down:
							MoveReleased(Players[i], Directions.Down);
							break;
						case PlayerControls.Types.Left:
							MoveReleased(Players[i], Directions.Left);
							break;
						case PlayerControls.Types.Right:
							MoveReleased(Players[i], Directions.Right);
							break;
						default:
							break;
					}
					break;
				}
		}
	}
}
