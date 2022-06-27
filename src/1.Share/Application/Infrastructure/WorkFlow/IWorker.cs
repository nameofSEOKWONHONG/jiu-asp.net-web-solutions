using System;

namespace Application.Infrastructure.WorkFlow;

public interface IChecker
{
    bool Check(Object o);
}