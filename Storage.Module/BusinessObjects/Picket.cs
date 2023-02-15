using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;
using System;
using System.ComponentModel;

namespace Storage.Module.BusinessObjects
{
	/// <summary>
	/// �������� ������
	/// </summary>
	[DefaultClassOptions]
	[DefaultProperty(nameof(Number))]
	public class Picket : BaseObject
	{
		/// <summary>
		/// ����� ������
		/// </summary>
		[RuleUniqueValue(DefaultContexts.Save, CustomMessageTemplate = "Picket number must be unique.")]
		public virtual int Number { get; set; }

		/// <summary>
		/// �����, �� ������� ��������� �����
		/// </summary>
		[RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = nameof(Storage) + " must be selected.")]
		public virtual Storage Storage { get; set; }

		/// <summary>
		/// ��������, � ������� ������ �����
		/// </summary>
		[VisibleInDetailView(false)]
		[VisibleInListView(false)]
		public virtual Area Area { get; set; }

		[DisplayName("Area")]
		public Area ReadonlyArea { get => Area; }
	}
}