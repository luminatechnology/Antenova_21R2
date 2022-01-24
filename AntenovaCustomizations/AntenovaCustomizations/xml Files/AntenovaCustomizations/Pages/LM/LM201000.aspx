<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM201000.aspx.cs" Inherits="Page_LM201000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="AntenovaCustomizations.LUMItemLotSerFileMaint" PrimaryView="ItemLotSerial">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid AllowSearch="True" FastFilterFields="InventoryID,LotSerialNbr,InventoryID_description" PreservePageIndex="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="ItemLotSerial">
			    <Columns>
				<px:PXGridColumn DataField="InventoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LotSerialNbr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="QtyAvail" Width="100" ></px:PXGridColumn></Columns>
			
				<RowTemplate>
					<px:PXSegmentMask runat="server" ID="CstPXSegmentMask1" DataField="InventoryID" AllowEdit="True" ></px:PXSegmentMask></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar PagerVisible="Bottom" >
			<PagerSettings Mode="NumericCompact" ></PagerSettings>
		</ActionBar>
	
		<Mode AllowAddNew="False" ></Mode>
		<Mode AllowDelete="False" ></Mode>
		<Mode AllowUpdate="False" ></Mode></px:PXGrid>
</asp:Content>