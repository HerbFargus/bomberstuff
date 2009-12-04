//
// Participant.cs - Participant class
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

using System;

namespace BomberStuff.Core
{
	/// <summary>
	/// Represents a participant in a game
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class represents a participant, which can in turn
	/// be able to control zero (spectator) or more BombDudes (players)
	/// in the game.
	/// </para><para>
	/// In a server-based network game, each client is a participant from
	/// the server's perspective, while each client sees only itself and
	/// the server as participants.
	/// </para><para>
	/// In a peer to peer game, each client sees itself and every other
	/// client as a participant
	/// </para>
	/// <para>
	/// Important: The results of a public method call, as well as every
	/// event must be idempotent!
	/// </para>
	/// </remarks>
	public abstract class Participant
	{
		/// <summary>
		/// The players that this participant controls
		/// </summary>
		protected Player[] Players;

		/// <summary>
		/// Is the participant authorized to make changes to the board
		/// other than its own dudes' actions? This applies to the server
		/// normally, or to everyone in peer-to-peer games
		/// </summary>
		public readonly bool HasAuthority;

		/// <summary>
		/// Initializes a new participant
		/// </summary>
		/// <param name="hasAuthority">
		/// boolean value specifying whether the participant has authority,
		/// that is, whether it can control more than just its players'
		/// movement
		/// </param>
		protected Participant(bool hasAuthority)
		{
			HasAuthority = hasAuthority;
		}

		/// <summary>
		/// Returns whether the given player is controlled by
		/// this participant
		/// </summary>
		/// <param name="p"></param>
		/// <returns>
		/// <c>true</c> if this participant controls <paramref name="p" />,
		/// <c>false</c> otherwise
		/// </returns>
		public bool ControlsPlayer(Player p)
		{
			foreach (Player pi in Players)
				if (pi == p)
					return true;
			return false;
		}

		/// <summary>
		/// Returns whether the given player is controlled by
		/// this participant
		/// </summary>
		/// <param name="player">Index of the player to check</param>
		/// <returns>
		/// <c>true</c> if this participant controls <paramref name="player" />,
		/// <c>false</c> otherwise
		/// </returns>
		public bool ControlsPlayer(int player)
		{
			foreach (Player pi in Players)
				if (pi.PlayerIndex == player)
					return true;
			return false;
		}

		#region Notification methods
		// Notification methods are virtual methods which allow
		// the participant to handle events by overriding them.
		// A derived class need not override methods in whose
		// corresponding events it has no interest.
		// StartNegotiation, StartRound (and EndRound?) are the
		// exceptions which must be provided by every participant

		/// <summary>
		/// When overridden in a derived class, is called to notify
		/// the participant that game negotiation begins
		/// </summary>
		/// <remarks>
		/// After receiving this, the participant should raise
		/// the Negotiate event to state its preferences.
		/// This can be done from inside the method, if possible.
		/// </remarks>
		public abstract void StartNegotiation();
		
		/// <summary>
		/// When overridden in a derived class, is called to notify
		/// the participant that the game round starts
		/// </summary>
		/// <param name="yourPlayers">
		/// the players that the participant owns
		/// </param>
		/// <remarks>
		/// Receiving this means negotiation is complete, subsequent
		/// Negotiate events will be ignored.
		/// </remarks>
		public abstract void StartRound(Player[] yourPlayers);
		#endregion

		#region Notification events
		// Notification events are events which allow the
		// participant to control its player(s), and, if
		// the participant has authority, other aspects
		// of the game
		// TRYTRY: this is weird. Making the OnEvent methods
		//         virtual has no point in this scenario

		/// <summary>
		/// Signals the preferences of the participant
		/// during negotiation phase
		/// </summary>
		/// <remarks>
		/// This should be raised (once) between receiving the
		/// StartNegotiation and StartRound notifications
		/// </remarks>
		public event EventHandler<NegotiateEventArgs> Negotiate;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnNegotiate(NegotiateEventArgs e)
		{
			if (Negotiate != null)
				Negotiate(this, e);
		}

		/// <summary>
		/// Signales that the specified player (owned by the participant,
		/// unless the participant has authority) is changing its
		/// movement
		/// </summary>
		public event EventHandler<MovePlayerEventArgs> MovePlayer;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnMovePlayer(MovePlayerEventArgs e)
		{
			if (MovePlayer != null)
				MovePlayer(this, e);
		}

		/// <summary>
		/// Signales that the specified player (owned by the participant,
		/// unless the participant has authority) is doing an action
		/// (placing a bomb, pulling the trigger, ...)
		/// </summary>
		public event EventHandler<PlayerActionEventArgs> PlayerAction;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnPlayerAction(PlayerActionEventArgs e)
		{
			if (PlayerAction != null)
				PlayerAction(this, e);
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public class NegotiateEventArgs : EventArgs
	{
		/// <summary>
		/// number of players that the participant would like to control
		/// </summary>
		public readonly int NumberOfOwnPlayers;

		/// <summary>
		/// 
		/// </summary>
		public NegotiateEventArgs(int numberOfOwnPlayers)
		{
			NumberOfOwnPlayers = numberOfOwnPlayers;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class MovePlayerEventArgs : EventArgs
	{
		/// <summary>Specifies the index of the player being controlled</summary>
		public readonly int PlayerIndex;
		/// <summary>Specifies the player's direction</summary>
		public readonly Directions Direction;
		/// <summary>Specifies whether the player is moving</summary>
		public readonly bool Moving;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerIndex"></param>
		/// <param name="direction"></param>
		/// <param name="moving"></param>
		public MovePlayerEventArgs(int playerIndex, Directions direction, bool moving)
		{
			PlayerIndex = playerIndex;
			Direction = direction;
			Moving = moving;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class PlayerActionEventArgs : EventArgs
	{
		/// <summary>
		/// 
		/// </summary>
		public enum Types
		{
			/// <summary>
			/// Player action 1:
			/// - place a bomb
			/// - pick up a bomb
			/// - spooge
			/// </summary>
			Action1,
			/// <summary>
			/// Player action 2:
			/// - stop moving bombs
			/// - pull the trigger
			/// - punch a bomb
			/// </summary>
			Action2
		}

		/// <summary>Specifies the index of the player being controlled</summary>
		public readonly int PlayerIndex;

		/// <summary>
		/// Specifies the type of action the player is performing
		/// </summary>
		public readonly Types Type;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerIndex"></param>
		/// <param name="type"></param>
		public PlayerActionEventArgs(int playerIndex, Types type)
		{
			PlayerIndex = playerIndex;
			Type = type;
		}
	}
}
