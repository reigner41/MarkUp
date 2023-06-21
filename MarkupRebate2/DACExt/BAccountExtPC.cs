using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR.MassProcess;
using PX.Objects.CR.Workflows;
using PX.Objects.CR;
using PX.Objects.CS.DAC;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.PO;
using PX.Objects.TX;
using PX.Objects;
using PX.SM;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PX.Objects.CR
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public sealed class BAccountExtPC : PXCacheExtension<PX.Objects.CR.BAccount>
  {
        #region UsrMarkup
        [PXDBInt]
        [PXUIField(DisplayName = "Markup")]
        [PXIntList(new int[] { 1, 2, 3 },
            new string[] { "1", "2", "3" })]
        public int? UsrMarkup { get; set; }
        public abstract class usrMarkup : PX.Data.BQL.BqlInt.Field<usrMarkup> { }
        #endregion

        #region UsrRebate
        [PXDBInt]
        [PXUIField(DisplayName = "Rebate")]
        [PXIntList(new int[] { 1, 2, 3 },
            new string[] { "1", "2", "3" })]
        public  int? UsrRebate { get; set; }
        public abstract class usrRebate : PX.Data.BQL.BqlInt.Field<usrRebate> { }
        #endregion

        #region UsrStandard
        [PXDBInt]
        [PXUIField(DisplayName = "Standard")]
        [PXIntList(new int[] { 1, 2, 3 },
            new string[] { "1", "2", "3" })]
        public  int? UsrStandard { get; set; }
        public abstract class usrStandard : PX.Data.BQL.BqlInt.Field<usrStandard> { }
        #endregion

        #region UsrMarkupPricing
        [PXDBBool]
        [PXUIField(DisplayName = "Markup Pricing")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public  bool? UsrMarkupPricing { get; set; }
        public abstract class usrMarkupPricing : PX.Data.BQL.BqlBool.Field<usrMarkupPricing> { }

        #endregion
    }
}