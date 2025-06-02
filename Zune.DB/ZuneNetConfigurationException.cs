using System;

namespace Zune.DB;

public class ZuneNetConfigurationException : Exception
{
    public ZuneNetConfigurationException()
    {
    }

    public ZuneNetConfigurationException(string message) : base(message)
    {
    }
}