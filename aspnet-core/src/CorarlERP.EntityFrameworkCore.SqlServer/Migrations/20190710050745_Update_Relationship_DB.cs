using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_Relationship_DB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                
                migrationBuilder.Sql(@" INSERT INTO CarlErpLocations (LocationName,LocationParent, CreationTime, CreatorUserId,  TenantId, IsActive) 
                                        SELECT N'My Default',0, GETUTCDATE(), NULL, a.Id, 1 
                                        FROM AbpTenants a");

                //update all data in location into lots
                migrationBuilder.Sql(@" INSERT INTO CarlErpLots (LotName, CreationTime, CreatorUserId, TenantId, IsActive, LocationId) 
                                        SELECT l.LocationName, GETUTCDATE(), NULL, l.TenantId, 1, 
                                        (SELECT TOP 1 Id FROM CarlErpLocations WHERE LocationName = N'My Default' AND TenantId = l.TenantId ORDER BY Id DESC)
                                        FROM AbpTenants a 
                                        INNER JOIN CarlErpLocations l ON a.Id = l.TenantId
                                        WHERE l.LocationName != N'My Default'");

               // update bill items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpBillItems SET 
                                            LotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpBills b 
                                        ON a.Id = b.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON b.LocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpBillItems.BillId = b.Id 
                                        AND lo.TenantId = a.Id");
                
                migrationBuilder.Sql(@"UPDATE CarlErpBills SET 
                                     LocationId = l.Id
                                    FROM CarlErpLocations l 
			                        WHERE 
									l.LocationName = N'My Default' AND
									CarlErpBills.TenantId = l.TenantId ");



                // update itemReceipt items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpItemReceiptItems SET 
                                            LotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpItemReceipts ir 
                                        ON a.Id = ir.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON ir.LocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpItemReceiptItems.ItemReceiptId = ir.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpItemReceipts SET 
                                        LocationId = l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default'  AND
									CarlErpItemReceipts.TenantId = l.TenantId");


                // update invoice items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpInvoiceItems SET 
                                            LotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpInvoices inv 
                                        ON a.Id = inv.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON inv.LocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpInvoiceItems.InvoiceId = inv.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpInvoices SET 
                                        LocationId = l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default'  AND
									CarlErpInvoices.TenantId = l.TenantId");

                // update item issue items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpItemIssueItems SET 
                                            LotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpItemIssues iss 
                                        ON a.Id = iss.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON iss.LocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpItemIssueItems.ItemIssueId = iss.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpItemIssues SET 
                                        LocationId = l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default' AND
									CarlErpItemIssues.TenantId = l.TenantId");


                // update item issue vendorcredit items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpItemIssueVendorCreditItem SET 
                                            LotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpItemIssueVendorCredit isv 
                                        ON a.Id = isv.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON isv.LocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpItemIssueVendorCreditItem.ItemIssueVendorCreditId = isv.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpItemIssueVendorCredit SET 
                                        LocationId = l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default' AND
									CarlErpItemIssueVendorCredit.TenantId = l.TenantId");

                // update item receipt customercredit items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpItemReceiptCustomerCreditItem SET 
                                            LotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpItemReceiptCustomerCredit irc 
                                        ON a.Id = irc.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON irc.LocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId = irc.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpItemReceiptCustomerCredit SET 
                                        LocationId = l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default'  AND
									CarlErpItemReceiptCustomerCredit.TenantId = l.TenantId");


                // update  customercredit items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpCusotmerCreditDetails SET 
                                            LotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpCustomerCredits cc 
                                        ON a.Id = cc.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON cc.LocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpCusotmerCreditDetails.CustomerCreditId = cc.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpCustomerCredits SET 
                                        LocationId = l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default' AND
									CarlErpCustomerCredits.TenantId = l.TenantId");


                // update  vendorcredit items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpVendorCreditDetails SET 
                                            LotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpVendorCredit vc 
                                        ON a.Id = vc.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON vc.LocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpVendorCreditDetails.VendorCreditId = vc.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpVendorCredit SET 
                                        LocationId = l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default' AND
									CarlErpVendorCredit.TenantId = l.TenantId");


                // update  transfer order items location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpTransferOrderItems SET 
                                            FromLotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpTransferOrders tro 
                                        ON a.Id = tro.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON tro.TransferFromLocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpTransferOrderItems.TransferOrderId = tro.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpTransferOrderItems SET 
                                            ToLotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpTransferOrders tro 
                                        ON a.Id = tro.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON tro.TransferToLocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpTransferOrderItems.TransferOrderId = tro.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@"UPDATE CarlErpTransferOrders SET 
                                        TransferFromLocationId = l.Id,
                                        TransferToLocationId= l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default' AND
									CarlErpTransferOrders.TenantId = l.TenantId");


                // update  production order location id by into lotid and location id to default id 
                migrationBuilder.Sql(@" UPDATE CarlErpRawMaterialItems SET 
                                            FromLotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpTransProductions rm 
                                        ON a.Id = rm.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON rm.FromLocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpRawMaterialItems.ProductionId = rm.Id
                                        AND lo.TenantId = a.Id");

                migrationBuilder.Sql(@" UPDATE CarlErpFinishItems SET 
                                            ToLotId = lo.Id
                                        FROM AbpTenants a 
			                            INNER JOIN CarlErpTransProductions fi 
                                        ON a.Id = fi.TenantId 
			                            INNER JOIN CarlErpLocations l 
			                            ON fi.ToLocationId = l.Id
			                            INNER JOIN CarlErpLots lo
			                            ON l.LocationName = lo.LotName
			                            WHERE CarlErpFinishItems.ProductionId = fi.Id 
                                        AND lo.TenantId = a.Id");


                migrationBuilder.Sql(@"UPDATE CarlErpTransProductions SET 
                                        ToLocationId = l.Id,
										FromLocationId = l.Id
                                    FROM AbpTenants a 
			                        INNER JOIN CarlErpLocations l 
                                    ON a.Id = l.TenantId 
			                        WHERE l.LocationName = N'My Default' AND
									CarlErpTransProductions.TenantId = l.TenantId");

                //Update table tenant company profile
                migrationBuilder.Sql(@"UPDATE AbpTenants SET 
                                     LocationId = l.Id
                                    FROM CarlErpLocations l 
			                        WHERE l.LocationName = N'My Default' 
									AND AbpTenants.Id = l.TenantId ");
                // final delete all record in location which is not default after update 
                migrationBuilder.Sql(@"DELETE FROM CarlErpLocations
			                        WHERE LocationName != N'My Default'");


            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
