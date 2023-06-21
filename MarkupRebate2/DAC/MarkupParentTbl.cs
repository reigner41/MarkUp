using System;
using PX.Data;

namespace PrecisionCust
{
  [Serializable]
  [PXCacheName("MarkupParentTbl")]
  public class MarkupParentTbl : IBqlTable
  {
    #region ParentMarkupID
    [PXDBString(1, IsKey = true, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Parent Markup ID")]
    [PXDefault("A")]
    public virtual string ParentMarkupID { get; set; }
    public abstract class parentMarkupID : PX.Data.BQL.BqlString.Field<parentMarkupID> { }
    #endregion
      
    #region LineCntr
    [PXDBInt]
    [PXUIField(DisplayName = "Line Cntr")]
    [PXDefault(0)]
    public virtual int? LineCntr{ get; set; }
    public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
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