using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PX.Api;
using PX.Common;
using PX.Data;
using PX.SM;
using PX.Objects.AR.CCPaymentProcessing;
using PX.Objects.AR.Repositories;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CR.Extensions;
using PX.Objects.CS;
using PX.Objects.SO;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Data.Descriptor;
using CashAccountAttribute = PX.Objects.GL.CashAccountAttribute;
using PX.Objects.GL.Helpers;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.CR.Extensions.Relational;
using PX.Objects.CR.Extensions.CRCreateActions;
using PX.Objects.GDPR;
using PX.Objects.GraphExtensions.ExtendBAccount;
using PX.Data.ReferentialIntegrity.Attributes;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects;
using PX.Objects.AR;

namespace PX.Objects.AR
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class CustomerMaintExtPC : PXGraphExtension<PX.Objects.AR.CustomerMaint>
  {
        #region Event Handlers

        protected void _(Events.RowSelected<Customer> e)
        {
            var row = e.Row as Customer;
            if (row == null) return;
            PXCache sender = e.Cache;
            var rowExt = sender.GetExtension<BAccountExtPC>(row);
            PXUIFieldAttribute.SetVisible(sender, row, typeof(BAccountExtPC.usrMarkup).Name, rowExt.UsrMarkupPricing == true);
            PXUIFieldAttribute.SetVisible(sender, row, typeof(BAccountExtPC.usrStandard).Name, rowExt.UsrMarkupPricing == true);
            PXUIFieldAttribute.SetVisible(sender, row, typeof(BAccountExtPC.usrRebate).Name, rowExt.UsrMarkupPricing == true);

            PXUIFieldAttribute.SetRequired(sender, typeof(BAccountExtPC.usrMarkup).Name, rowExt.UsrMarkupPricing == true);
            PXUIFieldAttribute.SetRequired(sender, typeof(BAccountExtPC.usrStandard).Name, rowExt.UsrMarkupPricing == true);
            PXUIFieldAttribute.SetRequired(sender, typeof(BAccountExtPC.usrRebate).Name, rowExt.UsrMarkupPricing == true);
        }

        protected void _(Events.FieldUpdated<Customer,BAccountExtPC.usrMarkup> e)
        {
            var row = e.Row as Customer;
            if (row == null) return;
            PXCache sender = e.Cache;
            var rowExt = sender.GetExtension<BAccountExtPC>(row);
            if(rowExt.UsrMarkup == rowExt.UsrStandard)
            {
                sender.SetValue<BAccountExtPC.usrStandard>(row, e.OldValue);
            }
            else if(rowExt.UsrMarkup == rowExt.UsrRebate)
            {
                sender.SetValue<BAccountExtPC.usrRebate>(row, e.OldValue);
            }
        }
        protected void _(Events.FieldUpdated<Customer, BAccountExtPC.usrStandard> e)
        {
            var row = e.Row as Customer;
            if (row == null) return;
            PXCache sender = e.Cache;
            var rowExt = sender.GetExtension<BAccountExtPC>(row);
            if (rowExt.UsrStandard == rowExt.UsrMarkup)
            {
                sender.SetValue<BAccountExtPC.usrMarkup>(row, e.OldValue);
            }
            else if (rowExt.UsrStandard == rowExt.UsrRebate)
            {
                sender.SetValue<BAccountExtPC.usrRebate>(row, e.OldValue);
            }
        }
        protected void _(Events.FieldUpdated<Customer, BAccountExtPC.usrRebate> e)
        {
            var row = e.Row as Customer;
            if (row == null) return;
            PXCache sender = e.Cache;
            var rowExt = sender.GetExtension<BAccountExtPC>(row);
            if (rowExt.UsrRebate == rowExt.UsrMarkup)
            {
                sender.SetValue<BAccountExtPC.usrMarkup>(row, e.OldValue);
            }
            else if (rowExt.UsrRebate == rowExt.UsrStandard)
            {
                sender.SetValue<BAccountExtPC.usrStandard>(row, e.OldValue);
            }
        }

        protected void _(Events.FieldUpdated<Customer, BAccountExtPC.usrMarkupPricing> e)
        {
            var row = e.Row as Customer;
            if (row == null) return;
            PXCache sender = e.Cache;
            var rowExt = sender.GetExtension<BAccountExtPC>(row);
            if(rowExt.UsrMarkupPricing == false)
            {
                sender.SetValue<BAccountExtPC.usrStandard>(row, null);
                sender.SetValue<BAccountExtPC.usrMarkup>(row, null);
                sender.SetValue<BAccountExtPC.usrRebate>(row, null);
            }
        }

        protected void _(Events.RowPersisting<Customer> e, PXRowPersisting baseHandler)
        {
            var row = e.Row as Customer;
            if (row == null) return;
            PXCache sender = e.Cache;
            baseHandler?.Invoke(sender, e.Args);
            bool hasError = false;
            var rowExt = sender.GetExtension<BAccountExtPC>(row);
            if (rowExt.UsrMarkupPricing == true)
            {
                if (rowExt.UsrStandard == null)
                {
                    hasError = true;
                    sender.SetValue<BAccountExtPC.usrStandard>(row, rowExt.UsrStandard);
                    sender.RaiseExceptionHandling<BAccountExtPC.usrStandard>(row, rowExt.UsrStandard, new PXSetPropertyException("Error: Standard cannot be empty.", PXErrorLevel.Error));
                }
                if (rowExt.UsrMarkup == null)
                {
                    hasError = true;
                    sender.SetValue<BAccountExtPC.usrMarkup>(row, rowExt.UsrMarkup);
                    sender.RaiseExceptionHandling<BAccountExtPC.usrMarkup>(row, rowExt.UsrMarkup, new PXSetPropertyException("Error: Markup cannot be empty.", PXErrorLevel.Error));
                }
                if (rowExt.UsrRebate == null)
                {
                    hasError = true;
                    sender.SetValue<BAccountExtPC.usrRebate>(row, rowExt.UsrRebate);
                    sender.RaiseExceptionHandling<BAccountExtPC.usrRebate>(row, rowExt.UsrRebate, new PXSetPropertyException("Error: Rebate cannot be empty.", PXErrorLevel.Error));
                }
            }
            if(hasError == true) { throw new PXException("Error: Inserting 'Customer' record raised at least one error. Please review the errors."); }
        }
        #endregion
    }
}