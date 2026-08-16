using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Tenant;


namespace TenantVerse.Application.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;

    public TenantService(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<ApiResponse<List<TenantModel>>> GetAllAsync()
    {
        try
        {
            var tenants = await _tenantRepository.GetAllAsync();

            return new ApiResponse<List<TenantModel>>
            {
                IsSuccess = true,
                Data = tenants
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<TenantModel>>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<TenantModel>> GetByIdAsync(int tenantId)
    {
        try
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);

            if (tenant is null)
            {
                return new ApiResponse<TenantModel>
                {
                    IsSuccess = false,
                    Message = "Tenant not found."
                };
            }

            return new ApiResponse<TenantModel>
            {
                IsSuccess = true,
                Data = tenant
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TenantModel>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<List<TenantModel>>> GetByPropertyIdAsync(
        int propertyId)
    {
        try
        {
            var tenants =
                await _tenantRepository.GetByPropertyIdAsync(propertyId);

            return new ApiResponse<List<TenantModel>>
            {
                IsSuccess = true,
                Data = tenants
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<TenantModel>>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<List<TenantModel>>> GetByUnitIdAsync(
        int unitId)
    {
        try
        {
            var tenants =
                await _tenantRepository.GetByUnitIdAsync(unitId);

            return new ApiResponse<List<TenantModel>>
            {
                IsSuccess = true,
                Data = tenants
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<TenantModel>>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<int>> CreateAsync(
        CreateTenantRequest request)
    {
        try
        {
            var tenantId =
                await _tenantRepository.CreateAsync(request);

            return new ApiResponse<int>
            {
                IsSuccess = true,
                Data = tenantId,
                Message = "Tenant created successfully."
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
        UpdateTenantRequest request)
    {
        try
        {
            var tenantId =
                await _tenantRepository.UpdateAsync(request);

            return new ApiResponse<int>
            {
                IsSuccess = true,
                Data = tenantId,
                Message = "Tenant updated successfully."
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

    public async Task<ApiResponse<int>> DeleteAsync(
        int tenantId,
        string updatedBy)
    {
        try
        {
            var deletedTenantId =
                await _tenantRepository.DeleteAsync(
                    tenantId,
                    updatedBy);

            return new ApiResponse<int>
            {
                IsSuccess = true,
                Data = deletedTenantId,
                Message = "Tenant deleted successfully."
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
}