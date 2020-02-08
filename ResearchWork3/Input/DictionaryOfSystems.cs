using System;
using System.Collections.Generic;

namespace ResearchWork3.Input
{
    public class DictionaryOfSystems : InputParametersOfSystem
    {
        private readonly Dictionary<string, InputParametersOfSystem> _systems =
            new Dictionary<string, InputParametersOfSystem>
            {
                {
                    "SDSS J1439+1117", new InputParametersOfSystem
                    {
                        SystemName = "SDSS J1439+1117",
                        RotationLevelsPr = new[]
                        {
                            13.27, 13.48, 13.18
                        },
                        DRotationLevelsPr = new[]
                        {
                            0.03, 0.02, 0.06
                        },
                        ExportName = "System 3.1 (SDSS J1439+1117).dat",
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
                        NStep = 0.25,
                        NRound = 2,
                        TemperatureKinMin = 70,
                        TemperatureKinMax = 150,
                        TemperatureKinStep = 1,
                        TemperatureKinRound = 0,
                        TemperatureCmbMin = 7.8,
                        TemperatureCmbMax = 9.5,
                        TemperatureCmbStep = 0.05,
                        TemperatureCmbRound = 2,
                        N0Min = 13.235,
                        N0Max = 13.29,
                        N0Step = 0.001,
                        N0Round = 3
                    }
                },
                {
                    "SDSS J1237+0647", new InputParametersOfSystem
                    {
                        SystemName = "SDSS J1237+0647",
                        RotationLevelsPr = new[]
                        {
                            13.53, 13.77, 13.54
                        },
                        DRotationLevelsPr = new[]
                        {
                            0.04, 0.02, 0.03
                        },
                        ExportName = "System 3.2 (SDSS J1237+0647).dat",
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
                        NStep = 0.25,
                        NRound = 2,
                        TemperatureKinMin = 70,
                        TemperatureKinMax = 190,
                        TemperatureKinStep = 1,
                        TemperatureKinRound = 0,
                        TemperatureCmbMin = 9.4,
                        TemperatureCmbMax = 10.9,
                        TemperatureCmbStep = 0.05,
                        TemperatureCmbRound = 2,
                        N0Min = 13.485,
                        N0Max = 13.545,
                        N0Step = 0.001,
                        N0Round = 3
                    }
                },
                {
                    "SDSS J1047+2057", new InputParametersOfSystem
                    {
                        SystemName = "SDSS J1047+2057",
                        RotationLevelsPr = new[]
                        {
                            14.28, 14.37, 14.05, 13.22
                        },
                        DRotationLevelsPr = new[]
                        {
                            0.16, 0.11, 0.05, 0.11
                        },
                        ExportName = "System 3.3 (SDSS J1047+2057).dat",
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
                        NStep = 0.25,
                        NRound = 2,
                        TemperatureKinMin = 40,
                        TemperatureKinMax = 250,
                        TemperatureKinStep = 2.5,
                        TemperatureKinRound = 1,
                        TemperatureCmbMin = 6.4,
                        TemperatureCmbMax = 8.4,
                        TemperatureCmbStep = 0.05,
                        TemperatureCmbRound = 2,
                        N0Min = 14.1,
                        N0Max = 14.4,
                        N0Step = 0.005,
                        N0Round = 3
                    }
                },
                {
                    "SDSS J0000+0048", new InputParametersOfSystem
                    {
                        SystemName = "SDSS J0000+0048",
                        RotationLevelsPr = new[]
                        {
                            14.43, 14.52, 14.33, 13.73
                        },
                        DRotationLevelsPr = new[]
                        {
                            0.12, 0.08, 0.06, 0.05
                        },
                        ExportName = "System 3.4 (SDSS J0000+0048).dat",
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
                        NStep = 0.25,
                        NRound = 2,
                        TemperatureKinMin = 40,
                        TemperatureKinMax = 80,
                        TemperatureKinStep = 0.5,
                        TemperatureKinRound = 1,
                        TemperatureCmbMin = 8.8,
                        TemperatureCmbMax = 10.4,
                        TemperatureCmbStep = 0.025,
                        TemperatureCmbRound = 3,
                        N0Min = 14.26,
                        N0Max = 14.41,
                        N0Step = 0.002,
                        N0Round = 3
                    }
                }
            };

        public InputParametersOfSystem System(string system)
        {
            if (_systems.TryGetValue(system, out var value))
            {
                return value;
            }

            throw new Exception("Нет такой системы!");
        }
    }
}