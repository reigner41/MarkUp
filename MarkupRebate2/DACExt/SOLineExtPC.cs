using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Attributes;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Common.Discount;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.GL; 
using PX.Objects.IN.Matrix.Interfaces;
using PX.Objects.IN.RelatedItems;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO.DAC.Projections;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System.Collections;
using System;
using PC;

namespace PX.Objects.SO
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public sealed class SOLineExtPC : PXCacheExtension<PX.Objects.SO.SOLine>
  {
    #region UsrStdCost
    [PXDBDecimal]
    [PXUIField(DisplayName="System Cost", Enabled = false)]
    [PXDefault(typeof(SOLine.curyUnitCost), PersistingCheck = PXPersistingCheck.Nothing)]
    public  Decimal? UsrStdCost { get; set; }
    public abstract class usrStdCost : PX.Data.BQL.BqlDecimal.Field<usrStdCost> { }
    #endregion

    #region UsrStdPrice
    [PXDBDecimal]
    [PXUIField(DisplayName="System Price", Enabled = false)]
        [PXDefault(typeof(SOLine.curyUnitPrice), PersistingCheck = PXPersistingCheck.Nothing)]

        public  Decimal? UsrStdPrice { get; set; } 
    public abstract class usrStdPrice : PX.Data.BQL.BqlDecimal.Field<usrStdPrice> { }
    #endregion

    #region UsrRebateNbr
    [PXDBString(15)]
    [PXUIField(DisplayName="Rebate Nbr", Enabled = false)]
    [PXSelector(typeof(Search<Rebate.rebateNbr>))]
    public  string UsrRebateNbr { get; set; }
    public abstract class usrRebateNbr : PX.Data.BQL.BqlString.Field<usrRebateNbr> { }
    #endregion
      
      #region UsrChangedRebate
    [PXDBBool]
    [PXUIField(DisplayName="Override Manual Price", Enabled = false)]
    [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
    public  bool? UsrChangedRebate { get; set; }
    public abstract class usrChangedRebate : PX.Data.BQL.BqlBool.Field<usrChangedRebate> { }
    #endregion
      
      #region UsrMarkupID
        [PXDBInt]
        public  int? UsrMarkupID { get; set; }
        public abstract class usrMarkupID : PX.Data.BQL.BqlInt.Field<usrMarkupID> { }

        #endregion

        #region UsrCostType
        [PXDBString(1)]
        [PXUIField(DisplayName = "Cost Type")]
        [CostType.List]
        public  string UsrCostType { get; set; }
        public abstract class usrCostType : PX.Data.BQL.BqlString.Field<usrCostType> { }
        #endregion


    }
}