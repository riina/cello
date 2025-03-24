## cello

Battery cell observation

| Package              | Release                                                                                                               |
|----------------------|-----------------------------------------------------------------------------------------------------------------------|
| `Cello`                | [![NuGet](https://img.shields.io/nuget/v/Cello.svg)](https://www.nuget.org/packages/Cello/)                               |

### Supported Platforms

Windows 10 1607+, Windows 11 22H2+

Linux Kernel 6.0+

macOS 13

### Standard Property Support

Library per-platform support for standardized properties is listed in this table. Some batteries may not report certain values.

| OS/Feature      | Windows | Linux | macOS |
| --------------- | ------- | ----- | ----- |
| Charge %        |✅|✅|✅|
| Health %        |✅|✅|✅|
| Current Charge  |✅[1]|✅[1]|✅[2]|
| Max Charge      |✅[1]|✅[1]|✅[2]|
| Design Charge   |✅[1]|✅[1]|✅[2]|
| Charge Rate     |✅|✅|✅|
| Current Voltage |✅|✅|✅|
| Temperature     |✅|✅|✅|
| Charge State    |✅|✅|✅|
| Discharge Time  |❓[3]|❓[3]|✅|
| Charge Time     |❌|❓[3]|✅|

[1] Charge values on Windows and Linux are available as Milliwatt-hours.

[2] Charge values on macOS are available as Milliampere-hours.

[3] Discharge/charge time is estimated based on remaining capacity and current draw.
