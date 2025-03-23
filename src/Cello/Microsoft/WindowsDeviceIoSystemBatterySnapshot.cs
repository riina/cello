using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Memory;
using Windows.Win32.System.Power;

namespace Cello.Microsoft;

/// <summary>
/// Represents a snapshot of the state of system batteries a Windows system.
/// </summary>
public record WindowsDeviceIoSystemBatterySnapshot : SystemBatterySnapshot
{
    private List<DeviceIoBatteryState> DeviceIoBatteryStates { get; init; }

    /// <summary>
    /// Initializes an instance of <see cref="WindowsDeviceIoSystemBatterySnapshot"/>.
    /// </summary>
    /// <param name="deviceIoBatteryStates">Battery states.</param>
    public WindowsDeviceIoSystemBatterySnapshot(List<DeviceIoBatteryState> deviceIoBatteryStates)
    {
        DeviceIoBatteryStates = deviceIoBatteryStates;
    }

    /// <inheritdoc />
    public override BatteryInfo GetPrimaryBatteryInfo()
    {
        if (DeviceIoBatteryStates.Count == 0)
        {
            return new BatteryInfo { HasBattery = false };
        }
        return CreateBatteryInfo(DeviceIoBatteryStates[0]);
    }

    /// <inheritdoc />
    public override List<BatteryInfo> GetBatteryInfos()
    {
        return [..DeviceIoBatteryStates.Select(CreateBatteryInfo)];
    }

    private BatteryInfo CreateBatteryInfo(DeviceIoBatteryState deviceIoBatteryState)
    {
        double? chargePercentage = null;
        {
            if (deviceIoBatteryState.Capacity is { } capacity)
            {
                chargePercentage = (double)capacity / deviceIoBatteryState.FullChargedCapacity * 100.0;
            }
        }
        {
            return new BatteryInfo
            {
                HasBattery = true, //
                ChargePercentage = chargePercentage, //
                ChargeHealthPercentage = Math.Clamp((double)deviceIoBatteryState.FullChargedCapacity / deviceIoBatteryState.DesignedCapacity * 100.0, 0, 100), //
                CurrentChargeCapacity = deviceIoBatteryState is { CapacityIsRelative: false, Capacity: { } capacity } ? BatteryCapacityValue.FromMilliwattHours((int)capacity) : null, //
                MaxChargeCapacity = !deviceIoBatteryState.CapacityIsRelative ? BatteryCapacityValue.FromMilliwattHours((int)deviceIoBatteryState.FullChargedCapacity) : null, //
                DesignChargeCapacity = !deviceIoBatteryState.CapacityIsRelative ? BatteryCapacityValue.FromMilliwattHours((int)deviceIoBatteryState.DesignedCapacity) : null, //
                ChargeRate = deviceIoBatteryState is { CapacityIsRelative: false, Rate: { } rate } ? rate / 1000.0 : null, //
                Voltage = deviceIoBatteryState.Voltage, //
                Temperature = deviceIoBatteryState.Temperature * 0.10 - 273.15, //
                ChargingFlags = deviceIoBatteryState.ChargingFlags, //
                TimeToDischargeCompletion = (deviceIoBatteryState.ChargingFlags & ChargingFlags.Discharging) != 0 && deviceIoBatteryState.EstimatedTime is { } estimatedTime ? estimatedTime : null, //
            };
        }
    }

    /// <inheritdoc />
    public override string GetDetails()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery using the Windows API.
    /// </summary>
    /// <returns>Snapshot content.</returns>
    /// <exception cref="Win32Exception">Thrown if failed to get system power status.</exception>
    [SupportedOSPlatform("windows5.1.2600")]
    public static unsafe WindowsDeviceIoSystemBatterySnapshot CreateSystemSnapshotFromDeviceIo()
    {
        List<DeviceIoBatteryState> batteryStates = new();
        CONFIGRET cr;
        PWSTR deviceInterfaceList = default;
        uint deviceInterfaceListLength = 0;
        try
        {
            do
            {
                cr = PInvoke.CM_Get_Device_Interface_List_Size(
                    out deviceInterfaceListLength,
                    PInvoke.GUID_DEVICE_BATTERY,
                    null, CM_GET_DEVICE_INTERFACE_LIST_FLAGS.CM_GET_DEVICE_INTERFACE_LIST_PRESENT);
                if (cr != CONFIGRET.CR_SUCCESS)
                {
                    break;
                }

                if (deviceInterfaceList != new PWSTR(null))
                {
                    PInvoke.HeapFree(PInvoke.GetProcessHeap(),
                        0,
                        deviceInterfaceList);
                }

                deviceInterfaceList = (PWSTR)PInvoke.HeapAlloc(PInvoke.GetProcessHeap(),
                    HEAP_FLAGS.HEAP_ZERO_MEMORY,
                    deviceInterfaceListLength * sizeof(char));

                if (deviceInterfaceList == new PWSTR(null))
                {
                    cr = CONFIGRET.CR_OUT_OF_MEMORY;
                    break;
                }

                cr = PInvoke.CM_Get_Device_Interface_List(
                    PInvoke.GUID_DEVICE_BATTERY,
                    null,
                    new PZZWSTR(deviceInterfaceList.Value),
                    deviceInterfaceListLength,
                    CM_GET_DEVICE_INTERFACE_LIST_FLAGS.CM_GET_DEVICE_INTERFACE_LIST_PRESENT);
            } while (cr == CONFIGRET.CR_BUFFER_SMALL);

            if (cr != CONFIGRET.CR_SUCCESS)
            {
                throw new IOException();
            }
            var deviceInterfaces = new List<string>();
            int currentStart = 0;
            int end = deviceInterfaceList.Length + 1;
            char* deviceInterfaceListPtr = deviceInterfaceList.Value;
            for (int i = 0; i < end; i++)
            {
                char c = deviceInterfaceListPtr[i];
                if (c == 0)
                {
                    deviceInterfaces.Add(new string(deviceInterfaceListPtr, currentStart, i - currentStart));
                    currentStart = i + 1;
                }
            }
            foreach (string deviceInterface in deviceInterfaces)
            {
                using var hBattery = PInvoke.CreateFile(deviceInterface,
                    (uint)(GENERIC_ACCESS_RIGHTS.GENERIC_READ | GENERIC_ACCESS_RIGHTS.GENERIC_WRITE),
                    FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                    null,
                    FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                    FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                    null);
                if (hBattery is { IsInvalid: false })
                {
                    BATTERY_QUERY_INFORMATION bqi = default;
                    int dwWait = 0;
                    uint dwOut;
                    if (!PInvoke.DeviceIoControl(
                            hBattery,
                            PInvoke.IOCTL_BATTERY_QUERY_TAG,
                            &dwWait,
                            sizeof(int),
                            &bqi.BatteryTag,
                            sizeof(uint),
                            &dwOut,
                            null))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                    if (bqi.BatteryTag != 0)
                    {
                        BATTERY_INFORMATION bi = default;
                        bqi.InformationLevel = BATTERY_QUERY_INFORMATION_LEVEL.BatteryInformation;

                        if (!PInvoke.DeviceIoControl(hBattery,
                                PInvoke.IOCTL_BATTERY_QUERY_INFORMATION,
                                &bqi,
                                (uint)sizeof(BATTERY_QUERY_INFORMATION),
                                &bi,
                                (uint)sizeof(BATTERY_INFORMATION),
                                &dwOut,
                                null))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                        uint? temperature = null;
                        /*{
                            uint temperatureV;
                            bqi.InformationLevel = BATTERY_QUERY_INFORMATION_LEVEL.BatteryTemperature;
                            if (!PInvoke.DeviceIoControl(hBattery,
                                    PInvoke.IOCTL_BATTERY_QUERY_INFORMATION,
                                    &bqi,
                                    (uint)sizeof(BATTERY_QUERY_INFORMATION),
                                    &temperatureV,
                                    sizeof(uint),
                                    &dwOut,
                                    null))
                            {
                                throw new Win32Exception(Marshal.GetLastWin32Error());
                            }
                            temperature = temperatureV;
                        }*/
                        uint estimatedTime;
                        bqi.InformationLevel = BATTERY_QUERY_INFORMATION_LEVEL.BatteryEstimatedTime;
                        bqi.AtRate = 0;
                        if (!PInvoke.DeviceIoControl(hBattery,
                                PInvoke.IOCTL_BATTERY_QUERY_INFORMATION,
                                &bqi,
                                (uint)sizeof(BATTERY_QUERY_INFORMATION),
                                &estimatedTime,
                                sizeof(uint),
                                &dwOut,
                                null))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                        if ((bi.Capabilities & PInvoke.BATTERY_SYSTEM_BATTERY) != 0)
                        {
                            if ((bi.Capabilities & PInvoke.BATTERY_IS_SHORT_TERM) == 0)
                            {
                                // backup battery
                            }

                            // Query the battery status.
                            BATTERY_WAIT_STATUS bws = default;
                            bws.BatteryTag = bqi.BatteryTag;

                            BATTERY_STATUS bs;
                            if (!PInvoke.DeviceIoControl(hBattery,
                                    PInvoke.IOCTL_BATTERY_QUERY_STATUS,
                                    &bws,
                                    (uint)sizeof(BATTERY_WAIT_STATUS),
                                    &bs,
                                    (uint)sizeof(BATTERY_STATUS),
                                    &dwOut,
                                    null))
                            {
                                throw new Win32Exception(Marshal.GetLastWin32Error());
                            }
                            ChargingFlags chargingFlags = ChargingFlags.None;
                            if ((bs.PowerState & PInvoke.BATTERY_CHARGING) != 0)
                            {
                                chargingFlags |= ChargingFlags.ExternalPowerCharging;
                            }
                            if ((bs.PowerState & PInvoke.BATTERY_POWER_ON_LINE) != 0)
                            {
                                chargingFlags |= ChargingFlags.ExternalPowerConnected;
                            }
                            if ((bs.PowerState & PInvoke.BATTERY_DISCHARGING) != 0)
                            {
                                chargingFlags |= ChargingFlags.Discharging;
                            }
                            if ((bs.PowerState & PInvoke.BATTERY_CRITICAL) != 0)
                            {
                                chargingFlags |= ChargingFlags.FailureImminent;
                            }
                            batteryStates.Add(new DeviceIoBatteryState
                            {
                                Capacity = bs.Capacity == PInvoke.BATTERY_UNKNOWN_CAPACITY ? null : bs.Capacity,
                                Rate = bs.Rate == unchecked((int)PInvoke.BATTERY_UNKNOWN_RATE) ? null : bs.Rate,
                                Voltage = bs.Voltage == PInvoke.BATTERY_UNKNOWN_VOLTAGE ? null : bs.Voltage,
                                FullChargedCapacity = bi.FullChargedCapacity,
                                DesignedCapacity = bi.DesignedCapacity,
                                CapacityIsRelative = (bi.Capabilities & PInvoke.BATTERY_CAPACITY_RELATIVE) != 0,
                                Temperature = temperature, //
                                EstimatedTime = estimatedTime == PInvoke.BATTERY_UNKNOWN_TIME ? null : estimatedTime, //
                                ChargingFlags = chargingFlags, //
                            });
                        }
                    }
                }
            }
        }
        finally
        {
            if (deviceInterfaceList != new PWSTR(null))
            {
                PInvoke.HeapFree(PInvoke.GetProcessHeap(),
                    0,
                    deviceInterfaceList);
            }
        }
        return new WindowsDeviceIoSystemBatterySnapshot(batteryStates);
    }
}
