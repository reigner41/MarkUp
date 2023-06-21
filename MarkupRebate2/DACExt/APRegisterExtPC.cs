using APQuickCheck = PX.Objects.AP.Standalone.APQuickCheck;
using CRLocation = PX.Objects.CR.Standalone.Location;
using IRegister = PX.Objects.CM.IRegister;
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CA;
using PX.Objects.CM.Extensions;
using PX.Objects.Common.Abstractions;
using PX.Objects.Common.MigrationMode;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using PX.Objects.SO;
using PX.Objects.AR;

namespace PX.Objects.AP
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public sealed class APRegisterExtPC : PXCacheExtension<PX.Objects.AP.APRegister>
  {
    #region UsrSOInvoiceNbr
    [PXDBString(15)] 
    [PXUIField(DisplayName="Invoice Nbr", Enabled = false)]
    [PXSelector(typeof(Search<SOInvoice.refNbr, Where<SOInvoice.docType, Equal<ARDocType.invoice>>>))]
    public string UsrSOInvoiceNbr { get; set; }
    public abstract class usrSOInvoiceNbr : PX.Data.BQL.BqlString.Field<usrSOInvoiceNbr> { }
    #endregion
  }
}