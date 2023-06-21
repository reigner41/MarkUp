using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.EP.Standalone;
using PX.Objects;
using System.Collections.Generic;
using System;
using PX.Objects.GL;
using PC;


namespace PX.Objects.AP
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public sealed class APSetupExtPC : PXCacheExtension<PX.Objects.AP.APSetup>
  {
        #region UsrRebateNumberingID
        [PXDBString(10)]
        [PXUIField(DisplayName="Rebate Numbering ID")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public string UsrRebateNumberingID { get; set; }
        public abstract class usrRebateNumberingID : PX.Data.BQL.BqlString.Field<usrRebateNumberingID> { }
        #endregion

        #region UsrExpenseAccountID
        // [PXDBInt]
        //[PXUIField(DisplayName = "Expense Account")]
        [Account(DisplayName = "Expense Account")]
        public int? UsrExpenseAccountID { get; set; }
        public abstract class usrExpenseAccountID : PX.Data.BQL.BqlInt.Field<usrExpenseAccountID> { }
        #endregion

        #region UsrLiabilityAccount
        //[PXDBInt]
        // [PXUIField]
        [Account(DisplayName = "Liability Account")]
        public int? UsrLiabilityAccount { get; set; }
        public abstract class usrLiabilityAccount : PX.Data.BQL.BqlInt.Field<usrLiabilityAccount> { }
        #endregion

        #region UsrRebateCostType
        [PXDBString(1)]
        [PXUIField(DisplayName = "Cost Type")]
        [CostType.List]
        public string UsrRebateCostType { get; set; }
        public abstract class usrRebateCostType : PX.Data.BQL.BqlString.Field<usrRebateCostType> { }
        #endregion
    }
}