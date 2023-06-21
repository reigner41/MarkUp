using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.PO;

namespace PC
{
  [Serializable]
  [PXCacheName("Rebate Item")]
  public class RebateItemLine : IBqlTable
  {
    #region RebateNbr
    [PXDBString(15, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Rebate Nbr")]
        [PXDBDefault(typeof(Rebate.rebateNbr))]
        [PXParent(typeof(SelectFrom<Rebate>.Where<Rebate.rebateNbr.IsEqual<RebateItemLine.rebateNbr.FromCurrent>>))]
        public virtual string RebateNbr { get; set; }
    public abstract class rebateNbr : PX.Data.BQL.BqlString.Field<rebateNbr> { }
    #endregion

    #region InventoryID
    [PXDBInt()]
    [PXUIField(DisplayName = "Inventory ID")]
    [PXSelector(typeof(Search2<InventoryItem.inventoryID, 
        LeftJoin<POVendorInventory, On<InventoryItem.inventoryID, Equal<POVendorInventory.inventoryID>>>,
        Where<POVendorInventory.vendorID, Equal<Current<Rebate.vendorID>>>>),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
    public virtual int? InventoryID { get; set; }
    public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region SiteID
        [PXDBInt()]
        [PXDefault()]
        [PXUIField(DisplayName = "Warehouse")]
        [PXSelector(typeof(Search2<INSite.siteID, InnerJoin<INItemSite, On<INSite.siteID, Equal<INItemSite.siteID>>>,
            Where<INItemSite.inventoryID, Equal<Current<inventoryID>>>>),
            SubstituteKey = typeof(INSite.siteCD), DescriptionField = typeof(INSite.descr))]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
        #endregion

        #region RecordID 
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Record ID")]
        public virtual int? RecordID { get; set; }
        public abstract class recordID : PX.Data.BQL.BqlInt.Field<recordID> { }
        #endregion

        #region Uom
        //   [PXDBString(6, IsUnicode = true, InputMask = "", IsKey = true)]
        //[PXUIField(DisplayName = "UOM")]
        [INUnit(typeof(RebateItemLine.inventoryID), DisplayName = "UOM", IsKey =true)]
        [PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<RebateItemLine.inventoryID>>>>))]

        public virtual string Uom { get; set; }
    public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
    #endregion

        #region StdCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "System Cost", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? StdCost { get; set; }
        public abstract class stdCost : PX.Data.BQL.BqlDecimal.Field<stdCost> { }
        #endregion

    #region StdPrice
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Base Price", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", typeof(Search<InventoryItem.basePrice, 
            Where<InventoryItem.inventoryID, Equal<Current<RebateItemLine.inventoryID>>>>))]

        public virtual Decimal? StdPrice { get; set; }
    public abstract class stdPrice : PX.Data.BQL.BqlDecimal.Field<stdPrice> { }
    #endregion

    #region RebateCost
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Rebate Cost")]
        [PXDefault(TypeCode.Decimal, "0.0")]

        public virtual Decimal? RebateCost { get; set; }
    public abstract class rebateCost : PX.Data.BQL.BqlDecimal.Field<rebateCost> { }
    #endregion

    #region RebatePrice
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Rebate Price")]
        [PXDefault(TypeCode.Decimal, "0.0")]

        public virtual Decimal? RebatePrice { get; set; }
    public abstract class rebatePrice : PX.Data.BQL.BqlDecimal.Field<rebatePrice> { }
    #endregion

    #region MinQty
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Min Qty")]
        [PXDefault(TypeCode.Decimal, "0.0")]

        public virtual Decimal? MinQty { get; set; }
    public abstract class minQty : PX.Data.BQL.BqlDecimal.Field<minQty> { }
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