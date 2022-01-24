<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="LM101000.aspx.cs" Inherits="Pages_LM101000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%" PrimaryView="Header" TypeName="AntenovaCustomizations.Graph.ENGSetupMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="server">
    <px:PXFormView ID="form" runat="server" Style="z-index: 100" Width="100%" DataMember="Header" Caption="Numbering Sequence" TemplateContainer=""
         MarkRequired="Dynamic">
		<Parameters>
			<px:PXQueryStringParam Name="Numbering.NumberingID" QueryStringField="NumberingID" Type="String" OnLoadOnly="true" />
		</Parameters>
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True"  LabelsWidth="SM" ControlSize="M"  />
			<px:PXSelector ID="edENGSequenceID" runat="server" DataField="ENGSequenceID" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" Style="z-index: 100;" Width="100%" Caption="ENG Project Type Detail" SkinID="Details" Height="300px">
        <Levels>
            <px:PXGridLevel DataMember="TypeList">
                <RowTemplate>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                    <px:PXTextEdit runat="server" DataField="Prjtype"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="Description"></px:PXTextEdit>
                    <px:PXNumberEdit runat="server" DataField="Sort"></px:PXNumberEdit>
                    <px:PXCheckBox runat="server" DataField="Active"></px:PXCheckBox>
                    <px:PXCheckBox runat="server" DataField="LinkOppr"></px:PXCheckBox>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="Prjtype" AllowNull="False" />
                    <px:PXGridColumn DataField="Description" Width="200px" />
                    <px:PXGridColumn DataField="Sort" Width="100px" />
                    <px:PXGridColumn DataField="Active" Width="100px" Type="CheckBox" />
                    <px:PXGridColumn DataField="LinkOppr" Width="100px" Type="CheckBox" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <CallbackCommands>
            <Save PostData="Page" />
        </CallbackCommands>
    </px:PXGrid>
</asp:Content>
