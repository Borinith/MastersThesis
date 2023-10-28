using ResearchWork.Application.StartCalculation;
using ResearchWork.Application.Utils;
using ResearchWork.IO.Export;
using ResearchWork.IO.Input;
using ResearchWork.IO.Names;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

// ReSharper disable ConvertTypeCheckPatternToNullCheck

namespace ResearchWork.Application
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string TIME_PROGRESS_FORMAT = @"d\ hh\:mm\:ss";
        private readonly IReadOnlyDictionary<string, Button> _buttons;
        private readonly IExportTable _exportTable;
        private readonly IStartCalculation _startCalculation;
        private CancellationTokenSource? _cancellationTokenSource;

        public MainWindow(IExportTable exportTable, IStartCalculation startCalculation)
        {
            _exportTable = exportTable;
            _startCalculation = startCalculation;

            InitializeComponent();
            CommonWindow.Children.Clear();

            _buttons = Buttons();
            CreateButtons();
        }

        private void CreateButtons()
        {
            CommonWindow.Children.Add(GridButtons);

            _buttons.TryGetValue(SystemNames.SYSTEM_J1439, out var systemJ1439);
            Grid.SetRow(systemJ1439 ?? throw new InvalidOperationException(), 1);
            Grid.SetColumn(systemJ1439, 0);
            GridButtons.Children.Add(systemJ1439);

            _buttons.TryGetValue(SystemNames.SYSTEM_J1237, out var systemJ1237);
            Grid.SetRow(systemJ1237 ?? throw new InvalidOperationException(), 2);
            Grid.SetColumn(systemJ1237, 0);
            GridButtons.Children.Add(systemJ1237);

            _buttons.TryGetValue(SystemNames.SYSTEM_J1047, out var systemJ1047);
            Grid.SetRow(systemJ1047 ?? throw new InvalidOperationException(), 3);
            Grid.SetColumn(systemJ1047, 0);
            GridButtons.Children.Add(systemJ1047);

            _buttons.TryGetValue(SystemNames.SYSTEM_J0000, out var systemJ0000);
            Grid.SetRow(systemJ0000 ?? throw new InvalidOperationException(), 4);
            Grid.SetColumn(systemJ0000, 0);
            GridButtons.Children.Add(systemJ0000);
        }

        private IReadOnlyDictionary<string, Button> Buttons()
        {
            var result = new Dictionary<string, Button>();

            var systemJ1439 = new Button
            {
                Width = 120,
                Height = 30,
                Content = SystemNames.SYSTEM_J1439,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            systemJ1439.Click += ButtonClick;

            result.Add(systemJ1439.Content.ToString()!, systemJ1439);

            var systemJ1237 = new Button
            {
                Width = 120,
                Height = 30,
                Content = SystemNames.SYSTEM_J1237,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            systemJ1237.Click += ButtonClick;

            result.Add(systemJ1237.Content.ToString()!, systemJ1237);

            var systemJ1047 = new Button
            {
                Width = 120,
                Height = 30,
                Content = SystemNames.SYSTEM_J1047,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            systemJ1047.Click += ButtonClick;

            result.Add(systemJ1047.Content.ToString()!, systemJ1047);

            var systemJ0000 = new Button
            {
                Width = 120,
                Height = 30,
                Content = SystemNames.SYSTEM_J0000,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            systemJ0000.Click += ButtonClick;

            result.Add(systemJ0000.Content.ToString()!, systemJ0000);

            var startButton = new Button
            {
                Width = 120,
                Height = 30,
                Content = ButtonNames.START,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            startButton.Click += ButtonSystemClickAsync;

            result.Add(startButton.Content.ToString()!, startButton);

            var stopButton = new Button
            {
                Width = 120,
                Height = 30,
                Content = ButtonNames.STOP,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            stopButton.Click += ButtonSystemClickAsync;

            result.Add(stopButton.Content.ToString()!, stopButton);

            var backButton = new Button
            {
                Width = 120,
                Height = 30,
                Content = ButtonNames.BACK,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            backButton.Click += ButtonSystemClickAsync;

            result.Add(backButton.Content.ToString()!, backButton);

            return result.AsReadOnly();
        }

        private void ButtonsStartStopAndBack(IReadOnlyDictionary<string, Button> buttons, bool isStart)
        {
            GridSystem.Children.OfType<Button>().ToList().ForEach(x => GridSystem.Children.Remove(x));

            buttons.TryGetValue(ButtonNames.START, out var startButton);
            buttons.TryGetValue(ButtonNames.BACK, out var backButton);
            buttons.TryGetValue(ButtonNames.STOP, out var stopButton);

            if (isStart)
            {
                Grid.SetColumnSpan(startButton ?? throw new InvalidOperationException(), 4);
                Grid.SetRow(startButton, 6);
                Grid.SetColumn(startButton, 4);
                GridSystem.Children.Add(startButton);

                Grid.SetColumnSpan(backButton ?? throw new InvalidOperationException(), 4);
                Grid.SetRow(backButton, 6);
                Grid.SetColumn(backButton, 0);
                GridSystem.Children.Add(backButton);
            }
            else
            {
                Grid.SetColumnSpan(stopButton ?? throw new InvalidOperationException(), 8);
                Grid.SetRow(stopButton, 6);
                Grid.SetColumn(stopButton, 0);
                GridSystem.Children.Add(stopButton);
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            GridButtons.Children.Clear();

            if (sender is Button senderButton)
            {
                var inputParameters = DictionaryOfSystems.Instance.GetSystem(senderButton.Content.ToString() ?? string.Empty);
                ParametersOfSystem(inputParameters, true);
            }

            ButtonsStartStopAndBack(_buttons, true);
        }

        private void ParametersOfSystem(InputParametersOfSystem inputParametersOfSystem, bool isEnabled)
        {
            CommonWindow.Children.Add(GridSystem);

            if (GridSystem.Children.OfType<Label>().FirstOrDefault(i => i.Tag?.ToString() == TextBoxNames.SYSTEM_NAME) is Label systemName)
            {
                systemName.Content = inputParametersOfSystem.SystemName;
            }

            var textBoxes = GridSystem.Children.OfType<TextBox>()
                .Where(x => x.Tag != null)
                .ToDictionary(x => x.Tag.ToString()!, x => x);

            #region Концентрация (n)

            if (textBoxes.TryGetValue(TextBoxNames.N_MIN_VALUE, out var nMinValue))
            {
                nMinValue.Text = inputParametersOfSystem.NMin.ToString(CultureInfo.InvariantCulture);
                nMinValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.N_MAX_VALUE, out var nMaxValue))
            {
                nMaxValue.Text = inputParametersOfSystem.NMax.ToString(CultureInfo.InvariantCulture);
                nMaxValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.N_STEP_VALUE, out var nStepValue))
            {
                nStepValue.Text = inputParametersOfSystem.NStep.ToString(CultureInfo.InvariantCulture);
                nStepValue.IsEnabled = isEnabled;
            }

            #endregion Концентрация (n)

            #region Кинетическая температура (Tkin)

            if (textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_KIN_MIN_VALUE, out var temperatureKinMinValue))
            {
                temperatureKinMinValue.Text =
                    inputParametersOfSystem.TemperatureKinMin.ToString(CultureInfo.InvariantCulture);

                temperatureKinMinValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_KIN_MAX_VALUE, out var temperatureKinMaxValue))
            {
                temperatureKinMaxValue.Text =
                    inputParametersOfSystem.TemperatureKinMax.ToString(CultureInfo.InvariantCulture);

                temperatureKinMaxValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_KIN_STEP_VALUE, out var temperatureKinStepValue))
            {
                temperatureKinStepValue.Text =
                    inputParametersOfSystem.TemperatureKinStep.ToString(CultureInfo.InvariantCulture);

                temperatureKinStepValue.IsEnabled = isEnabled;
            }

            #endregion Кинетическая температура (Tkin)

            #region Температура РИ (Tcmb)

            if (textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_CMB_MIN_VALUE, out var temperatureCmbMinValue))
            {
                temperatureCmbMinValue.Text =
                    inputParametersOfSystem.TemperatureCmbMin.ToString(CultureInfo.InvariantCulture);

                temperatureCmbMinValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_CMB_MAX_VALUE, out var temperatureCmbMaxValue))
            {
                temperatureCmbMaxValue.Text =
                    inputParametersOfSystem.TemperatureCmbMax.ToString(CultureInfo.InvariantCulture);

                temperatureCmbMaxValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_CMB_STEP_VALUE, out var temperatureCmbStepValue))
            {
                temperatureCmbStepValue.Text =
                    inputParametersOfSystem.TemperatureCmbStep.ToString(CultureInfo.InvariantCulture);

                temperatureCmbStepValue.IsEnabled = isEnabled;
            }

            #endregion Температура РИ (Tcmb)

            #region Лучевая концентрация (N0)

            if (textBoxes.TryGetValue(TextBoxNames.N0_MIN_VALUE, out var n0MinValue))
            {
                n0MinValue.Text = inputParametersOfSystem.N0Min.ToString(CultureInfo.InvariantCulture);
                n0MinValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.N0_MAX_VALUE, out var n0MaxValue))
            {
                n0MaxValue.Text = inputParametersOfSystem.N0Max.ToString(CultureInfo.InvariantCulture);
                n0MaxValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.N0_STEP_VALUE, out var n0StepValue))
            {
                n0StepValue.Text = inputParametersOfSystem.N0Step.ToString(CultureInfo.InvariantCulture);
                n0StepValue.IsEnabled = isEnabled;
            }

            #endregion Лучевая концентрация (N0)

            #region Количество уровней и имя файла

            if (GridSystem.Children.OfType<IntegerUpDown>().FirstOrDefault(i => i.Tag?.ToString() == TextBoxNames.N_LEVELS_VALUE) is IntegerUpDown nLevelsValue)
            {
                nLevelsValue.Text = inputParametersOfSystem.NumberOfLevels.ToString();
                nLevelsValue.Maximum = InputCommonParameters.MAX_CO_LEVEL;
                nLevelsValue.IsEnabled = isEnabled;
            }

            if (textBoxes.TryGetValue(TextBoxNames.FILE_NAME_VALUE, out var fileNameValue))
            {
                fileNameValue.Text = inputParametersOfSystem.ExportName;
                fileNameValue.IsEnabled = isEnabled;
            }

            #endregion Количество уровней и имя файла

            #region Обнуление прогресс бара и времени выполнения

            var progress = new Progress<double>(s => PbStatus.Value = s);
            var stopProgress = (IProgress<double>)progress;

            var timeProgress = new Progress<TimeSpan>(s => TbTime.Content = s.ToString(TIME_PROGRESS_FORMAT));
            var timeStopProgress = (IProgress<TimeSpan>)timeProgress;

            stopProgress.Report(0);
            timeStopProgress.Report(new TimeSpan(0));

            #endregion Обнуление прогресс бара и времени выполнения
        }

        private async void ButtonSystemClickAsync(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;

            switch (senderButton?.Content.ToString())
            {
                case ButtonNames.START:
                case ButtonNames.STOP:
                {
                    var systemName = GridSystem.Children.OfType<Label>().FirstOrDefault(i => i.Tag?.ToString() == TextBoxNames.SYSTEM_NAME)?.Content.ToString() ?? string.Empty;
                    var inputParameters = DictionaryOfSystems.Instance.GetSystem(systemName);

                    var newInputParameters = NewInputParametersOfSystem(inputParameters);
                    CommonWindow.Children.Clear();

                    switch (senderButton.Content.ToString())
                    {
                        case ButtonNames.START:
                        {
                            ParametersOfSystem(newInputParameters, false);
                            ButtonsStartStopAndBack(_buttons, false);

                            _cancellationTokenSource = new CancellationTokenSource();

                            var progress = new Progress<double>(s => PbStatus.Value = s);
                            var stopProgress = (IProgress<double>)progress;
                            var timeProgress = new Progress<TimeSpan>(s => TbTime.Content = s.ToString(TIME_PROGRESS_FORMAT));

                            var calculationX2 = await _startCalculation.CalculationX2Table(
                                newInputParameters,
                                progress,
                                timeProgress,
                                _cancellationTokenSource.Token);

                            if (!_cancellationTokenSource.IsCancellationRequested)
                            {
                                await _exportTable.ExportSortedTable(calculationX2, newInputParameters.ExportName);

                                CommonWindow.Children.Clear();

                                ParametersOfSystem(newInputParameters, true);
                                ButtonsStartStopAndBack(_buttons, true);

                                stopProgress.Report(100);
                            }

                            _cancellationTokenSource?.Dispose();

                            break;
                        }

                        case ButtonNames.STOP:
                            _cancellationTokenSource?.Cancel();
                            CommonWindow.Children.Clear();

                            ParametersOfSystem(newInputParameters, true);
                            ButtonsStartStopAndBack(_buttons, true);

                            _cancellationTokenSource?.Dispose();

                            break;

                        default:
                            throw new ArgumentException();
                    }

                    break;
                }

                case ButtonNames.BACK:
                    CommonWindow.Children.Clear();
                    CreateButtons();

                    break;

                default:
                    throw new ArgumentException();
            }

            SetProgress.SetProgressValue(0, 100);
            ReleaseMemory();
        }

        private static void ReleaseMemory()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

            for (var i = 0; i < 20; i++)
            {
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }
        }

        private InputParametersOfSystem NewInputParametersOfSystem(InputParametersOfSystem inputParameters)
        {
            var textBoxes = GridSystem.Children.OfType<TextBox>()
                .Where(x => x.Tag != null)
                .ToDictionary(x => x.Tag.ToString()!, x => x);

            #region Концентрация (n)

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.N_MIN_VALUE, out var nMinValue) ? nMinValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var nMinNumber);

            inputParameters.NMin = nMinNumber;

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.N_MAX_VALUE, out var nMaxValue) ? nMaxValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var nMaxNumber);

            inputParameters.NMax = nMaxNumber;

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.N_STEP_VALUE, out var nStepValue) ? nStepValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var nStepNumber);

            inputParameters.NStep = nStepNumber;

            #endregion Концентрация (n)

            #region Кинетическая температура (Tkin)

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_KIN_MIN_VALUE, out var temperatureKinMinValue) ? temperatureKinMinValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var temperatureKinMinNumber);

            inputParameters.TemperatureKinMin = temperatureKinMinNumber;

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_KIN_MAX_VALUE, out var temperatureKinMaxValue) ? temperatureKinMaxValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var temperatureKinMaxNumber);

            inputParameters.TemperatureKinMax = temperatureKinMaxNumber;

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_KIN_STEP_VALUE, out var temperatureKinStepValue) ? temperatureKinStepValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var temperatureKinStepNumber);

            inputParameters.TemperatureKinStep = temperatureKinStepNumber;

            #endregion Кинетическая температура (Tkin)

            #region Температура РИ (Tcmb)

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_CMB_MIN_VALUE, out var temperatureCmbMinValue) ? temperatureCmbMinValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var temperatureCmbMinNumber);

            inputParameters.TemperatureCmbMin = temperatureCmbMinNumber;

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_CMB_MAX_VALUE, out var temperatureCmbMaxValue) ? temperatureCmbMaxValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var temperatureCmbMaxNumber);

            inputParameters.TemperatureCmbMax = temperatureCmbMaxNumber;

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.TEMPERATURE_CMB_STEP_VALUE, out var temperatureCmbStepValue) ? temperatureCmbStepValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var temperatureCmbStepNumber);

            inputParameters.TemperatureCmbStep = temperatureCmbStepNumber;

            #endregion Температура РИ (Tcmb)

            #region Лучевая концентрация (N0)

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.N0_MIN_VALUE, out var n0MinValue) ? n0MinValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var n0MinNumber);

            inputParameters.N0Min = n0MinNumber;

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.N0_MAX_VALUE, out var n0MaxValue) ? n0MaxValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var n0MaxNumber);

            inputParameters.N0Max = n0MaxNumber;

            decimal.TryParse(textBoxes.TryGetValue(TextBoxNames.N0_STEP_VALUE, out var n0StepValue) ? n0StepValue.Text : null,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var n0StepNumber);

            inputParameters.N0Step = n0StepNumber;

            #endregion Лучевая концентрация (N0)

            #region Количество уровней и имя файла

            int.TryParse(GridSystem.Children.OfType<IntegerUpDown>().FirstOrDefault(i => i.Tag?.ToString() == TextBoxNames.N_LEVELS_VALUE)?.Text,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var nLevelsNumber);

            inputParameters.NumberOfLevels = nLevelsNumber;

            inputParameters.ExportName = textBoxes.TryGetValue(TextBoxNames.FILE_NAME_VALUE, out var fileNameValue) ? fileNameValue.Text : string.Empty;

            #endregion Количество уровней и имя файла

            return inputParameters;
        }
    }
}