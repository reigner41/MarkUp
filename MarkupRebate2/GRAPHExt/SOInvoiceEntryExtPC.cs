using System;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Api;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM.Extensions;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Objects.CA;
using ItemLotSerial = PX.Objects.IN.Overrides.INDocumentRelease.ItemLotSerial;
using SiteLotSerial = PX.Objects.IN.Overrides.INDocumentRelease.SiteLotSerial;
using LocationStatus = PX.Objects.IN.Overrides.INDocumentRelease.LocationStatus;
using LotSerialStatus = PX.Objects.IN.Overrides.INDocumentRelease.LotSerialStatus;
using POLineType = PX.Objects.PO.POLineType;
using POReceipt = PX.Objects.PO.POReceipt;
using POReceiptType = PX.Objects.PO.POReceiptType;
using POReceiptLine = PX.Objects.PO.POReceiptLine;
using SiteStatus = PX.Objects.IN.Overrides.INDocumentRelease.SiteStatus;
using System.Linq;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AR.CCPaymentProcessing;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.AR.CCPaymentProcessing.Interfaces;
using PX.Objects.AR.Repositories;
using PX.Objects.SO.GraphExtensions.SOInvoiceEntryExt;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Objects.Common.Extensions;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common.Bql;
using PX.Common.Collection;
using PX.Objects.GL.FinPeriods;
using PX.Objects.Extensions.PaymentTransaction;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Data.WorkflowAPI;
using PX.Objects;
using PX.Objects.SO;
using PX.Objects.AP;
using PC;

namespace PX.Objects.SO
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class SOInvoiceEntryExtPC : PXGraphExtension<PX.Objects.SO.SOInvoiceEntry>
    {
        #region Event Handlers
        public delegate IEnumerable ReleaseDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Release(PXAdapter adapter, ReleaseDelegate baseMethod)
        {
            var temp = baseMethod(adapter);
            foreach (ARInvoice ardoc in adapter.Get<ARInvoice>())
            {
                // throw new PXException("test");
                APInvoiceEntry apGraph = PXGraph.CreateInstance<APInvoiceEntry>();
                APSetup setup = apGraph.APSetup.Current;
                APSetupExtPC setupExt = setup.GetExtension<APSetupExtPC>();

                var list = new List<KeyValuePair<int?, APTran>>();
                if (ardoc.DocType == ARDocType.Invoice)
                {
                    foreach (SOLine line in PXSelectJoin<SOLine,
                    LeftJoin<ARTran, On<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
                    And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>,
                    Where<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
                    And<SOLineExtPC.usrRebateNbr, IsNotNull>>>.Select(Base, ardoc.RefNbr))
                    {
                        SOLineExtPC lineExt = line.GetExtension<SOLineExtPC>();
                        Rebate rb = PXSelect<Rebate,
                            Where<Rebate.rebateNbr, Equal<Required<Rebate.rebateNbr>>>>
                            .Select(Base, lineExt.UsrRebateNbr);

                        APTran newTran = new APTran();
                        newTran.TranDesc = line.OrderNbr + " - " + line.TranDesc;
                        newTran.Qty = line.Qty;
                        decimal tempAmt = (decimal)(lineExt.UsrStdCost - line.CuryUnitCost);
                        newTran.CuryUnitCost = Math.Abs(tempAmt);
                        newTran.AccountID = setupExt.UsrExpenseAccountID;
                        list.Add(new KeyValuePair<int?, APTran>(rb.VendorID, newTran));
                    }
                    if (list.Count > 0)
                    {
                        var vendorGroup = list.GroupBy(v => v.Key);
                        foreach (var grp in vendorGroup)
                        {
                            APInvoice newBill = apGraph.Document.Insert();
                            //newBill.DocType = APDocType.DebitAdj;
                            //newBill.VendorID = grp.Key;
                            //newBill.InvoiceNbr = ardoc.RefNbr;
                            apGraph.Document.Cache.SetValueExt<APInvoice.docType>(newBill, APDocType.DebitAdj);
                            apGraph.Document.Cache.SetValueExt<APInvoice.vendorID>(newBill, grp.Key);
                            apGraph.Document.Cache.SetValueExt<APInvoice.invoiceNbr>(newBill, ardoc.RefNbr);
                            apGraph.Document.Cache.SetValueExt<APInvoice.aPAccountID>(newBill, setupExt.UsrLiabilityAccount);
                            string soNbrs = "";

                            apGraph.Document.Cache.SetValueExt<APRegisterExtPC.usrSOInvoiceNbr>(newBill, ardoc.RefNbr);
                            apGraph.Document.Update(newBill);
                            foreach (var subGrp in grp)
                            {
                                string descr = (string)subGrp.Value.TranDesc;
                                string str = descr.Substring(0, 8);
                                PXTrace.WriteInformation(str);
                                soNbrs = soNbrs + " " + str + ",";
                                apGraph.Transactions.Insert(subGrp.Value);

                                newBill.DocDesc = "Rebates from order/s:" + soNbrs;
                                apGraph.Document.Update(newBill);
                            }
                           

                            apGraph.Save.Press();
                        }
                    }
                }

            }
            return temp;
        }


        #endregion
    }
}