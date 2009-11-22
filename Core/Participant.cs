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
		/// Is the participant authorized to make changes to the board
		/// other than its own dudes' actions? This applies to the server
		/// normally, or to everyone in peer-to-peer games
		/// </summary>
		public readonly bool HasAuthority;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hasAuthority"></param>
		protected Participant(bool hasAuthority)
		{
			HasAuthority = hasAuthority;
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
		public event EventHandler<ControlPlayerEventArgs> ControlPlayer;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnControlPlayer(ControlPlayerEventArgs e)
		{
			if (ControlPlayer != null)
				ControlPlayer(this, e);
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
	public class ControlPlayerEventArgs : EventArgs
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
		public ControlPlayerEventArgs(int playerIndex, Directions direction, bool moving)
		{
			PlayerIndex = playerIndex;
			Direction = direction;
			Moving = moving;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class LocalParticipant : Participant
	{
		private Player[] Players;
		/// <summary>
		/// 
		/// </summary>
		public LocalParticipant()
			: base(false)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		public override void StartNegotiation()
		{
			OnNegotiate(new NegotiateEventArgs(1));
		}

		/// <summary>
		/// 
		/// </summary>
		public override void StartRound(Player[] yourPlayers)
		{
			Players = yourPlayers;
			// ... okay... so the round is starting
			
			// let's do something crazy... move in a random direction! :D
			Directions direction = (Directions)new Random().Next(4);
			OnControlPlayer(new ControlPlayerEventArgs(Players[0].PlayerIndex, direction, true));
		}
	}
}
