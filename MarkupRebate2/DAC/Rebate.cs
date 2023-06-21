using System;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;

namespace PC
{
    public abstract class CostType : IBqlField
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                new string[] { Replacement, StandardCost },
                new string[] { "Replacement", "Standard" })
            { }
        }

        public const string Replacement = "R";
        public const string StandardCost = "S";

        public class replacement : Constant<string>
        {
            public replacement() : base(Replacement) { }
        }
        public class standardCost : Constant<string>
        {
            public standardCost() : base(StandardCost) { }
        }
    }

    [Serializable]
  [PXCacheName("Rebate")]
  [PXPrimaryGraph(typeof(RebatesMaint))]
  public class Rebate : IBqlTable
  {
    #region RebateNbr
    [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
    [PXUIField(DisplayName = "Rebate ID")]
    [AutoNumber(typeof(APSetupExtPC.usrRebateNumberingID), typeof(AccessInfo.businessDate))]
    [PXSelector(typeof(Rebate.rebateNbr))]
    public virtual string RebateNbr { get; set; }
    public abstract class rebateNbr : PX.Data.BQL.BqlString.Field<rebateNbr> { }
    #endregion

    #region VendorID
   // [PXDBInt()]
    [PXUIField(DisplayName = "Vendor ID")]
    [VendorActive]
    public virtual int? VendorID { get; set; }
    public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

    #region CostType
    [PXDBString(1)]
    [PXUIField(DisplayName = "Cost Type")]
    [CostType.List]
    [PXDefault(typeof(Search<APSetupExtPC.usrRebateCostType>))]
    public virtual string CostType { get; set; }
    public abstract class costType : PX.Data.BQL.BqlString.Field<costType> { }
    #endregion

        #region Descr
        [PXDBString(60, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Descr")]
    public virtual string Descr { get; set; }
    public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
    #endregion

    #region Active
    [PXDBBool()]
    [PXUIField(DisplayName = "Active")]
    public virtual bool? Active { get; set; }
    public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
    #endregion

    #region EffectiveFrom
    [PXDBDate()]
    [PXUIField(DisplayName = "Effective From")]
    [PXDefault(typeof(AccessInfo.businessDate))]
    public virtual DateTime? EffectiveFrom { get; set; }
    public abstract class effectiveFrom : PX.Data.BQL.BqlDateTime.Field<effectiveFrom> { }
    #endregion

    #region EffectiveTo
    [PXDBDate()]
    [PXUIField(DisplayName = "Expiration Date")]
    public virtual DateTime? EffectiveTo { get; set; }
    public abstract class effectiveTo : PX.Data.BQL.BqlDateTime.Field<effectiveTo> { }
    #endregion

    #region CreatedByID
    [PXDBCreatedByID()]
    public virtual Guid? CreatedByID { get; set; }
    public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
    #endregion

    #region CreatedByScreenID
    [PXDBCreatedByScreenID()]
    public virtual string CreatedByScreenID { get; set; }
    public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
    #endregion

    #region CreatedDateTime
    [PXDBCreatedDateTime()]
    public virtual DateTime? CreatedDateTime { get; set; }
    public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
    #endregion

    #region LastModifiedByID
    [PXDBLastModifiedByID()]
    public virtual Guid? LastModifiedByID { get; set; }
    public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
    #endregion

    #region LastModifiedByScreenID
    [PXDBLastModifiedByScreenID()]
    public virtual string LastModifiedByScreenID { get; set; }
    public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
    #endregion

    #region LastModifiedDateTime
    [PXDBLastModifiedDateTime()]
    public virtual DateTime? LastModifiedDateTime { get; set; }
    public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
    #endregion

    #region Tstamp
    [PXDBTimestamp()]
    [PXUIField(DisplayName = "Tstamp")]
    public virtual byte[] Tstamp { get; set; }
    public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
    #endregion
  }
}