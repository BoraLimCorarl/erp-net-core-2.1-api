namespace CorarlERP.Authorization.Roles
{
    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";
        }

        public static class Tenants
        {
            public const string Admin = "Admin";
            public const string AccountingManager = "Accounting Manager";
            public const string APAccountant = "AP Accountant";
            public const string ARAccountant = "AR Accountant";
            public const string SaleManager = "Sale Manager";
            public const string PurchaseManager = "Purchase Manager";
            public const string WarehouseManager = "Warehouse Manager";
            public const string StockController = "Stock Controller";
            // public const string User = "User";
        }
    }
}