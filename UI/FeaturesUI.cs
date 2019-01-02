using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveSplit.VAS.Models.Delta;
using LiveSplit.VAS.VASL;

using System.Text.RegularExpressions;

namespace LiveSplit.VAS.UI
{
    public partial class FeaturesUI : AbstractUI
    {
        private bool _Updating;
        private Queue<double>[] _ValueQueues;
        private ICollection<string> _VarsNames;

        public FeaturesUI(VASComponent component) : base(component)
        {
            InitializeComponent();
            _Updating = false;
        }

        override public void Rerender()
        {
            if (_Component.IsScriptLoaded())
            {
                SetRows(true);
                _Component.Script.ScriptUpdateFinished += UpdateRows;
            }
        }

        override public void Derender()
        {
            if (_Component.IsScriptLoaded())
                _Component.Script.ScriptUpdateFinished -= UpdateRows;
        }

        override internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            // sigh
            try
            {
                SetRows(scriptLoaded);
            }
            catch (Exception e1)
            {
                try
                {
                    tlpFeatures.Invoke((MethodInvoker)delegate
                    {
                        SetRows(scriptLoaded);
                    });
                }
                catch (Exception e2)
                {
                    Log.Error(e1, "Could not load VASL Settings.");
                    Log.Error(e2, "");
                }
            }
        }

        // Because two rows exist for different purposes, the first for headers and last for layout balance,
        // indexing became terrible.
        // I didn't want it to be spaghetti but I am italian, so...
        internal void SetRows(bool scriptLoaded)
        {
            _Updating = true;
            ClearRows();
            var cf = _Component.Scanner.CompiledFeatures;
            if (scriptLoaded && cf != null)
            {
                // Get the defined vars from the raw script.
                // The vars aren't added to the ExpandoObject until they're invoked. This gets around that.
                // There's probably a better solution but idk what it could be atm.
                var varsResults = Regex.Matches(_Component.GameProfile.RawScript, @"vars\.[a-z][0-9a-z]*", RegexOptions.IgnoreCase);

                _ValueQueues = new Queue<double>[cf.FeatureCount + varsResults.Count];
                _VarsNames = new List<string>();
                int f = 0;

                var count = cf.CWatchImages.Count();

                if (count > 0)
                {
                    tlpFeatures.RowCount += count;
                    for (int i = 0; i < cf.CWatchZones.Length; i++)
                    {
                        var wz = cf.CWatchZones[i];

                        for (int n = 0; n < wz.CWatches.Length; n++)
                        {
                            var w = wz.CWatches[n];

                            for (int t = 0; t < w.CWatchImages.Length; t++)
                            {
                                var wi = w.CWatchImages[t];

                                var name = wz.Name + "/" + w.Name + " - " + wi.Name;

                                var valueQueue = new Queue<double>();
                                _ValueQueues[f] = valueQueue;
                                f++;

                                var featureRow = new VariableRow(name);

                                tlpFeatures.RowStyles.Insert(t, new RowStyle(SizeType.Absolute, 21F));
                                tlpFeatures.Controls.Add(featureRow, 0, wi.Index + 1);
                                // Todo: Add hook for manual enable/disable.
                            }
                        }
                    }
                }

                if (varsResults.Count > 0)
                {
                    string[] variables = new string[varsResults.Count];
                    for (int i = 0; i < varsResults.Count; i++)
                    {
                        variables[i] = varsResults[i].Value;
                    }

                    // What does this and the VASL script do with case-sensitivity?
                    variables = variables.Distinct().ToArray();

                    var offset = tlpFeatures.RowCount - 1;
                    tlpFeatures.RowCount += variables.Length;
                    for (int i = 0; i < variables.Length; i++)
                    {
                        var variable = variables[i];

                        var valueQueue = new Queue<double>();
                        _ValueQueues[f] = valueQueue;
                        f++;

                        _VarsNames.Add(variable.Substring(5));

                        var variableRow = new VariableRow(variable);

                        tlpFeatures.RowStyles.Insert(i + offset, new RowStyle(SizeType.Absolute, 21F));
                        tlpFeatures.Controls.Add(variableRow, 0, i + offset);
                    }
                }
            }
            _Updating = false;
        }

        private void ClearRows()
        {
            for (int i = tlpFeatures.Controls.Count - 1; i > 0; i--)
            {
                tlpFeatures.Controls[i].Dispose();
                tlpFeatures.RowStyles.RemoveAt(i);
            }
            tlpFeatures.RowCount = 2;
        }

        private void UpdateRows(object sender, DeltaOutput d)
        {
            Task.Run(() => UpdateRowsAsync(d, tlpFeatures.RowCount));
        }

        private void UpdateRowsAsync(DeltaOutput d, int rowCount)
        {
            // It might be a bad idea to assume it's in the correct order,
            // but this might be a bit CPU intensive, and we're trying to
            // save as much of that as possible.
            var deltas = (double[])d.History[d.FrameIndex].Deltas.Clone();
            var frameRate = (int)Math.Ceiling(d.FrameRate);

            var textBlocks = new TextBlock[rowCount - 2];

            var deltaCount = deltas.Length;
            for (int i = 1; i < deltaCount + 1; i++)
            {
                var value = deltas[i - 1];
                var varState = !double.IsNaN(value);

                textBlocks[i - 1] = GetLabels(value, i - 1, frameRate, varState);
            }

            var vars = (IDictionary<string, object>)_Component.Script.Vars;
            for (int i = deltaCount + 1; i < rowCount - 1; i++)
            {
                var varsKey = _VarsNames.ElementAt(i - deltaCount - 1);
                if (vars.ContainsKey(varsKey))
                {
                    var value = vars[varsKey];
                    textBlocks[i - 1] = GetLabels(value, i - 1, frameRate);
                }
            }

            if (!_Updating)
            {
                tlpFeatures.Invoke((MethodInvoker)delegate
                {
                    for (int i = 1; i < tlpFeatures.RowCount - 1; i++)
                    {
                        var variableRow = (VariableRow)tlpFeatures.Controls[i];
                        variableRow.Update(textBlocks[i - 1]);
                    }
                });
            }
            _Updating = false;
        }

        private TextBlock GetLabels(dynamic value, int index, int frameRate, bool? varEnabled = null)
        {
            double numValue = double.NaN;
            string stringFormat = null;
            TextBlock textBlock = TextBlock.Blank;

            if (IsNumeric(value))
            {
                numValue = (double)value;
                stringFormat = IsFloat(value) ? "F4" : null;
            }
            else if (value is bool)
            {
                numValue = value ? 1 : 0;
            }

            if (double.IsNaN(numValue))
            {
                textBlock.Value = value.ToString();
            }
            else
            {
                try
                {
                    var valueQueue = _ValueQueues[index];

                    valueQueue.Enqueue(numValue);
                    if (valueQueue.Count >= frameRate)
                    {
                        valueQueue.Dequeue();
                    }

                    // Todo: Formatting based on ErrorMetric.
                    textBlock.Value = numValue.ToString(stringFormat);
                    if (true)
                    {
                        textBlock.Maximum = valueQueue.Max().ToString(stringFormat);
                        textBlock.Minimum = valueQueue.Min().ToString(stringFormat);
                    }
                }
                catch
                {
                    //Log.Verbose("Label exception for FeatureUI, ignoring. " + e.Message);
                }
            }

            textBlock.Enabled = varEnabled;

            return textBlock;
        }

        private static bool IsNumeric(dynamic value)
        {
            return IsFloat(value)
                || value is int
                || value is long
                || value is short
                || value is byte
                || value is uint
                || value is ulong
                || value is ushort
                || value is sbyte;
        }

        private static bool IsFloat(dynamic value)
        {
            return value is double
                || value is decimal
                || value is float;
        }

        private class VariableRow : TableLayoutPanel
        {
            public Label lblName { get; }
            public CheckBox ckbEnabled { get; private set; }
            public Label lblValue { get; private set; }
            public Label lblMaximum { get; private set; }
            public Label lblMinimum { get; private set; }

            private static readonly Label lblNumeric = new Label()
            {
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                Text = "",
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };

            public VariableRow(string name)
            {
                ColumnCount = 5;
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));
                ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));
                RowCount = 1;
                RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                Dock = DockStyle.Fill;
                Margin = new Padding(0);
                Name = name;

                lblName = new Label()
                {
                    Anchor = AnchorStyles.Left,
                    AutoSize = true,
                    Text = name,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent
                };
                ckbEnabled = new CheckBox()
                {
                    CheckState = CheckState.Indeterminate,
                    Padding = new Padding(0, 2, 0, 0),
                    Enabled = false, // @TODO: Implement
                    Visible = false
                };
                ckbEnabled.Click += ckbEnabled_Click;
                lblValue = lblNumeric.Clone();
                lblMaximum = lblNumeric.Clone();
                lblMinimum = lblNumeric.Clone();

                Controls.Add(lblName, 0, 0);
                Controls.Add(ckbEnabled, 1, 0);
                Controls.Add(lblValue, 2, 0);
                Controls.Add(lblMaximum, 3, 0);
                Controls.Add(lblMinimum, 4, 0);
            }

            public void ckbEnabled_Click(object sender, EventArgs e)
            {
                throw new NotImplementedException("Todo lol");
            }

            public override string ToString()
            {
                return Name;
            }

            public void Update(TextBlock textBlock)
            {
                lblValue.Text = textBlock.Value;
                lblMaximum.Text = textBlock.Maximum;
                lblMinimum.Text = textBlock.Minimum;
                if (textBlock.Enabled.HasValue)
                    ckbEnabled.Checked = textBlock.Enabled.Value;
                else
                    ckbEnabled.CheckState = CheckState.Indeterminate;
            }
        }

        private struct TextBlock
        {
            public bool? Enabled;
            public string Value;
            public string Maximum;
            public string Minimum;

            private TextBlock(bool _)
            {
                Enabled = null;
                Value = "Unset";
                Maximum = string.Empty;
                Minimum = string.Empty;
            }

            public static readonly TextBlock Blank = new TextBlock(true);
        }
    }
}
