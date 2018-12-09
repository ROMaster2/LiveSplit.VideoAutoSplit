﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveSplit.VAS.Models.Delta;
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
            Component.Script.ScriptUpdateFinished += UpdateRowsAsync;
        }

        override public void Derender()
        {
            Component.Script.ScriptUpdateFinished -= UpdateRowsAsync;
        }

        // Because two rows exist for different purposes, the first for headers and last for layout balance,
        // indexing became terrible.
        // I didn't want it to be spaghetti but I am italian, so...
        override internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            ClearRows();
            if (scriptLoaded)
            {
                var features = Component.GameProfile.WatchImages;

                if (features.Count > 0)
                {
                    tlpFeatures.RowCount += features.Count;
                    for (int i = 1; i < tlpFeatures.RowCount - 1; i++)
                    {
                        var feature = features[i - 1];
                        var featureRow = new VariableRow(feature.FullName, 60); // Todo: Add getting framerate.
                        tlpFeatures.RowStyles.Insert(i, new RowStyle(SizeType.Absolute, 21F));
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
                        variables[i] = varsResults[i].Value;
                    }

                    // What does this and the VASL script do with case-sensitivity?
                    variables = variables.Distinct().ToArray();

                    var offset = tlpFeatures.RowCount - 1;
                    tlpFeatures.RowCount += variables.Length;
                    for (int i = 0; i < variables.Length; i++)
                    {
                        var variable = variables[i];
                        var variableRow = new VariableRow(variable, 60); // Todo: Add getting framerate.
                        tlpFeatures.RowStyles.Insert(i + offset, new RowStyle(SizeType.Absolute, 21F));
                        tlpFeatures.Controls.Add(variableRow, 0, i + offset);
                    }
                }

            }
        }

        private void ClearRows()
        {
            for (int i = tlpFeatures.RowCount - 2; i > 0; i--)
            {
                if (tlpFeatures.Controls.ContainsKey(i.ToString()))
                {
                    tlpFeatures.Controls[i].Dispose();
                }
                tlpFeatures.RowStyles.RemoveAt(i);
            }
            tlpFeatures.RowCount = 2;
        }

        private void UpdateRowsAsync(object sender, DeltaOutput d)
        {
            Task.Run(() => UpdateRows(sender, d));
        }

        private void UpdateRows(object sender, DeltaOutput d)
        {
            // It might be a bad idea to assume it's in the correct order,
            // but this might be a bit CPU intensive, and we're trying to
            // save as much of that as possible.
            // Then again, why not make a Dictionary? Later, probably.
            var deltas = (double[])d.History[d.FrameIndex].Deltas.Clone();

            this.Invoke((MethodInvoker)delegate
            {
                var deltaCount = deltas.Length;
                for (int i = 1; i < deltaCount + 1; i++)
                {
                    var value = deltas[i - 1];
                    var varState = !double.IsNaN(value);
                    var featureRow = (VariableRow)tlpFeatures.Controls[i];
                    featureRow.Invoke((MethodInvoker)delegate { featureRow.Update(value, varState); });
                }

                var vars = (IDictionary<string, object>)Component.Script.Vars;
                for (int i = deltaCount + 1; i < tlpFeatures.RowCount - 1; i++)
                {
                    var variableRow = (VariableRow)tlpFeatures.Controls[i];
                    var value = vars[variableRow.Name.Substring(5)];
                    variableRow.Invoke((MethodInvoker)delegate { variableRow.Update(value); });
                }
            });
        }

    }
    
    internal class VariableRow : TableLayoutPanel
    {
        public Label    lblName     { get; }
        public CheckBox ckbEnabled  { get; private set; }
        public Label    lblValue    { get; private set; }
        public Label    lblMaximum  { get; private set; }
        public Label    lblMinimum  { get; private set; }

        private Queue<double> ValueQueue;
        private int QueueSize;

        private static readonly Label lblNumeric = new Label()
        {
            Anchor = AnchorStyles.Right,
            AutoSize = true,
            Text = "",
            TextAlign = ContentAlignment.MiddleRight
        };

        public VariableRow(string name, int queueSize)
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
            Name = name; // Might break

            lblName = new Label()
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Text = name,
                TextAlign = ContentAlignment.MiddleLeft
            };
            ckbEnabled = new CheckBox()
            {
                CheckState = CheckState.Indeterminate,
                Padding = new Padding(0, 2, 0, 0)
            };
            ckbEnabled.Click += ckbEnabled_Click;
            lblValue   = lblNumeric.Clone();
            lblMaximum = lblNumeric.Clone();
            lblMinimum = lblNumeric.Clone();

            ValueQueue = new Queue<double>(queueSize);
            QueueSize = queueSize;

            Controls.Add(lblName,     0, 0);
            Controls.Add(ckbEnabled,  1, 0);
            Controls.Add(lblValue,    2, 0);
            Controls.Add(lblMaximum,  3, 0);
            Controls.Add(lblMinimum,  4, 0);
        }

        public virtual void Update(dynamic value, bool? varEnabled = null)
        {
            if (value is double || value is int || value is short || value is byte || value is long || value is float)
            {
                lblValue.Text = value.ToString("F4"); // Todo: Formatting based on ErrorMetric.

                ValueQueue.Enqueue(value);
                if (ValueQueue.Count >= QueueSize)
                {
                    ValueQueue.Dequeue();
                }

                lblMaximum.Text = ValueQueue.Max().ToString("F4");
                lblMinimum.Text = ValueQueue.Min().ToString("F4");
            }
            else
            {
                lblValue.Text = value.ToString();
            }

            if (varEnabled.HasValue)
            {
                ckbEnabled.Checked = varEnabled.Value;
            }
        }

        internal void ckbEnabled_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("Todo lol");
        }

        // Mostly for debugging
        public override string ToString()
        {
            return Name;
        }

    }

}
