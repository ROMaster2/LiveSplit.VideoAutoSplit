using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveSplit.VAS.Models;

namespace LiveSplit.UI.Components
{
    public partial class FeaturesUI : UserControl
    {
        private readonly VASComponent ParentComponent;

        public FeaturesUI(VASComponent parentComponent)
        {
            InitializeComponent();

            ParentComponent = parentComponent;
        }

        private void Rerender()
        {
            ParentComponent.Script.ScriptUpdateFinished += UpdateRows;
        }

        private void Derender()
        {
            ParentComponent.Script.ScriptUpdateFinished -= UpdateRows;
        }

        private void SetLabels()
        {

        }

        private void UpdateRows(object sender, DeltaManager dm)
        {

        }

        private void AddFeatureRow()
        {
            label1 = new Label();
        }

    }

    internal class FeatureRow : Control
    {
        //public Label Name { get; }
        public CheckBox CheckBox { get; }
        public Label Confidence { get; }
        public Label Maximum { get; }
        public Label Minumum { get; }

        public FeatureRow()
        {

        }
    }
}
