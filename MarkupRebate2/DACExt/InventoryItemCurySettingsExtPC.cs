using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.IN;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System;

namespace PX.Objects.IN
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public sealed class InventoryItemCurySettingsExtPC : PXCacheExtension<PX.Objects.IN.InventoryItemCurySettings>
  {
        #region UsrReplacementCst
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Replacement Cost")]
        [PXStringList(
            new string[] { "L","A","V" },
            new string[] { "Last Cost", "Average Cost", "Vendor Price" })]
        public string UsrReplacementCst { get; set; }
        public abstract class usrReplacementCst : PX.Data.BQL.BqlString.Field<usrReplacementCst> { }
        #endregion
   }
}