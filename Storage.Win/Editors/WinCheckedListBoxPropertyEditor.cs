using System;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Storage.Module.BusinessObjects;

namespace Storage.Win.Editors;

[PropertyEditor(typeof(IList<Picket>), false)]
public class WinCheckedListBoxPropertyEditor : WinPropertyEditor, IComplexViewItem
{
	public WinCheckedListBoxPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
	protected override object CreateControlCore()
	{
		return new CheckedListBoxControl();
	}
	IList<Picket> checkedItems;
	XafApplication application;
	IObjectSpace objectSpace;
	protected override void ReadValueCore()
	{
		base.ReadValueCore();

		var currentObject = CurrentObject as Area;

		if (currentObject.Storage == null)
			return;
		if (PropertyValue is not IList<Picket>)
			return;

		Control.ItemCheck -= new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(control_ItemCheck);

		// ������ ��������
		checkedItems = PropertyValue as IList<Picket>;

		// ������� ������, �� �������� � ��������� �����
		foreach (var item in checkedItems.ToList())
			if (item.Storage.ID != currentObject.Storage.ID)
				checkedItems.Remove(item);

		// ��� ��������� ������ �� ������
		var allItems = objectSpace.GetObjects<Picket>()
			.Where(x => x.Storage?.ID == currentObject.Storage.ID &&
						(x.Area == null || x.Area.ID == currentObject.ID))
			.OrderBy(x => x.Number)
			.ToList();

		// ���������� ��������� � �������
		Control.DataSource = allItems;

		// ����������� �������� �� ���������
		IModelClass classInfo = application.Model.BOModel.GetClass(MemberInfo.ListElementTypeInfo.Type);
		Control.DisplayMember = classInfo.DefaultProperty;

		// ��������� ������� � ��������
		foreach (var item in checkedItems)
			Control.SetItemChecked(allItems.IndexOf(item), true);

		Control.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(control_ItemCheck);
	}

	void control_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
	{
		var item = Control.GetItemValue(e.Index) as Picket;

		// ���������� ������ ��������� �������
		switch (e.State)
		{
			case CheckState.Checked:
				checkedItems.Add(item);
				break;
			case CheckState.Unchecked:
				checkedItems.Remove(item);
				break;
		}
		OnControlValueChanged();
		objectSpace.SetModified(CurrentObject);
	}

	private void ObjectChanged(object sender, ObjectChangedEventArgs args)
	{
		// ���� ��������� �����
		if (args.Object is Area)
		{
			if (args.MemberInfo?.Name == nameof(Area.Storage))
			{
				// ��������� ������ �������
				ReadValueCore();
			}
		}
	}

	public new CheckedListBoxControl Control
	{
		get
		{
			return (CheckedListBoxControl)base.Control;
		}
	}

	#region IComplexPropertyEditor Members

	public void Setup(IObjectSpace objectSpace, XafApplication application)
	{
		this.application = application;
		this.objectSpace = objectSpace;
		objectSpace.ObjectChanged += ObjectChanged;
	}

	#endregion
}
