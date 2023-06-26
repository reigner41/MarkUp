<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PC100010.aspx.cs" Inherits="Page_PC100010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PrecisionCust.MarkupMaint"
        PrimaryView="MarkupParent"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
	<px:PXFormView Visible="False" Width="100%" DataMember="MarkupParent" Height="30px" runat="server" ID="CstFormView1" >
		<Template>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartRow="True" ></px:PXLayoutRule>
			<px:PXTextEdit Width="" runat="server" ID="CstPXParentMarkupID" DataField="ParentMarkupID" ></px:PXTextEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit3" DataField="LineCntr" ></px:PXNumberEdit></Template></px:PXFormView></asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="MarkupDet">
			    <Columns>
				<px:PXGridColumn DataField="LineNbr" Width="70" />
				<px:PXGridColumn CommitChanges="True" DataField="MarkupCD" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="BranchID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="Customer" DataField="CustomerID" Width="140" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID_BAccountR_acctName" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="ItemClass" CommitChanges="True" DataField="ItemClassID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="SiteID" CommitChanges="True" DataField="SiteID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="InventoryID" DataField="InventoryID" Width="70" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="QtyBreak" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EffectiveDate" Width="90" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="ExpirationDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="MarkupType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="MarkupValue" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ReplacementCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SalesOrderPrice" Width="100" ></px:PXGridColumn></Columns>
			
				<RowTemplate>
					<px:PXSelector AutoRefresh="True" FilterByAllFields="True" runat="server" ID="CstPXInventoryID" DataField="InventoryID" CommitChanges="True" ></px:PXSelector>
								<px:PXSelector runat="server" ID="CstPXSiteID" DataField="SiteID" CommitChanges="True" ></px:PXSelector></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<Mode InitNewRow="True" ></Mode></px:PXGrid>
</asp:Content>
