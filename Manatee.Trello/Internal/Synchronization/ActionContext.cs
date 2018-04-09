using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manatee.Trello.Internal.Caching;
using Manatee.Trello.Internal.DataAccess;
using Manatee.Trello.Internal.Validation;
using Manatee.Trello.Json;

namespace Manatee.Trello.Internal.Synchronization
{
	internal class ActionContext : SynchronizationContext<IJsonAction>
	{
		private bool _deleted;

		public ActionDataContext ActionDataContext { get; }
		public virtual bool HasValidId => IdRule.Instance.Validate(Data.Id, null) == null;

		static ActionContext()
		{
			_properties = new Dictionary<string, Property<IJsonAction>>
				{
					{
						"Creator", new Property<IJsonAction, Member>((d, a) => d.MemberCreator.GetFromCache<Member>(a),
						                                             (d, o) => { if (o != null) d.MemberCreator = o.Json; })
					},
					{"Date", new Property<IJsonAction, DateTime?>((d, a) => d.Date, (d, o) => d.Date = o)},
					{"Id", new Property<IJsonAction, string>((d, a) => d.Id, (d, o) => d.Id = o)},
					{"Text", new Property<IJsonAction, string>((d, a) => d.Data.Text, (d, o) => d.Text = o)},
					{"Type", new Property<IJsonAction, ActionType?>((d, a) => d.Type, (d, o) => d.Type = o)},
				};
		}
		public ActionContext(string id, TrelloAuthorization auth)
			: base(auth)
		{
			Data.Id = id;
			ActionDataContext = new ActionDataContext(Auth);
			ActionDataContext.SynchronizeRequested += () => Synchronize();
			ActionDataContext.SubmitRequested += () => HandleSubmitRequested("Text");
			Data.Data = ActionDataContext.Data;
		}

		public async Task Delete()
		{
			if (_deleted) return;

			var endpoint = EndpointFactory.Build(EntityRequestType.Action_Write_Delete, new Dictionary<string, object> {{"_id", Data.Id}});
			await JsonRepository.Execute(Auth, endpoint);

			_deleted = true;
		}
		public override async Task Expire()
		{
			await ActionDataContext.Expire();
			await base.Expire();
		}

		protected override async Task<IJsonAction> GetData()
		{
			try
			{
				var endpoint = EndpointFactory.Build(EntityRequestType.Action_Read_Refresh, new Dictionary<string, object> {{"_id", Data.Id}});
				var newData = await JsonRepository.Execute<IJsonAction>(Auth, endpoint);
				MarkInitialized();

				return newData;
			}
			catch (TrelloInteractionException e)
			{
				if (!e.IsNotFoundError() || !IsInitialized) throw;
				_deleted = true;
				return Data;
			}
		}
		protected override async Task SubmitData(IJsonAction json)
		{
			var endpoint = EndpointFactory.Build(EntityRequestType.Action_Write_Update, new Dictionary<string, object> {{"_id", Data.Id}});
			var newData = await JsonRepository.Execute(Auth, endpoint, json);

			Merge(newData);
			Data.Data = ActionDataContext.Data;
		}
		protected override void ApplyDependentChanges(IJsonAction json)
		{
			Data.Data = ActionDataContext.Data;
			if (ActionDataContext.HasChanges)
			{
				json.Data = ActionDataContext.GetChanges();
				ActionDataContext.ClearChanges();
			}
		}
		protected override IEnumerable<string> MergeDependencies(IJsonAction json)
		{
			return ActionDataContext.Merge(json.Data);
		}
		protected override bool CanUpdate()
		{
			return !_deleted;
		}
	}
}