﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LINGYUN.Abp.LocalizationManagement
{
    public interface ITextRepository : IRepository<Text, int>
    {
        Task<Text> GetByCultureKeyAsync(
            string resourceName,
            string cultureName,
            string key,
            CancellationToken cancellationToken = default
            );

        Task<List<Text>> GetListAsync(
            string resourceName = null,
            string cultureName = null,
            CancellationToken cancellationToken = default);
    }
}
