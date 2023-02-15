using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Module.BusinessObjects
{
	/// <summary>
	/// Non-persistent сущность площадки для представления состояния в конкретное время
	/// </summary>
	[DomainComponent]
	public class AreaSnapshot
	{
		[VisibleInListView(false)]
		[VisibleInDetailView(false)]
		public Guid ID { get; set; }

		/// <summary>
		/// Название площадки
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Груз на площадке (т)
		/// </summary>
		public float Weight { get; set; }

	}
}
