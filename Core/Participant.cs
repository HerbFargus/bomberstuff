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
		/// Notify the participant of changes to the board?
		/// </summary>
		public readonly bool Notify;

		/// <summary>
		/// Is the participant authorized to make changes to the board
		/// other than its own dudes' actions? This applies to the server
		/// normally, or to everyone if cheating mode is enabled
		/// </summary>
		public readonly bool HasAuthority;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="notify"></param>
		/// <param name="hasAuthority"></param>
		protected Participant(bool notify, bool hasAuthority)
		{
			Notify = notify;
			HasAuthority = hasAuthority;
		}

		// TODO: Notification methods
		// TODO: Notification events
	}

	/// <summary>
	/// 
	/// </summary>
	public class LocalParticipant : Participant
	{
		/// <summary>
		/// 
		/// </summary>
		public LocalParticipant()
			: base(false, false)
		{

		}
	}
}
