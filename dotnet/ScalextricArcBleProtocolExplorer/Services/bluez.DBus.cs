using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace bluez.DBus
{
    [DBusInterface("org.freedesktop.DBus.ObjectManager")]
    interface IObjectManager : IDBusObject
    {
        Task<IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>> GetManagedObjectsAsync();
        Task<IDisposable> WatchInterfacesAddedAsync(Action<(ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces)> handler, Action<Exception> onError = null);
        Task<IDisposable> WatchInterfacesRemovedAsync(Action<(ObjectPath @object, string[] interfaces)> handler, Action<Exception> onError = null);
    }

    [DBusInterface("org.bluez.AgentManager1")]
    interface IAgentManager1 : IDBusObject
    {
        Task RegisterAgentAsync(ObjectPath Agent, string Capability);
        Task UnregisterAgentAsync(ObjectPath Agent);
        Task RequestDefaultAgentAsync(ObjectPath Agent);
    }

    [DBusInterface("org.bluez.ProfileManager1")]
    interface IProfileManager1 : IDBusObject
    {
        Task RegisterProfileAsync(ObjectPath Profile, string UUID, IDictionary<string, object> Options);
        Task UnregisterProfileAsync(ObjectPath Profile);
    }

    [DBusInterface("org.bluez.Adapter1")]
    interface IAdapter1 : IDBusObject
    {
        Task StartDiscoveryAsync();
        Task SetDiscoveryFilterAsync(IDictionary<string, object> Properties);
        Task StopDiscoveryAsync();
        Task RemoveDeviceAsync(ObjectPath Device);
        Task<string[]> GetDiscoveryFiltersAsync();
        Task<T> GetAsync<T>(string prop);
        Task<Adapter1Properties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    [Dictionary]
    class Adapter1Properties
    {
        private string _Address = default(string);
        public string Address
        {
            get
            {
                return _Address;
            }

            set
            {
                _Address = (value);
            }
        }

        private string _AddressType = default(string);
        public string AddressType
        {
            get
            {
                return _AddressType;
            }

            set
            {
                _AddressType = (value);
            }
        }

        private string _Name = default(string);
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = (value);
            }
        }

        private string _Alias = default(string);
        public string Alias
        {
            get
            {
                return _Alias;
            }

            set
            {
                _Alias = (value);
            }
        }

        private uint _Class = default(uint);
        public uint Class
        {
            get
            {
                return _Class;
            }

            set
            {
                _Class = (value);
            }
        }

        private bool _Powered = default(bool);
        public bool Powered
        {
            get
            {
                return _Powered;
            }

            set
            {
                _Powered = (value);
            }
        }

        private bool _Discoverable = default(bool);
        public bool Discoverable
        {
            get
            {
                return _Discoverable;
            }

            set
            {
                _Discoverable = (value);
            }
        }

        private uint _DiscoverableTimeout = default(uint);
        public uint DiscoverableTimeout
        {
            get
            {
                return _DiscoverableTimeout;
            }

            set
            {
                _DiscoverableTimeout = (value);
            }
        }

        private bool _Pairable = default(bool);
        public bool Pairable
        {
            get
            {
                return _Pairable;
            }

            set
            {
                _Pairable = (value);
            }
        }

        private uint _PairableTimeout = default(uint);
        public uint PairableTimeout
        {
            get
            {
                return _PairableTimeout;
            }

            set
            {
                _PairableTimeout = (value);
            }
        }

        private bool _Discovering = default(bool);
        public bool Discovering
        {
            get
            {
                return _Discovering;
            }

            set
            {
                _Discovering = (value);
            }
        }

        private string[] _UUIDs = default(string[]);
        public string[] UUIDs
        {
            get
            {
                return _UUIDs;
            }

            set
            {
                _UUIDs = (value);
            }
        }

        private string _Modalias = default(string);
        public string Modalias
        {
            get
            {
                return _Modalias;
            }

            set
            {
                _Modalias = (value);
            }
        }

        private string[] _Roles = default(string[]);
        public string[] Roles
        {
            get
            {
                return _Roles;
            }

            set
            {
                _Roles = (value);
            }
        }
    }

    static class Adapter1Extensions
    {
        public static Task<string> GetAddressAsync(this IAdapter1 o) => o.GetAsync<string>("Address");
        public static Task<string> GetAddressTypeAsync(this IAdapter1 o) => o.GetAsync<string>("AddressType");
        public static Task<string> GetNameAsync(this IAdapter1 o) => o.GetAsync<string>("Name");
        public static Task<string> GetAliasAsync(this IAdapter1 o) => o.GetAsync<string>("Alias");
        public static Task<uint> GetClassAsync(this IAdapter1 o) => o.GetAsync<uint>("Class");
        public static Task<bool> GetPoweredAsync(this IAdapter1 o) => o.GetAsync<bool>("Powered");
        public static Task<bool> GetDiscoverableAsync(this IAdapter1 o) => o.GetAsync<bool>("Discoverable");
        public static Task<uint> GetDiscoverableTimeoutAsync(this IAdapter1 o) => o.GetAsync<uint>("DiscoverableTimeout");
        public static Task<bool> GetPairableAsync(this IAdapter1 o) => o.GetAsync<bool>("Pairable");
        public static Task<uint> GetPairableTimeoutAsync(this IAdapter1 o) => o.GetAsync<uint>("PairableTimeout");
        public static Task<bool> GetDiscoveringAsync(this IAdapter1 o) => o.GetAsync<bool>("Discovering");
        public static Task<string[]> GetUUIDsAsync(this IAdapter1 o) => o.GetAsync<string[]>("UUIDs");
        public static Task<string> GetModaliasAsync(this IAdapter1 o) => o.GetAsync<string>("Modalias");
        public static Task<string[]> GetRolesAsync(this IAdapter1 o) => o.GetAsync<string[]>("Roles");
        public static Task SetAliasAsync(this IAdapter1 o, string val) => o.SetAsync("Alias", val);
        public static Task SetPoweredAsync(this IAdapter1 o, bool val) => o.SetAsync("Powered", val);
        public static Task SetDiscoverableAsync(this IAdapter1 o, bool val) => o.SetAsync("Discoverable", val);
        public static Task SetDiscoverableTimeoutAsync(this IAdapter1 o, uint val) => o.SetAsync("DiscoverableTimeout", val);
        public static Task SetPairableAsync(this IAdapter1 o, bool val) => o.SetAsync("Pairable", val);
        public static Task SetPairableTimeoutAsync(this IAdapter1 o, uint val) => o.SetAsync("PairableTimeout", val);
    }

    [DBusInterface("org.bluez.GattManager1")]
    interface IGattManager1 : IDBusObject
    {
        Task RegisterApplicationAsync(ObjectPath Application, IDictionary<string, object> Options);
        Task UnregisterApplicationAsync(ObjectPath Application);
    }

    [DBusInterface("org.bluez.LEAdvertisingManager1")]
    interface ILEAdvertisingManager1 : IDBusObject
    {
        Task RegisterAdvertisementAsync(ObjectPath Advertisement, IDictionary<string, object> Options);
        Task UnregisterAdvertisementAsync(ObjectPath Service);
        Task<T> GetAsync<T>(string prop);
        Task<LEAdvertisingManager1Properties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    [Dictionary]
    class LEAdvertisingManager1Properties
    {
        private byte _ActiveInstances = default(byte);
        public byte ActiveInstances
        {
            get
            {
                return _ActiveInstances;
            }

            set
            {
                _ActiveInstances = (value);
            }
        }

        private byte _SupportedInstances = default(byte);
        public byte SupportedInstances
        {
            get
            {
                return _SupportedInstances;
            }

            set
            {
                _SupportedInstances = (value);
            }
        }

        private string[] _SupportedIncludes = default(string[]);
        public string[] SupportedIncludes
        {
            get
            {
                return _SupportedIncludes;
            }

            set
            {
                _SupportedIncludes = (value);
            }
        }

        private string[] _SupportedSecondaryChannels = default(string[]);
        public string[] SupportedSecondaryChannels
        {
            get
            {
                return _SupportedSecondaryChannels;
            }

            set
            {
                _SupportedSecondaryChannels = (value);
            }
        }
    }

    static class LEAdvertisingManager1Extensions
    {
        public static Task<byte> GetActiveInstancesAsync(this ILEAdvertisingManager1 o) => o.GetAsync<byte>("ActiveInstances");
        public static Task<byte> GetSupportedInstancesAsync(this ILEAdvertisingManager1 o) => o.GetAsync<byte>("SupportedInstances");
        public static Task<string[]> GetSupportedIncludesAsync(this ILEAdvertisingManager1 o) => o.GetAsync<string[]>("SupportedIncludes");
        public static Task<string[]> GetSupportedSecondaryChannelsAsync(this ILEAdvertisingManager1 o) => o.GetAsync<string[]>("SupportedSecondaryChannels");
    }

    [DBusInterface("org.bluez.Media1")]
    interface IMedia1 : IDBusObject
    {
        Task RegisterEndpointAsync(ObjectPath Endpoint, IDictionary<string, object> Properties);
        Task UnregisterEndpointAsync(ObjectPath Endpoint);
        Task RegisterPlayerAsync(ObjectPath Player, IDictionary<string, object> Properties);
        Task UnregisterPlayerAsync(ObjectPath Player);
        Task RegisterApplicationAsync(ObjectPath Application, IDictionary<string, object> Options);
        Task UnregisterApplicationAsync(ObjectPath Application);
    }

    [DBusInterface("org.bluez.NetworkServer1")]
    interface INetworkServer1 : IDBusObject
    {
        Task RegisterAsync(string Uuid, string Bridge);
        Task UnregisterAsync(string Uuid);
    }
}