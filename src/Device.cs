using System.Text.RegularExpressions;

namespace SLMP {
    /// <summary>
    /// This enum contains the type of supported device types.
    /// </summary>
    public enum DeviceType {
        Bit,
        Word
    }

    /// <summary>
    /// This enum encodes the supported devices that is available to operate on.
    /// </summary>
    public enum Device {
        D = 0xa8,
        W = 0xb4,
        R = 0xaf,
        ZR = 0xb0,
        SD = 0xa9,
        X = 0x9c,
        Y = 0x9d,
        M = 0x90,
        B = 0xa0,
        SM = 0x91,
    }

    public class DeviceMethods {
        /// <summary>
        /// Gets the subcommand for a given `(Bit/Word)Device`.
        /// </summary>
        public static ushort GetSubcommand(Device device) {
            return DeviceMethods.GetDeviceType(device) switch {
                DeviceType.Bit => 0x0001,
                DeviceType.Word => 0x0000,
                _ => throw new ArgumentException("invalid device type provided"),
            };
        }

        /// <summary>
        /// This helper function will return either `DeviceType.Word` or `DeviceType.Bit`
        /// for a given `device`.
        /// </summary>
        /// <returns>DeviceType</returns>
        /// <exception cref="ArgumentException"></exception>
        public static DeviceType GetDeviceType(Device device) {
            return device switch {
                Device.D => DeviceType.Word,
                Device.W => DeviceType.Word,
                Device.R => DeviceType.Word,
                Device.ZR => DeviceType.Word,
                Device.SD => DeviceType.Word,
                Device.X => DeviceType.Bit,
                Device.Y => DeviceType.Bit,
                Device.M => DeviceType.Bit,
                Device.B => DeviceType.Bit,
                Device.SM => DeviceType.Bit,

                _ => throw new ArgumentException("invalid device")
            };
        }

        /// <summary>
        /// Helper function to get a `Device` from a given string.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool FromString(string device, out Device? value) {
            switch (device.ToUpper()) {
                case "D": value = Device.D; break;
                case "W": value = Device.W; break;
                case "R": value = Device.R; break;
                case "ZR": value = Device.ZR; break;
                case "SD": value = Device.SD; break;
                case "X": value = Device.X; break;
                case "Y": value = Device.Y; break;
                case "M": value = Device.M; break;
                case "B": value = Device.B; break;
                case "SM": value = Device.SM; break;
                default:
                    value = null;
                    return false;
            };

            return true;
        }

        /// <summary>
        /// Helper function to parse strings in the form `{DeviceName}{DeviceAddress}`.
        /// </summary>
        /// <returns>Tuple<Device, ushort></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Tuple<Device, ushort> ParseDeviceAddress(string address) {
            Regex rx = new(@"([a-zA-Z]+)(\d+)");
            Match match = rx.Match(address);

            if (match.Groups.Count < 3)
                throw new ArgumentException($"couldn't parse device address: {address}");

            string sdevice = match.Groups[1].Value;
            string saddr = match.Groups[2].Value;

            if (!FromString(sdevice, out Device? device)) throw new ArgumentException($"invalid device provided: {sdevice}");
            if (!UInt16.TryParse(saddr, out ushort uaddr)) throw new ArgumentException($"invalid address provided: {saddr}");

            // this exists to convince the type checker that `device` won't be null here
            if (device == null)
                throw new ArgumentException("unreachable");

            return Tuple.Create((Device)device, uaddr);
        }
    }
}
