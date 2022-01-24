<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM204000.aspx.cs" Inherits="Pages_LM204000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource EnableAttributes="true" ID="ds" runat="server" Visible="True" Width="100%" TypeName="AntenovaCustomizations.Graph.ENGineeringMaint" PrimaryView="Document">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" />
            <px:PXDSCallbackCommand Name="Delete" PopupVisible="true" ClosePopup="True" />
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" PopupVisible="True" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Engineering Summary" DataMember="Document" FilesIndicator="True"
        NoteIndicator="True" LinkIndicator="True" NotifyIndicator="True" DefaultControlID="edEngrNbr" TabIndex="100">
        <CallbackCommands>
            <Save PostData="Self" />
        </CallbackCommands>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="true" LabelsWidth="S" ControlSize="S" />
            <px:PXSelector runat="server" ID="edEngrRef" DataField="EngrRef" CommitChanges="true" FilterByAllFields="True"></px:PXSelector>
            <px:PXDropDown runat="server" ID="edStatus" DataField="Status"></px:PXDropDown>
            <px:PXSelector runat="server" ID="edOpprid" DataField="Opprid" CommitChanges="true" AllowEdit="true" Size="L"></px:PXSelector>
            <px:PXSelector runat="server" ID="edOppBAccountID" DataField="OppBAccountID" Size="L"></px:PXSelector>
            <px:PXSelector runat="server" ID="edEndCust" DataField="EndCust" Size="L"></px:PXSelector>
            <px:PXTextEdit runat="server" ID="edMsh" DataField="Msh"></px:PXTextEdit>
            <px:PXLayoutRule runat="server" ColumnSpan="3" ControlSize="XM" />
            <px:PXTextEdit runat="server" ID="edDescription" DataField="Description" TextMode="MultiLine"></px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXTextEdit runat="server" ID="edEngNbr" DataField="EngNbr" Size="L"></px:PXTextEdit>
            <px:PXDropDown runat="server" ID="edRepeat" DataField="Repeat"></px:PXDropDown>
            <px:PXDropDown runat="server" ID="edPriority" DataField="Priority"></px:PXDropDown>
            <px:PXDropDown runat="server" ID="edPrjtype" DataField="Prjtype" Size="M" CommitChanges="True"></px:PXDropDown>
            <px:PXDropDown runat="server" ID="edGateStatus" DataField="GateStatus"></px:PXDropDown>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXSelector runat="server" ID="edEngineer" DataField="Engineer" Size="M"></px:PXSelector>
            <px:PXSelector runat="server" ID="edSalesPerson" DataField="SalesPerson" Size="M" CommitChanges="True"></px:PXSelector>
            <px:PXSelector runat="server" ID="edSalesRegion" DataField="SalesRegion"></px:PXSelector>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="280px" DataSourceID="ds" DataMember="Line">
        <Items>
            <px:PXTabItem Text="ENGINEER" RepaintOnDemand="False">
                <Template>
                    <px:PXFormView ID="edOpportunityCurrent" runat="server" DataMember="Line" DataSourceID="ds" RenderStyle="Simple">
                        <ContentStyle BackColor="Transparent" />
                    </px:PXFormView>
                    <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True" LabelsWidth="S" ControlSize="XM" />
                    <px:PXFormView ID="edENGLine" runat="server" DataMember="Line" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                            <px:PXDateTimeEdit runat="server" ID="edDeviceRcvDate" DataField="Document.DeviceRcvDate" Size="SM"></px:PXDateTimeEdit>
                            <px:PXCheckBox runat="server" ID="edIsGerber" DataField="IsGerber"></px:PXCheckBox>
                            <px:PXDateTimeEdit runat="server" ID="edEstStart" DataField="EstStart"></px:PXDateTimeEdit>
                            <px:PXDateTimeEdit runat="server" ID="edEstComplete" DataField="EstComplete"></px:PXDateTimeEdit>
                            <px:PXLayoutRule runat="server" ColumnSpan="3"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="edEngrNote" DataField="EngrNote" TextMode="MultiLine"></px:PXTextEdit>
                            <px:PXLayoutRule runat="server" ColumnSpan="3"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="edcompleteSummary" DataField="completeSummary" TextMode="MultiLine"></px:PXTextEdit>
                            <px:PXDateTimeEdit runat="server" ID="edAwaitdateFrom" DataField="AwaitdateFrom" Size="SM"></px:PXDateTimeEdit>
                            <px:PXDateTimeEdit runat="server" ID="edAwaitdateTo" DataField="AwaitdateTo"></px:PXDateTimeEdit>
                            <px:PXLayoutRule runat="server" ColumnSpan="3"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="edAwaitReason" DataField="AwaitReason" TextMode="MultiLine"></px:PXTextEdit>
                            <px:PXDateTimeEdit runat="server" ID="edOnholdDate" DataField="OnholdDate" Size="SM"></px:PXDateTimeEdit>
                            <px:PXLayoutRule runat="server" ColumnSpan="3"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="edOnholdReason" DataField="OnholdReason" TextMode="MultiLine"></px:PXTextEdit>
                            <px:PXDateTimeEdit runat="server" ID="edCloseDate" DataField="CloseDate" Size="SM"></px:PXDateTimeEdit>
                            <px:PXLayoutRule runat="server" ColumnSpan="3"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="edCloseReason" DataField="CloseReason" TextMode="MultiLine"></px:PXTextEdit>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                            <px:PXDateTimeEdit runat="server" ID="edRequstRcvDate" DataField="Document.RequstRcvDate"></px:PXDateTimeEdit>
                            <px:PXSelector runat="server" ID="edGerberNbr" DataField="GerberNbr"></px:PXSelector>
                            <px:PXDateTimeEdit runat="server" ID="edActStart" DataField="ActStart" Size="SM"></px:PXDateTimeEdit>
                            <px:PXDateTimeEdit runat="server" ID="edActComplete" DataField="ActComplete"></px:PXDateTimeEdit>
                            <px:PXLayoutRule runat="server" />
                        </Template>
                        <ContentStyle BackColor="Transparent" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="NRE Input" RepaintOnDemand="False">
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartRow="True" LabelsWidth="S" ControlSize="XM" />
                    <px:PXFormView ID="edENGCurrentLine" runat="server" DataMember="CurrentLine" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                            <px:PXCheckBox runat="server" ID="edIsNRE" DataField="IsNRE"></px:PXCheckBox>
                            <px:PXSelector runat="server" ID="edSONbr" DataField="SONbr" AllowEdit="true"></px:PXSelector>
                        </Template>
                        <ContentStyle BackColor="Transparent" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="OTHER INFOR" RepaintOnDemand="False">
                <Template>
                    <px:PXFormView ID="edENGCurrentLin2" runat="server" DataMember="CurrentLine" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                            <px:PXNumberEdit runat="server" ID="edEngrAgeDays" DataField="EngrAgeDays"></px:PXNumberEdit>
                            <px:PXDateTimeEdit runat="server" ID="edProcessDate" DataField="ProcessDate"></px:PXDateTimeEdit>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                            <px:PXTextEdit runat="server" ID="edCreatedByID" DataField="CreatedByID" Enabled="False"></px:PXTextEdit>
                            <px:PXDateTimeEdit runat="server" ID="edCreatedDateTime" DataField="CreatedDateTime" Enabled="false"></px:PXDateTimeEdit>
                            <px:PXTextEdit runat="server" ID="edLastModifiedByID" DataField="LastModifiedByID" Enabled="false"></px:PXTextEdit>
                            <px:PXDateTimeEdit runat="server" ID="edLastModifiedDateTime" DataField="LastModifiedDateTime" Enabled="false"></px:PXDateTimeEdit>
                        </Template>
                        <ContentStyle BackColor="Transparent" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="PRODUCTS">
                <Template>
                    <px:PXGrid ID="edREVenueLine" SkinID="Details" runat="server" Width="100%" Height="500px" DataSourceID="ds" ActionsPosition="Top" BorderWidth="0px" SyncPosition="true">
                        <AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
                        <Mode AllowUpload="True" AllowDragRows="true" />
                        <CallbackCommands PasteCommand="PasteLine">
                            <Save PostData="Container" />
                        </CallbackCommands>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Insert Row" SyncText="false" ImageSet="main" ImageKey="AddNew">
                                    <AutoCallBack Target="ProductsGrid" Command="AddNew" Argument="1"></AutoCallBack>
                                    <ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Cut Row" SyncText="false" ImageSet="main" ImageKey="Copy">
                                    <AutoCallBack Target="ProductsGrid" Command="Copy"></AutoCallBack>
                                    <ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Insert Cut Row" SyncText="false" ImageSet="main" ImageKey="Paste">
                                    <AutoCallBack Target="ProductsGrid" Command="Paste"></AutoCallBack>
                                    <ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="RevenueLine">
                                <Mode InitNewRow="true" />
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edSubItemID" runat="server" DataField="SubItemID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="edREVenueLine" Name="ENGRevenueLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="edREVenueLine" Name="ENGRevenueLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSelector>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="CCCCCCCCCCCCCCCCCCCC" AutoCallBack="True" AllowDragDrop="true" Width="150px" />
                                    <px:PXGridColumn DataField="Descr" Width="200px" />
                                    <px:PXGridColumn DataField="Quantity" TextAlign="Right" AutoCallBack="True" AllowDragDrop="true" Width="100px" />
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" AutoCallBack="True" AllowDragDrop="true" Width="100px" />
                                    <px:PXGridColumn DataField="UnitPrice" Width="100px" AutoCallBack="True"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ExtPrice" Width="100px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="GERBER INFO" RepaintOnDemand="False">
                <Template>
                    <px:PXFormView ID="edENGCurrentLin3" runat="server" DataMember="CurrentLine" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
                            <px:PXDropDown runat="server" ID="edGeberFile" DataField="GeberFile"></px:PXDropDown>
                            <px:PXDropDown runat="server" ID="edFile3D" DataField="File3D"></px:PXDropDown>
                            <px:PXDropDown runat="server" ID="edStackUpFile" DataField="StackUpFile"></px:PXDropDown>
                            <px:PXDropDown runat="server" ID="edDeviceTopology" DataField="DeviceTopology" AllowMultiSelect="True"></px:PXDropDown>
                            <px:PXDropDown runat="server" ID="edPCBATopology" DataField="PCBATopology"></px:PXDropDown>
                        </Template>
                        <ContentStyle BackColor="Transparent" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="250" MinWidth="300" />
    </px:PXTab>
    <script type="text/javascript">

        window.onload = function () {
            window.setTimeout(function () {
                var element = document.getElementsByTagName('textarea')
                for (var i = 0; i < element.length; i++) {
                    if (element[i].classList.contains('auto-size')) {
                        element[i].style.width = "795px"
                        element[i].style.height = "150px"
                        element[i].classList.remove('auto-size')
                        element[i].classList.add('size-m')
                    }
                }
            }, 1000);
        };
    </script>

</asp:Content>
