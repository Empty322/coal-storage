using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using System.Runtime.Intrinsics.X86;
using Storage.Module.BusinessObjects;
using DevExpress.Persistent.BaseImpl.EFCore.AuditTrail;
using System.Security.Cryptography.X509Certificates;

namespace Storage.Module.DatabaseUpdate;

enum RoleName
{
	Admin,
	User
}

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();

		AddInitialData();

        AddRolesAndUsers();

	}

	public override void UpdateDatabaseBeforeUpdateSchema()
	{
		base.UpdateDatabaseBeforeUpdateSchema();
	}

	private void AddInitialData()
	{
		// Добавление первого склада
		var storage1 = ObjectSpace.FirstOrDefault<BusinessObjects.Storage>(storage => storage.Name == "Storage 1");
		if (storage1 == null)
		{
			storage1 = ObjectSpace.CreateObject<BusinessObjects.Storage>();
			storage1.Name = "Storage 1";

			var picket101 = ObjectSpace.CreateObject<Picket>();
			picket101.Number = 101;
			picket101.Storage = storage1;
			var picket102 = ObjectSpace.CreateObject<Picket>();
			picket102.Number = 102;
			picket102.Storage = storage1;
			var picket103 = ObjectSpace.CreateObject<Picket>();
			picket103.Number = 103;
			picket103.Storage = storage1;
			var picket104 = ObjectSpace.CreateObject<Picket>();
			picket104.Number = 104;
			picket104.Storage = storage1;
			var picket105 = ObjectSpace.CreateObject<Picket>();
			picket105.Number = 105;
			picket105.Storage = storage1;

			var area1 = ObjectSpace.CreateObject<Area>();
			area1.Storage = storage1;
			area1.Pickets.Add(picket101);
			area1.Pickets.Add(picket102);
			area1.Pickets.Add(picket103);
			area1.Pickets.Add(picket104);
			area1.Weight = 53000;

			var area2 = ObjectSpace.CreateObject<Area>();
			area2.Storage = storage1;
			area2.Pickets.Add(picket105);
			area2.Weight = 5000;
		}


		// Добавление второго склада
		var storage2 = ObjectSpace.FirstOrDefault<BusinessObjects.Storage>(storage => storage.Name == "Storage 2");
		if (storage2 == null)
		{
			storage2 = ObjectSpace.CreateObject<BusinessObjects.Storage>();
			storage2.Name = "Storage 2";

			var picket201 = ObjectSpace.CreateObject<Picket>();
			picket201.Number = 201;
			picket201.Storage = storage2;
			var picket202 = ObjectSpace.CreateObject<Picket>();
			picket202.Number = 202;
			picket202.Storage = storage2;
			var picket203 = ObjectSpace.CreateObject<Picket>();
			picket203.Number = 203;
			picket203.Storage = storage2;
			var picket204 = ObjectSpace.CreateObject<Picket>();
			picket204.Number = 204;
			picket204.Storage = storage2;
			var picket205 = ObjectSpace.CreateObject<Picket>();
			picket205.Number = 205;
			picket205.Storage = storage2;

			var area1 = ObjectSpace.CreateObject<Area>();
			area1.Storage = storage2;
			area1.Pickets.Add(picket201);
			area1.Pickets.Add(picket202);
			area1.Weight = 8000;

			var area2 = ObjectSpace.CreateObject<Area>();
			area2.Storage = storage2;
			area2.Pickets.Add(picket203);
			area2.Pickets.Add(picket204);
			area2.Pickets.Add(picket205);
			area2.Weight = 15000;
		}

		ObjectSpace.CommitChanges();
	}

	private void AddRolesAndUsers()
    {
		// Добавление роли администратора
		PermissionPolicyRole adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == RoleName.Admin.ToString());
		if (adminRole == null)
		{
			adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
			adminRole.Name = RoleName.Admin.ToString();
			adminRole.IsAdministrative = true;
		}

		// Добавление роли пользователя
		PermissionPolicyRole userRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == RoleName.User.ToString());
		if (userRole == null)
		{
			userRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
			userRole.Name = RoleName.User.ToString();
			userRole.PermissionPolicy = SecurityPermissionPolicy.ReadOnlyAllByDefault;

			// Скрыть вкладку User
			userRole.AddNavigationPermission("User", SecurityPermissionState.Deny);
			// Allow CRUD for Area type
			userRole.AddTypePermission<Area>("Write;Delete;Navigate;Create", SecurityPermissionState.Allow);
			// Запретить доступ к ролям
			userRole.AddTypePermission<PermissionPolicyRole>(SecurityOperations.FullAccess, SecurityPermissionState.Deny);
			// Запретить доступ к пользователям
			userRole.AddTypePermission<PermissionPolicyUser>(SecurityOperations.FullAccess, SecurityPermissionState.Deny);


			userRole.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.Read, SecurityPermissionState.Allow);
			userRole.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.Create, SecurityPermissionState.Allow);
			userRole.AddTypePermission<AuditEFCoreWeakReference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
			userRole.AddTypePermission<AuditEFCoreWeakReference>(SecurityOperations.Create, SecurityPermissionState.Allow);
		}

		// Добавление пользователей
		PermissionPolicyUser admin = ObjectSpace.FirstOrDefault<PermissionPolicyUser>(u => u.UserName == "admin");
		if (admin == null)
		{
			admin = ObjectSpace.CreateObject<PermissionPolicyUser>();
			admin.UserName = "admin";
			admin.SetPassword("admin");
		}
		PermissionPolicyUser user = ObjectSpace.FirstOrDefault<PermissionPolicyUser>(user => user.UserName == "user");
		if (user == null)
		{
			user = ObjectSpace.CreateObject<PermissionPolicyUser>();
			user.UserName = "user";
			user.SetPassword("");
		}

		// Присвоение ролей пользователям
		admin.Roles.Add(adminRole);
		user.Roles.Add(userRole);

		ObjectSpace.CommitChanges();
	}
}
