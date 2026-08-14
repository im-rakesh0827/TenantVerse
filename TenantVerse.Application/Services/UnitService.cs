using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Unit.Requests;

namespace TenantVerse.Application.Services;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;

    public UnitService(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }


    public async Task<ApiResponse<List<UnitModel>>> GetAllAsync()
    {
        try
        {
            var units = await _unitRepository.GetAllAsync();

            return new ApiResponse<List<UnitModel>>
            {
                IsSuccess = true,
                Data = units,
                Message = "Units retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<UnitModel>>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }


    public async Task<ApiResponse<UnitModel>> GetByIdAsync(
        int unitId)
    {
        try
        {
            if (unitId <= 0)
            {
                return new ApiResponse<UnitModel>
                {
                    IsSuccess = false,
                    Message = "Invalid unit ID."
                };
            }

            var unit = await _unitRepository.GetByIdAsync(unitId);

            if (unit == null)
            {
                return new ApiResponse<UnitModel>
                {
                    IsSuccess = false,
                    Message = "Unit not found."
                };
            }

            return new ApiResponse<UnitModel>
            {
                IsSuccess = true,
                Data = unit,
                Message = "Unit retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<UnitModel>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }


    public async Task<ApiResponse<List<UnitModel>>> GetByPropertyIdAsync(
        int propertyId)
    {
        try
        {
            if (propertyId <= 0)
            {
                return new ApiResponse<List<UnitModel>>
                {
                    IsSuccess = false,
                    Message = "Invalid property ID."
                };
            }

            var units =
                await _unitRepository.GetByPropertyIdAsync(propertyId);

            return new ApiResponse<List<UnitModel>>
            {
                IsSuccess = true,
                Data = units,
                Message = "Units retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<UnitModel>>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }


    public async Task<ApiResponse<int>> CreateAsync(
        CreateUnitRequest request)
    {
        try
        {
            if (request == null)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Unit request is required."
                };
            }

            if (request.PropertyId <= 0)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Property is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.UnitNumber))
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Unit number is required."
                };
            }

            var unitId =
                await _unitRepository.CreateAsync(request);

            return new ApiResponse<int>
            {
                IsSuccess = unitId > 0,
                Data = unitId,
                Message = unitId > 0
                    ? "Unit created successfully."
                    : "Unable to create unit."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }


    public async Task<ApiResponse<int>> UpdateAsync(
        UpdateUnitRequest request)
    {
        try
        {
            if (request == null)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Unit request is required."
                };
            }

            if (request.UnitId <= 0)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Invalid unit ID."
                };
            }

            if (request.PropertyId <= 0)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Property is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.UnitNumber))
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Unit number is required."
                };
            }

            var result =
                await _unitRepository.UpdateAsync(request);

            return new ApiResponse<int>
            {
                IsSuccess = result > 0,
                Data = result,
                Message = result > 0
                    ? "Unit updated successfully."
                    : "Unable to update unit."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
    public async Task<ApiResponse<int>> DeleteAsync(int unitId, string updatedBy)
    {
        try
        {
            if (unitId <= 0)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Invalid unit ID.",
                    Data = 0
                };
            }

            var rowsAffected = await _unitRepository.DeleteAsync(
                unitId,
                updatedBy);

            if (rowsAffected <= 0)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Flat not found or already inactive.",
                    Data = 0
                };
            }

            return new ApiResponse<int>
            {
                IsSuccess = true,
                Message = "Flat deactivated successfully.",
                Data = rowsAffected
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = 0
            };
        }
    }
}