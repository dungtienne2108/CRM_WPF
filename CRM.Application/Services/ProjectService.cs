using AutoMapper;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Project;
using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Application.Services
{
    public class ProjectService(
            IProjectRepository projectRepository,
            IProductRepository productRepository,
            IRepository<ProductType> productTypeRepository,
            IRepository<ProductStatus> productStatusRepository,
            ISessionService sessionService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMemoryCache memoryCache) : IProjectService
    {
        public async Task<Result<int>> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                var project = await projectRepository.GetByIdAsync(request.ProjectId);
                if (project == null)
                {
                    return Result.Failure<int>(new Error("PROJECT_NOT_FOUND", $"Không tìm thấy dự án với Id: {request.ProjectId}"));
                }

                var product = new Product
                {
                    ProductName = request.ProductName,
                    ProductFloors = request.Floors,
                    ProductArea = request.Area,
                    ProductPrice = request.Price,
                    ProductStatusId = 1, // mặc định là chưa bán
                    ProductTypeId = request.TypeId,
                    ProjectId = request.ProjectId,
                    ProductAddress = project.ProjectAddress
                };

                await productRepository.AddAsync(product);
                var added = await unitOfWork.SaveChangesAsync();

                if (added > 0)
                {
                    memoryCache.Remove("AllProducts");
                    memoryCache.Remove($"Products_Project_{request.ProjectId}");
                    return Result.Success(product.ProductId);
                }
                else
                {
                    return Result.Failure<int>(new Error("CREATE_PRODUCT_FAILED", $"Lỗi xảy ra khi tạo sản phẩm mới."));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(new Error("CREATE_PRODUCT_EXCEPTION", $"Lỗi xảy ra khi tạo sản phẩm mới: {ex.Message}"));
            }
        }

        public async Task<Result<int>> CreateProjectAsync(CreateProjectRequest request)
        {
            try
            {
                var project = new Project
                {
                    ProjectName = request.ProjectName,
                    ProjectStartDate = DateOnly.FromDateTime(request.StartDate),
                    ProjectEndDate = DateOnly.FromDateTime(request.EndDate),
                    ProjectAddress = request.ProjectAddress,
                    TotalArea = request.ProjectArea
                };

                await projectRepository.AddAsync(project);
                var added = await unitOfWork.SaveChangesAsync();

                if (added > 0)
                {
                    memoryCache.Remove("AllProjects");
                    return Result.Success(project.ProjectId);
                }
                else
                {
                    return Result.Failure<int>(new Error("CREATE_PROJECT_FAILED", $"Lỗi xảy ra khi tạo dự án mới."));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(new Error("CREATE_PROJECT_EXCEPTION", $"Lỗi xảy ra khi tạo dự án mới: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await productRepository.GetByIdAsync(productId);

                if (product == null)
                {
                    return Result.Failure<ProductDto>(new Error("PRODUCT_NOT_FOUND", $"Không tìm thấy dự án với Id: {productId}"));
                }

                productRepository.Remove(product);
                var deleted = await unitOfWork.SaveChangesAsync();

                if (deleted > 0)
                {
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(new("DELETE_PRODUCT_FAILED", "Xóa sản phẩm thất bại"));
                }
            }
            catch { throw; }
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            //if (memoryCache.TryGetValue("AllProducts", out IEnumerable<ProductDto>? cachedProducts))
            //{
            //    return cachedProducts;
            //}

            var products = await productRepository.GetAllAsync();

            var productDtos = mapper.Map<IEnumerable<ProductDto>>(products);

            //memoryCache.Set("AllProducts", productDtos, TimeSpan.FromMinutes(1));

            return productDtos;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            if (memoryCache.TryGetValue("AllProjects", out IEnumerable<ProjectDto>? cachedProjects))
            {
                return cachedProjects;
            }

            var projects = await projectRepository.GetAllAsync();

            var projectDtos = mapper.Map<IEnumerable<ProjectDto>>(projects);

            memoryCache.Set("AllProjects", projectDtos, TimeSpan.FromMinutes(1));

            return projectDtos;
        }

        public async Task<Result<ProductDto?>> GetProductByIdAsync(int productId)
        {
            if (memoryCache.TryGetValue($"Product_{productId}", out ProductDto? cachedProduct))
            {
                return Result.Success(cachedProduct);
            }

            var product = await productRepository.GetProductByIdAsync(productId);

            if (product == null)
            {
                return Result.Failure<ProductDto?>(new Error("PRODUCT_NOT_FOUND", $"Không tìm thấy sản phẩm với Id: {productId}"));
            }

            var productDto = mapper.Map<ProductDto>(product);

            memoryCache.Set($"Product_{productId}", productDto, TimeSpan.FromMinutes(10));

            return Result.Success<ProductDto?>(productDto);
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetProductByOpportunityIdAsync(int opportunityId)
        {
            if (memoryCache.TryGetValue($"Products_Opportunity_{opportunityId}", out IEnumerable<ProductDto>? cachedProducts))
            {
                return Result.Success(cachedProducts);
            }

            var products = await projectRepository.GetProductsByOpportunityIdAsync(opportunityId);

            var productDtos = mapper.Map<IEnumerable<ProductDto>>(products);

            memoryCache.Set($"Products_Opportunity_{opportunityId}", productDtos, TimeSpan.FromMinutes(10));

            return Result.Success(productDtos);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByProjectIdAsync(int projectId)
        {
            if (memoryCache.TryGetValue($"Products_Project_{projectId}", out IEnumerable<ProductDto>? cachedProducts))
            {
                return cachedProducts;
            }

            // trừ cái đã bán
            var products = await productRepository.GetProductsByProjectIdAsync(projectId);

            var productDtos = mapper.Map<IEnumerable<ProductDto>>(products);

            memoryCache.Set($"Products_Project_{projectId}", productDtos, TimeSpan.FromMinutes(10));

            return productDtos;
        }

        public async Task<Result<IEnumerable<ProductStatusOption>>> GetProductStatusesAsync()
        {
            if (memoryCache.TryGetValue("ProductStatuses", out IEnumerable<ProductStatusOption>? cachedStatuses))
            {
                return Result.Success(cachedStatuses);
            }

            var productStatuses = await productStatusRepository.GetAllAsync();
            var productStatusOptions = productStatuses
                .Select(ps => new ProductStatusOption
                {
                    Id = ps.ProductStatusId,
                    Name = ps.ProductStatusName
                })
                .ToList();

            memoryCache.Set("ProductStatuses", productStatusOptions, TimeSpan.FromMinutes(10));
            return Result.Success<IEnumerable<ProductStatusOption>>(productStatusOptions);
        }

        public async Task<Result<IEnumerable<ProductTypeOption>>> GetProductTypesAsync()
        {
            if (memoryCache.TryGetValue("ProductTypes", out IEnumerable<ProductTypeOption>? cachedTypes))
            {
                return Result.Success(cachedTypes);
            }

            var productTypes = await productTypeRepository.GetAllAsync();
            var productTypeOptions = productTypes
                .Select(pt => new ProductTypeOption
                {
                    Id = pt.ProductTypeId,
                    Name = pt.ProductTypeName
                })
                .ToList();

            memoryCache.Set("ProductTypes", productTypeOptions, TimeSpan.FromHours(1));
            return Result.Success<IEnumerable<ProductTypeOption>>(productTypeOptions);
        }

        public async Task<Result<ProjectDto?>> GetProjectByIdAsync(int projectId)
        {
            if (memoryCache.TryGetValue($"Project_{projectId}", out ProjectDto? cachedProject))
            {
                return Result.Success(cachedProject);
            }

            var project = await projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return Result.Failure<ProjectDto?>(new Error("PROJECT_NOT_FOUND", $"Không tìm thấy dự án với Id: {projectId}"));
            }

            var projectDto = mapper.Map<ProjectDto>(project);

            memoryCache.Set($"Project_{projectId}", projectDto, TimeSpan.FromMinutes(10));

            return Result.Success<ProjectDto?>(projectDto);
        }

        public async Task<PagedResult<ProjectDto>> GetProjectsAsync(GetProjectRequest request)
        {
            try
            {
                //if (memoryCache.TryGetValue($"projects_page_{request.PageNumber}_size_{request.PageSize}_keyword_{request.Keyword}", out PagedResult<ProjectDto> cachedProjects))
                //{
                //    return cachedProjects;
                //}

                var filter = new ProjectFilter
                {
                    Keyword = request.Keyword,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                var pagedProjects = await projectRepository.GetProjectsAsync(filter);

                var projectDtos = mapper.Map<IEnumerable<ProjectDto>>(pagedProjects.Items);

                var pagedResult = new PagedResult<ProjectDto>(projectDtos, pagedProjects.TotalCount, pagedProjects.PageNumber, pagedProjects.PageSize);

                //memoryCache.Set($"projects_page_{request.PageNumber}_size_{request.PageSize}_keyword_{request.Keyword}", pagedResult, TimeSpan.FromMinutes(5));

                return pagedResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result<ProductDto>> UpdateProductAsync(UpdateProductRequest request)
        {
            try
            {
                var product = await productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return Result.Failure<ProductDto>(new Error("PRODUCT_NOT_FOUND", $"Không tìm thấy dự án với Id: {request.ProductId}"));
                }

                product.ProductPrice = request.ProductPrice;
                product.ProductName = request.ProductName;
                product.ProductFloors = request.ProductFloors;
                product.ProductArea = request.ProductArea;
                product.ProductTypeId = request.ProductTypeId;
                product.ProductStatusId = request.ProductStatusId;

                productRepository.Update(product);
                var updated = await unitOfWork.SaveChangesAsync();

                if (updated > 0)
                {
                    var productDtp = mapper.Map<ProductDto>(product);
                    memoryCache.Remove($"Product_{productDtp.ProductId}");
                    return Result.Success(productDtp);
                }
                else
                {
                    return Result.Failure<ProductDto>(new("UPDATE_PRODUCT_FAILED", "Lỗi xảy ra khi cập nhật sản phẩm"));
                }
            }
            catch { throw; }
        }

        public async Task<Result> UpdateProductStatusAsync(int opportunityId, int productId, int productStatusId)
        {
            try
            {
                var product = await productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return Result.Failure(new Error("PRODUCT_NOT_FOUND", $"Không tìm thấy sản phẩm với Id: {productId}"));
                }

                // kiểm tra nếu product đã có updatedBy thì updatedBy phải trung user hiện tại mới được cập nhật trạng thái
                if (product.ProductStatusId != 1 && !string.IsNullOrEmpty(product.UpdatedBy) && product.UpdatedBy != sessionService.CurrentAccount.EmployeeId.ToString())
                {
                    return Result.Failure(new Error("PRODUCT_UPDATE_FORBIDDEN", "Bạn không có quyền cập nhật trạng thái sản phẩm này"));
                }

                product.ProductStatusId = productStatusId;
                product.UpdatedBy = sessionService.CurrentAccount.EmployeeId.ToString();
                productRepository.Update(product);
                var updated = await unitOfWork.SaveChangesAsync();
                if (updated > 0)
                {
                    memoryCache.Remove($"Product_{product.ProductId}");
                    memoryCache.Remove($"Opportunity_{opportunityId}");
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(new Error("UPDATE_PRODUCT_STATUS_FAILED", "Lỗi xảy ra khi cập nhật trạng thái sản phẩm"));
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
