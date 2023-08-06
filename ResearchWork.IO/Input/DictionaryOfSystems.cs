using ResearchWork.IO.Names;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ResearchWork.IO.Input
{
    public class DictionaryOfSystems : InputParametersOfSystem
    {
        private static readonly Lazy<DictionaryOfSystems> Lazy = new(() => new DictionaryOfSystems());

        private Dictionary<string, InputParametersOfSystem> _systemsDictionary = null!;

        public DictionaryOfSystems()
        {
            Init();
        }

        public static DictionaryOfSystems Instance => Lazy.Value;

        private void Init()
        {
            var systems = new List<InputParametersOfSystem>
            {
                new()
                {
                    SystemName = SystemNames.SYSTEM_J1439,
                    RotationLevelsPr = new[]
                    {
                        13.27, 13.48, 13.18
                    },
                    DRotationLevelsPr = new[]
                    {
                        0.03, 0.02, 0.06
                    },
                    ExportName = $"System 3.1 ({SystemNames.SYSTEM_J1439}).dat",
                    Z = 2.41837,
                    NumberOfLevels = 9,
                    F = 1,
                    TemperatureKinPr = 105,
                    DTemperatureKinMin = 32,
                    DTemperatureKinMax = 42,
                    NPr = 54,
                    DnMin = 9,
                    DnMax = 8,
                    NMin = 43,
                    NMax = 63,
                    NStep = 0.25m,
                    NRound = 2,
                    TemperatureKinMin = 70,
                    TemperatureKinMax = 150,
                    TemperatureKinStep = 1,
                    TemperatureKinRound = 0,
                    TemperatureCmbMin = 7.8m,
                    TemperatureCmbMax = 9.5m,
                    TemperatureCmbStep = 0.05m,
                    TemperatureCmbRound = 2,
                    N0Min = 13.235m,
                    N0Max = 13.29m,
                    N0Step = 0.001m,
                    N0Round = 3
                },
                new()
                {
                    SystemName = SystemNames.SYSTEM_J1237,
                    RotationLevelsPr = new[]
                    {
                        13.53, 13.77, 13.54
                    },
                    DRotationLevelsPr = new[]
                    {
                        0.04, 0.02, 0.03
                    },
                    ExportName = $"System 3.2 ({SystemNames.SYSTEM_J1237}).dat",
                    Z = 2.68955,
                    NumberOfLevels = 9,
                    F = 1,
                    TemperatureKinPr = 108,
                    DTemperatureKinMin = 33,
                    DTemperatureKinMax = 84,
                    NPr = 55,
                    DnMin = 5,
                    DnMax = 5,
                    NMin = 49,
                    NMax = 60,
                    NStep = 0.25m,
                    NRound = 2,
                    TemperatureKinMin = 70,
                    TemperatureKinMax = 190,
                    TemperatureKinStep = 1,
                    TemperatureKinRound = 0,
                    TemperatureCmbMin = 9.4m,
                    TemperatureCmbMax = 10.9m,
                    TemperatureCmbStep = 0.05m,
                    TemperatureCmbRound = 2,
                    N0Min = 13.485m,
                    N0Max = 13.545m,
                    N0Step = 0.001m,
                    N0Round = 3
                },
                new()
                {
                    SystemName = SystemNames.SYSTEM_J1047,
                    RotationLevelsPr = new[]
                    {
                        14.28, 14.37, 14.05, 13.22
                    },
                    DRotationLevelsPr = new[]
                    {
                        0.16, 0.11, 0.05, 0.11
                    },
                    ExportName = $"System 3.3 ({SystemNames.SYSTEM_J1047}).dat",
                    Z = 1.7738,
                    NumberOfLevels = 9,
                    F = 1,
                    TemperatureKinPr = 100,
                    DTemperatureKinMin = 50,
                    DTemperatureKinMax = 150,
                    NPr = 55,
                    DnMin = 5,
                    DnMax = 5,
                    NMin = 49,
                    NMax = 60,
                    NStep = 0.25m,
                    NRound = 2,
                    TemperatureKinMin = 40,
                    TemperatureKinMax = 250,
                    TemperatureKinStep = 2.5m,
                    TemperatureKinRound = 1,
                    TemperatureCmbMin = 6.4m,
                    TemperatureCmbMax = 8.4m,
                    TemperatureCmbStep = 0.05m,
                    TemperatureCmbRound = 2,
                    N0Min = 14.1m,
                    N0Max = 14.4m,
                    N0Step = 0.005m,
                    N0Round = 3
                },
                new()
                {
                    SystemName = SystemNames.SYSTEM_J0000,
                    RotationLevelsPr = new[]
                    {
                        14.43, 14.52, 14.33, 13.73
                    },
                    DRotationLevelsPr = new[]
                    {
                        0.12, 0.08, 0.06, 0.05
                    },
                    ExportName = $"System 3.4 ({SystemNames.SYSTEM_J0000}).dat",
                    Z = 2.525456,
                    NumberOfLevels = 9,
                    F = 1,
                    TemperatureKinPr = 56,
                    DTemperatureKinMin = 8,
                    DTemperatureKinMax = 19,
                    NPr = 80,
                    DnMin = 5,
                    DnMax = 5,
                    NMin = 74,
                    NMax = 86,
                    NStep = 0.25m,
                    NRound = 2,
                    TemperatureKinMin = 40,
                    TemperatureKinMax = 80,
                    TemperatureKinStep = 0.5m,
                    TemperatureKinRound = 1,
                    TemperatureCmbMin = 8.8m,
                    TemperatureCmbMax = 10.4m,
                    TemperatureCmbStep = 0.025m,
                    TemperatureCmbRound = 3,
                    N0Min = 14.26m,
                    N0Max = 14.41m,
                    N0Step = 0.002m,
                    N0Round = 3
                }
            };

            _systemsDictionary = systems.ToDictionary(x => x.SystemName, x => x);
        }

        public InputParametersOfSystem GetSystem(string system)
        {
            if (_systemsDictionary.ContainsKey(system))
            {
                return _systemsDictionary[system];
            }

            throw new Exception("Нет такой системы!");
        }
    }
}