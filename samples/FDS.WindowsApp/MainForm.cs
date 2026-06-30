using FDS.Hydraulics.Models;

namespace FDS.WindowsApp;

public sealed class MainForm : Form
{
    private readonly UnitDisplaySettings unitSettings = UnitDisplaySettings.Default;
    private readonly ToolStripMenuItem pressurePascalMenuItem = CreateUnitMenuItem("Pa");
    private readonly ToolStripMenuItem pressureKilopascalMenuItem = CreateUnitMenuItem("kPa");
    private readonly ToolStripMenuItem pressureBarMenuItem = CreateUnitMenuItem("bar");
    private readonly ToolStripMenuItem lengthMeterMenuItem = CreateUnitMenuItem("m");
    private readonly ToolStripMenuItem lengthMillimeterMenuItem = CreateUnitMenuItem("mm");
    private readonly ToolStripMenuItem volumeFlowCubicMetersPerSecondMenuItem = CreateUnitMenuItem("m³/s");
    private readonly ToolStripMenuItem volumeFlowLitersPerSecondMenuItem = CreateUnitMenuItem("l/s");
    private readonly ToolStripMenuItem volumeFlowCubicMetersPerHourMenuItem = CreateUnitMenuItem("m³/h");
    private readonly ComboBox presetComboBox;
    private readonly Label presetDescriptionLabel;
    private readonly Label pressureDifferenceLabel;
    private readonly Label pipeDiameterLabel;
    private readonly Label totalFlowLabel;
    private readonly NumericUpDown pressureDifferenceInput;
    private readonly NumericUpDown pipeDiameterInput;
    private readonly NumericUpDown branchAZetaInput;
    private readonly NumericUpDown branchBZetaInput;
    private readonly NumericUpDown totalFlowInput;
    private readonly Label statusLabel;
    private readonly Label statusValueLabel;
    private readonly Label iterationsValueLabel;
    private readonly Label nodeResidualValueLabel;
    private readonly Label pressureResidualValueLabel;
    private readonly Label assessmentValueLabel;
    private readonly TextBox inputSummaryTextBox;
    private readonly TextBox reviewSummaryTextBox;
    private readonly DataGridView branchFlowGrid;
    private readonly DataGridView pressureResidualGrid;
    private readonly DataGridView iterationGrid;
    private readonly DataGridView presetComparisonGrid;
    private readonly TextBox outputTextBox;

    public MainForm()
    {
        Text = "Fluid Dynamics Simulator - Windows-App-Test";
        MinimumSize = new Size(1080, 720);
        StartPosition = FormStartPosition.CenterScreen;

        ConnectUnitMenuItems();
        MenuStrip menuStrip = CreateMenuStrip();
        MainMenuStrip = menuStrip;

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(16),
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var headerLabel = new Label
        {
            AutoSize = true,
            Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
            Text = "Hydraulischer Solver-Referenztest",
        };

        var presetPanel = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 3,
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 12, 0, 0),
        };
        presetPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        presetPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        presetPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        presetComboBox = new ComboBox
        {
            DisplayMember = nameof(SolverScenarioPreset.Name),
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };
        foreach (SolverScenarioPreset preset in SolverScenarioPreset.All)
        {
            presetComboBox.Items.Add(preset);
        }

        presetDescriptionLabel = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Padding = new Padding(12, 4, 0, 4),
            TextAlign = ContentAlignment.MiddleLeft,
        };

        presetComboBox.SelectedIndexChanged += (_, _) => ApplySelectedPreset(runScenario: true);
        presetPanel.Controls.Add(CreateInputLabel("Szenario-Preset"), 0, 0);
        presetPanel.Controls.Add(presetComboBox, 1, 0);
        presetPanel.Controls.Add(presetDescriptionLabel, 2, 0);

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

        pressureDifferenceInput = CreateNumericInput();
        pipeDiameterInput = CreateNumericInput();
        branchAZetaInput = CreateNumericInput();
        branchBZetaInput = CreateNumericInput();
        totalFlowInput = CreateNumericInput();

        pressureDifferenceLabel = CreateInputLabel(string.Empty);
        pipeDiameterLabel = CreateInputLabel(string.Empty);
        totalFlowLabel = CreateInputLabel(string.Empty);

        ConfigureUnitInputs();
        UpdateUnitLabels();
        UpdateUnitMenuChecks();
        ConfigureNumericInput(branchAZetaInput, 0, 10_000, 0.1M, decimalPlaces: 3);
        ConfigureNumericInput(branchBZetaInput, 0, 10_000, 0.1M, decimalPlaces: 3);

        AddInputRow(parameterGrid, 0, pressureDifferenceLabel, pressureDifferenceInput, pipeDiameterLabel, pipeDiameterInput);
        AddInputRow(parameterGrid, 1, CreateInputLabel("Zeta Strang A"), branchAZetaInput, CreateInputLabel("Zeta Strang B"), branchBZetaInput);
        AddInputRow(parameterGrid, 2, totalFlowLabel, totalFlowInput, null, null);

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
            presetComboBox.SelectedIndex = 0;
            SetInputValues(SolverScenarioParameters.Default);
            RunScenario();
        };

        var compareButton = new Button
        {
            AutoSize = true,
            Text = "Presets vergleichen",
        };
        compareButton.Click += (_, _) => RunPresetComparison();

        statusLabel = new Label
        {
            AutoSize = true,
            Padding = new Padding(12, 7, 0, 0),
            Text = "Bereit",
        };

        commandPanel.Controls.Add(runButton);
        commandPanel.Controls.Add(resetButton);
        commandPanel.Controls.Add(compareButton);
        commandPanel.Controls.Add(statusLabel);

        var topPanel = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
        };
        topPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        topPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        topPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        topPanel.Controls.Add(presetPanel, 0, 0);
        topPanel.Controls.Add(parameterGrid, 0, 1);
        topPanel.Controls.Add(commandPanel, 0, 2);

        statusValueLabel = CreateValueLabel();
        iterationsValueLabel = CreateValueLabel();
        nodeResidualValueLabel = CreateValueLabel();
        pressureResidualValueLabel = CreateValueLabel();
        assessmentValueLabel = CreateValueLabel();
        inputSummaryTextBox = CreateReadOnlyTextBox(wordWrap: false);
        inputSummaryTextBox.Height = 130;
        reviewSummaryTextBox = CreateReadOnlyTextBox(wordWrap: true);
        reviewSummaryTextBox.Height = 150;

        branchFlowGrid = CreateGrid(
            ("Strang", nameof(BranchFlowReportRow.BranchName), 160),
            ("Volumenstrom", nameof(BranchFlowReportRow.VolumeFlowRateText), 180));
        pressureResidualGrid = CreateGrid(
            ("Element", nameof(PressureResidualReportRow.ElementName), 160),
            ("Residuum", nameof(PressureResidualReportRow.ResidualText), 160),
            ("Verfügbar", nameof(PressureResidualReportRow.AvailablePressureText), 160),
            ("Erforderlich", nameof(PressureResidualReportRow.RequiredPressureText), 160));
        iterationGrid = CreateGrid(
            ("Iteration", nameof(IterationReportRow.IterationNumberText), 110),
            ("Knotenbilanz-Residuum", nameof(IterationReportRow.NodeResidualText), 220),
            ("Druck-Residuum", nameof(IterationReportRow.PressureResidualText), 180));
        presetComparisonGrid = CreateGrid(
            ("Szenario", nameof(PresetComparisonReportRow.ScenarioName), 170),
            ("Status", nameof(PresetComparisonReportRow.StatusText), 190),
            ("Iterationen", nameof(PresetComparisonReportRow.IterationsText), 110),
            ("Knotenbilanz", nameof(PresetComparisonReportRow.NodeResidualText), 160),
            ("Druck", nameof(PresetComparisonReportRow.PressureResidualText), 140),
            ("Strang A", nameof(PresetComparisonReportRow.BranchAFlowText), 140),
            ("Strang B", nameof(PresetComparisonReportRow.BranchBFlowText), 140),
            ("Bewertung", nameof(PresetComparisonReportRow.AssessmentText), 210));
        outputTextBox = CreateReadOnlyTextBox(wordWrap: false);

        var resultTabs = new TabControl
        {
            Dock = DockStyle.Fill,
        };
        resultTabs.TabPages.Add(CreateOverviewTab());
        resultTabs.TabPages.Add(CreateGridTab("Stränge", branchFlowGrid));
        resultTabs.TabPages.Add(CreateGridTab("Druckresiduen", pressureResidualGrid));
        resultTabs.TabPages.Add(CreateGridTab("Iterationen", iterationGrid));
        resultTabs.TabPages.Add(CreateGridTab("Preset-Vergleich", presetComparisonGrid));
        resultTabs.TabPages.Add(CreateTextTab("Textausgabe", outputTextBox));

        root.Controls.Add(headerLabel, 0, 0);
        root.Controls.Add(topPanel, 0, 1);
        root.Controls.Add(resultTabs, 0, 2);

        mainLayout.Controls.Add(menuStrip, 0, 0);
        mainLayout.Controls.Add(root, 0, 1);
        Controls.Add(mainLayout);

        presetComboBox.SelectedIndex = 0;
        SetInputValues(SolverScenarioParameters.Default);
        Load += (_, _) =>
        {
            RunScenario();
            RunPresetComparison();
        };
    }

    private void RunScenario()
    {
        try
        {
            SolverScenarioParameters parameters = ReadInputValues();
            HydraulicSolverResult result = SolverScenarioRunner.RunParallelBranchScenario(parameters);
            SolverScenarioReport report = SolverScenarioRunner.CreateReport(result, parameters, unitSettings);

            statusLabel.Text = $"Status: {report.StatusText}";
            statusValueLabel.Text = report.StatusText;
            iterationsValueLabel.Text = report.IterationsText;
            nodeResidualValueLabel.Text = report.NodeResidualText;
            pressureResidualValueLabel.Text = report.PressureResidualText;
            assessmentValueLabel.Text = report.AssessmentText;
            inputSummaryTextBox.Text = report.InputSummaryText;
            reviewSummaryTextBox.Text = report.ReviewSummaryText;
            branchFlowGrid.DataSource = report.BranchFlows.ToList();
            pressureResidualGrid.DataSource = report.PressureResiduals.ToList();
            iterationGrid.DataSource = report.Iterations.ToList();
            outputTextBox.Text = SolverScenarioRunner.FormatResult(result, parameters, unitSettings);
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Status: Fehler";
            statusValueLabel.Text = "Fehler";
            iterationsValueLabel.Text = "-";
            nodeResidualValueLabel.Text = "-";
            pressureResidualValueLabel.Text = "-";
            assessmentValueLabel.Text = "-";
            inputSummaryTextBox.Clear();
            reviewSummaryTextBox.Clear();
            branchFlowGrid.DataSource = null;
            pressureResidualGrid.DataSource = null;
            iterationGrid.DataSource = null;
            outputTextBox.Text = $"Fehler beim Ausführen des Solver-Tests:{Environment.NewLine}{ex}";
        }
    }

    private void RunPresetComparison()
    {
        try
        {
            presetComparisonGrid.DataSource = SolverScenarioRunner.CreatePresetComparison(unitSettings).ToList();
        }
        catch (Exception ex)
        {
            presetComparisonGrid.DataSource = null;
            outputTextBox.Text = $"Fehler beim Erstellen des Preset-Vergleichs:{Environment.NewLine}{ex}";
        }
    }

    private void ApplySelectedPreset(bool runScenario)
    {
        if (presetComboBox.SelectedItem is not SolverScenarioPreset preset)
        {
            return;
        }

        presetDescriptionLabel.Text = preset.Description;
        SetInputValues(preset.Parameters);

        if (runScenario)
        {
            RunScenario();
        }
    }

    private SolverScenarioParameters ReadInputValues()
    {
        return new SolverScenarioParameters
        {
            PressureDifferencePascals = unitSettings.ToPascals((double)pressureDifferenceInput.Value),
            PipeInnerDiameterMeters = unitSettings.ToMeters((double)pipeDiameterInput.Value),
            BranchAZeta = (double)branchAZetaInput.Value,
            BranchBZeta = (double)branchBZetaInput.Value,
            TotalVolumeFlowRateCubicMetersPerSecond = unitSettings.ToCubicMetersPerSecond((double)totalFlowInput.Value),
        };
    }

    private void SetInputValues(SolverScenarioParameters parameters)
    {
        pressureDifferenceInput.Value = ToDecimal(unitSettings.ToDisplayPressure(parameters.PressureDifferencePascals));
        pipeDiameterInput.Value = ToDecimal(unitSettings.ToDisplayLength(parameters.PipeInnerDiameterMeters));
        branchAZetaInput.Value = ToDecimal(parameters.BranchAZeta);
        branchBZetaInput.Value = ToDecimal(parameters.BranchBZeta);
        totalFlowInput.Value = ToDecimal(unitSettings.ToDisplayVolumeFlow(parameters.TotalVolumeFlowRateCubicMetersPerSecond));
    }

    private MenuStrip CreateMenuStrip()
    {
        var menuStrip = new MenuStrip
        {
            Dock = DockStyle.Fill,
        };

        var settingsMenu = new ToolStripMenuItem("Einstellungen");
        var unitsMenu = new ToolStripMenuItem("Einheiten");

        var pressureMenu = new ToolStripMenuItem("Druck");
        pressureMenu.DropDownItems.AddRange(
            [
                pressurePascalMenuItem,
                pressureKilopascalMenuItem,
                pressureBarMenuItem,
            ]);

        var lengthMenu = new ToolStripMenuItem("Länge / Durchmesser");
        lengthMenu.DropDownItems.AddRange(
            [
                lengthMeterMenuItem,
                lengthMillimeterMenuItem,
            ]);

        var volumeFlowMenu = new ToolStripMenuItem("Volumenstrom");
        volumeFlowMenu.DropDownItems.AddRange(
            [
                volumeFlowCubicMetersPerSecondMenuItem,
                volumeFlowLitersPerSecondMenuItem,
                volumeFlowCubicMetersPerHourMenuItem,
            ]);

        unitsMenu.DropDownItems.AddRange(
            [
                pressureMenu,
                lengthMenu,
                volumeFlowMenu,
            ]);
        settingsMenu.DropDownItems.Add(unitsMenu);
        menuStrip.Items.Add(settingsMenu);

        return menuStrip;
    }

    private void ConnectUnitMenuItems()
    {
        pressurePascalMenuItem.Click += (_, _) => ChangePressureUnit(PressureDisplayUnit.Pascal);
        pressureKilopascalMenuItem.Click += (_, _) => ChangePressureUnit(PressureDisplayUnit.Kilopascal);
        pressureBarMenuItem.Click += (_, _) => ChangePressureUnit(PressureDisplayUnit.Bar);
        lengthMeterMenuItem.Click += (_, _) => ChangeLengthUnit(LengthDisplayUnit.Meter);
        lengthMillimeterMenuItem.Click += (_, _) => ChangeLengthUnit(LengthDisplayUnit.Millimeter);
        volumeFlowCubicMetersPerSecondMenuItem.Click += (_, _) => ChangeVolumeFlowUnit(VolumeFlowDisplayUnit.CubicMetersPerSecond);
        volumeFlowLitersPerSecondMenuItem.Click += (_, _) => ChangeVolumeFlowUnit(VolumeFlowDisplayUnit.LitersPerSecond);
        volumeFlowCubicMetersPerHourMenuItem.Click += (_, _) => ChangeVolumeFlowUnit(VolumeFlowDisplayUnit.CubicMetersPerHour);
    }

    private void ChangePressureUnit(PressureDisplayUnit pressureUnit)
    {
        if (unitSettings.PressureUnit == pressureUnit)
        {
            return;
        }

        SolverScenarioParameters currentParameters = ReadInputValues();
        unitSettings.PressureUnit = pressureUnit;
        ApplyUnitSettings(currentParameters);
    }

    private void ChangeLengthUnit(LengthDisplayUnit lengthUnit)
    {
        if (unitSettings.LengthUnit == lengthUnit)
        {
            return;
        }

        SolverScenarioParameters currentParameters = ReadInputValues();
        unitSettings.LengthUnit = lengthUnit;
        ApplyUnitSettings(currentParameters);
    }

    private void ChangeVolumeFlowUnit(VolumeFlowDisplayUnit volumeFlowUnit)
    {
        if (unitSettings.VolumeFlowUnit == volumeFlowUnit)
        {
            return;
        }

        SolverScenarioParameters currentParameters = ReadInputValues();
        unitSettings.VolumeFlowUnit = volumeFlowUnit;
        ApplyUnitSettings(currentParameters);
    }

    private void ApplyUnitSettings(SolverScenarioParameters currentParameters)
    {
        ConfigureUnitInputs();
        UpdateUnitLabels();
        UpdateUnitMenuChecks();
        SetInputValues(currentParameters);
        RunScenario();
        RunPresetComparison();
    }

    private void ConfigureUnitInputs()
    {
        ConfigureNumericInput(
            pressureDifferenceInput,
            0,
            ToDecimal(unitSettings.ToDisplayPressure(1_000_000)),
            unitSettings.PressureIncrement,
            unitSettings.PressureDecimalPlaces);
        ConfigureNumericInput(
            pipeDiameterInput,
            ToDecimal(unitSettings.ToDisplayLength(0.001)),
            ToDecimal(unitSettings.ToDisplayLength(10)),
            unitSettings.LengthIncrement,
            unitSettings.LengthDecimalPlaces);
        ConfigureNumericInput(
            totalFlowInput,
            0,
            ToDecimal(unitSettings.ToDisplayVolumeFlow(100)),
            unitSettings.VolumeFlowIncrement,
            unitSettings.VolumeFlowDecimalPlaces);
    }

    private void UpdateUnitLabels()
    {
        pressureDifferenceLabel.Text = $"Druckdifferenz [{unitSettings.PressureUnitText}]";
        pipeDiameterLabel.Text = $"Rohrdurchmesser [{unitSettings.LengthUnitText}]";
        totalFlowLabel.Text = $"Gesamtvolumenstrom [{unitSettings.VolumeFlowUnitText}]";
    }

    private void UpdateUnitMenuChecks()
    {
        pressurePascalMenuItem.Checked = unitSettings.PressureUnit == PressureDisplayUnit.Pascal;
        pressureKilopascalMenuItem.Checked = unitSettings.PressureUnit == PressureDisplayUnit.Kilopascal;
        pressureBarMenuItem.Checked = unitSettings.PressureUnit == PressureDisplayUnit.Bar;
        lengthMeterMenuItem.Checked = unitSettings.LengthUnit == LengthDisplayUnit.Meter;
        lengthMillimeterMenuItem.Checked = unitSettings.LengthUnit == LengthDisplayUnit.Millimeter;
        volumeFlowCubicMetersPerSecondMenuItem.Checked = unitSettings.VolumeFlowUnit == VolumeFlowDisplayUnit.CubicMetersPerSecond;
        volumeFlowLitersPerSecondMenuItem.Checked = unitSettings.VolumeFlowUnit == VolumeFlowDisplayUnit.LitersPerSecond;
        volumeFlowCubicMetersPerHourMenuItem.Checked = unitSettings.VolumeFlowUnit == VolumeFlowDisplayUnit.CubicMetersPerHour;
    }

    private TabPage CreateOverviewTab()
    {
        var page = new TabPage("Übersicht");
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(12),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddSummaryRow(panel, 0, "Status", statusValueLabel);
        AddSummaryRow(panel, 1, "Iterationen", iterationsValueLabel);
        AddSummaryRow(panel, 2, "Knotenbilanz-Residuum", nodeResidualValueLabel);
        AddSummaryRow(panel, 3, "Druck-Residuum", pressureResidualValueLabel);
        AddSummaryRow(panel, 4, "Bewertung", assessmentValueLabel);
        AddSummaryRow(panel, 5, "Eingaben", inputSummaryTextBox);
        AddSummaryRow(panel, 6, "Prüfhinweise", reviewSummaryTextBox);

        page.Controls.Add(panel);
        return page;
    }

    private static TabPage CreateGridTab(string title, DataGridView grid)
    {
        var page = new TabPage(title);
        page.Controls.Add(grid);
        return page;
    }

    private static TabPage CreateTextTab(string title, TextBox textBox)
    {
        var page = new TabPage(title);
        page.Controls.Add(textBox);
        return page;
    }

    private static void AddSummaryRow(TableLayoutPanel panel, int rowIndex, string labelText, Control valueControl)
    {
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(CreateInputLabel(labelText), 0, rowIndex);
        panel.Controls.Add(valueControl, 1, rowIndex);
    }

    private static NumericUpDown CreateNumericInput()
    {
        return new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Maximum = 1_000_000_000,
            Minimum = 0,
            TextAlign = HorizontalAlignment.Right,
            ThousandsSeparator = false,
            Width = 150,
        };
    }

    private static void ConfigureNumericInput(
        NumericUpDown input,
        decimal minimum,
        decimal maximum,
        decimal increment,
        int decimalPlaces)
    {
        if (input.Value > maximum)
        {
            input.Value = maximum;
        }

        if (input.Value < minimum)
        {
            input.Value = minimum;
        }

        input.DecimalPlaces = decimalPlaces;
        input.Minimum = minimum;
        input.Maximum = maximum;
        input.Increment = increment;
    }

    private static DataGridView CreateGrid(params (string Header, string PropertyName, int Width)[] columns)
    {
        var grid = new DataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.FixedSingle,
            Dock = DockStyle.Fill,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        };

        foreach ((string header, string propertyName, int width) in columns)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                HeaderText = header,
                Width = width,
            });
        }

        return grid;
    }

    private static Label CreateValueLabel()
    {
        return new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
            Padding = new Padding(0, 4, 0, 4),
            Text = "-",
            TextAlign = ContentAlignment.MiddleLeft,
        };
    }

    private static TextBox CreateReadOnlyTextBox(bool wordWrap)
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font(FontFamily.GenericMonospace, 10),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = wordWrap,
        };
    }

    private static void AddInputRow(
        TableLayoutPanel grid,
        int rowIndex,
        Control leftLabel,
        Control leftControl,
        Control? rightLabel,
        Control? rightControl)
    {
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        grid.Controls.Add(leftLabel, 0, rowIndex);
        grid.Controls.Add(leftControl, 1, rowIndex);

        if (rightLabel is not null && rightControl is not null)
        {
            grid.Controls.Add(rightLabel, 2, rowIndex);
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

    private static ToolStripMenuItem CreateUnitMenuItem(string text)
    {
        return new ToolStripMenuItem(text)
        {
            CheckOnClick = false,
        };
    }

    private static decimal ToDecimal(double value)
    {
        return Convert.ToDecimal(value);
    }
}
