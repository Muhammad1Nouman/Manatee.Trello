﻿using System.Threading.Tasks;

namespace Manatee.Trello
{
	/// <summary>
	/// A collection of members.
	/// </summary>
	public interface IMemberCollection : IReadOnlyMemberCollection
	{
		/// <summary>
		/// Adds a member to the collection.
		/// </summary>
		/// <param name="member">The member to add.</param>
		Task Add(IMember member);

		/// <summary>
		/// Removes a member from the collection.
		/// </summary>
		/// <param name="member">The member to remove.</param>
		Task Remove(IMember member);
	}
}