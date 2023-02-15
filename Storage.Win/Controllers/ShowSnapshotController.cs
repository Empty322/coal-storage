using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EFCore.AuditTrail;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using Storage.Module.BusinessObjects;
using System;

namespace Storage.Module.Controllers
{
	/// <summary>
	/// Контроллер, добавляющий возможность показать состояние склада на указанную дату и время
	/// </summary>
	public partial class ShowSnapshotController : ViewController
	{
		public ShowSnapshotController() : base()
		{
			//Activate the controller only in the DetailView.
			TargetViewType = ViewType.DetailView;
			//Activate the controller only for root Views.
			TargetViewNesting = Nesting.Root;
			//Specify the type of objects that can use the controller.
			TargetObjectType = typeof(BusinessObjects.Storage);

			// Добавление действия с параметром
			ParametrizedAction showSnapshotAction = new ParametrizedAction(this, "ShowSnapshot", PredefinedCategory.View, typeof(DateTime))
			{
				ImageName = "BO_Audit_ChangeHistory",
			};
			showSnapshotAction.CustomizeControl += CustomizeShowSnapshot;
			showSnapshotAction.Execute += ShowSnapshotAction_Execute;
		}

		private void CustomizeShowSnapshot(object sender, CustomizeControlEventArgs e)
		{
			// Изменение параметров элемента панели инструментов
			var barItem = e.Control as BarEditItem;
			if (barItem != null)
			{
				RepositoryItemDateEdit repositoryItem = (RepositoryItemDateEdit)barItem.Edit;
				// Изменение маски для ввода
				repositoryItem.Mask.UseMaskAsDisplayFormat = true;
				repositoryItem.Mask.EditMask = "dd-MM-yyyy HH:mm";
				// Изменение ширины элемента
				barItem.Width = 270;
			}
		}

		private void ShowSnapshotAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
		{
			if (e.ParameterCurrentValue == null)
				return;

			DateTime selectedDate = (DateTime)e.ParameterCurrentValue;
			var currentObject = (BusinessObjects.Storage)View.CurrentObject;

			// Создание пространства объектов для восстановления в нем старого состояния склада
			var nonPersistentOS = Application.CreateObjectSpace(typeof(StorageSnapshot));
			
			// Создание объекта склада в текущем состоянии
			var storageSnapshot = nonPersistentOS.CreateObject<StorageSnapshot>();
			storageSnapshot.Name = currentObject.Name;
			storageSnapshot.Areas = currentObject.Areas.Select(x => new AreaSnapshot
			{
				ID = x.ID,
				Name = x.Name,
				Weight = x.Weight,
			}).ToList();

			// Восстановление состояния склада на выбранную дату
			// Откат модификаций коллекции Areas у склада
			var storageReference = ObjectSpace.FindObject<AuditEFCoreWeakReference>(CriteriaOperator.Parse("[Key] == ?", currentObject.ID.ToString()));
			if (storageReference != null)
			{
				var storageModifications = storageReference.AuditItems.Where(x => x.ModifiedOn > selectedDate).OrderByDescending(x => x.ModifiedOn);
				foreach (var modification in storageModifications)
				{
					if (modification.PropertyName == "Areas")
					{
						switch (modification.OperationType)
						{
							case "RemovedFromCollection":
								var item = new AreaSnapshot
								{
									ID = Guid.Parse(modification.OldObject.Key),
									Name = modification.OldObject.DefaultString,
								};
								storageSnapshot.Areas.Add(item);
								break;
							case "AddedToCollection":
								var areaToDelete = storageSnapshot.Areas.SingleOrDefault(x => x.ID.ToString() == modification.NewObject.Key);
								storageSnapshot.Areas.Remove(areaToDelete);
								break;
							default:
								break;
						}
					}
				}
			}

			// Откат модификаций свойства Weight у площадок склада
			foreach (var area in storageSnapshot.Areas)
			{
				var areaReference = ObjectSpace.FindObject<AuditEFCoreWeakReference>(CriteriaOperator.Parse("[Key] == ?", area.ID.ToString()));
				if (areaReference != null)
				{
					var modification = areaReference.AuditItems.Where(x => x.PropertyName == "Weight(tons)" && x.ModifiedOn < selectedDate).LastOrDefault();
					if (modification != null)
						area.Weight = float.Parse(modification.NewValue);
					else
						area.Weight = 0;
				}
			}

			nonPersistentOS.CommitChanges();

			// Создание представления деталей только для чтения
			DetailView detailView = Application.CreateDetailView(nonPersistentOS, storageSnapshot, true);
			detailView.Model.AllowNew = false;
			detailView.Model.AllowEdit = false;
			detailView.Model.AllowDelete = false;
			e.ShowViewParameters.CreatedView = detailView;
		}

		protected override void OnActivated()
		{
			base.OnActivated();
		}

		protected override void OnDeactivated()
		{
			// Unsubscribe from previously subscribed events and release other references and resources.
			base.OnDeactivated();
		}
	}
}
