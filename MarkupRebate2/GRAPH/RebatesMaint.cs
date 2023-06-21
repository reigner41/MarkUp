using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.IN;
using System.Collections;
using System.Collections.Generic;

namespace PC
{
  public class RebatesMaint : PXGraph<RebatesMaint, Rebate>
  {
        public PXSetup<APSetup> Setup;

        public SelectFrom<Rebate>.View Rebates;

        [PXImport]
        public SelectFrom<RebateItemLine>.Where<RebateItemLine.rebateNbr.IsEqual<Rebate.rebateNbr.FromCurrent>>.View ItemLines;

        [PXImport]
        public SelectFrom<RebatePriceClass>.Where<RebatePriceClass.rebateNbr.IsEqual<Rebate.rebateNbr.FromCurrent>>.View RBPriceCLass;

        [PXImport]
        public SelectFrom<RebateCustomer>.Where<RebateCustomer.rebateNbr.IsEqual<Rebate.rebateNbr.FromCurrent>>.View RBCustomers;
  
      
        protected void _(Events.RowPersisting<Rebate> e)
        {
            Rebate row = e.Row;
            PXCache cache = e.Cache;

            if(row.EffectiveFrom != null && row.EffectiveTo != null)
            {
                if(row.EffectiveFrom > row.EffectiveTo) 
                {
                    // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                    cache.RaiseExceptionHandling<Rebate.effectiveFrom>(row, row.EffectiveFrom,
                        new PXSetPropertyException("Effective from should not be later than the expiration date.", PXErrorLevel.Error));
                }

                if(row.EffectiveTo < Accessinfo.BusinessDate)
                {
                    // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                    cache.RaiseExceptionHandling<Rebate.effectiveTo>(row, row.EffectiveTo,
                        new PXSetPropertyException("Effective from date should be later than the current date.", PXErrorLevel.Error));
                }
            }
        }

        protected void _(Events.FieldUpdated<Rebate, Rebate.vendorID> e)
        {
            Rebate row = e.Row;

            foreach (RebateItemLine line in ItemLines.Select())
            {
                ItemLines.Delete(line);
            }
        }

        //protected void _(Events.FieldUpdated<RebateItemLine, RebateItemLine.inventoryID> e)
        //{
        //    RebateItemLine row = e.Row;
        //    PXCache cache = e.Cache;

        //    cache.SetDefaultExt<RebateItemLine.stdCost>(row);
        //}

        //protected void _(Events.FieldUpdated<RebateItemLine, RebateItemLine.uom> e)
        //{
        //    RebateItemLine row = e.Row;
        //    PXCache cache = e.Cache;

        //    cache.SetDefaultExt<RebateItemLine.stdCost>(row);
        //}

        protected void _(Events.FieldUpdated<RebateItemLine, RebateItemLine.siteID> e)
        {
            RebateItemLine row = e.Row;
            PXCache cache = e.Cache;

            cache.SetDefaultExt<RebateItemLine.stdCost>(row);
        }


        protected void _(Events.FieldDefaulting<RebateItemLine, RebateItemLine.stdCost> e)
        {
            RebateItemLine row = e.Row;
            APSetup payablesSetup = Setup.Current;
            APSetupExtPC setupExt = payablesSetup.GetExtension<APSetupExtPC>();

            if (row.InventoryID != null && row.Uom != null)
            {
                if (setupExt.UsrRebateCostType == "R")
                {
                    APVendorPrice price = null;
                    if (row.SiteID != null)
                    {
                        price = PXSelect<APVendorPrice,
                        Where<APVendorPrice.effectiveDate, LessEqual<Current<AccessInfo.businessDate>>,
                        And<Where<APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
                        And<Where<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>,
                        And<Where<APVendorPrice.uOM, Equal<Required<APVendorPrice.uOM>>,
                        And<Where<APVendorPrice.siteID, Equal<Required<APVendorPrice.siteID>>,
                        And<Where<APVendorPrice.expirationDate, LessEqual<Current<AccessInfo.businessDate>>,
                        Or<APVendorPrice.expirationDate, IsNull>>>>>>>>>>>>>.Select(this, Rebates.Current.VendorID, row.InventoryID, row.Uom, row.SiteID);
                    }
                    else
                    {
                        price = PXSelect<APVendorPrice,
                        Where<APVendorPrice.effectiveDate, LessEqual<Current<AccessInfo.businessDate>>,
                        And<Where<APVendorPrice.vendorID, Equal<Required<APVendorPrice.vendorID>>,
                        And<Where<APVendorPrice.inventoryID, Equal<Required<APVendorPrice.inventoryID>>,
                        And<Where<APVendorPrice.uOM, Equal<Required<APVendorPrice.uOM>>,
                        And<Where<APVendorPrice.siteID, IsNull,
                        And<Where<APVendorPrice.expirationDate, LessEqual<Current<AccessInfo.businessDate>>,
                        Or<APVendorPrice.expirationDate, IsNull>>>>>>>>>>>>>.Select(this, Rebates.Current.VendorID, row.InventoryID, row.Uom);
                        PXTrace.WriteInformation(price.SalesPrice.ToString());
                    
                        PXTrace.WriteInformation(price.SalesPrice.ToString());
                    }

                    

                    
                    if (price != null)
                        e.NewValue = price.SalesPrice;
                    else
                    {
                        e.NewValue = decimal.Zero;
                    }
                }
                else if (setupExt.UsrRebateCostType == "S" && row.SiteID != null)
                {
                    InventoryItem item = PXSelect<InventoryItem, 
                        Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                        .Select(this, row.InventoryID);

                    InventoryItemCurySettings itemSettings = PXSelect<InventoryItemCurySettings,
                            Where<InventoryItemCurySettings.inventoryID, Equal<Required<InventoryItemCurySettings.inventoryID>>>>
                            .Select(this, item.InventoryID);

                    
                    if (item.ValMethod == INValMethod.Average)
                    {
                        INItemSite itemSite = PXSelect<INItemSite, Where<INItemSite.siteID, Equal<Required<INItemSite.siteID>>,
                            And<INItemSite.inventoryID, Equal<Required<INItemSite.inventoryID>>>>>
                            .Select(this, row.SiteID, item.InventoryID);

                        e.NewValue = itemSite.AvgCost;
                    }
                    else
                    {
                        e.NewValue = itemSettings.StdCost;
                    }
                }
            }
        }

        protected void _(Events.RowSelected<RebateItemLine> e)
        {
            RebateItemLine row = e.Row;
            Rebate doc = Rebates.Current;
            PXCache cache = e.Cache;
            //if (doc.Active == true)
            //{
            //    if (row.StdCost == 0m)
            //    {
            //        cache.RaiseExceptionHandling<RebateItemLine.stdCost>(row, row.StdCost,
            //            new PXSetPropertyException("No vendor item price found", PXErrorLevel.Warning));
            //    }
            //    if (row.StdPrice == 0m)
            //    {
            //        cache.RaiseExceptionHandling<RebateItemLine.stdCost>(row, row.StdCost,
            //            new PXSetPropertyException("Current item price is 0", PXErrorLevel.Warning));
            //    }
            //}
        }
    }
}