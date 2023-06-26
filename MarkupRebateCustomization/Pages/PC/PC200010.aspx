<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PC200010.aspx.cs" Inherits="Page_PC200010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PC.RebatesMaint"
        PrimaryView="Rebates"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Rebates" Width="100%" Height="125px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector8" DataField="RebateNbr" ></px:PXSelector>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask9" DataField="VendorID" ></px:PXSegmentMask>
			<px:PXDropDown runat="server" ID="CstPXDropDown2" DataField="CostType" Enabled="False" ></px:PXDropDown>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="Descr" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule10" StartColumn="True" ></px:PXLayoutRule>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox4" DataField="Active" ></px:PXCheckBox>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit6" DataField="EffectiveFrom" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit7" DataField="EffectiveTo" ></px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds" AllowAutoHide="false">
		<Items>
			<px:PXTabItem Text="Items">
				<Template>
					<px:PXGrid SyncPosition="True" SkinID="Details" Width="100%" runat="server" ID="CstPXGrid1">
						<Levels>
							<px:PXGridLevel DataMember="ItemLines" >
								<Columns>
									<px:PXGridColumn CommitChanges="True" DataField="InventoryID" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="InventoryID_description" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="SiteID" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Uom" Width="72" ></px:PXGridColumn>
									<px:PXGridColumn DataField="StdCost" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="StdPrice" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="RebateCost" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="RebatePrice" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="MinQty" Width="100" ></px:PXGridColumn></Columns>
								<RowTemplate>
									<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector11" DataField="InventoryID" AutoRefresh="True" ></px:PXSelector>
									<px:PXSelector runat="server" ID="CstPXSelector1" DataField="SiteID" AutoRefresh="True" ></px:PXSelector></RowTemplate></px:PXGridLevel></Levels>
						<AutoSize Enabled="True" ></AutoSize>
						<AutoSize MinHeight="150" ></AutoSize>
						<Mode AllowUpload="True" ></Mode></px:PXGrid></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Price Class">
				<Template>
					<px:PXGrid SkinID="Details" Width="100%" runat="server" ID="CstPXGrid2">
						<Levels>
							<px:PXGridLevel DataMember="RBPriceCLass" >
								<Columns>
									<px:PXGridColumn CommitChanges="True" DataField="PriceClassID" Width="120" ></px:PXGridColumn>
									<px:PXGridColumn DataField="PriceClassID_ARPriceClass_description" Width="280" ></px:PXGridColumn></Columns>
								<RowTemplate>
									<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector12" DataField="PriceClassID" ></px:PXSelector></RowTemplate></px:PXGridLevel></Levels>
						<AutoSize Enabled="True" ></AutoSize>
						<AutoSize MinHeight="150" ></AutoSize>
						<Mode AllowUpload="True" /></px:PXGrid></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Customers">
				<Template>
					<px:PXGrid SkinID="Details" Width="100%" runat="server" ID="CstPXGrid3">
						<Levels>
							<px:PXGridLevel DataMember="RBCustomers" >
								<Columns>
									<px:PXGridColumn DataField="BAccountID" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="BAccountID_BAccountR_acctName" Width="280" ></px:PXGridColumn></Columns>
								<RowTemplate>
									<px:PXSegmentMask runat="server" ID="CstPXSegmentMask13" DataField="BAccountID" AllowEdit="True" ></px:PXSegmentMask></RowTemplate></px:PXGridLevel></Levels>
						<AutoSize Enabled="True" ></AutoSize>
						<AutoSize MinHeight="150" ></AutoSize>
						<Mode AllowUpload="True" /></px:PXGrid></Template></px:PXTabItem></Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	</px:PXTab>
</asp:Content>
