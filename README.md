[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_System.Device.I2c.Slave&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_System.Device.I2c.Slave) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_System.Device.I2c.Slave&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_System.Device.I2c.Slave) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.System.Device.I2c.Slave.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Device.I2c.Slave/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/master/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

# Welcome to the .NET **nanoFramework** System.Device.I2c.Slave Library repository

This repository contains the .NET **nanoFramework** System.Device.I2c.Slave class library.

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| System.Device.I2c.Slave | [![Build Status](https://dev.azure.com/nanoframework/System.Device.I2c.Slave/_apis/build/status/System.Device.I2c.Slave?repoName=nanoframework%2FSystem.Device.I2c.Slave&branchName=main)](https://dev.azure.com/nanoframework/System.Device.I2c.Slave/_build/latest?definitionId=64&repoName=nanoframework%2FSystem.Device.I2c.Slave&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.System.Device.I2c.Slave.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Device.I2c.Slave/) |

## System.Device.I2c.Slave usage

### Creating an I2C slave device

To instantiate a new I2C slave device call the constructor passing the device address in the parameter and the I2C hardware bus where this device will be exposed from. Like this:

```csharp
// create an I2C slave device on bus 1 with address 0x10
var device = new I2cSlaveDevice(1, 0x10);
```

### Typical read/write operation

A common operation on an I2C device is to read the content of a specified address.
The following code snippet reads a byte with the "register address" and returns the content as an array of two bytes.
For simplicity no timeouts will be specified. Be aware that there are overloaded methods on the API that accept a timeout parameter in milliseconds.

```csharp
byte registerAddress;
if(device.ReadByte(out registerAddress))
{
    switch(registerAddress)
    {
        // (...)

        // return dummy content for register 0x22
        case 0x22:
            device.Write(new byte[] { 0xBE, 0xEF});
            break;

        // (...)
    }
}
```

From the I2C master end, assuming that's a .NET nanoFramework device, using the [System.Device.I2c](https://github.com/nanoframework/System.Device.I2c) library, the code to perform the above operation on a slave device on I2C bus 1 would be like this:

```csharp
// create I2C device
var myI2cDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x10, I2cBusSpeed.FastMode));

// setup read buffer
var buffer = new byte[2];

// set address to read from
myI2cDevice.Write(new byte[] { 0x22 });
myI2cDevice.Read(buffer);

// expected buffer content is: 0xBE, 0xEF
Console.Writeline($"Register content: {buffer[0]:X2} {buffer[1]:X2}")

```

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
