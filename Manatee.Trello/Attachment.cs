﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Synchronization;
using Manatee.Trello.Internal.Validation;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	/// <summary>
	/// Represents an attachment to a card.
	/// </summary>
	public class Attachment : IAttachment
	{
		/// <summary>
		/// Enumerates the data which can be pulled for attachments.
		/// </summary>
		[Flags]
		public enum Fields
		{
			/// <summary>
			/// Indicates the Data property should be populated.
			/// </summary>
			[Display(Description="bytes")]
			Bytes = 1,
			/// <summary>
			/// Indicates the Date property should be populated.
			/// </summary>
			[Display(Description="date")]
			Date = 1 << 1,
			/// <summary>
			/// Indicates the IsUpload property should be populated.
			/// </summary>
			[Display(Description="isUpload")]
			IsUpload = 1 << 2,
			/// <summary>
			/// Indicates the Member property should be populated.
			/// </summary>
			[Display(Description="idMember")]
			Member = 1 << 3,
			/// <summary>
			/// Indicates the MimeType property should be populated.
			/// </summary>
			[Display(Description="mimeType")]
			MimeType = 1 << 4,
			/// <summary>
			/// Indicates the Name property should be populated.
			/// </summary>
			[Display(Description="name")]
			Name = 1 << 5,
			/// <summary>
			/// Indicates the Previews property should be populated.
			/// </summary>
			[Display(Description="previews")]
			Previews = 1 << 6,
			/// <summary>
			/// Indicates the Url property should be populated.
			/// </summary>
			[Display(Description="url")]
			Url = 1 << 7,
			/// <summary>
			/// Indicates the EdgeColor property should be populated.
			/// </summary>
			[Display(Description = "edgeColor")]
			EdgeColor = 1 << 8,
			/// <summary>
			/// Indicates the Position property should be populated.
			/// </summary>
			[Display(Description = "pos")]
			Position = 1 << 9,
		}

		private readonly Field<int?> _bytes;
		private readonly Field<DateTime?> _date;
		private readonly Field<bool?> _isUpload;
		private readonly Field<Member> _member;
		private readonly Field<string> _mimeType;
		private readonly Field<string> _name;
		private readonly Field<string> _url;
		private readonly Field<Position> _position;
		private readonly Field<WebColor> _edgeColor;
		private readonly AttachmentContext _context;
		private DateTime? _creation;

		/// <summary>
		/// Specifies which fields should be downloaded.
		/// </summary>
		public static Fields DownloadedFields { get; set; } = (Fields)Enum.GetValues(typeof(Fields)).Cast<int>().Sum();

		/// <summary>
		/// Gets the size of the attachment in bytes.
		/// </summary>
		public int? Bytes => _bytes.Value;
		/// <summary>
		/// Gets the creation date of the attachment.
		/// </summary>
		public DateTime CreationDate
		{
			get
			{
				if (_creation == null)
					_creation = Id.ExtractCreationDate();
				return _creation.Value;
			}
		}
		/// <summary>
		/// Gets the date and time the attachment was added to a card.
		/// </summary>
		public DateTime? Date => _date.Value;
		/// <summary>
		/// Gets the color used as a border for the attachment preview on the card.
		/// </summary>
		public WebColor EdgeColor => _edgeColor.Value;
		/// <summary>
		/// Gets the attachment's ID.
		/// </summary>
		public string Id { get; private set; }
		/// <summary>
		/// Gets whether the attachment was uploaded data or attached by URI.
		/// </summary>
		public bool? IsUpload => _isUpload.Value;
		/// <summary>
		/// Gets the <see cref="Member"/> who added the attachment.
		/// </summary>
		public IMember Member => _member.Value;
		/// <summary>
		/// Gets the MIME type of the attachment.
		/// </summary>
		public string MimeType => _mimeType.Value;

		/// <summary>
		/// Gets or sets the name of the attachment.
		/// </summary>
		public string Name
		{
			get { return _name.Value; }
			set { _name.Value = value; }
		}
		/// <summary>
		/// Gets or sets the attachment's position.
		/// </summary>
		public Position Position
		{
			get { return _position.Value; }
			set { _position.Value = value; }
		}
		/// <summary>
		/// Gets the collection of previews generated by Trello.
		/// </summary>
		public IReadOnlyCollection<IImagePreview> Previews { get; }
		/// <summary>
		/// Gets the URI of the attachment.
		/// </summary>
		public string Url => _url.Value;

		internal IJsonAttachment Json
		{
			get { return _context.Data; }
			set { _context.Merge(value); }
		}

		/// <summary>
		/// Raised when data on the attachment is updated.
		/// </summary>
		public event Action<IAttachment, IEnumerable<string>> Updated;

		internal Attachment(IJsonAttachment json, string ownerId, TrelloAuthorization auth)
		{
			Id = json.Id;
			_context = new AttachmentContext(Id, ownerId, auth);
			_context.Synchronized += Synchronized;

			_bytes = new Field<int?>(_context, nameof(Bytes));
			_date = new Field<DateTime?>(_context, nameof(Date));
			_member = new Field<Member>(_context, nameof(Member));
			_edgeColor = new Field<WebColor>(_context, nameof(EdgeColor));
			_isUpload = new Field<bool?>(_context, nameof(IsUpload));
			_mimeType = new Field<string>(_context, nameof(MimeType));
			_name = new Field<string>(_context, nameof(Name));
			_name.AddRule(NotNullOrWhiteSpaceRule.Instance);
			_position = new Field<Position>(_context, nameof(Position));
			_position.AddRule(PositionRule.Instance);
			Previews = new ReadOnlyAttachmentPreviewCollection(_context, auth);
			_url = new Field<string>(_context, nameof(Url));

			TrelloConfiguration.Cache.Add(this);

			_context.Merge(json);
		}

		/// <summary>
		/// Permanently deletes the attachment from Trello.
		/// </summary>
		/// <remarks>
		/// This instance will remain in memory and all properties will remain accessible.
		/// </remarks>
		public async Task Delete()
		{
			await _context.Delete();
			TrelloConfiguration.Cache.Remove(this);
		}
		/// <summary>
		/// Returns the <see cref="Name"/>.
		/// </summary>
		/// <returns>
		/// A string that represents the attachment.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}

		private void Synchronized(IEnumerable<string> properties)
		{
			Id = _context.Data.Id;
			var handler = Updated;
			handler?.Invoke(this, properties);
		}
	}
}