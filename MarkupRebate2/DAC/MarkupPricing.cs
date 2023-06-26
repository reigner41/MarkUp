using System;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.CS;

namespace PrecisionCust
{
    [Serializable]
    [PXCacheName("MarkupPricing")]
    public class MarkupPricing : IBqlTable
    {
        #region ChildMarkupID
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Markup ID")]
        [PXDBDefault(typeof(MarkupParentTbl.parentMarkupID))]
        [PXParent(typeof(Select<MarkupParentTbl,
            Where<MarkupParentTbl.parentMarkupID,Equal<Current<MarkupPricing.childMarkupID>>>>))]
        public virtual string ChildMarkupID { get; set; }
        public abstract class childMarkupID : PX.Data.BQL.BqlString.Field<childMarkupID> { }
        #endregion

        #region LineNbr
        protected int? _LineNbr;
        [PXDefault()]
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Markup ID", Visible = false)]
        [PXLineNbr(typeof(MarkupParentTbl.lineCntr))]
        public virtual int? LineNbr
        {
            get
            {
                return this._LineNbr;
            }
            set
            {
                this._LineNbr = value;
            }
        }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region MarkupID
        protected int? _MarkupID;
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Markup ID", Enabled = false)]
        public virtual int? MarkupID 
        {
            get
            {
                return this._MarkupID;
            }
            set
            {
                this._MarkupID = value;
            }
        }
        public abstract class markupID : PX.Data.BQL.BqlInt.Field<markupID> { }
        #endregion

        #region MarkupCD
        [PXDBString(255, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Markup Name", Required = true)]
        [PXDefault]
        public virtual string MarkupCD { get; set; }
        public abstract class markupCD : PX.Data.BQL.BqlString.Field<markupCD> { }
        #endregion

        #region BranchID
        [Branch()]
        //[PXCheckUnique]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region CustomerID
        [CustomerActive(Required = true)]
        //[PXCheckUnique]
        [PXDefault]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region MarkupType
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Type",Required = true)]
        [PXDefault]
        [PXStringList(
            new string[] { "P", "A" },
            new string[] { "Percent", "Amount" })]
        public virtual string MarkupType { get; set; }
        public abstract class markupType : PX.Data.BQL.BqlString.Field<markupType> { }
        #endregion

        #region MarkupValue
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Value")]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? MarkupValue { get; set; }
        public abstract class markupValue : PX.Data.BQL.BqlDecimal.Field<markupValue> { }
        #endregion

        #region IsActive
        [PXDBBool()]
        [PXUIField(DisplayName = "Active")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsActive { get; set; }
        public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
        #endregion

        #region SiteID
        [PXDBInt]
        //[PXCheckUnique]
        [PXUIField(DisplayName = "Warehouse")]
        [PXSelector(typeof(Search<INSite.siteID>),
            typeof(INSite.siteCD),
            typeof(INSite.descr),
            SubstituteKey = typeof(INSite.siteCD),
            DescriptionField = typeof(INSite.descr))]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
        #endregion

        #region ItemClassID
        [PXDBInt()]
        //[PXCheckUnique]
        [PXUIField(DisplayName = "Item Class")]
        [PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID,
            Where<INItemClass.stkItem, Equal<boolTrue>>>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr), CacheGlobal = true)]
        public virtual int? ItemClassID { get; set; }
        public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        //[PXCheckUnique]
        [PXSelector(typeof(Search<InventoryItem.inventoryID,
            Where2<Where<InventoryItem.stkItem, Equal<boolTrue>>,
                And<InventoryItem.itemClassID, Equal<Current<MarkupPricing.itemClassID>>, Or<Current<MarkupPricing.itemClassID>, IsNull>>>>),
            typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region QtyBreak
        [PXDBDecimal()]
        //[PXCheckUnique]
        [PXUIField(DisplayName = "Qty Break")]
        [PXDefault(TypeCode.Decimal, "1.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? QtyBreak { get; set; }
        public abstract class qtyBreak : PX.Data.BQL.BqlDecimal.Field<qtyBreak> { }
        #endregion

        #region ReplacementCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Replacement Cost", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? ReplacementCost { get; set; }
        public abstract class replacementCost : PX.Data.BQL.BqlDecimal.Field<replacementCost> { }
        #endregion

        #region SalesOrderPrice
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sales Order Price", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? SalesOrderPrice { get; set; }
        public abstract class salesOrderPrice : PX.Data.BQL.BqlDecimal.Field<salesOrderPrice> { }
        #endregion

        #region EffectiveDate
        [PXDBDate()]
        //[PXCheckUnique]
        [PXUIField(DisplayName = "Effective Date")]
        [PXDefault]
        public virtual DateTime? EffectiveDate { get; set; }
        public abstract class effectiveDate : PX.Data.BQL.BqlDateTime.Field<effectiveDate> { }
        #endregion

        #region ExpirationDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Expiration Date")]
        public virtual DateTime? ExpirationDate { get; set; }
        public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
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