using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using POLine = PX.Objects.PO.POLine;
using POOrder = PX.Objects.PO.POOrder;
using System.Threading.Tasks;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AR.CCPaymentProcessing;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.AR.CCPaymentProcessing.Interfaces;
using ARRegisterAlias = PX.Objects.AR.Standalone.ARRegisterAlias;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Objects.Common.Extensions;
using PX.Objects.IN.Overrides.INDocumentRelease;
using PX.CS.Contracts.Interfaces;
using PX.Data.DependencyInjection;
using PX.Data.WorkflowAPI;
using PX.Objects.Extensions.PaymentTransaction;
using PX.Objects.SO.GraphExtensions.CarrierRates;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;
using PX.Objects.SO.Attributes;
using PX.Objects.Common.Attributes;
using PX.Objects.Common.Bql;
using OrderActions = PX.Objects.SO.SOOrderEntryActionsAttribute;
using PX.Objects.SO.DAC.Projections;
using PX.Data.BQL.Fluent;
using PX.Objects;
using PX.Objects.SO;
using PC;
using PrecisionCust;
using PX.Data.BQL;

namespace PX.Objects.SO
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class SOOrderEntry_ExtensionPC : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        private const string Amount = "A";
        private const string Percent = "P";
        public PXSelectJoin<Rebate, LeftJoin<RebateCustomer, On<Rebate.rebateNbr, Equal<RebateCustomer.rebateNbr>>>,
            Where<Rebate.active, Equal<boolTrue>, And2<Where<Rebate.effectiveTo, GreaterEqual<Current<AccessInfo.businessDate>>,
                    Or<Rebate.effectiveTo, IsNull>>, And<Where<Rebate.effectiveFrom, LessEqual<Current<AccessInfo.businessDate>>,
                        And<RebateCustomer.bAccountID, Equal<Current<SOOrder.customerID>>>>>>>> ActiveRebates;

        public PXSelectJoin<Rebate, LeftJoin<RebatePriceClass, On<Rebate.rebateNbr, Equal<RebatePriceClass.rebateNbr>>>,
            Where<Rebate.active, Equal<boolTrue>, And2<Where<Rebate.effectiveTo, GreaterEqual<Current<AccessInfo.businessDate>>,
                    Or<Rebate.effectiveTo, IsNull>>, And<Where<Rebate.effectiveFrom, LessEqual<Current<AccessInfo.businessDate>>,
                        And<RebatePriceClass.priceClassID, Equal<Current<Location.cPriceClassID>>>>>>>> ActiveRebatebyPriceClass;

        public PXSetup<APSetup> APSetupView;

        public delegate IEnumerable ReleaseFromHoldDelegate(PXAdapter adapter);


        //#region Override Actions
        //[PXOverride]
        //public IEnumerable ReleaseFromHold(PXAdapter adapter, ReleaseFromHoldDelegate baseMethod)
        //{

        //    return baseMethod(adapter);
        //}
        //#endregion


        #region Event Handlers
        protected void _(Events.RowSelected<SOLine> e)
        {
            SOLine row = e.Row;
            PXCache cache = e.Cache;

            SOLineExtPC lineExt = row.GetExtension<SOLineExtPC>();
            APSetup setupAP = APSetupView.Current;
            APSetupExtPC setupExt = setupAP.GetExtension<APSetupExtPC>();

            if(setupAP != null && lineExt !=null)
            {
                if (lineExt.UsrRebateNbr != null && setupExt.UsrRebateCostType != lineExt.UsrCostType)
                {
                    // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                    cache.RaiseExceptionHandling<SOLine.curyUnitCost>(row, row.CuryUnitCost,
                        new PXSetPropertyException("Rebate Cost type applied to this line does not match the current setup.", PXErrorLevel.Warning));
                }
            }
          
        }

        protected void _(Events.RowPersisted<SOLine> e)
        {
            //SOLine row = e.Row;

            //MarkupPricing markUpPricing = PXSelect<MarkupPricing, Where<MarkupPricing.customerID, Equal<Required<MarkupPricing.customerID>>,
            //         And<Where<MarkupPricing.effectiveDate, LessEqual<Required<MarkupPricing.effectiveDate>>,
            //         And<Where<MarkupPricing.replacementCost, Equal<Required<MarkupPricing.replacementCost>>>>>>>>
            //         .Select(Base, row.CustomerID, Base.Accessinfo.BusinessDate, 0);

            //if (markUpPricing != null)
            //{
            //    if (Base.Transactions.Ask(message: $"The replacement cost is zero. you want to proceed?", MessageButtons.OKCancel) != WebDialogResult.Cancel) { }
            //    else
            //        return;
            //}
        }

        protected void _(Events.FieldUpdated<SOLine, SOLine.curyUnitPrice> e)
        {
            SOLine row = e.Row;
            PXCache cache = e.Cache;
            if (row == null) return;
            SOLineExtPC lineExt = row.GetExtension<SOLineExtPC>();

            //if (e.ExternalCall == true)
            //{
                if (lineExt.UsrRebateNbr != null)
                {
                    RebateItemLine line = PXSelect<RebateItemLine, Where<RebateItemLine.rebateNbr, Equal<Required<RebateItemLine.rebateNbr>>,
                        And<Where<RebateItemLine.inventoryID, Equal<Required<RebateItemLine.inventoryID>>,
                        And<Where<RebateItemLine.uom, Equal<Required<RebateItemLine.uom>>,
                        And<Where<RebateItemLine.siteID, Equal<Required<RebateItemLine.siteID>>>>>>>>>>
                        .Select(Base, lineExt.UsrRebateNbr, row.InventoryID, row.UOM, row.SiteID);

                    if(line != null)
                    {
                        if(line.RebatePrice != row.CuryUnitPrice)
                        {
                            cache.SetValueExt<SOLineExtPC.usrChangedRebate>(row, true);
                        }
                        else
                        {
                            cache.SetValueExt<SOLineExtPC.usrChangedRebate>(row, false);
                        }
                    }
                }
                if(lineExt.UsrMarkupID != null)
                {
                    decimal? replacementCostVal = 0m;
                    decimal? salesOrderPrice = 0m;
                    MarkupPricing mrkUpPricing = SelectFrom<MarkupPricing>.
                        Where<MarkupPricing.markupID.IsEqual<@P.AsInt>>.View.Select(Base,lineExt.UsrMarkupID);
                    if(mrkUpPricing != null)
                    {
                        if(mrkUpPricing.SalesOrderPrice != 0m)
                        {
                            if(row.CuryUnitPrice != mrkUpPricing.SalesOrderPrice)
                            {
                                cache.SetValueExt<SOLineExtPC.usrChangedRebate>(row, true);
                            }
                            else
                            {
                                cache.SetValueExt<SOLineExtPC.usrChangedRebate>(row, false);
                            }
                        }
                        else
                        {
                            InventoryItem markUpItem = SelectFrom<InventoryItem>.
                                                Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.Select(Base, row.InventoryID);
                            if(markUpItem != null)
                            {
                                InventoryItemCurySettings invCurSet = SelectFrom<InventoryItemCurySettings>.
                                                            Where<InventoryItemCurySettings.inventoryID.IsEqual<@P.AsInt>>.View.Select(Base, row.InventoryID);
                                var invCurSetExt = PXCache<InventoryItemCurySettings>.GetExtension<InventoryItemCurySettingsExtPC>(invCurSet);
                                if (!String.IsNullOrEmpty(invCurSetExt.UsrReplacementCst))
                                {
                                    if (invCurSetExt.UsrReplacementCst == MarkupMaint.LastPrice)
                                    {
                                        INItemCost itemCst = SelectFrom<INItemCost>.
                                            Where<INItemCost.inventoryID.IsEqual<@P.AsInt>.
                                            And<INItemCost.curyID.IsEqual<@P.AsString>>>.View.Select(Base, invCurSet.InventoryID, invCurSet.CuryID); // get last price from Stock Items Last Cost
                                        if (itemCst != null)
                                        {
                                            replacementCostVal = itemCst.LastCost;
                                        }
                                    }
                                    else if (invCurSetExt.UsrReplacementCst == MarkupMaint.AverageCost && row.SiteID != null)
                                    {
                                        INItemSite itemSite = SelectFrom<INItemSite>.
                                            Where<INItemSite.inventoryID.IsEqual<@P.AsInt>.
                                            And<INItemSite.siteID.IsEqual<@P.AsInt>>>.View.Select(Base, row.InventoryID, row.SiteID); // get default avg cost from default warehouse of item.
                                        if (itemSite != null)
                                        {
                                            replacementCostVal = itemSite.AvgCost;
                                        }

                                    }
                                    else if (invCurSetExt.UsrReplacementCst == MarkupMaint.VendorPrice)
                                    {
                                        if (invCurSet.PreferredVendorID != null) // default vendor of item.
                                        {
                                            POVendorInventory poVendor = SelectFrom<POVendorInventory>.
                                                Where<POVendorInventory.vendorID.IsEqual<@P.AsInt>.
                                                And<POVendorInventory.inventoryID.IsEqual<@P.AsInt>>>.View.Select(Base, invCurSet.PreferredVendorID, row.InventoryID);
                                            if (poVendor != null)
                                            {
                                                replacementCostVal = poVendor.LastPrice;
        
                                            }
                                        }
                                    }
                                    if (mrkUpPricing.MarkupType == Percent)
                                    {
                                        salesOrderPrice = ((mrkUpPricing.MarkupValue / 100) * replacementCostVal) + replacementCostVal;
                                    }
                                    else if (mrkUpPricing.MarkupType == Amount)
                                    {
                                        salesOrderPrice = mrkUpPricing.MarkupValue + replacementCostVal;
                                    }
                                }
                            }
                            if(salesOrderPrice != row.CuryUnitPrice)
                            {
                                cache.SetValueExt<SOLineExtPC.usrChangedRebate>(row, true);
                            }
                            else
                            {
                                cache.SetValueExt<SOLineExtPC.usrChangedRebate>(row, false);
                            }
                        }
                    }
                }
            //}
        }
        protected void _(Events.RowUpdated<SOLine> e, PXRowUpdated baseHandler /* add baseHandler*/)
        {
            SOLine row = e.Row;
            if (row == null) return;
            SOLine oldRow = e.OldRow;
            PXCache cache = e.Cache;
            
            var doc = Base.Document.Current; // add var doc for SOOrder current view;
            string standard = "Standard";
            string rebate = "Rebate";
            string markup = "Markup";

            Customer customer = SelectFrom<Customer>.
                Where<Customer.bAccountID.IsEqual<@P.AsInt>.
                And<BAccountExtPC.usrMarkupPricing.IsEqual<boolTrue>>>.View.Select(Base, doc.CustomerID); // query for selecting customer with Prioritization level of Rebate and Markup
            if (customer != null)
            {
                var customerExt = PXCache<BAccount>.GetExtension<BAccountExtPC>(customer);

                List<KeyValuePair<int?, string>> listOfPrioritization = new List<KeyValuePair<int?, string>>();
                listOfPrioritization.Add(new KeyValuePair<int?, string>(customerExt.UsrStandard, "Standard"));
                listOfPrioritization.Add(new KeyValuePair<int?, string>(customerExt.UsrMarkup, "Markup"));
                listOfPrioritization.Add(new KeyValuePair<int?, string>(customerExt.UsrRebate, "Rebate"));
                foreach (var item in listOfPrioritization.OrderBy(x => x.Key))
                {
                    var key = item.Key;
                    var value = item.Value;
                    if (value == standard)
                    {
                        baseHandler.Invoke(cache, e.Args);
                        //cache.SetValue<SOLineExtPC.usrMarkupID>(row, null);
                        return; // do nothing
                    }
                    if (value == rebate)
                    {
                        // query
                        if (row.InventoryID != oldRow.InventoryID && row.InventoryID != null || row.SiteID != oldRow.SiteID)
                        {
                            cache.SetValueExt<SOLineExtPC.usrStdCost>(row, row.CuryUnitCost);
                            cache.SetValueExt<SOLineExtPC.usrStdPrice>(row, row.CuryUnitPrice);
                            PXTrace.WriteInformation(row.CuryUnitCost.ToString());
                            PXTrace.WriteInformation(row.CuryUnitPrice.ToString());
                        }

                        if (row.InventoryID != null && row.InventoryID == row.InventoryID && row.SiteID != null &&
                            ((row.SiteID != oldRow.SiteID) ||  
                            (row.OrderQty != oldRow.OrderQty || row.OrderQty == 0m)))
                        {
                            List<Rebate> rebateByCustomerID = ActiveRebates.Select().RowCast<Rebate>().ToList();
                            Location loc = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>>>.Select(Base, Base.Document.Current.CustomerID);

                            List<Rebate> rebateByPriceClass = new List<Rebate>();
                            if (loc != null)
                                rebateByPriceClass = ActiveRebatebyPriceClass.Select().RowCast<Rebate>().ToList();

                            // ActiveRebatebyPriceClass.Select().Where<>.RowCast<Rebate>().ToList();

                            var rebateList = rebateByCustomerID.Union(rebateByPriceClass).ToList();
                            RebateItemLine latestRebateLine = null;
                            Rebate latestRebateDoc = null;


                            foreach (Rebate line in rebateList)
                            {
                                PXTrace.WriteInformation(line.RebateNbr.ToString());
                                RebateItemLine rebateItem = PXSelectJoin<RebateItemLine,
                                    LeftJoin<Rebate, On<Rebate.rebateNbr, Equal<RebateItemLine.rebateNbr>>>,
                                    Where<RebateItemLine.inventoryID, Equal<Required<RebateItemLine.inventoryID>>,
                                    And<Where<RebateItemLine.rebateNbr, Equal<Required<RebateItemLine.rebateNbr>>,
                                    And<Where<RebateItemLine.uom, Equal<Required<RebateItemLine.uom>>,
                                    And<Where<RebateItemLine.minQty, LessEqual<Required<RebateItemLine.minQty>>,
                                    And<Where<RebateItemLine.siteID, Equal<Required<RebateItemLine.siteID>>>>>>>>>>>>.Select
                                    (Base, row.InventoryID, line.RebateNbr, row.UOM, row.OrderQty, row.SiteID);
                                if (rebateItem != null)
                                {
                                    if (latestRebateLine == null)
                                    {
                                        latestRebateLine = rebateItem;
                                        latestRebateDoc = line;
                                    }
                                    else if (latestRebateDoc.EffectiveFrom == line.EffectiveFrom &&  
                                        latestRebateDoc.CreatedDateTime < line.CreatedDateTime)
                                    {
                                        latestRebateLine = rebateItem;
                                        latestRebateDoc = line;
                                    }
                                    else if (latestRebateDoc.EffectiveFrom < line.EffectiveFrom)
                                    {
                                        latestRebateLine = rebateItem;
                                        latestRebateDoc = line;
                                    }
                                }
                            }
                            if (latestRebateLine != null)
                            {
                                cache.SetValueExt<SOLineExtPC.usrRebateNbr>(row, latestRebateLine.RebateNbr);
                                cache.SetValueExt<SOLineExtPC.usrCostType>(row, latestRebateDoc.CostType);
                                cache.SetValue<SOLineExtPC.usrMarkupID>(row, null);
                                cache.SetValueExt<SOLine.manualPrice>(row, true);
                                cache.SetValueExt<SOLine.curyUnitPrice>(row, latestRebateLine.RebatePrice);
                                cache.SetValueExt<SOLine.curyUnitCost>(row, latestRebateLine.RebateCost);
                                baseHandler?.Invoke(cache, e.Args);
                                return;
                            }
                            else
                            {
                                cache.SetValueExt<SOLineExtPC.usrRebateNbr>(row, null);
                                cache.SetValueExt<SOLineExtPC.usrCostType>(row, null);

                                //cache.SetValue<SOLineExtPC.usrMarkupID>(row, null); 
                                //cache.SetValueExt<SOLine.manualPrice>(row, false); comment out so it will not return to old value if overriden manual price
                                //cache.SetDefaultExt<SOLine.curyUnitPrice>(row);  comment out so it will not return to old value if overriden manual price
                                //cache.SetDefaultExt<SOLine.curyUnitCost>(row);  comment out so it will not return to old value if overriden manual price
                                //baseHandler?.Invoke(cache, e.Args);
                                continue;
                            }
                        }
                    }
                    if (value == markup)
                    {
                        decimal? _ReplacementCost = 0m;
                        decimal? _SalesOrderPrice = 0m;
                        decimal? replacementCostVal = 0m;
                        decimal? salesOrderPrice = 0m;
                        bool hasInventoryIDinMarkup = false;
                        bool markUpisExpired = false;
                        bool itemIsQualifiedForMarkup = false;
                        int? markupID = 0;
                        if (row.InventoryID != null && row.SiteID != null && row.InventoryID == row.InventoryID && ((row.SiteID != oldRow.SiteID) ||
                            (row.OrderQty != oldRow.OrderQty || row.OrderQty == 0m)))
                        {
                            decimal? mrkUpTableBreakBy = SelectFrom<MarkupPricing>.
                               Where<MarkupPricing.customerID.IsEqual<@P.AsInt>.
                               And<MarkupPricing.effectiveDate.IsLessEqual<@P.AsDateTime>>>.View.Select(Base, doc.CustomerID, Base.Accessinfo.BusinessDate).RowCast<MarkupPricing>().Where(x => x.QtyBreak <= row.OrderQty).Max(x => x.QtyBreak); // checking for break qty qualification
                            if (row.InventoryID != oldRow.InventoryID && row.InventoryID != null || row.SiteID != oldRow.SiteID) {
                                cache.SetValueExt<SOLineExtPC.usrStdCost>(row, row.CuryUnitCost);
                                cache.SetValueExt<SOLineExtPC.usrStdPrice>(row, row.CuryUnitPrice);
                                PXTrace.WriteInformation(row.CuryUnitCost.ToString());
                                PXTrace.WriteInformation(row.CuryUnitPrice.ToString());
                            }
                            if (mrkUpTableBreakBy != null)
                            {
                                MarkupPricing mrkUpTable1 = SelectFrom<MarkupPricing>.
                                    Where<MarkupPricing.qtyBreak.IsEqual<@P.AsInt>.
                                    And<MarkupPricing.customerID.IsEqual<@P.AsInt>.
                                    And<MarkupPricing.effectiveDate.IsLessEqual<@P.AsDateTime>.
                                    And<MarkupPricing.inventoryID.IsNull.
                                    And<MarkupPricing.itemClassID.IsNotNull>>>>>.View.Select(Base, mrkUpTableBreakBy, doc.CustomerID, Base.Accessinfo.BusinessDate); // select specific record in MarkupPricing

                                // check first with inventory ID
                                foreach (MarkupPricing mrkUpData in SelectFrom<MarkupPricing>.
                                    Where<MarkupPricing.qtyBreak.IsEqual<@P.AsInt>.
                                    And<MarkupPricing.customerID.IsEqual<@P.AsInt>.
                                    And<MarkupPricing.siteID.IsEqual<@P.AsInt>.
                                    And<MarkupPricing.effectiveDate.IsLessEqual<@P.AsDateTime>.
                                    And<MarkupPricing.inventoryID.IsNotNull>>>>>.View.Select(Base, mrkUpTableBreakBy, doc.CustomerID, row.SiteID, Base.Accessinfo.BusinessDate))
                                {
                                    if (mrkUpData.InventoryID != null)
                                    {
                                        InventoryItem markUpInvItem = SelectFrom<InventoryItem>.
                                            Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.Select(Base, mrkUpData.InventoryID);
                                        if (markUpInvItem != null)
                                        {
                                            InventoryItemCurySettings markUpInvItemCurySet = SelectFrom<InventoryItemCurySettings>.
                                                Where<InventoryItemCurySettings.inventoryID.IsEqual<@P.AsInt>>.View.Select(Base, markUpInvItem.InventoryID);
                                            if (markUpInvItemCurySet != null)
                                            {
                                                var markUpInvItemCurySetExt = PXCache<InventoryItemCurySettings>.GetExtension<InventoryItemCurySettingsExtPC>(markUpInvItemCurySet);
                                                if (!String.IsNullOrEmpty(markUpInvItemCurySetExt.UsrReplacementCst))
                                                {
                                                    if (markUpInvItemCurySetExt.UsrReplacementCst == MarkupMaint.AverageCost)
                                                    {
                                                        if(row.InventoryID == mrkUpData.InventoryID && row.SiteID == mrkUpData.SiteID)
                                                        {
                                                            _ReplacementCost = mrkUpData.ReplacementCost;
                                                            _SalesOrderPrice = mrkUpData.SalesOrderPrice;
                                                            hasInventoryIDinMarkup = true;
                                                            itemIsQualifiedForMarkup = true;
                                                            markupID = mrkUpData.MarkupID;
                                                            if (mrkUpData.ExpirationDate != null)
                                                            {
                                                                if (mrkUpData.ExpirationDate < Base.Accessinfo.BusinessDate)
                                                                {
                                                                    markUpisExpired = true;
                                                                    continue;
                                                                }
                                                                else
                                                                    markUpisExpired = false;
                                                            }
                                                            break; // return the value and exit the loop.
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (row.InventoryID == mrkUpData.InventoryID && markUpInvItemCurySetExt.UsrReplacementCst != MarkupMaint.AverageCost)
                                                        {
                                                            _ReplacementCost = mrkUpData.ReplacementCost;
                                                            _SalesOrderPrice = mrkUpData.SalesOrderPrice;
                                                            hasInventoryIDinMarkup = true;
                                                            itemIsQualifiedForMarkup = true;
                                                            markupID = mrkUpData.MarkupID;
                                                            if (mrkUpData.ExpirationDate != null)
                                                            {
                                                                if (mrkUpData.ExpirationDate < Base.Accessinfo.BusinessDate)
                                                                {
                                                                    markUpisExpired = true;
                                                                    continue;
                                                                }
                                                                else
                                                                    markUpisExpired = false;
                                                            }
                                                            break; // return the value and exit the loop.
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                if (hasInventoryIDinMarkup == false)
                                {
                                    //then check for itemclass in markup
                                    foreach (MarkupPricing mrkUpTable in SelectFrom<MarkupPricing>.
                                        Where<MarkupPricing.qtyBreak.IsEqual<@P.AsInt>.
                                        And<MarkupPricing.customerID.IsEqual<@P.AsInt>.
                                        And<MarkupPricing.siteID.IsEqual<@P.AsInt>.
                                        And<MarkupPricing.effectiveDate.IsLessEqual<@P.AsDateTime>.
                                        And<MarkupPricing.inventoryID.IsNull.
                                        And<MarkupPricing.itemClassID.IsNotNull>>>>>>.View.Select(Base, mrkUpTableBreakBy, doc.CustomerID, row.SiteID, Base.Accessinfo.BusinessDate))
                                    {
                                        if (mrkUpTable != null)
                                        {
                                            InventoryItem markUpItem = SelectFrom<InventoryItem>.
                                                Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.Select(Base, row.InventoryID);
                                            if (mrkUpTable.ItemClassID != null)
                                            {
                                                if (mrkUpTable.ItemClassID == markUpItem.ItemClassID)
                                                {
                                                    InventoryItemCurySettings invCurSet = SelectFrom<InventoryItemCurySettings>.
                                                            Where<InventoryItemCurySettings.inventoryID.IsEqual<@P.AsInt>>.View.Select(Base, row.InventoryID);
                                                    var invCurSetExt = PXCache<InventoryItemCurySettings>.GetExtension<InventoryItemCurySettingsExtPC>(invCurSet);
                                                    if (!String.IsNullOrEmpty(invCurSetExt.UsrReplacementCst))
                                                    {
                                                        if (invCurSetExt.UsrReplacementCst == MarkupMaint.LastPrice)
                                                        {
                                                            INItemCost itemCst = SelectFrom<INItemCost>.
                                                                Where<INItemCost.inventoryID.IsEqual<@P.AsInt>.
                                                                And<INItemCost.curyID.IsEqual<@P.AsString>>>.View.Select(Base, invCurSet.InventoryID, invCurSet.CuryID); // get last price from Stock Items Last Cost
                                                            if (itemCst != null)
                                                            {
                                                                replacementCostVal = itemCst.LastCost;
                                                                markupID = mrkUpTable.MarkupID;
                                                            }
                                                        }
                                                        else if (invCurSetExt.UsrReplacementCst == MarkupMaint.AverageCost && row.SiteID != null)
                                                        {
                                                            INItemSite itemSite = SelectFrom<INItemSite>.
                                                                Where<INItemSite.inventoryID.IsEqual<@P.AsInt>.
                                                                And<INItemSite.siteID.IsEqual<@P.AsInt>>>.View.Select(Base, row.InventoryID, row.SiteID); // get default avg cost from default warehouse of item.
                                                            if (itemSite != null)
                                                            {
                                                                replacementCostVal = itemSite.AvgCost;
                                                                markupID = mrkUpTable.MarkupID;
                                                            }
                                                            
                                                        }
                                                        else if (invCurSetExt.UsrReplacementCst == MarkupMaint.VendorPrice)
                                                        {
                                                            if (invCurSet.PreferredVendorID != null) // default vendor of item.
                                                            {
                                                                POVendorInventory poVendor = SelectFrom<POVendorInventory>.
                                                                    Where<POVendorInventory.vendorID.IsEqual<@P.AsInt>.
                                                                    And<POVendorInventory.inventoryID.IsEqual<@P.AsInt>>>.View.Select(Base, invCurSet.PreferredVendorID, row.InventoryID);
                                                                if (poVendor != null)
                                                                {
                                                                    replacementCostVal = poVendor.LastPrice;
                                                                    markupID = mrkUpTable.MarkupID;
                                                                    row.CuryUnitPrice = replacementCostVal;
                                                                }
                                                                
                                                            }
                                                        }
                                                    }
                                                    _ReplacementCost = replacementCostVal;
                                                    if (mrkUpTable.MarkupType != null)
                                                    {
                                                        if (mrkUpTable.MarkupType == Percent)
                                                        {
                                                            salesOrderPrice = ((mrkUpTable.MarkupValue / 100)   * _ReplacementCost) + _ReplacementCost;
                                                        }
                                                        else if (mrkUpTable.MarkupType == Amount)
                                                        {
                                                            salesOrderPrice = mrkUpTable.MarkupValue + _ReplacementCost;
                                                        }
                                                    }
                                                    _SalesOrderPrice = salesOrderPrice;
                                                    if (mrkUpTable.ExpirationDate != null)
                                                    {
                                                        if (mrkUpTable.ExpirationDate < Base.Accessinfo.BusinessDate)
                                                        {
                                                            markUpisExpired = true;
                                                            continue;
                                                        }
                                                        else
                                                            markUpisExpired = false;
                                                    }
                                                    itemIsQualifiedForMarkup = true;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            break; // stop the loop
                                        }
                                    }
                                }
                                if (markUpisExpired == false && itemIsQualifiedForMarkup == true)
                                {
                                    if (_ReplacementCost ==  0) {
                                        ReturnMarkUpReplaceMentCostUsed();
                                    }
                                    cache.SetValueExt<SOLine.manualPrice>(row, true);
                                    cache.SetValueExt<SOLine.curyUnitPrice>(row, _SalesOrderPrice);
                                    cache.SetValueExt<SOLine.curyUnitCost>(row, _ReplacementCost);
                                    cache.SetValue<SOLineExtPC.usrMarkupID>(row,markupID);
                                    baseHandler?.Invoke(cache, e.Args);
                                    return; // stop here
                                }
                                else
                                {
                                    cache.SetValue<SOLineExtPC.usrChangedRebate>(row,false);
                                    cache.SetValue<SOLineExtPC.usrMarkupID>(row, null);
                                }
                            }
                            else
                            {
                                continue;  // next priority level
                            }
                        }
                    }
                }
            }
            Base.Transactions.View.RequestRefresh();
        }

        private void ReturnMarkUpReplaceMentCostUsed() {
            if (Base.Transactions.Ask(message: $"The replacement cost is zero. you want to proceed?", MessageButtons.OKCancel) != WebDialogResult.Cancel) { } else
                return;
        }
        #endregion
    }
}