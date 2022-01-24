<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="LM102000.aspx.cs" Inherits="Pages_LM102000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%" PrimaryView="CustList" TypeName="AntenovaCustomizations.Graph.CRMendCustMaint">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="280px" DataSourceID="ds" DataMember="CustList">
        <Items>
            <px:PXTabItem Text="End Customer" RepaintOnDemand="False">
                <Template>
                    <px:PXGrid ID="grdCust" runat="server" Height="400px" Width="100%"
                        ActionsPosition="Top" AllowSearch="true" DataSourceID="ds" SkinID="Details">
                        <AutoSize Enabled="True" MinHeight="300" />
                        <Levels>
                            <px:PXGridLevel DataMember="CustList">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                                    <px:PXTextEdit runat="server" DataField="CustId"></px:PXTextEdit>
                                    <px:PXTextEdit runat="server" DataField="Name"></px:PXTextEdit>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="CustId" Width="150px" />
                                    <px:PXGridColumn DataField="Name" Width="200px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
                        <CallbackCommands>
                            <Save PostData="Page" />
                        </CallbackCommands>
                        <Mode AllowUpload="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Source" RepaintOnDemand="False">
                <Template>
                    <px:PXGrid ID="grdSource" runat="server" Height="400px" Width="100%"
                        ActionsPosition="Top" AllowSearch="true" DataSourceID="ds" SkinID="Details">
                        <AutoSize Enabled="True" MinHeight="300" />
                        <Levels>
                            <px:PXGridLevel DataMember="SourceList">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                                    <px:PXTextEdit runat="server" DataField="SourceID"></px:PXTextEdit>
                                    <px:PXTextEdit runat="server" DataField="Descrption"></px:PXTextEdit>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SourceID" Width="150px" />
                                    <px:PXGridColumn DataField="Descrption" Width="200px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
                        <CallbackCommands>
                            <Save PostData="Page" />
                        </CallbackCommands>
                        <Mode AllowUpload="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
    </px:PXTab>
</asp:Content>
