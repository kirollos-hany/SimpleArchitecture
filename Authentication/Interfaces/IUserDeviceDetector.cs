using SimpleArchitecture.Common.ValueObjects;

namespace SimpleArchitecture.Authentication.Interfaces;

public interface IUserDeviceDetector
{
    DeviceInfo? DetectDevice();

    string UserAgent();
}