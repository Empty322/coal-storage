using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Storage.Module.BusinessObjects
{
	/// <summary>
	/// Сущность склада
	/// </summary>
	[DefaultClassOptions]
	public class Storage : BaseObject
	{
		public Storage()
		{
		}
		
		/// <summary>
		/// Название склада
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// Площадки, находящиеся на складе
		/// </summary>
		public virtual IList<Area> Areas { get; set; } = new ObservableCollection<Area>();

		/// <summary>
		/// Пикеты, находящиеся на складе
		/// </summary>
		public virtual IList<Picket> Pickets { get; set; } = new ObservableCollection<Picket>();
	}
}