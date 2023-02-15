using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EFCore.AuditTrail;
using DevExpress.Persistent.Validation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Module.BusinessObjects
{
	/// <summary>
	/// Сущность площадки
	/// </summary>
	[DefaultClassOptions]
	[DefaultProperty("Name")]
	[RuleCriteria(DefaultContexts.Save, "Pickets.Count() = 0", InvertResult = true, 
		CustomMessageTemplate = "Area must contain at least 1 picket.")]
	[RuleCriteria(DefaultContexts.Save, "(Pickets.Max(Number) - Pickets.Min(Number) + 1) = Pickets.Count()", 
		CustomMessageTemplate = "Pickets should not be separated.")]
	public class Area : BaseObject
	{
		/// <summary>
		/// Склад, на котором назодится данная площадка
		/// </summary>
		public virtual Storage Storage { get; set; }

		/// <summary>
		/// Пикеты, которые входят в площадку
		/// </summary>
		public virtual IList<Picket> Pickets { get; set; } = new ObservableCollection<Picket>();

		/// <summary>
		/// Груз на площадке (т)
		/// </summary>
		[DisplayName("Weight(tons)")]
		public virtual float Weight { get; set; }

		/// <summary>
		/// Название площадки
		/// </summary>
		[NotMapped]
		public string Name { get => 
				Pickets.Any() ? Pickets.Select(x => x.Number).Min() + " - " + Pickets.Select(x => x.Number).Max() : "Pickets not selected"; }

		/// <summary>
		/// История изменений площадки
		/// </summary>
		[CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
		[NotMapped]
		public virtual IList<AuditDataItemPersistent> ChangeHistory
		{
			get { return AuditDataItemPersistent.GetAuditTrail(ObjectSpace, this); }
		}
	}
}