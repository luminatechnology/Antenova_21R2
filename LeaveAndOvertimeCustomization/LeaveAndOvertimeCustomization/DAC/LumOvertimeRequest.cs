using System;
using System.Collections.Generic;
using LeaveAndOvertimeCustomization.Graph;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.TM;

namespace LeaveAndOvertimeCustomization.DAC
{
    public class LumOvertimeRequestStatus
    {
        public const string OnHold = "H";
        public const string PendingApproval = "P";
        public const string Approved = "A";
        public const string Rejected = "J";
        public const string Cancel = "C";

        public class onHold : PX.Data.BQL.BqlString.Constant<onHold>
        {
            public onHold() : base(OnHold) { }
        }

        public class pendingApproval : PX.Data.BQL.BqlString.Constant<pendingApproval>
        {
            public pendingApproval() : base(PendingApproval) { }
        }

        public class approved : PX.Data.BQL.BqlString.Constant<approved>
        {
            public approved() : base(Approved) { }
        }

        public class rejected : PX.Data.BQL.BqlString.Constant<rejected>
        {
            public rejected() : base(Rejected) { }
        }

        public class cancel : PX.Data.BQL.BqlString.Constant<cancel>
        {
            public cancel() : base(Cancel) { }
        }

        private static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
        {
            { OnHold, "On Hold" },
            { PendingApproval,"Pending Approval" },
            { Approved, "Approved" },
            { Rejected,"Rejected" },
            { Cancel,"Cancel"}
        };

        public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;

        public class ListAttribute : LabelListAttribute
        {
            public ListAttribute() : base(_valueLabelPairs) { }
        }
    }

    [Serializable]
    [PXEMailSource]
    [PXCacheName("Overtime Request")]
    [PXPrimaryGraph(typeof(OvertimeRequestEntry))]
    public class LumOvertimeRequest : IBqlTable, PX.Data.EP.IAssign
    {
        public class PK : PrimaryKeyOf<LumOvertimeRequest>.By<refNbr>
        {
            public static LumOvertimeRequest Find(PXGraph graph, string refNbr) => FindBy(graph, refNbr);
        }

        #region RefNbr
        [PXDefault]
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [AutoNumber(typeof(LumLeaveAndOvertimePreference.overtimeSequenceID), typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Ref Nbr")]
        [PXSelector(typeof(SelectFrom<LumOvertimeRequest>
                           .InnerJoin<EPEmployee>.On<LumOvertimeRequest.requestEmployeeID.IsEqual<EPEmployee.bAccountID>>
                           .Where<EPEmployee.userID.IsEqual<AccessInfo.userID.FromCurrent>>
                           .SearchFor<LumOvertimeRequest.refNbr>),
                    typeof(LumOvertimeRequest.refNbr),
                    typeof(LumOvertimeRequest.requestEmployeeID),
                    typeof(LumOvertimeRequest.status))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region Status
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXDefault(LumOvertimeRequestStatus.OnHold)]
        [LumOvertimeRequestStatus.List]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region OvertimeType
        [PXDBString]
        [PXDefault("Day in Lieu")]
        [PXStringList(new string[] { "Day in Lieu" }, new string[] { "Day in Lieu" })]
        [PXUIField(DisplayName = "Type")]
        public virtual string OvertimeType { get; set; }
        public abstract class overtimeType : PX.Data.BQL.BqlString.Field<overtimeType> { }
        #endregion

        #region RequestEmployeeID
        [PXDBInt()]
        [PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<AccessInfo.userID.FromCurrent>>>))]
        [PXUIField(DisplayName = "Request Employee ID", Enabled = false)]
        [PXSelector(typeof(
                SelectFrom<EPEmployee>
                .InnerJoin<BAccount2>.On<EPEmployee.bAccountID.IsEqual<BAccount2.bAccountID>>
                .LeftJoin<Contact>.On<BAccount2.defContactID.IsEqual<Contact.contactID>>
                .SearchFor<EPEmployee.bAccountID>),
                typeof(EPEmployee.acctCD),
                typeof(EPEmployee.acctName),
                typeof(EPEmployee.routeEmails),
                typeof(Contact.eMail),
                typeof(EPEmployee.status),
                typeof(EPEmployee.classID),
                SubstituteKey = typeof(EPEmployee.acctCD),
                DescriptionField = typeof(EPEmployee.acctName))]
        public virtual int? RequestEmployeeID { get; set; }
        public abstract class requestEmployeeID : PX.Data.BQL.BqlInt.Field<requestEmployeeID> { }
        #endregion

        #region RequestDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Request Date", Enabled = false)]
        public virtual DateTime? RequestDate { get; set; }
        public abstract class requestDate : PX.Data.BQL.BqlDateTime.Field<requestDate> { }
        #endregion

        #region Description
        [PXDefault]
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region OvertimeStart
        [PXDefault]
        [PXUIField(DisplayName = "Date From")]
        [PXDBDateAndTime(DisplayMask = "g", InputMask = "g", PreserveTime = true, UseTimeZone = true)]
        public virtual DateTime? OvertimeStart { get; set; }
        public abstract class overtimeStart : PX.Data.BQL.BqlDateTime.Field<overtimeStart> { }
        #endregion

        #region OverTimeEnd
        [PXDefault]
        [PXUIField(DisplayName = "Date To")]
        [PXDBDateAndTime(DisplayMask = "g", InputMask = "g", PreserveTime = true, UseTimeZone = true)]
        public virtual DateTime? OverTimeEnd { get; set; }
        public abstract class overTimeEnd : PX.Data.BQL.BqlDateTime.Field<overTimeEnd> { }
        #endregion

        #region WorkDayDuration
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Working Day Duration", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? WorkDayDuration { get; set; }
        public abstract class workDayDuration : PX.Data.BQL.BqlDecimal.Field<workDayDuration> { }
        #endregion

        #region HolidayDuration
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Holiday Duration", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? HolidayDuration { get; set; }
        public abstract class holidayDuration : PX.Data.BQL.BqlDecimal.Field<holidayDuration> { }
        #endregion

        #region NationalHolidayDuration
        [PXDBDecimal()]
        [PXUIField(DisplayName = "National Holiday Duration", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? NationalHolidayDuration { get; set; }
        public abstract class nationalHolidayDuration : PX.Data.BQL.BqlDecimal.Field<nationalHolidayDuration> { }
        #endregion

        #region WorkDayDurationDay
        [PXDecimal()]
        [PXFormula(typeof(Div<workDayDuration, Add<int4, int4>>))]
        [PXUIField(DisplayName = "Working Day Duration(Days)", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? WorkDayDurationDay { get; set; }
        public abstract class workDayDurationDay : PX.Data.BQL.BqlDecimal.Field<workDayDurationDay> { }
        #endregion

        #region HolidayDurationDay
        [PXDecimal()]
        [PXFormula(typeof(Div<holidayDuration, Add<int4, int4>>))]
        [PXUIField(DisplayName = "Holiday Duration (Days)", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? HolidayDurationDay { get; set; }
        public abstract class holidayDurationDay : PX.Data.BQL.BqlDecimal.Field<holidayDurationDay> { }
        #endregion

        #region NationalHolidayDurationDay
        [PXDecimal()]
        [PXFormula(typeof(Div<nationalHolidayDuration, Add<int4, int4>>))]
        [PXUIField(DisplayName = "National Holiday Duration(Days)", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? NationalHolidayDurationDay { get; set; }
        public abstract class nationalHolidayDurationDay : PX.Data.BQL.BqlDecimal.Field<nationalHolidayDurationDay> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
        #endregion

        #region PendingApproval
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "PendingApproval", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual bool? PendingApproval { get; set; }
        public abstract class pendingApproval : PX.Data.BQL.BqlBool.Field<pendingApproval> { }
        #endregion

        #region Approved
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual bool? Approved { get; set; }
        public abstract class approved : PX.Data.BQL.BqlBool.Field<approved> { }
        #endregion

        #region Rejected
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Rejected", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual bool? Rejected { get; set; }
        public abstract class rejected : PX.Data.BQL.BqlBool.Field<rejected> { }
        #endregion

        #region IsApprover
        [PXBool]
        public virtual bool? IsApprover { get; set; }
        public abstract class isApprover : PX.Data.BQL.BqlBool.Field<isApprover> { }
        #endregion

        #region OwnerID
        [Owner(typeof(workgroupID), DisplayName = "Approver")]
        [PXDefault(typeof(Search<CREmployee.defContactID, Where<CREmployee.userID, Equal<Current<AccessInfo.userID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? OwnerID { get; set; }
        public abstract class ownerID : PX.Data.BQL.BqlInt.Field<ownerID> { }
        #endregion

        #region WorkgroupID
        [PXDBInt]
        [PXDefault]
        [PXSelector(typeof(SelectFrom<EPCompanyTree>
                          .InnerJoin<EPCompanyTreeMember>.On<EPCompanyTreeMember.workGroupID.IsEqual<EPCompanyTree.workGroupID>>
                          .Where<EPCompanyTreeMember.contactID.IsEqual<AccessInfo.contactID.FromCurrent>>
                          .SearchFor<EPCompanyTree.workGroupID>),
                    SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Approval Workgroup", Enabled = false)]
        public virtual int? WorkgroupID { get; set; }
        public abstract class workgroupID : PX.Data.BQL.BqlInt.Field<workgroupID> { }
        #endregion

        #region RequestHours
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Request Holiday Hours")]
        [PXDefault(TypeCode.Decimal,"8")]
        [PXFormula(typeof(Mult<requestDays, Add<int4, int4>>))]
        public virtual Decimal? RequestHours { get; set; }
        public abstract class requestHours : PX.Data.BQL.BqlDecimal.Field<requestHours> { }
        #endregion

        #region ApprovedRequestHours
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Approved Holiday Hours", Enabled = false)]
        [PXFormula(typeof(Mult<approvedRequestDays, Add<int4, int4>>))]
        [PXDefault(TypeCode.Decimal, "8")]
        public virtual Decimal? ApprovedRequestHours { get; set; }
        public abstract class approvedRequestHours : PX.Data.BQL.BqlDecimal.Field<approvedRequestHours> { }
        #endregion

        #region RequestDays
        [PXDecimal]
        [PXDBScalar(typeof(SearchFor<LumOvertimeRequest.requestHours>))]
        [PXUIField(DisplayName = "Request Holiday Days")]
        [PXDefault(TypeCode.Decimal, "1")]
        public virtual Decimal? RequestDays { get; set; }
        public abstract class requestDays : PX.Data.BQL.BqlDecimal.Field<requestDays> { }
        #endregion

        #region ApprovedRequestDays
        [PXDecimal()]
        [PXUIField(DisplayName = "Approved Holiday Days", Enabled = false)]
        [PXDBScalar(typeof(SearchFor<LumOvertimeRequest.approvedRequestHours>))]
        [PXDefault(TypeCode.Decimal,"1")]
        public virtual Decimal? ApprovedRequestDays { get; set; }
        public abstract class approvedRequestDays : PX.Data.BQL.BqlDecimal.Field<approvedRequestDays> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
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