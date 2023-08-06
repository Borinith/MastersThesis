using ResearchWork.Application.Utils;
using ResearchWork.IO.Export;
using ResearchWork.IO.Input;
using ResearchWork.IO.Names;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace ResearchWork.Application
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IExportTable _exportTable;
        private CancellationTokenSource? _cancellationTokenSource;

        public MainWindow(IExportTable exportTable)
        {
            _exportTable = exportTable;
            InitializeComponent();
            CommonWindow.Children.Clear();
            CreateButtons();
        }

        private void CreateButtons()
        {
            CommonWindow.Children.Add(GridButtons);

            var buttons = Buttons();

            buttons.TryGetValue(SystemNames.SYSTEM_J1439, out var systemJ1439);
            Grid.SetRow(systemJ1439 ?? throw new InvalidOperationException(), 1);
            Grid.SetColumn(systemJ1439, 0);
            systemJ1439.Click += ButtonClick;
            GridButtons.Children.Add(systemJ1439);

            buttons.TryGetValue(SystemNames.SYSTEM_J1237, out var systemJ1237);
            Grid.SetRow(systemJ1237 ?? throw new InvalidOperationException(), 2);
            Grid.SetColumn(systemJ1237, 0);
            systemJ1237.Click += ButtonClick;
            GridButtons.Children.Add(systemJ1237);

            buttons.TryGetValue(SystemNames.SYSTEM_J1047, out var systemJ1047);
            Grid.SetRow(systemJ1047 ?? throw new InvalidOperationException(), 3);
            Grid.SetColumn(systemJ1047, 0);
            systemJ1047.Click += ButtonClick;
            GridButtons.Children.Add(systemJ1047);

            buttons.TryGetValue(SystemNames.SYSTEM_J0000, out var systemJ0000);
            Grid.SetRow(systemJ0000 ?? throw new InvalidOperationException(), 4);
            Grid.SetColumn(systemJ0000, 0);
            systemJ0000.Click += ButtonClick;
            GridButtons.Children.Add(systemJ0000);
        }

        private static Dictionary<string, Button> Buttons()
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

            result.Add(systemJ1439.Content.ToString()!, systemJ1439);

            var systemJ1237 = new Button
            {
                Width = 120,
                Height = 30,
                Content = SystemNames.SYSTEM_J1237,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            result.Add(systemJ1237.Content.ToString()!, systemJ1237);

            var systemJ1047 = new Button
            {
                Width = 120,
                Height = 30,
                Content = SystemNames.SYSTEM_J1047,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            result.Add(systemJ1047.Content.ToString()!, systemJ1047);

            var systemJ0000 = new Button
            {
                Width = 120,
                Height = 30,
                Content = SystemNames.SYSTEM_J0000,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            result.Add(systemJ0000.Content.ToString()!, systemJ0000);

            var startButton = new Button
            {
                Width = 120,
                Height = 30,
                Content = ButtonNames.START,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            result.Add(startButton.Content.ToString()!, startButton);

            var stopButton = new Button
            {
                Width = 120,
                Height = 30,
                Content = ButtonNames.STOP,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            result.Add(stopButton.Content.ToString()!, stopButton);

            var backButton = new Button
            {
                Width = 120,
                Height = 30,
                Content = ButtonNames.BACK,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            result.Add(backButton.Content.ToString()!, backButton);

            return result;
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
                startButton.Click += ButtonSystemClickAsync;
                GridSystem.Children.Add(startButton);

                Grid.SetColumnSpan(backButton ?? throw new InvalidOperationException(), 4);
                Grid.SetRow(backButton, 6);
                Grid.SetColumn(backButton, 0);
                backButton.Click += ButtonSystemClickAsync;
                GridSystem.Children.Add(backButton);
            }
            else
            {
                Grid.SetColumnSpan(stopButton ?? throw new InvalidOperationException(), 8);
                Grid.SetRow(stopButton, 6);
                Grid.SetColumn(stopButton, 0);
                stopButton.Click += ButtonSystemClickAsync;
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

            ButtonsStartStopAndBack(Buttons(), true);
        }

        private void ParametersOfSystem(InputParametersOfSystem inputParametersOfSystem, bool isEnabled)
        {
            CommonWindow.Children.Add(GridSystem);

            if (GridSystem.FindName("SystemName") is Label systemName)
            {
                systemName.Content = inputParametersOfSystem.SystemName;
            }

            #region Концентрация (n)

            if (GridSystem.FindName("NMinValue") is TextBox nMinValue)
            {
                nMinValue.Text = inputParametersOfSystem.NMin.ToString(CultureInfo.InvariantCulture);
                nMinValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("NMaxValue") is TextBox nMaxValue)
            {
                nMaxValue.Text = inputParametersOfSystem.NMax.ToString(CultureInfo.InvariantCulture);
                nMaxValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("NStepValue") is TextBox nStepValue)
            {
                nStepValue.Text = inputParametersOfSystem.NStep.ToString(CultureInfo.InvariantCulture);
                nStepValue.IsEnabled = isEnabled;
            }

            #endregion Концентрация (n)

            #region Кинетическая температура (Tkin)

            if (GridSystem.FindName("TemperatureKinMinValue") is TextBox temperatureKinMinValue)
            {
                temperatureKinMinValue.Text =
                    inputParametersOfSystem.TemperatureKinMin.ToString(CultureInfo.InvariantCulture);

                temperatureKinMinValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("TemperatureKinMaxValue") is TextBox temperatureKinMaxValue)
            {
                temperatureKinMaxValue.Text =
                    inputParametersOfSystem.TemperatureKinMax.ToString(CultureInfo.InvariantCulture);

                temperatureKinMaxValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("TemperatureKinStepValue") is TextBox temperatureKinStepValue)
            {
                temperatureKinStepValue.Text =
                    inputParametersOfSystem.TemperatureKinStep.ToString(CultureInfo.InvariantCulture);

                temperatureKinStepValue.IsEnabled = isEnabled;
            }

            #endregion Кинетическая температура (Tkin)

            #region Температура РИ (Tcmb)

            if (GridSystem.FindName("TemperatureCmbMinValue") is TextBox temperatureCmbMinValue)
            {
                temperatureCmbMinValue.Text =
                    inputParametersOfSystem.TemperatureCmbMin.ToString(CultureInfo.InvariantCulture);

                temperatureCmbMinValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("TemperatureCmbMaxValue") is TextBox temperatureCmbMaxValue)
            {
                temperatureCmbMaxValue.Text =
                    inputParametersOfSystem.TemperatureCmbMax.ToString(CultureInfo.InvariantCulture);

                temperatureCmbMaxValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("TemperatureCmbStepValue") is TextBox temperatureCmbStepValue)
            {
                temperatureCmbStepValue.Text =
                    inputParametersOfSystem.TemperatureCmbStep.ToString(CultureInfo.InvariantCulture);

                temperatureCmbStepValue.IsEnabled = isEnabled;
            }

            #endregion Температура РИ (Tcmb)

            #region Лучевая концентрация (N0)

            if (GridSystem.FindName("N0MinValue") is TextBox n0MinValue)
            {
                n0MinValue.Text = inputParametersOfSystem.N0Min.ToString(CultureInfo.InvariantCulture);
                n0MinValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("N0MaxValue") is TextBox n0MaxValue)
            {
                n0MaxValue.Text = inputParametersOfSystem.N0Max.ToString(CultureInfo.InvariantCulture);
                n0MaxValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("N0StepValue") is TextBox n0StepValue)
            {
                n0StepValue.Text = inputParametersOfSystem.N0Step.ToString(CultureInfo.InvariantCulture);
                n0StepValue.IsEnabled = isEnabled;
            }

            #endregion Лучевая концентрация (N0)

            #region Количество уровней и имя файла

            if (GridSystem.FindName("NLevelsValue") is IntegerUpDown nLevelsValue)
            {
                nLevelsValue.Text = inputParametersOfSystem.NumberOfLevels.ToString();
                nLevelsValue.Maximum = InputCommonParameters.MAX_CO_LEVEL;
                nLevelsValue.IsEnabled = isEnabled;
            }

            if (GridSystem.FindName("FileNameValue") is TextBox fileNameValue)
            {
                fileNameValue.Text = inputParametersOfSystem.ExportName;
                fileNameValue.IsEnabled = isEnabled;
            }

            #endregion Количество уровней и имя файла

            #region Обнуление прогресс бара и времени выполнения

            var progress = new Progress<double>(s => PbStatus.Value = s);
            var stopProgress = (IProgress<double>)progress;

            var timeProgress = new Progress<TimeSpan>(s => TbTime.Content = s.ToString(@"d\ hh\:mm\:ss"));
            var timeStopProgress = (IProgress<TimeSpan>)timeProgress;

            stopProgress.Report(0);
            timeStopProgress.Report(new TimeSpan(0));

            #endregion Обнуление прогресс бара и времени выполнения
        }

        private async void ButtonSystemClickAsync(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;

            var systemName = (GridSystem.FindName("SystemName") as Label)?.Content.ToString() ?? string.Empty;
            var inputParameters = DictionaryOfSystems.Instance.GetSystem(systemName);

            var newInputParameters = NewInputParametersOfSystem(inputParameters);
            CommonWindow.Children.Clear();

            var progress = new Progress<double>(s => PbStatus.Value = s);
            var stopProgress = (IProgress<double>)progress;

            var timeProgress = new Progress<TimeSpan>(s => TbTime.Content = s.ToString(@"d\ hh\:mm\:ss"));

            switch (senderButton?.Content.ToString())
            {
                case ButtonNames.START:
                    ParametersOfSystem(newInputParameters, false);
                    ButtonsStartStopAndBack(Buttons(), false);

                    _cancellationTokenSource = new CancellationTokenSource();

                    var calculationX2 = await StartCalculation.CalculationX2Table(
                        newInputParameters,
                        progress,
                        timeProgress,
                        _cancellationTokenSource.Token);

                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        await _exportTable.ExportSortedTable(calculationX2, newInputParameters.ExportName);

                        CommonWindow.Children.Clear();

                        ParametersOfSystem(newInputParameters, true);
                        ButtonsStartStopAndBack(Buttons(), true);

                        stopProgress.Report(100);
                    }

                    _cancellationTokenSource?.Dispose();

                    break;

                case ButtonNames.STOP:
                    _cancellationTokenSource?.Cancel();
                    CommonWindow.Children.Clear();

                    ParametersOfSystem(newInputParameters, true);
                    ButtonsStartStopAndBack(Buttons(), true);

                    _cancellationTokenSource?.Dispose();

                    break;

                case ButtonNames.BACK:
                    CommonWindow.Children.Clear();
                    CreateButtons();

                    break;
            }

            SetProgress.SetProgressValue(0, 100);
        }

        private InputParametersOfSystem NewInputParametersOfSystem(InputParametersOfSystem inputParameters)
        {
            #region Концентрация (n)

            decimal.TryParse((GridSystem.FindName("NMinValue") as TextBox)?.Text, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var nMinValue);

            inputParameters.NMin = nMinValue;

            decimal.TryParse((GridSystem.FindName("NMaxValue") as TextBox)?.Text, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var nMaxValue);

            inputParameters.NMax = nMaxValue;

            decimal.TryParse((GridSystem.FindName("NStepValue") as TextBox)?.Text, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var nStepValue);

            inputParameters.NStep = nStepValue;

            #endregion Концентрация (n)

            #region Кинетическая температура (Tkin)

            decimal.TryParse((GridSystem.FindName("TemperatureKinMinValue") as TextBox)?.Text,
                NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out var temperatureKinMinValue);

            inputParameters.TemperatureKinMin = temperatureKinMinValue;

            decimal.TryParse((GridSystem.FindName("TemperatureKinMaxValue") as TextBox)?.Text,
                NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out var temperatureKinMaxValue);

            inputParameters.TemperatureKinMax = temperatureKinMaxValue;

            decimal.TryParse((GridSystem.FindName("TemperatureKinStepValue") as TextBox)?.Text,
                NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out var temperatureKinStepValue);

            inputParameters.TemperatureKinStep = temperatureKinStepValue;

            #endregion Кинетическая температура (Tkin)

            #region Температура РИ (Tcmb)

            decimal.TryParse((GridSystem.FindName("TemperatureCmbMinValue") as TextBox)?.Text,
                NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out var temperatureCmbMinValue);

            inputParameters.TemperatureCmbMin = temperatureCmbMinValue;

            decimal.TryParse((GridSystem.FindName("TemperatureCmbMaxValue") as TextBox)?.Text,
                NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out var temperatureCmbMaxValue);

            inputParameters.TemperatureCmbMax = temperatureCmbMaxValue;

            decimal.TryParse((GridSystem.FindName("TemperatureCmbStepValue") as TextBox)?.Text,
                NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out var temperatureCmbStepValue);

            inputParameters.TemperatureCmbStep = temperatureCmbStepValue;

            #endregion Температура РИ (Tcmb)

            #region Лучевая концентрация (N0)

            decimal.TryParse((GridSystem.FindName("N0MinValue") as TextBox)?.Text, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var n0MinValue);

            inputParameters.N0Min = n0MinValue;

            decimal.TryParse((GridSystem.FindName("N0MaxValue") as TextBox)?.Text, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var n0MaxValue);

            inputParameters.N0Max = n0MaxValue;

            decimal.TryParse((GridSystem.FindName("N0StepValue") as TextBox)?.Text, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var n0StepValue);

            inputParameters.N0Step = n0StepValue;

            #endregion Лучевая концентрация (N0)

            #region Количество уровней и имя файла

            int.TryParse((GridSystem.FindName("NLevelsValue") as IntegerUpDown)?.Text, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var nLevelsValue);

            inputParameters.NumberOfLevels = nLevelsValue;

            inputParameters.ExportName = (GridSystem.FindName("FileNameValue") as TextBox)?.Text ?? string.Empty;

            #endregion Количество уровней и имя файла

            return inputParameters;
        }
    }
}