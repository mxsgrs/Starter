﻿namespace Starter.WebApi.Services.Interfaces;

public interface IAppContextAccessor
{
    UserClaims UserClaims { get; }
}