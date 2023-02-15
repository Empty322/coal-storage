using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Module.BusinessObjects
{
	/// <summary>
	/// Non-persistent сущность склада для представления состояния в конкретное время
	/// </summary>
	[DomainComponent]
	public class StorageSnapshot
	{
		/// <summary>
		/// Название склада
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Площадки, находящиеся на складе
		/// </summary>
		public IList<AreaSnapshot> Areas { get; set; } = new ObservableCollection<AreaSnapshot>();
	}
}
