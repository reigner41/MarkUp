using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Common;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.SM;
using PX.Objects.IN.Overrides.INDocumentRelease;
using POLineType = PX.Objects.PO.POLineType;
using POReceiptLine = PX.Objects.PO.POReceiptLine;
using PX.Data.DependencyInjection;
using PX.Objects.SO.Services;
using PX.Objects.PO;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Objects.Common.Extensions;
using PX.Common.Collection;
using PX.Objects.SO.GraphExtensions.CarrierRates;
using PX.Objects.SO.GraphExtensions.SOShipmentEntryExt;
using PX.Api;
using LocationStatus = PX.Objects.IN.Overrides.INDocumentRelease.LocationStatus;
using ShipmentActions = PX.Objects.SO.SOShipmentEntryActionsAttribute;
using PX.Objects;
using PX.Objects.SO;
using PC;

namespace PX.Objects.SO
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class SOShipmentEntryExtPC : PXGraphExtension<PX.Objects.SO.SOShipmentEntry>
  {
    #region Event Handlers
  
  public delegate IEnumerable ConfirmShipmentActionDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable ConfirmShipmentAction(PXAdapter adapter, ConfirmShipmentActionDelegate baseMethod)
        {
            PXGraph.InstanceCreated.AddHandler<SOOrderEntry>((graph) =>
            {
                PXTrace.WriteInformation("1");
                graph.RowPersisting.AddHandler<SOLine>((sender, e) =>
                {
                    PXTrace.WriteInformation("2");
                    var soline = e.Row as SOLine;
                    var soLineExt = soline.GetExtension<SOLineExtPC>();

                    if (soline != null && soLineExt.UsrRebateNbr != null) //Add any conditions needed
                    {
                        RebateItemLine rbItem = PXSelect<RebateItemLine,
                        Where<RebateItemLine.rebateNbr, Equal<Required<RebateItemLine.rebateNbr>>,
                        And<Where<RebateItemLine.inventoryID, Equal<Required<RebateItemLine.inventoryID>>,
                        And<Where<RebateItemLine.siteID, Equal<Required<RebateItemLine.siteID>>>>>>>>
                        .Select(Base, soLineExt.UsrRebateNbr, soline.InventoryID, soline.SiteID);

                        if(rbItem != null)
                        {
                            soline.CuryUnitCost = rbItem.RebateCost;
                            graph.Transactions.Update(soline);
                        }
                    }
                });
            });

            return baseMethod(adapter);
        }

    #endregion
  }
}