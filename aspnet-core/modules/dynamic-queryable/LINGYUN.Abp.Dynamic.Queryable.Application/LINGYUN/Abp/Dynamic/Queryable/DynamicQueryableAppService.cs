﻿using LINGYUN.Linq.Dynamic.Queryable;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LINGYUN.Abp.Dynamic.Queryable;

public abstract class DynamicQueryableAppService<TEntity, TEntityDto> : ApplicationService, IDynamicQueryableAppService<TEntityDto>
{
    /// <summary>
    /// 获取可用字段列表
    /// </summary>
    /// <returns></returns>
    public virtual Task<ListResultDto<DynamicParamterDto>> GetAvailableFieldsAsync()
    {
        var options = LazyServiceProvider.LazyGetRequiredService<IOptions<AbpDynamicQueryableOptions>>().Value;

        var entityType = typeof(TEntity);
        var dynamicParamters = new List<DynamicParamterDto>();

        var propertyInfos = entityType
            .GetProperties()
            .Where(p => !options.IgnoreFields.Contains(p.Name));

        foreach (var propertyInfo in propertyInfos)
        {
            // 字段本地化描述规则
            // 在本地化文件中定义 DisplayName:PropertyName
            var localizedProp = L[$"DisplayName:{propertyInfo.Name}"];
            var propertyTypeMap = GetPropertyTypeMap(propertyInfo.PropertyType);
            dynamicParamters.Add(
                new DynamicParamterDto
                {
                    Name = propertyInfo.Name,
                    Type = propertyInfo.PropertyType.FullName,
                    Description = localizedProp.Value ?? propertyInfo.Name,
                    JavaScriptType = propertyTypeMap.JavaScriptType,
                    AvailableComparator = propertyTypeMap.AvailableComparator
                });
        }

        return Task.FromResult(new ListResultDto<DynamicParamterDto>(dynamicParamters));
    }

    /// <summary>
    /// 根据动态条件查询数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async virtual Task<PagedResultDto<TEntityDto>> GetListAsync(GetListByDynamicQueryableInput input)
    {
        Expression<Func<TEntity, bool>> condition = (e) => true;

        condition = condition.DynamicQuery(input.Queryable);

        var totalCount = await GetCountAsync(condition);
        var entities = await GetListAsync(condition, input);

        return new PagedResultDto<TEntityDto>(totalCount,
            MapToEntitiesDto(entities));
    }

    protected abstract Task<int> GetCountAsync(Expression<Func<TEntity, bool>> condition);

    protected abstract Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> condition,
        PagedAndSortedResultRequestDto pageRequest);

    protected virtual List<TEntityDto> MapToEntitiesDto(List<TEntity> entities)
    {
        return ObjectMapper.Map<List<TEntity>, List<TEntityDto>>(entities);
    }

    protected virtual (string JavaScriptType, DynamicComparison[] AvailableComparator) GetPropertyTypeMap(Type propertyType)
    {
        var isNullableType = false;
        var availableComparator = new List<DynamicComparison>();
        if (propertyType.IsNullableType())
        {
            isNullableType = true;
            propertyType = propertyType.GetGenericArguments().FirstOrDefault();
        }
        var typeFullName = propertyType.FullName.ToLower();

        switch (typeFullName)
        {
            case "System.Int16":
            case "System.Int32":
            case "System.Int64":
            case "System.UInt16":
            case "System.UInt32":
            case "System.UInt64":
            case "System.Single":
            case "System.Double":
            case "System.Byte":
            case "System.SByte":
            case "System.Decimal":
                // 数值类型只支持如下操作符
                // 小于、小于等于、大于、大于等于、等于、不等于、空、非空
                availableComparator.AddRange(new[]
                {
                    DynamicComparison.GreaterThan,
                    DynamicComparison.GreaterThanOrEqual,
                    DynamicComparison.LessThan,
                    DynamicComparison.LessThanOrEqual,
                    DynamicComparison.Equal,
                    DynamicComparison.NotEqual,
                });
                if (isNullableType)
                {
                    availableComparator.AddRange(new[]
                    {
                        DynamicComparison.Null,
                        DynamicComparison.NotNull
                    });
                }
                return ("number", availableComparator.ToArray());
            case "System.Boolean":
            case "System.Guid":
                // 布尔、Guid类型只支持如下操作符
                // 等于、不等于、空、非空
                availableComparator.AddRange(new[]
                {
                    DynamicComparison.Equal,
                    DynamicComparison.NotEqual,
                });
                if (isNullableType)
                {
                    availableComparator.AddRange(new[]
                    {
                        DynamicComparison.Null,
                        DynamicComparison.NotNull
                    });
                }
                return ("boolean", availableComparator.ToArray());
            case "System.Char":
            case "System.String":
                // 字符类型支持所有操作符
                return ("string", availableComparator.ToArray());
            case "System.DateTime":
                // 时间类型只支持如下操作符
                // 小于、小于等于、大于、大于等于、等于、不等于、空、非空
                availableComparator.AddRange(new[]
                {
                    DynamicComparison.GreaterThan,
                    DynamicComparison.GreaterThanOrEqual,
                    DynamicComparison.LessThan,
                    DynamicComparison.LessThanOrEqual,
                    DynamicComparison.Equal,
                    DynamicComparison.NotEqual,
                });
                if (isNullableType)
                {
                    availableComparator.AddRange(new[]
                    {
                        DynamicComparison.Null,
                        DynamicComparison.NotNull
                    });
                }
                return ("Date", availableComparator.ToArray());
            default:
            case "System.Object":
            case "System.DBNull":
                if (isNullableType)
                {
                    availableComparator.AddRange(new[]
                    {
                        DynamicComparison.Null,
                        DynamicComparison.NotNull
                    });
                }
                if (propertyType.IsArray)
                {
                    // 数组类型只支持如下操作符
                    // 包含、不包含、空、非空
                    availableComparator.AddRange(new[]
                    {
                        DynamicComparison.Contains,
                        DynamicComparison.NotContains,
                    });

                    return ("array", availableComparator.ToArray());
                }
                else
                {
                    // 未知对象类型只支持如下操作符
                    // 等于、不等于、空、非空
                    availableComparator.AddRange(new[]
                   {
                        DynamicComparison.Equal,
                        DynamicComparison.NotEqual,
                    });
                    return ("object", availableComparator.ToArray());
                }
        }
    }
}
