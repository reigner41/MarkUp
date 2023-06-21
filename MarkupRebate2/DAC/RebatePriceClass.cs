using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;

namespace PC
{
  [Serializable]
  [PXCacheName("RebatePriceClass")]
  public class RebatePriceClass : IBqlTable
  {
    #region RebateNbr
    [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
    [PXUIField(DisplayName = "Rebate Nbr")]
        [PXDBDefault(typeof(Rebate.rebateNbr))]
        [PXParent(typeof(SelectFrom<Rebate>.Where<Rebate.rebateNbr.IsEqual<RebatePriceClass.rebateNbr.FromCurrent>>))]

        public virtual string RebateNbr { get; set; }
    public abstract class rebateNbr : PX.Data.BQL.BqlString.Field<rebateNbr> { }
    #endregion

    #region PriceClassID
    [PXDBString(10, IsUnicode = true, InputMask = "", IsKey = true)]
    [PXUIField(DisplayName = "Price Class ID")]
    [CustomerPriceClass]
    public virtual string PriceClassID { get; set; }
    public abstract class priceClassID : PX.Data.BQL.BqlString.Field<priceClassID> { }
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