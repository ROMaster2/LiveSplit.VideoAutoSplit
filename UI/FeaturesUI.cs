using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.VASL;

using System.Text.RegularExpressions;

namespace LiveSplit.VAS.UI
{
    public partial class FeaturesUI : AbstractUI
    {
        public FeaturesUI(VASComponent component) : base(component)
        {
            InitializeComponent();
        }

        override public void Rerender()
        {
            Component.Script.ScriptUpdateFinished += UpdateRows;
        }

        override public void Derender()
        {
            Component.Script.ScriptUpdateFinished -= UpdateRows;
        }

        override internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            ClearRows();
            if (scriptLoaded)
            {
                var features = Component.GameProfile.WatchImages;

                if (features.Count > 0)
                {
                    tlpFeatures.RowCount = 1 + features.Count;
                    for (int i = 1; i < tlpFeatures.RowCount - 1; i++)
                    {
                        var feature = features[i];
                        var featureRow = new FeatureRow(feature.FullName, 60); // Todo: Add getting framerate.
                        tlpFeatures.RowStyles.Add(new RowStyle(SizeType.Absolute, 21F));
                        tlpFeatures.Controls.Add(featureRow, 0, i);
                        // Todo: Add hook for manual enable/disable.
                    }
                }

                // Get the defined vars from the raw script.
                // The vars aren't added to the ExpandoObject until they're invoked. This gets around that.
                // There's probably a better solution but idk what it could be atm.
                var varsResults = Regex.Matches(Component.GameProfile.RawScript, @"vars\.[a-z][0-9a-z]*", RegexOptions.IgnoreCase);

                if (varsResults.Count > 0)
                {
                    string[] variables = new string[varsResults.Count];
                    for (int i = 0; i < varsResults.Count; i++)
                    {
                        variables[i] = varsResults[i].Value.Substring(5);
                    }

                    // What does this and the VASL script do with case-sensitivity?
                    variables = variables.Distinct().ToArray();

                    tlpVariables.RowCount = 1 + variables.Length;
                    for (int i = 1; i < tlpVariables.RowCount; i++)
                    {
                        var variable = variables[i];
                        var variableRow = new VariableRow(variable, 60); // Todo: Add getting framerate.
                        tlpVariables.RowStyles.Add(new RowStyle(SizeType.Absolute, 21F));
                        tlpVariables.Controls.Add(variableRow, 0, i);
                    }
                }

            }
        }

        private void ClearRows()
        {
            for (int i = tlpFeatures.RowCount - 1; i > 0; i--)
            {
                if (tlpFeatures.Controls.ContainsKey(i.ToString()))
                {
                    tlpFeatures.Controls[i].Dispose();
                }
                tlpFeatures.RowStyles.RemoveAt(i);
            }
            tlpFeatures.RowCount = 1;

            for (int i = tlpVariables.RowCount - 1; i > 0; i--)
            {
                if (tlpVariables.Controls.ContainsKey(i.ToString()))
                {
                    tlpVariables.Controls[i].Dispose();
                }
                tlpVariables.RowStyles.RemoveAt(i);
            }
            tlpVariables.RowCount = 1;
        }

        private void UpdateRows(object sender, DeltaManager dm)
        {
            // It might be a bad idea to assume it's in the correct order,
            // but this might be a bit CPU intensive, and we're trying to
            // save as much of that as possible.
            // Then again, why not make a Dictionary? Later, probably.

            tlpCore.Invoke((MethodInvoker)delegate
            {
                for (int i = 1; i < tlpFeatures.RowCount - 1; i++)
                {
                    var value = dm[i - 1].current;
                    var varState = true;
                    var featureRow = (FeatureRow)tlpFeatures.Controls[i];
                    featureRow.Invoke((MethodInvoker)delegate { featureRow.Update(value, varState); });
                }

                var vars = (IDictionary<string, object>)Component.Script.Vars;
                for (int i = 1; i < tlpVariables.RowCount - 1; i++)
                {
                    var variableRow = (VariableRow)tlpVariables.Controls[i];
                    var value = vars[variableRow.Name]; // Probably not a good idea...
                    variableRow.Invoke((MethodInvoker)delegate { variableRow.Update(value); });
                }
            });
        }

    }
    
    internal class FeatureRow : VariableRow
    {
        public CheckBox ckbEnabled { get; private set; }

        public FeatureRow(string name, int stackSize) : base(name, stackSize)
        {
            ColumnCount = 5;
            ColumnStyles.Insert(1, new ColumnStyle(SizeType.Absolute, 44F));

            ckbEnabled = new CheckBox()
            {
                CheckState = CheckState.Indeterminate,
                Padding = new Padding(2, 0, 0, 0)
            };
            ckbEnabled.Click += ckbEnabled_Click;

            Controls.Add(ckbEnabled, 1, 0);
        }

        public void Update(double value, bool varEnabled)
        {
            base.Update(value);
            ckbEnabled.Checked = varEnabled;
        }

        // How do you hide base methods?
        public override void Update(dynamic value)
        {
            throw new MethodAccessException("Use Update(value, varState) instead.");
        }

        internal void ckbEnabled_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("Todo lol");
        }
    }

    internal class VariableRow : TableLayoutPanel
    {
        public Label    lblName     { get; }
        public Label    lblValue    { get; private set; }
        public Label    lblMaximum  { get; private set; }
        public Label    lblMinimum  { get; private set; }

        private Stack<double> ValueStack;

        private static readonly Label lblNumeric = new Label()
        {
            Anchor = AnchorStyles.Right,
            AutoSize = true,
            Text = "null",
            TextAlign = ContentAlignment.MiddleRight
        };

        public VariableRow(string name, int stackSize)
        {
            ColumnCount = 4;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));
            RowCount = 1;
            RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Dock = DockStyle.Fill;
            Margin = new Padding(0);
            Name = name; // Might break

            lblName = new Label()
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Text = name,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblValue   = lblNumeric.Clone();
            lblMaximum = lblNumeric.Clone();
            lblMinimum = lblNumeric.Clone();

            ValueStack = new Stack<double>(stackSize);

            Controls.Add(lblName,     0, 0);
            Controls.Add(lblValue,    1, 0);
            Controls.Add(lblMaximum,  2, 0);
            Controls.Add(lblMinimum,  3, 0);
        }

        public virtual void Update(dynamic value)
        {
            lblValue.Text = value.ToString(); // Todo: Formatting based on ErrorMetric.

            if (value is double || value is int || value is short || value is byte || value is long || value is float)
            {
                ValueStack.Push(value);
                //ValueStack.Pop(); // Is this fine if the size is pre-defined?

                lblMaximum.Text = ValueStack.Max().ToString();
                lblMinimum.Text = ValueStack.Min().ToString();
            }
        }
    }
}
