namespace RetailSphere.Domain.IdentityAccess;

/// <summary>
/// Single source of truth for every permission code the system understands.
/// Permissions are seeded reference data (§7 of the architecture doc) — the
/// development seeder inserts exactly this list and grants all of it to the
/// "Super Admin" role. New modules add their permission codes here as they're
/// built, and the Sidebar's `HasPermission(...)` checks stay in sync by
/// construction since both read from the same codes.
/// </summary>
public static class PermissionCatalog
{
    public sealed record Entry(long Id, string Code, string DisplayName, string Module);

    public static readonly IReadOnlyList<Entry> All =
    [
        // Administration
        new(1, "admin.users.view", "View Users", "Administration"),
        new(2, "admin.users.create", "Create Users", "Administration"),
        new(3, "admin.users.edit", "Edit Users", "Administration"),
        new(4, "admin.users.deactivate", "Activate/Deactivate Users", "Administration"),
        new(5, "admin.roles.view", "View Roles", "Administration"),
        new(6, "admin.roles.create", "Create Roles", "Administration"),
        new(7, "admin.roles.edit", "Edit Roles", "Administration"),
        new(8, "admin.roles.delete", "Delete Roles", "Administration"),
        new(9, "admin.branches.view", "View Branches", "Administration"),
        new(10, "admin.branches.create", "Create Branches", "Administration"),
        new(11, "admin.branches.edit", "Edit Branches", "Administration"),
        new(12, "admin.branches.deactivate", "Activate/Deactivate Branches", "Administration"),
        new(13, "admin.audit.view", "View Audit Logs", "Administration"),

        // Catalog
        new(20, "catalog.products.view", "View Products", "Catalog"),
        new(21, "catalog.products.edit", "Edit Products", "Catalog"),
        new(22, "catalog.categories.view", "View Categories", "Catalog"),
        new(23, "catalog.categories.edit", "Edit Categories", "Catalog"),
        new(24, "catalog.brands.view", "View Brands", "Catalog"),
        new(25, "catalog.brands.edit", "Edit Brands", "Catalog"),
        new(26, "catalog.units.view", "View Units", "Catalog"),
        new(27, "catalog.units.edit", "Edit Units", "Catalog"),

        // Inventory
        new(30, "inventory.stock.view", "View Branch Stock", "Inventory"),
        new(31, "inventory.transfer.view", "View Transfers", "Inventory"),
        new(32, "inventory.transfer.approve", "Approve Transfers", "Inventory"),
        new(33, "inventory.adjustment.view", "View Adjustments", "Inventory"),
        new(34, "inventory.adjustment.create", "Create Adjustments", "Inventory"),

        // Purchasing
        new(40, "purchasing.suppliers.view", "View Suppliers", "Purchasing"),
        new(41, "purchasing.suppliers.edit", "Edit Suppliers", "Purchasing"),
        new(42, "purchasing.orders.view", "View Purchase Orders", "Purchasing"),
        new(43, "purchasing.orders.create", "Create Purchase Orders", "Purchasing"),
        new(44, "purchasing.orders.edit", "Edit Purchase Orders", "Purchasing"),
        new(45, "purchasing.orders.receive", "Receive Purchase Orders", "Purchasing"),

        // Sales
        new(50, "sales.pos.create", "Use Point of Sale", "Sales"),
        new(51, "sales.orders.view", "View Sales Orders", "Sales"),
        new(52, "sales.returns.view", "View Returns & Exchanges", "Sales"),
        new(53, "sales.returns.create", "Process Returns & Exchanges", "Sales"),

        // Customers
        new(60, "customers.view", "View Customers", "Customers"),
        new(61, "customers.edit", "Edit Customers", "Customers"),

        // Finance
        new(70, "finance.cashregister.view", "View Cash Register", "Finance"),
        new(71, "finance.cashregister.operate", "Open/Close Cash Register", "Finance"),
        new(72, "reporting.view", "View Reports", "Finance"),
    ];
}
