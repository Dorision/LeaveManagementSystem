using LeaveManagementSystem.DTOs;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories.Interfaces;

namespace LeaveManagementSystem.Services;

public interface ILeaveRequestService
{
    Task<LeaveRequestDto> CreateAsync(string employeeId, CreateLeaveRequestDto dto);
    Task<LeaveRequestDto> UpdateAsync(string employeeId, Guid requestId, UpdateLeaveRequestDto dto);
    Task<LeaveRequestDto> MakeDecisionAsync(string managerId, Guid requestId, LeaveRequestDecisionDto dto);
    Task<LeaveRequestDto> RetractAsync(string employeeId, Guid requestId);
    Task<List<LeaveRequestDto>> GetEmployeeRequestsAsync(string employeeId);
    Task<List<LeaveRequestDto>> GetSubordinateRequestsAsync(string managerId);
    Task<int> CalculateWorkdaysAsync(DateTime startDate, DateTime endDate);
}

public class LeaveRequestService : ILeaveRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublicHolidayService _holidayService;

    public LeaveRequestService(
        IUnitOfWork unitOfWork,
        IPublicHolidayService holidayService)
    {
        _unitOfWork = unitOfWork;
        _holidayService = holidayService;
    }

    public async Task<LeaveRequestDto> CreateAsync(string employeeId, CreateLeaveRequestDto dto)
    {
        var employee = await _unitOfWork.Users.GetByIdAsync(employeeId)
            ?? throw new InvalidOperationException("Employee not found");

        if (dto.StartDate > dto.EndDate)
        {
            throw new InvalidOperationException("Start date must be before or equal to end date");
        }

        var workdays = await CalculateWorkdaysAsync(dto.StartDate, dto.EndDate);
        if (workdays == 0)
        {
            throw new InvalidOperationException("Selected date range contains no working days");
        }

        var request = new LeaveRequest
        {
            EmployeeId = employeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            DaysRequested = workdays,
            Status = LeaveRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Comment = dto.Comment
        };

        await _unitOfWork.LeaveRequests.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        return await GetLeaveRequestDtoAsync(request.Id);
    }

    public async Task<LeaveRequestDto> UpdateAsync(string employeeId, Guid requestId, UpdateLeaveRequestDto dto)
    {
        var request = (await _unitOfWork.LeaveRequests.FindAsync(r => r.Id == requestId && r.EmployeeId == employeeId)).FirstOrDefault()
            ?? throw new InvalidOperationException("Leave request not found");

        if (request.Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be updated");
        }

        if (dto.StartDate > dto.EndDate)
        {
            throw new InvalidOperationException("Start date must be before or equal to end date");
        }

        var workdays = await CalculateWorkdaysAsync(dto.StartDate, dto.EndDate);
        if (workdays == 0)
        {
            throw new InvalidOperationException("Selected date range contains no working days");
        }

        request.StartDate = dto.StartDate;
        request.EndDate = dto.EndDate;
        request.DaysRequested = workdays;
        request.Comment = dto.Comment;

        _unitOfWork.LeaveRequests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return await GetLeaveRequestDtoAsync(request.Id);
    }

    public async Task<LeaveRequestDto> MakeDecisionAsync(string managerId, Guid requestId, LeaveRequestDecisionDto dto)
    {
        var requests = await _unitOfWork.LeaveRequests.FindAsync(r => r.Id == requestId);
        var request = requests.FirstOrDefault() ?? throw new InvalidOperationException("Leave request not found");

        var employee = await _unitOfWork.Users.GetByIdAsync(request.EmployeeId);
        if (employee?.ManagerId != managerId)
        {
            throw new InvalidOperationException("You are not authorized to make decisions for this employee");
        }

        if (request.Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be approved or rejected");
        }

        request.Status = dto.IsApproved ? LeaveRequestStatus.Approved : LeaveRequestStatus.Rejected;
        request.DecisionById = managerId;
        request.DecisionAt = DateTime.UtcNow;
        request.Comment = dto.Comment;

        _unitOfWork.LeaveRequests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return await GetLeaveRequestDtoAsync(request.Id);
    }

    public async Task<LeaveRequestDto> RetractAsync(string employeeId, Guid requestId)
    {
        var request = (await _unitOfWork.LeaveRequests.FindAsync(r => r.Id == requestId && r.EmployeeId == employeeId)).FirstOrDefault()
            ?? throw new InvalidOperationException("Leave request not found");

        if (request.Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be retracted");
        }

        request.Status = LeaveRequestStatus.Retracted;
        _unitOfWork.LeaveRequests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return await GetLeaveRequestDtoAsync(request.Id);
    }

    public async Task<List<LeaveRequestDto>> GetEmployeeRequestsAsync(string employeeId)
    {
        var requests = await _unitOfWork.LeaveRequests.GetEmployeeRequestsAsync(employeeId);
        var dtos = await Task.WhenAll(
            requests.Select(async r => await MapToLeaveRequestDtoAsync(r)));
        return dtos.ToList();
    }

    public async Task<List<LeaveRequestDto>> GetSubordinateRequestsAsync(string managerId)
    {
        var requests = await _unitOfWork.LeaveRequests.GetSubordinateRequestsAsync(managerId);
        var dtos = await Task.WhenAll(
            requests.Select(async r => await MapToLeaveRequestDtoAsync(r)));
        return dtos.ToList();
    }

    public async Task<int> CalculateWorkdaysAsync(DateTime startDate, DateTime endDate)
    {
        var currentDate = startDate.Date;
        var workdays = 0;

        while (currentDate <= endDate.Date)
        {
            if (await _holidayService.IsWorkingDayAsync(currentDate))
            {
                workdays++;
            }
            currentDate = currentDate.AddDays(1);
        }

        return workdays;
    }

    private async Task<LeaveRequestDto> GetLeaveRequestDtoAsync(Guid requestId)
    {
        var request = (await _unitOfWork.LeaveRequests.FindAsync(r => r.Id == requestId)).FirstOrDefault()
            ?? throw new InvalidOperationException("Leave request not found");

        var employee = await _unitOfWork.Users.GetByIdAsync(request.EmployeeId);
        var decisionBy = request.DecisionById != null ? await _unitOfWork.Users.GetByIdAsync(request.DecisionById) : null;

        return new LeaveRequestDto
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            EmployeeName = employee?.UserName ?? "Unknown",
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DaysRequested = request.DaysRequested,
            Status = request.Status,
            CreatedAt = request.CreatedAt,
            DecisionAt = request.DecisionAt,
            DecisionBy = decisionBy?.UserName,
            Comment = request.Comment
        };
    }

    private async Task<LeaveRequestDto> MapToLeaveRequestDtoAsync(LeaveRequest request)
    {
        return new LeaveRequestDto
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            EmployeeName = request.Employee?.UserName ?? "Unknown",
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DaysRequested = request.DaysRequested,
            Status = request.Status,
            CreatedAt = request.CreatedAt,
            DecisionAt = request.DecisionAt,
            DecisionBy = request.DecisionBy?.UserName,
            Comment = request.Comment
        };
    }
}