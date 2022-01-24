using System;
using PX.Data;
using PX.Objects.CS;

namespace LeaveAndOvertimeCustomization.DAC
{
    [Serializable]
    [PXCacheName("LumEmployeeAnnualLeaveLine")]
    public class LumEmployeeAnnualLeaveLine : IBqlTable
    {

        #region FK
        public class FK
        {
            public class Nbr : LumEmployeeAnnualLeave.PK.ForeignKeyOf<LumEmployeeAnnualLeaveLine>.By<employeeID, leaveType> { }
        }
        #endregion

        #region EmployeeID
        [PXDBInt(IsKey = true)]
        [PXParent(typeof(FK.Nbr))]
        [PXUIField(DisplayName = "Employee ID")]
        [PXDefault(typeof(LumEmployeeAnnualLeave.employeeID))]
        public virtual int? EmployeeID { get; set; }
        public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
        #endregion

        #region LeaveType
        [PXDBString(20, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Leave Type")]
        [PXDefault(typeof(LumEmployeeAnnualLeave.leaveType))]
        public virtual string LeaveType { get; set; }
        public abstract class leaveType : PX.Data.BQL.BqlString.Field<leaveType> { }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr", Enabled = false)]
        [PXLineNbr(typeof(LumEmployeeAnnualLeave.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region StartDate
        [PXDBDate()]
        [PXDefault]
        [PXUIField(DisplayName = "Start Date")]
        public virtual DateTime? StartDate { get; set; }
        public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
        #endregion

        #region EndDate
        [PXDBDate()]
        [PXDefault]
        [PXUIField(DisplayName = "End Date")]
        public virtual DateTime? EndDate { get; set; }
        public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
        #endregion

        #region LeaveHours
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "0")]
        [PXFormula(typeof(Mult<annualLeaveDays,Add<int4,int4>>))]
        [PXUIField(DisplayName = "Annual Leave Hours ")]
        public virtual decimal? LeaveHours { get; set; }
        public abstract class leaveHours : PX.Data.BQL.BqlDecimal.Field<leaveHours> { }
        #endregion

        #region CarryForwardHours
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "0")]
        [PXUIField(DisplayName = "Carry Forward Hours")]
        [PXFormula(typeof(Mult<carryForwardDays, Add<int4, int4>>))]
        public virtual decimal? CarryForwardHours { get; set; }
        public abstract class carryForwardHours : PX.Data.BQL.BqlDecimal.Field<carryForwardHours> { }
        #endregion

        #region AnnualLeaveDays
        [PXDecimal()]
        [PXDefault(TypeCode.Decimal, "0")]
        [PXUIField(DisplayName = "Annual Leave Days ")]
        public virtual decimal? AnnualLeaveDays { get; set; }
        public abstract class annualLeaveDays : PX.Data.BQL.BqlDecimal.Field<annualLeaveDays> { }
        #endregion

        #region CarryForwardDays
        [PXDecimal()]
        [PXDefault(TypeCode.Decimal, "0")]
        [PXUIField(DisplayName = "Carry Forward Days")]
        public virtual decimal? CarryForwardDays { get; set; }
        public abstract class carryForwardDays : PX.Data.BQL.BqlDecimal.Field<carryForwardDays> { }
        #endregion

        #region EntitledDays
        [PXDecimal]
        [PXDefault(TypeCode.Decimal, "0")]
        [PXUIField(DisplayName = "Entitled Days", Enabled = false)]
        public virtual decimal EntitledDays { get; set; }
        public abstract class entitledDays : PX.Data.BQL.BqlDecimal.Field<entitledDays> { }
        #endregion

        #region AllowCarryForward
        [PXDBBool()]
        [PXUIField(DisplayName = "Allow Carry Forward")]
        public virtual bool? AllowCarryForward { get; set; }
        public abstract class allowCarryForward : PX.Data.BQL.BqlBool.Field<allowCarryForward> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
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