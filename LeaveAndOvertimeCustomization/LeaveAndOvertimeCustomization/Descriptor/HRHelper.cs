﻿using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Data;
using LeaveAndOvertimeCustomization.DAC;
using PX.Common;

namespace LeaveAndOvertimeCustomization.Descriptor
{
    public class HRHelper
    {
        /// <summary> 取得Employee年假可用時數 </summary>
        public decimal GetAvailableAnnualLeaveTime(EPEmployee employee, string leaveType, string refNbr, DateTime? startTime, DateTime? endTime)
        {
            decimal usedTime = 0;
            var empLeaveLine = SelectFrom<LumEmployeeAnnualLeaveLine>
                            .Where<P.AsDateTime.IsBetween<LumEmployeeAnnualLeaveLine.startDate, LumEmployeeAnnualLeaveLine.endDate>
                                  .And<LumEmployeeAnnualLeaveLine.employeeID.IsEqual<P.AsInt>>
                                  .And<LumEmployeeAnnualLeaveLine.leaveType.IsEqual<P.AsString>>>
                            .View.Select(new PXGraph(), startTime, employee.BAccountID, leaveType).RowCast<LumEmployeeAnnualLeaveLine>().FirstOrDefault();
            var availTime = (empLeaveLine?.LeaveHours ?? 0) + (empLeaveLine?.CarryForwardHours ?? 0);
            var usedRequests = SelectFrom<LumLeaveRequest>
                           .Where<LumLeaveRequest.requestEmployeeID.IsEqual<P.AsInt>
                                .And<LumLeaveRequest.status.IsEqual<P.AsString>>
                                .And<LumLeaveRequest.leaveType.IsEqual<P.AsString>>>
                          .View.Select(new PXGraph(), employee.BAccountID, LumLeaveRequestStatus.Approved, leaveType).RowCast<LumLeaveRequest>().ToList();
            usedRequests = usedRequests.Where(x => x.LeaveStart >= empLeaveLine?.StartDate && x.LeaveStart <= empLeaveLine?.EndDate).ToList();
            foreach (var item in usedRequests)
                usedTime += GetLeaveDuration(employee, item.LeaveStart, item.LeaveEnd, true);
            return availTime - usedTime;
        }

        /// <summary> 取得非年假休假可用時數 </summary>
        public decimal GetAvailableLeaveTime(EPEmployee employee, string leaveType, string refNbr, DateTime? startTime, DateTime? endTime, bool? isOnlyWorkDay)
        {
            decimal usedTime = 0;
            var availTime = leaveType == "Day in Lieu" ?
                            SelectFrom<LumEmployeeCompensated>
                            .Where<LumEmployeeCompensated.employeeID.IsEqual<P.AsInt>>
                            .View.Select(new PXGraph(), employee.BAccountID).RowCast<LumEmployeeCompensated>().ToList()
                            .Where(x => x.AvailableYear == DateTime.Now.Year).Sum(x => x.TransferHours) :
                            SelectFrom<LumLeaveType>
                            .Where<LumLeaveType.leaveType.IsEqual<P.AsString>>
                            .View.Select(new PXGraph(), leaveType).RowCast<LumLeaveType>().FirstOrDefault()?.MaxLeaveDays * 8;
            var approvedRequests = SelectFrom<LumLeaveRequest>
                          .Where<LumLeaveRequest.requestEmployeeID.IsEqual<P.AsInt>
                               .And<LumLeaveRequest.status.IsEqual<P.AsString>>
                               .And<LumLeaveRequest.leaveType.IsEqual<P.AsString>>>
                         .View.Select(new PXGraph(), employee.BAccountID, LumLeaveRequestStatus.Approved, leaveType).RowCast<LumLeaveRequest>().ToList();
            foreach (var item in approvedRequests.Where(x => x.LeaveStart?.Year == DateTime.Now.Year || (GetLeaveTypeInfo(leaveType).IsBindingEmployee ?? false)))
                usedTime += GetLeaveDuration(employee, item.LeaveStart, item.LeaveEnd, isOnlyWorkDay);
            return (availTime ?? 0) - usedTime;
        }

        /// <summary> 計算請假時數(Hour) </summary>
        public decimal GetLeaveDuration(EPEmployee employee, DateTime? LeaveStart, DateTime? LeaveEnd, bool? IsOnlyWorkDay)
        {
            if (employee != null)
            {
                var userTimezone = PXContext.PXIdentity.TimeZone;
                // 判斷是否只請下午
                bool isHalfday = false;
                // breaktime
                var breaktimeInfo = GetBreakTimeInfo(employee);
                // worktime
                CSCalendar calendar = SelectFrom<CSCalendar>.Where<CSCalendar.calendarID.IsEqual<P.AsString>>.View.Select(new PXGraph(), employee.CalendarID);
                var calendarTimeZoneInfo = PXTimeZoneInfo.FindSystemTimeZoneById(calendar.TimeZone);
                var workTimeInfo = GetWorkTimeInfo(employee, calendar, calendarTimeZoneInfo);
                if (workTimeInfo == null)
                    throw new PXException("can not find working day");

                // 請假人的工作Calander時區請假開始時間
                var timezoneStartTime = userTimezone.ShortName == "GMT+00:00" ? 
                                            TimeZoneInfo.ConvertTimeFromUtc(LeaveStart.Value, TimeZoneInfo.FindSystemTimeZoneById(calendarTimeZoneInfo.RegionId)) :
                                            TimeZoneInfo.ConvertTimeFromUtc(LeaveStart.Value.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(calendarTimeZoneInfo.RegionId));
                // 請假人的工作Calander時區請假結束時間
                var timezoneEndTime = userTimezone.ShortName == "GMT+00:00" ?
                                            TimeZoneInfo.ConvertTimeFromUtc(LeaveEnd.Value, TimeZoneInfo.FindSystemTimeZoneById(calendarTimeZoneInfo.RegionId)) :
                                            TimeZoneInfo.ConvertTimeFromUtc(LeaveEnd.Value.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(calendarTimeZoneInfo.RegionId));
                // minute
                double duration = 0;
                while (timezoneStartTime.Date <= timezoneEndTime.Date)
                {
                    var actWorkTime = workTimeInfo.FirstOrDefault(x => x.DayofWeek.ToLower() == timezoneStartTime.DayOfWeek.ToString().ToLower());
                    var actBreakTime = breaktimeInfo.FirstOrDefault(x => x.DayofWeek.ToLower() == timezoneStartTime.DayOfWeek.ToString().ToLower());

                    // 當日是特定假日
                    if (CheckIsExceptionWorkDay(employee, timezoneStartTime.Date))
                    {
                        if (!(IsOnlyWorkDay ?? false))
                            duration += 480;
                        timezoneStartTime = timezoneStartTime.AddDays(1);
                        continue;
                    }

                    // 當日非工作日
                    if (actWorkTime == null)
                    {
                        if (!(IsOnlyWorkDay ?? false))
                            duration += 480;
                        timezoneStartTime = timezoneStartTime.AddDays(1);
                        continue;
                    }

                    // 校正起始時間
                    var actStartTime = timezoneStartTime.TimeOfDay <= actWorkTime.StartTime?.TimeOfDay ? timezoneStartTime.Date + actWorkTime.StartTime?.TimeOfDay : timezoneStartTime;
                    // 跨日起始時間為上班起始時間
                    if (actStartTime?.Date != LeaveStart?.Date)
                        actStartTime = actStartTime?.Date + actWorkTime?.StartTime?.TimeOfDay;
                    // 開始日期位於下班後
                    if (actStartTime?.TimeOfDay >= actWorkTime?.EndTime?.TimeOfDay)
                        actStartTime = actStartTime?.Date + actWorkTime?.EndTime?.TimeOfDay;

                    // 校正結束時間
                    var actEndTime = timezoneEndTime.TimeOfDay >= actWorkTime.EndTime?.TimeOfDay || LeaveEnd?.Date != actStartTime?.Date ? timezoneStartTime.Date + actWorkTime.EndTime?.TimeOfDay : timezoneStartTime.Date + timezoneEndTime.TimeOfDay;
                    // 跨日結束時間為上班結束時間
                    if (actEndTime?.Date != timezoneEndTime.Date)
                        actEndTime = actEndTime?.Date + actWorkTime?.EndTime?.TimeOfDay;
                    // 結束日期位於上班前
                    if (actEndTime?.TimeOfDay <= actWorkTime?.StartTime?.TimeOfDay)
                        actEndTime = actEndTime?.Date + actWorkTime?.StartTime?.TimeOfDay;

                    // 檢查是否只請下午
                    if (actStartTime.Value.TimeOfDay >= new DateTime(2022, 1, 1, 13, 00, 00).TimeOfDay && actStartTime.Value.TimeOfDay <= actWorkTime.EndTime?.TimeOfDay)
                        isHalfday = true;

                    // 扣除休息時間 開始時間 小於 休息開始時間 && 結束時間 > "當日"休息結束時間
                    if (actBreakTime != null && actStartTime?.TimeOfDay < actBreakTime.StartTime?.TimeOfDay && timezoneEndTime > actStartTime?.Date + actBreakTime.EndTime?.TimeOfDay)
                    {
                        duration -= (double)actBreakTime.BreakTime;
                        duration += new TimeSpan(actEndTime.Value.Ticks - actStartTime.Value.Ticks).TotalMinutes;
                    }
                    // 請假時間介於休息時間內
                    else if (actBreakTime != null && (actStartTime?.TimeOfDay >= actBreakTime.StartTime?.TimeOfDay && actStartTime?.TimeOfDay <= actBreakTime.EndTime?.TimeOfDay) &&
                        (actEndTime?.TimeOfDay >= actBreakTime.StartTime?.TimeOfDay && actEndTime?.TimeOfDay <= actBreakTime.EndTime?.TimeOfDay))
                    {
                        timezoneStartTime = timezoneStartTime.AddDays(1);
                        continue;
                    }
                    else
                    {
                        // 起始時間位於休息時間，校正為休息結束時間
                        if (actBreakTime != null && actStartTime?.TimeOfDay >= actBreakTime.StartTime?.TimeOfDay && actStartTime?.TimeOfDay <= actBreakTime.EndTime?.TimeOfDay)
                            actStartTime = actStartTime?.Date + actBreakTime.EndTime?.TimeOfDay;

                        // 結束時間位於休息時間，校正為休息起始時間
                        if (actBreakTime != null && actEndTime?.TimeOfDay >= actBreakTime.StartTime?.TimeOfDay && actEndTime?.TimeOfDay <= actBreakTime.EndTime?.TimeOfDay)
                            actEndTime = actEndTime?.Date + actBreakTime.StartTime?.TimeOfDay;
                        duration += new TimeSpan(actEndTime.Value.Ticks - actStartTime.Value.Ticks).TotalMinutes;
                        if (isHalfday && duration > 240 && duration <= 300)
                            duration = 240;
                    }
                    timezoneStartTime = timezoneStartTime.AddDays(1);
                }
                duration = ((duration < 0 ? 0 : duration) / 60);
                return duration % 8 == 0 ? (decimal)duration :
                       duration % 8 > 4 ? (decimal)(duration - (duration % 8) + 8) :
                       (decimal)(duration - (duration % 8) + 4);
            }
            return 0;
        }

        /// <summary> 計算加班時數(Hour) </summary>
        public (decimal wkTimes, decimal HolidayTimes, decimal NationalTimes) GetOvertimeDuration(EPEmployee employee, DateTime? OTStart, DateTime? OTEnd)
        {
            double wkTimes = 0;
            double holidayTimes = 0;
            double nationalTimes = 0;
            if (employee != null)
            {
                // worktime
                CSCalendar calendar = SelectFrom<CSCalendar>.Where<CSCalendar.calendarID.IsEqual<P.AsString>>.View.Select(new PXGraph(), employee.CalendarID);
                var calendarTimeZoneInfo = PXTimeZoneInfo.FindSystemTimeZoneById(calendar.TimeZone);
                var workTimeInfo = GetWorkTimeInfo(employee, calendar, calendarTimeZoneInfo);

                var tmpStartTime = OTStart.Value;
                while (tmpStartTime.Date <= OTEnd?.Date)
                {
                    DateTime? actStartTime = tmpStartTime;
                    // 如果結束日 > 起始日，則將結束時間設為當天24時
                    DateTime? actEndTime = tmpStartTime.Date == OTEnd?.Date ? OTEnd.Value : tmpStartTime.Date.AddDays(1);
                    // 判斷是否為工作日
                    var actWorkTime = workTimeInfo.FirstOrDefault(x => x.DayofWeek.ToLower() == tmpStartTime.DayOfWeek.ToString().ToLower());
                    // 國定假日
                    if (CheckIsExceptionWorkDay(employee, tmpStartTime))
                        nationalTimes += new TimeSpan(actEndTime.Value.Ticks - actStartTime.Value.Ticks).TotalMinutes;
                    // 例假日
                    else if (actWorkTime == null)
                        holidayTimes += new TimeSpan(actEndTime.Value.Ticks - actStartTime.Value.Ticks).TotalMinutes;
                    // 工作日
                    else
                    {
                        // 如果加班起始日期 比上班日期早 且 加班結束日比下班時間晚 -> 先計算起始日到上班起始時間，再將加班起始改為下班時間
                        if (actStartTime?.TimeOfDay < actWorkTime.StartTime?.TimeOfDay && actEndTime > (actStartTime?.Date + actWorkTime?.StartTime?.TimeOfDay))
                        {
                            wkTimes += new TimeSpan(actWorkTime.StartTime.Value.TimeOfDay.Ticks - actStartTime.Value.TimeOfDay.Ticks).TotalMinutes;
                            actStartTime = actStartTime?.Date + actWorkTime?.StartTime?.TimeOfDay;
                        }

                        // T WS WE -> T ; WS T WE -> WE; WS WE T -> T
                        actStartTime = actStartTime?.TimeOfDay < actWorkTime.StartTime?.TimeOfDay ? actStartTime : actStartTime?.TimeOfDay >= actWorkTime.StartTime?.TimeOfDay && actStartTime?.TimeOfDay <= actWorkTime.EndTime?.TimeOfDay
                                                                                                  ? actStartTime?.Date + actWorkTime.EndTime?.TimeOfDay : actStartTime;
                        actEndTime = actEndTime?.TimeOfDay < actWorkTime.StartTime?.TimeOfDay ? actEndTime : actEndTime?.TimeOfDay >= actWorkTime.StartTime?.TimeOfDay && actEndTime?.TimeOfDay <= actWorkTime.EndTime?.TimeOfDay
                                                                                              ? actEndTime?.Date + actWorkTime.StartTime?.TimeOfDay : actEndTime;
                        if (actEndTime >= actStartTime)
                            wkTimes += new TimeSpan(actEndTime.Value.Ticks - actStartTime.Value.Ticks).TotalMinutes;
                    }
                    tmpStartTime = tmpStartTime.Date.AddDays(1);
                }
            }
            return ((decimal)wkTimes / 60, (decimal)holidayTimes / 60, (decimal)nationalTimes / 60);
        }

        /// <summary> Calculate Employee Total Overtime Hours in request month </summary>
        public decimal? GetApprovedOvertimeByMonth(EPEmployee employee, DateTime? OTStart, string refNbr)
        {
            var approvedData = SelectFrom<LumOvertimeRequest>
                               .Where<LumOvertimeRequest.status.IsEqual<LumOvertimeRequestStatus.approved>
                                 .And<LumOvertimeRequest.requestEmployeeID.IsEqual<P.AsInt>>
                                 .And<LumOvertimeRequest.refNbr.IsNotEqual<P.AsString>>>
                               .View.Select(new PXGraph(), employee.BAccountID, refNbr).RowCast<LumOvertimeRequest>().ToList();
            return approvedData.Where(x => x.OvertimeStart?.Month == OTStart?.Month).Sum(x => x?.HolidayDuration + x?.NationalHolidayDuration + x?.WorkDayDuration);
        }

        /// <summary> Get Work Time </summary>
        public List<WorktimeInfo> GetWorkTimeInfo(EPEmployee employee, CSCalendar calendar, PXTimeZoneInfo calendarTimeZoneInfo)
        {
            var workCalendar = new List<WorktimeInfo>();
            if (employee == null)
                return null;
            #region Build WorkDay object
            if (calendar.SunWorkDay ?? false)
            {
                workCalendar.Add(new WorktimeInfo()
                {
                    DayofWeek = GetDayofWeekName(0),
                    StartTime = calendar.SunStartTime,
                    EndTime = calendar.SunEndTime
                });
            }
            if (calendar.MonWorkDay ?? false)
            {
                workCalendar.Add(new WorktimeInfo()
                {
                    DayofWeek = GetDayofWeekName(1),
                    StartTime = calendar.MonStartTime,
                    EndTime = calendar.MonEndTime
                });
            }
            if (calendar.TueWorkDay ?? false)
            {
                workCalendar.Add(new WorktimeInfo()
                {
                    DayofWeek = GetDayofWeekName(2),
                    StartTime = calendar.TueStartTime,
                    EndTime = calendar.TueEndTime
                });
            }
            if (calendar.WedWorkDay ?? false)
            {
                workCalendar.Add(new WorktimeInfo()
                {
                    DayofWeek = GetDayofWeekName(3),
                    StartTime = calendar.WedStartTime,
                    EndTime = calendar.WedEndTime
                });
            }
            if (calendar.ThuWorkDay ?? false)
            {
                workCalendar.Add(new WorktimeInfo()
                {
                    DayofWeek = GetDayofWeekName(4),
                    StartTime = calendar.ThuStartTime,
                    EndTime = calendar.ThuEndTime
                });
            }
            if (calendar.FriWorkDay ?? false)
            {
                workCalendar.Add(new WorktimeInfo()
                {
                    DayofWeek = GetDayofWeekName(5),
                    StartTime = calendar.FriStartTime,
                    EndTime = calendar.FriEndTime
                });
            }
            if (calendar.SatWorkDay ?? false)
            {
                workCalendar.Add(new WorktimeInfo()
                {
                    DayofWeek = GetDayofWeekName(6),
                    StartTime = calendar.SatStartTime,
                    EndTime = calendar.SatEndTime
                });
            }
            #endregion
            return workCalendar;
        }

        /// <summary> Get Break Time </summary>
        public List<BreaktimeInfo> GetBreakTimeInfo(EPEmployee employee)
        {
            var result = new List<BreaktimeInfo>();
            if (employee == null)
                return null;
            for (int i = 0; i < 7; i++)
            {
                result.Add(new BreaktimeInfo()
                {
                    DayofWeek = GetDayofWeekName(i),
                    StartTime = new DateTime(1900, 1, 1, 12, 0, 0),
                    EndTime = new DateTime(1900, 1, 1, 13, 0, 0),
                    BreakTime = 60
                });
            }

            return result;
        }

        /// <summary> 判斷是否為特定假日 </summary>
        public bool CheckIsExceptionWorkDay(EPEmployee employee, DateTime? leaveStart)
        {
            var CalendarExceptions = SelectFrom<CSCalendarExceptions>
                                     .Where<CSCalendarExceptions.calendarID.IsEqual<P.AsString>>
                                     .View.Select(new PXGraph(), employee.CalendarID).RowCast<CSCalendarExceptions>().ToList();
            return CalendarExceptions.Any(x => x.Date?.Date == leaveStart?.Date) ? true : false;
        }

        public string GetDayofWeekName(int day)
        {
            switch (day)
            {
                case 0:
                    return "Sunday";
                case 1:
                    return "Monday";
                case 2:
                    return "Tuesday";
                case 3:
                    return "Wednesday";
                case 4:
                    return "Thursday";
                case 5:
                    return "Friday";
                case 6:
                    return "Saturday";
            }
            return string.Empty;
        }

        /// <summary> 取得請假類型設定 </summary>
        public LumLeaveType GetLeaveTypeInfo(string leaveType)
        {
            return SelectFrom<LumLeaveType>.Where<LumLeaveType.leaveType.IsEqual<P.AsString>>.View.Select(new PXGraph(), leaveType).TopFirst;
        }
    }

    public class BreaktimeInfo
    {
        public string DayofWeek { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? BreakTime { get; set; }
    }

    public class WorktimeInfo
    {
        public string DayofWeek { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

}
