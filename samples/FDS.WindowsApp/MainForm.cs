using FDS.Hydraulics.Models;

namespace FDS.WindowsApp;

public sealed class MainForm : Form
{
    private readonly TextBox outputTextBox;
    private readonly NumericUpDown pressureDifferenceInput;
    private readonly NumericUpDown pipeDiameterInput;
    private readonly NumericUpDown branchAZetaInput;
    private readonly NumericUpDown branchBZetaInput;
    private readonly NumericUpDown totalFlowInput;
    private readonly Label statusLabel;

    public MainForm()
    {
        Text = "Fluid Dynamics Simulator - Windows-App-Test";
        MinimumSize = new Size(920, 620);
        StartPosition = FormStartPosition.CenterScreen;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(16),
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var headerLabel = new Label
        {
            AutoSize = true,
            Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
            Text = "Hydraulischer Solver-Referenztest",
        };

        var parameterGrid = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 4,
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 12, 0, 0),
        };
        parameterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        parameterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        parameterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        parameterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));

        pressureDifferenceInput = CreateNumericInput(0, 1_000_000, 100, decimalPlaces: 2);
        pipeDiameterInput = CreateNumericInput(0.001M, 10, 0.01M, decimalPlaces: 4);
        branchAZetaInput = CreateNumericInput(0, 10_000, 0.1M, decimalPlaces: 3);
        branchBZetaInput = CreateNumericInput(0, 10_000, 0.1M, decimalPlaces: 3);
        totalFlowInput = CreateNumericInput(0, 100, 0.001M, decimalPlaces: 8);

        AddInputRow(parameterGrid, 0, "Druckdifferenz Pa", pressureDifferenceInput, "Rohrdurchmesser m", pipeDiameterInput);
        AddInputRow(parameterGrid, 1, "Zeta Strang A", branchAZetaInput, "Zeta Strang B", branchBZetaInput);
        AddInputRow(parameterGrid, 2, "Gesamtvolumenstrom m3/s", totalFlowInput, string.Empty, null);

        var commandPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 12, 0, 12),
        };

        var runButton = new Button
        {
            AutoSize = true,
            Text = "Solver-Test starten",
        };
        runButton.Click += (_, _) => RunScenario();

        var resetButton = new Button
        {
            AutoSize = true,
            Text = "Zurücksetzen",
        };
        resetButton.Click += (_, _) =>
        {
            SetInputValues(SolverScenarioParameters.Default);
            RunScenario();
        };

        statusLabel = new Label
        {
            AutoSize = true,
            Padding = new Padding(12, 7, 0, 0),
            Text = "Bereit",
        };

        commandPanel.Controls.Add(runButton);
        commandPanel.Controls.Add(resetButton);
        commandPanel.Controls.Add(statusLabel);

        outputTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font(FontFamily.GenericMonospace, 10),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
        };

        root.Controls.Add(headerLabel, 0, 0);
        root.Controls.Add(parameterGrid, 0, 1);
        root.Controls.Add(commandPanel, 0, 2);
        root.Controls.Add(outputTextBox, 0, 3);

        Controls.Add(root);
        SetInputValues(SolverScenarioParameters.Default);
        Load += (_, _) => RunScenario();
    }

    private void RunScenario()
    {
        try
        {
            SolverScenarioParameters parameters = ReadInputValues();
            HydraulicSolverResult result = SolverScenarioRunner.RunParallelBranchScenario(parameters);
            statusLabel.Text = $"Status: {SolverScenarioRunner.FormatStatus(result.Status)}";
            outputTextBox.Text = SolverScenarioRunner.FormatResult(result, parameters);
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Status: Fehler";
            outputTextBox.Text = $"Fehler beim Ausführen des Solver-Tests:{Environment.NewLine}{ex}";
        }
    }

    private SolverScenarioParameters ReadInputValues()
    {
        return new SolverScenarioParameters
        {
            PressureDifferencePascals = (double)pressureDifferenceInput.Value,
            PipeInnerDiameterMeters = (double)pipeDiameterInput.Value,
            BranchAZeta = (double)branchAZetaInput.Value,
            BranchBZeta = (double)branchBZetaInput.Value,
            TotalVolumeFlowRateCubicMetersPerSecond = (double)totalFlowInput.Value,
        };
    }

    private void SetInputValues(SolverScenarioParameters parameters)
    {
        pressureDifferenceInput.Value = ToDecimal(parameters.PressureDifferencePascals);
        pipeDiameterInput.Value = ToDecimal(parameters.PipeInnerDiameterMeters);
        branchAZetaInput.Value = ToDecimal(parameters.BranchAZeta);
        branchBZetaInput.Value = ToDecimal(parameters.BranchBZeta);
        totalFlowInput.Value = ToDecimal(parameters.TotalVolumeFlowRateCubicMetersPerSecond);
    }

    private static NumericUpDown CreateNumericInput(decimal minimum, decimal maximum, decimal increment, int decimalPlaces)
    {
        return new NumericUpDown
        {
            DecimalPlaces = decimalPlaces,
            Dock = DockStyle.Fill,
            Increment = increment,
            Maximum = maximum,
            Minimum = minimum,
            TextAlign = HorizontalAlignment.Right,
            ThousandsSeparator = false,
            Width = 150,
        };
    }

    private static void AddInputRow(
        TableLayoutPanel grid,
        int rowIndex,
        string leftLabelText,
        Control leftControl,
        string rightLabelText,
        Control? rightControl)
    {
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        grid.Controls.Add(CreateInputLabel(leftLabelText), 0, rowIndex);
        grid.Controls.Add(leftControl, 1, rowIndex);

        if (rightControl is not null)
        {
            grid.Controls.Add(CreateInputLabel(rightLabelText), 2, rowIndex);
            grid.Controls.Add(rightControl, 3, rowIndex);
        }
    }

    private static Label CreateInputLabel(string text)
    {
        return new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 4, 12, 4),
            Text = text,
            TextAlign = ContentAlignment.MiddleLeft,
        };
    }

    private static decimal ToDecimal(double value)
    {
        return Convert.ToDecimal(value);
    }
}
